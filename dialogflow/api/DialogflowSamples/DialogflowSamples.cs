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

namespace GoogleCloudSamples
{
    public class DialogflowSamples
    {
        public static int Main(string[] args)
        {
            var verbMap = new VerbMap<object>();

            DetectIntentTexts.RegisterCommands(verbMap);
            DetectIntentStream.RegisterCommands(verbMap);
            IntentManagement.RegisterCommands(verbMap);

            verbMap.NotParsedFunc = (err) => 1;

            return (int)verbMap.Run(args);
        }
    }

    /*
     * Shared options. All commands require projectId. Many require sessionId as well.
     */

    public class OptionsWithProjectId
    {
        [Option('p', "projectId", HelpText = "Your Google Cloud project ID", Required = true)]
        public string ProjectId { get; set; }
    }

    public class OptionsWithProjectIdAndSessionId : OptionsWithProjectId
    {
        [Option('s', "sessionId", HelpText = "Session ID", Required = true)]
        public string SessionId { get; set; }
    }
}
