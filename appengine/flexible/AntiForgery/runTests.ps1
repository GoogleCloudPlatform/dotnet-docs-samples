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

Import-Module -DisableNameChecking ..\..\..\BuildTools.psm1

Backup-File appsettings.json {
    $email = (Get-Content $env:GOOGLE_APPLICATION_CREDENTIALS | ConvertFrom-Json).client_email
    .\SetUp.ps1 -serviceAccountEmail $email
    Run-KestrelTest 5512 -CasperJs11
}