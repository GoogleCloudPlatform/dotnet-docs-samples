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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using CommandLine;
using Google.Cloud.Iam.V1;
using Google.Cloud.Kms.V1;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

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
        public string locationId { get; set; }

        [Value(2, HelpText = "The name of the key ring containing the crypto key.", Required = true)]
        public string keyRingId { get; set; }

        [Value(3, HelpText = "The name of the crypto key.", Required = true)]
        public string cryptoKeyId { get; set; }

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
        public string locationId { get; set; }
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
        public string locationId { get; set; }
        [Value(2, HelpText = "The name of the key ring to use when accessing Cloud KMS resources.", Required = true)]
        public string keyRingId { get; set; }
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
        public string locationId { get; set; }
        [Value(2, HelpText = "The name of the key ring holding the crypto key.", Required = true)]
        public string keyRingId { get; set; }
        [Value(3, HelpText = "The name of the crypto key.", Required = true)]
        public string cryptoKeyId { get; set; }
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
        public string locationId { get; set; }
        [Value(2, HelpText = "The name of the key ring that contains the crypto key to update.", Required = true)]
        public string keyRingId { get; set; }
        [Value(3, HelpText = "The name of the crypto key to update.", Required = true)]
        public string cryptoKeyId { get; set; }
        [Value(4, HelpText = "The name of the crypto key version to update.", Required = true)]
        public string versionId { get; set; }
    }

    [Verb("getCryptoKeyIamPolicy", HelpText = "Get a crypto key's IAM policy.")]
    class GetCryptoKeyIamPolicyOptions
    {
        [Value(0, HelpText = "The project ID of the project to use when listing Cloud KMS resources.", Required = true)]
        public string projectId { get; set; }
        [Value(1, HelpText = "The location to use when listing Cloud KMS resources. Ex. 'global'", Required = true)]
        public string locationId { get; set; }
        [Value(2, HelpText = "The name of the keyRing containing the crypto key.", Required = true)]
        public string keyRingId { get; set; }
        [Value(3, HelpText = "The name of the crypto key to get the IAM policy details for.", Required = true)]
        public string cryptoKeyId { get; set; }
    }


    [Verb("addMemberToCryptoKeyPolicy", HelpText = "Add member to crypto key's IAM policy.")]
    class AddMemberToCryptoKeyPolicyOptions
    {
        [Value(0, HelpText = "The project ID of the project to use when updating Cloud KMS resources.", Required = true)]
        public string projectId { get; set; }
        [Value(1, HelpText = "The location to use when updating Cloud KMS resources. Ex. 'global'", Required = true)]
        public string locationId { get; set; }
        [Value(2, HelpText = "The name of the keyRing containing the crypto key.", Required = true)]
        public string keyRingId { get; set; }
        [Value(3, HelpText = "The name of the crypto key to set the IAM policy details for.", Required = true)]
        public string cryptoKeyId { get; set; }
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
        public string locationId { get; set; }
        [Value(2, HelpText = "The name of the keyRing containing the crypto key.", Required = true)]
        public string keyRingId { get; set; }
        [Value(3, HelpText = "The name of the crypto key to set the IAM policy details for.", Required = true)]
        public string cryptoKeyId { get; set; }
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
        public string locationId { get; set; }
        [Value(2, HelpText = "The name of the keyRing to get the IAM policy details for.", Required = true)]
        public string keyRingId { get; set; }
    }

    [Verb("addMemberToKeyRingPolicy", HelpText = "Add member to a keyRing's IAM policy.")]
    class AddMemberToKeyRingPolicyOptions
    {
        [Value(0, HelpText = "The project ID of the project to use when updating Cloud KMS resources.", Required = true)]
        public string projectId { get; set; }
        [Value(1, HelpText = "The location to use when updating Cloud KMS resources. Ex. 'global'", Required = true)]
        public string locationId { get; set; }
        [Value(2, HelpText = "The name of the keyRing to set the IAM policy details for.", Required = true)]
        public string keyRingId { get; set; }
        [Value(3, HelpText = "The role to add to the keyRing's IAM policy.", Required = true)]
        public string role { get; set; }
        [Value(4, HelpText = "The member to add to the keyRing's IAM policy.", Required = true)]
        public string member { get; set; }
    }

    public class CloudKmsSample
    {
        // [START kms_list_keyrings]
        public static void ListKeyRings(string projectId, string locationId)
        {
            KeyManagementServiceClient client = KeyManagementServiceClient.Create();

            // The resource name of the location associated with the key rings.
            LocationName locationName = new LocationName(projectId, locationId);

            // Print all key rings to the console.
            foreach (KeyRing keyRing in client.ListKeyRings(locationName))
            {
                Console.WriteLine(keyRing.Name);
            }
        }
        // [END kms_list_keyrings]

        // [START kms_list_cryptokeys]
        public static void ListCryptoKeys(string projectId, string locationId, string keyRingId)
        {
            KeyManagementServiceClient client = KeyManagementServiceClient.Create();

            // Generate the full path of the parent to use for listing crypto keys.
            KeyRingName keyRingName = new KeyRingName(projectId, locationId, keyRingId);

            foreach (CryptoKey result in client.ListCryptoKeys(keyRingName))
            {
                Console.WriteLine(result.Name);
                Console.WriteLine($"  Created: {result.CreateTime}");
                Console.WriteLine($"  Purpose: {result.Purpose}");
                Console.WriteLine($"  Primary: {result.Primary}");
                Console.WriteLine($"    State: {result.Primary.State}");
                Console.WriteLine($"    Created: {result.Primary.CreateTime}");
            }
        }
        // [END kms_list_cryptokeys]

        // [START kms_create_keyring]
        public static void CreateKeyRing(string projectId, string locationId, string keyRingId)
        {
            KeyManagementServiceClient client = KeyManagementServiceClient.Create();

            // The location in which to create the key ring.
            LocationName locationName = new LocationName(projectId, locationId);

            // Initial values for the KeyRing (currently unused).
            KeyRing keyRing = new KeyRing();

            KeyRing result = client.CreateKeyRing(locationName, keyRingId, keyRing);
            Console.Write($"Created Key Ring: {result.Name}");
        }
        // [END kms_create_keyring]

        // [START kms_get_keyring]
        public static void GetKeyRing(string projectId, string locationId, string keyRingId)
        {
            KeyManagementServiceClient client = KeyManagementServiceClient.Create();
            KeyRingName keyRingName = new KeyRingName(projectId, locationId, keyRingId);

            KeyRing result = client.GetKeyRing(keyRingName);

            Console.WriteLine($"Found KeyRing: {result.Name}");
            Console.WriteLine($"  Created on:{result.CreateTime}");
        }
        // [END kms_get_keyring]

        // [START kms_get_cryptokey]
        public static void GetCryptoKey(string projectId, string locationId, string keyRingId, string cryptoKeyId)
        {
            KeyManagementServiceClient client = KeyManagementServiceClient.Create();
            CryptoKeyName cryptoKeyName =
                new CryptoKeyName(projectId, locationId, keyRingId, cryptoKeyId);

            CryptoKey result = client.GetCryptoKey(cryptoKeyName);

            Console.WriteLine($"Name: {result.Name}");
            Console.WriteLine($"Created: {result.CreateTime}");
            Console.WriteLine($"Purpose: {result.Purpose}");
            Console.WriteLine($"Primary: {result.Primary}");
            Console.WriteLine($"  State: { result.Primary.State}");
            Console.WriteLine($"  Created: { result.Primary.CreateTime}");
        }
        // [END kms_get_cryptokey]

        // [START kms_create_cryptokey]
        public static void CreateCryptoKey(string projectId, string locationId, string keyRingId, string cryptoKeyId)
        {
            KeyManagementServiceClient client = KeyManagementServiceClient.Create();

            // The KeyRing in which to create the CryptoKey.
            KeyRingName keyRingName = new KeyRingName(projectId, locationId, keyRingId);

            CryptoKey cryptoKeyToCreate = new CryptoKey();
            cryptoKeyToCreate.Purpose = CryptoKey.Types.CryptoKeyPurpose.EncryptDecrypt;

            CryptoKey result = client.CreateCryptoKey(keyRingName, cryptoKeyId, cryptoKeyToCreate);
            Console.Write($"Created Crypto Key: {result.Name}");
        }
        // [END kms_create_cryptokey]


        // [START kms_disable_cryptokey_version]
        public static void DisableCryptoKeyVersion(string projectId, string locationId, string keyRingId, string cryptoKeyId, string versionId)
        {
            KeyManagementServiceClient client = KeyManagementServiceClient.Create();

            // The CryptoKeyVersion to disable.
            CryptoKeyVersionName versionName =
                new CryptoKeyVersionName(projectId, locationId, keyRingId, cryptoKeyId, versionId);

            CryptoKeyVersion version = client.GetCryptoKeyVersion(versionName);

            version.State = CryptoKeyVersion.Types.CryptoKeyVersionState.Disabled;
            FieldMask fieldMask = new FieldMask();
            fieldMask.Paths.Add("state");

            CryptoKeyVersion patchResult = client.UpdateCryptoKeyVersion(version, fieldMask);

            Console.Write($"Disabled Crypto Key Version: {patchResult.Name}");
        }
        // [END kms_disable_cryptokey_version]

        // [START kms_enable_cryptokey_version]
        public static void EnableCryptoKeyVersion(string projectId, string locationId, string keyRingId, string cryptoKeyId, string versionId)
        {
            KeyManagementServiceClient client = KeyManagementServiceClient.Create();

            // The CryptoKeyVersion to enable.
            CryptoKeyVersionName versionName =
                new CryptoKeyVersionName(projectId, locationId, keyRingId, cryptoKeyId, versionId);

            CryptoKeyVersion version = client.GetCryptoKeyVersion(versionName);

            version.State = CryptoKeyVersion.Types.CryptoKeyVersionState.Enabled;
            FieldMask fieldMask = new FieldMask();
            fieldMask.Paths.Add("state");

            CryptoKeyVersion patchResult = client.UpdateCryptoKeyVersion(version, fieldMask);
            Console.Write($"Enabled Crypto Key Version: {patchResult.Name}");
        }
        // [END kms_enable_cryptokey_version]

        // [START kms_get_cryptokey_version]
        public static void GetCryptoKeyVersion(string projectId, string locationId, string keyRingId, string cryptoKeyId, string versionId)
        {
            KeyManagementServiceClient client = KeyManagementServiceClient.Create();

            // The CryptoKeyVersion to enable.
            CryptoKeyVersionName versionName =
                new CryptoKeyVersionName(projectId, locationId, keyRingId, cryptoKeyId, versionId);

            CryptoKeyVersion result = client.GetCryptoKeyVersion(versionName);

            Console.WriteLine($"Name: {result.Name}");
            Console.WriteLine($"Created: {result.CreateTime}");
            Console.WriteLine($"State: {result.State}");
        }
        // [END kms_get_cryptokey_version]

        // [START kms_destroy_cryptokey_version]
        public static void DestroyCryptoKeyVersion(string projectId, string locationId, string keyRingId, string cryptoKeyId, string versionId)
        {
            KeyManagementServiceClient client = KeyManagementServiceClient.Create();

            // The CryptoKeyVersion to destroy.
            CryptoKeyVersionName versionName =
                new CryptoKeyVersionName(projectId, locationId, keyRingId, cryptoKeyId, versionId);

            CryptoKeyVersion result = client.DestroyCryptoKeyVersion(versionName);

            Console.Write($"Destroyed Crypto Key Version: {result.Name}");
        }
        // [END kms_destroy_cryptokey_version]

        // [START kms_restore_cryptokey_version]
        public static void RestoreCryptoKeyVersion(string projectId, string locationId, string keyRingId, string cryptoKeyId, string versionId)
        {
            KeyManagementServiceClient client = KeyManagementServiceClient.Create();

            // The CryptoKeyVersion to restore.
            CryptoKeyVersionName versionName =
                new CryptoKeyVersionName(projectId, locationId, keyRingId, cryptoKeyId, versionId);

            CryptoKeyVersion result = client.RestoreCryptoKeyVersion(versionName);

            Console.Write($"Restored Crypto Key Version: {result.Name}");
        }
        // [END kms_restore_cryptokey_version]

        // [START kms_get_cryptokey_policy]
        public static void GetCryptoKeyIamPolicy(string projectId, string locationId,
            string keyRingId, string cryptoKeyId)
        {
            KeyManagementServiceClient client = KeyManagementServiceClient.Create();
            CryptoKeyName cryptoKeyName =
                new CryptoKeyName(projectId, locationId, keyRingId, cryptoKeyId);

            Policy result = client.GetIamPolicy(KeyNameOneof.From(cryptoKeyName));

            foreach (Binding binding in result.Bindings)
            {
                Console.WriteLine($"Role: {binding.Role}");
                foreach (String member in binding.Members)
                {
                    Console.WriteLine($"  Member: {member}");
                }
            }
        }
        // [END kms_get_cryptokey_policy]

        // [START kms_add_member_to_cryptokey_policy]
        public static void AddMemberToCryptoKeyPolicy(string projectId, string locationId,
            string keyRingId, string cryptoKeyId, string role, string member)
        {
            KeyManagementServiceClient client = KeyManagementServiceClient.Create();
            CryptoKeyName cryptoKeyName =
                new CryptoKeyName(projectId, locationId, keyRingId, cryptoKeyId);

            Policy policy = client.GetIamPolicy(KeyNameOneof.From(cryptoKeyName));

            Binding binding = new Binding();
            binding.Role = role;
            binding.Members.Add(member);

            policy.Bindings.Add(binding);

            Policy updateResult = client.SetIamPolicy(KeyNameOneof.From(cryptoKeyName), policy);

            foreach (Binding bindingResult in updateResult.Bindings)
            {
                Console.WriteLine($"Role: {bindingResult.Role}");
                foreach (string memberResult in bindingResult.Members)
                {
                    Console.WriteLine($"  Member: {memberResult}");
                }
            }
        }
        // [END kms_add_member_to_cryptokey_policy]

        // [START kms_remove_member_from_cryptokey_policy]
        public static void RemoveMemberFromCryptoKeyPolicy(string projectId, string locationId,
            string keyRingId, string cryptoKeyId, string role, string member)
        {
            KeyManagementServiceClient client = KeyManagementServiceClient.Create();
            CryptoKeyName cryptoKeyName =
                new CryptoKeyName(projectId, locationId, keyRingId, cryptoKeyId);

            Policy policy = client.GetIamPolicy(KeyNameOneof.From(cryptoKeyName));

            foreach (Binding binding in policy.Bindings.Where(b => b.Role == role))
            {
                binding.Members.Remove(member);
            }

            Policy updateResult = client.SetIamPolicy(KeyNameOneof.From(cryptoKeyName), policy);

            foreach (Binding bindingResult in updateResult.Bindings)
            {
                Console.WriteLine($"Role: {bindingResult.Role}");
                foreach (string memberResult in bindingResult.Members)
                {
                    Console.WriteLine($"  Member: {memberResult}");
                }
            }
        }
        // [END kms_remove_member_from_cryptokey_policy]

        // [START kms_get_keyring_policy]
        public static void GetKeyRingIamPolicy(string projectId, string locationId, string keyRingId)
        {
            KeyManagementServiceClient client = KeyManagementServiceClient.Create();
            KeyRingName keyRingName = new KeyRingName(projectId, locationId, keyRingId);

            Policy result = client.GetIamPolicy(KeyNameOneof.From(keyRingName));

            foreach (Binding binding in result.Bindings)
            {
                Console.WriteLine($"Role: {binding.Role}");
                foreach (String member in binding.Members)
                {
                    Console.WriteLine($"  Member: {member}");
                }
            }
        }
        // [END kms_get_keyring_policy]

        // [START kms_add_member_to_keyring_policy]
        public static void AddMemberToKeyRingPolicy(string projectId, string locationId,
            string keyRingId, string role, string member)
        {
            KeyManagementServiceClient client = KeyManagementServiceClient.Create();
            KeyRingName keyRingName = new KeyRingName(projectId, locationId, keyRingId);

            Policy policy = client.GetIamPolicy(KeyNameOneof.From(keyRingName));

            Binding binding = new Binding();
            binding.Role = role;
            binding.Members.Add(member);

            policy.Bindings.Add(binding);

            Policy updateResult = client.SetIamPolicy(KeyNameOneof.From(keyRingName), policy);

            foreach (Binding bindingResult in updateResult.Bindings)
            {
                Console.WriteLine($"Role: {bindingResult.Role}");
                foreach (string memberResult in bindingResult.Members)
                {
                    Console.WriteLine($"  Member: {memberResult}");
                }
            }
        }
        // [END kms_add_member_to_keyring_policy]

        // [START kms_encrypt]
        public static void Encrypt(string projectId, string locationId, string keyRingId, string cryptoKeyId,
string plaintextFile, string ciphertextFile)
        {
            KeyManagementServiceClient client = KeyManagementServiceClient.Create();
            CryptoKeyName cryptoKeyName =
                new CryptoKeyName(projectId, locationId, keyRingId, cryptoKeyId);

            byte[] plaintext = File.ReadAllBytes(plaintextFile);
            CryptoKeyPathName pathName = CryptoKeyPathName.Parse(cryptoKeyName.ToString());
            EncryptResponse result = client.Encrypt(pathName, ByteString.CopyFrom(plaintext));

            // Output encrypted data to a file.
            File.WriteAllBytes(ciphertextFile, result.Ciphertext.ToByteArray());
            Console.Write($"Encrypted file created: {ciphertextFile}");
        }
        // [END kms_encrypt]

        // [START kms_decrypt]
        public static void Decrypt(string projectId, string locationId, string keyRingId, string cryptoKeyId,
    string ciphertextFile, string plaintextFile)
        {
            KeyManagementServiceClient client = KeyManagementServiceClient.Create();
            CryptoKeyName cryptoKeyName =
                new CryptoKeyName(projectId, locationId, keyRingId, cryptoKeyId);

            byte[] ciphertext = File.ReadAllBytes(ciphertextFile);
            DecryptResponse result = client.Decrypt(cryptoKeyName, ByteString.CopyFrom(ciphertext));

            // Output decrypted data to a file.
            File.WriteAllBytes(plaintextFile, result.Plaintext.ToByteArray());
            Console.Write($"Decrypted file created: {plaintextFile}");
        }
        // [END kms_decrypt]

        public static void Main(string[] args)
        {
            Parser.Default.ParseArguments(args,
                    typeof(CreateKeyRingOptions), typeof(GetKeyRingOptions), typeof(CreateCryptoKeyOptions),
                    typeof(GetCryptoKeyOptions), typeof(ListKeyRingsOptions), typeof(ListCryptoKeysOptions),
                    typeof(EncryptOptions), typeof(DecryptOptions), typeof(GetCryptoKeyVersionOptions),
                    typeof(DisableCryptoKeyVersionOptions), typeof(EnableCryptoKeyVersionOptions),
                    typeof(DestroyCryptoKeyVersionOptions), typeof(RestoreCryptoKeyVersionOptions),
                    typeof(GetCryptoKeyIamPolicyOptions), typeof(AddMemberToCryptoKeyPolicyOptions),
                    typeof(GetKeyRingIamPolicyOptions), typeof(AddMemberToKeyRingPolicyOptions),
                    typeof(RemoveMemberFromCryptoKeyPolicyOptions))
                .WithParsed<CreateKeyRingOptions>(opts => CreateKeyRing(opts.projectId, opts.locationId, opts.keyRingId))
                .WithParsed<GetKeyRingOptions>(opts => GetKeyRing(opts.projectId, opts.locationId, opts.keyRingId))
                .WithParsed<CreateCryptoKeyOptions>(opts => CreateCryptoKey(opts.projectId, opts.locationId, opts.keyRingId, opts.cryptoKeyId))
                .WithParsed<GetCryptoKeyOptions>(opts => GetCryptoKey(opts.projectId, opts.locationId, opts.keyRingId, opts.cryptoKeyId))
                .WithParsed<ListKeyRingsOptions>(opts => ListKeyRings(opts.projectId, opts.locationId))
                .WithParsed<ListCryptoKeysOptions>(opts => ListCryptoKeys(opts.projectId, opts.locationId, opts.keyRingId))
                .WithParsed<EncryptOptions>(opts => Encrypt(opts.projectId, opts.locationId,
                        opts.keyRingId, opts.cryptoKeyId, opts.inFile, opts.outFile))
                .WithParsed<DecryptOptions>(opts => Decrypt(opts.projectId, opts.locationId,
                        opts.keyRingId, opts.cryptoKeyId, opts.inFile, opts.outFile))
                .WithParsed<GetCryptoKeyVersionOptions>(opts => GetCryptoKeyVersion(opts.projectId, opts.locationId, opts.keyRingId, opts.cryptoKeyId, opts.versionId))
                .WithParsed<DisableCryptoKeyVersionOptions>(opts => DisableCryptoKeyVersion(opts.projectId, opts.locationId, opts.keyRingId, opts.cryptoKeyId, opts.versionId))
                .WithParsed<EnableCryptoKeyVersionOptions>(opts => EnableCryptoKeyVersion(opts.projectId, opts.locationId, opts.keyRingId, opts.cryptoKeyId, opts.versionId))
                .WithParsed<DestroyCryptoKeyVersionOptions>(opts => DestroyCryptoKeyVersion(opts.projectId, opts.locationId, opts.keyRingId, opts.cryptoKeyId, opts.versionId))
                .WithParsed<RestoreCryptoKeyVersionOptions>(opts => RestoreCryptoKeyVersion(opts.projectId, opts.locationId, opts.keyRingId, opts.cryptoKeyId, opts.versionId))
                .WithParsed<AddMemberToCryptoKeyPolicyOptions>(opts => AddMemberToCryptoKeyPolicy(opts.projectId, opts.locationId, opts.keyRingId, opts.cryptoKeyId, opts.role, opts.member))
                .WithParsed<GetCryptoKeyIamPolicyOptions>(opts => GetCryptoKeyIamPolicy(opts.projectId, opts.locationId, opts.keyRingId, opts.cryptoKeyId))
                .WithParsed<AddMemberToKeyRingPolicyOptions>(opts => AddMemberToKeyRingPolicy(opts.projectId, opts.locationId, opts.keyRingId, opts.role, opts.member))
                .WithParsed<GetKeyRingIamPolicyOptions>(opts => GetKeyRingIamPolicy(opts.projectId, opts.locationId, opts.keyRingId))
                .WithParsed<RemoveMemberFromCryptoKeyPolicyOptions>(opts => RemoveMemberFromCryptoKeyPolicy(opts.projectId, opts.locationId, opts.keyRingId, opts.cryptoKeyId, opts.role, opts.member));
        }
    }
}