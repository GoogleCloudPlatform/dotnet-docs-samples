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

/**
 * This application demonstrates how to perform basic operations on alerting
 * policies with the Google Stackdriver Monitoring API.
 *
 * For more information, see https://cloud.google.com/monitoring/docs/.
 */

using CommandLine;
using Google.Api.Gax.ResourceNames;
using Google.Cloud.Monitoring.V3;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.IO;

namespace GoogleCloudSamples
{
    class OptionsBase
    {
        [Option('p', "projectid", Required = true, HelpText = "Your Google project id.")]
        public string ProjectId { get; set; }
    };

    [Verb("delete", HelpText = "Delete alert policy")]
    class DeleteAlertPolicyOptions
    {
        [Option('m', "policyname", HelpText = "The name of the policy to delete.", Required = true)]
        public string PolicyName { get; set; }
    }

    [Verb("delete-channel", HelpText = "Delete alert policy")]
    class DeleteNotificationChannelOptions
    {
        [Option('m', "channelname", HelpText = "The name of the channel to delete.", Required = true)]
        public string ChannelName { get; set; }
        [Option('f', "force", Required = false)]
        public bool Force { get; set; } = false;
    }

    [Verb("list", HelpText = "List alert policies.")]
    class ListPoliciesOptions : OptionsBase { };

    [Verb("list-channels", HelpText = "List notification channels.")]
    class ListChannelsOptions : OptionsBase { };

    [Verb("backup", HelpText = "Save the current list of alert policies to a .json file.")]
    class BackupPoliciesOptions : OptionsBase
    {
        [Option('j', "jsonPath", HelpText = "Path to json file where alert polices are saved and restored.")]
        public string OutputFilePath { get; set; } = "policies-backup.json";
    };

    [Verb("restore", HelpText = "Restore the list of alert policies from a .json file.")]
    class RestorePoliciesOptions : BackupPoliciesOptions { };

    [Verb("replace-channels", HelpText = "Set the list of channel for an alert policy.")]
    class ReplaceChannelsOptions : OptionsBase
    {
        [Option('a', "alertid", Required = true,
            HelpText = "The id of the alert policy whose channels will be replaced.")]
        public string AlertId { get; set; }

        [Option('c', "channelid", Required = true,
            HelpText = "A channel id.  Repeat this option to set multiple channel ids.")]
        public IEnumerable<string> ChannelId { get; set; }
    };

    [Verb("enable", HelpText = "Enable alert policies.")]
    class EnablePoliciesOptions : OptionsBase
    {
        [Option('e', "filter",
            HelpText = "See https://cloud.google.com/monitoring/api/v3/filters")]
        public string Filter { get; set; } = "";
    };

    [Verb("disable", HelpText = "Disable alert policies.")]
    class DisablePoliciesOptions : EnablePoliciesOptions { };


    public class AlertProgram
    {
        public static int Main(string[] args)
        {
            var verbMap = new VerbMap<int>()
                .Add((ListPoliciesOptions opts) => ListAlertPolicies(opts.ProjectId))
                .Add((BackupPoliciesOptions opts) => BackupPolicies(opts.ProjectId, opts.OutputFilePath))
                .Add((RestorePoliciesOptions opts) => RestorePolicies(opts.ProjectId, opts.OutputFilePath))
                .Add((ReplaceChannelsOptions opts) => ReplaceChannels(opts.ProjectId, opts.AlertId, opts.ChannelId))
                .Add((EnablePoliciesOptions opts) => EnablePolicies(opts.ProjectId, opts.Filter, true))
                .Add((DisablePoliciesOptions opts) => EnablePolicies(opts.ProjectId, opts.Filter, false))
                .Add((DeleteAlertPolicyOptions opts) => DeleteAlertPolicy(opts.PolicyName))
                .Add((ListChannelsOptions opts) => ListNotificationChannels(opts.ProjectId))
                .Add((DeleteNotificationChannelOptions opts) => DeleteNotificationChannel(opts.ChannelName, opts.Force))
                .SetNotParsedFunc((errs) => 1);
            return verbMap.Run(args);
        }

        // [START monitoring_alert_list_policies]
        static void ListAlertPolicies(string projectId)
        {
            var client = AlertPolicyServiceClient.Create();
            var response = client.ListAlertPolicies(new ProjectName(projectId));
            foreach (AlertPolicy policy in response)
            {
                Console.WriteLine(policy.Name);
                if (policy.DisplayName != null)
                {
                    Console.WriteLine(policy.DisplayName);
                }
                if (policy.Documentation?.Content != null)
                {
                    Console.WriteLine(policy.Documentation.Content);
                }
                Console.WriteLine();
            }
        }
        // [END monitoring_alert_list_policies]

