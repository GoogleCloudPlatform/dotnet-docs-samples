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
# List all the resources visible to Google Cloud Tools for PowerShell.
#
#.PARAMETER $ProjectId
# List all the resources for this project id.
# 
#.DESCRIPTION
# This doesn't list everything.  For example, it tells you nothing about logs
# or datastore.  As additional commands are added to
# Google Cloud Tools for PowerShell, they'll be added here.
#
#.OUTPUTs
# A formatted list of Google Cloud resources.
##############################################################################
Param([string] $ProjectId)


##############################################################################
#.SYNOPSIS
# Get the project id that Google Cloud commands will use by default.
#
#.OUTPUTs
# The current project id.
##############################################################################

function Get-GcProject {
    $stderr = [System.IO.Path]::GetTempFileName()
    $lines = gcloud config list project 2> $stderr
    if (0 -ne $LASTEXITCODE) {
        Write-Error (Get-Content $stderr)
    }
    foreach ($line in $lines) {
        $subs = $line.Split("=", 2)
        if ($subs.Length -eq 2) {
            $subs[1].Trim()
        }
    }
}

##############################################################################
#.SYNOPSIS
# Set the project id that Google Cloud commands will use by default.
#
#.PARAMETER $ProjectId
# The new project id to use by default.
##############################################################################
function Set-GcProject([string]$ProjectId) {
    $stderr = [System.IO.Path]::GetTempFileName()
    gcloud config set project $ProjectID
}
    
##############################################################################
#.SYNOPSIS
# List all the Google Cloud projects that you have permission to see.
#
#.OUTPUTs
# A list of Google Cloud projects.
##############################################################################
function Find-GcProject {
    gcloud projects list
}


##############################################################################
#.SYNOPSIS
# List all the resources visible to Google Cloud Tools for PowerShell.
#
#.PARAMETER $ProjectId
# List all the resources for this project id.
# 
#.DESCRIPTION
# This doesn't list everything.  For example, it tells you nothing about logs
# or datastore.  As additional commands are added to
# Google Cloud Tools for PowerShell, they'll be added here.
#
#.OUTPUTs
# A list of Google Cloud resources.
##############################################################################
function Find-GcResource([string]$ProjectId) {
    if (-not $ProjectId) {
        $ProjectId = Get-GcProject
    }
    # Google Cloud Storage
    Ignore-ApiNotEnabled {
        $buckets = Get-GcsBucket -Project $ProjectId
        $buckets
        foreach ($bucket in $buckets) {
          Get-GcsObject -Bucket $bucket
        }
    }

    # Google Compute Engine
    Ignore-ApiNotEnabled {
        Get-GceAddress -Project $ProjectId
        Get-GceBackendService -Project $ProjectId
        Get-GceDisk -Project $ProjectId
        Get-GceFirewall -Project $ProjectId
        Get-GceForwardingRule -Project $ProjectId
        Get-GceHealthCheck -Project $ProjectId
        Get-GceInstance -Project $ProjectId
        Get-GceInstanceTemplate -Project $ProjectId
        Get-GceManagedInstanceGroup -Project $ProjectId
        Get-GceNetwork -Project $ProjectId
        Get-GceRoute -Project $ProjectId
        Get-GceSnapshot -Project $ProjectId
        Get-GceTargetPool -Project $ProjectId
        Get-GceTargetProxy -Project $ProjectId
        Get-GceUrlMap -Project $ProjectId
    }

    # Google Cloud Sql
    Ignore-ApiNotEnabled {
        Get-GcSqlInstance -Project $ProjectId
    }

    # Google Cloud Dns
    Ignore-ApiNotEnabled {
        $zones = Get-GcdManagedZone -Project $ProjectId
        foreach ($zone in $zones) {
            Get-GcdResourceRecordSet $zone -Project $ProjectId
        }
    }
}

##############################################################################
#.SYNOPSIS
# Ignores accessNotConfigured exceptions.
#
#.DESCRIPTION
# If you haven't enabled cloud dns in your project, then there are no cloud
# dns resources to report.  No need to report an error.
##############################################################################
function Ignore-ApiNotEnabled($ScriptBlock) {
    try {
        . $ScriptBlock
    }
    catch {
        if (-not $_.Exception.Message.Contains("accessNotConfigured")) {
            throw
        }
    }
}


##############################################################################
#.SYNOPSIS
# Formats Google Cloud Resources in a pretty table.
#
#.INPUTS
# Google Cloud Resource objects.
#
#.OUTPUTS
# A pretty text table.
##############################################################################
function Format-GcResource {
    $input | Format-Table -GroupBy Kind -Property Size, SizeGb, DiskSizeGb, Name
}

Find-GcResource $ProjectId | Format-GcResource