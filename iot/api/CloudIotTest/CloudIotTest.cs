// Copyright(c) 2018 Google Inc.
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

using Google.Apis.CloudIot.v1.Data;
using Google.Cloud.PubSub.V1;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.IO;
using Xunit;
using Binding = Google.Cloud.Iam.V1.Binding;
using Policy = Google.Cloud.Iam.V1.Policy;
using SetIamPolicyRequest = Google.Cloud.Iam.V1.SetIamPolicyRequest;

namespace GoogleCloudSamples
{
    // <summary>
    /// Runs the sample app's methods and tests the outputs
    // </summary>
    public class CommonTests : IClassFixture<IotTestFixture>
    {
        private readonly RetryRobot _retryRobot = new RetryRobot()
        {
            RetryWhenExceptions = new[] { typeof(Xunit.Sdk.XunitException) }
        };
        private readonly IotTestFixture _fixture;

        public CommonTests(IotTestFixture fixture)
        {
            _fixture = fixture;
        }

        ConsoleOutput Run(params string[] args) => _fixture.Run(args);

        private void Eventually(Action action) => _retryRobot.Eventually(action);

        [Fact]
        public void TestListRegistries()
        {
            var listRegistryOutput = Run("listRegistries", _fixture.ProjectId, _fixture.RegionId);
            Assert.Contains("Registries:", listRegistryOutput.Stdout);
        }

        [Fact]
        public void TestGetRegistry()
        {
            var getRegistryOutput = Run("getRegistry", _fixture.ProjectId, _fixture.RegionId,
                _fixture.RegistryId);
            Assert.Contains("Registry:", getRegistryOutput.Stdout);
        }

        [Fact]
        public void TestCreateUnauthDevice()
        {
            var deviceId = "dotnettest-unauth-" + _fixture.TestId;
            try
            {
                var createUnauthOut = Run("createDeviceNoAuth", _fixture.ProjectId, _fixture.RegionId,
                    _fixture.RegistryId, deviceId);
                Assert.Contains("Device created:", createUnauthOut.Stdout);
            }
            finally
            {
                var deleteUnauthOut = Run("deleteDevice", _fixture.ProjectId, _fixture.RegionId,
                    _fixture.RegistryId, deviceId);
                Assert.Contains("Removed device:", deleteUnauthOut.Stdout);
            }
        }

        [Fact]
        public void TestGetDeviceConfigs()
        {
            var deviceId = "dotnettest-unauth-" + _fixture.TestId;

            Run("createDeviceNoAuth", _fixture.ProjectId, _fixture.RegionId, _fixture.RegistryId,
                deviceId);
            try
            {
                var getConfigsOut = Run("getDeviceConfigs", _fixture.ProjectId, _fixture.RegionId,
                    _fixture.RegistryId, deviceId);
                Assert.Contains("Configurations:", getConfigsOut.Stdout);
            }
            finally
            {
                // Tear down Device, Registry, and IoT PubSub topic
                Run("deleteDevice", _fixture.ProjectId, _fixture.RegionId, _fixture.RegistryId,
                    deviceId);
            }
        }

        [Fact]
        public void TestCreateEsDevice()
        {
            var deviceId = "dotnettest-createES-" + _fixture.TestId;

            var createEsOut = Run("createDeviceEs", _fixture.ProjectId, _fixture.RegionId,
                _fixture.RegistryId, deviceId, "test/data/ec_public.pem");
            Assert.Contains("Device created:", createEsOut.Stdout);

            var deleteEsOut = Run("deleteDevice", _fixture.ProjectId, _fixture.RegionId,
                _fixture.RegistryId, deviceId);
            Assert.Contains("Removed device:", deleteEsOut.Stdout);
        }

        [Fact]
        public void TestCreateRsaDevice()
        {
            var deviceId = "dotnettest-createRSA-" + _fixture.TestId;

            var createRsaOut = Run("createDeviceRsa", _fixture.ProjectId, _fixture.RegionId,
                _fixture.RegistryId, deviceId, "test/data/rsa_cert.pem");
            Assert.Contains("Device created:", createRsaOut.Stdout);

            var deleteRsaOut = Run("deleteDevice", _fixture.ProjectId, _fixture.RegionId,
                _fixture.RegistryId, deviceId);
            Assert.Contains("Removed device:", deleteRsaOut.Stdout);
        }

