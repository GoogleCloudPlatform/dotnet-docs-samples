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

param([switch]$Lint, [switch]$UpdatePackages, [string]$PackageMask="Google.*")

$private:invocation = (Get-Variable MyInvocation -Scope 0).Value

Import-Module (Join-Path (Split-Path $invocation.MyCommand.Path) `
    BuildTools.psm1) -DisableNameChecking

# First, lint everything.  If the lint fails, don't waste time running
# tests.
if ($Lint) {
    Lint-Code
}
if ($UpdatePackages) {
    Update-Packages $PackageMask
}
$private:modifiedConfigs = Update-Config
Try {
    $private:testScripts = Find-Files -Masks '*runtests*.ps1' 
    # Avoid infinitely recursing and invoking this script.
    $testScripts | where {$_ -ne $invocation.MyCommand.Path} |  Run-TestScripts
}
Finally {
    Revert-Config $modifiedConfigs
}
