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
# Runs all the gcloud commands necessary to run this sample.  Includes:
# 1. Creating KMS keyring
# 2. Creating KMS key
# 3. Giving permission to the App Engine service account to encrypt and
#    decrypt with the key.
# 4. Creating a Google Cloud Storage bucket.
# 5. Updating appsettings.json with names of key ring, key, and bucket.
#
#.PARAMETER keyRingId
# The key ring id to store the key in.
#
#.PARAMETER keyId
# The id for the new key.
#
#.PARAMETER bucketName
# The name of the bucket to use.
#
#.PARAMETER serviceAccountEmail
# Instead of using the App Engine default service account, use this service
# acccount.
#
#.OUTPUTS
# Log of success and failure. 
#
#.EXAMPLE
# .\SetUp.ps1
##############################################################################
Param ([string]$keyRingId = 'dataprotectionprovider', [string]$keyId = 'key',
    [string]$bucketName, [string]$serviceAccountEmail)

if ($serviceAccountEmail) {
    $email = $serviceAccountEmail
    $email -match '[^@]+@([^\.]+).*' | Out-Null
    $projectId = $matches[1]    
} else {
    # Look up the app engine account email address and project name.
    $accounts = gcloud iam service-accounts list --format=json | ConvertFrom-Json
    $appEngineAccount = $accounts | `
        Where-Object displayName -eq 'App Engine default service account'

    if (-not $appEngineAccount) {
        throw "Could not find the App Engine Default service account in $accounts"
    }
    $email = $appEngineAccount.email
    $projectId = $appEngineAccount.projectId
}

# Check to see if the key ring already exists.
$matchingKeyRing = (gcloud kms keyrings list --format json --location global `
    --filter="name=projects/$projectId/locations/global/keyRings/$keyRingId" | convertfrom-json).name
if ($matchingKeyRing) {
    Write-Host "The key ring $matchingKeyRing already exists."
} else { 
    # Create the new key ring.
    Write-Host "Creating new key ring $keyRingId..." 
    gcloud kms keyrings create $keyRingId --location global
}

# Check to see if the key already exists
$matchingKey = (gcloud kms keys list --format json --location global `
    --keyring $keyRingId --filter="name=projects/$projectId/locations/global/keyRings/$keyRingId/cryptoKeys/$keyId" | convertfrom-json).name
if ($matchingKey) {
    Write-Host "The key $matchingKey already exists."
} else { 
    # Create the new key.
    Write-Host "Creating new key $keyId..."
    gcloud kms keys create $keyId --location global --keyring $keyRingId --purpose=encryption
}

# Write the new key name to appsettings.json
$keyName = (gcloud kms keys list --location global --keyring $keyRingId --format json | ConvertFrom-Json).name | Where-Object {$_ -like "*/$keyId" }
$appsettings = Get-Content appsettings.json | ConvertFrom-Json
$appsettings.DataProtection.KmsKeyName = $keyName
$keyName


# Add Permissions for App Engine to encrypt and decrypt secrets for
# Kms DataProtectionProvider.
$roles = @('roles/cloudkms.admin', 'roles/cloudkms.cryptoKeyEncrypterDecrypter')
foreach ($role in $roles) {
    Write-Host "Adding role $role to $email for $keyRingId."
    gcloud kms keyrings add-iam-policy-binding $keyRingId `
        --project $projectId --location 'global' `
        --member serviceAccount:$email --role $role
}

if (-not $bucketName) {
    $bucketName = "$projectId-bucket"
}

# Check to see if the bucket already exists.
$matchingBucket = (gsutil ls -b gs://$bucketName) 2> $null
if ($matchingBucket) {
    Write-Host "The bucket $bucketName already exists."
} else {
    # Create the bucket.
    gsutil mb -p $projectId gs://$bucketName
}

$appsettings.DataProtection.Bucket = $bucketName
ConvertTo-Json $appsettings | Out-File -Encoding utf8 -FilePath appsettings.json

