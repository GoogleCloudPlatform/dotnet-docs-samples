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
# Copies files to or from Google Cloud Storage.
#
#.DESCRIPTION
#
# Requires the Google Cloud SDK be installed.
# 
# Google Cloud Storage paths look like:
# gs://bucket/a/b/c.txt
#
# Does not support wildcards like * or ?.
# Does not support paths with . or ..
#
#.PARAMETER SourcePath
# The file or directory to copy.
#
#.PARAMETER DestPath
# The location to copy to.
#
#.PARAMETER Force
# Copy over existing files.
#
#.PARAMETER Recurse
# Recursively copy the files in directories.
#
#.OUTPUTS
# The newly created files.
##############################################################################
param(
    [string][Parameter(Mandatory=$true)][ValidateNotNullOrEmpty()]$SourcePath, 
    [string][Parameter(Mandatory=$true)][ValidateNotNullOrEmpty()] $DestPath,
    [switch] $Force, [switch] $Recurse)

# Google Cloud Storage has no concept of folder.  So to simulate having a
# folder named foo, we create an empty file name foo_$folder$.  Many tools
# use this convention.
$SCRIPT:FOLDER_SUFFIX = '_$folder$'

function Make-FolderName([string] $Path) {
    if ($Path.EndsWith($SCRIPT:FOLDER_SUFFIX)) {
        return $Path
    }
    if ($Path.EndsWith('/') -or $Path.EndsWith('\')) {
        $Path = $Path.Substring(0, $Path.Length - 1);
    }
    return "$Path$SCRIPT:FOLDER_SUFFIX"
}


##############################################################################
#.SYNOPSIS
# Splits a Cloud Storage path into its bucket name and object name.
#
#.PARAMETER Path
# A Google Cloud Storage path, or not.  Slashes can be forward or backward.
#
#.OUTPUTS
# If Path is a valid Cloud Storage path, outputs two items:
#   bucket name
#   object name
# Otherwise, outputs nothing.
##############################################################################
function Split-GcsPath([string] $Path) {
    $Path = $Path.replace('\', '/')
    if ($Path -match '^[gG][sS]://([^/]+)/(.*)') {
        $matches[1], $matches[2]
    }
}

##############################################################################
#.SYNOPSIS
# Tests whether a Cloud Storage object with the given object name exists.
#
#.PARAMETER Bucket
# The name of the Google Cloud Storage bucket.
#
#.PARAMETER ObjectName
# The name of the Google Cloud Storage object.
#
#.OUTPUTS
# True or False
##############################################################################
function Test-GcsObject([string] $Bucket, [string] $ObjectName) {
    try { 
        $obj = Get-GcsObject -Bucket $Bucket -ObjectName $ObjectName
        return $obj
    } catch {
        if ($_.Exception.HttpStatusCode -eq "NotFound") {
            return $False
        }
        throw
    }
}

##############################################################################
#.SYNOPSIS
# Appends a slash to the path if it doesn't already end in a slash.
#
#.PARAMETER Path
# The path to append.
#
#.PARAMETER Slash
# The slash character to append.
#
#.OUTPUTS
# A path that always ends with a Slash.
##############################################################################
function Append-Slash([string] $Path, [string]$Slash = '\') {
    if ($Path.EndsWith($Slash)) { $Path } else { "$Path$Slash" }
}

##############################################################################
#.SYNOPSIS
# Creates a directory on cloud storage.
#
#.DESCRIPTION
# There is no concept of a directory in Google Cloud Storage.  To simulate a
# directory, we create a zero-length file that ends in a /.
#
# First checks to see if a directory by the same name already exists, and
# reports no error if it does.
#
#.PARAMETER Path
# The directory name.
#
#.PARAMETER Bucket
# The Cloud Storage bucket.
#
#.OUTPUTS
# The Cloud Storage object created.
##############################################################################
function Make-GcsDirectory([string] $Path, [string] $Bucket, [switch] $Force) {
    $folder = Make-FolderName $Path
    if (-not $Force) {
        $existing = Test-GcsObject -Bucket $Bucket -ObjectName $folder
        if ($existing -and $existing.Size -eq 0) {
            return # Directory already exists.
        }
    }
    New-GcsObject -Bucket $Bucket -ObjectName $folder `
        -Contents "" -Force:$Force
}

##############################################################################
#.SYNOPSIS
# Uploads an item from the local file system to Google Cloud Storage.
#
#.PARAMETER SourcePath
# The local file system path to upload.
#
#.PARAMETER DestPath
# The prefix of the Cloud Storage object name to be created.
#
#.PARAMETER Bucket
# The Cloud Storage bucket to upload to.
#
#.OUTPUTS
# The list of Cloud Storage objects created.
##############################################################################
function Upload-Item([string] $SourcePath, [string] $DestPath,
        [string] $Bucket) {
    # Is the source path a file or a directory?  Does the
    # destination directory already exist?  It takes a lot of logic
    # to match the behavior of cp and copy.
    $DestDir = Append-Slash $DestPath '/'
    $DestFolder = Make-FolderName $DestPath
    if (Test-Path -Path $SourcePath -PathType Leaf) {
        # It's a file.
        if (Test-GcsObject $Bucket $DestFolder) {
            # Copying a single file to a directory.
            New-GcsObject -Bucket $Bucket `
                -ObjectName "$DestDir$(Split-Path $SourcePath -Leaf)" `
                -File $SourcePath -Force:$Force
        } else {
            # Copying a single file to a file name.
            New-GcsObject -Bucket $Bucket -ObjectName $DestPath `
                -File $SourcePath -Force:$Force
        }
    } elseif (Test-Path -Path $SourcePath -PathType Container) {
        # It's a directory.
        if (-not $Recurse) {
            throw [System.IO.FileNotFoundException] `
                "Use the -Recurse flag to copy directories."
        }
        if (Test-GcsObject $Bucket $DestFolder) {
            # Copying a directory to an existing directory.
            $DestDir = "$DestDir$(Split-Path $SourcePath -Leaf)/"
        }
        Make-GcsDirectory -Bucket $Bucket -Path $DestDir -Force $Force
        Upload-Dir $SourcePath $DestDir $Bucket
    } else {
        throw [System.IO.FileNotFoundException] `
        "$SourcePath does not exist."
    }
}

##############################################################################
#.SYNOPSIS
# Uploads a directory local file system to Google Cloud Storage.
#
#.PARAMETER SourcePath
# The local file system directory to upload.
#
#.PARAMETER DestDir
# The prefix of the Cloud Storage object name to be created.  Must end in /.
#
#.PARAMETER Bucket
# The Cloud Storage bucket to upload to.
#
#.OUTPUTS
# The list of Cloud Storage objects created.
##############################################################################
function Upload-Dir([string] $SourcePath, [string] $DestDir,
        [string] $Bucket) {
    $sourceDir = Append-Slash $SourcePath '\'
    $items = Get-ChildItem $sourceDir | Sort-Object -Property Mode,Name
    foreach ($item in $items) {
        if (Test-Path -Path $item.FullName -PathType Container) {
            Make-GcsDirectory -Bucket $Bucket -Path "$DestDir$($item.Name)/" `
                -Force $Force
            Upload-Dir "$sourceDir$($item.Name)" "$DestDir$($item.Name)/" `
                $Bucket
        } else {
            New-GcsObject -Bucket $Bucket -ObjectName "$DestDir$($item.Name)" `
                -File $item.FullName -Force:$Force
        }
    }
}

##############################################################################
#.SYNOPSIS
# Downloads an object from Google Cloud Storage to the local file system.
#
#.PARAMETER SourcePath
# The prefix of the Cloud Storage object name to be downloaded.
#
#.PARAMETER DestPath
# The local file system path to create.
#
#.PARAMETER Bucket
# The Cloud Storage bucket to download from.
#
#.OUTPUTS
# The list of local files created.
##############################################################################
function Download-Object([string] $SourcePath, [string] $DestPath,
        [string] $Bucket, [switch] $ShowProgress) {
    $outFile = if (Test-Path -Path $DestPath -PathType Container) {
        Join-Path $DestPath (Split-Path $SourcePath -Leaf)
    } else {
        $DestPath
    }
    if (-not $SourcePath.EndsWith('/') `
        -and (Test-GcsObject $Bucket $SourcePath)) {
        # Source path is a simple file.
        Read-GcsObject -Bucket $Bucket -ObjectName $SourcePath `
            -OutFile ([System.IO.Path]::GetFullPath($outFile)) -Force:$Force
        Get-Item $outFile
    } else {
        # Source is a directory.
        if (-not $Recurse) {
            throw [System.IO.FileNotFoundException] `
                "Use the -Recurse flag to copy directories."
        }
        if (-not (Test-Path -Path $outFile -PathType Container)) {
            New-Item -Path $outFile -ItemType Directory
        }
        Download-Dir $SourcePath $outFile $Bucket -ShowProgress:$ShowProgress
    }
}


##############################################################################
#.SYNOPSIS
# Downloads a directory from Google Cloud Storage to the local file system.
#
#.PARAMETER SourcePath
# The prefix of the Cloud Storage object name to be downloaded.
#
#.PARAMETER DestPath
# The local file system path to create.
#
#.PARAMETER Bucket
# The Cloud Storage bucket to download from.
#
#.OUTPUTS
# The list of local files created.
##############################################################################
function Download-Dir([string] $SourcePath, [string] $DestPath, 
        [string] $Bucket, [switch]$ShowProgress) {
    $sourceDir = Append-Slash $SourcePath '/'
    if ($ShowProgress) {
        $progress = 0
        Write-Progress -Activity "Downloading objects" `
            -CurrentOperation "Finding objects" -PercentComplete 0
    }
    $objects = Find-GcsObject -Bucket $Bucket -Prefix $sourceDir
    foreach ($object in $objects) {
        if ($ShowProgress) {
            Write-Progress -Activity "Downloading objects" `
                -CurrentOperation "Downloading $($object.Name)" `
                -PercentComplete (++$progress * 100 / $objects.Length) `
                -Completed:($progress -eq $objects.Length)
        }
        $relPath = $object.Name.Substring(
            $sourceDir.Length, $object.Name.Length - $sourceDir.Length)
        if ($relPath.Length -eq 0) {
            continue
        }
        $destFilePath = (Join-Path $DestPath $relPath)
        $DestDirPath = (Split-Path -Path $destFilePath)
        if ($relPath.EndsWith($SCRIPT:FOLDER_SUFFIX)) {
            # It's a directory
            New-Item -ItemType Directory -Force -Path $destFilePath.Substring(
                0, $destFilePath.Length - $SCRIPT:FOLDER_SUFFIX.Length)
        } else {
            # It's a file
            $DestDir = New-Item -ItemType Directory -Force -Path $DestDirPath
            Read-GcsObject -Bucket $Bucket -ObjectName $object.Name `
                -OutFile ([System.IO.Path]::GetFullPath($destFilePath)) `
                -Force:$Force
            Get-Item $destFilePath
        }
    }
}

