#
# Copyright 2020 Google LLC
#
# Licensed to the Apache Software Foundation (ASF) under one
# or more contributor license agreements.  See the NOTICE file
# distributed with this work for additional information
# regarding copyright ownership.  The ASF licenses this file
# to you under the Apache License, Version 2.0 (the
# "License"); you may not use this file except in compliance
# with the License.  You may obtain a copy of the License at
# 
#   http://www.apache.org/licenses/LICENSE-2.0
# 
# Unless required by applicable law or agreed to in writing,
# software distributed under the License is distributed on an
# "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
# KIND, either express or implied.  See the License for the
# specific language governing permissions and limitations
# under the License.
#

#
# This file contains a collection of functions for automating
# common deployment tasks.
#

#Requires -RunAsAdministrator
#Requires -version 4

Set-StrictMode -Version Latest
$ErrorActionPreference = "stop"

$_DefaultWebRoot = "c:\inetpub\wwwroot"

<#-----------------------------------------------------------------------------
 #  .SYNOPSIS
 #      Install IIS and dependencies requires to run ASP.NET 4.5 
 #        web applications.
 #
 #>
function Install-Iis() {
    [CmdletBinding()]
    Param()

    $WindowsFeatures = @(
        "Web-Server",
        "NET-FRAMEWORK-45-Core", 
        "NET-FRAMEWORK-45-ASPNET", 
        "Web-HTTP-Logging", 
        "Web-NET-Ext45", 
        "Web-ASP-Net45"
    )
    
    Add-WindowsFeature $WindowsFeatures
}

<#-----------------------------------------------------------------------------
 #   .SYNOPSIS
 #       Extract a WebDeploy (.zip) package to a target directory, stripping
 #        intermediate directories.
 #
 #   .PARAMETER DeploymentPackage
 #       Path to the package (.zip) file.
 #
 #   .PARAMETER TargetPath
 #       Path to extract to, typically the web root.
 #>

function Install-WebDeployPackage {
    [CmdletBinding()]
    Param(
        [Parameter(Mandatory=$True)][string] $DeploymentPackage,
        [Parameter(Mandatory=$False)][string] $TargetPath = $_DefaultWebRoot 
    )

    #
    # WebDeploy zip files contains a deeply nested directory structure.
    # Extract Zip to a temporary folder first to strip unnecessary folders.
    #

    Remove-Item -Recurse -Force -ErrorAction Ignore $env:TEMP\app
    Remove-Item -Recurse -Force $TargetPath\*
    New-Item -ItemType directory -Force $TargetPath | Out-Null

    $TempDir = New-Item -ItemType directory -Force -Path $env:TEMP\app
    Add-Type -AssemblyName System.IO.Compression.FileSystem
    [System.IO.Compression.ZipFile]::ExtractToDirectory($DeploymentPackage, $TempDir.FullName)

    $PackageTmpPath = (dir -recurse "$env:TEMP\app\**\PackageTmp" | % { $_.FullName })
    Move-Item -Path "$PackageTmpPath/*" -Destination $TargetPath -Force
}

<#-----------------------------------------------------------------------------
 #    .SYNOPSIS
 #        Create a web application and pool in IIS.
 #
 #    .PARAMETER AppName
 #        Name of the web application.
 #
 #    .PARAMETER TargetPath
 #        Path on disk where the application files reside.
 #>

function Register-WebApplication {
    [CmdletBinding()]
    Param(
        [Parameter(Mandatory=$True)][string] $AppName,
        [Parameter(Mandatory=$False)][string] $TargetPath = $_DefaultWebRoot 
    )

    #
    # Configure IIS web application pool and application
    #
    Import-Module WebAdministration
    if (-not (Test-Path IIS:\AppPools\$AppName)) {
        New-WebAppPool -Force $AppName
        New-WebApplication -Name $AppName -Site 'Default Web Site' -PhysicalPath $TargetPath -ApplicationPool $AppName
    }

    Set-ItemProperty "IIS:\AppPools\$AppName" "managedRuntimeVersion" "v4.0"

    #
    # Grant read/execute access to the application pool user.
    #
    & icacls $TargetPath /T /Q /C /reset
    & icacls $TargetPath /grant "IIS AppPool\${AppName}:(OI)(CI)(RX)"
}