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

param([string][Parameter(Mandatory=$true)]$serviceAccountEmail, 
    [string][Parameter(Mandatory=$true)]$projectId)

$services = @(
    'dlp.googleapis.com', 
    'clouderrorreporting.googleapis.com')
$roles = @(
    'roles/dlp.admin',
    'roles/errorreporting.admin',
    'roles/cloudkms.admin',
    'roles/cloudkms.cryptoKeyEncrypterDecrypter',
    'roles/logging.logWriter',
    'roles/logging.admin',
    'roles/logging.privateLogViewer')

Write-Host "Enabling $services..."
gcloud services enable $services
function Bind($serviceAccountEmail, $role, $projectId) {
    Write-Host "Binding $serviceAccountEmail to $role..."
    $out = gcloud projects add-iam-policy-binding $projectId --member=serviceAccount:$serviceAccountEmail --role=$role
    if ($LASTEXITCODE) {
        throw $out
    }
}

foreach ($role in $roles) {
    Bind $serviceAccountEmail $role $projectId
}

# Special binding for IAP.
Bind $serviceAccountEmail roles/iap.httpsResourceAccessor surferjeff-iap
