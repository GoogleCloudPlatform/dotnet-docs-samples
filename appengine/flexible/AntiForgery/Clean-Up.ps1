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

Import-Module .\SetUp.psm1

# Read the name of the bucket and key from appsettings.json.
$appsettings = Get-Content -Raw appsettings.json | ConvertFrom-Json

# Delete the storage bucket
$bucket = $appsettings.DataProtection.Bucket
gsutil rm -r "gs://$bucket"

# Destroy all versions of the key.
$chunks = Split-KmsKey $appsettings.DataProtection.KmsKeyName
$versions = gcloud kms keys versions list --project $chunks.projectId `
    --location $chunks.locationId ` --keyring $chunks.keyRingId `
    --key $chunks.keyId --format json | ConvertFrom-Json
foreach ($version in $versions) {
    if ($version.state -eq 'DESTROY_SCHEDULED') {
        continue
    }
    $versionId = $version.name.split('/')[-1]
    gcloud kms keys versions destroy $versionId --project $chunks.projectId `
        --location $chunks.locationId ` --keyring $chunks.keyRingId `
        --key $chunks.keyId
}
