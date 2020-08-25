// Copyright 2020 Google Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

// [START storage_generate_encryption_key]

using Google.Cloud.Storage.V1;
using System;

public class GenerateEncryptionKeySample
{
    public string GenerateEncryptionKey()
    {
        var encryptionKey = EncryptionKey.Generate().Base64Key;
        Console.WriteLine($"Generated Base64-encoded AES-256 encryption key: {encryptionKey}");
        return encryptionKey;
    }
}
// [END storage_generate_encryption_key]
