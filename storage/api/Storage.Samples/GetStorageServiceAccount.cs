// Copyright 2021 Google Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

// [START storage_get_service_account]

using Google.Cloud.Storage.V1;
using System;

public class GetStorageServiceAccountSample
{
	public string GetStorageServiceAccount(string projectId = "your-project-id")
	{
		var storage = StorageClient.Create();

		var serviceAccountEmail = storage.GetStorageServiceAccountEmail(projectId);

		Console.WriteLine($"The GCS service account for project {projectId} is: {serviceAccountEmail}.");
		return serviceAccountEmail;
	}
}
// [END storage_get_service_account]
