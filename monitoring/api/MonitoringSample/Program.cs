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

using CommandLine;
using Google.Api;
using Google.Api.Gax;
using Google.Api.Gax.ResourceNames;
using Google.Cloud.Monitoring.V3;
using Google.Protobuf.WellKnownTypes;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using static Google.Api.MetricDescriptor.Types;

namespace GoogleCloudSamples
{
    [Verb("create", HelpText = "Create a metric descriptor for this project.")]
    class CreateOptions
    {
        [Value(0, HelpText = "The project ID of the project to use for monitoring operations.", Required = true)]
        public string projectId { get; set; }
        [Value(1, HelpText = "The metric type of the metric to create.", Required = false,
            Default = "custom.googleapis.com/stores/daily_sales")]
        public string metricType { get; set; }
    }

    [Verb("list", HelpText = "List metric descriptors for this project.")]
    class ListOptions : CreateAndListOptions { }

    [Verb("listGroups", HelpText = "List metric groups for this project.")]
    class ListGroupsOptions : CreateAndListOptions { }

    [Verb("listResources", HelpText = "List monitored resources for this project.")]
    class ListMonitoredResourcesOptions : CreateAndListOptions { }

    class CreateAndListOptions
    {
        [Value(0, HelpText = "The project ID of the project to use for monitoring operations.", Required = true)]
        public string projectId { get; set; }
    }

    [Verb("get", HelpText = "Get a metric descriptor's details.")]
    class GetOptions
    {
        [Value(0, HelpText = "The project ID of the project to use for monitoring operations.", Required = true)]
        public string projectId { get; set; }
        [Value(1, HelpText = "The metric descriptor type of the metric to display details for.", Required = true)]
        public string metricType { get; set; }
    }

    [Verb("getResource", HelpText = "Get a monitored resource descriptor.")]
    class GetMonitoredResourceOptions
    {
        [Value(0, HelpText = "The project ID of the project to use for monitoring operations.", Required = true)]
        public string projectId { get; set; }
        [Value(1, HelpText = "The resource descriptor ID of the metric to display details for.", Required = true)]
        public string resourceId { get; set; }
    }

    [Verb("write", HelpText = "Write TimeSeries data to custom metric.")]
    class WriteTimeSeriesDataOptions
    {
        [Value(0, HelpText = "The project ID of the project to use for monitoring operations.", Required = true)]
        public string projectId { get; set; }
    }

    [Verb("read", HelpText = "Read TimeSeries data from a metric.")]
    class ReadTimeSeriesDataOptions
    {
        [Value(0, HelpText = "The project ID of the project to use for monitoring operations.", Required = true)]
        public string projectId { get; set; }
        [Value(1, HelpText = "The metric type of the metric to read timeseries data from.", Required = false,
            Default = "compute.googleapis.com/instance/cpu/utilization")]
        public string metricType { get; set; }
    }

    [Verb("readFields", HelpText = "Reads headers of time series data.")]
    class ReadTimeSeriesFieldsOptions
    {
        [Value(0, HelpText = "The project ID of the project to use for monitoring operations.", Required = true)]
        public string projectId { get; set; }
        [Value(1, HelpText = "The metric type of the metric to read timeseries data from.", Required = false, Default = "compute.googleapis.com/instance/cpu/utilization")]
        public string metricType { get; set; }
    }

    [Verb("readAggregate", HelpText = "Aggregates time series data that matches the specified metric type.")]
    class ReadTimeSeriesAggregateOptions
    {
        [Value(0, HelpText = "The project ID of the project to use for monitoring operations.", Required = true)]
        public string projectId { get; set; }
        [Value(1, HelpText = "The metric type of the metric to read time series data from.", Required = false, Default = "compute.googleapis.com/instance/cpu/utilization")]
        public string metricType { get; set; }
    }

    [Verb("readReduce", HelpText = "Reduces time series data that matches the specified metric type.")]
    class ReadTimeSeriesReduceOptions
    {
        [Value(0, HelpText = "The project ID of the project to use for monitoring operations.", Required = true)]
        public string projectId { get; set; }
        [Value(1, HelpText = "The metric type of the metric to read time series data from.", Required = false, Default = "compute.googleapis.com/instance/cpu/utilization")]
        public string metricType { get; set; }
    }