        [Fact]
        public void TestCreatePatchEsDevice()
        {
            var deviceId = "dotnettest-unauth-es-" + _fixture.TestId;

            Run("createDeviceNoAuth", _fixture.ProjectId, _fixture.RegionId, _fixture.RegistryId,
                deviceId);
            try
            {
                var patchUnauthOut = Run("patchDeviceEs", _fixture.ProjectId, _fixture.RegionId,
                    _fixture.RegistryId, deviceId, "test/data/ec_public.pem");
                Assert.Contains("Device patched:", patchUnauthOut.Stdout);
            }
            finally
            {
                Run("deleteDevice", _fixture.ProjectId, _fixture.RegionId, _fixture.RegistryId,
                    deviceId);
            }
        }

        [Fact]
        public void TestCreatePatchRsaDevice()
        {
            var deviceId = "dotnettest-unauth-rsa-" + _fixture.TestId;

            Run("createDeviceNoAuth", _fixture.ProjectId, _fixture.RegionId, _fixture.RegistryId,
                deviceId);
            try
            {
                var patchUnauthOut = Run("patchDeviceRsa", _fixture.ProjectId, _fixture.RegionId,
                    _fixture.RegistryId, deviceId, "test/data/rsa_cert.pem");
                Assert.Contains("Device patched:", patchUnauthOut.Stdout);
            }
            finally
            {
                Run("deleteDevice", _fixture.ProjectId, _fixture.RegionId, _fixture.RegistryId,
                    deviceId);
            }
        }

        [Fact]
        public void TestSetDeviceConfig()
        {
            var deviceId = "dotnettest-config-" + _fixture.TestId;

            Run("createDeviceNoAuth", _fixture.ProjectId, _fixture.RegionId, _fixture.RegistryId,
                deviceId);
            try
            {
                var setDeviceOut = Run("setDeviceConfig", _fixture.ProjectId, _fixture.RegionId,
                    _fixture.RegistryId, deviceId, "test");
                Assert.Contains("Configuration updated to: 2", setDeviceOut.Stdout);
            }
            finally
            {
                // Tear down Registry and IoT PubSub topic
                Run("deleteDevice", _fixture.ProjectId, _fixture.RegionId, _fixture.RegistryId, deviceId);
            }
        }

        [Fact]
        public void TestGetSetIamBinding()
        {
            var member = "group:dpebot@google.com";
            var role = "roles/viewer";

            var setIamOutput = Run("setIamPolicy", _fixture.ProjectId, _fixture.RegionId, _fixture.RegistryId,
                role, member);
            Assert.DoesNotContain("RequestError", setIamOutput.Stdout);
            var getIamOutput = Run("getIamPolicy", _fixture.ProjectId, _fixture.RegionId,
                _fixture.RegistryId);
            System.Diagnostics.Trace.WriteLine(getIamOutput.Stdout);
            Assert.Contains("Role: roles/viewer", getIamOutput.Stdout);
        }

        [Fact]
        public void TestListDevicesNoRegistry()
        {
            var registryId = $"{_fixture.TestId}-notfounddevicereg";
            var listDevicesOutput = Run("listDevices", _fixture.ProjectId, _fixture.RegionId,
                registryId);
            Assert.Contains("A registry with the name", listDevicesOutput.Stdout);
        }

        [Fact]
        public void TestGetDeviceRegistryNotFound()
        {
            var registryId = $"{_fixture.TestId}-notfoundregistry";
            var listDevicesOutput = Run("getRegistry", _fixture.ProjectId, _fixture.RegionId,
                registryId);
            Assert.Contains("A registry with the name", listDevicesOutput.Stdout);
        }

