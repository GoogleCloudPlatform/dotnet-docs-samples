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

# Fetch Pester test framework.
$pesterDir = [System.IO.Path]::GetFullPath(
    [System.IO.Path]::Combine($env:TEMP, 'PesterRepo', (Get-Random)))
mkdir -Force $pesterDir
Push-Location .
try {
    Set-Location $pesterDir
    git clone https://github.com/pester/Pester.git
    Set-Location Pester
    # Checkout a known-good commit for stability and security.
    git checkout --detach 9517657b5af3aba9f272336a06371b4791f18b93
    if ($LASTEXITCODE) {
        throw "Failed to checkout known good Pester commit."
    }
    Import-Module .\Pester.psm1
} finally {
    Pop-Location
}

# Run the tests.
$report = Invoke-Pester -PassThru
if ($report.PassedCount -lt $report.TotalCount) {
    throw $report
}
