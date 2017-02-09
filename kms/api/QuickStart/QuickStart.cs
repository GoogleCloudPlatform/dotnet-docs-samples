/*
 * Copyright (c) 2017 Google Inc.
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

// [START kms_quickstart]

using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using System;
// Imports the Google Cloud KMS client library
using Google.Apis.CloudKMS.v1beta1;
using Google.Apis.CloudKMS.v1beta1.Data;
using System.Linq;

namespace GoogleCloudSamples
{
    public class QuickStart
    {
        public static void Main(string[] args)
        {
            // Your Google Cloud Platform project ID.
            string projectId = "YOUR-PROJECT-ID";

            // Lists keys in the "global" location.
            string location = "global";

            // The resource name of the location associated with the key rings.
            string parent = $"projects/{projectId}/locations/{location}";

            // Authorize the client using Application Default Credentials.
            // See: https://developers.google.com/identity/protocols/application-default-credentials
            GoogleCredential credential = GoogleCredential.GetApplicationDefaultAsync().Result;
            // Specify the Cloud Key Management Service scope.
            if (credential.IsCreateScopedRequired)
            {
                credential = credential.CreateScoped(new[]
                {
                    Google.Apis.CloudKMS.v1beta1.CloudKMSService.Scope.CloudPlatform
                });
            }
            // Instantiate the Cloud Key Management Service API.
            CloudKMSService cloudKms = new CloudKMSService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                GZipEnabled = false
            });

            // List key rings.
            ListKeyRingsResponse result = cloudKms.Projects.Locations.KeyRings.List(parent).Execute();
            if (result.KeyRings != null)
            {
                Console.WriteLine("Key Rings: ");
                result.KeyRings.ToList().ForEach(response => Console.WriteLine(response.Name));
            }
            else { Console.WriteLine("No Key Rings found."); }
        }
    }
}
// [END kms_quickstart]
