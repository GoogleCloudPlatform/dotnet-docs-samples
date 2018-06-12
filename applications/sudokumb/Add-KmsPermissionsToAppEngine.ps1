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
# Updates permissions for KMS so that the App Engine default service account
# can create KMS keys and encrypt and decrypt data.
#
#.DESCRIPTION
# Make sure you have created an App Engine application before running this
# script.
#
#.EXAMPLE
# .\Add-KmsPermissionsToAppEngine.ps1.
##############################
# Keep this list sorted so it's easy to find an api and avoid duplicates.
$services = @"
cloudkms.googleapis.com
"@

# Enabling services takes a while, so only enable the services that are not
# already enabled.
$enabledServices = (gcloud services list --enabled --format json | convertfrom-json).serviceName
$alreadyEnabledServices = $enabledServices | Where-Object {$enabledServices.Contains($_)}
$servicesToEnable = $services.Split() | Where-Object {-not $enabledServices.Contains($_)}
if ($alreadyEnabledServices) {
    "Some services are already enabled:", $alreadyEnabledServices | Write-Host
}
if ($servicesToEnable) {
    Write-Host "Enabling $servicesToEnable..."
    gcloud services enable $servicesToEnable
}

$accounts = gcloud iam service-accounts list --format=json | ConvertFrom-Json
$appEngineAccount = $accounts | `
    Where-Object displayName -eq 'App Engine default service account'

if (-not $appEngineAccount) {
    throw "Could not find the App Engine Default service account in $accounts"
}

$email = $appEngineAccount.email
$projectId = $appEngineAccount.projectId
$roles = @('roles/cloudkms.admin', 'roles/cloudkms.cryptoKeyEncrypterDecrypter')
foreach ($role in $roles) {
    Write-Output "Adding role $role to $email."
    gcloud projects add-iam-policy-binding $projectId `
        --member serviceAccount:$email --role $role
}
