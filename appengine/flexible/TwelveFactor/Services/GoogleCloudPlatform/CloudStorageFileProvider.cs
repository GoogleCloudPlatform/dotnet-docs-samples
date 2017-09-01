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
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Google.Apis.Auth.OAuth2;
using Google.Api.Gax;
using Google.Apis.CloudRuntimeConfig.v1beta1;
using Google.Apis.Services;
using Google.Cloud.Storage.V1;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;
using System.Collections;
using System.IO;
using System.Linq;
using System.Net;

namespace TwelveFactor.Services.GoogleCloudPlatform {

    class CloudStorageFileProvider : IFileProvider
    {
        // Configuration Sources are loaded before logging is configured,
        // because logging is controlled by configuration.  Therefore,
        // we have to queue all our log messages and then log them later.
        readonly DelayedLogger _logger = new DelayedLogger();
        public ILogger Logger
        {
            get { return _logger; }
            set { _logger.InnerLogger = value; }
        }        

        readonly StorageClient _storage = StorageClient.Create();
        public IDirectoryContents GetDirectoryContents(string subpath)
        {
            throw new NotImplementedException();            
        }

        public IFileInfo GetFileInfo(string subpath)
        {
            // Accept paths in a variety of forms:
            //   bucket/a/b/c
            //   /bucket/a/b/c
            //   gs://bucket/a/b/c
            string gsPrefix = "gs:/";
            if (subpath.ToLower().Substring(0, gsPrefix.Length) == gsPrefix) {
                subpath = subpath.Substring(gsPrefix.Length);
            }
            if ('/' == subpath.FirstOrDefault()) {
                subpath = subpath.Substring(1);
            }
            string[] fragments = subpath.Split(new [] {'/'}, 2);
            string bucketName = fragments.FirstOrDefault();
            string objectName = fragments.LastOrDefault();
            try {
                var obj = _storage.GetObject(bucketName, objectName);
                if (obj != null) {
                    return new CloudStorageFileInfo(obj, _storage, _logger);                    
                }
            }
            catch (Google.GoogleApiException e)
            when (e.HttpStatusCode == HttpStatusCode.NotFound) 
            {
                _logger.LogWarning("{0} not found.", subpath);
            }
            catch (Exception e)
            {
                _logger.LogError(0, e, "GetFileInfo({0}) failed.", subpath);
            }
            return new NotFoundFileInfo(subpath);
        }

        public IChangeToken Watch(string filter)
        {
            return null;
        }
    }

    class CloudStorageFileInfo : IFileInfo
    {
        readonly Google.Apis.Storage.v1.Data.Object _cloudStorageObject;
        readonly StorageClient _storage;
        readonly ILogger _logger;

        public CloudStorageFileInfo(
            Google.Apis.Storage.v1.Data.Object cloudStorageObject,
            StorageClient storageClient, ILogger logger)
        {
            _cloudStorageObject = cloudStorageObject;
            _storage = storageClient;
            _logger = logger;
        }

        public bool Exists => true;

        public long Length => (long) _cloudStorageObject.Size.Value;

        public string PhysicalPath => null;

        public string Name => _cloudStorageObject.Name;

        public DateTimeOffset LastModified => 
            _cloudStorageObject.TimeCreated.Value;

        public bool IsDirectory => false;

        public Stream CreateReadStream()
        {
            // Json config files are small, and will easily fit in memory.
            var stream = new MemoryStream();
            _storage.DownloadObject(_cloudStorageObject.Bucket, 
                _cloudStorageObject.Name, stream);
            _logger.LogInformation("Loaded {0}/{1}", _cloudStorageObject.Bucket,
                _cloudStorageObject.Name);
            return stream;
        }
    }
}
