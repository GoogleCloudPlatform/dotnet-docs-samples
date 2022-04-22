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

param([int]$GroupNumber = 0, [bool]$IsRunningOnWindows = $false)

Push-Location
try {
    # Import BuildTools.psm1
    $private:invocation = (Get-Variable MyInvocation -Scope 0).Value
    Set-Location (Join-Path (Split-Path $invocation.MyCommand.Path) ..)
    Import-Module  .\BuildTools.psm1 -DisableNameChecking

    # Import secrets:
    .\.kokoro-windows\Import-Secrets.ps1

    # The list of changed subdirectories.
    git config --global --add safe.directory /tmpfs/src/github/dotnet-docs-samples
    $changedDirs = ($(git diff main --name-only | cut -d/ -f 1 | uniq))

    # The list of all subdirectories.
    $allDirs = Get-ChildItem | Where-Object {$_.PSIsContainer} | Select-Object -ExpandProperty Name

    # If no dirs have changed we run everything since we are most likely on CI.
    if ($changedDirs.Count -gt 0)
    {
        $testDirs = $changedDirs
    }
    else
    {
        $testDirs = $allDirs
    }

    # There are too many tests to run in a single Kokoro job.  So, we split
    # the tests into groups.  Each Kokoro job runs one group.

    # Groups of subdirectories.
    $groups = @(
        $false,  # 0: Everything.
        $false,  # 1: Everything starting from a to e.
        $false,  # 2: Everything starting from f to r, except for iot when on Linux because of the BouncyCastle and other dependencies.
        $false   # 3: Everything starting from s to z.
    )

    $groups[0] = $testDirs
    $groups[1] = $testDirs | Where-Object { ($_.Substring(0, 1).CompareTo("a") -ge 0) -and ($_.Substring(0, 1).CompareTo("e") -le 0) }
    $groups[2] = $testDirs | Where-Object { ($_.Substring(0, 1).CompareTo("f") -ge 0) -and ($_.Substring(0, 1).CompareTo("r") -le 0) -and ($IsRunningOnWindows -or -not ($_.Equals("iot"))) }
    $groups[3] = $testDirs | Where-Object { ($_.Substring(0, 1).CompareTo("s") -ge 0) -and ($_.Substring(0, 1).CompareTo("z") -le 0) }
    $dirs = $groups[$GroupNumber]

    if ($dirs.Count -gt 0)
    {
        # Find all the runTest scripts.
        $scripts = Get-ChildItem -Path $dirs -Filter *runTest*.ps* -Recurse
        $scripts.VersionInfo.FileName `
            | Sort-Object -Descending -Property {Get-GitTimeStampForScript $_} `
            | Run-TestScripts -TimeoutSeconds 600
    }
} finally {
    Pop-Location
}

