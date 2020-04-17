/*
 * Copyright 2020 Google LLC
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *       http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
 
 using System;
 using System.Collections.Generic;
 using Google.Cloud.PubSub.V1;
 using Grpc.Core;
 using Xunit;

[CollectionDefinition(nameof(ConfigFixture))]
 public class ConfigFixture : IDisposable, ICollectionFixture<ConfigFixture>
 {
     public string OrganizationId { get; }
     public string ProjectId { get; }
     public string Topic { get; }
     private List<string> CleanupItems = new List<string>();
     public string DefaultNotificationConfigId {get; }

     public ConfigFixture()
     {
        OrganizationId = Environment.GetEnvironmentVariable("GOOGLE_ORGANIZATION_ID");
        ProjectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
        Topic = Environment.GetEnvironmentVariable("GOOGLE_TOPIC");
        DefaultNotificationConfigId = CreateNotificationConfig(RandomId());
     }

     public void Dispose()
     {
        foreach (var configId in CleanupItems)
        {
            DeleteNotificationConfigSnippets.DeleteNotificationConfig(OrganizationId, configId);
        }
     }

    // Returns epoch time as ID
     public string RandomId()
     {
        return DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString();
     }

    // Delete assets when fixture is disposed.
     public void MarkForDeletion(string id) 
     {
         CleanupItems.Add(id);
     }

    public string CreateNotificationConfig(string configId) 
    {
        CreateNotificationConfigSnippets.CreateNotificationConfig(OrganizationId, configId, ProjectId, Topic);
        CleanupItems.Add(configId);
        return configId;
    }
     private string CreateTopic(string projectId, string topicId) 
     {
        PublisherServiceApiClient publisher = PublisherServiceApiClient.Create();
        TopicName topicName = new TopicName(projectId, topicId);
        try
        {
            publisher.CreateTopic(topicName);
        }
        catch (RpcException e)
        when (e.Status.StatusCode == StatusCode.AlreadyExists)
        {
            // Already exists.  That's fine.
        }
        return topicId;
     }
 }