        // [START monitoring_alert_list_channels]
        static void ListNotificationChannels(string projectId)
        {
            var client = NotificationChannelServiceClient.Create();
            var response = client.ListNotificationChannels(new ProjectName(projectId));
            foreach (NotificationChannel channel in response)
            {
                Console.WriteLine(channel.Name);
                if (channel.DisplayName != null)
                {
                    Console.WriteLine(channel.DisplayName);
                }
                Console.WriteLine();
            }
        }
        // [END monitoring_alert_list_channels]

        // [START monitoring_alert_backup_policies]
        static void BackupPolicies(string projectId, string filePath)
        {
            var policyClient = AlertPolicyServiceClient.Create();
            var channelClient = NotificationChannelServiceClient.Create();
            var projectName = new ProjectName(projectId);
            File.WriteAllText(filePath, JsonConvert.SerializeObject(
                new BackupRecord()
                {
                    ProjectId = projectId,
                    Policies = policyClient.ListAlertPolicies(projectName),
                    Channels = channelClient.ListNotificationChannels(projectName)
                }, new ProtoMessageConverter()));
        }
        // [END monitoring_alert_backup_policies]

        // [START monitoring_alert_restore_policies]
        // [START monitoring_alert_create_policy]
        // [START monitoring_alert_create_channel]
        // [START monitoring_alert_update_channel]
        static void RestorePolicies(string projectId, string filePath)
        {
            var policyClient = AlertPolicyServiceClient.Create();
            var channelClient = NotificationChannelServiceClient.Create();
            List<Exception> exceptions = new List<Exception>();
            var backup = JsonConvert.DeserializeObject<BackupRecord>(
                File.ReadAllText(filePath), new ProtoMessageConverter());
            var projectName = new ProjectName(projectId);
            bool isSameProject = projectId == backup.ProjectId;
            // When a channel is recreated, rather than updated, it will get
            // a new name.  We have to update the AlertPolicy with the new
            // name.  Track the names in this map.
            var channelNameMap = new Dictionary<string, string>();
            foreach (NotificationChannel channel in backup.Channels)
            {
                // [END monitoring_alert_create_policy]
                try
                {
                    bool updated = false;
                    Console.WriteLine("Updating channel.\n{0}",
                        channel.DisplayName);
                    // This field is immutable and it is illegal to specify a
                    // non-default value (UNVERIFIED or VERIFIED) in the
                    // Create() or Update() operations.
                    channel.VerificationStatus = NotificationChannel.Types
                        .VerificationStatus.Unspecified;
                    if (isSameProject)
                        try
                        {
                            channelClient.UpdateNotificationChannel(
                                null, channel);
                            updated = true;
                        }
                        catch (Grpc.Core.RpcException e)
                        when (e.Status.StatusCode == StatusCode.NotFound)
                        { }
                    if (!updated)
                    {
                        // The channel no longer exists.  Recreate it.
                        string oldName = channel.Name;
                        channel.Name = null;
                        var response = channelClient.CreateNotificationChannel(
                            projectName, channel);
                        channelNameMap.Add(oldName, response.Name);
                    }
                }
                catch (Exception e)
                {
                    // If one failed, continue trying to update the others.
                    exceptions.Add(e);
                }
                // [START monitoring_alert_create_policy]
            }
            foreach (AlertPolicy policy in backup.Policies)
            {
                // [END monitoring_alert_create_channel]
                // [END monitoring_alert_update_channel]
                string policyName = policy.Name;
                // These two fields cannot be set directly, so clear them.
                policy.CreationRecord = null;
                policy.MutationRecord = null;
                // Update channel names if the channel was recreated with
                // another name.
                for (int i = 0; i < policy.NotificationChannels.Count; ++i)
                {
                    if (channelNameMap.ContainsKey(policy.NotificationChannels[i]))
                    {
                        policy.NotificationChannels[i] =
                            channelNameMap[policy.NotificationChannels[i]];
                    }
                }
                try
                {
                    Console.WriteLine("Updating policy.\n{0}",
                        policy.DisplayName);
                    bool updated = false;
                    if (isSameProject)
                        try
                        {
                            policyClient.UpdateAlertPolicy(null, policy);
                            updated = true;
                        }
                        catch (Grpc.Core.RpcException e)
                        when (e.Status.StatusCode == StatusCode.NotFound)
                        { }
                    if (!updated)
                    {
                        // The policy no longer exists.  Recreate it.
                        policy.Name = null;
                        foreach (var condition in policy.Conditions)
                        {
                            condition.Name = null;
                        }
                        policyClient.CreateAlertPolicy(projectName, policy);
                    }
                    Console.WriteLine("Restored {0}.", policyName);
                }
                catch (Exception e)
                {
                    // If one failed, continue trying to update the others.
                    exceptions.Add(e);
                }
                // [START monitoring_alert_create_channel]
                // [START monitoring_alert_update_channel]
            }
            if (exceptions.Count > 0)
            {
                throw new AggregateException(exceptions);
            }
        }
        // [END monitoring_alert_create_policy]
        // [END monitoring_alert_create_channel]
        // [END monitoring_alert_update_channel]

