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
using Google.Cloud.Dialogflow.V2;
using Google.Cloud.Vision.V1;
using System.Linq;
using System.Threading.Tasks;

namespace GoogleHomeAspNetCoreDemoServer.Dialogflow.Intents.Vision
{
    /// <summary>
    /// Handler for "vision.safe" DialogFlow intent.
    /// </summary>
    [Intent("vision.safe")]
    public class VisionSafeHandler : BaseVisionHandler
    {
        /// <summary>
        /// Initializes class with the conversation.
        /// </summary>
        /// <param name="conversation">Conversation</param>
        public VisionSafeHandler(Conversation conversation) : base(conversation)
        {
        }

        /// <summary>
        /// Handle the Dialogflow intent.
        /// </summary>
        /// <param name="req">Webhook request</param>
        /// <returns>Webhook response</returns>
        public override async Task<WebhookResponse> HandleAsync(WebhookRequest req)
        {
            // Create the client and ask for safe-search info from Vision API ML service.
            var visionClient = ImageAnnotatorClient.Create();
            var safe = await visionClient.DetectSafeSearchAsync(Image.FromUri(_conversation.State.FocusedImage.Url));

            // Format safe-search result into an English sentence
            (string text, int likelyhood) Likely(string s, Likelihood l)
            {
                switch (l)
                {
                    case Likelihood.Likely: case Likelihood.VeryLikely: return (s, 2);
                    case Likelihood.Possible: case Likelihood.Unlikely: return (s, 1);
                    default: return (s, 0);
                }
            }

            var result = new[]
            {
                Likely("has medical content", safe.Medical),
                Likely("is a spoof", safe.Spoof),
                Likely("is violent", safe.Violence),
                Likely("has racy content", safe.Racy),
                Likely("has adult content", safe.Adult)
            };

            var likely = result.Where(x => x.likelyhood == 2).Select(x => x.text).ToList();
            var possible = result.Where(x => x.likelyhood == 1).Select(x => x.text).ToList();
            if (likely.Count == 0 && possible.Count == 0)
            {
                return new WebhookResponse { FulfillmentText = "Let's see. This picture is fine." };
            }

            string reply = "Let's see. ";
            if (likely.Count > 0)
            {
                reply += CombineList(likely, "It's likely this picture", "") + ".";
            }

            if (possible.Count > 0)
            {
                reply += CombineList(possible, "It's possible this picture", "") + ".";
            }

            return new WebhookResponse { FulfillmentText = reply };
        }
    }
}