/*
 * Copyright (c) 2018 Google LLC.
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

using Google.Apis.Storage.v1.Data;
using Google.Cloud.Storage.V1;
using System;
using Xunit;

namespace ExportAssetsSample
{
    public class ExportAssetsTest
    {
        static readonly CommandLineRunner s_runner = new CommandLineRunner
        {
            Command = "ExportAssets.exe",
            VoidMain = Program.Main,
        };

        private StorageClient storageClient = StorageClient.Create();
	public readonly ITestOutputHelper _testOutput;

	public ExportAssetsTest(ITestOutputHelper output)
	{
	    _testOutput = output;
	}

        [Fact]
        public void TestExportAsests()
        {
            string projectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
            string bucketName = String.Format("{0}-for-assets", projectId);
            if (CheckBucketExists(bucketName))
            {
                EmptyBucket(bucketName);
            } else
            {
                CreateBucket(projectId, bucketName);
            }
	    var output = s_runner.Run();
	    _testOutput.WriteLine(output.Stdout);
        }

        bool CheckBucketExists(string bucketName)
        {
            Bucket bucket = storageClient.GetBucket(bucketName, new GetBucketOptions());
            return bucket != null;
        }

        void EmptyBucket(string bucketName)
        {
            foreach(var storageObject in storageClient.ListObjects(bucketName)) {
                storageClient.DeleteObject(storageObject);
            }
        }

        void CreateBucket(string projectId, string bucketName)
        {
            storageClient.CreateBucket(projectId, bucketName);
        }
    }
}

