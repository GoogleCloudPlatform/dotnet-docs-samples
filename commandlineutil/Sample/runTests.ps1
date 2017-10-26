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

##############################
#.SYNOPSIS
#Need a function with a return value so I can throw within a boolean expression.
##############################
function Throw($message) {
    throw $message
}

dotnet restore
dotnet build 
dotnet run --no-build -- v1
$LASTEXITCODE -eq 0 -or (Throw "Expected exit code 0 for v1")
dotnet run --no-build -- v5
$LASTEXITCODE -eq 5 -or (Throw "Expected exit code 5 for v5")
dotnet run --no-build -- no-such-verb
$LASTEXITCODE -eq 255 -or (Throw "Expected exit code 255 for no-such-verb")
dotnet run --no-build -- v17
$LASTEXITCODE -eq 0 -or (Throw "Expected exit code 0 for v17")
