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

namespace GoogleCloudSamples
{
    // <summary>
    /// Runs the sample app's methods and tests the outputs
    // </summary>
    public class CommonTests
    {
        private static readonly string s_projectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
        private static readonly string s_regionId = "us-central1";
        private static readonly string s_serviceAccount = "serviceAccount:cloud-iot@system.gserviceaccount.com";


        readonly CommandLineRunner _cloudIot = new CommandLineRunner()
        {
            VoidMain = CloudIotSample.Main,
            Command = "CloudIotSample"
        };

        protected ConsoleOutput Run(params string[] args)
        {
            return _cloudIot.Run(args);
        }

        private readonly RetryRobot _retryRobot = new RetryRobot()
        {
            RetryWhenExceptions = new[] { typeof(Xunit.Sdk.XunitException) }
        };

        private void Eventually(Action action) => _retryRobot.Eventually(action);

        private void CreatePubSubTopic(String name)
        {
            var publisher = PublisherClient.Create();
            TopicName topicName = new TopicName(s_projectId, name);
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
                            Members = { s_serviceAccount }
                        }
                    }
            };
            SetIamPolicyRequest request = new SetIamPolicyRequest
            {
                Resource = $"projects/{s_projectId}/topics/{topicName.TopicId}",
                Policy = policy
            };
            Policy response = publisher.SetIamPolicy(request);
            Console.WriteLine($"Topic IAM Policy updated: {response}");
        }

        private void DeletePubSubTopic(String name)
        {
            PublisherClient publisher = PublisherClient.Create();
            TopicName topicName = new TopicName(s_projectId, name);
            publisher.DeleteTopic(topicName);
        }

        [Fact]
        public void TestCreateDeleteRegistry()
        {
            var registryId = "testcreatereg";
            var topicId = "dotnettest-create";

            // Build up IoT PubSub Topic
            CreatePubSubTopic(topicId);

            var createRegistryOutput = Run("createRegistry", s_projectId, s_regionId, registryId,  topicId);
            Assert.DoesNotContain("A registry with the name", createRegistryOutput.Stdout);

            var deleteRegOutput = Run("deleteRegistry", s_projectId, s_regionId, registryId);
            Assert.DoesNotContain("was not found", deleteRegOutput.Stdout);

            // Tear down IoT PubSub topic
            DeletePubSubTopic(topicId);
        }

        [Fact]
        public void TestListRegistries()
        {
            var registryId = "testlistreg";
            var topicId = "dotnettest-list";

            // Build up IoT PubSub Topic
            CreatePubSubTopic(topicId);
            Run("createRegistry", s_projectId, s_regionId, registryId, topicId);

            var listRegistryOutput = Run("listRegistries", s_projectId, s_regionId);
            Assert.Contains("Registries:", listRegistryOutput.Stdout);

            // Tear down Registry and IoT PubSub topic
            Run("deleteRegistry", s_projectId, s_regionId, registryId);
            DeletePubSubTopic(topicId);
        }

        [Fact]
        public void TestGetRegistry()
        {
            var registryId = "testgetreg";
            var topicId = "dotnettest-list";

            // Build up IoT PubSub Topic
            CreatePubSubTopic(topicId);
            Run("createRegistry", s_projectId, s_regionId, registryId, topicId);

            var getRegistryOutput = Run("getRegistry", s_projectId, s_regionId, registryId);
            Assert.Contains("Registry:", getRegistryOutput.Stdout);

            // Tear down Registry and IoT PubSub topic
            Run("deleteRegistry", s_projectId, s_regionId, registryId);
            DeletePubSubTopic(topicId);
        }

        [Fact]
        public void TestCreateUnauthDevice()
        {
            var topicId = "dotnettest-createunauth";
            var registryId = "testcreatedevice-unauth";
            var deviceId = "dotnettest-unauth";

            // Build up IoT PubSub Topic
            CreatePubSubTopic(topicId);
            Run("createRegistry", s_projectId, s_regionId, registryId, topicId);

            var createUnauthOut = Run("createDeviceNoAuth", s_projectId, s_regionId, registryId, deviceId);
            Assert.Contains("Device created:", createUnauthOut.Stdout);

            var deleteUnauthOut = Run("deleteDevice", s_projectId, s_regionId, registryId, deviceId);
            Assert.Contains("Removed device:", deleteUnauthOut.Stdout);

            // Tear down Registry and IoT PubSub topic
            Run("deleteRegistry", s_projectId, s_regionId, registryId);
            DeletePubSubTopic(topicId);
        }

        [Fact]
        public void TestGetDeviceConfigs()
        {
            var topicId = "dotnettest-createunauth";
            var registryId = "testcreatedevice-unauth";
            var deviceId = "dotnettest-unauth";

            // Build up IoT PubSub Topic, registry, and device
            CreatePubSubTopic(topicId);
            Run("createRegistry", s_projectId, s_regionId, registryId, topicId);
            Run("createDeviceNoAuth", s_projectId, s_regionId, registryId, deviceId);

            var getConfigsOut = Run("getDeviceConfigs", s_projectId, s_regionId, registryId, deviceId);
            Assert.Contains("Configurations:", getConfigsOut.Stdout);

            // Tear down Device, Registry, and IoT PubSub topic
            Run("deleteDevice", s_projectId, s_regionId, registryId, deviceId);
            Run("deleteRegistry", s_projectId, s_regionId, registryId);
            DeletePubSubTopic(topicId);
        }

        [Fact]
        public void TestCreateEsDevice()
        {
            var topicId = "dotnettest-createES";
            var registryId = "testcreatedevice-createES";
            var deviceId = "dotnettest-createES";

            // Build up IoT PubSub Topic
            CreatePubSubTopic(topicId);
            Run("createRegistry", s_projectId, s_regionId, registryId, topicId);

            var createEsOut = Run("createDeviceEs", s_projectId, s_regionId, registryId, deviceId, "test/data/ec_public.pem");
            Assert.Contains("Device created:", createEsOut.Stdout);

            var deleteEsOut = Run("deleteDevice", s_projectId, s_regionId, registryId, deviceId);
            Assert.Contains("Removed device:", deleteEsOut.Stdout);

            // Tear down Registry and IoT PubSub topic
            Run("deleteRegistry", s_projectId, s_regionId, registryId);
            DeletePubSubTopic(topicId);
        }

        [Fact]
        public void TestCreateRsaDevice()
        {
            var topicId = "dotnettest-createRSA";
            var registryId = "testcreatedevice-createRSA";
            var deviceId = "dotnettest-createRSA";

            // Build up IoT PubSub Topic
            CreatePubSubTopic(topicId);
            Run("createRegistry", s_projectId, s_regionId, registryId, topicId);

            var createRsaOut = Run("createDeviceRsa", s_projectId, s_regionId, registryId, deviceId, "test/data/rsa_cert.pem");
            Assert.Contains("Device created:", createRsaOut.Stdout);

            var deleteRsaOut = Run("deleteDevice", s_projectId, s_regionId, registryId, deviceId);
            Assert.Contains("Removed device:", deleteRsaOut.Stdout);

            // Tear down Registry and IoT PubSub topic
            Run("deleteRegistry", s_projectId, s_regionId, registryId);
            DeletePubSubTopic(topicId);
        }

        [Fact]
        public void TestCreatePatchEsDevice()
        {
            var topicId = "dotnettest-createunauth-es";
            var registryId = "testcreatedevice-createunauth-es";
            var deviceId = "dotnettest-unauth-es";

            // Build up IoT PubSub Topic
            CreatePubSubTopic(topicId);
            Run("createRegistry", s_projectId, s_regionId, registryId, topicId);
            Run("createDeviceNoAuth", s_projectId, s_regionId, registryId, deviceId);

            var patchUnauthOut = Run("patchDeviceEs", s_projectId, s_regionId, registryId, deviceId, "test/data/ec_public.pem");
            Assert.Contains("Device patched:", patchUnauthOut.Stdout);

            // Tear down Registry and IoT PubSub topic
            Run("deleteDevice", s_projectId, s_regionId, registryId, deviceId);
            Run("deleteRegistry", s_projectId, s_regionId, registryId);
            DeletePubSubTopic(topicId);
        }

        [Fact]
        public void TestCreatePatchRsaDevice()
        {
            var topicId = "dotnettest-createunauth-rsa";
            var registryId = "testcreatedevice-unauth-rsa";
            var deviceId = "dotnettest-unauth-rsa";

            // Build up IoT PubSub Topic
            CreatePubSubTopic(topicId);
            Run("createRegistry", s_projectId, s_regionId, registryId, topicId);
            Run("createDeviceNoAuth", s_projectId, s_regionId, registryId, deviceId);

            var patchUnauthOut = Run("patchDeviceRsa", s_projectId, s_regionId, registryId, deviceId, "test/data/rsa_cert.pem");
            Assert.Contains("Device patched:", patchUnauthOut.Stdout);

            // Tear down Registry and IoT PubSub topic
            Run("deleteDevice", s_projectId, s_regionId, registryId, deviceId);
            Run("deleteRegistry", s_projectId, s_regionId, registryId);
            DeletePubSubTopic(topicId);
        }

        [Fact]
        public void TestSetDeviceConfig()
        {
            var topicId = "dotnettest-configtest";
            var registryId = "testcreatedevice-config";
            var deviceId = "dotnettest-config";

            // Build up IoT PubSub Topic
            CreatePubSubTopic(topicId);
            Run("createRegistry", s_projectId, s_regionId, registryId, topicId);
            Run("createDeviceNoAuth", s_projectId, s_regionId, registryId, deviceId);

            var setDeviceOut = Run("setDeviceConfig", s_projectId, s_regionId, registryId, deviceId, "test");
            Assert.Contains("Configuration updated to: 2", setDeviceOut.Stdout);

            // Tear down Registry and IoT PubSub topic
            Run("deleteDevice", s_projectId, s_regionId, registryId, deviceId);
            Run("deleteRegistry", s_projectId, s_regionId, registryId);
            DeletePubSubTopic(topicId);
        }

        [Fact]
        public void TestGetSetIamBinding()
        {
            var registryId = "testsetiamreg";
            var topicId = "dotnettest-getsetiam";
            var member = "group:dpebot@google.com";
            var role = "roles/viewer";

            // Build up IoT PubSub Topic
            CreatePubSubTopic(topicId);
            Run("createRegistry", s_projectId, s_regionId, registryId, topicId);

            var setIamOutput = Run("setIamPolicy", s_projectId, s_regionId, registryId, role, member);
            Assert.DoesNotContain("RequestError", setIamOutput.Stdout);

            var getIamOutput = Run("getIamPolicy", s_projectId, s_regionId, registryId);
            System.Diagnostics.Trace.WriteLine(getIamOutput.Stdout);
            Assert.Contains("Role: roles/viewer", getIamOutput.Stdout);

            // Tear down Registry and IoT PubSub topic
            Run("deleteRegistry", s_projectId, s_regionId, registryId);
            DeletePubSubTopic(topicId);
        }

        [Fact]
        public void TestListDevicesNoRegistry()
        {
            var registryId = "notfoundregistry";
            var listDevicesOutput = Run("listDevices", s_projectId, s_regionId, registryId);
            Assert.Contains("A registry with the name", listDevicesOutput.Stdout);
        }

        [Fact]
        public void TestGetDeviceRegistryNotFound()
        {
            var registryId = "notfoundregistry";
            var listDevicesOutput = Run("getRegistry", s_projectId, s_regionId, registryId);
            Assert.Contains("A registry with the name", listDevicesOutput.Stdout);
        }
    }
}
