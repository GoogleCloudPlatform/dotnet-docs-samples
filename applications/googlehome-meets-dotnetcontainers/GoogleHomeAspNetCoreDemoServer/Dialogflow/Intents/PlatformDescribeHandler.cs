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
using Google.Cloud.Dialogflow.V2;
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
        /// <param name="req">Webhook request</param>
        /// <returns>Webhook response</returns>
        public override async Task<WebhookResponse> HandleAsync(WebhookRequest req)
        {
            var platform = await Platform.InstanceAsync();
            (var spokenDescription, string[] textDescription) = GetDetailedDescription(platform);

            DialogflowApp.Show(string.Join("", textDescription.Select(x => $"<div>{x}</div>")));
            return new WebhookResponse { FulfillmentText = spokenDescription };
        }

        /// <summary>
        /// Given a platform, it returns more detailed description of the platform in text and spoken form.
        /// </summary>
        /// <param name="platform">Platform to describe</param>
        /// <returns>Spoken description of the platform and text description of the platform</returns>
        private static (string spokenDescription, string[] textDescription) GetDetailedDescription(Platform platform)
        {
            // Create a text description, and a spoken description of each platform.
            switch (platform.Type)
            {
                case PlatformType.Gae:
                    return ($"Running on Google App Engine. Project ID is '{platform.ProjectId}'",
                        new[] { "Google App Engine", $"ProjectID: '{platform.ProjectId}'" });
                case PlatformType.Gce:
                    return ($"Running on Google Compute Engine. Project ID is '{platform.ProjectId}'", 
                        new[] { "Google Compute Engine", $"ProjectID: '{platform.ProjectId}'" });
                case PlatformType.Gke:
                    var gke = platform.GkeDetails;
                    return ($"Running on Google Kubernetes Engine, at location {gke.Location}, on cluster named {gke.ClusterName}",
                        new[] {"Google Kubernetes Engine", $"ProjectID: '{gke.ProjectId}'", $"Location: '{gke.Location}'", $"Cluster name: '{gke.ClusterName}'", $"Kubernetes pod ID: {gke.PodId}",});
                default:
                    return ("Oops, I don't know where I'm running. Possibly a local dev machine.",
                        new[] { "Oops, I don't know where I'm running. Possibly a local dev machine." });
            }
        }
    }
}