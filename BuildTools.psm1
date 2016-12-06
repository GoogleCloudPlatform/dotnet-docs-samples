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
    When-Empty $Target $ArgList {Find-Files -Masks $Mask} | Resolve-Path -Relative
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
# Runs powershell scripts and prints a summary of successes and errors.
#
#.INPUTS
# Powershell scripts.  If empty, recursively searches directories for
# scripts with 'runtests' in their names.
#
#.EXAMPLE
# Run-Tests
##############################################################################
function Run-TestScripts($MaxParallelJobCount = 2)
{
    $scripts = When-Empty -ArgList ($input + $args) -ScriptBlock { Find-Files -Masks '*runtests*.ps1' } | Get-Item
    $rootDir = pwd
    # Keep running lists of successes and failures.
    # Array of strings: the relative path of the inner script.
    $successes = @()
    $failures = @()
    $timeOuts = @()
    $liveJobs = @()
    $i = 0
    do {
        if ($i -lt $scripts.Length) {
            $script = $scripts[$i++]
            $relativePath = Resolve-Path -Relative $script.FullName
            echo "Starting $relativePath..."
            $job = Start-Job -ArgumentList $relativePath, $script.Directory, `
                ('.\"{0}"' -f $script.Name) -Name $relativePath {
                echo ("-" * 79)
                echo ("Results for " + $args[0])
                echo ("-" * 79)
                Set-Location $args[1]
                Invoke-Expression $args[2]
                if ($LASTEXITCODE) {
                    throw "FAILED with exit code $LASTEXITCODE"
                }
            }
            $liveJobs += $job
            if ($liveJobs.Length -lt $MaxParallelJobCount) { continue }
        }
        if ($remoteJob = (Wait-Job $liveJobs -Timeout 300 -Any)) {
            $job = Get-Job $remoteJob.Name
            $liveJobs = $liveJobs -ne $job
            Receive-Job $job
            if ($job.State -eq 'Failed') {
                $failures += $job.Name
            } else {
                $successes += $job.Name
            }
            Remove-Job $job
        } else {
            $timeOuts += $liveJobs.Name
            Remove-Job $liveJobs
            break
        }
    } while ($liveJobs)
    # Print a final summary.
    echo ("=" * 79)
    $successCount = $successes.Count
    echo "$successCount SUCCEEDED"
    echo $successes
    $failureCount = $failures.Count
    echo "$failureCount FAILED"
    echo $failures
    $timeOutCount = $timeOuts.Count
    echo "$timeOutCount TIMED OUT"
    echo $timeOuts
    # Throw an exception to set ERRORLEVEL to 1 in the calling process.
    if ($failureCount) {
        throw "$failureCount FAILED"
    }
    if ($timeOutCount) {
        throw "$timeOutCount TIMED OUT"
    }
}

##############################################################################
#.SYNOPSIS
# Builds and runs .NET core web application in the current directory.
# Runs test using casperjs.
#
#.INPUTS
# Javascript test files to pass to casperjs.
##############################################################################
function BuildAndRun-CoreTest($TestJs = "test.js") {
    dnvm use 1.0.0-rc1-update1 -r clr
    dnu restore
    dnu build
    $webProcess = Start-Process dnx web -PassThru
    Try
    {
        Start-Sleep -Seconds 4  # Wait for web process to start up.
        casperjs $TestJs http://localhost:5000
    }
    Finally
    {
        Stop-Process $webProcess
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
        codeformatter.exe /rule:BraceNewLine /rule:ExplicitThis /rule-:ExplicitVisibility /rule:FieldNames /rule:FormatDocument /rule:ReadonlyFields /rule:UsingLocation /nocopyright $project
        if ($LASTEXITCODE) {
            $project.FullName
            throw "codeformatter failed with exit code $LASTEXITCODE."
        }
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
