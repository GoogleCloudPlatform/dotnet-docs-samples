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

using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using System;
using CommandLine;
using Google.Apis.CloudKMS.v1beta1;
using Google.Apis.CloudKMS.v1beta1.Data;
using System.Linq;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace GoogleCloudSamples
{
    [Verb("encrypt", HelpText = "Encrypt the data in a text file using Cloud KMS.")]
    class EncryptOptions : EncryptionOptions { }

    [Verb("decrypt", HelpText = "Decrypt the data in a text file using Cloud KMS.")]
    class DecryptOptions : EncryptionOptions { }

    class EncryptionOptions
    {
        [Value(0, HelpText = "The project containing the crypto key.", Required = true)]
        public string projectId { get; set; }

        [Value(1, HelpText = "The location of the crypto key. Ex. 'global'", Required = true)]
        public string location { get; set; }

        [Value(2, HelpText = "The name of the key ring containing the crypto key.", Required = true)]
        public string keyRing { get; set; }

        [Value(3, HelpText = "The name of the crypto key.", Required = true)]
        public string keyName { get; set; }

        [Value(4, HelpText = "The name of the input file to encrypt or decrypt.", Required = true)]
        public string inFile { get; set; }

        [Value(5, HelpText = "The name of the output file.", Required = true)]
        public string outFile { get; set; }
    }

    [Verb("listKeyRings", HelpText = "List all key rings associated with your project for a given location.")]
    class ListKeyRingsOptions
    {
        [Value(0, HelpText = "The project ID of the project to use when listing Cloud KMS resources.", Required = true)]
        public string projectId { get; set; }
        [Value(1, HelpText = "The location to use when listing Cloud KMS resources. Ex. 'global'", Required = true)]
        public string location { get; set; }
    }

    [Verb("listCryptoKeys", HelpText = "List all crypto keys for the specified key ring.")]
    class ListCryptoKeysOptions : KeyRingOptions { }

    [Verb("createKeyRing", HelpText = "Create a key ring for holding crypto keys.")]
    class CreateKeyRingOptions : KeyRingOptions { }

    [Verb("getKeyRing", HelpText = "Get a key ring's full path details and its create time.")]
    class GetKeyRingOptions : KeyRingOptions { }

    class KeyRingOptions
    {
        [Value(0, HelpText = "The project ID of the project to use when accessing Cloud KMS resources.", Required = true)]
        public string projectId { get; set; }
        [Value(1, HelpText = "The location to use when accessing Cloud KMS resources. Ex. 'global'", Required = true)]
        public string location { get; set; }
        [Value(2, HelpText = "The name of the key ring to use when accessing Cloud KMS resources.", Required = true)]
        public string keyRing { get; set; }
    }

    [Verb("createCryptoKey", HelpText = "Create a crypto key.")]
    class CreateCryptoKeyOptions : CryptoKeyOptions { }

    [Verb("getCryptoKey", HelpText = "Get an crypto key's details.")]
    class GetCryptoKeyOptions : CryptoKeyOptions { }

    class CryptoKeyOptions
    {
        [Value(0, HelpText = "The project ID of the project to use when accessing Cloud KMS resources.", Required = true)]
        public string projectId { get; set; }
        [Value(1, HelpText = "The location to use when accessing Cloud KMS resources. Ex. 'global'", Required = true)]
        public string location { get; set; }
        [Value(2, HelpText = "The name of the key ring holding the crypto key.", Required = true)]
        public string keyRing { get; set; }
        [Value(3, HelpText = "The name of the crypto key.", Required = true)]
        public string cryptoKey { get; set; }
    }

    [Verb("disableCryptoKeyVersion", HelpText = "Disable a crypto key version.")]
    class DisableCryptoKeyVersionOptions : CryptoKeyVersionOptions { }

    [Verb("enableCryptoKeyVersion", HelpText = "Enable a crypto key version.")]
    class EnableCryptoKeyVersionOptions : CryptoKeyVersionOptions { }

    [Verb("getCryptoKeyVersion", HelpText = "Get a crypto key version.")]
    class GetCryptoKeyVersionOptions : CryptoKeyVersionOptions { }

    [Verb("destroyCryptoKeyVersion", HelpText = "Destroy a crypto key version.")]
    class DestroyCryptoKeyVersionOptions : CryptoKeyVersionOptions { }

    [Verb("restoreCryptoKeyVersion", HelpText = "Restore a crypto key version.")]
    class RestoreCryptoKeyVersionOptions : CryptoKeyVersionOptions { }

    class CryptoKeyVersionOptions
    {
        [Value(0, HelpText = "The project ID of the project to use when updating Cloud KMS resources.", Required = true)]
        public string projectId { get; set; }
        [Value(1, HelpText = "The location to use when updating Cloud KMS resources. Ex. 'global'", Required = true)]
        public string location { get; set; }
        [Value(2, HelpText = "The name of the key ring that contains the crypto key to update.", Required = true)]
        public string keyRing { get; set; }
        [Value(3, HelpText = "The name of the crypto key to update.", Required = true)]
        public string cryptoKey { get; set; }
        [Value(4, HelpText = "The name of the crypto key version to update.", Required = true)]
        public string version { get; set; }
    }

    [Verb("getCryptoKeyIamPolicy", HelpText = "Get a crypto key's IAM policy.")]
    class GetCryptoKeyIamPolicyOptions
    {
        [Value(0, HelpText = "The project ID of the project to use when listing Cloud KMS resources.", Required = true)]
        public string projectId { get; set; }
        [Value(1, HelpText = "The location to use when listing Cloud KMS resources. Ex. 'global'", Required = true)]
        public string location { get; set; }
        [Value(2, HelpText = "The name of the keyRing containing the crypto key.", Required = true)]
        public string keyRing { get; set; }
        [Value(3, HelpText = "The name of the crypto key to get the IAM policy details for.", Required = true)]
        public string cryptoKey { get; set; }
    }


    [Verb("addMemberToCryptoKeyPolicy", HelpText = "Add member to crypto key's IAM policy.")]
    class AddMemberToCryptoKeyPolicyOptions
    {
        [Value(0, HelpText = "The project ID of the project to use when updating Cloud KMS resources.", Required = true)]
        public string projectId { get; set; }
        [Value(1, HelpText = "The location to use when updating Cloud KMS resources. Ex. 'global'", Required = true)]
        public string location { get; set; }
        [Value(2, HelpText = "The name of the keyRing containing the crypto key.", Required = true)]
        public string keyRing { get; set; }
        [Value(3, HelpText = "The name of the crypto key to set the IAM policy details for.", Required = true)]
        public string cryptoKey { get; set; }
        [Value(4, HelpText = "The role to add to the keyRing's IAM policy.", Required = true)]
        public string role { get; set; }
        [Value(5, HelpText = "The member to add to the keyRing's IAM policy.", Required = true)]
        public string member { get; set; }
    }

    [Verb("removeMemberFromCryptoKeyPolicy", HelpText = "Remove member from crypto key's IAM policy.")]
    class RemoveMemberFromCryptoKeyPolicyOptions
    {
        [Value(0, HelpText = "The project ID of the project to use when updating Cloud KMS resources.", Required = true)]
        public string projectId { get; set; }
        [Value(1, HelpText = "The location to use when updating Cloud KMS resources. Ex. 'global'", Required = true)]
        public string location { get; set; }
        [Value(2, HelpText = "The name of the keyRing containing the crypto key.", Required = true)]
        public string keyRing { get; set; }
        [Value(3, HelpText = "The name of the crypto key to set the IAM policy details for.", Required = true)]
        public string cryptoKey { get; set; }
        [Value(4, HelpText = "The role to remove to the keyRing's IAM policy.", Required = true)]
        public string role { get; set; }
        [Value(5, HelpText = "The member to remove to the keyRing's IAM policy.", Required = true)]
        public string member { get; set; }
    }

    [Verb("getKeyRingIamPolicy", HelpText = "Get a keyRing's IAM policy.")]
    class GetKeyRingIamPolicyOptions
    {
        [Value(0, HelpText = "The project ID of the project to use when listing Cloud KMS resources.", Required = true)]
        public string projectId { get; set; }
        [Value(1, HelpText = "The location to use when listing Cloud KMS resources. Ex. 'global'", Required = true)]
        public string location { get; set; }
        [Value(2, HelpText = "The name of the keyRing to get the IAM policy details for.", Required = true)]
        public string keyRing { get; set; }
    }

    [Verb("addMemberToKeyRingPolicy", HelpText = "Add member to a keyRing's IAM policy.")]
    class AddMemberToKeyRingPolicyOptions
    {
        [Value(0, HelpText = "The project ID of the project to use when updating Cloud KMS resources.", Required = true)]
        public string projectId { get; set; }
        [Value(1, HelpText = "The location to use when updating Cloud KMS resources. Ex. 'global'", Required = true)]
        public string location { get; set; }
        [Value(2, HelpText = "The name of the keyRing to set the IAM policy details for.", Required = true)]
        public string keyRing { get; set; }
        [Value(3, HelpText = "The role to add to the keyRing's IAM policy.", Required = true)]
        public string role { get; set; }
        [Value(4, HelpText = "The member to add to the keyRing's IAM policy.", Required = true)]
        public string member { get; set; }
    }

    public class CloudKmsSample
    {
        private static readonly string s_projectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");

        /// <summary>
        /// Creates an authorized Cloud Key Management Service client using Application 
        /// Default Credentials.
        /// </summary>
        /// <returns>an authorized Cloud Key Management Service client.</returns>
        public static CloudKMSService CreateAuthorizedClient()
        {
            GoogleCredential credential =
                GoogleCredential.GetApplicationDefaultAsync().Result;
            // Inject the Cloud Key Management Service scope
            if (credential.IsCreateScopedRequired)
            {
                credential = credential.CreateScoped(new[]
                {
                    CloudKMSService.Scope.CloudPlatform
                });
            }
            return new CloudKMSService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                GZipEnabled = false
            });
        }

        // [START kms_list_keyrings]
        public static object ListKeyRings(string projectId, string location)
        {
            var cloudKms = CreateAuthorizedClient();
            // The resource name of the location associated with the key rings.
            var parent = $"projects/{projectId}/locations/{location}";
            try
            {
                var result = cloudKms.Projects.Locations.KeyRings.List(parent).Execute();
                Console.WriteLine("KeyRings: ");
                result.KeyRings.ToList().ForEach(response => Console.WriteLine(response.Name));
            }
            catch (Google.GoogleApiException e)
            {
                Console.WriteLine(e.Message);
                return e.Error.Code;
            }
            return 0;
        }
        // [END kms_list_keyrings]

        // [START kms_list_cryptokeys]
        public static object ListCryptoKeys(string projectId, string location, string keyRing)
        {
            var cloudKms = CreateAuthorizedClient();
            // Generate the full path of the parent to use for listing crypto keys.
            var parent = $"projects/{projectId}/locations/{location}/keyRings/{keyRing}";
            var result = cloudKms.Projects.Locations.KeyRings.CryptoKeys.List(parent).Execute();
            Console.WriteLine("Crypto Keys: ");
            result.CryptoKeys.ToList().ForEach(response =>
            {
                Console.WriteLine(response.Name);
                Console.WriteLine($"  Created:{response.CreateTime}");
                Console.WriteLine($"  Purpose: {response.Purpose}");
                Console.WriteLine($"  Primary: {response.Primary}");
                Console.WriteLine($"    State: {response.Primary.State}");
                Console.WriteLine($"    Created: {response.Primary.CreateTime}");
            });
            return 0;
        }
        // [END kms_list_cryptokeys]

        // [START kms_create_keyring]
        public static object CreateKeyRing(string projectId, string location, string keyRing)
        {
            var cloudKms = CreateAuthorizedClient();
            // Generate the full path of the parent to use for creating key rings.
            var parent = $"projects/{projectId}/locations/{location}";
            KeyRing keyRingToCreate = new KeyRing();
            var request = new ProjectsResource.LocationsResource.KeyRingsResource.CreateRequest(
                cloudKms, keyRingToCreate, parent);
            request.KeyRingId = keyRing;
            var result = request.Execute();
            Console.Write($"Created Key Ring: {result.Name}");
            return 0;
        }
        // [END kms_create_keyring]

        // [START kms_get_keyring]
        public static object GetKeyRing(string projectId, string location, string keyRing)
        {
            var cloudKms = CreateAuthorizedClient();
            // Generate the full path of the parent to use for getting the key ring.
            var parent = $"projects/{projectId}/locations/{location}/keyRings/{keyRing}";
            var result = cloudKms.Projects.Locations.KeyRings.Get(parent).Execute();
            Console.WriteLine($"Found KeyRing: {result.Name}");
            Console.WriteLine($"  Created on:{result.CreateTime}");
            return 0;
        }
        // [END kms_get_keyring]

        // [START kms_get_cryptokey]
        public static object GetCryptoKey(string projectId, string location, string keyRing, string cryptoKey)
        {
            var cloudKms = CreateAuthorizedClient();
            // Generate the full path of the parent to use for getting the key ring.
            var parent = $"projects/{projectId}/locations/{location}/keyRings/{keyRing}/cryptoKeys/{cryptoKey}";
            var result = cloudKms.Projects.Locations.KeyRings.CryptoKeys.Get(parent).Execute();
            Console.WriteLine($"Name: {result.Name}");
            Console.WriteLine($"Created: {result.CreateTime}");
            Console.WriteLine($"Purpose:{result.Purpose}");
            Console.WriteLine($"Primary: {result.Primary}");
            Console.WriteLine($"  State: { result.Primary.State}");
            Console.WriteLine($"  Created: { result.Primary.CreateTime}");
            return 0;
        }
        // [END kms_get_cryptokey]

        // [START kms_create_cryptokey]
        public static object CreateCryptoKey(string projectId, string location, string keyRing, string cryptoKey)
        {
            var cloudKms = CreateAuthorizedClient();
            // Generate the full path of the parent to use for creating the crypto key.
            var parent = $"projects/{projectId}/locations/{location}/keyRings/{keyRing}";
            CryptoKey cryptoKeyToCreate = new CryptoKey();
            cryptoKeyToCreate.Purpose = "ENCRYPT_DECRYPT";
            var request = new ProjectsResource.LocationsResource.KeyRingsResource.CryptoKeysResource.CreateRequest(
                cloudKms, cryptoKeyToCreate, parent);
            request.CryptoKeyId = cryptoKey;
            var result = request.Execute();
            Console.Write($"Created Crypto Key: {result.Name}");
            return 0;
        }
        // [END kms_create_cryptokey]


        // [START kms_disable_cryptokey_version]
        public static object DisableCryptoKeyVersion(string projectId, string location, string keyRing, string cryptoKey, string version)
        {
            var cloudKms = CreateAuthorizedClient();
            // Generate the full path of the parent to use for disabling the crypto key Version.
            var parent = $"projects/{projectId}/locations/{location}/keyRings/{keyRing}/cryptoKeys/{cryptoKey}/cryptoKeyVersions/{version}";
            // Get crypto key version.
            var request = new ProjectsResource.LocationsResource.KeyRingsResource.CryptoKeysResource
                .CryptoKeyVersionsResource.GetRequest(cloudKms, parent);
            var result = request.Execute();
            result.State = "DISABLED";
            var patchRequest = new ProjectsResource.LocationsResource.KeyRingsResource.CryptoKeysResource
                .CryptoKeyVersionsResource.PatchRequest(cloudKms, result, parent);
            patchRequest.UpdateMask = "state";
            var patchResult = patchRequest.Execute();
            Console.Write($"Disabled Crypto Key Version: {patchResult.Name}");
            return 0;
        }
        // [END kms_disable_cryptokey_version]

        // [START kms_enable_cryptokey_version]
        public static object EnableCryptoKeyVersion(string projectId, string location, string keyRing, string cryptoKey, string version)
        {
            var cloudKms = CreateAuthorizedClient();
            // Generate the full path of the parent to use for enabling the crypto key Version.
            var parent = $"projects/{projectId}/locations/{location}/keyRings/{keyRing}/cryptoKeys/{cryptoKey}/cryptoKeyVersions/{version}";
            // Get crypto key version.
            var request = new ProjectsResource.LocationsResource.KeyRingsResource.CryptoKeysResource
            .CryptoKeyVersionsResource.GetRequest(cloudKms, parent);
            var result = request.Execute();
            result.State = "ENABLED";
            var patchRequest = new ProjectsResource.LocationsResource.KeyRingsResource.CryptoKeysResource
                .CryptoKeyVersionsResource.PatchRequest(cloudKms, result, parent);
            patchRequest.UpdateMask = "state";
            var patchResult = patchRequest.Execute();
            Console.Write($"Enabled Crypto Key Version: {patchResult.Name}");
            return 0;
        }
        // [END kms_enable_cryptokey_version]

        // [START kms_get_cryptokey_version]
        public static object GetCryptoKeyVersion(string projectId, string location, string keyRing, string cryptoKey, string version)
        {
            var cloudKms = CreateAuthorizedClient();
            // Generate the full path of the parent to use for getting the crypto key Version.
            var parent = $"projects/{projectId}/locations/{location}/keyRings/{keyRing}/cryptoKeys/{cryptoKey}/cryptoKeyVersions/{version}";
            var result = cloudKms.Projects.Locations.KeyRings.CryptoKeys.CryptoKeyVersions.Get(parent).Execute();
            Console.WriteLine($"Name: {result.Name}");
            Console.WriteLine($"Created: {result.CreateTime}");
            return 0;
        }
        // [END kms_get_cryptokey_version]

        // [START kms_destroy_cryptokey_version]
        public static object DestroyCryptoKeyVersion(string projectId, string location, string keyRing, string cryptoKey, string version)
        {
            var cloudKms = CreateAuthorizedClient();
            // Generate the full path of the parent to use for destroying the crypto key Version.
            var parent = $"projects/{projectId}/locations/{location}/keyRings/{keyRing}/cryptoKeys/{cryptoKey}/cryptoKeyVersions/{version}";
            DestroyCryptoKeyVersionRequest destroyRequest = new DestroyCryptoKeyVersionRequest();
            // Destroy crypto key version.
            var request = new ProjectsResource.LocationsResource.KeyRingsResource.CryptoKeysResource
                .CryptoKeyVersionsResource.DestroyRequest(cloudKms, destroyRequest, parent);
            var result = request.Execute();
            Console.Write($"Destroyed Crypto Key Version: {result.Name}");
            return 0;
        }
        // [END kms_destroy_cryptokey_version]

        // [START kms_restore_cryptokey_version]
        public static object RestoreCryptoKeyVersion(string projectId, string location, string keyRing, string cryptoKey, string version)
        {
            var cloudKms = CreateAuthorizedClient();
            // Generate the full path of the parent to use for restoring the crypto key Version.
            var parent = $"projects/{projectId}/locations/{location}/keyRings/{keyRing}/cryptoKeys/{cryptoKey}/cryptoKeyVersions/{version}";
            RestoreCryptoKeyVersionRequest restoreRequest = new RestoreCryptoKeyVersionRequest();
            // Restore crypto key version.
            var request = new ProjectsResource.LocationsResource.KeyRingsResource.CryptoKeysResource
                .CryptoKeyVersionsResource.RestoreRequest(cloudKms, restoreRequest, parent);
            var result = request.Execute();
            Console.Write($"Restored Crypto Key Version: {result.Name}");
            return 0;
        }
        // [END kms_restore_cryptokey_version]

        // [START kms_get_cryptokey_policy]
        public static object GetCryptoKeyIamPolicy(string projectId, string location,
            string keyRing, string cryptoKey)
        {
            var cloudKms = CreateAuthorizedClient();
            // Generate the full path of the parent to use for getting the crypto key IAM policy.
            var parent = $"projects/{projectId}/locations/{location}/keyRings/{keyRing}/cryptoKeys/{cryptoKey}";
            var result = cloudKms.Projects.Locations.KeyRings.CryptoKeys.GetIamPolicy(parent).Execute();
            if (result.Bindings != null)
            {
                result.Bindings.ToList().ForEach(response =>
                {
                    Console.WriteLine($"Role: {response.Role}");
                    response.Members.ToList().ForEach(member =>
                    {
                        Console.WriteLine($"  Member: {member.ToString()}");
                    });
                });
            }
            else
            {
                Console.WriteLine($"Empty IAM policy found for CryptoKey: {parent}");
            }
            return 0;
        }
        // [END kms_get_cryptokey_policy]

        // [START kms_add_member_to_cryptokey_policy]
        public static object AddMemberToCryptoKeyPolicy(string projectId, string location,
            string keyRing, string cryptoKey, string role, string member)
        {
            var cloudKms = CreateAuthorizedClient();
            // Generate the full path of the parent to use for updating the crypto key IAM policy.
            var parent = $"projects/{projectId}/locations/{location}/keyRings/{keyRing}/cryptoKeys/{cryptoKey}";
            SetIamPolicyRequest setIamPolicyRequest = new SetIamPolicyRequest();
            var result = cloudKms.Projects.Locations.KeyRings.CryptoKeys.GetIamPolicy(parent).Execute();
            if (result.Bindings != null)
            {
                // Policy already exists, so add a new Binding to it.
                Binding bindingToAdd = new Binding();
                bindingToAdd.Role = role;
                string[] testMembers = { member };
                bindingToAdd.Members = testMembers;
                result.Bindings.Add(bindingToAdd);
                setIamPolicyRequest.Policy = result;
            }
            else
            {
                // Policy does not yet exist, so create a new one.
                Policy newPolicy = new Policy();
                newPolicy.Bindings = new List<Binding>();
                Binding bindingToAdd = new Binding();
                bindingToAdd.Role = role;
                string[] testMembers = { member };
                bindingToAdd.Members = testMembers;
                newPolicy.Bindings.Add(bindingToAdd);
                setIamPolicyRequest.Policy = newPolicy;
            }
            var request = new ProjectsResource.LocationsResource.KeyRingsResource.CryptoKeysResource
                .SetIamPolicyRequest(cloudKms, setIamPolicyRequest, parent);
            var setIamPolicyResult = request.Execute();
            var updateResult = cloudKms.Projects.Locations.KeyRings.CryptoKeys.GetIamPolicy(parent).Execute();
            updateResult.Bindings.ToList().ForEach(response =>
            {
                Console.WriteLine($"Role: {response.Role}");
                response.Members.ToList().ForEach(memberFound =>
                {
                    Console.WriteLine($"  Member: {memberFound}");
                });
            });
            return 0;
        }
        // [END kms_add_member_to_cryptokey_policy]

        // [START kms_remove_member_from_cryptokey_policy]
        public static object RemoveMemberFromCryptoKeyPolicy(string projectId, string location,
            string keyRing, string cryptoKey, string role, string member)
        {
            var cloudKms = CreateAuthorizedClient();
            // Generate the full path of the parent to use for updating the crypto key IAM policy.
            var parent = $"projects/{projectId}/locations/{location}/keyRings/{keyRing}/cryptoKeys/{cryptoKey}";
            var result = cloudKms.Projects.Locations.KeyRings.CryptoKeys.GetIamPolicy(parent).Execute();
            if (result.Bindings != null)
            {
                result.Bindings.ToList().ForEach(response =>
                {
                    if (response.Role == role)
                    {
                        // Remove the role/member combo from the crypto key IAM policy.
                        response.Members = response.Members.Where(m => m != member).ToList();
                    }
                });
                // Set the modified crypto key IAM policy to be the cryto key's current IAM policy.
                SetIamPolicyRequest setIamPolicyRequest = new SetIamPolicyRequest();
                setIamPolicyRequest.Policy = result;
                var request = new ProjectsResource.LocationsResource.KeyRingsResource.CryptoKeysResource
                    .SetIamPolicyRequest(cloudKms, setIamPolicyRequest, parent);
                var setIamPolicyResult = request.Execute();
                // Get and display the modified crypto key IAM policy.
                var resultAfterUpdate = cloudKms.Projects.Locations.KeyRings.CryptoKeys.GetIamPolicy(parent).Execute();
                if (resultAfterUpdate.Bindings != null)
                {
                    Console.WriteLine($"Policy Bindings: {resultAfterUpdate.Bindings}");
                    resultAfterUpdate.Bindings.ToList().ForEach(response =>
                    {
                        Console.WriteLine($"Role: {response.Role}");
                        response.Members.ToList().ForEach(memberAfterUpdate =>
                        {
                            Console.WriteLine($"  Member: {memberAfterUpdate}");
                        });
                    });
                }
                else
                {
                    Console.WriteLine($"Empty IAM policy found for CryptoKey: {parent}");
                }
            }
            else
            {
                Console.WriteLine($"Empty IAM policy found for CryptoKey: {parent}");
            }
            return 0;
        }
        // [END kms_remove_member_from_cryptokey_policy]

        // [START kms_get_keyring_policy]
        public static object GetKeyRingIamPolicy(string projectId, string location, string keyRing)
        {
            var cloudKms = CreateAuthorizedClient();
            // Generate the full path of the parent to use for getting the key ring IAM policy.
            var parent = $"projects/{projectId}/locations/{location}/keyRings/{keyRing}";
            var result = cloudKms.Projects.Locations.KeyRings.GetIamPolicy(parent).Execute();
            if (result.Bindings != null)
            {
                Console.WriteLine($"Policy Bindings: {result.Bindings}");
                result.Bindings.ToList().ForEach(response =>
                {
                    Console.WriteLine($"Role: {response.Role}");

                    response.Members.ToList().ForEach(member =>
                    {
                        Console.WriteLine($"  Member: {member}");
                    });
                });
            }
            else
            {
                Console.WriteLine($"Empty IAM policy found for KeyRing: {parent}");
            }
            return 0;
        }
        // [END kms_get_keyring_policy]

        // [START kms_add_member_to_keyring_policy]
        public static object AddMemberToKeyRingPolicy(string projectId, string location,
            string keyRing, string role, string member)
        {
            var cloudKms = CreateAuthorizedClient();
            // Generate the full path of the parent to use for updating the key ring IAM policy.
            var parent = $"projects/{projectId}/locations/{location}/keyRings/{keyRing}";
            SetIamPolicyRequest setIamPolicyRequest = new SetIamPolicyRequest();
            var result = cloudKms.Projects.Locations.KeyRings.GetIamPolicy(parent).Execute();
            if (result.Bindings != null)
            {
                // Policy already exists, so add a new Binding to it.
                Binding bindingToAdd = new Binding();
                bindingToAdd.Role = role;
                string[] testMembers = { member };
                bindingToAdd.Members = testMembers;
                result.Bindings.Add(bindingToAdd);
                setIamPolicyRequest.Policy = result;
            }
            else
            {
                // Policy does not yet exist, so create a new one
                Policy newPolicy = new Policy();
                newPolicy.Bindings = new List<Binding>();
                Binding bindingToAdd = new Binding();
                bindingToAdd.Role = role;
                string[] testMembers = { member };
                bindingToAdd.Members = testMembers;
                newPolicy.Bindings.Add(bindingToAdd);
                setIamPolicyRequest.Policy = newPolicy;
            }
            var request = new ProjectsResource.LocationsResource.KeyRingsResource
                .SetIamPolicyRequest(cloudKms, setIamPolicyRequest, parent);
            var setIamPolicyResult = request.Execute();
            var updateResult = cloudKms.Projects.Locations.KeyRings.GetIamPolicy(parent).Execute();
            updateResult.Bindings.ToList().ForEach(response =>
            {
                Console.WriteLine($"Role: {response.Role}");
                response.Members.ToList().ForEach(memberFound =>
                {
                    Console.WriteLine($"  Member: {memberFound}");
                });
            });
            return 0;
        }
        // [END kms_add_member_to_keyring_policy]

        // [START kms_encrypt]
        public static object Encrypt(string projectId, string location, string keyRing, string cryptoKeyName,
string fileToEncrypt, string fileToOutput)
        {
            var cloudKms = CreateAuthorizedClient();
            // Generate the full path of the crypto key to use for encryption.
            var cryptoKey = $"projects/{projectId}/locations/{location}/keyRings/{keyRing}/cryptoKeys/{cryptoKeyName}";
            EncryptRequest dataToEncrypt = new EncryptRequest();
            byte[] bytes = File.ReadAllBytes(fileToEncrypt);
            string contents = Convert.ToBase64String(bytes);
            dataToEncrypt.Plaintext = contents;
            Console.WriteLine($"dataToEncrypt.Plaintext: {dataToEncrypt.Plaintext}");
            var result = cloudKms.Projects.Locations.KeyRings.CryptoKeys.Encrypt(body: dataToEncrypt, name: cryptoKey).Execute();
            // Output encypted data to a file.
            File.WriteAllBytes(fileToOutput, Convert.FromBase64String(result.Ciphertext));
            Console.Write($"Encypted file created: {fileToOutput}");
            return 0;
        }
        // [END kms_encrypt]

        // [START kms_decrypt]
        public static object Decrypt(string projectId, string location, string keyRing, string cryptoKeyName,
    string fileToDecrypt, string fileToOutput)
        {
            var cloudKms = CreateAuthorizedClient();
            // Generate the full path of the crypto key to use for encryption.
            var cryptoKey = $"projects/{projectId}/locations/{location}/keyRings/{keyRing}/cryptoKeys/{cryptoKeyName}";
            DecryptRequest dataToDecrypt = new DecryptRequest();
            byte[] bytes = File.ReadAllBytes(fileToDecrypt);
            string contents = Convert.ToBase64String(bytes);
            dataToDecrypt.Ciphertext = contents;
            Console.WriteLine($"dataToDecrypt.Ciphertext: {dataToDecrypt.Ciphertext}");
            var result = cloudKms.Projects.Locations.KeyRings.CryptoKeys.Decrypt(dataToDecrypt, cryptoKey).Execute();
            // Output decypted data to a file
            File.WriteAllBytes(fileToOutput, Convert.FromBase64String(result.Plaintext));
            Console.Write($"Encypted file created: {fileToOutput}");
            return 0;
        }
        // [END kms_decrypt]

        public static void Main(string[] args)
        {
            Parser.Default.ParseArguments<
                CreateKeyRingOptions, GetKeyRingOptions, CreateCryptoKeyOptions, GetCryptoKeyOptions,
                ListKeyRingsOptions, ListCryptoKeysOptions, EncryptOptions, DecryptOptions,
                DisableCryptoKeyVersionOptions, EnableCryptoKeyVersionOptions, DestroyCryptoKeyVersionOptions,
                RestoreCryptoKeyVersionOptions, GetCryptoKeyIamPolicyOptions,
                 AddMemberToCryptoKeyPolicyOptions, AddMemberToKeyRingPolicyOptions, RemoveMemberFromCryptoKeyPolicyOptions
                >(args)
              .MapResult(
                (CreateKeyRingOptions opts) => CreateKeyRing(opts.projectId, opts.location, opts.keyRing),
                (GetKeyRingOptions opts) => GetKeyRing(opts.projectId, opts.location, opts.keyRing),
                (CreateCryptoKeyOptions opts) => CreateCryptoKey(opts.projectId, opts.location, opts.keyRing, opts.cryptoKey),
                (GetCryptoKeyOptions opts) => GetCryptoKey(opts.projectId, opts.location, opts.keyRing, opts.cryptoKey),
                (ListKeyRingsOptions opts) => ListKeyRings(opts.projectId, opts.location),
                (ListCryptoKeysOptions opts) => ListCryptoKeys(opts.projectId, opts.location, opts.keyRing),
                (EncryptOptions opts) => Encrypt(opts.projectId, opts.location,
                opts.keyRing, opts.keyName, opts.inFile, opts.outFile),
                (DecryptOptions opts) => Decrypt(opts.projectId, opts.location,
                opts.keyRing, opts.keyName, opts.inFile, opts.outFile),
                (DisableCryptoKeyVersionOptions opts) => DisableCryptoKeyVersion(opts.projectId, opts.location, opts.keyRing, opts.cryptoKey, opts.version),
                (EnableCryptoKeyVersionOptions opts) => EnableCryptoKeyVersion(opts.projectId, opts.location, opts.keyRing, opts.cryptoKey, opts.version),
                (DestroyCryptoKeyVersionOptions opts) => DestroyCryptoKeyVersion(opts.projectId, opts.location, opts.keyRing, opts.cryptoKey, opts.version),
                (RestoreCryptoKeyVersionOptions opts) => RestoreCryptoKeyVersion(opts.projectId, opts.location, opts.keyRing, opts.cryptoKey, opts.version),
                (AddMemberToCryptoKeyPolicyOptions opts) => AddMemberToCryptoKeyPolicy(opts.projectId, opts.location, opts.keyRing, opts.cryptoKey, opts.role, opts.member),
                (GetCryptoKeyIamPolicyOptions opts) => GetCryptoKeyIamPolicy(opts.projectId, opts.location, opts.keyRing, opts.cryptoKey),
                (AddMemberToKeyRingPolicyOptions opts) => AddMemberToKeyRingPolicy(opts.projectId, opts.location, opts.keyRing, opts.role, opts.member),
                (RemoveMemberFromCryptoKeyPolicyOptions opts) => RemoveMemberFromCryptoKeyPolicy(opts.projectId, opts.location, opts.keyRing, opts.cryptoKey, opts.role, opts.member),
                errs => 1);
        }
    }
}