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
using System.Linq;
using System.Collections.Generic;
using CommandLine;
using Google.Cloud.Dialogflow.V2;

namespace GoogleCloudSamples
{
    // Samples demonstrating how to Create, List, Delete Contexts
    public class ContextManagement
    {
        public static void RegisterCommands(VerbMap<object> verbMap)
        {
            verbMap
                .Add((CreateOptions opts) => Create(opts.ProjectId, opts.SessionId, opts.ContextId, opts.LifespanCount))
                .Add((ListOptions opts) => List(opts.ProjectId, opts.SessionId))
                .Add((DeleteOptions opts) => Delete(opts.ProjectId, opts.SessionId, opts.ContextId));
        }

        [Verb("contexts:create", HelpText = "Create new Context")]
        public class CreateOptions : OptionsWithProjectIdAndSessionId
        {
            [Value(0, MetaName = "contextId", HelpText = "ID of new Context", Required = true)]
            public string ContextId { get; set; }

            [Value(1, MetaName = "lifespanCount", HelpText = "Lifespan Count of new Context", Default = 1)]
            public int LifespanCount { get; set; }
        }

        // [START dialogflow_create_context]
        public static int Create(string projectId,
                                 string sessionId,
                                 string contextId,
                                 int lifespanCount = 1)
        {
            var client = ContextsClient.Create();

            var context = new Context();
            context.ContextName = new ContextName(projectId, sessionId, contextId);
            context.LifespanCount = lifespanCount;

            var newContext = client.CreateContext(
                parent: new SessionName(projectId, sessionId),
                context: context
            );

            Console.WriteLine($"Created Context: {newContext.Name}");

            return 0;
        }
        // [END dialogflow_create_context]

        [Verb("contexts:list", HelpText = "Print list of entities for given Context")]
        public class ListOptions : OptionsWithProjectIdAndSessionId { }

        // [START dialogflow_list_contexts]
        public static int List(string projectId, string sessionId)
        {
            var client = ContextsClient.Create();

            var contexts = client.ListContexts(
                new SessionName(projectId, sessionId)
            );

            foreach (var context in contexts)
            {
                Console.WriteLine($"Context name: {context.Name}");
                Console.WriteLine($"Context lifespan count: {context.LifespanCount}");
                if (context.Parameters != null)
                {
                    Console.WriteLine("Fields:");
                    foreach (var field in context.Parameters.Fields)
                    {
                        Console.WriteLine($"{field.Key}: {field.Value}");
                    }
                }
            }

            return 0;
        }
        // [END dialogflow_list_contexts]

        [Verb("contexts:delete", HelpText = "Delete specified Context")]
        public class DeleteOptions : OptionsWithProjectIdAndSessionId
        {
            [Value(0, MetaName = "contextId", HelpText = "ID of existing Context", Required = true)]
            public string ContextId { get; set; }
        }

        // [START dialogflow_delete_context]
        public static int Delete(string projectId, string sessionId, string contextId)
        {
            var client = ContextsClient.Create();

            client.DeleteContext(new ContextName(projectId, sessionId, contextId));

            Console.WriteLine($"Deleted Context: {contextId}");

            return 0;
        }
        // [END dialogflow_delete_context]
    }
}