    [Verb("delete", HelpText = "Delete a metric descriptor for this project.")]
    class DeleteOptions
    {
        [Value(0, HelpText = "The project ID of the project containing the metric descriptor to delete.", Required = true)]
        public string projectId { get; set; }
        [Value(1, HelpText = "The metric descriptor type of the metric to delete.", Required = true)]
        public string metricType { get; set; }
    }

    public class Monitoring
    {
        private static readonly DateTime s_unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        // [START monitoring_create_metric]
        public static object CreateMetric(string projectId,
            string metricType = "custom.googleapis.com/stores/daily_sales")
        {
            // Create client.
            MetricServiceClient metricServiceClient = MetricServiceClient.Create();

            // Prepare custom metric descriptor.      
            MetricDescriptor metricDescriptor = new MetricDescriptor();
            metricDescriptor.DisplayName = "Daily Sales";
            metricDescriptor.Description = "Daily sales records from all branch stores.";
            metricDescriptor.MetricKind = MetricKind.Gauge;
            metricDescriptor.ValueType = MetricDescriptor.Types.ValueType.Double;
            metricDescriptor.Type = metricType;
            metricDescriptor.Unit = "{USD}";
            LabelDescriptor labels = new LabelDescriptor();
            labels.Key = "store_id";
            labels.ValueType = LabelDescriptor.Types.ValueType.String;
            labels.Description = "The ID of the store.";
            metricDescriptor.Labels.Add(labels);
            CreateMetricDescriptorRequest request = new CreateMetricDescriptorRequest
            {
                ProjectName = new ProjectName(projectId),
            };
            request.MetricDescriptor = metricDescriptor;
            // Make the request.
            MetricDescriptor response = metricServiceClient.CreateMetricDescriptor(request);
            Console.WriteLine("Done creating metric descriptor:");
            Console.WriteLine(JObject.Parse($"{response}").ToString());
            return 0;
        }
        // [END monitoring_create_metric]


        // [START monitoring_delete_metric]
        public static object DeleteMetric(string projectId, string metricType)
        {
            // Create client.
            MetricServiceClient metricServiceClient = MetricServiceClient.Create();
            // Initialize request argument(s).
            MetricDescriptorName name = new MetricDescriptorName(projectId, metricType);
            // Make the request.
            metricServiceClient.DeleteMetricDescriptor(name);
            Console.WriteLine($"Done deleting metric descriptor: {name}");
            return 0;
        }
        // [END monitoring_delete_metric]

        // [START monitoring_list_descriptors]
        public static object ListMetrics(string projectId)
        {
            MetricServiceClient client = MetricServiceClient.Create();
            ProjectName projectName = new ProjectName(projectId);
            PagedEnumerable<ListMetricDescriptorsResponse, MetricDescriptor> metrics = client.ListMetricDescriptors(projectName);
            foreach (MetricDescriptor metric in metrics)
            {
                Console.WriteLine($"{metric.Name}: {metric.DisplayName}");
            }
            return 0;
        }
        // [END monitoring_list_descriptors]

        // [START monitoring_get_descriptor]
        public static object GetMetricDetails(string projectId, string metricType)
        {
            MetricServiceClient client = MetricServiceClient.Create();
            MetricDescriptorName name = new MetricDescriptorName(projectId, metricType);
            try
            {
                var response = client.GetMetricDescriptor(name);
                string metric = JObject.Parse($"{response}").ToString();
                Console.WriteLine($"{ metric }");
            }
            catch (Grpc.Core.RpcException ex)
                when (ex.Status.StatusCode == Grpc.Core.StatusCode.NotFound)
            { }
            return 0;
        }
        // [END monitoring_get_descriptor]

        // [START monitoring_write_timeseries]
        public static object WriteTimeSeriesData(string projectId)
        {
            // Create client.
            MetricServiceClient metricServiceClient = MetricServiceClient.Create();
            // Initialize request argument(s).
            ProjectName name = new ProjectName(projectId);
            // Prepare a data point. 
            Point dataPoint = new Point();
            TypedValue salesTotal = new TypedValue();
            salesTotal.DoubleValue = 123.45;
            dataPoint.Value = salesTotal;
            Timestamp timeStamp = new Timestamp();
            timeStamp.Seconds = (long)(DateTime.UtcNow - s_unixEpoch).TotalSeconds;
            TimeInterval interval = new TimeInterval();
            interval.EndTime = timeStamp;
            dataPoint.Interval = interval;

