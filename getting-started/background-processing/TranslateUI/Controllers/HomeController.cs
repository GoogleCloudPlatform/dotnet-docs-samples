// Copyright (c) 2019 Google LLC.
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

using Google.Cloud.Firestore;
using Google.Cloud.PubSub.V1;
using Google.Protobuf;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TranslateUI.Models;

namespace TranslateUI.Controllers
{
    public class HomeController : Controller
    {
        private readonly FirestoreDb _firestore;
        private readonly PublisherClient _publisher;
        private CollectionReference _translations;

        public HomeController(FirestoreDb firestore, PublisherClient publisher)
        {
            _firestore = firestore;
            _publisher = publisher;
            _translations = _firestore.Collection("Translations");
        }

        [HttpPost]
        [HttpGet]
        public async Task<IActionResult> Index(string SourceText)
        {
            // Look up the most recent 20 translations.
            var query = _translations.OrderByDescending("TimeStamp")
                .Limit(20);
            var snapshotTask = query.GetSnapshotAsync();

            if (!string.IsNullOrWhiteSpace(SourceText))
            {
                // Submit a new translation request.
                await _publisher.PublishAsync(new PubsubMessage()
                {
                    Data = ByteString.CopyFromUtf8(SourceText)
                });
            }

            // Render the page.
            var model = new HomeViewModel()
            {
                Translations = (await snapshotTask).Documents.Select(
                    doc => doc.ConvertTo<Translation>()).ToList(),
                SourceText = SourceText
            };
            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
