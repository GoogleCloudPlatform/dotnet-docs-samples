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

##############################################################################
#.SYNOPSIS
# Builds and runs tests in subdirectories.
#
#.PARAMETER Lint
# Run the linter on projects in subdirectories.  Works be fixing the code in
# place, and then looking for git diffs.  So, be sure the git client is clean
# before running.
#
#.PARAMETER UpdatePackages
# Update each project's NuGet packages to the latest version before running tests.
#
#.PARAMETER PackageMask
# When UpdatePackages is set, specifies which packages to update.
#
#.PARAMETER Skip
# Skip tests with the named like 'skiptest'
#
#.EXAMPLE
# .\buildAndRunTests.ps1 -Lint
##############################################################################
param([switch]$Lint, [switch]$UpdatePackages, [string]$PackageMask="Google.*",
    [switch]$Skip, [switch]$Deploy, 
    [string[]]$TestMasks=@('*runtest*.ps1', '*skiptest*.ps1'))

$private:invocation = (Get-Variable MyInvocation -Scope 0).Value

Import-Module (Join-Path (Split-Path $invocation.MyCommand.Path) `
    BuildTools.psm1) -DisableNameChecking

# Dump the current state of gcloud, because it can affect a lot of tests.
gcloud info

# First, lint everything.  If the lint fails, don't waste time running
# tests.
if ($Lint) {
    Add-Copyright
    Find-Files -Masks *.csproj -AntiMasks appengine,endpoints,QuickStartCore | Lint-Code
}
if ($UpdatePackages) {
    Update-Packages $PackageMask
}
$private:modifiedConfigs = Update-Config
Try {
    $timeoutSeconds = 600
    $masks = if ($Skip) {
        "*runTest*.ps1"
    } elseif ($Deploy) {
        "*deployTest*.ps1"
    } else {
        $TestMasks
    }
    $private:testScripts = Find-Files -Masks $masks
    # Avoid infinitely recursing and invoking this script.
    $testScripts | where {$_ -ne $invocation.MyCommand.Path} |  Run-TestScripts $timeoutSeconds
}
Finally {
    Revert-Config $modifiedConfigs
}
