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

using System;
using System.Linq;
using System.Collections.Generic;
using CommandLine;
using Google.Cloud.Dialogflow.V2;

namespace GoogleCloudSamples
{
    // Samples demonstrating how to Create, List, Delete Intents
    public class IntentManagement
    {
        public static void RegisterCommands(VerbMap<object> verbMap)
        {
            verbMap
                .Add((CreateOptions opts) => Create(opts.ProjectId, opts.DisplayName, opts.MessageText, opts.TrainingPhrasesParts))
                .Add((ListOptions opts) => List(opts.ProjectId))
                .Add((DeleteOptions opts) =>
                    opts.IntentIds.Count() == 1 ?
                    Delete(opts.ProjectId, opts.IntentIds.First()) :
                    BatchDelete(opts.ProjectId, opts.IntentIds));
        }

        [Verb("intents:create", HelpText = "Create new Intent")]
        public class CreateOptions : OptionsWithProjectId
        {
            [Value(0, MetaName = "displayName", HelpText = "Display name of new Intent", Required = true)]
            public string DisplayName { get; set; }

            [Value(1, MetaName = "messageText", HelpText = "Message text", Required = true)]
            public string MessageText { get; set; }

            [Value(2, MetaName = "trainingPhrasesParts", HelpText = "Comma separated training phrases parts", Required = true)]
            public string TrainingPhrasesPartsInput { get; set; }

            public string[] TrainingPhrasesParts => TrainingPhrasesPartsInput.Split(',');
        }

        // [START dialogflow_create_intent]
        public static int Create(string projectId,
                                 string displayName,
                                 string messageText,
                                 string[] trainingPhrasesParts)
        {
            var client = IntentsClient.Create();

            var text = new Intent.Types.Message.Types.Text();
            text.Text_.Add(messageText);

            var message = new Intent.Types.Message()
            {
                Text = text
            };

            var phraseParts = new List<Intent.Types.TrainingPhrase.Types.Part>();
            foreach (var part in trainingPhrasesParts)
            {
                phraseParts.Add(new Intent.Types.TrainingPhrase.Types.Part()
                {
                    Text = part
                });
            }

            var trainingPhrase = new Intent.Types.TrainingPhrase();
            trainingPhrase.Parts.AddRange(phraseParts);

            var intent = new Intent();
            intent.DisplayName = displayName;
            intent.Messages.Add(message);
            intent.TrainingPhrases.Add(trainingPhrase);

            var newIntent = client.CreateIntent(
                parent: new AgentName(projectId),
                intent: intent
            );

            Console.WriteLine($"Created Intent: {newIntent.Name}");

            return 0;
        }
        // [END dialogflow_create_intent]

        [Verb("intents:list", HelpText = "Print list of entities for given Intent")]
        public class ListOptions : OptionsWithProjectId { }

        // [START dialogflow_list_intents]
        public static int List(string projectId)
        {
            var client = IntentsClient.Create();

            var intents = client.ListIntents(new AgentName(projectId));

            foreach (var intent in intents)
            {
                Console.WriteLine($"Intent name: {intent.Name}");
                Console.WriteLine($"Intent display name: {intent.DisplayName}");
                Console.WriteLine($"Action: {intent.Action}");
                Console.WriteLine($"Root follow-up intent: {intent.RootFollowupIntentName}");
                Console.WriteLine($"Parent follow-up intent: {intent.ParentFollowupIntentName}");

                Console.WriteLine($"Input contexts:");
                foreach (var inputContextName in intent.InputContextNames)
                {
                    Console.WriteLine($"Input context name: {inputContextName}");
                }

                Console.WriteLine($"Output contexts:");
                foreach (var outputContex in intent.OutputContexts)
                {
                    Console.WriteLine($"Output context name: {outputContex.Name}");
                }
                Console.WriteLine("Messages:");
                foreach (var message in intent.Messages)
                {
                    if (message.Text != null)
                    {
                        foreach (var text in message.Text.Text_)
                        {
                            Console.WriteLine($"Message text: {text}");
                        }
                    }
                }
                Console.WriteLine($"Training Phrases ({intent.TrainingPhrases.Count})");
                foreach (var trainingPhrase in intent.TrainingPhrases)
                {
                    Console.WriteLine($"Phrase name: {trainingPhrase.Name}");
                    Console.WriteLine($"Phrase type: {trainingPhrase.Type}");
                    foreach (var phrasePart in trainingPhrase.Parts)
                    {
                        Console.WriteLine($"Phrase part: {phrasePart.Text}");
                    }
                }
                Console.WriteLine();
            }

            return 0;
        }
        // [END dialogflow_list_intents]

        [Verb("intents:delete", HelpText = "Delete specified Intent")]
        public class DeleteOptions : OptionsWithProjectId
        {
            [Value(0, MetaName = "intentId", HelpText = "ID of existing Intent", Required = true)]
            public IEnumerable<string> IntentIds { get; set; }
        }

        // [START dialogflow_delete_intent]
        public static int Delete(string projectId, string intentId)
        {
            var client = IntentsClient.Create();
            client.DeleteIntent(new IntentName(projectId, intentId));

            Console.WriteLine($"Deleted Intent: {intentId}");

            return 0;
        }
        // [END dialogflow_delete_intent]

        public static int BatchDelete(string projectId,
            IEnumerable<string> intentIds)
        {
            var client = IntentsClient.Create();
            var intents = intentIds.Select(id => new Intent()
            {
                Name = new IntentName(projectId, id).ToString()
            });
            client.BatchDeleteIntents(new AgentName(projectId), intents);
            return 0;
        }
    }
}
