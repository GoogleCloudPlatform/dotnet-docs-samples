/*
 * Copyright 2026 Google LLC
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

// [START secretmanager_enable_regional_secret_managed_rotation]

using Google.Cloud.SecretManager.V1;
using System;

public class EnableRegionalSecretManagedRotationSample
{
    /// <summary>
    /// Enables managed rotation of a CLOUD_SQL_DB_CREDENTIALS typed secret.
    /// It validates and enables the rotation, adding a version and sets the
    /// passed password (optional).
    /// Note: AddSecretVersion is disabled on the CLOUD_SQL_DB_CREDENTIALS
    /// currently and for any necessary manual rotations please trigger
    /// rotate_secret.
    /// </summary>
    public SecretVersion EnableRegionalSecretManagedRotation(
      string projectId = "my-project",
      string locationId = "my-location",
      string secretId = "my-secret",
      string instanceId = "my-instance",
      string username = "my-user"
    )
    {
        // Create the Regional Secret Manager Client.
        SecretManagerServiceClient client = new SecretManagerServiceClientBuilder
        {
            Endpoint = $"secretmanager.{locationId}.rep.googleapis.com"
        }.Build();

        // Build the request.
        EnableManagedRotationRequest request = new EnableManagedRotationRequest
        {
            ParentAsSecretName = SecretName.FromProjectLocationSecret(projectId, locationId, secretId),
            CloudSqlSingleUserCredentials = new EnableManagedRotationRequest.Types.CloudSQLSingleUserCredentials
            {
                InstanceId = instanceId,
                Username = username,
            },
        };

        // Enable managed rotation.
        SecretVersion version = client.EnableManagedRotation(request);
        Console.WriteLine($"Enabled managed rotation, created secret version: {version.Name}");
        return version;
    }
}
// [END secretmanager_enable_regional_secret_managed_rotation]
