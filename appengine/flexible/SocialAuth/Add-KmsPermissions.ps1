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

##############################
#.SYNOPSIS
# Updates permissions for KMS so that the service account can create KMS keys
# and encrypt and decrypt data.
#
#.PARAMETER ServiceAccountEmail
# The email address of the service account.
#
#.PARAMETER ProjectId
# The Google Cloud Project ID to add permissions to.
#
#.EXAMPLE
# .\Add-KmsPermissions.ps1. my-service-account@my-project.iam.gserviceaccount.com
##############################

Param (
    [Parameter(Mandatory=$true)][string]$ServiceAccountEmail, 
    [string]$ProjectId
)

gcloud services enable cloudkms.googleapis.com

$email = if (-not $ServiceAccountEmail.Contains('@')) {
    (gcloud iam service-accounts list --format=json "--filter=email:($ServiceAccountEmail@*)" `
        | ConvertFrom-Json).email
} else {
    $ServiceAccountEmail
}

$projectId = if ($ProjectId) { 
    $ProjectId
} else { 
    gcloud config get-value project 
}

###############################################################################
# Permissions for App Engine to decrypt appsecrets.json.
$keyName = [string] (Get-Content ./appsecrets.json.keyname)
# Drop the last two segments of the path to get the key ring name.
$keyRingName = ($keyName.Split('/') | Select-Object -SkipLast 2) -join "/"
# Give App Engine permission to decrypt using keys in this keyring.
$role = 'roles/cloudkms.cryptoKeyDecrypter'
Write-Host "Adding role $role to $email for $keyRingName."
gcloud kms keyrings add-iam-policy-binding $keyRingName `
    --member serviceAccount:$email --role $role

###############################################################################    
# Permissions for App Engine to encrypt and decrypt secrets for
# KmsDataProtectionProvider.

# Check to see if the key ring already exists.
$keyRingId = 'dataprotectionprovider'
# Check to see if the key ring already exists.
$matchingKeyRing = (gcloud kms keyrings list --format json --location global --filter="name~.*/$keyRingId" | convertfrom-json).name
if (-not $matchingKeyRing) {
    # Create the new key ring.
    Write-Host "Creating new key ring $keyRingId..." 
    gcloud kms keyrings create $keyRingId --location global
}
    
$roles = @('roles/cloudkms.admin', 'roles/cloudkms.cryptoKeyEncrypterDecrypter')
foreach ($role in $roles) {
    Write-Host "Adding role $role to $email for $keyRingId."
    gcloud kms keyrings add-iam-policy-binding $keyRingId `
        --project $projectId --location 'global' `
        --member serviceAccount:$email --role $role
}
