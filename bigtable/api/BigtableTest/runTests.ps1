# Copyright(c) 2018 Google Inc.
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

# TODO: Resurrect this test when bug 68199801 is fixed.
Import-Module ..\..\..\BuildTools.psm1 -DisableNameChecking
Require-Platform Win*
Set-TestTimeout 600

BackupAndEdit-TextFile "..\QuickStart\QuickStart.cs", "..\HelloWorld\HelloWorld.cs", "..\TableAdminExample\TableAdmin.cs",  "..\InstanceAdminExample\InstanceAdmin.cs" `
    @{"YOUR-PROJECT-ID" = $env:GOOGLE_PROJECT_ID;
    "YOUR-INSTANCE-ID" = $env:TEST_BIGTABLE_INSTANCE;} `
{
    dotnet restore
    dotnet build
    dotnet test
}