            // Prepare custom metric.
            Metric metric = new Metric();
            metric.Type = "custom.googleapis.com/stores/daily_sales";
            metric.Labels.Add("store_id", "Pittsburgh");

            // Prepare monitored resource.
            MonitoredResource resource = new MonitoredResource();
            resource.Type = "global";
            resource.Labels.Add("project_id", projectId);

            // Create a new time series using inputs.
            TimeSeries timeSeriesData = new TimeSeries();
            timeSeriesData.Metric = metric;
            timeSeriesData.Resource = resource;
            timeSeriesData.Points.Add(dataPoint);

            // Add newly created time series to list of time series to be written.
            IEnumerable<TimeSeries> timeSeries = new List<TimeSeries> { timeSeriesData };
            // Write time series data.
            metricServiceClient.CreateTimeSeries(name, timeSeries);
            Console.WriteLine("Done writing time series data:");
            Console.WriteLine(JObject.Parse($"{timeSeriesData}").ToString());
            return 0;
        }
        // [END monitoring_write_timeseries]

        // [START monitoring_read_timeseries_simple]
        public static object ReadTimeSeriesData(string projectId,
            string metricType = "compute.googleapis.com/instance/cpu/utilization")
        {
            // Create client.
            MetricServiceClient metricServiceClient = MetricServiceClient.Create();
            // Initialize request argument(s).
            string filter = $"metric.type=\"{metricType}\"";
            ListTimeSeriesRequest request = new ListTimeSeriesRequest
            {
                ProjectName = new ProjectName(projectId),
                Filter = filter,
                Interval = new TimeInterval(),
                View = ListTimeSeriesRequest.Types.TimeSeriesView.Full,
            };
            // Create timestamp for current time formatted in seconds.
            long timeStamp = (long)(DateTime.UtcNow - s_unixEpoch).TotalSeconds;
            Timestamp startTimeStamp = new Timestamp();
            // Set startTime to limit results to the last 20 minutes.
            startTimeStamp.Seconds = timeStamp - (60 * 20);
            Timestamp endTimeStamp = new Timestamp();
            // Set endTime to current time.
            endTimeStamp.Seconds = timeStamp;
            TimeInterval interval = new TimeInterval();
            interval.StartTime = startTimeStamp;
            interval.EndTime = endTimeStamp;
            request.Interval = interval;
            // Make the request.
            PagedEnumerable<ListTimeSeriesResponse, TimeSeries> response =
                metricServiceClient.ListTimeSeries(request);
            // Iterate over all response items, lazily performing RPCs as required.
            foreach (TimeSeries item in response)
            {
                Console.WriteLine(JObject.Parse($"{item}").ToString());
            }
            return 0;
        }
        // [END monitoring_read_timeseries_simple]

        // [START monitoring_read_timeseries_fields]
        public static object ReadTimeSeriesFields(string projectId,
            string metricType = "compute.googleapis.com/instance/cpu/utilization")
        {
            Console.WriteLine($"metricType{ metricType}");
            // Create client.
            MetricServiceClient metricServiceClient = MetricServiceClient.Create();
            // Initialize request argument(s).
            string filter = $"metric.type=\"{metricType}\"";
            ListTimeSeriesRequest request = new ListTimeSeriesRequest
            {
                ProjectName = new ProjectName(projectId),
                Filter = filter,
                Interval = new TimeInterval(),
                View = ListTimeSeriesRequest.Types.TimeSeriesView.Headers,
            };
            // Create timestamp for current time formatted in seconds.
            long timeStamp = (long)(DateTime.UtcNow - s_unixEpoch).TotalSeconds;
            Timestamp startTimeStamp = new Timestamp();
            // Set startTime to limit results to the last 20 minutes.
            startTimeStamp.Seconds = timeStamp - (60 * 20);
            Timestamp endTimeStamp = new Timestamp();
            // Set endTime to current time.
            endTimeStamp.Seconds = timeStamp;
            TimeInterval interval = new TimeInterval();
            interval.StartTime = startTimeStamp;
            interval.EndTime = endTimeStamp;
            request.Interval = interval;
            // Make the request.
            PagedEnumerable<ListTimeSeriesResponse, TimeSeries> response =
                metricServiceClient.ListTimeSeries(request);
            // Iterate over all response items, lazily performing RPCs as required.
            Console.Write("Found data points for the following instances:");
            foreach (var item in response)
            {
                Console.WriteLine(JObject.Parse($"{item}").ToString());
            }
            return 0;
        }
        // [END monitoring_read_timeseries_fields]

