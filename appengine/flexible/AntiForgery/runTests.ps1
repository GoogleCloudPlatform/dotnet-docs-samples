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

Param ([switch]$SetUp)

Import-Module -DisableNameChecking ..\..\..\BuildTools.psm1
Import-Module .\SetUp.psm1

$creds = Get-Content -Raw $env:GOOGLE_APPLICATION_CREDENTIALS | ConvertFrom-Json
$projectId = $creds.project_id
$email = $creds.client_email
$keyRingId = 'test-dataprotectionprovider'
$keyId = 'test-key'
$keyName = "projects/$projectId/locations/global/keyRings/$keyRingId/cryptoKeys/$keyId"
$bucketName = "$projectId-test-bucket"

Backup-File appsettings.json {
    if ($SetUp) {
        .\Set-Up.ps1 -keyRingId $keyRingId -keyId $keyId -bucketName $bucketName `
            -serviceAccountEmail $email -projectId $projectId
    } else {
        Update-Appsettings $keyName $bucketName
    }
    Run-KestrelTest 5512 -CasperJs11
}