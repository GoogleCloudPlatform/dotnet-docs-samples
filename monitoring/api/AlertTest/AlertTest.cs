/**
 * Copyright 2017, Google, Inc.
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *    http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using Google.Cloud.Monitoring.V3;
using Google.Protobuf.WellKnownTypes;
using System;
using Xunit;
using static Google.Cloud.Monitoring.V3.Aggregation.Types;
using static Google.Cloud.Monitoring.V3.AlertPolicy.Types.Condition.Types;
using static Google.Cloud.Monitoring.V3.AlertPolicy.Types;

namespace GoogleCloudSamples
{
    public class AlertTest : IClassFixture<AlertTestFixture>
    {
        readonly CommandLineRunner _alert = new CommandLineRunner()
        {
            Main = AlertProgram.Main,
            Command = "AlertSample"
        };
        private readonly AlertTestFixture _fixture;

        public AlertTest(AlertTestFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public void TestList()
        {
            var result = _alert.Run("list", "-p", _fixture.ProjectId);
            Assert.Equal(0, result.ExitCode);
        }

        [Fact]
        /// Fails due to https://buganizer.corp.google.com/issues/70801404
        public void TestBackupAndRestore()
        {
            var result = _alert.Run("backup", "-p", _fixture.ProjectId);
            Assert.Equal(0, result.ExitCode);
            result = _alert.Run("restore", "-p", _fixture.ProjectId);
            Assert.Equal(0, result.ExitCode);
        }

        [Fact]
        public void TestEnableDisable()
        {
            var result = _alert.Run("enable", "-p", _fixture.ProjectId);
            Assert.Equal(0, result.ExitCode);
            Assert.Contains("enabled", result.Stdout.ToLower());
            result = _alert.Run("disable", "-p", _fixture.ProjectId);
            Assert.Equal(0, result.ExitCode);
            Assert.Contains("disabled", result.Stdout.ToLower());
            result = _alert.Run("enable", "-p", _fixture.ProjectId);
            Assert.Equal(0, result.ExitCode);
            Assert.Contains("enabled", result.Stdout.ToLower());
        }

        [Fact]
        public void TestReplaceChannels()
        {
            var result = _alert.Run("replace-channels", "-p", _fixture.ProjectId,
                "-a", AlertPolicyName.Parse(_fixture.Alert.Name).AlertPolicyId,
                "-c", NotificationChannelName.Parse(_fixture.Channel.Name)
                .NotificationChannelId);
            Assert.Equal(0, result.ExitCode);
        }
    }

    /// <summary>
    /// Creates an AlertPolicy and NotificationChannel for the duration
    /// if the tests.
    /// </summary>
    public class AlertTestFixture : IDisposable
    {
        public AlertTestFixture()
        {
            var channel = new NotificationChannel()
            {
                Type = "email",
                DisplayName = "Email joe.",
                Description = "AlertTest.cs",
                Labels = { { "email_address", "joe@example.com" } },
                UserLabels =
                {
                    { "role", "operations" },
                    { "level", "5" },
                    { "office", "california_westcoast_usa" },
                    { "division", "fulfillment"}
                }
            };
            Channel = NotificationChannelClient.CreateNotificationChannel(
                new ProjectName(ProjectId), channel);

            Alert = AlertPolicyClient.CreateAlertPolicy(
                new ProjectName(ProjectId), new AlertPolicy()
                {
                    DisplayName = "AlertTest.cs",
                    Enabled = false,
                    Combiner = ConditionCombinerType.Or,
                    Conditions =
                {
                    new AlertPolicy.Types.Condition()
                    {
                        ConditionThreshold = new MetricThreshold()
                        {
                            Filter = "metric.label.state=\"blocked\" AND metric.type=\"agent.googleapis.com/processes/count_by_state\"  AND resource.type=\"gce_instance\"",
                            Aggregations = {
                                new Aggregation() {
                                    AlignmentPeriod = Duration.FromTimeSpan(
                                        TimeSpan.FromSeconds(60)),
                                    PerSeriesAligner = Aligner.AlignMean,
                                    CrossSeriesReducer = Reducer.ReduceMean,
                                    GroupByFields = {
                                        "project",
                                        "resource.label.instance_id",
                                        "resource.label.zone"
                                    }
                                }
                            },
                            DenominatorFilter = "",
                            DenominatorAggregations = {},
                            Comparison = ComparisonType.ComparisonGt,
                            ThresholdValue = 100.0,
                            Duration = Duration.FromTimeSpan(
                                TimeSpan.FromSeconds(900)),
                            Trigger = new Trigger() {
                                Count = 1,
                                Percent = 0.0,
                            }
                        },
                        DisplayName = "AlertTest.cs",
                    }
                },
                });
        }

        public NotificationChannel Channel { get; private set; }
        public AlertPolicy Alert { get; private set; }
        public string ProjectId { get; private set; } =
            Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");

        public NotificationChannelServiceClient NotificationChannelClient
        { get; private set; } = NotificationChannelServiceClient.Create();
        public AlertPolicyServiceClient AlertPolicyClient
        { get; private set; } = AlertPolicyServiceClient.Create();


        public void Dispose()
        {
            NotificationChannelClient.DeleteNotificationChannel(
                NotificationChannelName.Parse(Channel.Name), true);
            AlertPolicyClient.DeleteAlertPolicy(
                AlertPolicyName.Parse(Alert.Name));
        }
    }
}
