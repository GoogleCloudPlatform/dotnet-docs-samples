# Copyright (c) 2018 Google LLC.
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

##############################################################################
#.SYNOPSIS
# Create a new kms encryption key and store it in appsecrets.json.keyname
#
#.PARAMETER keyRingId
# The key ring id to store the key in.
#
#.PARAMETER keyId
# The id for the new key.
#
#.OUTPUTS
# The full name of the new key. 
#
#.EXAMPLE
# .\New-EncryptionKey.ps1
# projects/<your-project-id>/locations/global/keyRings/webapp/cryptoKeys/appsecrets
##############################################################################
Param ([string]$keyRingId = 'webapp', [string]$keyId = 'appsecrets')

# Check to see if the key ring already exists.
$globalKeyRings = (gcloud kms keyrings list --format json --location global | convertfrom-json).name
$matchingKeyRing = foreach ($globalKeyRing in $globalKeyRings) {
    if ($globalKeyRing.EndsWith('/' + $keyRingId)) {
        $globalKeyRing
        break
    }
}
if (-not $matchingKeyRing) {
    # Create the new key ring.
    Write-Information "Creating new key ring $keyRingId..." 
    gcloud kms keyrings create $keyRingId --location global
}

# Create the new key.
Write-Information "Creating new key $keyId..."
gcloud kms keys create $keyId --location global --keyring $keyRingId --purpose=encryption
# Write the new key name to appsecrets.json.keyname.
$keyName = (gcloud kms keys list --location global --keyring $keyRingId --format json | ConvertFrom-Json).name | Where-Object {$_ -like "*/$keyId" }
$keyName | Out-File appsecrets.json.keyname
$keyName
