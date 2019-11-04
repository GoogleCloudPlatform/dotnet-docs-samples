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
//
// [START translate_text_with_model]

using Google.Cloud.Translation.V2;
using System;

public partial class TranslateSample
{
    public TranslationResult TranslateTextWithModel()
    {
        TranslationClient client = TranslationClient.Create();
        TranslationResult result = client.TranslateText(
            text: "Hello World.",
            targetLanguage: "ja",  // Japanese
            sourceLanguage: "en",  // English
            model: TranslationModel.NeuralMachineTranslation);
        Console.WriteLine($"Model: {result.Model}");
        Console.WriteLine(result.TranslatedText);
        return result;
    }
}
// [END translate_text_with_model]