        [Fact]
        public void TestSendCommand()
        {
            // Setup scenario
            var deviceId = "dotnettest-unauth-rsa-" + _fixture.TestId;
            try
            {
                var createRsaOut = Run("createDeviceRsa", _fixture.ProjectId, _fixture.RegionId,
                    _fixture.RegistryId, deviceId, "test/data/rsa_cert.pem");
                Assert.Contains("Device created:", createRsaOut.Stdout);
                Run("sendCommand", deviceId, _fixture.ProjectId, _fixture.RegionId,
                    _fixture.RegistryId, "test");
            }
            catch (Google.GoogleApiException e)
            {
                Assert.True(e.Error.Code == 400);
            }
            finally
            {
                Run("deleteDevice", _fixture.ProjectId, _fixture.RegionId, _fixture.RegistryId,
                    deviceId);
            }
        }

        //[START iot_gateway_tests]
        [Fact]
        public void TestCreateGateway()
        {
            string gatewayName = "dotnettest-create-gateway" + _fixture.TestId;
            try
            {
                var createGatewayOut = Run("createGateway", _fixture.ProjectId, _fixture.RegionId,
                    _fixture.RegistryId, gatewayName, "test/data/rsa_cert.pem", "RS256");
                Assert.Contains("Creating gateway with id", createGatewayOut.Stdout);
                Assert.Contains("Created gateway:", createGatewayOut.Stdout);
            }
            finally
            {
                Run("deleteDevice", _fixture.ProjectId, _fixture.RegionId, _fixture.RegistryId,
                    gatewayName);
            }
        }

        [Fact]
        public void TestListGateways()
        {
            string gatewayName = "dotnettest-gateway-" + _fixture.TestId;
            //Setup scenario
            try
            {
                var createGatewayOut = Run("createGateway", _fixture.ProjectId, _fixture.RegionId,
                    _fixture.RegistryId, gatewayName, "test/data/rsa_cert.pem", "RS256");
                Assert.Contains("Creating gateway with id", createGatewayOut.Stdout);
                var listGatewaysOut = Run("listGateways", _fixture.ProjectId, _fixture.RegionId,
                    _fixture.RegistryId);
                Assert.Contains("Found", listGatewaysOut.Stdout);
                Assert.Contains(string.Format("Id :{0}", gatewayName), listGatewaysOut.Stdout);
            }
            finally
            {
                Run("deleteDevice", _fixture.ProjectId, _fixture.RegionId,
                    _fixture.RegistryId, gatewayName);
            }
        }

        [Fact]
        public void TestListDevicesForGateway()
        {
            //Setup scenario
            string gatewayName = "dotnet-test-gateway-" + _fixture.TestId;
            string deviceId = "dotnet-test-device-" + _fixture.TestId;
            try
            {
                var createGatewayOut = Run("createGateway", _fixture.ProjectId, _fixture.RegionId,
                    _fixture.RegistryId, gatewayName, "test/data/rsa_cert.pem", "RS256");
                Assert.Contains("Creating gateway with id", createGatewayOut.Stdout);
                //Bind new device to new gateway
                var bindDeviceToGatewayOut = Run("bindDeviceToGateway", _fixture.ProjectId,
                    _fixture.RegionId, _fixture.RegistryId, deviceId, gatewayName);
                Assert.Contains($"Creating device with id: {deviceId}",
                    bindDeviceToGatewayOut.Stdout);
                Assert.Contains("Created device:", bindDeviceToGatewayOut.Stdout);
                Assert.Contains("Device bound:", bindDeviceToGatewayOut.Stdout);
                //Check if device is bound to the gateway
                var listDevicesForGatewayOut = Run("listDevicesForGateway", _fixture.ProjectId,
                    _fixture.RegionId, _fixture.RegistryId, gatewayName);
                Assert.Contains("Found", listDevicesForGatewayOut.Stdout);
                Assert.Contains($"ID: {deviceId}", listDevicesForGatewayOut.Stdout);
            }
            finally
            {
                Run("unbindDeviceFromGateway", _fixture.ProjectId, _fixture.RegionId,
                    _fixture.RegistryId, deviceId, gatewayName);
                Run("deleteDevice", _fixture.ProjectId, _fixture.RegionId,
                    _fixture.RegistryId, deviceId);
                Run("deleteDevice", _fixture.ProjectId, _fixture.RegionId,
                    _fixture.RegistryId, gatewayName);
            }
        }
        //[END iot_gateway_tests]
    }

