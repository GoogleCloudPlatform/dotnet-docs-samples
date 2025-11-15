/*
 * Copyright 2025 Google LLC
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     https:www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

// [START translate_translate_text]

using Google.Cloud.Translation.V2;

public class TranslateTextSample
{
    /// <summary>
    /// Translate the given text into the specified targetLanguage.
    /// </summary>
    /// <param name="targetLanguage">The ISO 639 language code to translate the text into.</param>
    /// <param name="sourceLanguage">
    /// Optional. The ISO 639 language code of the input text. If null, the API will attempt
    /// to detect the source language automatically.
    /// </param>
    /// <param name="text">The text to translate.</param>
    /// <returns>
    /// An Execute object representing the completed workflow execution.
    /// </returns>
    public async Task<TranslationResult> TranslateText(
        string targetLanguage,
        string? sourceLanguage,
        string text)
    {
        // Create TranslationClient.
        TranslationClient client = await TranslationClient.CreateAsync();

        // Translate text.
        TranslationResult translationResult = client.TranslateText(text, targetLanguage, sourceLanguage);

        // Print results.
        Console.WriteLine($"Input text: {translationResult.OriginalText}");
        Console.WriteLine($"Translated text: {translationResult.TranslatedText}");

        return translationResult;
    }
}
// [END translate_translate_text]
