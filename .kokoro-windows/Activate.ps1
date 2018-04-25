# Copyright 2017 Google Inc.
#
# Licensed under the Apache License, Version 2.0 (the "License");
# you may not use this file except in compliance with the License.
# You may obtain a copy of the License at
#
#      http://www.apache.org/licenses/LICENSE-2.0
#
# Unless required by applicable law or agreed to in writing, software
# distributed under the License is distributed on an "AS IS" BASIS,
# WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
# See the License for the specific language governing permissions and
# limitations under the License.

$installDir = Resolve-Path "$PSScriptRoot\install"

$env:PATH = @("$installDir\codeformatter\bin",
    "$installDir\phantomjs-2.1.1-windows\bin",
    "$installDir\n1k0-casperjs-76fc831\batchbin",
    "$env:SystemDrive\Python27",
    $env:PATH,
    "$env:SystemDrive/Program Files (x86)/MSBuild/14.0/Bin",
    "$env:SystemDrive\Program Files (x86)\IIS Express") -join ';'

$env:CASPERJS11_BIN = "$installDir\casperjs-1.1.4-1\bin"

if (Get-Module BuildTools) {
    Remove-Module BuildTools
}
# 3 more lines will be appended here.