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

using System;
using uPLibrary.Networking.M2Mqtt;
using System.Security.Cryptography;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Crypto;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography.X509Certificates;
using CommandLine;
using uPLibrary.Networking.M2Mqtt.Messages;
using uPLibrary.Networking.M2Mqtt.Exceptions;
using NodaTime;
using Google.Apis.CloudIot.v1;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.CloudIot.v1.Data;

namespace GoogleCloudSamples
{
    [Verb("createRegistry", HelpText = "Create a new Device registry.")]
    class CreateRegistryOptions
    {
        [Value(0, HelpText = "The project containing device registry.", Required = true)]
        public string projectId { get; set; }

        [Value(1, HelpText = "The region (e.g. us-central1) the registry is located in.", Required = true)]
        public string regionId { get; set; }

        [Value(2, HelpText = "The ID of the registry to create.", Required = true)]
        public string registryId { get; set; }

        [Value(3, HelpText = "The pubsub topic to associate to the registry.", Required = true)]
        public string pubsubTopic { get; set; }
    }

    [Verb("createDeviceRsa", HelpText = "Create device with RSA encryption type.")]
    class CreateRsaDeviceOptions
    {
        [Value(0, HelpText = "The project containing device registry.", Required = true)]
        public string projectId { get; set; }

        [Value(1, HelpText = "The region (e.g. us-central1) the registry is located in.", Required = true)]
        public string regionId { get; set; }

        [Value(2, HelpText = "The registry to create the device in.", Required = true)]
        public string registryId { get; set; }

        [Value(3, HelpText = "The ID of the device to create.", Required = true)]
        public string deviceId { get; set; }

        [Value(4, HelpText = "The path to the RSA **public** key.")]
        public string certificiatePath { get; set; }
    }

    [Verb("deleteRegistry", HelpText = "Delete a registry.")]
    class DeleteRegistryOptions
    {
        [Value(0, HelpText = "The project containing device registry.", Required = true)]
        public string projectId { get; set; }

        [Value(1, HelpText = "The region (e.g. us-central1) the registry is located in.", Required = true)]
        public string regionId { get; set; }

        [Value(2, HelpText = "The registry to delete.", Required = true)]
        public string registryId { get; set; }
    }

    [Verb("deleteDevice", HelpText = "Delete a device from a registry.")]
    class DeleteDeviceOptions
    {
        [Value(0, HelpText = "The project containing device registry.", Required = true)]
        public string projectId { get; set; }

        [Value(1, HelpText = "The region (e.g. us-central1) the registry is located in.", Required = true)]
        public string regionId { get; set; }

        [Value(2, HelpText = "The registry to get from.", Required = true)]
        public string registryId { get; set; }

        [Value(3, HelpText = "The ID of the device to remove.", Required = true)]
        public string deviceId { get; set; }
    }


    [Verb("sendCommand", HelpText = "Send a command to a device.")]
    class SendCommandOptions
    {
        [Value(0, HelpText = "The device ID.", Required = true)]
        public string deviceId { get; set; }

        [Value(1, HelpText = "The project containing the device registry.", Required = true)]
        public string projectId { get; set; }

        [Value(2, HelpText = "The region (e.g. us-central1) the registry is located in.", Required = true)]
        public string regionId { get; set; }

        [Value(3, HelpText = "The registry containing the device.", Required = true)]
        public string registryId { get; set; }

        [Value(4, HelpText = "The command sent to the device", Required = true)]
        public string command { get; set; }
    }

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

        [Value(7, HelpText = "Number of messages to publish.")]
        public int numMessages { get; set; }

        [Value(8, HelpText = "Indicates whether the message to be published is a telemetry event or a device state message.")]
        public string messageType { get; set; }

        [Value(9, HelpText = "MQTT bridge hostname.", Default = "mqtt.googleapis.com")]
        public string mqttBridgeHostname { get; set; }

        [Value(10, HelpText = "MQTT bridge port.", Default = 8883)]
        public int mqttBridgePort { get; set; }

        [Value(11, HelpText = "Expiration time, in minutes, for JWT tokens.")]
        public int jwtExpiresMinutes { get; set; }

