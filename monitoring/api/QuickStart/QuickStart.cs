/*
 * Copyright (c) 2017 Google Inc.
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

// [START monitoring_quickstart]

using System;
using System.Collections.Generic;
using Google.Cloud.Monitoring.V3;
using Google.Protobuf.WellKnownTypes;
using Google.Api;
using Google.Api.Gax.ResourceNames;

namespace GoogleCloudSamples
{
    public class QuickStart
    {
        public static void Main(string[] args)
        {
            // Your Google Cloud Platform project ID.
            string projectId = "YOUR-PROJECT-ID";

            // Create client.
            MetricServiceClient metricServiceClient = MetricServiceClient.Create();

            // Initialize request argument(s).
            ProjectName name = new ProjectName(projectId);

            // Prepare a data point.
            TypedValue salesTotal = new TypedValue
            {
                DoubleValue = 123.45
            };
            Point dataPoint = new Point
            {
                Value = salesTotal
            };
            // Sets data point's interval end time to current time.
            DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            Timestamp timeStamp = new Timestamp
            {
                Seconds = (long)(DateTime.UtcNow - UnixEpoch).TotalSeconds
            };
            TimeInterval interval = new TimeInterval
            {
                EndTime = timeStamp
            };
            dataPoint.Interval = interval;

            // Prepare custom metric.
            Metric metric = new Metric
            {
                Type = "custom.googleapis.com/my_metric"
            };
            metric.Labels.Add("store_id", "Pittsburgh");

            // Prepare monitored resource.
            MonitoredResource resource = new MonitoredResource
            {
                Type = "gce_instance"
            };
            
            resource.Labels.Add("project_id", projectId);
            resource.Labels.Add("instance_id", "1234567890123456789");
            resource.Labels.Add("zone", "us-central1-f");

            // Create a new time series using inputs.
            TimeSeries timeSeriesData = new TimeSeries
            {
                Metric = metric,
                Resource = resource
            };
            timeSeriesData.Points.Add(dataPoint);

            // Add newly created time series to list of time series to be written.
            IEnumerable<TimeSeries> timeSeries = new List<TimeSeries> { timeSeriesData };
            // Write time series data.
            metricServiceClient.CreateTimeSeries(name, timeSeries);
            Console.WriteLine("Done writing time series data.");
        }
    }
}
// [END monitoring_quickstart]
