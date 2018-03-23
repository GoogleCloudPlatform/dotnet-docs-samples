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
# HOW TO USE THE FUNCTIONS IN THIS MODULE
##############################################################################
<#

PS ...> Import-Module .\BuildTools.psm1
WARNING: The names of some imported commands from the module 'BuildTools' include unapproved verbs that might make them less
discoverable. To find the commands with unapproved verbs, run the Import-Module command again with the Verbose parameter. For
 a list of approved verbs, type Get-Verb.
PS ...> cd .\aspnet\2-structured-data
PS ...\aspnet\2-structured-data> Run-TestScripts
...
0 SUCCEEDED
2 FAILED
.\runTestsWithDatastore.ps1
.\runTestsWithSql.ps1

# Oh no!  My tests failed.  I forgot to update the Web.Configs from my
# environment variables.
PS ...\aspnet\2-structured-data> Update-Config
.\Web.config is modified.  Overwrite? [Y]es, [N]o, Yes to [A]ll: a
C:\Users\Jeffrey Rennie\gitrepos\getting-started-dotnet\aspnet\2-structured-data\Web.config
C:\Users\Jeffrey Rennie\gitrepos\getting-started-dotnet\aspnet\2-structured-data\Views\Web.config

# And run the tests again.
PS ...\aspnet\2-structured-data> Run-TestScripts
...
2 SUCCEEDED
.\runTestsWithDatastore.ps1
.\runTestsWithSql.ps1
0 FAILED

# Yay!  They succeeded.
# I can also just run any test script directly:
PS ...\aspnet\2-structured-data> .\runTestsWithDatastore.ps1
...
PASS 4 tests executed in 10.671s, 4 passed, 0 failed.

# Or, I can run the test and leave IISExpress running to debug:
PS ...\aspnet\2-structured-data> Set-BookStore mysql
PS ...\aspnet\2-structured-data> Run-IISExpressTest -LeaveRunning

# Don't submit the changes to Web.configs.
PS ...\aspnet\2-structured-data> Unstage-Config
Unstaged changes after reset:
M	aspnet/2-structured-data/Views/Web.config
M	aspnet/2-structured-data/Web.config

# Submit my changes.
PS ...\aspnet\2-structured-data>git commit

# Clean up, cause I'm done.
PS ...\aspnet\2-structured-data> Revert-Config

#>

##############################################################################
#.SYNOPSIS
# Adds a setting to a Web.config configuration.
#
#.PARAMETER Config
# The root xml object from a Web.config or App.config file.
#
#.PARAMETER Key
# The name of the key to set.
#
#.PARAMETER Value
# The value to set.
#
#.EXAMPLE
# Add-Setting $config 'GoogleCloudSamples:ProjectId' $env:GoogleCloudSamples:ProjectId
##############################################################################
function Add-Setting($Config, [string]$Key, [string]$Value) {
    $x = Select-Xml -Xml $Config.Node -XPath "appSettings/add[@key='$Key']"
    if ($x) {
        $x.Node.value = $Value
    }
}

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
# Finds all the Web.config files in subdirectories.
#
##############################################################################
filter Get-Config ($Target, $ArgList, $Mask="Web.config") {
    $paths = When-Empty $Target $ArgList {Find-Files -Masks $Mask}
    if ($paths) {
        $paths | Resolve-Path -Relative
    }
}

##############################################################################
#.SYNOPSIS
# Updates Web.config files, pulling values from environment variables.
#
#.DESCRIPTION
# Asks the user before overwriting files that have already been modified.
# Don't forget to Revert-Config or Unstage-Config before 'git commit'ing!
#
#.PARAMETER Yes
# Never ask the user if they want to overwrite a modified Web.config.  Just
# overwrite it.
#
#.INPUTS
# Paths to Web.config.  If empty, recursively searches directories for
# Web.config files.
#
#.OUTPUTs
# Paths to Web.configs that this function modified.
#
#.EXAMPLE
# Update-Config
##############################################################################
filter Update-Config ([switch]$Yes) {
    $configs = Get-Config $_ $args
    foreach($configPath in $configs) {
        if (-not $Yes -and (git status -s $configPath)) {
            do {
                $reply = Read-Host "$configPath is modified.  Overwrite? [Y]es, [N]o, Yes to [A]ll"
            } until ("y", "n", "a" -contains $reply)
            if ("n" -eq $reply) { continue }
            if ("a" -eq $reply) { $Yes = $true }
        }
        $config = Select-Xml -Path $configPath -XPath configuration
        if (-not $config) {
            continue;
        }
        Add-Setting $config 'GoogleCloudSamples:BookStore' $env:GoogleCloudSamples:BookStore
        Add-Setting $config 'GoogleCloudSamples:ProjectId' $env:GoogleCloudSamples:ProjectId
        Add-Setting $config 'GoogleCloudSamples:BucketName' $env:GoogleCloudSamples:BucketName
        Add-Setting $config 'GoogleCloudSamples:AuthClientId' $env:GoogleCloudSamples:AuthClientId
        Add-Setting $config 'GoogleCloudSamples:AuthClientSecret' $env:GoogleCloudSamples:AuthClientSecret
        $connectionString = Select-Xml -Xml $config.Node -XPath "connectionStrings/add[@name='LocalMySqlServer']"
        if ($connectionString) {
            if ($env:GoogleCloudSamples:ConnectionString) {
                $connectionString.Node.connectionString = $env:GoogleCloudSamples:ConnectionString;
            } elseif ($env:Data:MySql:ConnectionString) {
                # TODO: Stop checking this old environment variable name when we've
                # updated all the scripts.
                $connectionString.Node.connectionString = $env:Data:MySql:ConnectionString;
            }
        }
        $config.Node.OwnerDocument.Save($config.Path);
        $config.Path
    }
}

