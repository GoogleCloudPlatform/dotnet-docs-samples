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
    // Samples demonstrating how to Create, List, Delete EntityTypes
    public class EntityTypeManagement
    {
        public static void RegisterCommands(VerbMap<object> verbMap)
        {
            verbMap.Add((CreateOptions opts) => Create(opts.ProjectId, opts.DisplayName, opts.Kind))
                .Add((ListOptions opts) => List(opts.ProjectId))
                .Add((DeleteOptions opts) => Delete(opts.ProjectId, opts.EntityTypeId));
        }

        [Verb("entity-types:create", HelpText = "Create new entity type")]
        public class CreateOptions : OptionsWithProjectId
        {
            [Value(0, MetaName = "displayName", HelpText = "Display name of new EntityType", Required = true)]
            public string DisplayName { get; set; }

            [Value(1, MetaName = "kindName", HelpText = "Kind name, eg. Map or List", Required = true)]
            public string KindName { get; set; }

            public EntityType.Types.Kind Kind => Enum.Parse<EntityType.Types.Kind>(KindName);
        }

        // [START dialogflow_create_entity_type]
        public static int Create(string projectId, string displayName, EntityType.Types.Kind kind = EntityType.Types.Kind.Map)
        {
            var client = EntityTypesClient.Create();

            var entityType = new EntityType();
            entityType.DisplayName = displayName;
            entityType.Kind = kind;

            var createdEntityType = client.CreateEntityType(
                parent: new ProjectAgentName(projectId),
                entityType: entityType
            );

            Console.WriteLine($"Created EntityType: {createdEntityType.Name}");

            return 0;
        }
        // [END dialogflow_create_entity_type]

        [Verb("entity-types:list", HelpText = "Print list of all entity types")]
        public class ListOptions : OptionsWithProjectId {}

        // [START dialogflow_list_entity_types]
        public static int List(string projectId)
        {
            var client = EntityTypesClient.Create();
            var response = client.ListEntityTypes(
                parent: new ProjectAgentName(projectId)
            );

            foreach (var entityType in response)
            {
                Console.WriteLine($"EntityType name: {entityType.Name}");
                Console.WriteLine($"EntityType display name: {entityType.DisplayName}");
                Console.WriteLine($"Number of entities: {entityType.Entities.Count}");
                if (entityType.Entities.Count > 0)
                {
                    Console.WriteLine("Entity values:");
                    foreach (var entity in entityType.Entities)
                    {
                        Console.WriteLine(entity.Value);
                    }
                }
            }

            return 0;
        }
        // [END dialogflow_list_entity_types]

        [Verb("entity-types:delete", HelpText = "Delete specified EntityType")]
        public class DeleteOptions : OptionsWithProjectId
        {
            [Value(0, MetaName = "entityTypeId", HelpText = "ID of EntityType", Required = true)]
            public string EntityTypeId { get; set; }
        }

        // [START dialogflow_delete_entity_type]
        public static int Delete(string projectId, string entityTypeId)
        {
            var client = EntityTypesClient.Create();

            client.DeleteEntityType(new EntityTypeName(projectId, entityTypeId: entityTypeId));

            Console.WriteLine($"Deleted EntityType: {entityTypeId}");

            return 0;
        }
        // [END dialogflow_delete_entity_type]
    }
}
