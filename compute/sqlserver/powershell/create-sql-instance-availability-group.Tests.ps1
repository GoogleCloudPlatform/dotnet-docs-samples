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

# Tests for the create-sql-instance-availabilit y-group.ps1 script
# Requirements: The Pester module must be installed in the computer
# Install-Module Pester

# Find the name of the source PowerShell script that we want to test
$path = Split-Path -Parent $MyInvocation.MyCommand.Path
$src_file = (Split-Path -Leaf $MyInvocation.MyCommand.Path).Replace(".Tests.", ".")

$ErrorActionPreference = 'Stop'
Set-Location $PSScriptRoot

# Run the source PowerShell script
. "$path\$src_file"

## Run tests to verify that the PowerShell script ran succesfully
Write-Host "$(Get-Date) Testing that Availability Group was created succesfully"

## Validate that the Availability Group was created
Describe "Availability-Group-Created" {
  It "Availability Group has two nodes that are synchronized" {

    # Run the Cmdlet to test the Availability Group
    $results = Invoke-Command -ComputerName $node1 -Credential $cred -ScriptBlock {
      param($node1, $name_ag)

      Test-SqlAvailabilityGroup `
        -Path "SQLSERVER:\SQL\$($node1)\DEFAULT\AvailabilityGroups\$($name_ag)"
    } -ArgumentList $node1, $name_ag

    # TEST: Should have two nodes and they should be in a "Synchronized" state
    [string]$results[0].HealthState | Should Match "Healthy"


    # Use SMO instead of PowerShell Cmdlet to query the DatabaseReplicaStates
    $results = Invoke-Command -ComputerName $node1 -Credential $cred -ScriptBlock {
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
    $results = Invoke-Command -ComputerName $node1 -Credential $cred `
      -ScriptBlock { 
      param($name_ag_listener)

      Invoke-Sqlcmd `
        -Query "SELECT ServerName = @@SERVERNAME" `
        -ServerInstance $name_ag_listener
    } -ArgumentList $name_ag_listener

    # TEST: The @@SERVERNAME should be the name of Node 1
    [string]$results[0].ServerName | Should Match $node1

    # Switch to Node 2
    Invoke-Command -ComputerName $node2 -Credential $cred -ScriptBlock {
      param($node2, $name_ag)

      Switch-SqlAvailabilityGroup `
        -Path "SQLSERVER:\SQL\$($node2)\DEFAULT\AvailabilityGroups\$($name_ag)"

        Start-Sleep 15
    } -ArgumentList $node2, $name_ag


    # Connect to Listener and return @@SERVERNAME. Should match Node 2.
    $results = Invoke-Command -ComputerName $node2 -Credential $cred -ScriptBlock {
      param($name_ag_listener)

      Invoke-Sqlcmd `
        -Query "SELECT ServerName = @@SERVERNAME" `
        -ServerInstance $name_ag_listener
    } -ArgumentList $name_ag_listener

    # TEST: The @@SERVERNAME should be the name of Node 2
    [string]$results[0].ServerName | Should Match $node2

    # Switch to Node 1 again
    Invoke-Command -ComputerName $node1 -Credential $cred -ScriptBlock {
      param($node1, $name_ag)

      Switch-SqlAvailabilityGroup `
        -Path "SQLSERVER:\SQL\$($node1)\DEFAULT\AvailabilityGroups\$($name_ag)"
    } -ArgumentList $node1, $name_ag
  }
}