##############################################################################
#.SYNOPSIS
# Updates Web.config files.  Sets the BookStore setting.
#
#.INPUTS
# Paths to Web.config.  If empty, recursively searches directories for
# Web.config files.
#
#.EXAMPLE
# Set-BookStore mysql
##############################################################################
filter Set-BookStore($BookStore) {
    $configs = Get-Config $_ $args
    foreach($configPath in $configs) {
        $config = Select-Xml -Path $configPath -XPath configuration
        Add-Setting $config 'GoogleCloudSamples:BookStore' $BookStore
        $config.Node.OwnerDocument.Save($config.Path)
    }
}

##############################################################################
#.SYNOPSIS
# Reverts Web.config files.
#
#.DESCRIPTION
# git must be in the current PATH.
#
#.INPUTS
# Paths to Web.config.  If empty, recursively searches directories for
# Web.config files.
##############################################################################
filter Revert-Config {
    $configs = Get-Config $_ $args
    $silent = git reset HEAD $configs
    git checkout -- $configs
}

##############################################################################
#.SYNOPSIS
# Unstages Web.config files.
#
#.DESCRIPTION
# git must be in the current PATH.
#
#.INPUTS
# Paths to Web.config.  If empty, recursively searches directories for
# Web.config files.
##############################################################################
filter Unstage-Config {
    $configs = Get-Config $_ $args
    git reset HEAD $configs
}

##############################################################################
#.SYNOPSIS
# Recursively find all the files that match a mask.
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
        $startDate = Get-Date
        $relativePath = Resolve-Path -Relative $script
        $jobState = 'Failed'
        Write-Output "$verb $relativePath..."
        $job = Start-Job -ArgumentList $relativePath, $script.Directory, `
            ('.\"{0}"' -f $script.Name) {
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
            foreach ($line in (Receive-Job $job)) {
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
        Remove-Job -Force $job
        $results[$jobState] += @($relativePath)
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
    Get-ChildItem -Recurse TestResults.xml | Rename-Item -NewName 01_sponge_log.xml
    # Retry the failures once.
    $failed = $results['Failed']
    if ($failed) {
        $results['Failed'] = @()
        Run-TestScriptsOnce ($failed | Get-Item) $TimeoutSeconds `
            'Retrying' $results
        # Rename all the test logs to a name Sponge will find.
        Get-ChildItem -Recurse TestResults.xml | Rename-Item -NewName 02_sponge_log.xml
    }

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
    $projects = When-Empty $_ $args { Find-Files -Masks *.csproj }
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
    }
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
    # Applicationhost.config expects the environment variable
    # DOTNET_DOCS_SAMPLES to point to the same directory containing
    # applicationhost.config.
    $env:DOTNET_DOCS_SAMPLES = (Get-Item $ApplicationhostConfig).DirectoryName
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
        casperjs $TestJs http://localhost:$port
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
function Run-Kestrel([Parameter(mandatory=$true)][string]$url) {
    Start-Job -ArgumentList (Get-Location), $url -ScriptBlock {
        Set-Location $args[0]
        $env:ASPNETCORE_URLS = $args[1]
        dotnet run
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
    $job = Run-Kestrel($url)
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
function Run-CasperJs($TestJs='test.js', $Url, [switch]$v11 = $false) {
    $sleepSeconds = 2
    for ($tryCount = 0; $tryCount -lt 5; $tryCount++) {
        Start-Sleep -Seconds $sleepSeconds  # Wait for web process to start up.
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
        $sleepSeconds *= 2
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
# Migrate the database.
#
#.DESCRIPTION
# Must be called from a directory with a .csproj and Web.config.
#
#.PARAMETER DllName
# The name of the built binary.  Defaults to the current directory name.
##############################################################################
function Migrate-Database($DllName = '') {
    if (!$DllName) {
        # Default to the name of the current directory + .dll
        # For example, if the current directory is 3-binary-data, then the
        # dll name will be 3-binary-data.dll.
        $DllName =  (get-item .).Name + ".dll"
    }
    # Migrate.exe cannot be run in place.  It must be copied to the bin directory
    # and run from there.
    cp (Join-Path (UpFind-File packages) EntityFramework.*\tools\migrate.exe) bin\.
    $originalDir = pwd
    Try {
        cd bin
        .\migrate.exe $dllName /startupConfigurationFile="..\Web.config"
        if ($LASTEXITCODE) {
            throw "migrate.exe failed with error code $LASTEXITCODE"
        }
    }
    Finally {
        cd $originalDir
    }
}

##############################################################################
#.SYNOPSIS
# Calls nuget update on all the packages that match the mask.
#
#.PARAMETER Mask
# Which packages should be updated?
#
#.INPUTS
# Paths to .sln files.  If empty, recursively searches directories for
# *.sln files.
#
#.EXAMPLE
# Update-Packages Google.*
##############################################################################
filter Update-Packages ([string] $Mask) {
    $solutions = When-Empty $_ $args { Find-Files -Masks *.sln }
    foreach ($solution in $solutions) {
        # Nuget refuses to update without calling restore first.
        nuget restore $solution
        # Assume all packages.configs in the same directory, or a subdirectory
        # as the solution are for projects in the solution.
        $packageConfigs = Find-Files (Get-Item $solution).Directory -Masks packages.config
        # Inspect each packages.config and find matching Ids.
        $packageIds = $packageConfigs `
            | ForEach-Object {(Select-Xml -Path $_ -XPath packages/package).Node.Id} `
            | Where {$_ -like $Mask}
        # Calling nuget update with no packageIds means update *all* packages,
        # and that's definitely not what we want.
        if ($packageIds) {
            nuget update -Prerelease $solution ($packageIds | ForEach-Object {"-Id", $_})
        }
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

