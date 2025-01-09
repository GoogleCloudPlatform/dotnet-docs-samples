/*
 * Copyright 2024 Google LLC
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     https://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

// [START secretmanager_regional_quickstart]

using Google.Api.Gax.ResourceNames;
using Google.Cloud.SecretManager.V1;
using Google.Protobuf;
using System;
using System.Text;

public class RegionalQuickstartSample
{
    public void RegionalQuickstart(
      string projectId = "my-project",
      string locationId = "my-location",
      string secretId = "my-secret"
    )
    {
        // Create the Regional Secret Manager Client.
        SecretManagerServiceClient client = new SecretManagerServiceClientBuilder
        {
            Endpoint = $"secretmanager.{locationId}.rep.googleapis.com"
        }.Build();

        // Build the parent resource name.
        LocationName locationName = new LocationName(projectId, locationId);

        // Build the secret to create.
        Secret secret = new Secret { };


        Secret createdSecret = client.CreateSecret(locationName, secretId, secret);

        // Build a payload.
        SecretPayload payload = new SecretPayload
        {
            Data = ByteString.CopyFrom("my super secret data", Encoding.UTF8),
        };

        // Add a secret version.
        SecretVersion createdVersion = client.AddSecretVersion(createdSecret.SecretName, payload);

        // Access the secret version.
        AccessSecretVersionResponse result = client.AccessSecretVersion(createdVersion.SecretVersionName);

        // Print the results
        // WARNING: Do not print secrets in production environments. This
        // snippet is for demonstration purposes only.
        string data = result.Payload.Data.ToStringUtf8();
        Console.WriteLine($"Plaintext: {data}");
    }
}
// [END secretmanager_regional_quickstart]
