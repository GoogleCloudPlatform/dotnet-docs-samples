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
# Logical operator.
#
#.DESCRIPTION
# When anything in pipelined to the function, outputs the inputs.
# Otherwise, evaluates the script block and returns the result.
#
#.PARAMETER ScriptBlock
# The script block to execute if $input is empty.
##############################################################################
function When-Empty($Target, $ArgList, [ScriptBlock]$ScriptBlock) {
    if ($Target) {
        @($Target) + $ArgList
    } elseif ($ArgList) {
        $ArgList
    } else {
        &$ScriptBlock
    }
}

##############################################################################
#.SYNOPSIS
# Recursively find all the files that match a mask.
#
#.DESCRIPTION
# Why not simply cal Get-ChildItem -Recurse ...?
# Traversing the many directories that are full of junk we're not interested
# can take a long time.  Unlike Get-ChildItem, this function does not
# traverse directories that match the AntiMasks.
#
#.PARAMETER Path
# Start searching from where?  Defaults to the current directory.
#
#.PARAMETER Masks
# A list of masks to match against the files.
#
#.PARAMETER MaxDepth
# How deep should we look into subdirectories?  Default is no limit.
#
#.PARAMETER AntiMasks
# Stop recursing when we reach a directory with a matching name.
#
#.EXAMPLE
# Find-Files -Masks *.txt
##############################################################################
function Find-Files($Path = $null, [string[]]$Masks = '*', $MaxDepth = -1,
    $Depth=0, [string[]]$AntiMasks = @('bin', 'obj', 'packages', '.git'))
{
    foreach ($item in Get-ChildItem $Path | Sort-Object -Property Mode,Name)
    {
        if ($Masks | Where {$item -like $_})
        {
            $item.FullName
        }
        if ($AntiMasks | Where {$item -like $_})
        {
            # Do not recurse.
        }
        elseif ($MaxDepth -ge 0 -and $Depth -ge $MaxDepth)
        {
            # We have reached the max depth.  Do not recurse.
        }
        elseif (Test-Path $item.FullName -PathType Container)
        {
            Find-Files $item.FullName $Masks $MaxDepth ($Depth + 1) $AntiMasks
        }
    }
}

##############################################################################
#.SYNOPSIS
# Look for a matching file in this directory and parent directories.
#
#.PARAMETER Masks
# A list of masks to match against the files.
#
#.EXAMPLE
# UpFind-File *.txt
##############################################################################
function UpFind-File([string[]]$Masks = '*')
{
    $dir = Get-Item .
    while (1)
    {
        foreach ($item in Get-ChildItem $dir | Sort-Object -Property Mode,Name)
        {
            if ($Masks | Where {$item -like $_})
            {
                return $item.FullName
            }
        }
        if (!$dir.parent)
        {
            return
        }
        $dir = Get-Item $dir.parent.FullName
    }
}

##############################################################################
#.SYNOPSIS
# Sets the timeout for a runTests.ps1.
#
#.DESCRIPTION
# When Run-TestScripts calls a runTests.ps1, then the runTests.ps1 can call
# Set-TestTimeout to override the default timeout.
#
#.EXAMPLE
# Set-TestTimeout 900
##############################################################################
function Set-TestTimeout($seconds) {
    New-Object PSObject -Property @{TimeoutSeconds = $seconds}
}

##############################################################################
#.SYNOPSIS
# Skips the currently running test.  Exits the currently running script.
#
#.EXAMPLE
# Skip-Test
##############################################################################
function Skip-Test() {
    New-Object PSObject -Property @{Skipped = $true} | Write-Output
    exit
}

##############################################################################
#.SYNOPSIS
# Checks the currrently running platform.  Exits if it's the wrong one.
#
#.EXAMPLE
# Require-Platform Win*
##############################################################################
function Require-Platform([string[]] $platforms) {
    foreach ($platform in $platforms) {
        if ([environment]::OSVersion.Platform -match $platform) {
            return
        }
    }
    Skip-Test
}

$junitOutputTemplate = @"
<testsuites failures="" tests="1" time="">
    <testsuite id="0" failures="" name="" tests="1" time="">
        <testcase classname="runTests" name="runTests" time="">
        </testcase>
    </testsuite>
