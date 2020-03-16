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
                .Add((DeleteOptions opts) => Delete(opts.ProjectId, opts.SessionId, opts.ContextId))
                .Add((DeleteAllOptions opts) => DeleteAllContexts(opts.ProjectId, opts.SessionId));
        }

        [Verb("contexts:create", HelpText = "Create new Context")]
        public class CreateOptions : OptionsWithProjectIdAndSessionId
        {
            [Value(0, MetaName = "contextId", HelpText = "ID of new Context", Required = true)]
            public string ContextId { get; set; }

            [Value(1, MetaName = "lifespanCount", HelpText = "Lifespan Count of new Context", Default = 1)]
            public int LifespanCount { get; set; }
        }

        [Verb("contexts:delete", HelpText = "Delete specified Context")]
        public class DeleteOptions : OptionsWithProjectIdAndSessionId
        {
            [Value(0, MetaName = "contextId", HelpText = "ID of existing Context", Required = true)]
            public string ContextId { get; set; }
        }

        [Verb("contexts:delete-all", HelpText = "Delete specified Context")]
        public class DeleteAllOptions : OptionsWithProjectIdAndSessionId { }

        public static int DeleteAllContexts(string projectId, string sessionId)
        {
            var client = ContextsClient.Create();
            client.DeleteAllContexts(new SessionName(projectId, sessionId));
            return 0;
        }
    }
}
