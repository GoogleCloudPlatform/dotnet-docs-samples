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

using System;
using System.Linq;
// Imports the Google Cloud KMS client library
using Google.Cloud.Kms.V1;

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
            LocationName locationName = new LocationName(projectId, location);

            // Instantiate a Cloud KMS client.
            KeyManagementServiceClient client = KeyManagementServiceClient.Create();

            // List key rings.
            foreach (KeyRing keyRing in client.ListKeyRings(locationName))
            {
                Console.WriteLine(keyRing.Name);
            }
        }
    }
}
// [END kms_quickstart]
