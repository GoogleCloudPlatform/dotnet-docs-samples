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
Add-Type -AssemblyName System.IO.Compression.FileSystem
function Unzip([string]$zipfile, [string]$outpath)
{
    [System.IO.Compression.ZipFile]::ExtractToDirectory($zipfile, $outpath)
}

# Install codeformatter
Unzip $env:KOKORO_GFILE_DIR\codeformatter.zip \codeformatter
$codeformatterInstallPath = Resolve-Path \codeformatter
$env:PATH = "$env:PATH;$codeformatterInstallPath\bin"

# Add msbuild 14 to the path for for code-formatter.
$env:PATH="$env:PATH;C:\Program Files (x86)\MSBuild\14.0\Bin"
Get-Command MSBuild.exe

# Lint the code
Push-Location
try {
    Set-Location github\dotnet-docs-samples\
    Import-Module .\BuildTools.psm1
    Lint-Code
} finally {
    Pop-Location
}

# Install phantomjs
Unzip $env:KOKORO_GFILE_DIR\phantomjs-2.1.1-windows.zip \
$env:PATH = "$env:PATH;$(Resolve-Path \phantomjs-2.1.1-windows)\bin"

# Install casperjs
Unzip $env:KOKORO_GFILE_DIR\n1k0-casperjs-1.0.3-0-g76fc831.zip \
$casperJsInstallPath = Resolve-Path \n1k0-casperjs-76fc831
$env:PATH = "$env:PATH;$casperJsInstallPath\batchbin"
# Patch casperjs
Copy-Item -Force github\dotnet-docs-samples\.kokoro\docker\bootstrap.js `
    $casperJsInstallPath\bin\bootstrap.js

# Install casperjs 1.1
Unzip $env:KOKORO_GFILE_DIR\casperjs-1.1.4-1.zip \
$casperJsInstallPath = Resolve-Path \casperjs-1.1.4-1
$env:CASPERJS11_BIN = "$casperJsInstallPath\bin"

# Casperjs 1.1 needs python in the path.
$env:PATH = "$env:PATH;C:\Python27"

# Install dotnet core sdk.
choco install -y dotnetcore-sdk --version 2.0.0
choco install -y --sxs dotnetcore-sdk --version 1.1.2

# Install nuget command line.
choco install nuget.commandline

# Run the tests.
github\dotnet-docs-samples\.kokoro\main.ps1
