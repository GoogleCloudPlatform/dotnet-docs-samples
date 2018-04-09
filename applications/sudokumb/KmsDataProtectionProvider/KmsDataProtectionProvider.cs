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
using Google.Apis.CloudKMS.v1;
using Google.Apis.CloudKMS.v1.Data;
using Google.Apis.Services;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;

namespace SocialAuth.Services
{
    public class KmsDataProtectionProviderOptions
    {
        /// <summary>
        /// Your Google project id.
        /// </summary>
        public string ProjectId { get; set; }
        /// <summary>
        /// global, us-east1, etc.
        /// </summary>
        public string Location { get; set; } = "global";
        /// <summary>
        /// Name of the key ring to store the keys in.
        /// </summary>
        public string KeyRing { get; set; }
    }

    /// <summary>
    /// Implements a DataProtectionProvider using Google's Cloud Key
    /// Management Service.  https://cloud.google.com/kms/
    /// </summary>
    public class KmsDataProtectionProvider : IDataProtectionProvider
    {
        // The kms service.
        readonly CloudKMSService _kms;
        readonly IOptions<KmsDataProtectionProviderOptions> _options;
        // Keep a cache of DataProtectors we create to reduce calls to the
        // _kms service.
        readonly ConcurrentDictionary<string, IDataProtector>
            _dataProtectorCache =
            new ConcurrentDictionary<string, IDataProtector>();

        public KmsDataProtectionProvider(
            IOptions<KmsDataProtectionProviderOptions> options)
        {
            _options = options;
            // Create a KMS service client with credentials.
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
            _kms = new CloudKMSService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                GZipEnabled = false
            });
            // Create the key ring.
            var parent = string.Format("projects/{0}/locations/{1}",
                options.Value.ProjectId, options.Value.Location);
            KeyRing keyRingToCreate = new KeyRing();
            var request = new ProjectsResource.LocationsResource
                .KeyRingsResource.CreateRequest(_kms, keyRingToCreate, parent);
            request.KeyRingId = options.Value.KeyRing;
            try
            {
                request.Execute();
            }
            catch (Google.GoogleApiException e)
            when (e.HttpStatusCode == System.Net.HttpStatusCode.Conflict)
            {
                // Already exists.  Ok.
            }
        }

        IDataProtector IDataProtectionProvider.CreateProtector(string purpose)
        {
            IDataProtector cached;
            if (_dataProtectorCache.TryGetValue(purpose, out cached))
            {
                return cached;
            }
            // Create the crypto key:
            var keyRingName = string.Format(
                "projects/{0}/locations/{1}/keyRings/{2}",
                _options.Value.ProjectId, _options.Value.Location,
                _options.Value.KeyRing);
            string rotationPeriod = string.Format("{0}s",
                    TimeSpan.FromDays(7).TotalSeconds);
            CryptoKey cryptoKeyToCreate = new CryptoKey()
            {
                Purpose = "ENCRYPT_DECRYPT",
                NextRotationTime = DateTime.UtcNow.AddDays(7),
                RotationPeriod = rotationPeriod
            };
            var request = new ProjectsResource.LocationsResource
                .KeyRingsResource.CryptoKeysResource.CreateRequest(
                _kms, cryptoKeyToCreate, keyRingName);
            string keyId = EscapeKeyId(purpose);
            request.CryptoKeyId = keyId;
            string keyName;
            try
            {
                keyName = request.Execute().Name;
            }
            catch (Google.GoogleApiException e)
                when (e.HttpStatusCode == System.Net.HttpStatusCode.Conflict)
            {
                // Already exists.  Ok.
                keyName = string.Format("{0}/cryptoKeys/{1}",
                    keyRingName, keyId);
            }
            var newProtector = new KmsDataProtector(_kms, keyName,
                (string innerPurpose) =>
                this.CreateProtector($"{purpose}.{innerPurpose}"));
            _dataProtectorCache.TryAdd(purpose, newProtector);
            return newProtector;
        }

        /// <summary>
        /// Creates a key id given a string purpose.
        /// Key ids must match the regex [a-zA-Z0-9_-]{1,63}.
        /// </summary>
        /// <param name="purpose">The purpose of the key.</param>
        /// <returns>A key id that's safe to pass to Create().</returns>
        static string EscapeKeyId(string purpose)
        {
            StringBuilder keyIdBuilder = new StringBuilder();
            char prevC = ' ';
            foreach (char c in purpose)
            {
                if (c == '.')
                {
                    keyIdBuilder.Append('-');
                }
                else if (prevC == '0' && c == 'x' ||
                    !"abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890_"
                    .Contains(c))
                {
                    keyIdBuilder.AppendFormat("0x{0:X4}", (int)c);
                }
                else
                {
                    keyIdBuilder.Append(c);
                }
                prevC = c;
            }
            string keyId = keyIdBuilder.ToString();
            if (keyId.Length > 63)
            {
                // For strings that are too long to be key ids, tag them with a
                // hash code.
                keyId = string.Format("{0}-{1:x8}", keyId.Substring(0, 54),
                    QuickHash(keyId));
            }
            return keyId;
        }

        /// <summary>
        /// A simple hash function used to avoid collisions when mapping 
        /// purposes to key ids.  Must be stable across platforms.
        /// </summary>
        static int QuickHash(string s)
        {
            int hash = 17;
            foreach (char c in s)
            {
                hash = hash * 31 + c;
            }
            return hash;
        }
    }

    public class KmsDataProtector : IDataProtector
    {
        readonly CloudKMSService _kms;
        readonly string _keyName;
        readonly Func<string, IDataProtector> _dataProtectorFactory;

        internal KmsDataProtector(CloudKMSService kms, string keyName,
            Func<string, IDataProtector> dataProtectorFactory)
        {
            _kms = kms;
            _keyName = keyName;
            _dataProtectorFactory = dataProtectorFactory;
        }

        IDataProtector IDataProtectionProvider.CreateProtector(string purpose)
        {
            return _dataProtectorFactory(purpose);
        }

        byte[] IDataProtector.Protect(byte[] plaintext)
        {
            var result = _kms.Projects.Locations.KeyRings.CryptoKeys
                .Encrypt(new EncryptRequest()
                {
                    Plaintext = Convert.ToBase64String(plaintext)
                }, _keyName).Execute();
            return Convert.FromBase64String(result.Ciphertext);
        }

        byte[] IDataProtector.Unprotect(byte[] protectedData)
        {
            var result = _kms.Projects.Locations.KeyRings.CryptoKeys
                .Decrypt(new DecryptRequest()
                {
                    Ciphertext = Convert.ToBase64String(protectedData)
                }, _keyName).Execute();
            return Convert.FromBase64String(result.Plaintext);
        }
    }
}
