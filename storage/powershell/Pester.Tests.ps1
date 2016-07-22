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
$here = Split-Path -Parent $MyInvocation.MyCommand.Path
$sut = (Split-Path -Leaf $MyInvocation.MyCommand.Path) -replace '\.Tests\.', '.'
. "$here\$sut"


##############################################################################
#.SYNOPSIS
# Cleans out Google Cloud Storage directory.
##############################################################################
function Clear-GcsTestDir([string]$TestDir = 'testdata') {
    $objects = Find-GcsObject -Bucket $env:GOOGLE_BUCKET -Prefix $TestDir
    Write-Progress -Activity "Removing old objects" `
        -CurrentOperation "Finding objects" -PercentComplete 0
    foreach ($object in $objects) {
        Write-Progress -Activity "Removing old objects" `
            -CurrentOperation "Removing $($object.Name)" `
            -PercentComplete (++$progress * 100 / $objects.Length) `
            -Completed:($progress -eq $objects.Length)
        $ignore = Remove-GcsObject -Bucket $env:GOOGLE_BUCKET `
            -ObjectName $object.Name
    }
}

##############################################################################
#.SYNOPSIS
# Copies local testdata/ directory to Google Cloud Storage.
#
#.PARAMETER PassThru
# Return the Google Cloud Storage objects that were created.
#
#.OUTPUTS
# Nothing, or the Google Cloud Storage objects that were created.
##############################################################################
function Upload-Testdata([switch]$PassThru) {
    $output = .\Copy-GcsObject.ps1 testdata gs://$env:GOOGLE_BUCKET/testdata `
        -Recurse
    if ($PassThru) {
        $output
    }
}


# Pester cannot compare arrays.  So, we have to join them into strings
# and compare strings.
function Groom-Expected($expected) {
    $groomedLines = $expected.Trim().Split("`n") | ForEach-Object { $_.Trim() } | Sort-Object
    [string]::Join("`n", $groomedLines)
}

function Join-Output {
    ($input | ForEach-Object { $_ }) -join "`n"
}

Describe "Uploads" {
    BeforeEach {
        Clear-GcsTestDir
    }

    It "uploads a new directory." {
        (Upload-Testdata -PassThru).Name | Sort-Object `
        | Join-Output | Should Be (Groom-Expected "testdata_`$folder`$
                testdata/hello.txt
                testdata/a_`$folder`$
                testdata/a/b_`$folder`$
                testdata/a/b/c.txt
                testdata/a/empty_`$folder`$")
    }

    It "uploads a single file to a directory." {
        Upload-Testdata
        (.\Copy-GcsObject.ps1 testdata\hello.txt `
            gs://$env:GOOGLE_BUCKET/testdata/a `
            ).Name | Should Be "testdata/a/hello.txt"
    }

    It "uploads a single file to a file name." {
        (.\Copy-GcsObject.ps1 testdata/hello.txt `
            gs://$env:GOOGLE_BUCKET/testdata/a/b/bye.txt `
            ).Name | Should Be "testdata/a/b/bye.txt"
    }

    It "uploads a directory into an existing directory structure." {
        Upload-Testdata
        (.\Copy-GcsObject.ps1 testdata/a/b `
            gs://$env:GOOGLE_BUCKET/testdata/ `
            -Recurse).Name | Sort-Object | Join-Output | Should Be (Groom-Expected `
            "testdata/b_`$folder`$
            testdata/b/c.txt")
    }
}

Describe "Downloads" {
    BeforeEach {
        Clear-GcsTestDir
        Upload-Testdata
        $tempPath = [System.IO.Path]::GetFullPath(
            [System.IO.Path]::Combine($env:TEMP, 'Pester.Test', (Get-Random)))
    }

    It "downloads the testdata directory." {        
        .\Copy-GcsObject.ps1 gs://$env:GOOGLE_BUCKET/testdata $tempPath -Recurse `
            | Sort-Object -Property FullName | Join-Output | Should Be (
                Groom-Expected "$tempPath
                $tempPath\a
                $tempPath\a\b
                $tempPath\a\b\c.txt
                $tempPath\a\empty
                $tempPath\hello.txt")
    }

    It "downloads the testdata directory to an existing directory." {        
        New-Item -ItemType Directory -Path $tempPath
        .\Copy-GcsObject.ps1 gs://$env:GOOGLE_BUCKET/testdata $tempPath -Recurse `
            | Sort-Object -Property FullName | Join-Output | Should Be (
                Groom-Expected "$tempPath\testdata
                $tempPath\testdata\a
                $tempPath\testdata\a\b
                $tempPath\testdata\a\b\c.txt
                $tempPath\testdata\a\empty
                $tempPath\testdata\hello.txt")
    }

    It "downloads hello.txt to a file." {        
        .\Copy-GcsObject.ps1 gs://$env:GOOGLE_BUCKET/testdata/hello.txt $tempPath `
            | Should Be "$tempPath"
    }

    It "downloads hello.txt to a directory." {
        New-Item -ItemType Directory -Path $tempPath
        .\Copy-GcsObject.ps1 gs://$env:GOOGLE_BUCKET/testdata/hello.txt $tempPath `
            | Should Be "$tempPath\hello.txt"
    }
}

Describe "Copies" {
    BeforeEach {
        Clear-GcsTestDir
        Upload-Testdata
    }

    It "copies a whole directory." {        
        (.\Copy-GcsObject.ps1 gs://$env:GOOGLE_BUCKET/testdata `
            gs://$env:GOOGLE_BUCKET/testdata2 -Recurse).Name `
            | Sort-Object | Join-Output | Should Be (
                Groom-Expected "testdata2_`$folder`$
                testdata2/hello.txt
                testdata2/a_`$folder`$
                testdata2/a/b_`$folder`$
                testdata2/a/b/c.txt
                testdata2/a/empty_`$folder`$")

        # Now that testdata2 exists, the same copy statement should copy
        # to an inner directory
        (.\Copy-GcsObject.ps1 gs://$env:GOOGLE_BUCKET/testdata `
            gs://$env:GOOGLE_BUCKET/testdata2 -Recurse).Name `
            | Sort-Object | Join-Output | Should Be (
                Groom-Expected "testdata2/testdata_`$folder`$
                testdata2/testdata/hello.txt
                testdata2/testdata/a_`$folder`$
                testdata2/testdata/a/b_`$folder`$
                testdata2/testdata/a/b/c.txt
                testdata2/testdata/a/empty_`$folder`$")
    }

    It "copies one file." {        
        (.\Copy-GcsObject.ps1 gs://$env:GOOGLE_BUCKET/testdata/hello.txt `
            gs://$env:GOOGLE_BUCKET/testdata/bye.txt).Name `
            | Should Be "testdata/bye.txt"
    }

    It "copies one file to an existing directory." {        
        (.\Copy-GcsObject.ps1 gs://$env:GOOGLE_BUCKET/testdata/hello.txt `
            gs://$env:GOOGLE_BUCKET/testdata/a).Name `
            | Should Be "testdata/a/hello.txt"
    }


}

