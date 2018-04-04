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

Import-Module -DisableNameChecking ..\..\..\BuildTools.psm1

dotnet restore
dotnet build
# Detect if I'm running in the Google Cloud.
$runningWithGoogleCloudIpAddress = $false
try {
	$metadataResponse = Invoke-WebRequest `
		"http://metadata.google.internal/computeMetadata/v1/instance/network-interfaces/0/access-configs/0/external-ip" `
		-Headers @{"Metadata-Flavor"="Google"}
	
	if ($metadataResponse.StatusCode -eq 200 -and $metadataResponse.RawContentLength -gt 8) {
		$runningWithGoogleCloudIpAddress = $true
	}
} catch {
}
$jsTest = if ($runningWithGoogleCloudIpAddress) {"cloudTest.js"} else {"localTest.js"}
Run-KestrelTest 5571 $jsTest -CasperJs11
