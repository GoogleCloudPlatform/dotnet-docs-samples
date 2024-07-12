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

    $private:currentDir = (Get-Location).Path

    # The list of changed subdirectories.
    # On the Windows Kokoro image we need to run these Git commands to make things work.
    git config --global --add safe.directory "$currentDir"
    git config --global --add core.autocrlf input
    git --git-dir="$currentDir/.git" --work-tree="$currentDir" config core.filemode false
    $changedDirs = ($(git --git-dir="$currentDir/.git" --work-tree="$currentDir" diff main --name-only | cut -d/ -f 1 | uniq))
    # On some environments we get empty folder names, remove those
    $changedDirs = ($changedDirs | Where { $null -ne $_ -and $_.Length -gt 0})

    # We run everything ...
    $testDirs = Get-ChildItem | Where-Object {$_.PSIsContainer} | Select-Object -ExpandProperty Name
    # On some environments we get empty folder names, remove those
    $testDirs = ($testDirs | Where { $null -ne $_ -and $_.Length -gt 0})

    # ... unless we detect changes to specific directories
    if ($changedDirs.Count -gt 0)
    {
        $testDirs = $changedDirs
    }

    # For diagnosis purposes only
    Write-Output "Changed dirs: $changedDirs"
    Write-Output "Test dirs: $testDirs"

    # There are too many tests to run in a single Kokoro job.  So, we split
    # the tests into groups.  Each Kokoro job runs one group.

    # Groups of subdirectories.
    $groups = @(
        $false,  # 0: Everything.
        $false,  # 1: Everything starting from a to e.
        $false,  # 2: Everything starting from f to r.
        $false   # 3: Everything starting from s to z.
    )

    $groups[0] = $testDirs
    $groups[1] = $testDirs | Where-Object { ($_.Substring(0, 1).CompareTo("a") -ge 0) -and ($_.Substring(0, 1).CompareTo("e") -le 0) }
    $groups[2] = $testDirs | Where-Object { ($_.Substring(0, 1).CompareTo("f") -ge 0) -and ($_.Substring(0, 1).CompareTo("r") -le 0) }
    $groups[3] = $testDirs | Where-Object { ($_.Substring(0, 1).CompareTo("s") -ge 0) -and ($_.Substring(0, 1).CompareTo("z") -le 0) }
    $dirs = $groups[$GroupNumber]

    Write-Output "Shard: $GroupNumber"
    Write-Output "Sharded dirs: $dirs"

    if ($dirs.Count -gt 0)
    {
        # Find all the runTest scripts.
        $scripts = Get-ChildItem -Path $dirs -Filter *runTest*.ps* -Recurse
        Write-Output "Scripts: $scripts"

        if ($scripts.Count -gt 0)
        {
            $scripts.VersionInfo.FileName `
                | Run-TestScripts -TimeoutSeconds 600
        }
    }
} finally {
    Pop-Location
}

