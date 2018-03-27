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
using System.Linq;
using System.Threading.Tasks;

namespace GoogleHomeAspNetCoreDemoServer.Dialogflow.Intents.Vision
{
    /// <summary>
    /// Handler for "vision.backtoall" DialogFlow intent.
    /// </summary>
    [Intent("vision.backtoall")]
    public class VisitionBacktoallHandler : BaseVisionHandler
    {
        /// <summary>
        /// Initializes class with the conversation.
        /// </summary>
        /// <param name="conversation">Conversation</param>
        public VisitionBacktoallHandler(Conversation conversation) : base(conversation)
        {
        }

        /// <summary>
        /// Handle the intent.
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public override string Handle(ConvRequest req)
        {
            // Unfocus the image
            _conversation.State.FocusedImage = null;

            // Update state with list of pictures
            var images = _conversation.State.ImageList;
            var imagesList = images.Select(x => $"<li><img src=\"{x.Url}\" alt=\"{x.Title}\" style=\"width:200px\" /></li>");
            DialogflowApp.Show($"<ol>{string.Join("", imagesList)}</ol>");

            return DialogflowApp.Tell("OK, looking at all images now.");
        }
    }
}