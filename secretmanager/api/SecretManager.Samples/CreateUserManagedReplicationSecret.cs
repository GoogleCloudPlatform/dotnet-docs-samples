/*
 * Copyright 2022 Google LLC
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

using Google.Api.Gax.ResourceNames;
using Google.Cloud.SecretManager.V1;
using System.Linq;

public class CreateUserManagedReplicationSecretSample
{
    public Secret CreateUserManagedReplicationSecret(string projectId, string secretId, string[] locations)
    {
        // Create the client.
        SecretManagerServiceClient client = SecretManagerServiceClient.Create();

        // Build the parent resource name.
        ProjectName projectName = new ProjectName(projectId);

        // Build the secret.
        Secret secret = new Secret
        {
            Replication = new Replication
            {
                UserManaged = new Replication.Types.UserManaged
                {
                    Replicas = 
                    { 
                        locations.Select(location => new Replication.Types.UserManaged.Types.Replica 
                        {
                            Location = location 
                        })
                    }
                }
            },
        };

        // Call the API.
        Secret createdSecret = client.CreateSecret(projectName, secretId, secret);
        return createdSecret;
    }
}