</testsuites>
"@

##############################
#.SYNOPSIS
# Composes a junit xml file that reports a build failure, or timeout.
#
#.PARAMETER script
# The path the the test script that failed.
#
#.PARAMETER log
# The output of the test script.
#
#.PARAMETER elapsed
# The time spent running the test script.
#
#.PARAMETER timedOut
# $True when the test scripped timed out rather than failed.
##############################
function Write-FailureXml([string]$script, [string[]] $log, 
    [System.TimeSpan]$elapsed, [switch]$timedOut) 
{
    $elapsedSeconds = [string] $elapsed.TotalSeconds
    $xml = [xml]$junitOutputTemplate
    $xml.testsuites.failures = "1"
    $xml.testsuites.time = $elapsedSeconds
    $xml.testsuites.testsuite.name = $script
    $xml.testsuites.testsuite.time = $elapsedSeconds
    $xml.testsuites.testsuite.failures = "1"
    $xml.testsuites.testsuite.testcase.time = $elapsedSeconds
    $errorMessage = if ($timedOut) { 'TIMED OUT' } else { 'BUILD FAILED' }
    $errorXml = [xml]"<error message='$errorMessage' />"
    $errorXml.FirstChild.InnerText = ($log -join "`n") + "`n$errorMessage"
    $xml.testsuites.testsuite.testcase.AppendChild($xml.ImportNode(
        $errorXml.FirstChild, $True)) | Out-Null
    $testResultsXml = Join-Path (Split-Path -Parent $script) "TestResults.xml"
    $xml.Save($testResultsXml)
}

##############################
#.SYNOPSIS
# Composes a junit test script reporting a skipped test.
#
#.PARAMETER script
# The path the the test script that failed.
#
#.PARAMETER elapsed
# The time spent running the test script.
##############################
function Write-SkippedXml([string]$script, [System.TimeSpan]$elapsed) 
{
    $elapsedSeconds = [string] $elapsed.TotalSeconds
    $xml = [xml]$junitOutputTemplate
    $xml.testsuites.failures = "0"
    $xml.testsuites.time = $elapsedSeconds
    $xml.testsuites.testsuite.name = $script
    $xml.testsuites.testsuite.time = $elapsedSeconds
    $xml.testsuites.testsuite.failures = "0"
    $xml.testsuites.testsuite.testcase.time = $elapsedSeconds
    $skippedXml = [xml]'<skipped/>'
    $xml.testsuites.testsuite.testcase.AppendChild($xml.ImportNode(
        $skippedXml.FirstChild, $True)) | Out-Null
    $testResultsXml = Join-Path (Split-Path -Parent $script) "TestResults.xml"
    $xml.Save($testResultsXml)
}

