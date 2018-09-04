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
    /// Handler for all "vision.landmarks" DialogFlow intent.
    /// </summary>
    [Intent("vision.landmarks")]
    public class VisionLandmarksHandler : BaseVisionHandler
    {
        /// <summary>
        /// Initializes class with the conversation.
        /// </summary>
        /// <param name="conversation">Conversation</param>
        public VisionLandmarksHandler(Conversation conversation) : base(conversation)
        {
        }

        /// <summary>
        /// Handle the intent.
        /// </summary>
        /// <param name="req">Webhook request</param>
        /// <returns>Webhook response</returns>
        public override async Task<WebhookResponse> HandleAsync(WebhookRequest req)
        {
            // Create the client and ask for landmarks from Vision API ML service.
            var visionClient = ImageAnnotatorClient.Create();
            var landmarks = await visionClient.DetectLandmarksAsync(Image.FromUri(_conversation.State.FocusedImage.Url));

            // Order landmarks by score, and prepare the landmark descriptions for DialogFlow
            var toSay = landmarks.OrderByDescending(x => x.Score)
                .TakeWhile((x, i) => i == 0 || x.Score > 0.75)
                .Select(x => x.Description)
                .ToList();

            return new WebhookResponse 
            {
                FulfillmentText = CombineList(toSay, "This picture contains: ", "This picture contains no landmarks")
            };
        }
    }
}