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

param([switch]$lint)

# Recursively retrieve all the files in a directory that match one of the
# masks.
function GetFiles($path = $null, [string[]]$masks = '*', $maxDepth = -1, $depth=0)
{
    foreach ($item in Get-ChildItem $path | Sort-Object -Property Mode,Name)
    {
        if ($masks | Where {$item -like $_})
        {
            $item
        }
        if ($maxDepth -ge 0 -and $depth -ge $maxDepth)
        {
            # We have reached the max depth.  Do not recurse.
        }
        elseif (Test-Path $item.FullName -PathType Container)
        {
            GetFiles $item.FullName $masks $maxDepth ($depth + 1)
        }
    }
}

function GetRootDirectory
{
    $Invocation = (Get-Variable MyInvocation -Scope 1).Value
    Split-Path $Invocation.MyCommand.Path
}

$rootDir = GetRootDirectory

function RunTests 
{
    # Keep running lists of successes and failures.
    # Array of strings: the relative path of the inner script.
    $successes = @()
    $failures = @()
    foreach ($testproj in $input) {
        Set-Location $testproj.Directory
        $relativePath = $testproj.FullName.Substring($rootDir.Length + 1, $testproj.FullName.Length - $rootDir.Length - 1)
        echo $relativePath
        # A script can fail two ways.
        # 1. Throw an exception.
        # 2. The last command it executed failed. 
        Try {
            MSTest /testcontainer:bin\debug\test.dll
            if ($LASTEXITCODE) {
                $failures += $relativePath
            } else {
                $successes += $relativePath
            }
        }
        Catch {
            echo  $_.Exception.Message
            $failures += $relativePath
        }
    }
    # Print a final summary.
    echo "==============================================================================="
    $successCount = $successes.Count
    echo "$successCount SUCCEEDED"
    echo $successes
    $failureCount = $failures.Count
    echo "$failureCount FAILED"
    echo $failures
    # Throw an exception to set ERRORLEVEL to 1 in the calling process.
    if ($failureCount) {
        throw "$failureCount FAILED"
    }
}

filter BuildSolution 
{
    cd $_.Directory
    nuget restore
    if ($LASTEXITCODE) 
    {
        throw "Nuget failed with error code $LASTEXITCODE"
    }
    msbuild /p:Configuration=Debug
    if ($LASTEXITCODE) 
    {
        throw "Msbuild failed with error code $LASTEXITCODE"
    }
}

function Format-Code
{
    GetFiles -masks '*.csproj' | ForEach-Object {
        codeformatter.exe /rule:BraceNewLine /rule:ExplicitThis /rule-:ExplicitVisibility /rule:FieldNames /rule:FormatDocument /rule:ReadonlyFields /rule:UsingLocation "/copyright:$(Join-Path $rootDir Copyright.txt)" $_.FullName
    }
}

filter RunLint 
{
    codeformatter.exe /rule:BraceNewLine /rule:ExplicitThis /rule-:ExplicitVisibility /rule:FieldNames /rule:FormatDocument /rule:ReadonlyFields /rule:UsingLocation /nocopyright $_.FullName
    if ($LASTEXITCODE) 
    {
        throw "codeformatter failed with exit code $LASTEXITCODE."
    }
    # If git reports a diff, codeformatter changed something, and that's bad.
    $diff = git diff
    if ($diff) 
    {
        $diff
        throw "Lint failed for $_"
    }
}

##############################################################################
# main
# Leave the user in the same directory as they started.
$originalDir = Get-Location
Try
{
    # First, lint everything.  If the lint fails, don't waste time running
    # tests.
    if ($lint) {
        GetFiles -masks '*.csproj' | RunLint
    }
    GetFiles -masks '*.sln' | BuildSolution
    Set-Location $originalDir
    GetFiles -masks 'test.csproj' | RunTests
}
Finally
{
    Set-Location $originalDir
}
