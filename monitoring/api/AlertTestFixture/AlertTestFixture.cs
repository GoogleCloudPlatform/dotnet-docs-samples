// Copyright (c) 2018 Google LLC.
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

using Google.Api.Gax.ResourceNames;
using Google.Cloud.Monitoring.V3;
using Google.Protobuf.WellKnownTypes;
using System;
using static Google.Cloud.Monitoring.V3.Aggregation.Types;
using static Google.Cloud.Monitoring.V3.AlertPolicy.Types;
using static Google.Cloud.Monitoring.V3.AlertPolicy.Types.Condition.Types;

/// <summary>
/// Creates an AlertPolicy and NotificationChannel for the duration
/// of the tests.
/// </summary>
public class AlertTestFixture : IDisposable
{
    public NotificationChannel CreateChannel()
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
        return NotificationChannelClient.CreateNotificationChannel(
            new ProjectName(ProjectId), channel);
    }

    public AlertTestFixture()
    {
        Channel = CreateChannel();
        string filter = @"metric.label.state=""blocked"" AND 
            metric.type=""agent.googleapis.com/processes/count_by_state""  
            AND resource.type=""gce_instance""";
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
                            Filter = filter,
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