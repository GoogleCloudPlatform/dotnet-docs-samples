/*
 * Copyright (c) 2018 Google Inc.
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
using Google.Api.Gax.Grpc;
using Google.Api.Gax.ResourceNames;
using Google.Cloud.Monitoring.V3;
using Google.Protobuf.WellKnownTypes;
using System;

namespace GoogleCloudSamples
{
    class OptionsWithProjectId
    {
        [Option('p', HelpText = "The project ID of the project to use for monitoring operations.", Required = true)]
        public string ProjectId { get; set; }
    }

    [Verb("create", HelpText = "Create an uptime check.")]
    class CreateOptions : OptionsWithProjectId
    {
        [Option('h', HelpText = "Host name.")]
        public string HostName { get; set; } = "example.com";
        [Option('d', HelpText = "Display name.")]
        public string DisplayName { get; set; } = "New uptime check";
    }

    [Verb("update", HelpText = "Update an uptime check.")]
    class UpdateOptions : OptionsWithConfigName
    {
        [Option('h', HelpText = "The new http path to check.")]
        public string HttpPath { get; set; }

        [Option('d', HelpText = "The new display name.")]
        public string DisplayName { get; set; }
    }

    class OptionsWithConfigName
    {
        [Value('0', HelpText = "The full name of the config to delete. " +
            "Example: projects/my-project/uptimeCheckConfigs/my-config-name",
            Required = true)]
        public string ConfigName { get; set; }
    }

    [Verb("delete", HelpText = "Delete an uptime check.")]
    class DeleteOptions : OptionsWithConfigName
    {
    }

    [Verb("list", HelpText = "List metric descriptors for this project.")]
    class ListOptions : OptionsWithProjectId
    {
    }

    [Verb("list-ips", HelpText = "List IP addresses of uptime-check servers.")]
    class ListIpsOptions
    {
    }

    [Verb("get", HelpText = "Get details for an uptime check.")]
    class GetOptions : OptionsWithConfigName
    {
    }

    public class UptimeCheck
    {
        // [START monitoring_uptime_check_create]
        public static object CreateUptimeCheck(string projectId, string hostName,
            string displayName)
        {
            // Define a new config.
            var config = new UptimeCheckConfig()
            {
                DisplayName = displayName,
                MonitoredResource = new MonitoredResource()
                {
                    Type = "uptime_url",
                    Labels = { { "host", hostName } }
                },
                HttpCheck = new UptimeCheckConfig.Types.HttpCheck()
                {
                    Path = "/",
                    Port = 80,
                },
                Timeout = TimeSpan.FromSeconds(10).ToDuration(),
                Period = TimeSpan.FromMinutes(5).ToDuration()
            };
            // Create a client.
            var client = UptimeCheckServiceClient.Create();
            ProjectName projectName = new ProjectName(projectId);
            // Create the config.
            var newConfig = client.CreateUptimeCheckConfig(
                projectName,
                config,
                CallSettings.FromExpiration(
                    Expiration.FromTimeout(
                        TimeSpan.FromMinutes(2))));
            Console.WriteLine(newConfig.Name);
            return 0;
        }
        // [END monitoring_uptime_check_create]

        // [START monitoring_uptime_check_delete]
        public static object DeleteUptimeCheckConfig(string configName)
        {
            var client = UptimeCheckServiceClient.Create();
            client.DeleteUptimeCheckConfig(configName);
            Console.WriteLine($"Deleted {configName}");
            return 0;
        }
        // [END monitoring_uptime_check_delete]


        // [START monitoring_uptime_check_list_configs]
        public static object ListUptimeCheckConfigs(string projectId)
        {
            var client = UptimeCheckServiceClient.Create();
            var configs = client.ListUptimeCheckConfigs(new ProjectName(projectId));
            foreach (UptimeCheckConfig config in configs)
            {
                Console.WriteLine(config.Name);
            }
            return 0;
        }
        // [END monitoring_uptime_check_list_configs]

        // [START monitoring_uptime_check_list_ips]
        public static object ListUptimeCheckIps()
        {
            var client = UptimeCheckServiceClient.Create();
            var ips = client.ListUptimeCheckIps(new ListUptimeCheckIpsRequest());
            foreach (UptimeCheckIp ip in ips)
            {
                Console.WriteLine("{0,20} {1}", ip.IpAddress, ip.Location);
            }
            return 0;
        }
        // [END monitoring_uptime_check_list_ips]

        // [START monitoring_uptime_check_update]
        public static object UpdateUptimeCheck(string configName,
            string newHttpPath, string newDisplayName)
        {
            var client = UptimeCheckServiceClient.Create();
            var config = client.GetUptimeCheckConfig(configName);
            var fieldMask = new FieldMask();
            if (newDisplayName != null)
            {
                config.DisplayName = newDisplayName;
                fieldMask.Paths.Add("display_name");
            }
            if (newHttpPath != null)
            {
                config.HttpCheck.Path = newHttpPath;
                fieldMask.Paths.Add("http_check.path");
            }
            client.UpdateUptimeCheckConfig(config);
            return 0;
        }
        // [END monitoring_uptime_check_update]

        // [START monitoring_uptime_check_get]
        public static object GetUptimeCheckConfig(string configName)
        {
            var client = UptimeCheckServiceClient.Create();
            UptimeCheckConfig config = client.GetUptimeCheckConfig(configName);
            if (config == null)
            {
                Console.Error.WriteLine(
                    "No configuration found with the name {0}", configName);
                return -1;
            }
            Console.WriteLine("Name: {0}", config.Name);
            Console.WriteLine("Display Name: {0}", config.DisplayName);
            Console.WriteLine("Http Path: {0}", config.HttpCheck.Path);
            return 0;
        }
        // [END monitoring_uptime_check_get]

        public static int Main(string[] args)
        {
            var verbMap = new VerbMap<int>();
            verbMap
                .Add((CreateOptions opts) => CreateUptimeCheck(opts.ProjectId,
                        opts.HostName, opts.DisplayName))
                .Add((DeleteOptions opts) => DeleteUptimeCheckConfig(opts.ConfigName))
                .Add((ListOptions opts) => ListUptimeCheckConfigs(opts.ProjectId))
                .Add((ListIpsOptions opts) => ListUptimeCheckIps())
                .Add((GetOptions opts) => GetUptimeCheckConfig(opts.ConfigName))
                .Add((UpdateOptions opts) => UpdateUptimeCheck(opts.ConfigName, opts.HttpPath, opts.DisplayName))
                .NotParsedFunc = (err) => 255;
            return verbMap.Run(args);
        }
    }
}
