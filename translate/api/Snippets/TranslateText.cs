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
// [START translate_translate_text]

using Google.Cloud.Translation.V2;
using System;

public partial class TranslateSample
{
    public string TranslateText()
    {
        TranslationClient client = TranslationClient.Create();
        var response = client.TranslateText(
            text: "Hello World.",
            targetLanguage: "ru",  // Russian
            sourceLanguage: "en");  // English
        Console.WriteLine(response.TranslatedText);
        return response.TranslatedText;
    }
}
// [END translate_translate_text]
