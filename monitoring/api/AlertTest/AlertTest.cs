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
    public class AlertTest
    {
readonly         string _projectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");

        readonly CommandLineRunner _alert = new CommandLineRunner()
        {
            Main = AlertProgram.Main,
            Command = "AlertSample"
        };
        [Fact]
        public void TestList()
        {
            var result = _alert.Run("list", "-p", _projectId);
            Assert.Equal(0, result.ExitCode);
        }

        [Fact]
        /// Fails due to https://buganizer.corp.google.com/issues/70801404
        public void TestBackupAndRestore()
        {
            var result = _alert.Run("backup", "-p", _projectId);
            Assert.Equal(0, result.ExitCode);
            result = _alert.Run("restore", "-p", _projectId);
            Assert.Equal(0, result.ExitCode);
        }

        [Fact]
        public void TestEnableDisable()
        {
            var result = _alert.Run("enable", "-p", _projectId);
            Assert.Equal(0, result.ExitCode);
            Assert.Contains("enabled", result.Stdout.ToLower());
            result = _alert.Run("disable", "-p", _projectId);
            Assert.Equal(0, result.ExitCode);
            Assert.Contains("disabled", result.Stdout.ToLower());
            result = _alert.Run("enable", "-p", _projectId);
            Assert.Equal(0, result.ExitCode);
            Assert.Contains("enabled", result.Stdout.ToLower());
        }

        /// Create a NotificationChannel for testing, and delete it when
        /// this object is disposed.
        class ScopedChannel : IDisposable
        {
            public NotificationChannel Channel { get; set; }
            public ScopedChannel(NotificationChannelServiceClient client, string projectId)
            {
                _client = client;
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
                Channel = client.CreateNotificationChannel(
                    new ProjectName(projectId), channel);
            }
            public void Dispose()
            {
                var channelName = NotificationChannelName.Parse(Channel.Name);
                _client.DeleteNotificationChannel(channelName, true);
            }

            readonly NotificationChannelServiceClient _client;
        }

        /// Create a NotificationPolicy for testing, and delete it when
        /// this object is disposed.
        class ScopedAlert : IDisposable
        {
            public AlertPolicy Alert { get; set; }
            public ScopedAlert(string projectId)
            {
                _client = AlertPolicyServiceClient.Create();
                Alert = _client.CreateAlertPolicy(new ProjectName(projectId), new AlertPolicy()
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

            public void Dispose()
            {
                _client.DeleteAlertPolicy(AlertPolicyName.Parse(Alert.Name));
            }

            readonly AlertPolicyServiceClient _client;
        }

        [Fact]
        public void TestReplaceChannels()
        {
            NotificationChannelServiceClient notif =
                NotificationChannelServiceClient.Create();

            using (var alert = new ScopedAlert(_projectId))
            using (var channel = new ScopedChannel(notif, _projectId))
            {
                var result = _alert.Run("replace-channels", "-p", _projectId,
                    "-a", AlertPolicyName.Parse(alert.Alert.Name).AlertPolicyId,
                    "-c", NotificationChannelName.Parse(channel.Channel.Name)
                    .NotificationChannelId);
                Assert.Equal(0, result.ExitCode);
            }
        }
    }
}
