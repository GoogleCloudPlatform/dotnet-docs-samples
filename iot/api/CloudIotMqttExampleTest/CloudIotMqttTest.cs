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

using Google.Cloud.Iam.V1;
using Google.Cloud.PubSub.V1;
using Grpc.Core;
using Xunit;
using System;
using Google.Api;
using System.IO;
using Xunit.Abstractions;
using System.Threading;
using System.Threading.Tasks;

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
            var createRsaOut = Run("createDeviceRsa", _fixture.ProjectId, _fixture.RegionId, _fixture.RegistryId, deviceId, "test/data/rsa_cert.pem");
            Assert.Contains("Device created:", createRsaOut.Stdout);
            try
            {
                var mqttExampleOut = Run("startMqtt",
                   _fixture.ProjectId,
                   _fixture.RegionId,
                   _fixture.RegistryId,
                   deviceId,
               _fixture.PrivateKeyPath,
               "RS256",
               "test/data/roots.pem",
               "1",
               "events",
               "mqtt.googleapis.com",
               "443",
               "1",
               "--waittime",
               "5");
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
                Run("deleteDevice", _fixture.ProjectId, _fixture.RegionId, _fixture.RegistryId, deviceId);
            }
        }

        [Fact]
        public void TestMqttDeviceCommand()
        {
            var deviceId = "rsa-device-mqtt-commands-" + _fixture.TestId;

            //Setup screnario
            var createRsaOut = Run("createDeviceRsa", _fixture.ProjectId, _fixture.RegionId, _fixture.RegistryId, deviceId, "test/data/rsa_cert.pem");
            Assert.Contains("Device created:", createRsaOut.Stdout);

            try
            {
                Task tast = new Task(() => StartMqtt(deviceId, _fixture.PrivateKeyPath));
                tast.Start();

                //Wait for the device to connect
                Thread.Sleep(5000);

                var sendCommandOutput = Run("sendCommand", deviceId, _fixture.ProjectId, _fixture.RegionId, _fixture.RegistryId, "me want steak");
                Assert.Contains("me want steak", sendCommandOutput.Stdout);
                Assert.DoesNotContain("Failure", sendCommandOutput.Stdout);
            }
            catch
            (Google.GoogleApiException e)
            {
                _output.WriteLine(e.Message);
                Console.WriteLine(e.Message);
                throw new Google.GoogleApiException(e.ServiceName, e.Message);
            }
            finally
            {
                //Clean up
                Run("deleteDevice", _fixture.ProjectId, _fixture.RegionId, _fixture.RegistryId, deviceId);
            }
        }

        private void StartMqtt(string deviceId, string privateKeyPath)
        {
            Console.WriteLine("WHAATT {0}", privateKeyPath);
            CloudIotMqttExample.StartMqtt(_fixture.ProjectId, _fixture.RegionId, _fixture.RegistryId,
               deviceId, privateKeyPath, "RS256", "test/data/roots.pem", 1, "events", "mqtt.googleapis.com", 443, 1, 20);
        }


        [Fact]
        public void TestMqttDeviceState()
        {
            var deviceId = "rsa-device-mqtt-state-" + _fixture.TestId;
            try
            {
                //Setup screnario
                Run("createDeviceRsa", _fixture.ProjectId, _fixture.RegionId, _fixture.RegistryId, deviceId, "test/data/rsa_cert.pem");
                var mqttExampleOut = Run("startMqtt",
                      _fixture.ProjectId,
                      _fixture.RegionId,
                      _fixture.RegistryId,
                      deviceId,
                      _fixture.PrivateKeyPath,
                      "RS256",
                      "test/data/roots.pem",
                      "1",
                      "state",
                      "mqtt.googleapis.com",
                      "443",
                      "1",
                      "--waittime",
                      "5");
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
                Run("deleteDevice", _fixture.ProjectId, _fixture.RegionId, _fixture.RegistryId, deviceId);
            }
        }
        //[END iot_mqtt_tests]
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
        public IotTestFixture()
        {
            RegionId = "us-central1";
            ProjectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
            string absolutePath = Directory.GetParent(Directory.GetCurrentDirectory()).FullName;
            string privateKeyPath = absolutePath.Substring(0, absolutePath.IndexOf("iot")) + Environment.GetEnvironmentVariable("TEST_IOT_PRIVATE_KEY_PATH");
            if (!File.Exists(privateKeyPath))
            {
                // Set environment variable for linux machine.
                privateKeyPath = absolutePath.Substring(0, absolutePath.IndexOf("iot")) + Environment.GetEnvironmentVariable("TEST_IOT_PRIVATE_KEY_PATH_LINUX");
            }
            if (privateKeyPath.Length == 0) throw new NullReferenceException("Private key path is not for unit tests.");
            PrivateKeyPath = privateKeyPath;
            ServiceAccount = "serviceAccount:cloud-iot@system.gserviceaccount.com";
            TestId = TestUtil.RandomName();
            TopicName = new TopicName(ProjectId, "iot-test-" + TestId);
            RegistryId = "iot-test-" + TestId;
            CreatePubSubTopic(this.TopicName);
            Assert.Equal(0, Run("createRegistry", ProjectId, RegionId,
                RegistryId, TopicName.TopicId).ExitCode);
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
            var deleteRegOutput = Run("deleteRegistry", ProjectId, RegionId, RegistryId);
            DeletePubSubTopic(this.TopicName);
        }
    }
}
