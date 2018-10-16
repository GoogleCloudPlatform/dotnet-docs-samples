// Copyright (c) 2018 Google LLC.
//
// Licensed under the Apache License, Version 2.0 (the "License"); you may not
// use this file except in compliance with the License. You may obtain a copy of
// the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
// License for the specific language governing permissions and limitations under
// the License.

using Google.Cloud.Kms.V1;
using Google.Protobuf;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace SocialAuthMVC.Services.Kms
{
    /// <summary>
    /// Decrypts encrypted files in memory,
    /// using Google Cloud Key Management Service
    /// https://cloud.google.com/kms/
    ///
    /// Encrypted files end with the extension .encrypted and have a
    /// corresponding file ending with the extension .encrypted.
    /// </summary>
    public class EncryptedFileProvider : IFileProvider
    {
        public const string EncryptedFileExtension = ".encrypted";
        public const string EncryptedFileKeyNameExtension = ".keyname";
        private readonly KeyManagementServiceClient kms;
        private readonly IFileProvider innerProvider;

        /// <summary>
        /// Creates an EncryptedFileProvider.
        /// </summary>
        /// <param name="kms">Optional.</param>
        /// <param name="innerProvider">Optional.  An IFileProvider
        /// containing encrypted files.</param>
        public EncryptedFileProvider(
            KeyManagementServiceClient kms = null,
            IFileProvider innerProvider = null)
        {
            this.kms = kms ?? KeyManagementServiceClient.Create();
            if (innerProvider == null)
            {
                string fullPath = System.Reflection.Assembly
                    .GetAssembly(typeof(EncryptedFileProvider)).Location;
                string directory = Path.GetDirectoryName(fullPath);
                this.innerProvider = new PhysicalFileProvider(directory);
            }
            else
            {
                this.innerProvider = innerProvider;
            }
        }

        public IDirectoryContents GetDirectoryContents(string subpath)
        {
            var innerContents = innerProvider.GetDirectoryContents(subpath);
            if (innerContents == null)
            {
                return null;
            }
            return new EncryptedDirectoryContents(kms, innerContents);
        }

        public IFileInfo GetFileInfo(string subpath)
        {
            return EncryptedFileInfo.FromFileInfo(kms,
                innerProvider.GetFileInfo(subpath),
                innerProvider.GetFileInfo(
                    Path.ChangeExtension(subpath, EncryptedFileKeyNameExtension)));
        }

        public IChangeToken Watch(string filter)
        {
            return innerProvider.Watch(filter);
        }
    }

    public class EncryptedFileInfo : IFileInfo
    {
        private readonly KeyManagementServiceClient kms;
        private readonly IFileInfo keynameFileInfo;
        private readonly Lazy<CryptoKeyName> cryptoKeyName;
        private readonly IFileInfo innerFileInfo;

        public static IFileInfo FromFileInfo(
            KeyManagementServiceClient kms,
            IFileInfo fileInfo, 
            IFileInfo keynameFileInfo)
        {
            if (fileInfo == null)
            {
                return null;
            }
            if (fileInfo.IsDirectory)
            {
                return fileInfo;
            }
            if (!fileInfo.Name.EndsWith(EncryptedFileProvider.EncryptedFileExtension))
            {
                return null;
            }
            return new EncryptedFileInfo(kms, fileInfo, keynameFileInfo);
        }

        private EncryptedFileInfo(
            KeyManagementServiceClient kms,
            IFileInfo innerFileInfo, 
            IFileInfo keynameFileInfo)
        {
            this.kms = kms;
            this.keynameFileInfo = keynameFileInfo;
            this.innerFileInfo = innerFileInfo;
            this.cryptoKeyName = new Lazy<CryptoKeyName>(() => UnpackKeyName(keynameFileInfo));
        }

        bool IFileInfo.Exists => innerFileInfo.Exists && innerFileInfo.Name.EndsWith(
            EncryptedFileProvider.EncryptedFileExtension);

        long IFileInfo.Length => ((IFileInfo)this).CreateReadStream().Length;

        string IFileInfo.PhysicalPath => null;

        string IFileInfo.Name => innerFileInfo.Name;

        DateTimeOffset IFileInfo.LastModified => innerFileInfo.LastModified;

        bool IFileInfo.IsDirectory => innerFileInfo.IsDirectory;

        /// <summary>
        /// Create a stream that decrypts the encrypted file.
        /// </summary>
        /// <returns>An unencrypted stream.</returns>
        Stream IFileInfo.CreateReadStream()
        {
            if (!((IFileInfo)this).Exists)
            {
                throw new FileNotFoundException(innerFileInfo.Name);
            }
            DecryptResponse response;
            // Read the encrypted bytes from the file.
            using (var stream = innerFileInfo.CreateReadStream())
            {
                // Call kms to Decrypt them.
                response = kms.Decrypt(cryptoKeyName.Value,
                    ByteString.FromStream(stream));
            }
            // Dump the unencrypted bytes to a memory stream.
            MemoryStream memStream = new MemoryStream();
            response.Plaintext.WriteTo(memStream);
            memStream.Seek(0, SeekOrigin.Begin);
            return memStream;
        }

        private static CryptoKeyName UnpackKeyName(IFileInfo keynameFileInfo)
        {
            if (keynameFileInfo == null || !keynameFileInfo.Exists || keynameFileInfo.IsDirectory)
            {
                throw new FileNotFoundException("Encrypted file found, but "
                    + "failed to find corresponding keyname file.",
                    keynameFileInfo.Name);
            }

            using (var reader = new StreamReader(keynameFileInfo.CreateReadStream()))
            {
                string line = "";
                while (!reader.EndOfStream)
                {
                    line = reader.ReadLine().Trim();
                    if (string.IsNullOrWhiteSpace(line) || line.StartsWith('#'))
                    {
                        continue; // blank or comment;
                    }
                    var keyName = CryptoKeyName.Parse(line);
                    if (keyName != null)
                    {
                        return keyName;
                    }
                    break;
                }
                throw new Exception(
                    $"Incorrectly formatted keyname file {keynameFileInfo.Name}.\n" +
                    "Expected projects/<projectId>/locations/<locationId>/keyRings/<keyringId>/cryptoKeys/<keyId>\n" +
                    $"Instead, found {line}.");
            }
        }
    }

    public class EncryptedDirectoryContents : IDirectoryContents
    {
        private readonly KeyManagementServiceClient kms;
        private readonly IDirectoryContents innerDirectoryContents;

        public EncryptedDirectoryContents(KeyManagementServiceClient kms,
            IDirectoryContents innerDirectoryContents)
        {
            this.kms = kms;
            this.innerDirectoryContents = innerDirectoryContents;
        }

        public bool Exists => innerDirectoryContents.Exists;

        public IEnumerator<IFileInfo> GetEnumerator()
        {
            // Only enumerate directories, and files with matching
            // .encrypted and .keyname extensions.
            var keynameFiles = new Dictionary<String, IFileInfo>();
            var encryptedFiles = new List<IFileInfo>();
            foreach (var fileInfo in innerDirectoryContents)
            {
                if (fileInfo.IsDirectory)
                {
                    yield return fileInfo;
                }
                else
                {
                    string extension = Path.GetExtension(fileInfo.Name);
                    if (extension == EncryptedFileProvider.EncryptedFileExtension)
                    {
                        encryptedFiles.Add(fileInfo);
                    }
                    else if (extension == EncryptedFileProvider.EncryptedFileKeyNameExtension)
                    {
                        keynameFiles[fileInfo.Name] = fileInfo;
                    }
                }
            }
            foreach (IFileInfo fileInfo in encryptedFiles)
            {
                IFileInfo keynameFile = keynameFiles.GetValueOrDefault(
                    Path.ChangeExtension(fileInfo.Name, EncryptedFileProvider.EncryptedFileKeyNameExtension));
                if (keynameFile != null)
                {
                    yield return EncryptedFileInfo.FromFileInfo(kms, fileInfo,
                        keynameFile);
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            IEnumerator<IFileInfo> x = this.GetEnumerator();
            return x;
        }
    }
}