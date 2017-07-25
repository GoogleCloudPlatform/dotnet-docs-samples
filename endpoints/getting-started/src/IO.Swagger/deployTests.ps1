# Copyright(c) 2017 Google Inc.
#
# Licensed under the Apache License, Version 2.0 (the "License"); you may not
# use this file except in compliance with the License. You may obtain a copy of
# the License at
#
# http://www.apache.org/licenses/LICENSE-2.0
#
# Unless required by applicable law or agreed to in writing, software
# distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
# WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
# License for the specific language governing permissions and limitations under
# the License.

Import-Module -DisableNameChecking ..\..\..\..\BuildTools.psm1

dotnet restore
$projectId = gcloud config get-value project
$openapiYamlPath = "..\..\openapi-appengine.yaml"
BackupAndEdit-TextFile $openapiYamlPath `
    @{"YOUR-PROJECT-ID" = $projectId} `
{
	# Now deploy it.
	gcloud service-management deploy ..\..\openapi-appengine.yaml
	$configs = gcloud service-management configs list --service=$projectId.appspot.com --sort-by=~CONFIG_ID
	$configId, $serviceName = $configs[1].split()
	BackupAndEdit-TextFile "app.yaml" @{
		"ENDPOINTS SERVICE NAME" = $serviceName;
		"ENDPOINTS CONFIG ID" = $configId
	} {
		dotnet publish
		gcloud beta app deploy --quiet --no-promote --version=deploytest .\bin\Debug\netcoreapp1.0\publish\app.yaml
		.\Test.ps1 "https://deploytest-dot-$projectId.appspot.com/echo?key=$env:TEST_GOOGLE_ENDPOINTS_APIKEY"
	}
 }
