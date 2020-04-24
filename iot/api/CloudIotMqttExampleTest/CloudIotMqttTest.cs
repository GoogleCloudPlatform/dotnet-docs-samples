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
using Xunit.Abstractions;
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
        private readonly ITestOutputHelper _output;

        public CommonTests(IotTestFixture fixture, ITestOutputHelper helper)
        {
            _fixture = fixture;
            //for displaying unit tests output in the console.
            _output = helper;
        }

        ConsoleOutput Run(params string[] args) => _fixture.Run(args);

        private void Eventually(Action action) => _retryRobot.Eventually(action);

        //[START iot_mqtt_tests]
        [Fact]
        public void TestMqttDeviceEvents()
        {
            var deviceId = "rsa-device-mqttconfig-" + _fixture.TestId;

            //Setup scenario
            CloudIotSample.CreateRsaDevice(_fixture.ProjectId, _fixture.RegionId, _fixture.RegistryId, deviceId, "test/data/rsa_cert.pem");
            try
            {
                var mqttExampleOut = Run("startMqtt",
                   _fixture.ProjectId,
                   _fixture.RegionId,
                   _fixture.RegistryId,
                   deviceId,
               _fixture.PrivateKeyPath,
               "RS256",
               _fixture.CertPath,
               "events",
               "--waittime",
               "5",
               "--nummessages",
               "1");
                Assert.Contains("Publishing events message", mqttExampleOut.Stdout);
                Assert.Contains("On Publish", mqttExampleOut.Stdout);
            }
            catch (Google.GoogleApiException e)
            {
                _output.WriteLine("Failure on exception: {0}", e.Message);
                Console.WriteLine("Failure on exception: {0}", e.Message);
            }
            finally
            {
                //Clean up
                CloudIotSample.DeleteDevice(_fixture.ProjectId, _fixture.RegionId, _fixture.RegistryId, deviceId);
            }
        }

        [Fact]
        public void TestMqttDeviceState()
        {
            var deviceId = "rsa-device-mqtt-state-" + _fixture.TestId;
            try
            {
                //Setup screnario
                CloudIotSample.CreateRsaDevice(_fixture.ProjectId, _fixture.RegionId, _fixture.RegistryId, deviceId, "test/data/rsa_cert.pem");
                var mqttExampleOut = Run("startMqtt",
                      _fixture.ProjectId,
                      _fixture.RegionId,
                      _fixture.RegistryId,
                      deviceId,
                      _fixture.PrivateKeyPath,
                      "RS256",
                      _fixture.CertPath,
                      "state",
                      "--waittime",
                      "5",
                      "--nummessages",
                      "1");
                Assert.Contains("Publishing state message", mqttExampleOut.Stdout);
                Assert.Contains("On Publish", mqttExampleOut.Stdout);
            }
            catch (Google.GoogleApiException e)
            {
                Console.WriteLine("Failure on exception: {0}", e.Message);
            }
            finally
            {
                //Clean up
                CloudIotSample.DeleteDevice(_fixture.ProjectId, _fixture.RegionId, _fixture.RegistryId, deviceId);
            }
        }
        //[END iot_mqtt_tests]

        //[START iot_gateway_mqtt_tests]
        [Fact]
        public void TestSendDataForBoundDevice()
        {
            var gatewayId = string.Format("test-gateway-{0}", "RS256");
            var deviceId = string.Format("test-device-{0}", TestUtil.RandomName());

            try
            {
                //Setup
                CloudIotSample.CreateGateway(_fixture.ProjectId, _fixture.RegionId, _fixture.RegistryId,
                    gatewayId, "test/data/rsa_cert.pem", "RS256");
                CloudIotSample.BindDeviceToGateway(_fixture.ProjectId, _fixture.RegionId,
                    _fixture.RegistryId, deviceId, gatewayId);

                //Connect the gateway
                var sendDataBoundDeviceOut = Run("sendDataFromBoundDevice", _fixture.ProjectId,
                    _fixture.RegionId, _fixture.RegistryId,
                deviceId, gatewayId, _fixture.PrivateKeyPath, "RS256", _fixture.CertPath,
                "state", "test-message");
                Assert.Contains("Data sent", sendDataBoundDeviceOut.Stdout);
                Assert.Contains("On Publish", sendDataBoundDeviceOut.Stdout);
                Assert.DoesNotContain("An error occured", sendDataBoundDeviceOut.Stdout);
            }
            finally
            {
                //Clean up
                CloudIotSample.UnbindDeviceFromGateway(_fixture.ProjectId, _fixture.RegionId,
                    _fixture.RegistryId, deviceId, gatewayId);
                CloudIotSample.DeleteDevice(_fixture.ProjectId, _fixture.RegionId,
                    _fixture.RegistryId, deviceId);
                CloudIotSample.DeleteDevice(_fixture.ProjectId, _fixture.RegionId,
                    _fixture.RegistryId, gatewayId);
            }
        }

        [Fact]
        public void TestGatewayListenForDevice()
        {
            var gatewayId = "rsa-listen-gateway";
            var deviceId = "rsa-listen-device";

            try
            {
                //Setup
                CloudIotSample.CreateGateway(_fixture.ProjectId, _fixture.RegionId,
                    _fixture.RegistryId, gatewayId, "test/data/rsa_cert.pem", "RS256");
                CloudIotSample.BindDeviceToGateway(_fixture.ProjectId, _fixture.RegionId, _fixture.RegistryId,
                    deviceId, gatewayId);

                //Connect 
                var listenConfigMsgOut = Run("listenForConfigMessages", _fixture.ProjectId,
                    _fixture.RegionId, _fixture.RegistryId, gatewayId, deviceId, _fixture.CertPath,
                    _fixture.PrivateKeyPath, "RS256", "--listentime", "10");

                //Assertions
                Assert.DoesNotContain("error occurred", listenConfigMsgOut.Stdout);
                Assert.Contains("On Subscribe", listenConfigMsgOut.Stdout);
                Assert.Contains("On Publish", listenConfigMsgOut.Stdout);
            }
            finally
            {
                //Clean up
                CloudIotSample.UnbindDeviceFromGateway(_fixture.ProjectId, _fixture.RegionId,
                    _fixture.RegistryId, deviceId, gatewayId);
                CloudIotSample.DeleteDevice(_fixture.ProjectId, _fixture.RegionId, _fixture.RegistryId, deviceId);
                CloudIotSample.DeleteDevice(_fixture.ProjectId, _fixture.RegionId, _fixture.RegistryId, gatewayId);
            }
        }
        //[END iot_gateway_mqtt_tests]
    }

    public class IotTestFixture : IDisposable
    {
        public TopicName TopicName { get; private set; }
        public string RegistryId { get; private set; }

        public string TestId { get; private set; }

        public string ProjectId { get; private set; }
        public string ServiceAccount { get; private set; }

        public string RegionId { get; private set; }

        public string PrivateKeyPath { get; private set; }
        public string CertPath { get; private set; }
        public IotTestFixture()
        {
            RegionId = "us-central1";
            ProjectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
            string privateKeyPath = Environment.GetEnvironmentVariable("IOT_PRIVATE_KEY_PATH");
            if (privateKeyPath.Length == 0 || !File.Exists(privateKeyPath))
            {
                throw new NullReferenceException("Private key path is not for unit tests.");
            }
            CertPath = Environment.GetEnvironmentVariable("IOT_CERT_KEY_PATH");
            PrivateKeyPath = privateKeyPath;
            ServiceAccount = "serviceAccount:cloud-iot@system.gserviceaccount.com";
            TestId = ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds().ToString();
            TopicName = new TopicName(ProjectId, "iot-test-" + TestId);
            RegistryId = "iot-test-" + TestId;
            CreatePubSubTopic(this.TopicName);
            // Check if the number of registries does not exceed 90.
            CheckRegistriesLimit(ProjectId, RegionId);
            Assert.Equal(0, CloudIotSample.CreateRegistry(ProjectId, RegionId,
                RegistryId, TopicName.TopicId));
        }
        public void CheckRegistriesLimit(string projectId, string regionId)
        {
            List<DeviceRegistry> listRegistries = (List<DeviceRegistry>)CloudIotMqttExample.GetRegistries(projectId, regionId);
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
            Main = CloudIotMqttExample.Main,
            Command = "CloudIotMqttExample"
        };

        public ConsoleOutput Run(params string[] args)
        {
            return _cloudIot.Run(args);
        }

        public void Dispose()
        {
            var deleteRegOutput = CloudIotSample.DeleteRegistry(ProjectId, RegionId, RegistryId);
            DeletePubSubTopic(this.TopicName);
        }
    }
}
