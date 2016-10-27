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
Import-Module ..\..\BuildTools.psm1 -DisableNameChecking

$SUCCEEDED = $true

$quickStartCopy = [System.IO.Path]::GetTempFileName()
Copy-Item -Force QuickStart\Program.cs $quickStartCopy
try {
    Get-Content $quickStartCopy | ForEach-Object { 
        $_.Replace("YOUR-PROJECT-ID", $env:GOOGLE_PROJECT_ID)
    } | Out-File -Encoding UTF8 QuickStart\Program.cs

    Build-Solution
    packages\xunit.runner.console.2.1.0\tools\xunit.console.exe `
        .\QuickStartTest\bin\Debug\QuickStartTest.dll `
        -parallel none
    $SUCCEEDED = $SUCCEEDED -and $LASTEXITCODE -eq 0
} finally {
    Copy-Item -Force $quickStartCopy QuickStart\Program.cs
}

if (-not $SUCCEEDED) { throw "FAILED" }