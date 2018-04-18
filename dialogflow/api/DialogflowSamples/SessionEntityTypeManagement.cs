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

using System;
using CommandLine;
using Google.Cloud.Dialogflow.V2;

namespace GoogleCloudSamples
{
    // Samples demonstrating how to Create, List, Delete SessionEntityTypes
    public class SessionEntityTypeManagement
    {
        public static void RegisterCommands(VerbMap<object> verbMap)
        {
            verbMap
                .Add((CreateOptions opts) => Create(opts.ProjectId, opts.SessionId, opts.EntityTypeDisplayName, opts.EntityValues))
                .Add((ListOptions opts) => List(opts.ProjectId, opts.SessionId))
                .Add((DeleteOptions opts) => Delete(opts.ProjectId, opts.SessionId, opts.EntityTypeDisplayName));
        }

        [Verb("session-entity-types:create", HelpText = "Create new session entity type")]
        public class CreateOptions : OptionsWithProjectIdAndSessionId
        {
            [Value(0, MetaName = "entityTypeDisplayName", HelpText = "Display name of EntityType", Required = true)]
            public string EntityTypeDisplayName { get; set; }

            [Value(1, MetaName = "entityValues", HelpText = "Comma separated text values of entities", Required = true)]
            public string EntityValueInput { get; set; }

            public string[] EntityValues => EntityValueInput.Split(',');
        }

        // [START dialogflow_create_session_entity_type]
        public static int Create(string projectId,
                                 string sessionId,
                                 string entityTypeDisplayName,
                                 string[] entityValues)
        {
            var client = SessionEntityTypesClient.Create();

            var sessionEntityType = new SessionEntityType();
            sessionEntityType.SessionEntityTypeName = new SessionEntityTypeName(projectId, sessionId, entityTypeDisplayName);
            sessionEntityType.EntityOverrideMode = SessionEntityType.Types.EntityOverrideMode.Override;
            foreach (var entityValue in entityValues)
            {
                var entity = new EntityType.Types.Entity() { Value = entityValue };
                entity.Synonyms.Add(entityValue);
                sessionEntityType.Entities.Add(entity);
            }

            var createdSessionEntityType = client.CreateSessionEntityType(
                parent: new SessionName(projectId, sessionId),
                sessionEntityType: sessionEntityType
            );

            Console.WriteLine($"Created SessionEntityType: {createdSessionEntityType.Name}");

            return 0;
        }
        // [END dialogflow_create_session_entity_type]

        [Verb("session-entity-types:list", HelpText = "Print list of all session entity types")]
        public class ListOptions : OptionsWithProjectIdAndSessionId { }

        // [START dialogflow_list_session_entity_types]
        public static int List(string projectId, string sessionId)
        {
            var client = SessionEntityTypesClient.Create();
            var response = client.ListSessionEntityTypes(
                parent: new SessionName(projectId, sessionId)
            );

            foreach (var sessionEntityType in response)
            {
                Console.WriteLine($"SessionEntityType name: {sessionEntityType.Name}");
                Console.WriteLine($"Number of entities: {sessionEntityType.Entities.Count}");
                if (sessionEntityType.Entities.Count > 0)
                {
                    Console.WriteLine("Entity values:");
                    foreach (var entity in sessionEntityType.Entities)
                    {
                        Console.WriteLine(entity.Value);
                    }
                }
            }

            return 0;
        }
        // [END dialogflow_list_session_entity_types]

        [Verb("session-entity-types:delete", HelpText = "Delete specified SessionEntityType")]
        public class DeleteOptions : OptionsWithProjectIdAndSessionId
        {
            [Value(0, MetaName = "entityTypeDisplayName", HelpText = "Display name of EntityType", Required = true)]
            public string EntityTypeDisplayName { get; set; }
        }

        // [START dialogflow_delete_session_entity_type]
        public static int Delete(string projectId, string sessionId, string entityTypeDisplayName)
        {
            var client = SessionEntityTypesClient.Create();

            client.DeleteSessionEntityType(new SessionEntityTypeName(
                projectId, sessionId, entityTypeDisplayName
            ));

            Console.WriteLine($"Deleted SessionEntityType: {entityTypeDisplayName}");

            return 0;
        }
        // [END dialogflow_delete_session_entity_type]
    }
}
