# Copyright(c) 2020 Google Inc.
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
# Tests for the create-sql-instance-availabilit y-group.ps1 script
# Requirements: 
#  The Pester module must be installed in the computer (Install-Module Pester)
#
######################################################################################
# IMPORTANT: 
# Before you test the scripts make sure you understand the pre-requisites to create
# a SQL Server Availability Group:
#
#   1) A Windows AD Domain and Domain Controller
#   2) A VPC network with two subnets, one for each SQL Server instance
#
# For automated testing we need to assign a password to the variable $domain_pwd,
# which is the password for an account that can add computers to the domain. This
# can be done by modifying the file parameters-config.ps1 or assigning a value to
# the $domain_pwd variable. We assume is the (domain\Admistrator) but it can be
# changed in the "parameters-config.ps1" file.
#
# Take a look at "create-sql-instance-availability-group-prerequisites.ps1" for an
# example of creating the pre-requisites. Also check "parameters-config.ps1" for the 
# default parameters.
######################################################################################

$ErrorActionPreference = 'Stop'
Set-Location $PSScriptRoot

# Find the path of this script
$path = Split-Path -Parent $MyInvocation.MyCommand.Path


# Generate a random password for the domain administrator
# If using an existing AD Domain you will need to change this instruction
$domain_pwd = -join(33..122 | %{[char]$_} | Get-Random -C 36)

# Run the PowerShell script that creates the pre-requisites
# If using an existing AD Domain you can comment this section
. "$path\create-sql-instance-availability-group-prerequisites.ps1"

# Run the PowerShell script that creates the Availability Group
. "$path\create-sql-instance-availability-group.ps1"


## Run tests to verify that the PowerShell script ran succesfully
Write-Host "$(Get-Date) Testing that Availability Group was created succesfully"

## Validate that the Availability Group was created
Describe "Availability-Group-Created" {
  It "Availability Group has two nodes that are synchronized" {
    
    $session_options = New-PSSessionOption -SkipCACheck -SkipCNCheck `
      -SkipRevocationCheck

    # Use SMO to query the DatabaseReplicaStates
    $results = Invoke-Command -ComputerName $ip_address1 -UseSSL `
      -Credential $cred -SessionOption $session_options `
      -ScriptBlock {
      
      param($name_ag)

      [System.Reflection.Assembly]::LoadWithPartialName("Microsoft.SqlServer.Smo") | 
        Out-Null
      $sql_server = New-Object `
        -TypeName Microsoft.SqlServer.Management.Smo.Server `
        -ArgumentList 'localhost'
      
      $sql_server.AvailabilityGroups[$($name_ag)].DatabaseReplicaStates | 
        Select-Object `
          AvailabilityReplicaServerName, `
          AvailabilityDatabaseName, `
          SynchronizationState
    } -ArgumentList $name_ag

    # TEST: Should have two nodes and they should be in a "Synchronized" state
    [int]$results.Count | Should BeExactly 2
    [string]$results[0].SynchronizationState | Should Match "Synchronized"
    [string]$results[1].SynchronizationState | Should Match "Synchronized"
  }
}


## Validate that the Availability Group can switch nodes
Describe "Availability-Group-can-switch-nodes" {
  It "Availability Group can switch nodes" {

    # Connect to Listener and return @@SERVERNAME. Should match Node 1.
    $results = Invoke-Command -ComputerName $ip_address1 -UseSSL `
      -Credential $cred -SessionOption $session_options `
      -ScriptBlock { 
      param($name_ag_listener)

      Import-Module SQLPS -DisableNameChecking

      Invoke-Sqlcmd `
        -Query "SELECT ServerName = @@SERVERNAME" `
        -ServerInstance $name_ag_listener
    } -ArgumentList $name_ag_listener

    # TEST: The @@SERVERNAME should be the name of Node 1
    [string]$results[0].ServerName | Should Match $node1

    # Switch to Node 2
    Invoke-Command -ComputerName $ip_address2 -UseSSL `
      -Credential $cred -SessionOption $session_options `
      -ScriptBlock {
      param($node2, $name_ag)

      Switch-SqlAvailabilityGroup `
        -Path "SQLSERVER:\SQL\$($node2)\DEFAULT\AvailabilityGroups\$($name_ag)"

        Start-Sleep 15
    } -ArgumentList $node2, $name_ag


    # Connect to Listener and return @@SERVERNAME. Should match Node 2.
    $results = Invoke-Command -ComputerName $ip_address2 -UseSSL `
      -Credential $cred -SessionOption $session_options `
      -ScriptBlock {
      param($name_ag_listener)

      Invoke-Sqlcmd `
        -Query "SELECT ServerName = @@SERVERNAME" `
        -ServerInstance $name_ag_listener
    } -ArgumentList $name_ag_listener

    # TEST: The @@SERVERNAME should be the name of Node 2
    [string]$results[0].ServerName | Should Match $node2

    # Switch to Node 1 again
    Invoke-Command -ComputerName $ip_address1 -UseSSL `
      -Credential $cred -SessionOption $session_options `
      -ScriptBlock {
      param($node1, $name_ag)

      Switch-SqlAvailabilityGroup `
        -Path "SQLSERVER:\SQL\$($node1)\DEFAULT\AvailabilityGroups\$($name_ag)"
    } -ArgumentList $node1, $name_ag
  }
}
