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
# Enables all the necessary APIs and binds user to roles to be able to run
# kokoro tests.
#
#
#.PARAMETER serviceAccountEmail
#The email address of the service account used to run tests.
#
#.PARAMETER projectId
#The google cloud project id where the tests will run.
##############################
param([string]$serviceAccountEmail, [string]$projectId)

if (-not $serviceAccountEmail) {
    $serviceAccountEmail = (Get-Content $env:GOOGLE_APPLICATION_CREDENTIALS `
        | ConvertFrom-Json).client_email
}

if (-not $projectId) {
    $projectId = (Get-Content $env:GOOGLE_APPLICATION_CREDENTIALS `
    | ConvertFrom-Json).project_id
}

# Keep this list sorted so it's easy to find an api and avoid duplicates.
$services = @"
bigquery-json.googleapis.com
cloudapis.googleapis.com
clouddebugger.googleapis.com
clouderrorreporting.googleapis.com
cloudiot.googleapis.com
cloudkms.googleapis.com
cloudresourcemanager.googleapis.com
cloudtrace.googleapis.com
compute.googleapis.com
datastore.googleapis.com
dlp.googleapis.com
iam.googleapis.com
jobs.googleapis.com
language.googleapis.com
logging.googleapis.com
monitoring.googleapis.com
pubsub.googleapis.com
servicemanagement.googleapis.com
speech.googleapis.com
sql-component.googleapis.com
stackdriver.googleapis.com
stackdriverprovisioning.googleapis.com
storage-api.googleapis.com
storage-component.googleapis.com
texttospeech.googleapis.com
translate.googleapis.com
videointelligence.googleapis.com
vision.googleapis.com
"@

$roles = @"
roles/bigquery.admin
roles/clouddebugger.user
roles/cloudiot.admin
roles/cloudkms.admin
roles/cloudkms.cryptoKeyEncrypterDecrypter
roles/cloudsql.client
roles/cloudsql.editor
roles/cloudtrace.user
roles/datastore.user
roles/dlp.admin
roles/dlp.user
roles/errorreporting.admin
roles/errorreporting.user
roles/iam.roleAdmin
roles/iam.securityReviewer
roles/iam.serviceAccountAdmin
roles/iam.serviceAccountKeyAdmin
roles/iap.httpsResourceAccessor
roles/logging.admin
roles/logging.logWriter
roles/logging.privateLogViewer
roles/logging.viewer
roles/monitoring.admin
roles/pubsub.admin
roles/resourcemanager.projectIamAdmin
roles/storage.admin
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

function Bind($serviceAccountEmail, $role, $projectId) {
    Write-Host "Binding $serviceAccountEmail to $role..."
    $out = gcloud projects add-iam-policy-binding $projectId --member=serviceAccount:$serviceAccountEmail --role=$role
    if ($LASTEXITCODE) {
        throw $out
    }
}

foreach ($role in $roles.Split()) {
    if ($role) {
        Bind $serviceAccountEmail $role $projectId
    }
}

# Special binding for IAP.
Bind $serviceAccountEmail roles/iap.httpsResourceAccessor surferjeff-iap