        // [START monitoring_alert_backup_policies]
        class BackupRecord
        {
            public string ProjectId { get; set; }
            public IEnumerable<AlertPolicy> Policies { get; set; }
            public IEnumerable<NotificationChannel> Channels { get; set; }
        }

        /// <summary>
        /// Lets Newtonsoft.Json and Protobuf's json converters play nicely
        /// together.  The default Netwtonsoft.Json Deserialize method will
        /// not correctly deserialize proto messages.
        /// </summary>
        class ProtoMessageConverter : JsonConverter
        {
            public override bool CanConvert(System.Type objectType)
            {
                return typeof(Google.Protobuf.IMessage)
                    .IsAssignableFrom(objectType);
            }

            public override object ReadJson(JsonReader reader,
                System.Type objectType, object existingValue,
                JsonSerializer serializer)
            {
                // Read an entire object from the reader.
                var converter = new ExpandoObjectConverter();
                object o = converter.ReadJson(reader, objectType, existingValue,
                    serializer);
                // Convert it back to json text.
                string text = JsonConvert.SerializeObject(o);
                // And let protobuf's parser parse the text.
                IMessage message = (IMessage)Activator
                    .CreateInstance(objectType);
                return Google.Protobuf.JsonParser.Default.Parse(text,
                    message.Descriptor);
            }

            public override void WriteJson(JsonWriter writer, object value,
                JsonSerializer serializer)
            {
                writer.WriteRawValue(Google.Protobuf.JsonFormatter.Default
                    .Format((IMessage)value));
            }
        }
        // [END monitoring_alert_restore_policies]
        // [END monitoring_alert_backup_policies]

        // [START monitoring_alert_replace_channels]
        static void ReplaceChannels(string projectId, string alertPolicyId,
            IEnumerable<string> channelIds)
        {
            var alertClient = AlertPolicyServiceClient.Create();
            var policy = new AlertPolicy()
            {
                Name = new AlertPolicyName(projectId, alertPolicyId).ToString()
            };
            foreach (string channelId in channelIds)
            {
                policy.NotificationChannels.Add(
                    new NotificationChannelName(projectId, channelId)
                    .ToString());
            }
            var response = alertClient.UpdateAlertPolicy(
                new FieldMask { Paths = { "notification_channels" } }, policy);
            Console.WriteLine("Updated {0}.", response.Name);
        }
        // [END monitoring_alert_replace_channels]

        // [START monitoring_alert_disable_policies]
        // [START monitoring_alert_enable_policies]
        static object EnablePolicies(string projectId, string filter, bool enable)
        {
            var client = AlertPolicyServiceClient.Create();
            var request = new ListAlertPoliciesRequest()
            {
                ProjectName = new ProjectName(projectId),
                Filter = filter
            };
            var response = client.ListAlertPolicies(request);
            int result = 0;
            foreach (AlertPolicy policy in response)
            {
                try
                {
                    if (policy.Enabled == enable)
                    {
                        Console.WriteLine("Policy {0} is already {1}.",
                            policy.Name, enable ? "enabled" : "disabled");
                        continue;
                    }
                    policy.Enabled = enable;
                    var fieldMask = new FieldMask { Paths = { "enabled" } };
                    client.UpdateAlertPolicy(fieldMask, policy);
                    Console.WriteLine("{0} {1}.", enable ? "Enabled" : "Disabled",
                        policy.Name);
                }
                catch (Grpc.Core.RpcException e)
                when (e.Status.StatusCode == StatusCode.InvalidArgument)
                {
                    Console.WriteLine(e.Message);
                    result -= 1;
                }
            }
            // Return a negative count of how many enable operations failed.
            return result;
        }
        // [END monitoring_alert_enable_policies]
        // [END monitoring_alert_disable_policies]

        static void DeleteAlertPolicy(string policyNameString)
        {
            var client = AlertPolicyServiceClient.Create();
            AlertPolicyName policyName;
            if (!AlertPolicyName.TryParse(policyNameString, out policyName))
            {
                string message = string.Format(
                    @"{0} is not a valid alert policy name.
                    Policy names look like this: {1}.",
                    policyNameString,
                    new AlertPolicyName("project-id", "alert-policy-id"));
                throw new Exception(message);
            }
            client.DeleteAlertPolicy(policyName);
            Console.WriteLine($"Deleted {policyName}.");
        }

        static void DeleteNotificationChannel(string channelNameString,
            bool force)
        {
            var client = NotificationChannelServiceClient.Create();
            NotificationChannelName channelName;
            if (!NotificationChannelName.TryParse(channelNameString,
                out channelName))
            {
                string message = string.Format(
                    @"{0} is not a valid notification channel name.
                    Channel names look like this: {1}.",
                    channelNameString,
                    new NotificationChannelName("project-id",
                        "notification-channel-id"));
                throw new Exception(message);
            }
            client.DeleteNotificationChannel(channelName, force);
            Console.WriteLine($"Deleted {channelName}.");
        }
    }
}
