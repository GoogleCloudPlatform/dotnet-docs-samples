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


Param ([string]$serviceAccountEmail)

# Get App Engine's default service account.
$filter = if ($serviceAccountEmail) {
    "email=$serviceAccountEmail"
} else {
    "displayName='App Engine default service account'"
}
$account = gcloud iam service-accounts list --format=json "--filter=$filter" `
    | ConvertFrom-Json

if (-not $account) {
    throw @'
        Could not find the App Engine Default service account.
        Visit https://console.cloud.google.com/appengine/start to create an
        App Engine application.
'@
}
$email = $account.email
$projectId = $account.projectId

# Enable KMS
gcloud services enable cloudkms.googleapis.com


<#
.SYNOPSIS
Creates a new KMS key.

.DESCRIPTION
Will also create the parent key ring, if necessary.
If the keyring or key already exist, creation is skipped.

.PARAMETER keyRingId
The key ring id.

.PARAMETER keyId
The key name id.

.EXAMPLE
$appsecretsKey = New-Key appsecrets appkey
#>
function New-Key([string]$keyRingId, [string]$keyId) {
    # Check to see if the key ring already exists.
    gcloud kms keyrings describe --location global $keyRingId | Out-Null
    if ($LASTEXITCODE -eq 0) {
        Write-Host "The key ring $keyRingId already exists."
    } else { 
        # Not found.  Create the new key ring.
        Write-Host "Creating new key ring $keyRingId..." 
        gcloud kms keyrings create $keyRingId --location global | Write-Host
        Try-Again
    }

    # Check to see if the key already exists
    $keyName = "projects/$projectId/locations/global/keyRings/$keyRingId/cryptoKeys/$keyId"
    gcloud kms keys describe --location global `
        --keyring $keyRingId $keyId | Out-Null
    if ($LASTEXITCODE -eq 0) {
        Write-Host "The key $keyId already exists."
    } else { 
        # Create the new key.
        Write-Host "Creating new key $keyId..."
        gcloud kms keys create $keyId --location global --keyring $keyRingId `
            --purpose=encryption | Write-Host
        Try-Again
    }
    return $keyName
}

<#
.SYNOPSIS
Grants a role to the App Engine default service account for a given key ring.

.PARAMETER keyRingId
The key ring that the new role should grant access to.

.PARAMETER role
The role to grant.

.EXAMPLE
Add-Role appsecrets roles/cloudkms.cryptoKeyDecrypter

#>
function Add-Role([string]$keyRingId, [string]$role) {
    $keyRingName = "projects/$projectId/locations/global/keyRings/$keyRingId"
    Write-Host "Adding role $role to $email for $keyRingName."
    gcloud kms keyrings add-iam-policy-binding $keyRingName `
        --member serviceAccount:$email --role $role | Write-Host    
    Try-Again
}

function Try-Again {
    if ($LASTEXITCODE) {
        throw "Try again in a few minutes."
    }
}

###############################################################################
# The appsecrets key encrypts and decrypts appsecrets.json.
$appsecretsKey = New-Key appsecrets appkey
# Give App Engine permission to decrypt using keys in this keyring.
Add-Role appsecrets roles/cloudkms.cryptoKeyDecrypter
# Write the new key name to appsecrets.json.keyname.
$appsecretsKey | Out-File appsecrets.json.keyname

##############################################################################
# The dataprotection masterkey encrypts and decrypts the keys used by
# ASP.NET core's data protection stack.
$dataprotectionKey = New-Key dataprotection masterkey
# Give App Engine permission to encrypt and decrypt using keys in this keyring.
Add-Role dataprotection roles/cloudkms.cryptoKeyDecrypter
Add-Role dataprotection roles/cloudkms.cryptoKeyEncrypter

# Create the bucket where ASP.NET core's data protection stack will store its
# keys.
$bucketName = "$projectId-bucket"
# Check to see if the bucket already exists.
$matchingBucket = (gsutil ls -b gs://$bucketName) 2> $null
if ($matchingBucket) {
    Write-Host "The bucket $bucketName already exists."
} else {
    # Create the bucket.
    gsutil mb -p $projectId gs://$bucketName
}

# Save the names of the key and bucket to appsettings.json.
$appsettings = Get-Content -Raw appsettings.json | ConvertFrom-Json
$appsettings.DataProtection.KmsKeyName = $dataprotectionKey
$appsettings.DataProtection.Bucket = $bucketName
ConvertTo-Json $appsettings | Out-File -Encoding utf8 -FilePath appsettings.json 
