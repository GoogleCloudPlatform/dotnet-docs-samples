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

# Load secrets from the files downloaded from google cloud storage.
Get-ChildItem $env:KOKORO_GFILE_DIR
& "$env:KOKORO_GFILE_DIR/secrets.ps1"
$env:GOOGLE_APPLICATION_CREDENTIALS="$env:KOKORO_GFILE_DIR/silver-python2-69452e94c2bf.json"

Push-Location
try {
    # buildAndRunTests
    $private:invocation = (Get-Variable MyInvocation -Scope 0).Value
    Set-Location (Join-Path (Split-Path $invocation.MyCommand.Path) ..)
    .\buildAndRunTests.ps1
} finally {
    Pop-Location
}