        // [START monitoring_read_timeseries_align]
        public static object ReadTimeSeriesAggregate(string projectId,
            string metricType = "compute.googleapis.com/instance/cpu/utilization")
        {
            // Create client.
            MetricServiceClient metricServiceClient = MetricServiceClient.Create();
            // Initialize request argument(s).
            string filter = $"metric.type=\"{metricType}\"";
            ListTimeSeriesRequest request = new ListTimeSeriesRequest
            {
                ProjectName = new ProjectName(projectId),
                Filter = filter,
                Interval = new TimeInterval(),
            };
            // Create timestamp for current time formatted in seconds.
            long timeStamp = (long)(DateTime.UtcNow - s_unixEpoch).TotalSeconds;
            Timestamp startTimeStamp = new Timestamp();
            // Set startTime to limit results to the last 20 minutes.
            startTimeStamp.Seconds = timeStamp - (60 * 20);
            Timestamp endTimeStamp = new Timestamp();
            // Set endTime to current time.
            endTimeStamp.Seconds = timeStamp;
            TimeInterval interval = new TimeInterval();
            interval.StartTime = startTimeStamp;
            interval.EndTime = endTimeStamp;
            request.Interval = interval;
            // Aggregate results per matching instance
            Aggregation aggregation = new Aggregation();
            Duration alignmentPeriod = new Duration();
            alignmentPeriod.Seconds = 600;
            aggregation.AlignmentPeriod = alignmentPeriod;
            aggregation.PerSeriesAligner = Aggregation.Types.Aligner.AlignMean;
            // Add the aggregation to the request.
            request.Aggregation = aggregation;
            // Make the request.
            PagedEnumerable<ListTimeSeriesResponse, TimeSeries> response =
                metricServiceClient.ListTimeSeries(request);
            // Iterate over all response items, lazily performing RPCs as required.
            Console.WriteLine($"{projectId} CPU utilization:");
            foreach (var item in response)
            {
                var points = item.Points;
                var labels = item.Metric.Labels;
                Console.WriteLine($"{labels.Values.FirstOrDefault()}");
                if (points.Count > 0)
                {
                    Console.WriteLine($"  Now: {points[0].Value.DoubleValue}");
                }
                if (points.Count > 1)
                {
                    Console.WriteLine($"  10 min ago: {points[1].Value.DoubleValue}");
                }
            }
            return 0;
        }
        // [END monitoring_read_timeseries_align]

        // [START monitoring_read_timeseries_reduce]
        public static object ReadTimeSeriesReduce(string projectId,
            string metricType = "compute.googleapis.com/instance/cpu/utilization")
        {
            // Create client.
            MetricServiceClient metricServiceClient = MetricServiceClient.Create();
            // Initialize request argument(s).
            string filter = $"metric.type=\"{metricType}\"";
            ListTimeSeriesRequest request = new ListTimeSeriesRequest
            {
                ProjectName = new ProjectName(projectId),
                Filter = filter,
                Interval = new TimeInterval(),
            };
            // Create timestamp for current time formatted in seconds.
            long timeStamp = (long)(DateTime.UtcNow - s_unixEpoch).TotalSeconds;
            Timestamp startTimeStamp = new Timestamp();
            // Set startTime to limit results to the last 20 minutes.
            startTimeStamp.Seconds = timeStamp - (60 * 20);
            Timestamp endTimeStamp = new Timestamp();
            // Set endTime to current time.
            endTimeStamp.Seconds = timeStamp;
            TimeInterval interval = new TimeInterval();
            interval.StartTime = startTimeStamp;
            interval.EndTime = endTimeStamp;
            request.Interval = interval;
            // Aggregate results per matching instance.
            Aggregation aggregation = new Aggregation();
            Duration alignmentPeriod = new Duration();
            alignmentPeriod.Seconds = 600;
            aggregation.AlignmentPeriod = alignmentPeriod;
            aggregation.CrossSeriesReducer = Aggregation.Types.Reducer.ReduceMean;
            aggregation.PerSeriesAligner = Aggregation.Types.Aligner.AlignMean;
            // Add the aggregation to the request.
            request.Aggregation = aggregation;
            // Make the request.
            PagedEnumerable<ListTimeSeriesResponse, TimeSeries> response =
                metricServiceClient.ListTimeSeries(request);
            // Iterate over all response items, lazily performing RPCs as required.
            Console.WriteLine("CPU utilization:");
            foreach (var item in response)
            {
                var points = item.Points;
                Console.WriteLine("Average CPU utilization across all GCE instances:");
                Console.WriteLine($"  Last 10 min: {points[0].Value.DoubleValue}");
                Console.WriteLine($"  Last 10-20 min ago: {points[1].Value.DoubleValue}");
            }
            return 0;
        }
        // [END monitoring_read_timeseries_reduce]

