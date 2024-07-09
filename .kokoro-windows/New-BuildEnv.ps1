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
Param ($Dir, [switch]$SkipDownloadKokoroDir)

if (-Not $Dir) {
    $Dir = "$PSScriptRoot\..\env"
}

# Install choco packages.
get-command choco -ErrorAction Stop
choco upgrade -y --sxs --no-progress chocolatey-windowsupdate.extension
choco list -li | Tee-Object -Variable chocoList
$chocoPackages = ($chocoList) -join ' '

if (-not $chocoPackages.Contains('Microsoft .NET SDK 6.')) {
    choco install -y --sxs --no-progress dotnet-6.0-sdk
}

if (-not $chocoPackages.Contains('Microsoft .NET SDK 8.')) {
    choco install -y --sxs --no-progress dotnet-8.0-sdk
}

if (-not $chocoPackages.Contains('nuget.commandline 4.5.')) {
    choco install -y --no-progress nuget.commandline
}

dotnet nuget add source https://api.nuget.org/v3/index.json -n nuget.org --configfile $env:APPDATA\NuGet\NuGet.Config

if (-not $chocoPackages.Contains('microsoft-build-tools 14.')) {
    choco install -y --sxs --no-progress microsoft-build-tools --version 14.0.23107.10
}

if (-not (($chocoPackages.Contains('python 2.7.') -or
    (Test-Path "$env:SystemDrive\Python27")))) {
    choco install -y --sxs --no-progress python --version 2.7.6
}

if (-not $chocoPackages.Contains('selenium-chrome-driver 77.')) {
    choco install -y --no-progress selenium-chrome-driver --version 77.0.3865.40
}
if (-not $chocoPackages.Contains('iisexpress')) {
    choco install -y --sxs --no-progress iisexpress
}

# Create environment directory structure.
$Dir = (New-Item -Path $Dir -ItemType Directory -Force).FullName
$installDir = Join-Path $Dir 'install'
(New-Item -Path $installDir -ItemType Directory -Force).FullName
if (-not $SkipDownloadKokoroDir) {
    $env:KOKORO_GFILE_DIR = Join-Path $Dir 'kokoro'
    (New-Item -Path $env:KOKORO_GFILE_DIR -ItemType Directory -Force).FullName
    # Copy all the files from our kokoro secrets bucket.
    gsutil -m cp gs://cloud-devrel-kokoro-resources/dotnet-docs-samples/* $env:KOKORO_GFILE_DIR
}

# Prepare to unzip the files.
Add-Type -AssemblyName System.IO.Compression.FileSystem
function Unzip([string]$zipfile, [string]$outpath)
{
    [System.IO.Compression.ZipFile]::ExtractToDirectory($zipfile, $outpath)
}

Set-PsDebug -Trace 1
# Install codeformatter
Unzip $env:KOKORO_GFILE_DIR\codeformatter.zip $installDir\codeformatter
# Install phantomjs
Unzip $env:KOKORO_GFILE_DIR\phantomjs-2.1.1-windows.zip $installDir
# Install casperjs
Unzip $env:KOKORO_GFILE_DIR\n1k0-casperjs-1.0.3-0-g76fc831.zip $installDir
# Patch casperjs
Copy-Item -Force $PSScriptRoot\..\.kokoro\docker\bootstrap.js `
    $installDir\n1k0-casperjs-76fc831\bin\bootstrap.js
# Install casperjs 1.1
Unzip $env:KOKORO_GFILE_DIR\casperjs-1.1.4-1.zip $installDir
Set-PsDebug -Off

# Copy Activate.ps1 to the environment directory.
# And append 3 more lines.
$activatePs1 = Get-Content $PSScriptRoot\Activate.ps1
$importSecretsPath = "$PSScriptRoot\Import-Secrets.ps1"
$buildToolsPath = Resolve-Path "$PSScriptRoot\..\BuildTools.psm1"
@($activatePs1, '', 
    "`$env:KOKORO_GFILE_DIR = '$env:KOKORO_GFILE_DIR'",
    "& '$importSecretsPath'",
    "Import-Module -DisableNameChecking '$buildToolsPath'") `
    | Out-File -Force $Dir\Activate.ps1