##############################
#.SYNOPSIS
# Runs the list of test scripts.
#
#.DESCRIPTION
# Runs each test script once.  Records the results in $Results.
#
#.PARAMETER Scripts
# A list of scripts to run.  An array of Items.
#
#.PARAMETER TimeoutSeconds
# Give up if a test script takes longer than TimeoutSeconds to run.
#
#.PARAMETER Verb
# Verb to print in output message.
#
#.PARAMETER Results
# Gets filled with the resulting job status and list.  For example:
# { 'Failed' = @('.\pubsub\api\PubsubTest\runTest.ps1');
#   'Succeeded' = @('.\trace\api\runTests.ps1', '.\storage\api\runTests.ps1') }
##############################
function Run-TestScriptsOnce([array]$Scripts, [int]$TimeoutSeconds,
    [string]$Verb, [hashtable]$Results)
{
    foreach ($script in $Scripts) {
        # Fork a job for each script for a couple reasons:
        # 1. If it runs beyond its timeout, we can kill it and move on.
        # 2. We can tee its output to a separate file, and dump it into an
        #    XML failure report if it fails.
        $startDate = Get-Date
        $relativePath = Resolve-Path -Relative $script
        $jobState = 'Failed'
        $tempOut = [System.IO.Path]::GetTempFileName()
        Write-Output "$verb $relativePath..."
        $job = Start-Job -ArgumentList $relativePath, $script.Directory, `
            ('.\"{0}"' -f $script.Name) {
            $ErrorActionPreference = "Continue"
            Write-Output ("-" * 79)
            Write-Output $args[0]
            Set-Location $args[1]
            Invoke-Expression $args[2]
            if ($LASTEXITCODE) {
                throw "FAILED with exit code $LASTEXITCODE"
            }
        }
        # Call Receive-Job every second so the stdout for the job
        # streams to my stdout.
        while ($true) {
            Wait-Job $job -Timeout 1 | Out-Null
            $jobState = $job.State
            foreach ($line in (Receive-Job $job) | Tee-Object -Append -FilePath $tempOut) {
                # Look at the output of the job to see if it requested
                # a longer timeout.
                if ($line.TimeoutSeconds) {
                    $TimeoutSeconds = $line.TimeoutSeconds
                    "Set timeout to $TimeoutSeconds seconds."
                } elseif ($line.Skipped) {
                    Write-Output "SKIPPED"
                    $jobState = 'Skipped'
                    break
                } else {
                    $line
                }
            }
            if ($jobState -eq 'Running') {
                $deadline = $startDate.AddSeconds($TimeoutSeconds)
                if ((Get-Date) -gt $deadline) {
                    Write-Output "TIME OUT"
                    $jobState = 'Timed Out'
                    break
                }
            } else {
                break
            }
        }
        $results[$jobState] += @($relativePath)
        # If the script left no TestResults.xml, create one.
        $parentDir = (Get-Item $script).Directory
        if ($jobState -ne 'Completed' -and -not (Get-ChildItem -Path $parentDir -Recurse -Filter TestResults.xml)) {
            $elapsed = (Get-Date) - $startDate
            if ($jobState -eq 'Skipped') {
                Write-SkippedXml $script $elapsed
            } else {
                $job.ChildJobs[0].JobStateInfo.Reason.Message | Out-File -Append -FilePath $tempOut
                $job.ChildJobs[0].Error | Out-File -Append -FilePath $tempOut
                Write-FailureXml $script (Get-Content $tempOut) $elapsed -timedOut:($jobState -eq 'Timed Out')
            }
        }
        Remove-Job -Force $job
    }
}

##############################################################################
#.SYNOPSIS
# Runs powershell scripts and prints a summary of successes and errors.
#
#.INPUTS
# Powershell scripts.  If empty, recursively searches directories for
# scripts with 'runtests' in their names.
#
#.EXAMPLE
# Run-Tests
##############################################################################
function Run-TestScripts($TimeoutSeconds=300) {
    $scripts = When-Empty -ArgList ($input + $args) -ScriptBlock { Find-Files -Masks '*runtests*.ps1' } | Get-Item
    # Keep running lists of successes and failures.
    $results = @{}
    Run-TestScriptsOnce $scripts $TimeoutSeconds 'Starting' $results
    # Rename all the test logs to a name Sponge will find.
    Get-ChildItem -Recurse TestResults.xml | Rename-Item -Force -NewName 01_sponge_log.xml

    # Print a final summary.
    Write-Output ("=" * 79)
    foreach ($key in $results.Keys) {
        $count = $results[$key].Length
        $result = $key.Replace('Completed', 'Succeeded').ToUpper()
        Write-Output "$count $result"
        Write-Output $results[$key]
    }
    # Throw an exception to set ERRORLEVEL to 1 in the calling process.
    $failureCount = $results['Failed'].Length
    if ($failureCount) {
        throw "$failureCount FAILED"
    }
    $timeOutCount = $results['Timed Out'].Length
    if ($timeOutCount) {
        throw "$timeOutCount TIMED OUT"
    }
}

##############################################################################
#.SYNOPSIS
# Runs code formatter on a project or solution.
#
#.INPUTS
# .sln and .csproj files. If empty, recursively searches directories for
# project files.
#
#.EXAMPLE
# Format-Code
##############################################################################
filter Format-Code {
    $projects = When-Empty $_ $args { Find-Files -Masks *.csproj }
    foreach ($project in $projects) {
        Backup-File -Files $project -ScriptBlock {
            Convert-2003ProjectToCore $project
            "codeformatter.exe $project" | Write-Host
            codeformatter.exe /rule:BraceNewLine /rule:ExplicitThis `
                /rule-:ExplicitVisibility /rule:FieldNames `
                /rule:FormatDocument /rule:ReadonlyFields /rule:UsingLocation `
                /nocopyright $project
            if ($LASTEXITCODE) {
                $project.FullName
                throw "codeformatter failed with exit code $LASTEXITCODE."
            }
        }
    }
}

