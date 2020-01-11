// Copyright(c) 2020 Google Inc.
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

// [START secretmanager_quickstart]

using System;
using System.Linq;
using System.Text;
using Google.Api.Gax.ResourceNames;
using Google.Protobuf;

// Imports the Secret Manager client library
using Google.Cloud.SecretManager.V1Beta1;

namespace GoogleCloudSamples
{
    public class QuickStart
    {
        public static void Main(string[] args)
        {
            // GCP project in which to store secrets in Secret Manager.
            string projectId = "YOUR-PROJECT-ID";

            // ID of the secret to create.
            string secretId = "YOUR-SECRET-ID";

            // [END secretmanager_quickstart]
            if (args.Length > 1)
            {
                projectId = args[0];
                secretId = args[1];
            }
            // [START secretmanager_quickstart]
            // Create a Secret Manager client.
            SecretManagerServiceClient client = SecretManagerServiceClient.Create();

            // Create the parent secret.
            var createSecretRequest = new CreateSecretRequest
            {
                ParentAsProjectName = new ProjectName(projectId),
                SecretId = secretId,
                Secret = new Secret
                {
                    Replication = new Replication
                    {
                        Automatic = new Replication.Types.Automatic(),
                    },
                },
            };

            var secret = client.CreateSecret(createSecretRequest);

            // Add a secret version.
            var addSecretVersionRequest = new AddSecretVersionRequest
            {
                ParentAsSecretName = secret.SecretName,
                Payload = new SecretPayload
                {
                    Data = ByteString.CopyFrom("my super secret data", Encoding.UTF8),
                },
            };

            var version = client.AddSecretVersion(addSecretVersionRequest);

            // Access the secret version.
            var accessSecretVersionRequest = new AccessSecretVersionRequest
            {
                SecretVersionName = version.SecretVersionName,
            };

            var result = client.AccessSecretVersion(accessSecretVersionRequest);

            // Print the results
            //
            // WARNING: Do not print secrets in production environments. This
            // snippet is for demonstration purposes only.
            string payload = result.Payload.Data.ToStringUtf8();
            Console.WriteLine($"Plaintext: {payload}");
        }
    }
}
// [END secretmanager_quickstart]
