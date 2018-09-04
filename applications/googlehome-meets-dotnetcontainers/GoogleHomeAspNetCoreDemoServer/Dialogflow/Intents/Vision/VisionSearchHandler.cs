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
using Google.Apis.Customsearch.v1;
using Google.Apis.Services;
using Google.Cloud.Dialogflow.V2;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace GoogleHomeAspNetCoreDemoServer.Dialogflow.Intents.Vision
{
    /// <summary>
    /// Handler for "vision.search" DialogFlow intent.
    /// </summary>
    [Intent("vision.search")]
    public class VisionSearchHandler : BaseVisionHandler
    {
        /// <summary>
        /// Initializes class with the conversation.
        /// </summary>
        /// <param name="conversation">Conversation</param>
        public VisionSearchHandler(Conversation conversation) : base(conversation)
        {
        }

        /// <summary>
        /// Handle the intent.
        /// </summary>
        /// <param name="req">Webhook request</param>
        /// <returns>Webhook response</returns>
        public override async Task<WebhookResponse> HandleAsync(WebhookRequest req)
        {
            var searchTerm = req.QueryResult.Parameters.Fields["searchterm"].StringValue;

            DialogflowApp.Show($"<div>Searching for pictures of: {searchTerm}</div><div>Please wait...</div>");

            var searchService = CreateSearchClient();
            var query = CreateSearchQuery(searchService, searchTerm);
            var result = await query.ExecuteAsync();

            // Store images in state
            var images = result.Items
                .Select(x => new ConvState.Image { Title = x.Title, Url = x.Link })
                .ToList();

            _conversation.State.ImageList = images;

            var imageList = images.Select(x => $"<li><img src=\"{x.Url}\" alt=\"{WebUtility.HtmlEncode(x.Title)}\" style=\"width:200px\" /></li>");
            DialogflowApp.Show($"<ol>{string.Join("", imageList)}</ol>");

            return new WebhookResponse 
            { 
                FulfillmentText = $"Found some pictures of: {searchTerm}. Now, select a picture."
            };
        }

        // Configure the search query with the requested subject, asking for images
        private static CseResource.ListRequest CreateSearchQuery(CustomsearchService searchService, string searchTerm)
        {
            var query = searchService.Cse.List(searchTerm);
            query.Cx = Program.AppSettings.CustomSearchSettings.EngineId;
            query.SearchType = CseResource.ListRequest.SearchTypeEnum.Image;
            query.Num = 10;
            query.ImgSize = CseResource.ListRequest.ImgSizeEnum.Large;
            query.Safe = CseResource.ListRequest.SafeEnum.High;
            return query;
        }

        // Create the CustomSearch client. Note this is not a cloud library.
        private static CustomsearchService CreateSearchClient()
        {
            return new CustomsearchService(new BaseClientService.Initializer
            {
                ApiKey = Program.AppSettings.GoogleCloudSettings.ApiKey,
                ApplicationName = Program.AppSettings.CustomSearchSettings.ApplicationName
            });
        }
    }
}