function Main {
    $destBucketAndPath = Split-GcsPath $DestPath
    $sourceBucketAndPath = Split-GcsPath $SourcePath
    if ($sourceBucketAndPath) {
        $sourceBucket, $sourcePath = $sourceBucketAndPath
        if ($destBucketAndPath) {
            # Copying from Cloud Storage to Cloud Storage.
            # Download to a temp directory, then upload.
            $sourceName = Split-Path $SourcePath -Leaf
            $tempParentPath = [System.IO.Path]::Combine(
                $env:TEMP, 'GcsCopies', (Get-Random))
            $tempParentDir = New-Item -ItemType Directory -Path $tempParentPath
            $tempPath = [System.IO.Path]::Combine($tempParentPath, $sourceName)
            $localFiles = Download-Object $sourcePath $tempPath $sourceBucket `
                -ShowProgress
            Upload-Item $tempPath $destBucketAndPath[1] $destBucketAndPath[0]
        } else {
            Download-Object $sourcePath $DestPath $sourceBucket
        }
    } else {
        if ($destBucketAndPath) {
            Upload-Item $SourcePath $destBucketAndPath[1] $destBucketAndPath[0]
        } else {
            # Both paths are local.  Let the local file system do it.
            Copy-Item -Path $SourcePath -Destination $DestPath -Force:$Force `
                -Recurse:$Recurse
        }
    }        
}

if (Get-Command Find-GcsObject) {
    # Synchronize the powershell current working directory and the .NET current
    # working directory.
    [System.IO.Directory]::SetCurrentDirectory((Get-Location).Path)
    Main
} else {
    Write-Warning "Requires the Google Cloud SDK.  Download it from:
https://cloud.google.com/sdk"
}