        [Option(HelpText = "Wait time (in seconds) for commands.", Default = 120)]
        public int waitTime { get; set; }
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

        // [START iot_create_rsa_device]
        public static object CreateRsaDevice(string projectId, string cloudRegion,
            string registryId, string deviceId, string keyPath)
        {
            var cloudIot = CreateAuthorizedClient();
            var parent = $"projects/{projectId}" +
                $"/locations/{cloudRegion}" +
                $"/registries/{registryId}";

            try
            {
                String keyText = File.ReadAllText(keyPath);
                Device body = new Device()
                {
                    Id = deviceId
                };
                body.Credentials = new List<DeviceCredential>();
                body.Credentials.Add(new DeviceCredential()
                {
                    PublicKey = new PublicKeyCredential()
                    {
                        Key = keyText,
                        Format = "RSA_X509_PEM"
                    },
                });

                var device = cloudIot
                    .Projects
                    .Locations
                    .Registries
                    .Devices
                    .Create(body, parent)
                    .Execute();
                Console.WriteLine("Device created: ");
                Console.WriteLine($"{device.Id}");
                Console.WriteLine($"\tBlocked: {device.Blocked == true}");
                Console.WriteLine($"\tConfig version: {device.Config.Version}");
                Console.WriteLine($"\tName: {device.Name}");
                Console.WriteLine($"\tState:{device.State}");
            }
            catch (Google.GoogleApiException e)
            {
                Console.WriteLine(e.Message);
                if (e.Error != null) return e.Error.Code;
                return -1;
            }
            return 0;
        }
        // [END iot_create_rsa_device]

        // [START iot_delete_registry]
        public static object DeleteRegistry(string projectId, string cloudRegion,
            string registryId)
        {
            var cloudIot = CreateAuthorizedClient();
            // The resource name of the location associated with the key rings.
            var name = $"projects/{projectId}" +
                $"/locations/{cloudRegion}" +
                $"/registries/{registryId}";

            try
            {
                var registry = cloudIot
                    .Projects
                    .Locations
                    .Registries
                    .Delete(name)
                    .Execute();
            }
            catch (Google.GoogleApiException e)
            {
                Console.WriteLine(e.Message);
                if (e.Error != null) return e.Error.Code;
                return -1;
            }
            return 0;
        }
        // [END iot_delete_registry]


        // [START iot_delete_device]
        public static object DeleteDevice(string projectId, string cloudRegion,
            string registryId, string deviceId)
        {
            var cloudIot = CreateAuthorizedClient();
            // The resource name of the location associated with the key rings.
            var name = $"projects/{projectId}" +
                $"/locations/{cloudRegion}" +
                $"/registries/{registryId}" +
                $"/devices/{deviceId}";

            try
            {
                var res = cloudIot
                    .Projects
                    .Locations
                    .Registries
                    .Devices
                    .Delete(name)
                    .Execute();
                Console.WriteLine($"Removed device: {deviceId}");
            }
            catch (Google.GoogleApiException e)
            {
                Console.WriteLine(e.Message);
                if (e.Error != null) return e.Error.Code;
                return -1;
            }
            return 0;
        }
        // [END iot_delete_device]


        //[START iot_send_command]  
        public static object SendCommand(string deviceId, string projectId,
            string cloudRegion, string registryName, string data)
        {
            var cloudIot = CreateAuthorizedClient();

            var devicePath = String.Format("projects/{0}/" +
                "locations/{1}/" +
                "registries/{2}/" +
                "devices/{3}",
                projectId, cloudRegion, registryName, deviceId);
            // Data sent through the wire has to be base64 encoded.
            SendCommandToDeviceRequest req = new SendCommandToDeviceRequest()
            {
                BinaryData = Convert.ToBase64String(
                    Encoding.UTF8.GetBytes(data))
            };

            Console.WriteLine("Sending command to {0}\n", devicePath);

            var res =
                cloudIot
                .Projects
                .Locations
                .Registries
                .Devices
                .SendCommandToDevice(req, devicePath)
                .Execute();

            Console.WriteLine("Command response: " + res.ToString());
            return 0;
        }
        //[END iot_send_command]

