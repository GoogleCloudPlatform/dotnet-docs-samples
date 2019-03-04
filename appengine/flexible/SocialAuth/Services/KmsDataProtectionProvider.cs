/*
 * Copyright (c) 2018 Google Inc.
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

using Google.Cloud.Kms.V1;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.DataProtection;
using System;
using System.Collections.Concurrent;
using System.Text;

namespace SocialAuthMVC.Services
{
    /// <summary>
    /// Implements a DataProtectionProvider using Google's Cloud Key
    /// Management Service.  https://cloud.google.com/kms/
    /// </summary>
    public class KmsDataProtectionProvider : IDataProtectionProvider
    {
        // The kms service.
        private readonly KeyManagementServiceClient _kms;

        private readonly KeyRingName _keyRingName;

        private readonly string _googleProjectId;
        private readonly string _keyRingLocation;
        private readonly string _keyRingId;

        // Keep a cache of DataProtectors we create to reduce calls to the
        // _kms service.
        private readonly ConcurrentDictionary<string, IDataProtector>
            _dataProtectorCache =
            new ConcurrentDictionary<string, IDataProtector>();

        public KmsDataProtectionProvider(
            string googleProjectId,
            string keyRingLocation,
            string keyRingId)
        {
            _googleProjectId = googleProjectId ??
                throw new ArgumentNullException(nameof(googleProjectId));
            _keyRingLocation = keyRingLocation ??
                throw new ArgumentNullException(nameof(keyRingLocation));
            _keyRingId = keyRingId ??
                throw new ArgumentNullException(nameof(keyRingId));
            _kms = KeyManagementServiceClient.Create();
            _keyRingName = new KeyRingName(_googleProjectId,
                _keyRingLocation, _keyRingId);
            try
            {
                // Create the key ring.
                _kms.CreateKeyRing(
                    new LocationName(_googleProjectId, _keyRingLocation),
                    _keyRingId, new KeyRing());
            }
            catch (Grpc.Core.RpcException e)
            when (e.StatusCode == StatusCode.AlreadyExists)
            {
                // Already exists.  Ok.
            }
            catch (Grpc.Core.RpcException e)
            when (e.StatusCode == StatusCode.PermissionDenied)
            {
                // We don't need to create it as long as it exists.
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
            CryptoKey cryptoKeyToCreate = new CryptoKey()
            {
                Purpose = CryptoKey.Types.CryptoKeyPurpose.EncryptDecrypt,
                NextRotationTime = Timestamp.FromDateTime(DateTime.UtcNow.AddDays(7)),
                RotationPeriod = Duration.FromTimeSpan(TimeSpan.FromDays(7))
            };
            CryptoKeyName keyName = new CryptoKeyName(_googleProjectId,
                    _keyRingLocation, _keyRingId, EscapeKeyId(purpose));
            try
            {
                _kms.CreateCryptoKey(_keyRingName, keyName.CryptoKeyId,
                    cryptoKeyToCreate);
            }
            catch (Grpc.Core.RpcException e)
            when (e.StatusCode == StatusCode.AlreadyExists)
            {
                // Already exists.  Ok.
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
        private static string EscapeKeyId(string purpose)
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
                // hash code.  Insert it into the middle of the string.  Because
                // the beginning and end of the string are more interesting to
                // humans.
                keyId = string.Format("{0}-{1:x8}-{2}", keyId.Substring(0, 27),
                    QuickHash(keyId), keyId.Substring(keyId.Length - 26, 26));
            }
            return keyId;
        }

        /// <summary>
        /// A simple hash function used to avoid collisions when mapping
        /// purposes to key ids.  Must be stable across platforms.
        /// </summary>
        private static int QuickHash(string s)
        {
            int hash = 17;
            foreach (char c in s)
            {
                hash = hash * 31 + c;
            }
            return hash;
        }
    }

    /// <summary>
    /// Implements IDataProtector with Google Key Management Service.
    /// </summary>
    public class KmsDataProtector : IDataProtector
    {
        private readonly KeyManagementServiceClient _kms;
        private readonly CryptoKeyName _keyName;
        private readonly CryptoKeyPathName _keyPathName;
        private readonly Func<string, IDataProtector> _dataProtectorFactory;

        internal KmsDataProtector(KeyManagementServiceClient kms,
            CryptoKeyName keyName,
            Func<string, IDataProtector> dataProtectorFactory)
        {
            _kms = kms;
            _keyName = keyName;
            _keyPathName = new CryptoKeyPathName(keyName.ProjectId,
                keyName.LocationId, keyName.KeyRingId, keyName.CryptoKeyId);
            _dataProtectorFactory = dataProtectorFactory;
        }

        IDataProtector IDataProtectionProvider.CreateProtector(string purpose)
        {
            return _dataProtectorFactory(purpose);
        }

        byte[] IDataProtector.Protect(byte[] plaintext)
        {
            var response =
                _kms.Encrypt(_keyPathName, ByteString.CopyFrom(plaintext));
            return response.Ciphertext.ToByteArray();
        }

        byte[] IDataProtector.Unprotect(byte[] protectedData)
        {
            var response =
                _kms.Decrypt(_keyName, ByteString.CopyFrom(protectedData));
            return response.Plaintext.ToByteArray();
        }
    }
}