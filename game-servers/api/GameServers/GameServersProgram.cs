// Copyright(c) 2020 Google LLC.
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

using CommandLine;

namespace GoogleCloudSamples
{
    public class BaseOptions
    {
        [Value(0, HelpText = "Your project ID")]
        public string ProjectID { get; set; }

        [Value(1, HelpText = "Location name")]
        public string Location { get; set; }
    }

    public class GameServersProgram
    {
        public static int Main(string[] args)
        {
            var verbMap = new VerbMap<object>();
            Realms.RegisterCommands(verbMap);
            GameServerClusters.RegisterCommands(verbMap);
            return (int)verbMap.Run(args);
        }
    }
}