    public class IotTestFixture : IDisposable
    {
        public TopicName TopicName { get; private set; }
        public string RegistryId { get; private set; }

        public string TestId { get; private set; }

        public string ProjectId { get; private set; }
        public string ServiceAccount { get; private set; }

        public string RegionId { get; private set; }
        public string CertPath { get; private set; }
        public string PrivateKeyPath { get; private set; }

        public IotTestFixture()
        {
            RegionId = "us-central1";
            ProjectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
            ServiceAccount = "serviceAccount:cloud-iot@system.gserviceaccount.com";
            TestId = ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds().ToString();
            Console.WriteLine(TestId);
            TopicName = new TopicName(ProjectId, "iot-test-" + TestId);
            RegistryId = "iot-test-" + TestId;
            string privateKeyPath = Environment.GetEnvironmentVariable("IOT_PRIVATE_KEY_PATH");
            if (privateKeyPath.Length == 0 || !File.Exists(privateKeyPath))
            {
                throw new NullReferenceException("Private key path is not for unit tests.");
            }
            CertPath = Environment.GetEnvironmentVariable("IOT_CERT_KEY_PATH");
            PrivateKeyPath = privateKeyPath;
            CreatePubSubTopic(this.TopicName);
            // Check if the number of registries does not exceed 90.

            CheckRegistriesLimit(ProjectId, RegionId);
            Assert.Equal(0, Run("createRegistry", ProjectId, RegionId,
                RegistryId, TopicName.TopicId).ExitCode);
        }

        public void CheckRegistriesLimit(string projectId, string regionId)
        {
            List<DeviceRegistry> listRegistries = (List<DeviceRegistry>)CloudIotSample.GetRegistries(projectId, regionId);
            if (listRegistries != null && listRegistries.Count > 90)
            {
                //Clean 20 oldest registries with testing prefix in the project.

                Console.WriteLine("The maximum number of registries is about to exceed.");
                Console.WriteLine("Deleting the oldest 20 registries with IoT Test prefix");
                var count = 20;
                var index = 0;
                while (count > 0)
                {
                    if (listRegistries[index].Id.Contains("iot-test-"))
                    {
                        CloudIotSample.UnbindAllDevices(projectId, regionId, listRegistries[index].Id);
                        CloudIotSample.ClearRegistry(projectId, regionId, listRegistries[index].Id);
                        count--;
                    }
                    index++;
                }
            }
        }
        public void CreatePubSubTopic(TopicName topicName)
        {
            var publisher = PublisherServiceApiClient.Create();
            try
            {
                publisher.CreateTopic(topicName);
            }
            catch (RpcException e)
            when (e.Status.StatusCode == StatusCode.AlreadyExists)
            {
            }
            Policy policy = new Policy
            {
                Bindings =
                    {
                        new Binding {
                            Role = "roles/pubsub.publisher",
                            Members = { ServiceAccount }
                        }
                    }
            };
            SetIamPolicyRequest request = new SetIamPolicyRequest
            {
                Resource = $"projects/{ProjectId}/topics/{topicName.TopicId}",
                Policy = policy
            };
            Policy response = publisher.SetIamPolicy(request);
            Console.WriteLine($"Topic IAM Policy updated: {response}");
        }

        public void DeletePubSubTopic(TopicName topicName)
        {
            PublisherServiceApiClient publisher = PublisherServiceApiClient.Create();
            publisher.DeleteTopic(topicName);
        }

        readonly CommandLineRunner _cloudIot = new CommandLineRunner()
        {
            Main = CloudIotSample.Main,
            Command = "CloudIotSample"
        };

        public ConsoleOutput Run(params string[] args)
        {
            return _cloudIot.Run(args);
        }

        public void Dispose()
        {
            var deleteRegOutput = Run("deleteRegistry", ProjectId, RegionId, RegistryId);
            DeletePubSubTopic(this.TopicName);
        }
    }
}
