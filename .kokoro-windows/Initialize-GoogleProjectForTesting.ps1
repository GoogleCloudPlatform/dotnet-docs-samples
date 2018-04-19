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
param([string][Parameter(Mandatory=$true)]$serviceAccountEmail, 
    [string][Parameter(Mandatory=$true)]$projectId)

$services = @"
bigquery-json.googleapis.com
clouddebugger.googleapis.com
datastore.googleapis.com
storage-component.googleapis.com
pubsub.googleapis.com
speech.googleapis.com
vision.googleapis.com
storage-api.googleapis.com
stackdriver.googleapis.com
clouderrorreporting.googleapis.com
logging.googleapis.com
cloudapis.googleapis.com
monitoring.googleapis.com
language.googleapis.com
stackdriverprovisioning.googleapis.com
compute.googleapis.com
sql-component.googleapis.com
cloudkms.googleapis.com
cloudtrace.googleapis.com
dlp.googleapis.com
servicemanagement.googleapis.com
videointelligence.googleapis.com
translate.googleapis.com
"@

$roles = @"
roles/bigquery.admin
roles/clouddebugger.user
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
roles/iap.httpsResourceAccessor
roles/logging.admin
roles/logging.logWriter
roles/logging.privateLogViewer
roles/logging.viewer
roles/monitoring.admin
roles/pubsub.admin
roles/storage.admin
"@

Write-Host "Enabling $services..."
gcloud services enable $services
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
