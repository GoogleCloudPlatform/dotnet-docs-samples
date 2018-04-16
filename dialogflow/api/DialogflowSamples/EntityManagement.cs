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
    // Samples demonstrating how to Create, List, Delete Entities
    public class EntityManagement
    {
        public static void RegisterCommands(VerbMap<object> verbMap)
        {
            verbMap
                .Add((CreateOptions opts) => Create(opts.ProjectId, opts.EntityTypeId, opts.EntityValue, opts.Synonyms))
                .Add((ListOptions opts) => List(opts.ProjectId, opts.EntityTypeId))
                .Add((DeleteOptions opts) => Delete(opts.ProjectId, opts.EntityTypeId, opts.EntityValue));
        }

        [Verb("entities:create", HelpText = "Create new entity type")]
        public class CreateOptions : OptionsWithProjectId
        {
            [Value(0, MetaName = "entityTypeId", HelpText = "ID of existing EntityType", Required = true)]
            public string EntityTypeId { get; set; }

            [Value(1, MetaName = "entityValue", HelpText = "New entity value", Required = true)]
            public string EntityValue { get; set; }

            [Value(2, MetaName = "synonyms", HelpText = "Comma separated synonyms", Required = true)]
            public string SynonymsInput { get; set; }

            public string[] Synonyms => SynonymsInput.Split(',');
        }

        // [START dialogflow_create_entity]
        public static int Create(string projectId,
                                 string entityTypeId,
                                 string entityValue,
                                 string[] synonyms)
        {
            var client = EntityTypesClient.Create();

            var entity = new EntityType.Types.Entity() { Value = entityValue };
            entity.Synonyms.AddRange(synonyms);

            var operation = client.BatchCreateEntities(
                parent: new EntityTypeName(projectId, entityTypeId),
                entities: new [] { entity }
            );

            operation.PollUntilCompleted();

            Console.WriteLine($"Completed create entity operation for {entityValue}");

            return 0;
        }
        // [END dialogflow_create_entity]

        [Verb("entities:list", HelpText = "Print list of entities for given EntityType")]
        public class ListOptions : OptionsWithProjectId
        {
            [Value(0, MetaName = "entityTypeId", HelpText = "ID of existing EntityType", Required = true)]
            public string EntityTypeId { get; set; }
        }

        // [START dialogflow_list_entities]
        public static int List(string projectId, string entityTypeId)
        {
            var client = EntityTypesClient.Create();
            var entityType = client.GetEntityType(new EntityTypeName(
                projectId, entityTypeId
            ));

            foreach (var entity in entityType.Entities)
            {
                Console.WriteLine($"Entity value: {entity.Value}");
                Console.WriteLine($"Entity synonyms: {string.Join(',', entity.Synonyms)}");
            }

            return 0;
        }
        // [END dialogflow_list_entities]

        [Verb("entities:delete", HelpText = "Delete specified EntityType")]
        public class DeleteOptions : OptionsWithProjectId
        {
            [Value(0, MetaName = "entityTypeId", HelpText = "ID of existing EntityType", Required = true)]
            public string EntityTypeId { get; set; }

            [Value(1, MetaName = "entityValue", HelpText = "Existing entity value", Required = true)]
            public string EntityValue { get; set; }
        }

        // [START dialogflow_delete_entity]
        public static int Delete(string projectId, string entityTypeId, string entityValue)
        {
            var client = EntityTypesClient.Create();

            var operation = client.BatchDeleteEntities(
                parent: new EntityTypeName(projectId, entityTypeId),
                entityValues: new [] { entityValue }
            );

            operation.PollUntilCompleted();

            Console.WriteLine($"Completed delete entity operation for {entityValue}");

            return 0;
        }
        // [END dialogflow_delete_entity]
    }
}
