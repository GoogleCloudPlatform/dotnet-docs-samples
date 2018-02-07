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

Import-Module -DisableNameChecking ..\..\..\..\BuildTools.psm1

Require-Platform Win*

dotnet restore
$url = "http://localhost:7412"
$job = Run-Kestrel($url)
Start-Sleep -Seconds 5
try {
	.\Test.ps1 "$url/echo"
} finally {
	Stop-Job $job
	Receive-Job $job
	Remove-Job $job
}