        // [START iot_create_registry]
        public static object CreateRegistry(string projectId, string cloudRegion,
            string registryId, string pubsubTopic)
        {
            var cloudIot = CreateAuthorizedClient();
            // The resource name of the location associated with the key rings.
            var parent = $"projects/{projectId}/locations/{cloudRegion}";

            Console.WriteLine(parent);

            try
            {
                Console.WriteLine($"Creating {registryId}");
                DeviceRegistry body = new DeviceRegistry()
                {
                    Id = registryId,
                };
                body.EventNotificationConfigs =
                    new List<EventNotificationConfig>();
                var toAdd = new EventNotificationConfig()
                {
                    PubsubTopicName = pubsubTopic.StartsWith("projects/") ?
                        pubsubTopic : $"projects/{projectId}" +
                        $"/topics/{pubsubTopic}",
                };
                body.EventNotificationConfigs.Add(toAdd);
                var registry = cloudIot
                    .Projects
                    .Locations
                    .Registries
                    .Create(body, parent)
                    .Execute();
                Console.WriteLine("Registry: ");
                Console.WriteLine($"{registry.Id}");
                Console.WriteLine($"\tName: {registry.Name}");
                Console.WriteLine($"\tHTTP Enabled: " +
                    $"{registry.HttpConfig.HttpEnabledState}");
                Console.WriteLine($"\tMQTT Enabled: " +
                    $"{registry.MqttConfig.MqttEnabledState}");
            }
            catch (Google.GoogleApiException e)
            {
                Console.WriteLine(e.Message);
                if (e.Error != null) return e.Error.Code;
                return -1;
            }
            return 0;
        }
        // [END iot_create_registry]

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

        // [START iot_mqtt_jwt]
        public static string CreateJwtRsa(string projectId, string privateKeyFile)
        {
            string privateKey = File.ReadAllText(privateKeyFile);

            RSAParameters rsaParams;

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
        //[END iot_mqtt_jwt]

        // [START iot_mqtt_client]
        public static MqttClient GetClient(string projectId, string cloudRegion,
            string registryId, string deviceId, string privateKeyFile,
            string algorithm, string caCert, string mqttBridgeHostname,
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
        // [END iot_mqtt_client]

        public static object SetupMqttTopics(MqttClient client, string deviceId)
        {
            // This is the topic that the device will receive configuration updates on.
            string mqttConfigTopic = $"/devices/{deviceId}/config";
            // The topic that the device will receive commands on.
            string mqttCommandTopic = $"/devices/{deviceId}/commands/#";
            string mqttErrorTopic = $"/devices/{deviceId}/errors";

            string[] topics = new string[] {
                mqttConfigTopic,
                mqttCommandTopic,
                mqttErrorTopic
            };

            byte[] qosLevels = new byte[] {
                MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE , 
                // config topic, QoS 1 enables message acknowledgement.
                MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE , 
                // command topic
                MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE  
                // error topic
            };
            // Subscribe to the config topic, and command topic.
            Console.WriteLine("Subscribing to {0}", mqttCommandTopic);
            client.Subscribe(topics, qosLevels);
            return 0;
        }

        // [START iot_mqtt_event_handlers]
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
        // [END iot_mqtt_event_handlers]

        // [START iot_mqtt_example]
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
                privateKeyFile,
                algorithm,
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
        // [END iot_mqtt_example]


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
                .Add((CreateRegistryOptions opts) => CreateRegistry(
                        opts.projectId, opts.regionId,
                        opts.registryId, opts.pubsubTopic))
                .Add((CreateRsaDeviceOptions opts) => CreateRsaDevice(
                    opts.projectId, opts.regionId, opts.registryId,
                    opts.deviceId, opts.certificiatePath))
                .Add((DeleteRegistryOptions opts) => DeleteRegistry(
                    opts.projectId, opts.regionId, opts.registryId))
                .Add((DeleteDeviceOptions opts) => DeleteDevice(
                    opts.projectId, opts.regionId, opts.registryId, opts.deviceId))
                .Add((SendCommandOptions opts) => SendCommand(
                    opts.deviceId, opts.projectId, opts.regionId,
                    opts.registryId, opts.command))
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
