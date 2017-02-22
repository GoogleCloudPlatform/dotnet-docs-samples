#Requires -Version 5
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
#
##############################################################################
#.SYNOPSIS
# Do some cleanup after done testing with the script: 
#   create-sql-instance-availability-group.ps1
#
#.DESCRIPTION
# The script create-sql-instance-availability-group.ps1 creates a two node
# availability group. After you are done testing with the script you may want
# to remove the nodes, networks and firewall rules. This script provides some
# sample commands. You may need to modify some parameters to match your
# configuration.
# 
# For more information see "Configuring SQL Server Availability Groups" at:
# https://cloud.google.com/compute/docs/instances/sql-server/configure-availability
#
#
#.NOTES
# AUTHOR: Anibal Santiago - @SQLThinker
##############################################################################


##### Cleanup - Run with caution as everything will be deleted #####
# All the commands are commented to make sure they don't run automatically

<#
$ErrorActionPreference = 'continue'

# Delete instances
Get-GceInstance -Zone "us-central1-f" | Where Name -EQ "dc-windows"   | Remove-GceInstance
Get-GceInstance -Zone "us-central1-f" | Where Name -EQ "cluster-sql1" | Remove-GceInstance
Get-GceInstance -Zone "us-central1-f" | Where Name -EQ "cluster-sql2" | Remove-GceInstance

# Delete the 3 subnetworks
Invoke-Command -ScriptBlock { gcloud compute networks subnets delete wsfcsubnet1 --quiet 2> $null }
Invoke-Command -ScriptBlock { gcloud compute networks subnets delete wsfcsubnet2 --quiet 2> $null }
Invoke-Command -ScriptBlock { gcloud compute networks subnets delete wsfcsubnet3 --quiet 2> $null }

# Delete the firewall rules, routes and custom network
Get-GceFirewall | Where-Object Network -Like "*wsfcnet" | Remove-GceFirewall
Get-GceRoute | Where-Object Network -Like "*wsfcnet" | Remove-GceRoute
Invoke-Command -ScriptBlock { gcloud compute networks delete wsfcnet --quiet 2> $null }

#>