        // [START monitoring_list_resources]
        public static object ListMonitoredResources(string projectId)
        {
            Console.WriteLine("Starting to List Monitored Resources...");
            MetricServiceClient client = MetricServiceClient.Create();
            ProjectName projectName = new ProjectName(projectId);

            PagedEnumerable<ListMonitoredResourceDescriptorsResponse, MonitoredResourceDescriptor>
                resources = client.ListMonitoredResourceDescriptors(projectName);
            if (resources.Any())
            {
                foreach (MonitoredResourceDescriptor resource in resources.Take(10))
                {
                    Console.WriteLine($"{resource.Name}: {resource.DisplayName}");
                }
            }
            else
            { 
                Console.WriteLine("No resources found.");
            }
            return 0;
        }
        // [END monitoring_list_resources]

        // [START monitoring_get_resource]
        public static object GetMonitoredResource(string projectId, string resourceId)
        {
            MetricServiceClient client = MetricServiceClient.Create();
            MonitoredResourceDescriptorName name = new MonitoredResourceDescriptorName(projectId, resourceId);
            var response = client.GetMonitoredResourceDescriptor(name);
            string resource = JObject.Parse($"{response}").ToString();
            Console.WriteLine($"{ resource }");
            return 0;
        }
        // [END monitoring_get_resource]

        public static object ListGroups(string projectId)
        {
            GroupServiceClient client = GroupServiceClient.Create();
            ProjectName projectName = new ProjectName(projectId);
            ListGroupsRequest request = new ListGroupsRequest { Name = projectName.ToString() };
            PagedEnumerable<ListGroupsResponse, Group> groups = client.ListGroups(request);
            foreach (Group group in groups.Take(10))
            {
                Console.WriteLine($"{group.Name}: {group.DisplayName}");
            }
            return 0;
        }

        public static void Main(string[] args)
        {
            Parser.Default.ParseArguments<
                CreateOptions, ListOptions, GetOptions,
                WriteTimeSeriesDataOptions,
                ReadTimeSeriesDataOptions, ReadTimeSeriesFieldsOptions,
                ReadTimeSeriesAggregateOptions, ReadTimeSeriesReduceOptions,
                DeleteOptions, ListMonitoredResourcesOptions,
                GetMonitoredResourceOptions, ListGroupsOptions
                >(args)
              .MapResult(
                (CreateOptions opts) => CreateMetric(opts.projectId, opts.metricType),
                (ListOptions opts) => ListMetrics(opts.projectId),
                (GetOptions opts) => GetMetricDetails(opts.projectId, opts.metricType),
                (WriteTimeSeriesDataOptions opts) => WriteTimeSeriesData(opts.projectId),
                (ReadTimeSeriesDataOptions opts) => ReadTimeSeriesData(opts.projectId, opts.metricType),
                (ReadTimeSeriesFieldsOptions opts) => ReadTimeSeriesFields(opts.projectId, opts.metricType),
                (ReadTimeSeriesAggregateOptions opts) => ReadTimeSeriesAggregate(opts.projectId, opts.metricType),
                (ReadTimeSeriesReduceOptions opts) => ReadTimeSeriesReduce(opts.projectId, opts.metricType),
                (DeleteOptions opts) => DeleteMetric(opts.projectId, opts.metricType),
                (ListMonitoredResourcesOptions opts) => ListMonitoredResources(opts.projectId),
                (GetMonitoredResourceOptions opts) => GetMonitoredResource(opts.projectId, opts.resourceId),
                (ListGroupsOptions opts) => ListGroups(opts.projectId),
                errs => 1);
        }
    }
}
