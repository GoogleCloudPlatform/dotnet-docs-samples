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
// [START translate_detect_language]

using Google.Cloud.Translation.V2;
using System;

public partial class TranslateSample
{
    public Detection DetectLanguage()
    {
        TranslationClient client = TranslationClient.Create();
        var detection = client.DetectLanguage(text: "Hello world.");
        Console.WriteLine(
            $"{detection.Language}\tConfidence: {detection.Confidence}");
        return detection;
    }
}
// [END translate_detect_language]
