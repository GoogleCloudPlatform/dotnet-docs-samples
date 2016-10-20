# Copyright(c) 2016 Google Inc.
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
Import-Module ..\..\..\BuildTools.psm1 -DisableNameChecking

# Update Quickstart sample to include Project ID from Environment Variable
$filePath = "QuickStart\Program.cs"
$projectId = $env:GOOGLE_PROJECT_ID
$projectIdPlaceholder = "YOUR_PROJECT_ID"
(get-content $filePath) | foreach-object {$_ -replace $projectIdPlaceholder, $projectId} | set-content $filePath

Build-Solution
packages\xunit.runner.console.2.1.0\tools\xunit.console.exe .\BigqueryTest\bin\Debug\BigqueryTest.dll

# Update Quickstart sample to replace Project ID with original placeholder text
(get-content $filePath) | foreach-object {$_ -replace $projectId, $projectIdPlaceholder} | set-content $filePath