$coreProjectText = @"
<?xml version="1.0" encoding="utf-8"?>
<Project ToolsVersion="4.0" DefaultTargets="Build" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
    <ItemGroup>
        <Compile Include="Program.cs" />
    </ItemGroup>
</Project>
"@

##############################################################################
#.SYNOPSIS
# Converts .NET Core csproj to MSBuild 2003.
#
#.DESCRIPTION
# Creates the bare minimal project for code-formatter to work.
# If you open the project with Visual Studio everything will be broken.
#
#.PARAMETER csproj
# A project to convert.
#
#.EXAMPLE
# Convert-2003ProjectToCore AuthSample.csproj
##############################################################################
function Convert-2003ProjectToCore($csproj) {
    $cspath = (Resolve-Path $csproj).Path
    $xml = [xml](Get-Content $csproj)
    if (-not $xml.Project.xmlns -eq "http://schemas.microsoft.com/developer/msbuild/2003") {
        Write-Host "Converting .NET core $cspath to msbuild/2003..."
        $doc = [xml]$coreProjectText
        $group = $doc.Project.ItemGroup
        $compileTemplate = $group.Compile
        $sourceFiles = (Get-ChildItem -Path (Split-Path $cspath) -Filter "*.cs")
        foreach ($sourceFile in $sourceFiles) {
            $compile = $compileTemplate.Clone()
            $compile.Include = $sourceFile.FullName
            $group.AppendChild($compile) | Out-Null
        }
        $group.RemoveChild($compileTemplate) | Out-Null
        $doc.save($cspath)
    } else {
        Write-Host "$cspath looks like a 2003 .csproj to me!"
        $doc = [xml] (Get-Content $cspath)
        # This one import causes codeformatter to choke.  Turn it off.
        foreach ($import in $doc.Project.Import | Where-Object `
            {$_.Project -like '*\Microsoft.WebApplication.targets'})
        {
            $import.Condition = 'false'
        }
        $doc.save($cspath)
    }
}

##############################################################################
#.SYNOPSIS
# Runs code formatter on a project or solution.
#
#.DESCRIPTION Throws an exception if code formatter actually changed something.
#
#.INPUTS
# .sln and .csproj files. If empty, recursively searches directories for
# project files.
#
#.EXAMPLE
# Lint-Project
##############################################################################
filter Lint-Code {
<#     $projects = When-Empty $_ $args { Find-Files -Masks *.csproj }
    foreach ($project in $projects) {
        @($project) | Format-Code
        # If git reports a diff, codeformatter changed something, and that's bad.
        $diff = git diff
        if ($diff) {
            $diff
            throw "Lint failed for $_"
        }
    }
    $utf16 = Find-Utf16
    if ($utf16) {
        $utf16
        throw "Found UTF-16 encoded source files. Run Find-Utf16 | ConvertTo-Utf8 to fix."
    } #>
}

##############################################################################
#.SYNOPSIS
# Builds the .sln in the current working directory.
#
#.DESCRIPTION
# Invokes nuget first, then msbuild.  Throws an exception if nuget or the
# build fails.
##############################################################################
function Build-Solution($solution) {
    nuget restore $solution
    if ($LASTEXITCODE) {
        throw "Nuget failed with error code $LASTEXITCODE"
    }
    msbuild /p:Configuration=Debug $solution
    if ($LASTEXITCODE) {
        throw "Msbuild failed with error code $LASTEXITCODE"
    }
}

##############################################################################
#.SYNOPSIS
# Gets the port number for an IISExpress web site.
#
#.PARAMETER SiteName
# The name of the website, as listed in applicationhost.config
#
#.PARAMETER ApplicationhostConfig
# The path to applicationhost.config.
#
#.OUTPUTS
# The port number where the web site is specified to run.
##############################################################################
function Get-PortNumber($SiteName, $ApplicationhostConfig) {
    $node = Select-Xml -Path $ApplicationhostConfig `
        -XPath "/configuration/system.applicationHost/sites/site[@name='$SiteName']/bindings/binding" |
        Select-Object -ExpandProperty Node
    $chunks = $node.bindingInformation -split ':'
    $chunks[1]
}

##############################################################################
#.SYNOPSIS
# Runs IISExpress for a web site.
#
#.PARAMETER SiteName
# The name of the website, as listed in applicationhost.config
#
#.PARAMETER ApplicationhostConfig
# The path to applicationhost.config.
#
#.OUTPUTS
# The process object
##############################################################################
function Run-IISExpress($SiteName, $ApplicationhostConfig) {
    if (!$SiteName) {
        $SiteName = (get-item -Path ".\").Name
    }
    if (!$ApplicationhostConfig) {
        $ApplicationhostConfig = UpFind-File 'applicationhost.config'
    }
    $argList = ('/config:"' + $ApplicationhostConfig + '"'), "/site:$SiteName", "/apppool:Clr4IntegratedAppPool"
    Start-Process iisexpress.exe  -ArgumentList $argList -PassThru
}

##############################################################################
#.SYNOPSIS
# Run the website, then run the test javascript file with casper.
#
#.DESCRIPTION
# Throws an exception if the test fails.
#
#.PARAMETER SiteName
# The name of the website, as listed in applicationhost.config.
#
#.PARAMETER ApplicationhostConfig
# The path to applicationhost.config.  If not
# specified, searches parent directories for the file.
#
##############################################################################
function Run-IISExpressTest($SiteName = '', $ApplicationhostConfig = '',
    $TestJs = 'test.js', [switch]$LeaveRunning = $false) {
    if (!$SiteName) {
        $SiteName = (get-item -Path ".\").Name
    }
    if (!$ApplicationhostConfig) {
        $ApplicationhostConfig = UpFind-File 'applicationhost.config'
    }

    $port = Get-PortNumber $SiteName $ApplicationhostConfig
    $webProcess = Run-IISExpress $SiteName $ApplicationhostConfig
    Try
    {
        Start-Sleep -Seconds 4  # Wait for web process to start up.
        Run-CasperJs $TestJs http://localhost:$port -v11
        if ($LASTEXITCODE) {
            throw "Casperjs failed with error code $LASTEXITCODE"
        }
    }
    Finally
    {
        if (!$LeaveRunning) {
            Stop-Process $webProcess
        }
    }
}

##############################################################################
#.SYNOPSIS
# Run the website.
#
#.PARAMTER url
# The partial url to serve.  It should contain the scheme and host.  For
# example: https://localhost:2342
#
#.RETURNS
# The job running kestrel.
##############################################################################
function Run-Kestrel([Parameter(mandatory=$true)][string]$url)
{
    $kestrelJob = Start-Job -ArgumentList (Get-Location), $url -ScriptBlock {
        Set-Location $args[0]
        $env:ASPNETCORE_URLS = $args[1]
        dotnet run
    }
    # Wait for Kestrel to come up.
    while(-not $started) {
        Start-Sleep -Seconds 1
        $lines = Receive-Job $kestrelJob
        $lines | Write-Host
        if (($kestrelJob | Get-Job).State -ne 'Running') {
            throw "Kestrel failed to start."
        }
        foreach ($line in $lines) {
            if ($line.contains('Application started')) {
                return $kestrelJob
            }
        }
        $seconds++
        if ($seconds -gt 120) {
            throw "Kestrel took > 120 seconds to start up."
        }
    }
}

##############################################################################
#.SYNOPSIS
# Run the website, then run the test javascript file with casper.
#
#.PARAMETER PortNumber
# The port number to run the kestrel server on.
#
#.DESCRIPTION
# Throws an exception if the test fails.
#
##############################################################################
function Run-KestrelTest([Parameter(mandatory=$true)]$PortNumber, $TestJs = 'test.js', 
    [switch]$LeaveRunning = $false, [switch]$CasperJs11 = $false) {
    $url = "http://localhost:$PortNumber"
    $job = Run-Kestrel $url
    Try
    {
        Run-CasperJs $TestJs $Url -v11:$CasperJs11
    }
    Finally
    {
        if (!$LeaveRunning) {
            Stop-Job $job
            Receive-Job $job
            Remove-Job $job
        }
    }
}

##############################
#.SYNOPSIS
# Moves TestResults.xml into an output subdirectory.
#
#.PARAMETER OutDir
#The directory to which to move TestResults.xml.  This directory will be created
#if it does not already exist.
##############################
function Move-TestResults($OutDir) {
    if ($OutDir -and (Test-Path TestResults.xml)) {
        New-Item -ItemType Directory -Force -Path $OutDir
        Move-Item -Force TestResults.xml $OutDir
    }
}

##############################
#.SYNOPSIS
# Runs CasperJs
#
#.PARAMETER TestJs
# The Javascript file to run.
#
#.PARAMETER Url
# The url to pass to the test.  Usually points to running website to test.
#
#.PARAMETER v11
# Use CasperJs version 1.1 instead of 1.0.
##############################
function Run-CasperJs($TestJs='test.js', $Url, [switch]$v11 = $false,
    [string]$OutDir) {
    if ($v11) {
        $env:CASPERJS11_URL = $Url
        # Casperjs.exe creates a new terminal window, from which we
        # cannot capture output.  So we use python to invoke it and
        # capture output.
        $casperOut = python (Join-Path $env:CASPERJS11_BIN "casperjs") `
            -- test --xunit=TestResults.xml $TestJs
        # Casper 1.1 always returns 0, so inspect the xml output
        # to see if a test failed.
        [xml]$x = Get-Content TestResults.xml         
        $LASTEXITCODE = 0      
        foreach ($suite in $x.testsuites.testsuite) {
            $LASTEXITCODE += [int] $suite.failures 
        }
    } else {
        $casperOut = casperjs $TestJs $Url
    }
    if ($LASTEXITCODE -eq 0) {
        $casperOut | Write-Host
        return
    }
    $casperOut | Write-Host
    throw "Casperjs failed with error code $LASTEXITCODE"
}

##############################################################################
#.SYNOPSIS
# Deploy a dotnet core application to appengine and test it.
#
#.DESCRIPTION
# Assumes the app was already build with "dotnet publish."
# Assumes there is a subdirectory called appengine containing app.yaml and Dockerfile.
#
#.PARAMETER DllName
# The name of the built binary.  Defaults to the current directory name.
##############################################################################
function Deploy-CasperJsTest($testJs ='test.js') {
    while ($true) {
		$yamls = Get-Item .\bin\debug\netcoreapp*\publish\*.yaml | Resolve-Path -Relative
		echo "gcloud beta app deploy --quiet --no-promote -v deploytest $yamls"
        gcloud beta app deploy --quiet --no-promote -v deploytest $yamls
        if ($LASTEXITCODE -eq 0) {
            break
        }
        if (++$deployCount -gt 2) {
            throw "gcloud app deploy failed with exit code $LASTEXITCODE"
        }
    }
    gcloud app describe | where {$_ -match 'defaultHostName:\s+(\S+)' }
    try {
        Run-CasperJs $testJs ("https://deploytest-dot-" + $matches[1])
        return
    } catch {
      # Work around issue 35673193.
        Run-CasperJs $testJs ("https://deploytest-dot-" + $matches[1].Replace('appspot.com', 'appspot-preview.com'))
    }
}


##############################################################################
#.SYNOPSIS
# Make a backup copy of a file, run the script, and restore the file.
#
#.PARAMETER Files
# A list of files to back up.
#
#.PARAMETER ScriptBlock
# The script to execute.
#
#.EXAMPLE
# Backup-File Program.cs { Build }
##############################################################################
function Backup-File(
    [string[]][Parameter(Mandatory=$true,ValueFromPipeline=$true)] $Files,
    [scriptblock][Parameter(Mandatory=$true)] $ScriptBlock)
{
    $fileMap = @{}
    try {
        foreach ($file in $files) {
            $tempCopy = [System.IO.Path]::GetTempFileName()
            Copy-Item -Force $file $tempCopy
            $fileMap[$file] = $tempCopy
        }
        . $ScriptBlock
    }
    finally {
        foreach ($file in $files) {
            Copy-Item -Force $fileMap[$file] $file
        }
    }
}

##############################################################################
#.SYNOPSIS
# Replace text in a text file.
#
#.PARAMETER Files
# A list of files to pack up.
#
#.PARAMETER Replacements.
# A hashtable.  Keys are the text to replace.  Values are replacements.
#
#.EXAMPLE
# Edit-TextFile Program.cs @{'YOUR-PROJECT-ID', $env:GOOGLE_PROJECT_ID}
##############################################################################
function Edit-TextFile(
    [string[]][Parameter(Mandatory=$true,ValueFromPipeline=$true)] $Files,
    [hashtable][Parameter(Mandatory=$true)] $Replacements)
{
    foreach ($file in $files) {
        $content = Get-Content $file | ForEach-Object {
            $line = $_
            foreach ($key in $Replacements.Keys) {
                $line = $line.Replace($key, $Replacements[$key])
            }
            $line
        }
        $content | Out-File -Force -Encoding UTF8 $file
    }
}

##############################################################################
#.SYNOPSIS
# Make a backup copy of a file, edit the file, run the script, and restore
# the file.
#
#.PARAMETER Files
# A list of files to back up.
#
#.PARAMETER Replacements.
# A hashtable.  Keys are the text to replace.  Values are replacements.
#
#.PARAMETER ScriptBlock
# The script to execute.
#
#.EXAMPLE
# BackupAndEdit-TextFile "QuickStart\Program.cs" `
#     @{"YOUR-PROJECT-ID" = $env:GOOGLE_PROJECT_ID} `
# {
#     Build-Solution
# }
##############################################################################
function BackupAndEdit-TextFile(
    [string[]][Parameter(Mandatory=$true,ValueFromPipeline=$true)] $Files,
    [hashtable][Parameter(Mandatory=$true)] $Replacements,
    [scriptblock][Parameter(Mandatory=$true)] $ScriptBlock)
{
    Backup-File $Files {
        Edit-TextFile $Files $Replacements
        . $ScriptBlock
    }.GetNewClosure()
}

##############################################################################
#.SYNOPSIS
# Find files that are utf16 encoded.
#
#.PARAMETER Path
# A list of paths or masks for files to check.
#
#.EXAMPLE
# Find-Utf16
##############################################################################
function Find-Utf16($Path=@('*.yaml', '*.cs', '*.xml')) {
    foreach ($file in (Get-ChildItem -Recurse -Path $Path)) {
        $bytes =  [System.IO.File]::ReadAllBytes($file.FullName)
        if ($bytes[0] -eq 255 -and $bytes[1] -eq 254) {
            $file
        }
    }
}

##############################################################################
#.SYNOPSIS
# Converts a file to utf8 encoding.
#
#.INPUT
# Files that are encoded some other way.
#
#.EXAMPLE
# Find-Utf16 | ConvertTo-Utf8
##############################################################################
filter ConvertTo-Utf8 {
    $lines = [System.IO.File]::ReadAllLines($_)
    [System.IO.File]::WriteAllLines($_, $lines)
}

##############################################################################
#.SYNOPSIS
# Given a path to a runTests.ps1 script, find the git timestamp of changes in
# the same directory.
##############################################################################
function Get-GitTimeStampForScript($script) {
    Push-Location
    try {
        Set-Location (Split-Path $script)
        $dateText = git log -n 1 --format=%cd --date=iso .
        $newestDate = @(($dateText | Get-Date).ToUniversalTime().ToString("o"))
        # Search for dependencies too.
        $references = dotnet list reference | Where-Object {Test-Path $_ }
        # Look up the timestamp for each dependency.
        foreach ($ref in $references) {
            $dateText = git log -n 1 --format=%cd --date=iso (Split-Path $ref)
            $newestDate += @(($dateText | Get-Date).ToUniversalTime().ToString("o"))
        }
        # Return a string combining all the timestamps in descending order.
        return ($newestDate | Sort-Object -Descending) -join "+"
    } finally {
        Pop-Location
    }
}

# Notice the year is incomplete.  We fill it in Add-Copyright below.
$copyrightTemplate = @"
Copyright 20 Google

Licensed under the Apache License, Version 2.0 (the "License"); you may not
use this file except in compliance with the License. You may obtain a copy of
the License at

http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
License for the specific language governing permissions and limitations under
the License.
"@


##############################
#.SYNOPSIS
# Detects whether the source file contains a copyright notice.
#
#.PARAMETER path
# The path the the source file to examine.
#
#.EXAMPLE
# Has-Copyright foo.cs
##############################
function Has-Copyright ($path)
{
    # Just search for all the letters in the same order.  Imperfect, but
    # quick and effective given the variety of whitespace and comment syntaxes
    # across programming languages.
    $haystack = Get-Content -Raw $path
    $needle = ($copyrightTemplate -replace '\s', '').ToCharArray()
    if ($haystack.Length -lt $needle.Length) {
        return $false
    }
    $ineedle = 0
    foreach ($hay in $haystack.ToCharArray()) {
        if ($needle[$ineedle] -eq $hay) {
            $ineedle += 1;
            if ($ineedle -eq $needle.Length) {
                return $true
            }
        }
    }
    return $false
}

##############################
#.SYNOPSIS
# Adds copyright notice to source files that lack a copyright notice.
#
#.DESCRIPTION
# Does not modify files that already have a copyright notice.
#
#.PARAMETER Files
# The files to examine.  When empty, recursively searches directory for
# files with extensins .cs, .cshtml, and .ps1.
#
#.EXAMPLE
# Add-Copyright
##############################
function Add-Copyright([string[]][Parameter(ValueFromPipeline=$true)] $Files)
{
    if (-not $Files)
    {
        $Files = '.cs', '.cshtml', '.ps1' | ForEach-Object {
            Get-ChildItem -Recurse "*$_"
        }
    }
    $year = (Get-Date -UFormat "%Y")
    $copyrightLines = $copyrightTemplate.Replace(
        "Copyright 20 Google", "Copyright (c) $year Google LLC.") `
        -split '\r\n|\r|\n'
    foreach ($path in $Files) {
        if (Has-Copyright $path) { continue }
        $dot = $path.LastIndexOf('.')
        $ext = $path.Substring($dot)
        $lineCommentPrefix = switch ($ext)
        {
            '.cs' { '//' }
            '.cshtml' { '//' }
            '.ps1' { '#'}
        }
        $tempPath = $path + ".tmp"
        $copyright = "$lineCommentPrefix " + (
            $copyrightLines -join "`n$lineCommentPrefix ")
        $header = if ('.cshtml' -eq $ext) {
            '@{', $copyright, '}', ''
        } else {
            $copyright, '' # Append an empty line too.
        }
        $header | Out-File -Encoding UTF8 $tempPath
        Get-Content $path | Out-File -Append -Encoding UTF8 $tempPath
        Move-Item -Force $tempPath $path
        "Add copyright to $path."
    }
}

<#
.SYNOPSIS
Generate a new random name that can be used to name GCP objects.

.PARAMETER Prefix
An optional prefix for the random name.

.PARAMETER Length
The number of random characters to be included in the name.

.PARAMETER CharSet
The set of characters from which the random name is generated.

.EXAMPLE
> New-RandomName
hpywzrkcjuebdgsqvatx
#>

function New-RandomName([string]$Prefix = '', [int]$Length = 20, 
    [string]$CharSet = 'abcdefghijklmnopqrstuvwxyz') 
{
    $randomChars = $CharSet.ToCharArray() | Get-Random -Count $Length
    $randomString = $randomChars -join ''
    return "$Prefix$randomString"
}
