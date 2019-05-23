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
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TranslateWorker.Controllers
{
    /// <summary>
    /// The message Pubsub posts to our controller.
    /// </summary>
    public class PostMessage
    {
        public PubsubMessage message { get; set; }
        public string subscription { get; set; }
    }

    /// <summary>
    /// Pubsub's inner message.
    /// </summary>
    public class PubsubMessage
    {
        public string data { get; set; }
        public string messageId { get; set; }
        public Dictionary<string, string> attributes { get; set; }
    }


    [Route("api/[controller]")]
    [ApiController]
    public class TranslateController : ControllerBase
    {
        private readonly ILogger<TranslateController> _logger;
        private readonly FirestoreDb _firestore;
        private readonly Google.Cloud.Translation.V2.TranslationClient _translator;
        // The Firestore collection where we store translations.
        private readonly CollectionReference _translations;

        public TranslateController(ILogger<TranslateController> logger,
            FirestoreDb firestore,
            Google.Cloud.Translation.V2.TranslationClient translator)
        {
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this._firestore = firestore ?? throw new ArgumentNullException(nameof(firestore));
            this._translator = translator ?? throw new ArgumentNullException(nameof(translator));
            _translations = _firestore.Collection("Translations");
        }

        /// <summary>
        /// Handle a posted message from Pubsub.
        /// </summary>
        /// <param name="request">The message Pubsub posts to this process.</param>
        /// <returns>NoContent on success.</returns>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] PostMessage request)
        {
            // Unpack the message from Pubsub.
            string sourceText;
            try
            {
                byte[] data = Convert.FromBase64String(request.message.data);
                sourceText = Encoding.UTF8.GetString(data);
            }
            catch (Exception e)
            {
                _logger.LogError(1, e, "Bad request");
                return BadRequest();
            }
            // Translate the source text.
            _logger.LogDebug(2, "Translating {0} to Spanish.", sourceText);
            var result = await _translator.TranslateTextAsync(sourceText, "es");
            // Store the result in Firestore.
            Translation translation = new Translation()
            {
                TimeStamp = DateTime.UtcNow,
                SourceText = sourceText,
                TranslatedText = result.TranslatedText
            };
            _logger.LogDebug(3, "Saving translation {0} to {1}.",
                translation.TranslatedText, _translations.Path);
            await _translations.AddAsync(translation);
            // Return a success code.
            return NoContent();
        }

        /// <summary>
        /// Serve a root page so Cloud Run knows this process is healthy.
        /// </summary>
        [Route("/")]
        public IActionResult Index()
        {
            return Content("Serving translate requests...");
        }
    }
}
