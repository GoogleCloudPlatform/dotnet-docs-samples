// Copyright 2018 Google Inc.
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

// [START iam_create_key]

using System;
using System.Text;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Iam.v1;
using Google.Apis.Iam.v1.Data;

public partial class ServiceAccountKeys
{
    public static ServiceAccountKey CreateKey(string serviceAccountEmail)
    {
        var credential = GoogleCredential.GetApplicationDefault()
            .CreateScoped(IamService.Scope.CloudPlatform);
        var service = new IamService(new IamService.Initializer
        {
            HttpClientInitializer = credential
        });

        var key = service.Projects.ServiceAccounts.Keys.Create(
            new CreateServiceAccountKeyRequest(),
            "projects/-/serviceAccounts/" + serviceAccountEmail)
            .Execute();

        // The PrivateKeyData field contains the base64-encoded service account key
        // in JSON format.
        // TODO(Developer): Save the below key (jsonKeyFile) to a secure location.
        //  You cannot download it later.
        byte[] valueBytes = System.Convert.FromBase64String(key.PrivateKeyData);
        string jsonKeyContent = Encoding.UTF8.GetString(valueBytes);

        Console.WriteLine("Key created successfully");
        return key;
    }
}
// [END iam_create_key]
