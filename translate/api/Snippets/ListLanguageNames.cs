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
// [START translate_list_language_names]

using Google.Cloud.Translation.V2;
using System;
using System.Collections.Generic;

public partial class TranslateSample
{
    public IList<Language> ListLanguageNames()
    {
        TranslationClient client = TranslationClient.Create();
        // List all the languages in English.
        IList<Language> languages = client.ListLanguages(target: "en");
        foreach (var language in languages)
        {
            Console.WriteLine($"{language.Code}\t{language.Name}");
        }
        return languages;
    }
}
// [END translate_list_language_names]
