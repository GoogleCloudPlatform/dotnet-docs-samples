# Copyright (c) 2019 Google LLC.
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

# Common functions called by Set-Up.ps1, runTests.ps1, and Clean-Up.ps1.


# Called by Set-Up.ps1 and runTests.ps1 to modify appsettings.json to
# insert the key name and bucket name.
function Update-Appsettings([Parameter(Mandatory=$true)][string]$keyName, 
    [Parameter(Mandatory=$true)][string]$bucketName)
{
    $appsettings = Get-Content -Raw appsettings.json | ConvertFrom-Json
    $appsettings.DataProtection.KmsKeyName = $keyName
    $appsettings.DataProtection.Bucket = $bucketName
    ConvertTo-Json $appsettings | Out-File -Encoding utf8 -FilePath appsettings.json    
}

# Splits a kms key name, which looks like
# projects/YOUR-PROJECT-ID/locations/global/keyRings/dataprotectionprovider/cryptoKeys/key
# into its constituent chunks.
function Split-KmsKey([Parameter(Mandatory=$true)][string]$keyName) {
    $keyName -match "projects/([^/]+)/locations/([^/]+)/keyRings/([^/]+)/cryptoKeys/([^/]+)"
    return New-Object -TypeName PSObject -Property @{
        'projectId' = $Matches[1];
        'locationId' = $Matches[2];
        'keyRingId' = $Matches[3];
        'keyId' = $matches[4]
    }
}

