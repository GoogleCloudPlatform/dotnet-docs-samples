/*
 * Copyright (c) 2018 Google Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not
 * use this file except in compliance with the License. You may obtain a copy of
 * the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
 * License for the specific language governing permissions and limitations under
 * the License.
 */

using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using System;
using CommandLine;
using Google.Apis.CloudIot.v1;
using Google.Apis.CloudIot.v1.Data;
using System.Linq;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;
using NodaTime;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using uPLibrary.Networking.M2Mqtt.Exceptions;

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

    [Verb("getRegistry", HelpText = "Retrieve a registry.")]
    class GetRegistryOptions
    {
        [Value(0, HelpText = "The project containing device registry.", Required = true)]
        public string projectId { get; set; }

        [Value(1, HelpText = "The region (e.g. us-central1) the registry is located in.", Required = true)]
        public string regionId { get; set; }

        [Value(2, HelpText = "The registry to retrieve.", Required = true)]
        public string registryId { get; set; }
    }

    [Verb("listRegistries", HelpText = "List the registry IDs.")]
    class ListRegistryOptions
    {
        [Value(0, HelpText = "The project containing device registry.", Required = true)]
        public string projectId { get; set; }

        [Value(1, HelpText = "The region (e.g. us-central1) the registry is located in.", Required = true)]
        public string regionId { get; set; }
    }

    class DeviceOptions
    {
        [Value(0, HelpText = "The project containing device registry.", Required = true)]
        public string projectId { get; set; }

        [Value(1, HelpText = "The region (e.g. us-central1) the registry is located in.", Required = true)]
        public string regionId { get; set; }

        [Value(2, HelpText = "The registry to use.", Required = true)]
        public string registryId { get; set; }
    }

    [Verb("createDeviceNoAuth", HelpText = "Create a device without associated credentials.")]
    class CreateDeviceOptions
    {
        [Value(0, HelpText = "The project containing device registry.", Required = true)]
        public string projectId { get; set; }

        [Value(1, HelpText = "The region (e.g. us-central1) the registry is located in.", Required = true)]
        public string regionId { get; set; }

        [Value(2, HelpText = "The registry to get from.", Required = true)]
        public string registryId { get; set; }

        [Value(3, HelpText = "The ID of the device to create.", Required = true)]
        public string deviceId { get; set; }
    }

    [Verb("createDeviceEs", HelpText = "Create device with ES encryption type.")]
    class CreateEsDeviceOptions
    {
        [Value(0, HelpText = "The project containing device registry.", Required = true)]
        public string projectId { get; set; }

        [Value(1, HelpText = "The region (e.g. us-central1) the registry is located in.", Required = true)]
        public string regionId { get; set; }

        [Value(2, HelpText = "The registry to create the device in.", Required = true)]
        public string registryId { get; set; }

        [Value(3, HelpText = "The ID of the device to create.", Required = true)]
        public string deviceId { get; set; }

        [Value(4, HelpText = "The path to the ES **public** key.")]
        public string certificiatePath { get; set; }
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

    [Verb("getDevice", HelpText = "Retrieve information for a specific device.")]
    class GetDeviceOptions
    {
        [Value(0, HelpText = "The project containing device registry.", Required = true)]
        public string projectId { get; set; }

        [Value(1, HelpText = "The region (e.g. us-central1) the registry is located in.", Required = true)]
        public string regionId { get; set; }

        [Value(2, HelpText = "The registry to get from.", Required = true)]
        public string registryId { get; set; }

        [Value(3, HelpText = "The ID of the device to retrive.", Required = true)]
        public string deviceId { get; set; }
    }

    [Verb("getDeviceConfigs", HelpText = "Retrieve configurations for a specific device.")]
    class GetDeviceConfigsOptions
    {
        [Value(0, HelpText = "The project containing device registry.", Required = true)]
        public string projectId { get; set; }

        [Value(1, HelpText = "The region (e.g. us-central1) the registry is located in.", Required = true)]
        public string regionId { get; set; }

        [Value(2, HelpText = "The registry to get configurations from.", Required = true)]
        public string registryId { get; set; }

        [Value(3, HelpText = "The ID of the device used to retrive the configurations.", Required = true)]
        public string deviceId { get; set; }
    }

    [Verb("getDeviceStates", HelpText = "Retrieve states for a specific device.")]
    class GetDeviceStatesOptions
    {
        [Value(0, HelpText = "The project containing device registry.", Required = true)]
        public string projectId { get; set; }

        [Value(1, HelpText = "The region (e.g. us-central1) the device is located in.", Required = true)]
        public string regionId { get; set; }

        [Value(2, HelpText = "The registry containing the device.", Required = true)]
        public string registryId { get; set; }

        [Value(3, HelpText = "The ID of the device used to retrive the states.", Required = true)]
        public string deviceId { get; set; }
    }

    [Verb("listDevices", HelpText = "List devices in the provided Cloud IoT Core Registry.")]
    class ListDevicesOptions : DeviceOptions { }

    [Verb("getIamPolicy", HelpText = "Get the IAM policy for a device registry.")]
    class GetIamPolicyOptions : DeviceOptions { }

    [Verb("patchDeviceEs", HelpText = "Patch device with ES encryption type.")]
    class PatchEsDeviceOptions
    {
        [Value(0, HelpText = "The project containing device registry.", Required = true)]
        public string projectId { get; set; }

        [Value(1, HelpText = "The region (e.g. us-central1) the registry is located in.", Required = true)]
        public string regionId { get; set; }

        [Value(2, HelpText = "The registry to patch the device in.", Required = true)]
        public string registryId { get; set; }

        [Value(3, HelpText = "The ID of the device to patch.", Required = true)]
        public string deviceId { get; set; }

        [Value(4, HelpText = "The path to the ES **public** key.")]
        public string certificiatePath { get; set; }
    }

    [Verb("patchDeviceRsa", HelpText = "Create device with RSA encryption type.")]
    class PatchRsaDeviceOptions
    {
        [Value(0, HelpText = "The project containing device registry.", Required = true)]
        public string projectId { get; set; }

        [Value(1, HelpText = "The region (e.g. us-central1) the registry is located in.", Required = true)]
        public string regionId { get; set; }

        [Value(2, HelpText = "The registry to patch the device in.", Required = true)]
        public string registryId { get; set; }

        [Value(3, HelpText = "The ID of the device to patch.", Required = true)]
        public string deviceId { get; set; }

        [Value(4, HelpText = "The path to the RSA **public** key.")]
        public string certificiatePath { get; set; }
    }

    [Verb("setDeviceConfig", HelpText = "Set the configuration data for a device.")]
    class SetDeviceConfigOptions
    {
        [Value(0, HelpText = "The project containing the registry.", Required = true)]
        public string projectId { get; set; }

        [Value(1, HelpText = "The region (e.g. us-central1) the registry is located in.", Required = true)]
        public string regionId { get; set; }

        [Value(2, HelpText = "The registry containing the device.", Required = true)]
        public string registryId { get; set; }

        [Value(3, HelpText = "The device ID.", Required = true)]
        public string deviceId { get; set; }

        [Value(4, HelpText = "The data representing the device configuration.", Required = true)]
        public string data { get; set; }
    }

    [Verb("setIamPolicy", HelpText = "Set the IAM policy for a device registry.")]
    class SetIamPolicyOptions
    {
        [Value(0, HelpText = "The project containing the device registry.", Required = true)]
        public string projectId { get; set; }

        [Value(1, HelpText = "The region (e.g. us-central1) the registry is located in.", Required = true)]
        public string regionId { get; set; }

        [Value(2, HelpText = "The registry to set the policy on.", Required = true)]
        public string registryId { get; set; }

        [Value(3, HelpText = "The role to add the member as.", Required = true)]
        public string role { get; set; }

        [Value(4, HelpText = "The member to set the policy to.", Required = true)]
        public string member { get; set; }
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

    [Verb("createGateway", HelpText = "Create a gateway to bind devices to.")]
    class CreateGatewayOptions
    {
        [Value(0, HelpText = "The project containing the device registry.", Required = true)]
        public string projectId { get; set; }

        [Value(1, HelpText = "The region (e.g. us-central1) the registry is located in.", Required = true)]
        public string regionId { get; set; }

        [Value(2, HelpText = "The registry containing the device.", Required = true)]
        public string registryId { get; set; }

        [Value(3, HelpText = "The gateway ID that will be created.", Required = true)]
        public string gatewayId { get; set; }

        [Value(4, HelpText = "Public key file used for registering devices and gateways.", Required = true)]
        public string publicKeyFile { get; set; }

        [Value(5, HelpText = "Algorithm.", Required = true)]
        public string algorithm { get; set; }
    }

    [Verb("listGateways", HelpText = "List gateways in a registry.")]
    class ListGatewaysOptions
    {
        [Value(0, HelpText = "The project containing the device registry.", Required = true)]
        public string projectId { get; set; }

        [Value(1, HelpText = "The region (e.g. us-central1) the registry is located in.", Required = true)]
        public string regionId { get; set; }

        [Value(2, HelpText = "The registry containing the device.", Required = true)]
        public string registryId { get; set; }
    }

    [Verb("listDevicesForGateway", HelpText = "List devices bound to a gateway.")]
    class ListDevicesForGatewayOptions
    {
        [Value(0, HelpText = "The project containing the device registry.", Required = true)]
        public string projectId { get; set; }

        [Value(1, HelpText = "The region (e.g. us-central1) the registry is located in.", Required = true)]
        public string regionId { get; set; }

        [Value(2, HelpText = "The registry containing the device.", Required = true)]
        public string registryId { get; set; }

        [Value(3, HelpText = "The gateway ID that contains the devices.", Required = true)]
        public string gatewayId { get; set; }
    }

    [Verb("bindDeviceToGateway", HelpText = "Binds a device to a gateway.")]
    class BindDeviceToGatewayOptions
    {
        [Value(0, HelpText = "The project containing the device registry.", Required = true)]
        public string projectId { get; set; }

        [Value(1, HelpText = "The region (e.g. us-central1) the registry is located in.", Required = true)]
        public string regionId { get; set; }

        [Value(2, HelpText = "The registry containing the device.", Required = true)]
        public string registryId { get; set; }

        [Value(3, HelpText = "The device ID that will be bound to the gateway.", Required = true)]
        public string deviceId { get; set; }

        [Value(4, HelpText = "The gateway ID.")]
        public string gatewayId { get; set; }
    }

    [Verb("unbindDeviceFromGateway", HelpText = "Unbinds a device to a gateway.")]
    class UnbindDeviceFromGatewayOptions
    {
        [Value(0, HelpText = "The project containing the device registry.", Required = true)]
        public string projectId { get; set; }

        [Value(1, HelpText = "The region (e.g. us-central1) the registry is located in.", Required = true)]
        public string regionId { get; set; }

        [Value(2, HelpText = "The registry containing the device.", Required = true)]
        public string registryId { get; set; }

        [Value(3, HelpText = "The device ID that will be unbound from the gateway.", Required = true)]
        public string deviceId { get; set; }

        [Value(4, HelpText = "The gateway ID.")]
        public string gatewayId { get; set; }
    }

    [Verb("clearRegistry", HelpText = "Be careful! Removes all devices and then deletes a device Registry!!")]
    class ClearRegistryOptions
    {
        [Value(0, HelpText = "The project containing the device registry.", Required = true)]
        public string projectId { get; set; }

        [Value(1, HelpText = "The region (e.g. us-central1) the registry is located in.", Required = true)]
        public string regionId { get; set; }

        [Value(2, HelpText = "The registry containing the device.", Required = true)]
        public string registryId { get; set; }
    }

    [Verb("unbindAllDevices", HelpText = "Unbinds all devices in a given registry. Mainly for clearing registries.")]
    class UnbindAllDevicesOptions
    {
        [Value(0, HelpText = "The project containing the device registry.", Required = true)]
        public string projectId { get; set; }

        [Value(1, HelpText = "The region (e.g. us-central1) the registry is located in.", Required = true)]
        public string regionId { get; set; }

        [Value(2, HelpText = "The registry containing the device.", Required = true)]
        public string registryId { get; set; }
    }

    [Verb("listenForConfigMessages", HelpText = "")]
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

    [Verb("sendDataFromBoundDevice", HelpText = "")]
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


    public class CloudIotSample
    {
        private static readonly string s_projectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");

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

        // [START iot_create_registry]
        public static object CreateRegistry(string projectId, string cloudRegion, string registryId, string pubsubTopic)
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
                body.EventNotificationConfigs = new List<EventNotificationConfig>();
                var toAdd = new EventNotificationConfig()
                {
                    PubsubTopicName = pubsubTopic.StartsWith("projects/") ?
                        pubsubTopic : $"projects/{projectId}/topics/{pubsubTopic}",
                };
                body.EventNotificationConfigs.Add(toAdd);
                var registry = cloudIot.Projects.Locations.Registries.Create(body, parent).Execute();
                Console.WriteLine("Registry: ");
                Console.WriteLine($"{registry.Id}");
                Console.WriteLine($"\tName: {registry.Name}");
                Console.WriteLine($"\tHTTP Enabled: {registry.HttpConfig.HttpEnabledState}");
                Console.WriteLine($"\tMQTT Enabled: {registry.MqttConfig.MqttEnabledState}");
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

        // [START iot_delete_registry]
        public static object DeleteRegistry(string projectId, string cloudRegion, string registryId)
        {
            var cloudIot = CreateAuthorizedClient();
            // The resource name of the location associated with the key rings.
            var name = $"projects/{projectId}/locations/{cloudRegion}/registries/{registryId}";

            try
            {
                var registry = cloudIot.Projects.Locations.Registries.Delete(name).Execute();
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

        // [START iot_get_registry]
        public static object GetRegistry(string projectId, string cloudRegion, string registryId)
        {
            var cloudIot = CreateAuthorizedClient();
            // The resource name of the location associated with the key rings.
            var name = $"projects/{projectId}/locations/{cloudRegion}/registries/{registryId}";

            try
            {
                var registry = cloudIot.Projects.Locations.Registries.Get(name).Execute();
                Console.WriteLine($"Registry: {registry.Id}");
                Console.WriteLine("Notification Configurations:");
                registry.EventNotificationConfigs.ToList().ForEach(config =>
                {
                    Console.WriteLine($"\tPubSub: {config.PubsubTopicName}");
                });
                Console.WriteLine($"HTTP Enabled: {registry.HttpConfig.HttpEnabledState}");
                Console.WriteLine($"MQTT Enabled: {registry.MqttConfig.MqttEnabledState}");
            }
            catch (Google.GoogleApiException e)
            {
                Console.WriteLine(e.Message);
                if (e.Error != null) return e.Error.Code;
                return -1;
            }
            return 0;
        }
        // [END iot_get_registry]

        // [START iot_list_registries]
        public static object ListRegistries(string projectId, string cloudRegion)
        {
            var cloudIot = CreateAuthorizedClient();
            // The resource name of the location associated with the key rings.
            var parent = $"projects/{projectId}/locations/{cloudRegion}";
            Console.WriteLine("Registries:");
            try
            {
                var result = cloudIot.Projects.Locations.Registries.List(parent).Execute();

                result.DeviceRegistries.ToList().ForEach(registry =>
                {
                    Console.WriteLine($"\t{registry.Id}");
                });
            }
            catch (Google.GoogleApiException e)
            {
                Console.WriteLine(e.Message);
                if (e.Error != null) return e.Error.Code;
                return -1;
            }
            return 0;
        }
        // [END iot_list_registries]

        // [START iot_create_unauth_device]
        public static object CreateUnauthDevice(string projectId, string cloudRegion, string registryId, string deviceId)
        {
            var cloudIot = CreateAuthorizedClient();
            // The resource name of the location associated with the key rings.
            var parent = $"projects/{projectId}/locations/{cloudRegion}/registries/{registryId}";

            try
            {
                Device body = new Device()
                {
                    Id = deviceId
                };
                var device = cloudIot.Projects.Locations.Registries.Devices.Create(body, parent).Execute();
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
        // [END iot_create_unauth_device]

        // [START iot_create_es_device]
        public static object CreateEsDevice(string projectId, string cloudRegion, string registryId, string deviceId, string keyPath)
        {
            var cloudIot = CreateAuthorizedClient();
            var parent = $"projects/{projectId}/locations/{cloudRegion}/registries/{registryId}";

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
                        Format = "ES256_PEM"
                    },
                });

                var device = cloudIot.Projects.Locations.Registries.Devices.Create(body, parent).Execute();
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
        // [END iot_create_es_device]

        // [START iot_create_rsa_device]
        public static object CreateRsaDevice(string projectId, string cloudRegion, string registryId, string deviceId, string keyPath)
        {
            var cloudIot = CreateAuthorizedClient();
            var parent = $"projects/{projectId}/locations/{cloudRegion}/registries/{registryId}";

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

                var device = cloudIot.Projects.Locations.Registries.Devices.Create(body, parent).Execute();
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

        // [START iot_delete_device]
        public static object DeleteDevice(string projectId, string cloudRegion, string registryId, string deviceId)
        {
            var cloudIot = CreateAuthorizedClient();
            // The resource name of the location associated with the key rings.
            var name = $"projects/{projectId}/locations/{cloudRegion}/registries/{registryId}/devices/{deviceId}";

            try
            {
                var res = cloudIot.Projects.Locations.Registries.Devices.Delete(name).Execute();
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

        // [START iot_get_device]
        public static object GetDevice(string projectId, string cloudRegion, string registryId, string deviceId)
        {
            var cloudIot = CreateAuthorizedClient();
            // The resource name of the location associated with the key rings.
            var name = $"projects/{projectId}/locations/{cloudRegion}/registries/{registryId}/devices/{deviceId}";

            try
            {
                var device = cloudIot.Projects.Locations.Registries.Devices.Get(name).Execute();
                Console.WriteLine("Device: ");
                Console.WriteLine($"{device.Id}");
                Console.WriteLine($"\tBlocked: {device.Blocked == true}");
                Console.WriteLine($"\tConfig version: {device.Config.Version}");
                Console.WriteLine($"\tFirst Credential Expiry: {device.Credentials.First().ExpirationTime}");
                Console.WriteLine($"\tLast State Time:{device.LastStateTime}");
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
        // [END iot_get_device]

        // [START iot_get_device_configs]
        public static object GetDeviceConfigurations(string projectId, string cloudRegion, string registryId, string deviceId)
        {
            var cloudIot = CreateAuthorizedClient();
            // The resource name of the location associated with the key rings.
            var name = $"projects/{projectId}/locations/{cloudRegion}/registries/{registryId}/devices/{deviceId}";

            try
            {
                Console.WriteLine("Configurations: ");
                var res = cloudIot.Projects.Locations.Registries.Devices.ConfigVersions.List(name).Execute();
                res.DeviceConfigs.ToList().ForEach(config =>
                {
                    Console.WriteLine($"Version: {config.Version}");
                    Console.WriteLine($"\tUpdated: {config.CloudUpdateTime}");
                    Console.WriteLine($"\tDevice Ack: {config.DeviceAckTime}");
                    Console.WriteLine($"\tData: {config.BinaryData}");
                });
            }
            catch (Google.GoogleApiException e)
            {
                Console.WriteLine(e.Message);
                if (e.Error != null) return e.Error.Code;
                return -1;
            }
            return 0;
        }
        // [END iot_get_device_configs]

        // [START iot_get_device_state]
        public static object GetDeviceStates(string projectId, string cloudRegion, string registryId, string deviceId)
        {
            var cloudIot = CreateAuthorizedClient();

            // The resource name of the location associated with the key rings.
            var name = $"projects/{projectId}/locations/{cloudRegion}/registries/{registryId}/devices/{deviceId}";

            try
            {
                Console.WriteLine("States: ");
                var res = cloudIot.Projects.Locations.Registries.Devices.States.List(name).Execute();
                res.DeviceStates.ToList().ForEach(state =>
                {
                    Console.WriteLine($"\t{state.UpdateTime}: {state.BinaryData}");
                });
            }
            catch (Google.GoogleApiException e)
            {
                Console.WriteLine(e.Message);
                if (e.Error != null) return e.Error.Code;
                return -1;
            }
            return 0;
        }
        // [END iot_get_device_state]

        // [START iot_list_devices]
        public static object ListDevices(string projectId, string cloudRegion, string registryId)
        {
            var cloudIot = CreateAuthorizedClient();
            // The resource name of the location associated with the key rings.
            var parent = $"projects/{projectId}/locations/{cloudRegion}/registries/{registryId}";
            try
            {
                var result = cloudIot.Projects.Locations.Registries.Devices.List(parent).Execute();
                Console.WriteLine("Devices: ");
                result.Devices.ToList().ForEach(response =>
                {
                    Console.WriteLine($"{response.Id}");
                    Console.WriteLine($"\t{response.Config}");
                    Console.WriteLine($"\t{response.Name}");
                });
            }
            catch (Google.GoogleApiException e)
            {
                Console.WriteLine(e.Message);
                return e.Error.Code;
            }
            return 0;
        }
        // [END iot_list_devices]

        // [START iot_get_iam_policy]
        public static object GetIamPolicy(string projectId, string cloudRegion, string registryId)
        {
            var cloudIot = CreateAuthorizedClient();
            // The resource name of the location associated with the key rings.
            var parent = $"projects/{projectId}/locations/{cloudRegion}/registries/{registryId}";
            try
            {
                var req = new GetIamPolicyRequest();
                var result = cloudIot.Projects.Locations.Registries.GetIamPolicy(req, parent).Execute();
                Console.WriteLine("Bindings: ");
                if (result.Bindings != null)
                {
                    result.Bindings.ToList().ForEach(binding =>
                    {
                        Console.WriteLine($"Role: {binding.Role}");
                        binding.Members.ToList().ForEach(member =>
                        {
                            Console.WriteLine($"\tMember: {member}");
                        });
                    });
                }
                else
                {
                    Console.WriteLine($"\tNo IAM bindings for {registryId}.");
                }
            }
            catch (Google.GoogleApiException e)
            {
                Console.WriteLine(e.Message);
                return e.Error.Code;
            }
            return 0;
        }
        // [END iot_get_iam_policy]

        // [START iot_patch_es]
        public static object PatchEsDevice(string projectId, string cloudRegion, string registryId, string deviceId, string keyPath)
        {
            var cloudIot = CreateAuthorizedClient();
            var name = $"projects/{projectId}/locations/{cloudRegion}/registries/{registryId}/devices/{deviceId}";

            try
            {
                String keyText = File.ReadAllText(keyPath);
                Device body = new Device();
                body.Credentials = new List<DeviceCredential>();
                body.Credentials.Add(new DeviceCredential()
                {
                    PublicKey = new PublicKeyCredential()
                    {
                        Key = keyText,
                        Format = "ES256_PEM"
                    },
                });

                var req = cloudIot.Projects.Locations.Registries.Devices.Patch(body, name);
                req.UpdateMask = "credentials";
                var device = req.Execute();
                Console.WriteLine("Device patched: ");
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
        // [END iot_patch_es]

        // [START iot_patch_rsa]
        public static object PatchRsaDevice(string projectId, string cloudRegion, string registryId, string deviceId, string keyPath)
        {
            var cloudIot = CreateAuthorizedClient();
            var name = $"projects/{projectId}/locations/{cloudRegion}/registries/{registryId}/devices/{deviceId}";

            try
            {
                String keyText = File.ReadAllText(keyPath);
                Device body = new Device();
                body.Credentials = new List<DeviceCredential>();
                body.Credentials.Add(new DeviceCredential()
                {
                    PublicKey = new PublicKeyCredential()
                    {
                        Key = keyText,
                        Format = "RSA_X509_PEM"
                    },
                });

                var req = cloudIot.Projects.Locations.Registries.Devices.Patch(body, name);
                req.UpdateMask = "credentials";
                var device = req.Execute();
                Console.WriteLine("Device patched: ");
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
        // [END iot_patch_rsa]

        // [START iot_set_device_config]
        public static object SetDeviceConfig(string projectId, string cloudRegion, string registryId, string deviceId, string data)
        {
            var cloudIot = CreateAuthorizedClient();
            // The resource name of the location associated with the key rings.
            var name = $"projects/{projectId}/locations/{cloudRegion}/registries/{registryId}/devices/{deviceId}";

            try
            {
                ModifyCloudToDeviceConfigRequest req = new ModifyCloudToDeviceConfigRequest()
                {
                    BinaryData = Convert.ToBase64String(Encoding.Unicode.GetBytes(data))
                };

                var res = cloudIot.Projects.Locations.Registries.Devices.ModifyCloudToDeviceConfig(req, name).Execute();

                Console.WriteLine($"Configuration updated to: {res.Version}");
            }
            catch (Google.GoogleApiException e)
            {
                Console.WriteLine(e.Message);
                if (e.Error != null) return e.Error.Code;
                return -1;
            }
            return 0;
        }
        // [END iot_set_device_config]

        // [START iot_set_iam_policy]
        public static object SetIamPolicy(string projectId, string cloudRegion, string registryId,
                                         string role, string member)
        {
            var cloudIot = CreateAuthorizedClient();
            // The resource name of the location associated with the key rings.
            var parent = $"projects/{projectId}/locations/{cloudRegion}/registries/{registryId}";
            try
            {
                var req = new SetIamPolicyRequest();
                Policy pol = new Policy();
                pol.Bindings = new List<Binding>();
                Binding bind = new Binding()
                {
                    Role = role,
                    Members = new List<string>()
                };
                bind.Members.Add(member);
                pol.Bindings.Add(bind);


                req.Policy = pol;
                var result = cloudIot.Projects.Locations.Registries.SetIamPolicy(req, parent).Execute();
            }
            catch (Google.GoogleApiException e)
            {
                Console.WriteLine(e.Message);
                return e.Error.Code;
            }
            return 0;
        }
        //[END iot_set_iam_policy]

        //[START iot_send_command]  
        public static object SendCommand(string deviceId, string projectId,
            string cloudRegion, string registryName, string data)
        {
            var cloudIot = CreateAuthorizedClient();

            var devicePath = String.Format("projects/{0}/locations/{1}/registries/{2}/devices/{3}",
                projectId, cloudRegion, registryName, deviceId);
            // Data sent through the wire has to be base64 encoded.
            SendCommandToDeviceRequest req = new SendCommandToDeviceRequest()
            {
                BinaryData = Convert.ToBase64String(Encoding.UTF8.GetBytes(data))
            };

            Console.WriteLine("Sending command to {0}\n", devicePath);

            var res =
                cloudIot.Projects.Locations.Registries.Devices.SendCommandToDevice(req, devicePath).Execute();

            Console.WriteLine("Command response: " + res.ToString());
            return 0;
        }
        //[END iot_send_command]

        //[START iot_create_gateway]
        public static object CreateGateway(string projectId, string cloudRegion,
            string registryName, string gatewayId, string publicKeyFilePath,
            string algorithm)
        {
            var cloudIot = CreateAuthorizedClient();
            var registryPath = $"projects/{projectId}/locations/{cloudRegion}"
                + $"/registries/{registryName}";
            Console.WriteLine("Creating gateway with id: {0}", gatewayId);

            Device body = new Device()
            {
                Id = gatewayId,
                GatewayConfig = new GatewayConfig()
                {
                    GatewayType = "GATEWAY",
                    GatewayAuthMethod = "ASSOCIATION_ONLY"
                },
                Credentials =
                new List<DeviceCredential>()
                {
                    new DeviceCredential()
                    {
                        PublicKey = new PublicKeyCredential()
                        {
                            Key = File.ReadAllText(publicKeyFilePath),
                            Format = (algorithm == "ES256" ?
                                "ES256_PEM" : "RSA_X509_PEM")
                        },
                    }
                }
            };

            Device createdDevice = cloudIot.Projects.Locations.Registries
                .Devices.Create(body, registryPath).Execute();
            Console.WriteLine("Created gateway: {0}", createdDevice.ToString());
            return 0;
        }
        //[END iot_create_gateway]

        //[START iot_create_device]
        public static object CreateDevice(string projectId, string cloudRegion, string registryName, string deviceId)
        {
            var cloudIot = CreateAuthorizedClient();
            var registryPath = $"projects/{projectId}/locations/{cloudRegion}/registries/{registryName}";

            var req = cloudIot
                .Projects
                .Locations
                .Registries
                .Devices
                .List(registryPath);
            req.FieldMask = "config,gatewayConfig";
            var devices = req.Execute().Devices;
            if (devices != null)
            {
                Console.WriteLine("Found {0} devices", devices.Count);
                devices.ToList().ForEach(singleDevice =>
                {
                    if ((singleDevice.Id != null && singleDevice.Id.Equals(deviceId))
                    || (singleDevice.Name != null && singleDevice.Name.Equals(deviceId)))
                    {
                        Console.WriteLine("Device exists, skipping. ");
                        return;
                    }
                }
                );
            }
            Console.WriteLine("Creating device with id: {0}", deviceId);

            GatewayConfig gwConfig = new GatewayConfig()
            {
                GatewayType = "NON_GATEWAY",
                GatewayAuthMethod = "ASSOCIATION_ONLY"
            };

            Device device = new Device()
            {
                Id = deviceId,
                GatewayConfig = gwConfig
            };

            Device createdDevice =
                cloudIot
                    .Projects
                    .Locations
                    .Registries
                    .Devices
                    .Create(device, registryPath)
                    .Execute();

            Console.WriteLine("Created device: {0}", createdDevice.ToString());
            return 0;
        }
        //[END iot_create_device]

        //[START iot_list_gateways]
        public static object ListGateways(string projectId, string cloudRegion, string registryName)
        {
            var cloudIot = CreateAuthorizedClient();
            var registryPath = $"projects/{projectId}/locations/{cloudRegion}/registries/{registryName}";

            var req = cloudIot
                .Projects
                .Locations
                .Registries
                .Devices
                .List(registryPath);
            req.FieldMask = "config,gatewayConfig";

            var devices = req.Execute().Devices;

            if (devices != null)
            {
                Console.WriteLine("Found {0} devices", devices.Count);
                devices.ToList().ForEach(device =>
                   {
                       if (device.GatewayConfig != null
                       && device.GatewayConfig.GatewayType != null
                          && device.GatewayConfig.GatewayType.Equals("GATEWAY"))
                       {
                           Console.WriteLine("Id :{0}", device.Id);
                           if (device.Config != null)
                           {
                               Console.WriteLine("Config: {0}", device.Config.ToString());
                           }
                       }
                   }
                );
            }
            else
            {
                Console.WriteLine("Registry has no gateway devices");
            }

            return 0;
        }
        //[END iot_list_gateways]

        //[START iot_bind_device_to_gateway]
        public static object BindDeviceToGateway(string projectId, string cloudRegion,
            string registryName, string deviceId, string gatewayId)
        {
            CreateDevice(projectId, cloudRegion, registryName, deviceId);
            var cloudIot = CreateAuthorizedClient();
            var registryPath = $"projects/{projectId}" +
                $"/locations/{cloudRegion}" +
                $"/registries/{registryName}";
            BindDeviceToGatewayRequest req = new BindDeviceToGatewayRequest
            {
                DeviceId = deviceId,
                GatewayId = gatewayId
            };

            var res = cloudIot
                .Projects
                .Locations
                .Registries
                .BindDeviceToGateway(req, registryPath)
                .Execute();
            Console.WriteLine("Device bound: {0}", res.ToString());

            return 0;
        }
        //[END iot_bind_device_to_gateway]

        //[START iot_unbind_device_from_gateway]
        public static object UnbindDeviceFromGateway(string projectId, string cloudRegion,
            string registryName, string deviceId, string gatewayId)
        {
            var cloudIot = CreateAuthorizedClient();
            var registryPath = $"projects/{projectId}" +
                $"/locations/{cloudRegion}" +
                $"/registries/{registryName}";
            UnbindDeviceFromGatewayRequest req = new UnbindDeviceFromGatewayRequest
            {
                DeviceId = deviceId,
                GatewayId = gatewayId
            };

            var res = cloudIot
                .Projects
                .Locations
                .Registries
                .UnbindDeviceFromGateway(req, registryPath)
                .Execute();
            Console.WriteLine("Device unbound: {0}", deviceId);

            return 0;
        }
        //[END iot_unbind_device_from_gateway]

        //[START iot_list_devices_for_gateway]
        public static object ListDevicesForGateways(string projectId,
         string cloudRegion, string registryName, string gatewayId)
        {
            var cloudIot = CreateAuthorizedClient();
            var gatewayPath = $"projects/{projectId}/locations/{cloudRegion}" +
                $"/registries/{registryName}/devices/{gatewayId}";
            var registryPath = $"projects/{projectId}/locations/{cloudRegion}" +
                $"/registries/{registryName}";
            var req = cloudIot.Projects.Locations.Registries.Devices.List(registryPath);
            req.GatewayListOptionsAssociationsGatewayId = gatewayId;
            var devices = req.Execute().Devices;

            if (devices != null)
            {
                Console.WriteLine("Found {0} devices", devices.Count);
                foreach (var device in devices)
                {
                    Console.WriteLine("ID: {0}", device.Id);
                }
            }
            else
            {
                Console.WriteLine("Gateway has no bound devices.");
            }

            return 0;
        }
        //[END iot_list_devices_for_gateway]

        //[START iot_unbind_all_devices]
        public static object UnbindAllDevices(
            string projectId, string cloudRegion, string registryId)
        {
            var cloudIot = CreateAuthorizedClient();
            // The resource name of the location associated with the key rings.
            var parent = $"projects/{projectId}/locations/{cloudRegion}/registries/{registryId}";
            try
            {
                var devices = cloudIot
                    .Projects
                    .Locations
                    .Registries
                    .Devices
                    .List(parent)
                    .Execute()
                    .Devices;
                Console.WriteLine("Devices: {0}", parent);

                if (devices != null)
                {
                    foreach (var response in devices)
                    {
                        UnbindDeviceFromAllGateways(cloudIot, projectId,
                        cloudRegion, registryId, response.Id);
                    }
                }
                else
                {
                    Console.WriteLine("No bound device found.");
                }
            }
            catch (Google.GoogleApiException e)
            {
                Console.WriteLine(e.Message);
                return e.Error.Code;
            }
            return 0;
        }
        //[END iot_unbind_all_devices]

        public static void UnbindDeviceFromAllGateways(CloudIotService cloudIot, string projectId,
             string cloudRegion, string registryId, string deviceId)
        {
            // The resource name of the location associated with the key rings.
            var parent = $"projects/{projectId}/locations/{cloudRegion}/registries/{registryId}";
            var fullpath = $"projects/{projectId}/locations/{cloudRegion}" +
                $"/registries/{registryId}/devices/{deviceId}";
            try
            {
                var device = cloudIot
                    .Projects
                    .Locations
                    .Registries
                    .Devices
                    .Get(fullpath)
                    .Execute();

                if (device != null)
                {
                    bool isGateway = device.GatewayConfig.GatewayType == "GATEWAY";
                    if (!isGateway)
                    {
                        // get list of gateways this non-gateway device bound to.
                        var req = cloudIot.Projects.Locations.Registries.Devices.List(parent);
                        req.GatewayListOptionsAssociationsDeviceId = device.Id;
                        var gateways = req.Execute().Devices;
                        if (gateways != null)
                        {
                            foreach (var gateway in gateways)
                            {
                                UnbindDeviceFromGatewayRequest unbindReq
                                    = new UnbindDeviceFromGatewayRequest
                                    {
                                        DeviceId = device.Id,
                                        GatewayId = gateway.Id
                                    };
                                try
                                {
                                    cloudIot
                                        .Projects
                                        .Locations
                                        .Registries
                                        .UnbindDeviceFromGateway(unbindReq, parent).Execute();
                                    Console.WriteLine("Unbound device from the gateway {0}",
                                        gateway.Id);
                                }
                                catch (AggregateException e)
                                {
                                    Console.WriteLine("Could not unbind device");
                                    foreach (var ex in e.InnerExceptions)
                                    {
                                        Console.WriteLine(ex.Message);
                                    }
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine("Could not list the gateways for device {0}",
                                device.Id);
                            return;
                        }
                    }
                }
            }
            catch (Google.GoogleApiException e)
            {
                Console.WriteLine(e.Message);
            }
        }

        //[START iot_clear_registry]
        public static object ClearRegistry(string projectId, string cloudRegion, string registryId)
        {
            var parentName = $"projects/{projectId}/locations/{cloudRegion}";
            var registryName = $"{parentName}/registries/{registryId}";
            var cloudIot = CreateAuthorizedClient();
            string responseRegistry = "";


            // The resource name of the location associated with the key rings.
            var parent = $"projects/{projectId}/locations/{cloudRegion}/registries/{registryId}";
            try
            {
                var devices = cloudIot
                    .Projects
                    .Locations
                    .Registries
                    .Devices
                    .List(parent)
                    .Execute()
                    .Devices;
                Console.WriteLine("Devices: {0}", parent);

                if (devices != null)
                {
                    foreach (var response in devices)
                    {
                        DeleteDevice(projectId, cloudRegion, registryId, response.Id);
                    }
                }
            }
            catch (Google.GoogleApiException e)
            {
                Console.WriteLine(e.Message);
                return e.Error.Code;
            }

            try
            {
                DeleteRegistry(projectId, cloudRegion, registryId);
                Console.WriteLine($"Successfully deleted registry {registryName}");
                Console.WriteLine(responseRegistry);
            }
            catch
            (Google.GoogleApiException e)
            {
                Console.WriteLine("Could not delete registry");
                Console.WriteLine(e.Message);
            }


            return 0;
        }
        //[END iot_clear_registry]


        //[START iot_attach_device]
        public static object AttachDevice(MqttClient client, string deviceId)
        {
            var attachTopic = $"/devices/{deviceId}/attach";
            Console.WriteLine("Attaching: {0}", attachTopic);
            var attachPayLoad = "{}";
            var BinaryData = Encoding.UTF8.GetBytes(attachPayLoad);
            client.Publish(attachTopic, BinaryData, MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE, true);

            Console.WriteLine("Waiting for device to attach.");
            System.Threading.Thread.Sleep(2000);
            return 0;
        }
        //[END iot_attach_device]

        //[START iot_detach_device]
        public static object DetachDevice(MqttClient client, string deviceId)
        {
            var detachTopic = $"/devices/{deviceId}/detach";
            Console.WriteLine("Detaching: {0}", detachTopic);
            var BinaryData = Encoding.UTF8.GetBytes("{}");
            client.Publish(detachTopic, BinaryData, MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE, true);
            System.Threading.Thread.Sleep(2000);
            return 0;
        }
        //[END iot_detach_device]

        //[START iot_listen_for_config_messages]
        public static object ListenForConfigMessages(string projectId, string cloudRegion,
            string registryId, string deviceId, string gatewayId, int numMessages,
            string privateKeyFile, string algorithm, string caCerts, string mqttBridgeHostname,
            int mqttBridgePort, int jwtExpiresMinutes, int duration)
        {
            var clientId = $"projects/{projectId}/locations/{cloudRegion}/registries/{registryId}" +
                $"/devices/{gatewayId}";
            // Listens for configuration and system error messages on the gateway and
            // bound devices.

            var jwtIatTime = SystemClock.Instance.GetCurrentInstant();
            // Create a duration
            Duration durationExp = Duration.FromMinutes(jwtExpiresMinutes);
            var jwtExpTime = SystemClock.Instance.GetCurrentInstant().Plus(durationExp);
            var pass = CloudIotMqttExample.CreateJwtRsa(projectId, privateKeyFile);

            // Use gateway to connect server
            var mqttClient = StartMqtt(mqttBridgeHostname,
                mqttBridgePort,
                projectId,
                cloudRegion,
                registryId,
                gatewayId,
                privateKeyFile,
                caCerts,
                algorithm,
                pass);

            AttachDevice(mqttClient, deviceId);
            // The topic devices receive configuration updates on.
            string deviceConfigTopic = $"/devices/{deviceId}/config";

            mqttClient.Subscribe(
                new string[] { deviceConfigTopic },
                new byte[] { MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE }
            );

            // Wait for about a minute for config messages.
            for (int i = 0; i < duration; ++i)
            {
                Console.WriteLine("Listening...");
                var secSinceIssue = SystemClock.Instance.GetCurrentInstant().Minus(jwtIatTime);
                if (secSinceIssue.TotalSeconds > (60 * jwtExpiresMinutes)
                    || !mqttClient.IsConnected)
                {
                    Console.WriteLine("Refreshing token after {0}s", secSinceIssue);
                    jwtIatTime = SystemClock.Instance.GetCurrentInstant();
                    mqttClient = CloudIotMqttExample.GetClient(projectId, cloudRegion,
                        registryId, deviceId, privateKeyFile, algorithm,
           caCerts, mqttBridgeHostname, mqttBridgePort);
                    mqttClient.Connect(clientId, "unused",
                        CloudIotMqttExample.CreateJwtRsa(projectId, privateKeyFile));
                }
                System.Threading.Thread.Sleep(1000);
            }

            DetachDevice(mqttClient, deviceId);
            mqttClient.Disconnect();
            Console.WriteLine("Finished.");
            return 0;
        }
        //[END iot_listen_for_config_messages]

        //[START iot_send_data_from_bound_device]
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
            var mqttClient = CloudIotMqttExample.GetClient(projectId, cloudRegion, registryId,
                deviceId, privateKeyFile, algorithm, caCerts, mqttBridgeHostname, mqttBridgePort);

            var clientId = $"projects/{projectId}/locations/{cloudRegion}/registries/{registryId}" +
                $"/devices/{gatewayId}";
            var password = CloudIotMqttExample.CreateJwtRsa(projectId, privateKeyFile);
            mqttClient.Connect(clientId, "unused", password);

            AttachDevice(mqttClient, deviceId);
            System.Threading.Thread.Sleep(2000);
            // Publish numMsgs messages to the MQTT bridge.
            SendDataFromDevice(mqttClient, deviceId, messageType, payload);

            DetachDevice(mqttClient, deviceId);
            mqttClient.Disconnect();
            return 0;
        }
        //[END iot_send_data_from_bound_device]

        public static MqttClient StartMqtt(string mqttBridgeHostname, int mqttBridgePort,
            string projectId, string cloudRegion, string registryId, string gatewayId,
            string privateKeyFile, string caCert, string algorithm, string pass)
        {
            // Create our MQTT client. The mqttClientId is a unique string that identifies this 
            // device. For Google Cloud IoT Core, it must be in the format below.
            string mqttClientId = $"projects/{projectId}/locations/{cloudRegion}" +
                $"/registries/{registryId}/devices/{gatewayId}";
            MqttClient mqttClient = CloudIotMqttExample.GetClient(projectId, cloudRegion,
                registryId, gatewayId, privateKeyFile, algorithm, caCert, mqttBridgeHostname,
                mqttBridgePort);

            double initialConnectIntervalMillis = 0.5;
            double maxConnectIntervalMillis = 6;
            double maxConnectRetryTimeElapsedMillis = 900;
            double intervalMultiplier = 1.5;

            double retryIntervalMs = initialConnectIntervalMillis;
            double totalRetryTimeMs = 0;

            while (!mqttClient.IsConnected && totalRetryTimeMs < maxConnectRetryTimeElapsedMillis)
            {
                try
                {
                    // Connect to the Google MQTT bridge.
                    mqttClient.Connect(mqttClientId, "unused", pass);
                }
                catch (MqttCommunicationException ex)
                {
                    Console.WriteLine("An error occured {0}", ex.InnerException.Message);
                    Console.WriteLine("Retrying in " + retryIntervalMs + " seconds.");

                    System.Threading.Thread.Sleep((int)retryIntervalMs);
                    totalRetryTimeMs += retryIntervalMs;
                    retryIntervalMs *= 1.5;
                    if (retryIntervalMs > maxConnectIntervalMillis)
                    {
                        retryIntervalMs = maxConnectIntervalMillis;
                    }
                }
                catch (MqttClientException ex)
                {
                    Console.WriteLine("Client Exception" + ex.InnerException.Message);

                    System.Threading.Thread.Sleep((int)retryIntervalMs);
                    totalRetryTimeMs += retryIntervalMs;
                    retryIntervalMs *= intervalMultiplier;
                    if (retryIntervalMs > maxConnectIntervalMillis)
                    {
                        retryIntervalMs = maxConnectIntervalMillis;
                    }
                }
            }
            CloudIotMqttExample.SetupMqttTopics(mqttClient, gatewayId);
            return mqttClient;
        }

        //[START iot_send_data_from_device]
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
        //[END iot_send_data_from_device]

        public static int Main(string[] args)
        {
            var verbMap = new VerbMap<object>();
            verbMap
                .Add((CreateRegistryOptions opts) => CreateRegistry(
                    opts.projectId, opts.regionId, opts.registryId, opts.pubsubTopic))
                .Add((DeleteRegistryOptions opts) => DeleteRegistry(
                    opts.projectId, opts.regionId, opts.registryId))
                .Add((GetRegistryOptions opts) => GetRegistry(
                    opts.projectId, opts.regionId, opts.registryId))
                .Add((ListRegistryOptions opts) => ListRegistries(
                    opts.projectId, opts.regionId))
                .Add((CreateDeviceOptions opts) => CreateUnauthDevice(
                    opts.projectId, opts.regionId, opts.registryId, opts.deviceId))
                .Add((CreateEsDeviceOptions opts) => CreateEsDevice(
                    opts.projectId, opts.regionId, opts.registryId, opts.deviceId, opts.certificiatePath))
                .Add((CreateRsaDeviceOptions opts) => CreateRsaDevice(
                    opts.projectId, opts.regionId, opts.registryId, opts.deviceId, opts.certificiatePath))
                .Add((DeleteDeviceOptions opts) => DeleteDevice(
                    opts.projectId, opts.regionId, opts.registryId, opts.deviceId))
                .Add((GetDeviceOptions opts) => GetDevice(
                    opts.projectId, opts.regionId, opts.registryId, opts.deviceId))
                .Add((GetDeviceConfigsOptions opts) => GetDeviceConfigurations(
                    opts.projectId, opts.regionId, opts.registryId, opts.deviceId))
                .Add((GetIamPolicyOptions opts) => GetIamPolicy(
                    opts.projectId, opts.regionId, opts.registryId))
                .Add((ListDevicesOptions opts) => ListDevices(
                    opts.projectId, opts.regionId, opts.registryId))
                .Add((PatchEsDeviceOptions opts) => PatchEsDevice(
                    opts.projectId, opts.regionId, opts.registryId, opts.deviceId, opts.certificiatePath))
                .Add((PatchRsaDeviceOptions opts) => PatchRsaDevice(
                    opts.projectId, opts.regionId, opts.registryId, opts.deviceId, opts.certificiatePath))
                .Add((SetDeviceConfigOptions opts) => SetDeviceConfig(
                    opts.projectId, opts.regionId, opts.registryId, opts.deviceId, opts.data))
                .Add((SetIamPolicyOptions opts) => SetIamPolicy(
                    opts.projectId, opts.regionId, opts.registryId, opts.role, opts.member))
                .Add((SendCommandOptions opts) => SendCommand(
                    opts.deviceId, opts.projectId, opts.regionId, opts.registryId, opts.command))
                .Add((CreateGatewayOptions opts) => CreateGateway(
                    opts.projectId, opts.regionId, opts.registryId, opts.gatewayId,
                    opts.publicKeyFile, opts.algorithm))
                .Add((ListGatewaysOptions opts) => ListGateways(
                    opts.projectId, opts.regionId, opts.registryId))
                .Add((ListDevicesForGatewayOptions opts) => ListDevicesForGateways(
                    opts.projectId, opts.regionId, opts.registryId, opts.gatewayId))
                .Add((BindDeviceToGatewayOptions opts) => BindDeviceToGateway(
                    opts.projectId, opts.regionId, opts.registryId, opts.deviceId, opts.gatewayId))
                .Add((UnbindDeviceFromGatewayOptions opts) => UnbindDeviceFromGateway(
                    opts.projectId, opts.regionId, opts.registryId, opts.deviceId, opts.gatewayId))
                .Add((ClearRegistryOptions opts) => ClearRegistry(opts.projectId, opts.regionId,
                    opts.registryId))
                .Add((UnbindAllDevicesOptions opts) => UnbindAllDevices(opts.projectId,
                    opts.regionId, opts.registryId))
                .Add((ListenForConfigMessagesOptions opts) => ListenForConfigMessages(
                    opts.projectId, opts.regionId, opts.registryId, opts.deviceId, opts.gatewayId,
                    opts.numMsgs, opts.privateKeyPath, opts.algorithm, opts.ca_certs,
                    "mqtt.googleapis.com", 8883, opts.jwtExpTime, opts.listenTime))
                .Add((SendDataFromBoundDeviceOptions opts) => SendDataFromBoundDevice(
                    opts.projectId, opts.regionId, opts.registryId, opts.deviceId, opts.gatewayId,
                    opts.privateKeyPath, opts.algorithm, opts.ca_certs, "mqtt.googleapis.com",
                    8883, opts.jwtExpTime, opts.messageType, opts.data))
                .NotParsedFunc = (err) => 1;
            return (int)verbMap.Run(args);
        }
    }
}