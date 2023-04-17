// Copyright(c) 2017 Google Inc.
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

using Grpc.Core;
using System;
using System.Linq;
using Xunit;

namespace GoogleCloudSamples
{
    public class TestFixture : IDisposable
    {
        private const string TimeSeriesQuotaError = "One or more TimeSeries could not be written:" +
            " One or more points were written more frequently than the maximum sampling period configured for the metric.";
        private static bool IsTimeSeriesQuotaError(Exception ex) =>
            ex is RpcException rpcEx
            && rpcEx.StatusCode == StatusCode.InvalidArgument
            && rpcEx.Status.Detail.Contains(TimeSeriesQuotaError);

        public static RetryRobot TimeSeriesQuota { get; } = new RetryRobot
        {
            ShouldRetry = ex => IsTimeSeriesQuotaError(ex)
                || (ex is AggregateException aggEx && aggEx.InnerExceptions.Any(innerEx => IsTimeSeriesQuotaError(innerEx))),
            FirstRetryDelayMs = 10_000,
            MaxTryCount = 10
        };

        public string ProjectId { get; private set; }
            = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
        public TestFixture()
        {
            WriteOutput = TimeSeriesQuota.Eventually(() => CloudMonitoring.Run("write", ProjectId));
        }

        public CommandLineRunner CloudMonitoring { get; private set; }
            = new CommandLineRunner()
            {
                VoidMain = Monitoring.Main,
                Command = "Monitoring"
            };

        public ConsoleOutput WriteOutput { get; private set; }

        public void Dispose()
        {
        }
    }
    // <summary>
    /// Runs the sample app's methods and tests the outputs
    // </summary>
    public class CommonTests : IClassFixture<TestFixture>
    {
        private readonly string _projectId;
        readonly CommandLineRunner _cloudMonitoring;
        private readonly TestFixture _fixture;

        public CommonTests(TestFixture fixture)
        {
            _projectId = fixture.ProjectId;
            _cloudMonitoring = fixture.CloudMonitoring;
            _fixture = fixture;
        }

        protected ConsoleOutput Run(params string[] args)
        {
            return _cloudMonitoring.Run(args);
        }

        [Fact]
        public void TestListMetricDescriptors()
        {
            string testMetricName = "compute.googleapis.com/instance/cpu/utilization";
            var output = _cloudMonitoring.Run("list", _projectId);
            Assert.Equal(0, output.ExitCode);
            Assert.Contains(testMetricName, output.Stdout);
        }

        [Fact]
        public void TestCreateCustomMetric()
        {
            var output = _cloudMonitoring.Run("create", _projectId);
            Assert.Equal(0, output.ExitCode);
            Assert.Contains("metricKind", output.Stdout);
        }

        [Fact]
        public void TestGetMetricDescriptor()
        {
            _cloudMonitoring.Run("create", _projectId);
            var output = _cloudMonitoring.Run("get", _projectId,
                "custom.googleapis.com/stores/daily_sales");
            Assert.Equal(0, output.ExitCode);
            Assert.Contains("metricKind", output.Stdout);
        }

        [Fact]
        public void TestWriteTimeSeriesData()
        {
            Assert.Equal(0, _fixture.WriteOutput.ExitCode);
            Assert.Contains("Pittsburgh", _fixture.WriteOutput.Stdout);
        }

        [Fact]
        public void TestListMonitoredResourceDescriptors()
        {
            string testResourceName = $"projects/{_projectId}/monitoredResourceDescriptors/";
            var output = _cloudMonitoring.Run("listResources", _projectId);
            Assert.Equal(0, output.ExitCode);
            Assert.Contains(testResourceName, output.Stdout);
        }

        [Fact]
        public void TestGetMonitoredResourceDescriptor()
        {
            string testResourceDisplayName = "Produced API";
            var output = _cloudMonitoring.Run("getResource", _projectId, "api");
            Assert.Equal(0, output.ExitCode);
            Assert.Contains(testResourceDisplayName, output.Stdout);
        }

        [Fact]
        public void TestReadTimeSeriesData()
        {
            var output = _cloudMonitoring.Run("read", _projectId,
                "custom.googleapis.com/stores/daily_sales");
            Assert.Equal(0, output.ExitCode);
            Assert.Contains("123.45", output.Stdout);
        }

        [Fact]
        public void TestReadTimeSeriesDataFields()
        {
            var output = _cloudMonitoring.Run("readFields", _projectId,
                "custom.googleapis.com/stores/daily_sales");
            Assert.Equal(0, output.ExitCode);
            Assert.DoesNotContain("123.45", output.Stdout);
            Assert.Contains("Pittsburgh", output.Stdout);
        }

        [Fact]
        public void TestReadTimeSeriesDataAggregated()
        {
            var output = _cloudMonitoring.Run("readAggregate", _projectId);
            Assert.Equal(0, output.ExitCode);
            Assert.Contains("Now:", output.Stdout);
            Assert.Contains("10 min ago:", output.Stdout);
        }

        [Fact]
        public void TestReadTimeSeriesDataReduced()
        {
            var output = _cloudMonitoring.Run("readReduce", _projectId);
            Assert.Equal(0, output.ExitCode);
            Assert.Contains("Last 10 min:", output.Stdout);
            Assert.Contains("Last 10-20 min ago:", output.Stdout);
        }

        [Fact]
        public void TestDeleteMetricDescriptor()
        {
            string randomNameSuffix = TestUtil.RandomName();
            string metricType = "custom.googleapis.com/stores/daily_sales" + randomNameSuffix;
            // Create Metric Descriptor.
            var output = _cloudMonitoring.Run("create", _projectId, metricType);
            // Confirm Metric Descriptor is created.
            Assert.Equal(0, output.ExitCode);
            // Get Metric Descriptor.
            var outputFromGet = _cloudMonitoring.Run("get", _projectId, metricType);
            Assert.Equal(0, outputFromGet.ExitCode);
            // Delete Metric Descriptor.
            var outputFromDelete = _cloudMonitoring.Run("delete", _projectId, metricType);
            Assert.Equal(0, outputFromDelete.ExitCode);
        }
    }

    public class QuickStartTests
    {
        readonly CommandLineRunner _quickStart = new CommandLineRunner()
        {
            VoidMain = QuickStart.Main,
            Command = "dotnet run"
        };

        [Fact]
        public void TestRun()
        { 

            var output = TestFixture.TimeSeriesQuota.Eventually(() => _quickStart.Run());
            Assert.Equal(0, output.ExitCode);
        }
    }
}
