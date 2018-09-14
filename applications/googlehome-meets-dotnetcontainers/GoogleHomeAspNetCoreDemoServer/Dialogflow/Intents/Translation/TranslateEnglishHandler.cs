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
using Google.Cloud.Translation.V2;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace GoogleHomeAspNetCoreDemoServer.Dialogflow.Intents
{
    /// <summary>
    /// Handler for "translation.english" DialogFlow intent.
    /// </summary>
    [Intent("translation.english")]
    public class TranslateEnglishHandler : BaseHandler
    {
        /// <summary>
        /// Initializes class with the conversation.
        /// </summary>
        /// <param name="conversation"></param>
        public TranslateEnglishHandler(Conversation conversation) : base(conversation)
        {
        }

        /// <summary>
        /// Handle the intent.
        /// </summary>
        /// <param name="req">Webhook request</param>
        /// <returns>Webhook response</returns>
        public override async Task<WebhookResponse> HandleAsync(WebhookRequest req)
        {
            var errorMessage = ExtractAndValidateParameters(req, out string englishPhrase, out string languageCode);
            if (errorMessage != null)
            {
                DialogflowApp.Show($"<div>{errorMessage}</div>");

                return new WebhookResponse 
                {
                    FulfillmentText = errorMessage
                };
            }

            var client = TranslationClient.Create();
            var response = await client.TranslateTextAsync(englishPhrase, languageCode, LanguageCodes.English);

            DialogflowApp.Show($"<div>'{englishPhrase}' is '{response.TranslatedText}' in {languageCode}</div>");
            return new WebhookResponse { FulfillmentText = response.TranslatedText };
        }

        /// <summary>
        /// Extract the parameters from the request and perform basic validation.
        /// </summary>
        /// <param name="req">Webhook request</param>
        /// <param name="englishPhrase">English phrase</param>
        /// <param name="languageCode">Language code</param>
        /// <returns>Error if the parameters do not exist or validate. Null, if everything is OK.</returns>
        private string ExtractAndValidateParameters(WebhookRequest req, out string englishPhrase, out string languageCode)
        {
            englishPhrase = req.QueryResult.Parameters.Fields["englishphrase"].StringValue;
            var language = req.QueryResult.Parameters.Fields["language"].StringValue;
           
            var fields = typeof(LanguageCodes).GetFields(BindingFlags.Public | BindingFlags.Static);
            var fieldInfo = fields.SingleOrDefault(x => x.Name == language);

            languageCode = fieldInfo?.GetRawConstantValue().ToString();

            if (string.IsNullOrEmpty(englishPhrase))
            {
                return "Sorry, I don't understand the English phrase.";
            }

            if (string.IsNullOrEmpty(language) || fieldInfo == null)
            {
                return $"Sorry, the specified language '{language}' is not supported.";
            }

            return null;
        }
    }
}