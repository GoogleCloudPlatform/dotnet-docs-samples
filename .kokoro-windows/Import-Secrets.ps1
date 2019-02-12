# Copyright(c) 2017 Google Inc.
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

<# .SYNOPSIS
    Imports the same secrets kokoro uses into your environment.
.DESCRIPTION
    Will load them immediately from $env:KOKORO_GFILE_DIR if it is set.
    Otherwise, downloads them from google cloud storage.
#>

# Names of the two files containing secrets.
$secrets = 'secrets2.ps1'
$credentials = 'dotnet-docs-samples-tests-9715b8c501e8.json'
$privatekeyfile = 'rsa_private.pem'
$certfile = 'roots.pem'
$kokorodir = $env:KOKORO_GFILE_DIR
if (-not $kokorodir) {
    $tempfile = [System.IO.Path]::GetTempFileName()
    remove-item $tempfile
    new-item -type directory -path $tempfile
    $kokorodir = $tempfile
    @($secrets, $credentials, $privatekeyfile, $certfile) | ForEach-Object { 
        gsutil cp gs://cloud-devrel-kokoro-resources/dotnet-docs-samples/$_  (Join-Path $kokorodir $_)
    }
}
& (Join-Path $kokorodir $secrets)
$env:GOOGLE_APPLICATION_CREDENTIALS=(Join-Path $kokorodir $credentials)

$env:IOT_PRIVATE_KEY_PATH=(Join-Path $kokorodir "rsa_private.pem")
$env:IOT_CERT_KEY_PATH=(Join-Path $kokorodir "roots.pem")