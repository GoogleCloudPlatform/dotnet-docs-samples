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
using System.Net;
using System.Threading.Tasks;

namespace GoogleHomeAspNetCoreDemoServer.Dialogflow.Intents.Vision
{
    /// <summary>
    /// Handler for all "vision.select" DialogFlow intent.
    /// </summary>
    [Intent("vision.select")]
    public class VisionSelectHandler : BaseVisionHandler
    {
        /// <summary>
        /// Initializes class with the conversation.
        /// </summary>
        /// <param name="conversation">Conversation</param>
        public VisionSelectHandler(Conversation conversation) : base(conversation)
        {
        }

        /// <summary>
        /// Handle the Dialogflow intent.
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public override string Handle(ConvRequest req)
        {
            if (!int.TryParse(req.Parameters["index"], out int index))
            {
                // Something went wrong with parsing, index is not a number
                return DialogflowApp.Tell($"I don't know which picture you're talking about.");
            }

            if (index < 1 || index > _conversation.State.ImageList.Count)
            {
                // Tried to select an out-of-range picture.
                return DialogflowApp.Tell($"That picture doesn't exist!");
            }

            // Store the selected image in the state
            var image = _conversation.State.ImageList[index - 1];
            _conversation.State.FocusedImage = image;

            DialogflowApp.Show($"<img src=\"{image.Url}\" alt=\"{WebUtility.HtmlEncode(image.Title)}\" style=\"height:100%;width:100%\" />");

            return DialogflowApp.Tell($"Picture {index} selected. You can describe, show landmarks or ask whether the image is safe.");
        }
    }
}
