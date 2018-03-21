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
using Google.Api.Gax;
using System.Linq;
using System.Threading.Tasks;

namespace GoogleHomeAspNetCoreDemoServer.Dialogflow.Intents
{
    /// <summary>
    /// Handler for "platform.describe" DialogFlow intent.
    /// </summary>
    [Intent("platform.describe")]
    public class PlatformDescribeHandler : BaseHandler
    {
        /// <summary>
        /// Initializes class with the conversation.
        /// </summary>
        /// <param name="conversation"></param>
        public PlatformDescribeHandler(Conversation conversation) : base(conversation)
        {
        }

        /// <summary>
        /// Handle the intent.
        /// </summary>
        /// <param name="req">Conversation request</param>
        /// <returns></returns>
        public override async Task<object> Handle(ConvRequest req)
        {
            var platform = await Platform.InstanceAsync();

            GetDetailedDescription(platform, out string spokenDescription, out string[] textDescription);

            DialogflowApp.Show(string.Join("", textDescription.Select(x => $"<div>{x}</div>")));
            
            return DialogflowApp.Tell(spokenDescription);
        }

        private static void GetDetailedDescription(Platform platform, out string spokenDescription, out string[] textDescription)
        {
            // Create a text description, and a spoken description of each platform.
            switch (platform.Type)
            {
                case PlatformType.Gae:
                    spokenDescription = $"Running on Google App Engine. Project ID is '{platform.ProjectId}'";
                    textDescription = new[] { "Google App Engine", $"ProjectID: '{platform.ProjectId}'" };
                    break;
                case PlatformType.Gce:
                    spokenDescription = $"Running on Google Compute Engine. Project ID is '{platform.ProjectId}'";
                    textDescription = new[] { "Google Compute Engine", $"ProjectID: '{platform.ProjectId}'" };
                    break;
                case PlatformType.Gke:
                    var gke = platform.GkeDetails;
                    spokenDescription = $"Running on Google Kubernetes Engine, at location {gke.Location}, on cluster named {gke.ClusterName}";
                    textDescription = new[]
                    {
                        "Google Kubernetes Engine",
                        $"ProjectID: '{gke.ProjectId}'",
                        $"Location: '{gke.Location}'",
                        $"Cluster name: '{gke.ClusterName}'",
                        $"Kubernetes pod ID: {gke.PodId}",
                    };
                    break;
                default:
                    spokenDescription = "Oops, I don't know where I'm running. Possibly a local dev machine.";
                    textDescription = new[] { "Oops, I don't know where I'm running. Possibly a local dev machine." };
                    break;
            }
        }
    }
}