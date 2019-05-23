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
using Google.Cloud.Firestore;

/// <summary>
/// The record that gets stored in Firestore.
/// </summary>
[FirestoreData]
public class Translation
{
    [FirestoreProperty]
    public string SourceText { get; set; }

    [FirestoreProperty]
    public string TranslatedText { get; set; }

    [FirestoreProperty]
    public DateTime TimeStamp { get; set; }
}
