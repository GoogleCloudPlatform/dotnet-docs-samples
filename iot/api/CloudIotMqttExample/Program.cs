// Copyright (c) 2019 Google LLC.
//
// Licensed under the Apache License, Version 2.0 (the "License"); you may not
// use this file except in compliance with the License. You may obtain a copy of
// the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
// License for the specific language governing permissions and limitations under
// the License.

using CommandLine;
using Google.Apis.Auth.OAuth2;
using Google.Apis.CloudIot.v1;
using Google.Apis.CloudIot.v1.Data;
using Google.Apis.Services;
using NodaTime;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Exceptions;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace GoogleCloudSamples
{
    [Verb("startMqtt", HelpText = "Start a new mqtt device.")]
    class MqttExampleOptions
    {
        [Value(0, HelpText = "The project containing device registry.", Required = true)]
        public string projectId { get; set; }

        [Value(1, HelpText = "The region (e.g. us-central1) the registry is located in.", Required = true)]
        public string regionId { get; set; }

        [Value(2, HelpText = "The ID of the registry to create.", Required = true)]
        public string registryId { get; set; }

        [Value(3, HelpText = "Cloud IoT Core device id.", Required = true)]
        public string deviceId { get; set; }

        [Value(4, HelpText = "Path to private key file.", Required = true)]
        public string private_key_file { get; set; }

        [Value(5, HelpText = "Encryption algorithm to use to generate the JWT. Either 'RS256' or 'ES256'.", Required = true)]
        public string algorithm { get; set; }

        [Value(6, HelpText = "CA root from https://pki.google.com/roots.pem.", Required = true)]
        public string caCert { get; set; }

        [Value(7, HelpText = "Indicates whether the message to be published is a telemetry event or a device state message.", Required = true)]
        public string messageType { get; set; }

        [Option(HelpText = "MQTT bridge hostname.", Default = "mqtt.googleapis.com")]
        public string mqttBridgeHostname { get; set; }

        [Option(HelpText = "MQTT bridge port.", Default = 443)]
        public int mqttBridgePort { get; set; }

        [Option(HelpText = "Expiration time, in minutes, for JWT tokens.", Default = 60)]
        public int jwtExpiresMinutes { get; set; }

        [Option(HelpText = "Number of messages to publish.", Default = 20)]
        public int numMessages { get; set; }

        [Option(HelpText = "Wait time (in seconds) for commands.", Default = 120)]
        public int waitTime { get; set; }
    }

    [Verb("listenForConfigMessages", HelpText = "Listens for configuration messages on the gateway and bound devices.")]
    class ListenForConfigMessagesOptions
    {
        [Value(0, HelpText = "The project containing the device registry.", Required = true)]
        public string projectId { get; set; }

        [Value(1, HelpText = "The region (e.g. us-central1) the registry is located in.", Required = true)]
        public string regionId { get; set; }

        [Value(2, HelpText = "The registry containing the device.", Required = true)]
        public string registryId { get; set; }

        [Value(3, HelpText = "The gateway ID.")]
        public string gatewayId { get; set; }

        [Value(4, HelpText = "The device ID.", Required = true)]
        public string deviceId { get; set; }

        [Value(5, HelpText = "CA root from https://pki.google.com/roots.pem", Required = true, Default = "roots.pem")]
        public string ca_certs { get; set; }

        [Value(6, HelpText = "The private key file path.", Required = true)]
        public string privateKeyPath { get; set; }

        [Value(7, HelpText = "Which encryption algorithm to use to generate the JWT. (RS256 or ES256)", Required = true)]
        public string algorithm { get; set; }

        [Option(HelpText = "The number of messages wish to publish.", Default = 10)]
        public int numMsgs { get; set; }

        [Option(HelpText = "Expiration time (in minutes) for JWT tokens.", Default = 60)]
        public int jwtExpTime { get; set; }

        [Option(HelpText = "Duration(seconds) to listen for configuration messages.", Default = 60)]
        public int listenTime { get; set; }
    }

    [Verb("sendDataFromBoundDevice", HelpText = "Sends data from a gateway on behalf of a device that is bound to it.")]
    class SendDataFromBoundDeviceOptions
    {
        [Value(0, HelpText = "The project containing the device registry.", Required = true)]
        public string projectId { get; set; }

        [Value(1, HelpText = "The region (e.g. us-central1) the registry is located in.", Required = true)]
        public string regionId { get; set; }

        [Value(2, HelpText = "The registry containing the device.", Required = true)]
        public string registryId { get; set; }

        [Value(3, HelpText = "The device ID.", Required = true)]
        public string deviceId { get; set; }

        [Value(4, HelpText = "The gateway ID.", Required = true)]
        public string gatewayId { get; set; }

        [Value(5, HelpText = "The private key file path.", Required = true)]
        public string privateKeyPath { get; set; }

        [Value(6, HelpText = "Which encryption algorithm to use to generate the JWT. (RS256 or ES256)", Required = true)]
        public string algorithm { get; set; }

        [Value(7, HelpText = "CA root from https://pki.google.com/roots.pem", Required = true)]
        public string ca_certs { get; set; }

        [Value(8, HelpText = "Message type (events or state)", Required = true)]
        public string messageType { get; set; }

        [Value(9, HelpText = "The telemetry data sent on behalf of a device.", Required = true)]
        public string data { get; set; }

        [Option(HelpText = "Expiration time (in minutes) for JWT tokens.", Default = 60)]
        public int jwtExpTime { get; set; }
    }

    public class CloudIotMqttExample
    {
        /// <summary>
        /// Creates an authorized Cloud IoT Core Service client using Application
        /// Default Credentials.
        /// </summary>
        /// <returns>an authorized Cloud IoT Core Service client.</returns>
        // [START iot_create_auth_client]
        public static CloudIotService CreateAuthorizedClient()
        {
            GoogleCredential credential =
                GoogleCredential.GetApplicationDefaultAsync().Result;
            // Inject the Cloud IoT Core Service scope
            if (credential.IsCreateScopedRequired)
            {
                credential = credential.CreateScoped(new[]
                {
                    CloudIotService.Scope.CloudPlatform // Used for IoT + PubSub + IAM
                    //CloudIotService.Scope.Cloudiot // Can be used if not accessing Pub/Sub
                });
            }
            return new CloudIotService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                GZipEnabled = false
            });
        }
        // [END iot_create_auth_client]

        private static byte[] PEM(string type, byte[] data)
        {
            string pem = Encoding.ASCII.GetString(data);
            string header = String.Format("-----BEGIN {0}-----\n", type);
            string footer = String.Format("\n-----END {0}-----", type);
            int start = pem.IndexOf(header) + header.Length;
            int end = pem.IndexOf(footer, start);
            string base64 = pem.Substring(start, (end - start));
            return Convert.FromBase64String(base64);
        }

        public static object GetRegistries(string projectId, string cloudRegion)
        {
            var cloudIot = CreateAuthorizedClient();
            // The resource name of the location associated with the key rings.
            var parent = $"projects/{projectId}/locations/{cloudRegion}";
            var listOfRegistries = new List<DeviceRegistry>();
            try
            {
                var result = cloudIot.Projects.Locations.Registries.List(parent).Execute();
                listOfRegistries = result.DeviceRegistries.ToList();
                Console.WriteLine($"Number of registries: {listOfRegistries.Count}");
            }
            catch (Google.GoogleApiException e)
            {
                Console.WriteLine(e.Message);
                if (e.Error != null) return null;
                return null;
            }
            return listOfRegistries;
        }


        /// <summary>
        /// Creates a JWT using RSA encryption to be used in connecting the client.
        /// </summary>
        // [START iot_mqtt_jwt]
        public static string CreateJwtRsa(string projectId, string privateKeyFile)
        {
            string privateKey = File.ReadAllText(privateKeyFile);

            RSAParameters rsaParams;
            if (privateKey.IndexOf("-----BEGIN PRIVATE KEY-----") < 0)
            {
                throw new NotSupportedException("Invalid private key was used.");
            }

            // Read the private key file.
            using (var tr = new StringReader(privateKey))
            {
                var pemReader = new PemReader(tr);
                var KeyParameter = (AsymmetricKeyParameter)
                    pemReader.ReadObject();
                var privateRsaParams = KeyParameter
                    as RsaPrivateCrtKeyParameters;
                rsaParams = DotNetUtilities.ToRSAParameters(privateRsaParams);

                pemReader.Reader.Close();
                tr.Close();
            }
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                rsa.ImportParameters(rsaParams);
                var jwtIatTime = SystemClock.Instance.GetCurrentInstant();
                // Create a duration
                Duration durationExp = Duration.FromMinutes(120);
                var jwtExpTime = SystemClock.Instance.
                    GetCurrentInstant().Plus(durationExp);

                Dictionary<string, object> payload =
                    new Dictionary<string, object> {
                      {"iat", jwtIatTime.ToUnixTimeSeconds()},
                      // The time that the token was issued at
                      {"aud",projectId},
                        // The audience field should always
                        // be set to the GCP project id.
                      {"exp", jwtExpTime.ToUnixTimeSeconds()}
                    // The time the token expires.
                };
                Console.WriteLine("iat time {0}", jwtIatTime);
                Console.WriteLine("exp time {0}", jwtExpTime);
                Console.WriteLine("Creating JWT using RSA from private key " +
                    "file {0}", privateKeyFile);
                return Jose.JWT.Encode(payload, rsa, Jose.JwsAlgorithm.RS256);
            }
        }
        // [END iot_mqtt_jwt]

        /// <summary>
        /// Configures and returns the MQTT client for connecting to Google Cloud
        /// IoT Core.
        /// </summary>
        // [START iot_mqtt_config]
        public static MqttClient GetClient(string projectId, string cloudRegion,
            string registryId, string deviceId, string caCert, string mqttBridgeHostname,
            int mqttBridgePort)
        {
            // Enable SSL/TLS support.
            MqttSslProtocols mqttSslProtocols = MqttSslProtocols.TLSv1_2;

            byte[] data;
            using (FileStream fs = File.OpenRead(caCert))
            {
                data = new byte[fs.Length];
                fs.Read(data, 0, data.Length);
                if (data[0] != 0x30)
                {
                    // maybe it's ASCII PEM base64 encoded ?
                    data = PEM("CERTIFICATE", data);
                }
                fs.Close();
            }
            X509Certificate x509Certificate = new X509Certificate(data);

            // Create client instance, Connect to the Google MQTT bridge.
            MqttClient client = new MqttClient(mqttBridgeHostname, mqttBridgePort,
                true, x509Certificate, null, mqttSslProtocols, null)
            {
                ProtocolVersion = MqttProtocolVersion.Version_3_1_1
            };

            Console.WriteLine("Creating the client {0}", client);
            // register to message received
            client.ConnectionClosed += Client_ConnectionClosed;
            client.MqttMsgSubscribed += Client_MqttMsgSubscribed;
            client.MqttMsgPublishReceived += Client_MqttMsgPublishReceived;
            client.MqttMsgPublished += Client_MqttMsgPublished;

            return client;
        }

        public static object SetupMqttTopics(MqttClient client, string deviceId)
        {
            // The configuration topic is used for acknowledged changes.
            string mqttConfigTopic = $"/devices/{deviceId}/config";
            // The commands topic is used for frequent, transitory, updates.
            string mqttCommandTopic = $"/devices/{deviceId}/commands/#";
            string mqttErrorTopic = $"/devices/{deviceId}/errors";

            string[] topics = new string[] {
                mqttConfigTopic,
                mqttCommandTopic,
                mqttErrorTopic
            };

            byte[] qosLevels = new byte[] {
                MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE, // config topic, Qos *1*
                MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE, // command topic, Qos 0
                MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE // error topic, Qos 0
            };
            Console.WriteLine("Subscribing to {0}", mqttCommandTopic);
            client.Subscribe(topics, qosLevels);
            return 0;
        }

        public static void Client_ConnectionClosed(object sender, EventArgs e)
        {
            Console.WriteLine("On Disconnect: {0} {1}", e.GetType(), sender);
        }

        public static void Client_MqttMsgSubscribed(object sender,
            MqttMsgSubscribedEventArgs e)
        {
            Console.WriteLine("On Subscribe: {0} {1}", e.MessageId, sender);
        }

        public static void Client_MqttMsgPublished(object sender,
            MqttMsgPublishedEventArgs e)
        {
            Console.WriteLine("On Publish {0}", e.IsPublished);
        }

        // the event MqttMsgPublishReceived raised when a message is published
        // on a topic the client is subscribed to.
        public static void Client_MqttMsgPublishReceived(object sender,
            MqttMsgPublishEventArgs e)
        {
            // handle message received
            var output = $"Received { Encoding.UTF8.GetString(e.Message)}" +
                $" on topic {e.Topic} with Qos {e.QosLevel}";
            Console.WriteLine(output);
        }
        // [END iot_mqtt_config]


        // [START iot_mqtt_run]
        public static object StartMqtt(string projectId, string cloudRegion,
            string registryId, string deviceId, string privateKeyFile,
            string algorithm, string caCert, int numMsgs = 20,
            string messageType = "events",
            string mqttBridgeHostName = "mqtt.googleapis.com",
            int mqttBridgePort = 8883, int jwtExpiresMin = 20,
            int waitTime = 10)
        {
            MqttClient client = GetClient(projectId,
                cloudRegion,
                registryId,
                deviceId,
                caCert,
                mqttBridgeHostName,
                mqttBridgePort);

            // Create our MQTT client. The client_id is a unique string
            // that identifies this device.For Google Cloud IoT Core,
            // it must be in the format below.
            var clientId = $"projects/{projectId}" +
                $"/locations/{cloudRegion}" +
                $"/registries/{registryId}" +
                $"/devices/{deviceId}";

            // With Google Cloud IoT Core, the username field is ignored,
            // and the password field is used to transmit a JWT to authorize
            // the device.
            DateTime iat = DateTime.Now;
            var pass = CreateJwtRsa(projectId, privateKeyFile);

            double initialConnectIntervalMillis = 0.5;
            double maxConnectIntervalMillis = 6;
            double maxConnectRetryTimeElapsedMillis = 900;
            double intervalMultiplier = 1.5;

            double retryIntervalMs = initialConnectIntervalMillis;
            double totalRetryTimeMs = 0;

            // Both connect and publish operations may fail. If they do,
            // allow retries but with an exponential backoff time period.
            while (!client.IsConnected &&
                totalRetryTimeMs < maxConnectRetryTimeElapsedMillis)
            {
                try
                {
                    // Connect to the Google MQTT bridge.
                    client.Connect(clientId, "unused", pass);
                }
                catch (AggregateException aggExceps)
                {
                    printExceptions(aggExceps);
                    Console.WriteLine("Retrying in " + retryIntervalMs
                        + " seconds.");

                    System.Threading.Thread.Sleep((int)retryIntervalMs);
                    totalRetryTimeMs += retryIntervalMs;
                    retryIntervalMs *= intervalMultiplier;
                    if (retryIntervalMs > maxConnectIntervalMillis)
                    {
                        retryIntervalMs = maxConnectIntervalMillis;
                    }
                }
            }

            // Publish number of messages and wait for given seconds.
            client = publishMsgsAndWait(client, messageType, deviceId,
                numMsgs, registryId, iat, jwtExpiresMin, algorithm,
                pass, projectId, privateKeyFile, clientId, waitTime);

            // Disconnect the client if still connected, and finish the run.
            if (client.IsConnected)
            {
                client.Disconnect();
            }
            Console.WriteLine("Finished loop successfully. Goodbye!");
            return 0;
        }
        // [END iot_mqtt_run]

        // [START iot_attach_device]
        public static object AttachDevice(MqttClient client, string deviceId, string auth)
        {
            var attachTopic = $"/devices/{deviceId}/attach";
            Console.WriteLine("Attaching: {0}", attachTopic);
            var BinaryData = Encoding.UTF8.GetBytes(auth);
            client.Publish(attachTopic, BinaryData, MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE, true);

            Console.WriteLine("Waiting for device to attach.");
            return 0;
        }
        // [END iot_attach_device]

        // [START iot_detach_device]
        public static object DetachDevice(MqttClient client, string deviceId)
        {
            var detachTopic = $"/devices/{deviceId}/detach";
            Console.WriteLine("Detaching: {0}", detachTopic);
            var BinaryData = Encoding.UTF8.GetBytes("{}");
            client.Publish(detachTopic, BinaryData, MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE, true);
            return 0;
        }
        // [END iot_detach_device]

        /// <summary>
        /// Listens for configuration and system error messages on the gateway and
        /// bound devices.
        /// </summary>
        // [START iot_listen_for_config_messages]
        public static object ListenForConfigMessages(string projectId, string cloudRegion,
            string registryId, string deviceId, string gatewayId, int numMessages,
            string privateKeyFile, string algorithm, string caCerts, string mqttBridgeHostname,
            int mqttBridgePort, int jwtExpiresMinutes, int duration)
        {
            var clientId = $"projects/{projectId}/locations/{cloudRegion}/registries/{registryId}" +
                $"/devices/{gatewayId}";
            var jwtIatTime = SystemClock.Instance.GetCurrentInstant();
            // Create a duration
            Duration durationExp = Duration.FromMinutes(jwtExpiresMinutes);
            var jwtExpTime = SystemClock.Instance.GetCurrentInstant().Plus(durationExp);
            var pass = "";
            if (algorithm == "RS256")
            {
                pass = CloudIotMqttExample.CreateJwtRsa(projectId, privateKeyFile);
            }
            else if (algorithm == "ES256")
            {
                Console.WriteLine("Currently, we do not support this algorithm.");
                return 0;
            }

            // Use gateway to connect server
            var mqttClient = GetClient(
              projectId,
              cloudRegion,
              registryId,
              gatewayId,
              caCerts,
              mqttBridgeHostname,
              mqttBridgePort);

            double initialConnectIntervalMillis = 0.5;
            double maxConnectIntervalMillis = 6;
            double maxConnectRetryTimeElapsedMillis = 900;
            double intervalMultiplier = 1.5;

            double retryIntervalMs = initialConnectIntervalMillis;
            double totalRetryTimeMs = 0;

            // Both connect and publish operations may fail. If they do,
            // allow retries but with an exponential backoff time period.
            while (!mqttClient.IsConnected &&
                totalRetryTimeMs < maxConnectRetryTimeElapsedMillis)
            {
                try
                {
                    // Connect to the Google MQTT bridge.
                    mqttClient.Connect(clientId, "unused", pass);
                }
                catch (AggregateException aggExceps)
                {
                    printExceptions(aggExceps);
                    Console.WriteLine("Retrying in " + retryIntervalMs
                        + " seconds.");

                    System.Threading.Thread.Sleep((int)retryIntervalMs);
                    totalRetryTimeMs += retryIntervalMs;
                    retryIntervalMs *= intervalMultiplier;
                    if (retryIntervalMs > maxConnectIntervalMillis)
                    {
                        retryIntervalMs = maxConnectIntervalMillis;
                    }
                }
            }

            SetupMqttTopics(mqttClient, gatewayId);
            AttachDevice(mqttClient, deviceId, "{}");

            // Wait for about a minute for config messages.
            Console.WriteLine("Listening...");
            for (int i = 0; i < duration; ++i)
            {
                Console.Write(".");
                var secSinceIssue = SystemClock.Instance.GetCurrentInstant().Minus(jwtIatTime);
                if (secSinceIssue.TotalSeconds > (60 * jwtExpiresMinutes))
                {
                    Console.WriteLine("Refreshing token after {0}s", secSinceIssue);
                    jwtIatTime = SystemClock.Instance.GetCurrentInstant();
                    // refresh token and reconnect.
                    pass = CloudIotMqttExample.CreateJwtRsa(projectId, privateKeyFile);
                    mqttClient = GetClient(
                                projectId,
                                cloudRegion,
                                registryId,
                                gatewayId,
                                caCerts,
                                mqttBridgeHostname,
                                mqttBridgePort);
                }
                System.Threading.Thread.Sleep(1000);
            }

            DetachDevice(mqttClient, deviceId);
            // wait for the device get detached.
            System.Threading.Thread.Sleep(2000);
            mqttClient.Disconnect();
            Console.WriteLine("Finished.");
            return 0;
        }
        // [END iot_listen_for_config_messages]

        // [START iot_send_data_from_bound_device]
        public static object SendDataFromBoundDevice(string projectId, string cloudRegion,
            string registryId, string deviceId, string gatewayId, string privateKeyFile,
            string algorithm, string caCerts, string mqttBridgeHostname, int mqttBridgePort,
            int jwtExpiresMinutes, string messageType, string payload)
        {
            // Publish device events and gateway state.
            string device_topic = $"/devices/{deviceId}/state";
            string gateway_topic = $"/devices/{gatewayId}/state";

            var jwtIatTime = SystemClock.Instance.GetCurrentInstant();
            Duration durationExp = Duration.FromMinutes(jwtExpiresMinutes);
            var jwtExpTime = SystemClock.Instance.GetCurrentInstant().Plus(durationExp);

            // Use gateway to connect to server.
            var password = CreateJwtRsa(projectId, privateKeyFile);
            var mqttClient = GetClient(projectId, cloudRegion,
                registryId, gatewayId, caCerts,
               mqttBridgeHostname, mqttBridgePort);

            var clientId = $"projects/{projectId}/locations/{cloudRegion}/registries/{registryId}" +
                $"/devices/{gatewayId}";
            mqttClient.Connect(clientId, "unused", password);
            SetupMqttTopics(mqttClient, gatewayId);

            AttachDevice(mqttClient, deviceId, "{}");
            System.Threading.Thread.Sleep(1000);
            // Publish numMsgs messages to the MQTT bridge.
            SendDataFromDevice(mqttClient, deviceId, messageType, payload);

            DetachDevice(mqttClient, deviceId);
            mqttClient.Disconnect();
            return 0;
        }
        // [END iot_send_data_from_bound_device]

        public static object SendDataFromDevice(MqttClient client, string deviceId,
            string messageType, string data)
        {
            if (!messageType.Equals("events") && !messageType.Equals("state"))
            {
                Console.WriteLine("Invalid message type, must ether be 'state' or events'");
                return 0;
            }
            string dataTopic = $"/devices/{deviceId}/{messageType}";

            // Publish state to gateway topic.
            client.Publish(dataTopic, Encoding.UTF8.GetBytes(data),
                MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE, true);
            Console.WriteLine("Data sent: {0}", data);

            return 0;
        }

        public static void printExceptions(AggregateException exceps)
        {
            exceps.Handle((ex) =>
                {
                    if (ex is MqttClientException)
                    {
                        Console.WriteLine("Client Exception"
                        + ex.InnerException.Message);
                    }
                    else if (ex is MqttCommunicationException)
                    {
                        if (ex.InnerException != null)
                        {
                            Console.WriteLine("An error occured {0}",
                                ex.InnerException.Message);
                        }
                        else
                        {
                            Console.WriteLine("An error occured {0}", ex.Message);
                        }
                    }
                    return false;
                }
            );
        }

        public static MqttClient publishMsgsAndWait(MqttClient client,
            string messageType, string deviceId, int numMsgs,
            string registryId, DateTime iat, int jwtExpiresMin,
            string algorithm, string pass, string projectId,
            string privateKeyFile, string clientId, int waitTime)
        {
            // Publish to the events or state topic based on the flag.
            string sub_topic = messageType == "event" ? "events" : "state";

            // The MQTT topic that this device will publish telemetry data to.
            // The MQTT topic name is required to be in the format below.
            // Note that this is not the same as the device registry's
            // Cloud Pub/Sub topic.
            string mqttTopic = $"/devices/{deviceId}/{sub_topic}";

            SetupMqttTopics(client, deviceId);

            //Publish num_messages mesages to the MQTT bridge once per second.
            for (var i = 1; i <= numMsgs; ++i)
            {
                string payload = string.Format("{0}/{1}-payload-{2}",
                    registryId, deviceId, i);
                Console.WriteLine("Publishing {0} message {1}/{2}: '{3}'\n"
                    , messageType, i, numMsgs, payload);
                var BinaryData = Encoding.Unicode.GetBytes(payload);

                // Refresh the connection credentials before the JWT expires.
                long secsSinceRefresh =
                    (DateTime.Now.Millisecond - iat.Millisecond) / 1000;
                if (secsSinceRefresh > 60 * jwtExpiresMin)
                {
                    Console.WriteLine("\tRefreshing token after: {0} seconds\n"
                        , secsSinceRefresh);
                    iat = DateTime.Now;
                    if (algorithm == "RS256")
                    {
                        pass = CreateJwtRsa(projectId, privateKeyFile);
                    }
                    else if (algorithm == "ES256")
                    {
                        //TODO: needs to be implemented
                    }
                    else
                    {
                        throw new ArgumentException("Invalid algorithm {0}. " +
                            "Should be one of 'RS256' or 'ES256'", algorithm);
                    }
                    client.Disconnect();
                    client.Connect(clientId, "unused", pass);
                }

                // Publish "payload" to the MQTT topic. qos=1 means at least
                // once delivery. Cloud IoT Core also supports qos=0 for at
                // most once delivery.
                client.Publish(mqttTopic, BinaryData,
                    MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE, false);

                if (messageType.Equals("event"))
                {
                    // Send telemetry events every second
                    System.Threading.Thread.Sleep(1000);
                }
                else
                {
                    // Note: Update Device state less frequently than with
                    // telemetry events
                    System.Threading.Thread.Sleep(5000);
                }
            }

            for (int i = 1; i < waitTime; i++)
            {
                Console.Write(".");
                System.Threading.Thread.Sleep(1000);
            }
            Console.WriteLine();
            return client;
        }

        public static int Main(string[] args)
        {
            var verbMap = new VerbMap<object>();
            verbMap
                .Add((ListenForConfigMessagesOptions opts) => ListenForConfigMessages(
                    opts.projectId, opts.regionId, opts.registryId, opts.deviceId, opts.gatewayId,
                    opts.numMsgs, opts.privateKeyPath, opts.algorithm, opts.ca_certs,
                    "mqtt.googleapis.com", 8883, opts.jwtExpTime, opts.listenTime))
                .Add((SendDataFromBoundDeviceOptions opts) => SendDataFromBoundDevice(
                    opts.projectId, opts.regionId, opts.registryId, opts.deviceId, opts.gatewayId,
                    opts.privateKeyPath, opts.algorithm, opts.ca_certs, "mqtt.googleapis.com",
                    8883, opts.jwtExpTime, opts.messageType, opts.data))
                .Add((MqttExampleOptions opts) => StartMqtt(
                    opts.projectId, opts.regionId, opts.registryId,
                    opts.deviceId, opts.private_key_file, opts.algorithm,
                    opts.caCert, opts.numMessages, opts.messageType,
                    opts.mqttBridgeHostname, opts.mqttBridgePort,
                    opts.jwtExpiresMinutes, opts.waitTime))
                .NotParsedFunc = (err) => 1;
            return (int)verbMap.Run(args);
        }
    }
}
