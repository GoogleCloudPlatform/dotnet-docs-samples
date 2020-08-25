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

# Skipping all tests: https://github.com/GoogleCloudPlatform/dotnet-docs-samples/issues/1066

# Import-Module ..\..\..\BuildTools.psm1 -DisableNameChecking

# $filesToProcess = "..\LoggingSample\Program.cs", "..\QuickStart\QuickStart.cs"

# BackupAndEdit-TextFile $filesToProcess `
#     @{"YOUR-PROJECT-ID" = $env:GOOGLE_PROJECT_ID} `
# {
#     dotnet test --test-adapter-path:. --logger:junit 2>&1 | %{ "$_" }
# }

# Let's at least build.

dotnet build 2>&1 | %{ "$_" }
