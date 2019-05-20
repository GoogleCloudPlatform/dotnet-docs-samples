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

using System;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using Google.Cloud.PubSub.V1;
using Google.Protobuf;

namespace TranslatorWorker
{
    class Program
    {
        static void Main(string[] args)
        {
            Program program = new Program();
            program.AsyncMain(args).Wait();
        }

        FirestoreDb _firestore;
        SubscriberClient _subscriber;
        Google.Cloud.Translation.V2.TranslationClient _translator;
        private CollectionReference _translations;

        async Task AsyncMain(string[] args)
        {
            // Create connection to Firestore.
            _firestore = FirestoreDb.Create(GetFirestoreProjectId());
            // Create connnection to Pub/Sub.
            _subscriber = await SubscriberClient.CreateAsync(
                new SubscriptionName(GetProjectId(), GetSubscriptionId()));
            // Create connection to Translation API.
            _translator = await Google.Cloud.Translation.V2.TranslationClient
                .CreateAsync();
            // Use the Translations collection in Firestore.
            _translations = _firestore.Collection("Translations");
            // Start handling one pubsub message at a time.
            await _subscriber.StartAsync((message, token) =>
                HandleOneRequestAsync(message));
            // Forever.
            await Task.Delay(-1);
        }

        async Task<SubscriberClient.Reply> HandleOneRequestAsync(
            PubsubMessage message)
        {
            // Translate the source text.
            string sourceText = message.Data.ToStringUtf8();
            var result = await _translator.TranslateTextAsync(sourceText, "es");
            // Store the result in Firestore.
            Translation translation = new Translation()
            {
                TimeStamp = DateTime.UtcNow,
                SourceText = sourceText,
                TranslatedText = result.TranslatedText
            };
            await _translations.AddAsync(translation);
            // Acknowledge this Pub/Sub message so we never see it again.
            return SubscriberClient.Reply.Ack;
        }

        public static string GetProjectId()
        {
            GoogleCredential googleCredential = Google.Apis.Auth.OAuth2
                .GoogleCredential.GetApplicationDefault();
            if (googleCredential != null)
            {
                ICredential credential = googleCredential.UnderlyingCredential;
                ServiceAccountCredential serviceAccountCredential =
                    credential as ServiceAccountCredential;
                if (serviceAccountCredential != null)
                {
                    return serviceAccountCredential.ProjectId;
                }
            }
            return Google.Api.Gax.Platform.Instance().ProjectId;
        }

        // Would normally be the same as the regular project id.  But in
        // our test environment, we need a different one.
        public string GetFirestoreProjectId() =>
            System.Environment.GetEnvironmentVariable("FIRESTORE_PROJECT_ID") ??
            GetProjectId();

        public string GetSubscriptionId() =>
            System.Environment.GetEnvironmentVariable(
                "TranslateRequestsSubscriptionId") ?? "translate-requests";
    }
}
