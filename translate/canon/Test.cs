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

using System;
using System.Collections.Generic;
using Google.Cloud.Translation.V2;
using Xunit;

public class TranslateTest
{
    readonly TranslateSample _sample = new TranslateSample();

    [Fact]
    public void TestTranslateText()
    {
        string translatedText = _sample.TranslateText();
        Assert.False(string.IsNullOrWhiteSpace(translatedText));
    }

    [Fact]
    public void TestListLanguageCodes()
    {
        IList<Language> languages = _sample.ListLanguageCodes();
        Assert.NotEmpty(languages);
    }

    [Fact]
    public void TestListLanguageNames()
    {
        IList<Language> languages = _sample.ListLanguageNames();
        Assert.NotEmpty(languages);
    }

    [Fact]
    public void TestDetectLanguage()
    {
        Detection detection = _sample.DetectLanguage();
        Assert.Equal("EN", detection.Language.ToUpper());
    }

    [Fact]
    public void TestTranslateTextWithModel()
    {
        TranslationResult result = _sample.TranslateTextWithModel();
        Assert.False(string.IsNullOrWhiteSpace(result.TranslatedText));
    }
}
