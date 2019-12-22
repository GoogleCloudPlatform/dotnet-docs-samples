/*
 * Copyright (c) 2019 Google Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not
 * use this file except in compliance with the License. You may obtain a copy of
 * the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
 * License for the specific language governing permissions and limitations under
 * the License.
 */

// [START secretmanager_quickstart]

using System;
using System.Linq;
using Google.Api.Gax.ResourceNames;
// Imports the Secret Manager client library
using Google.Cloud.Secrets.V1Beta1;

namespace GoogleCloudSamples
{
    public class QuickStart
    {
        public static void Main(string[] args)
        {
            // Your Google Cloud Platform project ID.
            string projectId = "YOUR-PROJECT-ID";

            // Instantiate a Secret Manager client.
            SecretManagerServiceClient client = SecretManagerServiceClient.Create();

            // Create the list secrets request.
            var request = new ListSecretsRequest{
              ParentAsProjectName = new ProjectName(projectId),
            };

            // List secrets in the project.
            foreach (var secret in client.ListSecrets(request))
            {
                Console.WriteLine(secret.Name);
            }
        }
    }
}
// [END secretmanager_quickstart]
