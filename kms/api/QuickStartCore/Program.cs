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
using Google.Apis.CloudKMS.v1;
using Google.Apis.CloudKMS.v1.Data;
using System.Text;

namespace GoogleCloudSamples
{
    public class QuickStart
    {
        public static int Main(string[] args)
        {
            // Your Google Cloud Platform project ID.
            string projectId = "YOUR-PROJECT-ID";

            if (projectId == "YOUR-" + "PROJECT-ID")
            {
                Console.Error.WriteLine("Modify Program.cs and replace YOUR-"
                    + "PROJECT-ID with your google project id.");
                return -1;
            }

            // Authorize the client using Application Default Credentials.
            // See: https://developers.google.com/identity/protocols/application-default-credentials
            GoogleCredential credential = 
                GoogleCredential.GetApplicationDefaultAsync().Result;
            // Specify the Cloud Key Management Service scope.
            if (credential.IsCreateScopedRequired) 
            {
                credential = credential.CreateScoped(new[]
                {
                    CloudKMSService.Scope.CloudPlatform
                });
            }
            var cloudKms = 
                new CloudKMSService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                GZipEnabled = false
            });

            // Create the key ring.
            string location = "global";
            // The resource name of the location associated with the key rings.
            string parent = $"projects/{projectId}/locations/{location}";
            KeyRing keyRingToCreate = new KeyRing();
            var request = new ProjectsResource.LocationsResource
                .KeyRingsResource.CreateRequest(cloudKms, keyRingToCreate, parent);
            string keyRingId = request.KeyRingId = "QuickStartCore";
            try
            {
                request.Execute();
            }
            catch (Google.GoogleApiException e)
            when (e.HttpStatusCode == System.Net.HttpStatusCode.Conflict)
            {
                // Already exists.  Ok.
            }
            
            // Create the crypto key:
            var keyRingName = string.Format(
                "projects/{0}/locations/{1}/keyRings/{2}",
                projectId, location, keyRingId);
            string rotationPeriod = string.Format("{0}s",
                    TimeSpan.FromDays(7).TotalSeconds);
            CryptoKey cryptoKeyToCreate = new CryptoKey()
            {
                Purpose = "ENCRYPT_DECRYPT",
                NextRotationTime = DateTime.UtcNow.AddDays(7),
                RotationPeriod = rotationPeriod
            };
            string keyId = "Key1";
            string keyName;
            try
            {
                keyName = new ProjectsResource.LocationsResource
                .KeyRingsResource.CryptoKeysResource.CreateRequest(
                cloudKms, cryptoKeyToCreate, keyRingName)
                { CryptoKeyId = keyId }.Execute().Name;
            }
            catch (Google.GoogleApiException e)
                when(e.HttpStatusCode == System.Net.HttpStatusCode.Conflict)
            {
                // Already exists.  Ok.
                keyName = string.Format("{0}/cryptoKeys/{1}",
                    keyRingName, keyId);
            }

            // Encrypt a string.
            var encryptResult = cloudKms.Projects.Locations.KeyRings.CryptoKeys
                .Encrypt(new EncryptRequest()
            {
                Plaintext = Convert.ToBase64String(Encoding.UTF8.GetBytes("Hello World."))
            }, keyName).Execute();
            var cipherText = 
                Convert.FromBase64String(encryptResult.Ciphertext);            

            // Decrypt the string.
            var result = cloudKms.Projects.Locations.KeyRings.CryptoKeys
                .Decrypt(new DecryptRequest()
            {
                Ciphertext = Convert.ToBase64String(cipherText)
            }, keyName).Execute();
            Console.WriteLine(Encoding.UTF8.GetString(Convert.FromBase64String(result.Plaintext)));
            return 0;
        }
    }
}
// [END kms_quickstart]
