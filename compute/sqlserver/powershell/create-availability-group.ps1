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

##############################################################################
#.SYNOPSIS
# Set up a two node SQL Server Availability Group
#
#.DESCRIPTION
# This script will set up a two node Availability Group in SQL Server.
# It would normally be called by the script
# create-sql-instance-availability-group.ps1, but it can also be called by 
# itself. Before running this script you must install 'Cloud SDK for Windows' 
# which includes the Powershell CmdLet for Google Cloud.
# See https://googlecloudplatform.github.io/google-cloud-powershell
# The script expects the file parameters-config.ps1 to exist in the same 
# directory.
#
#.NOTES
# AUTHOR: Anibal Santiago - @SQLThinker
#
#.EXAMPLE
# create-availability-group.ps1
# Before running the script make you need to modify the file 
# parameters-config.ps1 with the parameters specific to your configuration
##############################################################################

$ErrorActionPreference = 'Stop'
Set-Location $PSScriptRoot

################################################################################
# Read the parameters for this script. They are found in the file 
# parameters-config.ps1. We assume it is in the same folder as this script
################################################################################
if (!($cred)) {
  . ".\parameters-config.ps1"
}

# Only continue if SQL Server is installed in both nodes
$node1_sql_server = Get-Service -ComputerName $node1 | 
  Where Name -Like 'MSSQLSERVER*'
$node2_sql_server = Get-Service -ComputerName $node2 | 
  Where Name -Like 'MSSQLSERVER*'

If ( !($node1_sql_server) -Or !($node2_sql_server) ) {
  Throw 'Need to install SQL Server in both nodes before running this script.'
}

# Only continue if script is ran from a computer that is member of the domain
# Enable-WSManCredSSP will fail if the computer is not member of domain
if ( !( Get-ADDomain | Where Forest -eq $domain ) ) {
  Throw "Need to run script from a computer member of the '$domain' domain."
}

# We will refer to node1 and node2 with their full domain name
$node1_fqdn = $node1 + "." + $domain
$node2_fqdn = $node2 + "." + $domain

################################################################################
# Create the test database in Node1
################################################################################
Invoke-Command -ComputerName $node1_fqdn -Credential $cred -ScriptBlock { 
  param($db_name, $sql_data, $sql_log, $log_size, $log_growth, $data_size, $data_growth)

  $hostname = [System.Net.Dns]::GetHostName()

  Import-Module SQLPS -DisableNameChecking
  $objServer = New-Object -TypeName Microsoft.SqlServer.Management.Smo.Server `
    -ArgumentList 'localhost'

  # Only continue if the database does not exist
  $objDB = $objServer.Databases[$db_name]
  if (!($objDB)) {

    Write-Host "$(Get-Date) $hostname - Creating the database $db_name"
    $objDB = New-Object `
      -TypeName Microsoft.SqlServer.Management.Smo.Database($objServer, $db_name)

    # Create the primary file group and add it to the database
    $objPrimaryFG = New-Object `
      -TypeName Microsoft.SqlServer.Management.Smo.Filegroup($objDB, 'PRIMARY')
    $objDB.Filegroups.Add($objPrimaryFG)

    # Create a single data file and add it to the Primary filegroup
    $dataFileName = $db_name + '_Data'
    $objData = New-Object `
      -TypeName Microsoft.SqlServer.Management.Smo.DataFile($objPrimaryFG, $dataFileName)
    $objData.FileName = $sql_data + '\' + $dataFileName + '.mdf'
    $objData.Size = ($data_size * 1024)
    $objData.GrowthType = 'KB'
    $objData.Growth = ($data_growth * 1024)
    $objData.IsPrimaryFile = 'true'
    $objPrimaryFG.Files.Add($objData)

    # Create the log file and add it to the database
    $logName = $db_name + '_Log'
    $objLog = New-Object Microsoft.SqlServer.Management.Smo.LogFile($objDB, $logName)
    $objLog.FileName = $sql_log + '\' + $logName + '.ldf'
    $objLog.Size = ($log_size * 1024)
    $objLog.GrowthType = 'KB'
    $objLog.Growth = ($log_growth * 1024)
    $objDB.LogFiles.Add($objLog)

    # Create the database
    $objDB.Script()  # Show a script with the command we are about to run
    $objDB.Create()  # Create the database
    $objDB.SetOwner('sa')  # Change the owner to sa
  }
  else {
      Write-Host "$(Get-Date) $hostname - Skipping the creation of the $db_name DB"
  }
} -ArgumentList $db_name, $sql_data, $sql_log, $log_size, $log_growth, `
  $data_size, $data_growth



################################################################################
# Create a shared folder for initial backup used by Availability Groups
# We choose Node 2, but it can be any network share accesible by both nodes
################################################################################
Invoke-Command -ComputerName $node2_fqdn -Credential $cred -ScriptBlock { 
  param($sql_backup, $share_name, $domain, $svc_acct, $node1)

  $hostname = [System.Net.Dns]::GetHostName()
  
  # Create folder for the backup in Node2.
  # For this demo script we just use one of the nodes for simplicity
  Write-Host "$(Get-Date) $hostname - Creating folders: $sql_backup"
  if (!(Test-Path -Path $sql_backup )) { 
    New-item -ItemType Directory $sql_backup | Out-Null }

  # Create a Windows share for the folder
  Write-Host "$(Get-Date) $hostname - Creating Windows Share: $share_name"
  Get-SmbShare | Where-Object -Property Name -eq $share_name | 
    Remove-SmbShare -Force
  New-SMBShare –Name $share_name –Path $sql_backup –FullAccess "$($node1)`$","NT SERVICE\MSSQLSERVER" | Out-Null
} -ArgumentList $sql_backup, $share_name, $domain, $svc_acct, $node1


# Enable CredSSP in local computer
Write-Host "$(Get-Date) Enable CredSSP Client in local machine"
Enable-WSManCredSSP Client -DelegateComputer * -Force | Out-Null

# Wait 15 secs before enabling CredSSP in both servers
# On ocassions got errors when running the command that follows without waiting
Start-Sleep -s 15

# Enable CredSSP Server in remote nodes
Write-Host "$(Get-Date) Enable CredSSP Server nodes: $node1_fqdn, $node2_fqdn"
Invoke-Command -ComputerName $node1_fqdn,$node2_fqdn -ScriptBlock { 
  Enable-WSManCredSSP Server -Force 
} -Credential $cred -SessionOption $session_options | Out-Null


# We may need to remove AD objects, so we will need the RSAT-AD-PowerShell
Install-WindowsFeature RSAT-AD-PowerShell
Import-Module ActiveDirectory


################################################################################
# Create the Windows Failover Cluster
################################################################################
# Check if the Node1 is not a member of a cluster before trying to create it
if (!( Get-Service -ComputerName $node1 -DisplayName 'Cluster Service' | 
  Where Status -EQ 'Running' )) {

  # Remove the AD objects for the cluster and AG if they exist
  Write-Host "$(Get-Date) Removing AD Objects if they exist"
  Get-ADComputer -Filter 'Name -eq $name_wsfc' | 
    Set-ADObject -ProtectedFromAccidentalDeletion:$false -PassThru | 
      Remove-ADComputer -Confirm:$false
 
  Get-ADComputer -Filter 'Name -eq $name_ag' | 
    Set-ADObject -ProtectedFromAccidentalDeletion:$false -PassThru | 
      Remove-ADComputer -Confirm:$false

  Get-ADComputer -Filter 'Name -eq $name_ag_listener' | 
    Set-ADObject -ProtectedFromAccidentalDeletion:$false -PassThru | 
      Remove-ADComputer -Confirm:$false


  Write-Host "$(Get-Date) Creating WSFC:$name_wsfc; Nodes($node1, $node2)"
  Invoke-Command -ComputerName $node1_fqdn -Authentication Credssp `
    -Credential $cred -ScriptBlock {
    param($name_wsfc, $node1, $node2, $ip_wsfc1, $ip_wsfc2, $domain_cntr)

    $hostname = [System.Net.Dns]::GetHostName()

    # Enable clustering (just in case is  not already done)
    Install-WindowsFeature Failover-Clustering -IncludeManagementTools | 
      Format-Table

    # Create the cluster
    New-Cluster -Name $name_wsfc -Node $node1,$node2 -NoStorage `
      -StaticAddress $ip_wsfc1,$ip_wsfc2 | Format-Table

    # It is recommended to have a Witness in a 2-node cluster
    # We use a file share called "quorum" in the domain controller
    # Uncomment this command if using a fileshare witness
    #Get-Cluster | Set-ClusterQuorum -NodeAndFileShareMajority "\\$($domain_cntr)\quorum"

    # Enable Always-On on both nodes
    Enable-SqlAlwaysOn -ServerInstance $node1 -Force
    Enable-SqlAlwaysOn -ServerInstance $node2 -Force
  } -ArgumentList $name_wsfc, $node1_fqdn, $node2_fqdn, $ip_wsfc1, $ip_wsfc2, $domain_cntr
}
else {
  Write-Host "$(Get-Date) $hostname - Skipping the creation of the cluster"
}



################################################################################
# Create the SQL Server endpoints for the Availability Groups
################################################################################
Invoke-Command -ComputerName $node1_fqdn,$node2_fqdn -Credential `
  $cred -ScriptBlock { 

  param($svc_acct, $domain_netbios, $node1, $node2)

  $hostname = [System.Net.Dns]::GetHostName() 
  Write-Host "$(Get-Date) $hostname - Creating endpoint on node $hostname"

  # Figure out the remote node to grant connect permmission
  if ( $hostname.ToLower() -eq $node1.ToLower() ) {
    $remote_node = "$($domain_netbios)\$($node2)`$"
  }
  
  if ( $hostname.ToLower() -eq $node2.ToLower() ) {
    $remote_node = "$($domain_netbios)\$($node1)`$"
  }

  # Creating endpoint
  $endpoint = New-SqlHadrEndpoint "Hadr_endpoint" `
    -Port 5022 `
    -Path "SQLSERVER:\SQL\$hostname\Default"
  Set-SqlHadrEndpoint -InputObject $endpoint -State "Started" | Out-Null

  # Grant connect permissions to the endpoints
  $query = " `
    IF SUSER_ID('$($remote_node)') IS NULL CREATE LOGIN [$($remote_node)] FROM WINDOWS `
    GO
    GRANT CONNECT ON ENDPOINT::[Hadr_endpoint] TO [$($remote_node)] `
    GO `
    IF EXISTS(SELECT * FROM sys.server_event_sessions WHERE name='AlwaysOn_health') `
    BEGIN `
      ALTER EVENT SESSION [AlwaysOn_health] ON SERVER WITH (STARTUP_STATE=ON); `
    END `
    IF NOT EXISTS(SELECT * FROM sys.dm_xe_sessions WHERE name='AlwaysOn_health') `
    BEGIN `
      ALTER EVENT SESSION [AlwaysOn_health] ON SERVER STATE=START; `
    END `
    GO "

  Write-Host "$(Get-Date) $hostname - Granting permission to endpoint"
  Write-Host $query
  Invoke-Sqlcmd -Query $query
} -ArgumentList $svc_acct, $domain_netbios, $node1, $node2


################################################################################
# Create the Availability Group
################################################################################
Write-Host "$(Get-Date) Creating the SQL Server Availability Group"
Invoke-Command -ComputerName $node1_fqdn -Authentication Credssp -Credential `
  $cred -ScriptBlock {

  param($node1, $node2, $share_name, $db_name, $domain, $name_ag, `
    $name_ag_listener, $ip_ag_listener1, $ip_ag_listener2)

  $ErrorActionPreference = 'Stop'
  Import-Module SQLPS -DisableNameChecking

  # Check if the AG is already setup
  $AG = Get-ChildItem SQLSERVER:\SQL\$($node1)\DEFAULT\AvailabilityGroups

  if ( !($AG) ) {
    $hostname = [System.Net.Dns]::GetHostName()

    # Backup my database and its log on the primary
    Write-Host "$(Get-Date) $hostname - Creating backups of database $db_name"
    $backupDB = "\\$node2\$share_name\$($db_name)_db.bak"
    Backup-SqlDatabase `
      -Database $db_name `
      -BackupFile $backupDB `
      -ServerInstance $node1 `
      -Initialize

    $backupLog = "\\$node2\$share_name\$($db_name)_log.bak"
    Backup-SqlDatabase `
      -Database $db_name `
      -BackupFile $backupLog `
      -ServerInstance $node1 `
      -BackupAction Log -Initialize

    # Restore the database and log on the secondary (using NO RECOVERY)
    Write-Host "$(Get-Date) $hostname - Restoring backups of database in $node2"
    Restore-SqlDatabase `
      -Database $db_name `
      -BackupFile $backupDB `
      -ServerInstance $node2 `
      -NoRecovery -ReplaceDatabase
    Restore-SqlDatabase `
      -Database $db_name `
      -BackupFile $backupLog `
      -ServerInstance $node2 `
      -RestoreAction Log `
      -NoRecovery

    # Find the version of SQL Server that Node 1 is running
    $Srv = Get-Item SQLSERVER:\SQL\$($node1)\DEFAULT
    $Version = ($Srv.Version)

    # Create an in-memory representation of the primary replica
    $primaryReplica = New-SqlAvailabilityReplica `
      -Name $node1 `
      -EndpointURL "TCP://$($node1).$($domain):5022" `
      -AvailabilityMode SynchronousCommit `
      -FailoverMode Automatic `
      -Version $Version `
      -AsTemplate
  
    # Create an in-memory representation of the secondary replica
    $secondaryReplica = New-SqlAvailabilityReplica `
      -Name $node2 `
      -EndpointURL "TCP://$($node2).$($domain):5022" `
      -AvailabilityMode SynchronousCommit `
      -FailoverMode Automatic `
      -Version $Version `
      -AsTemplate

    # Create the availability group
    Write-Host "$(Get-Date) $hostname - Create Availability Group $name_ag"
    New-SqlAvailabilityGroup `
      -Name $name_ag `
      -Path "SQLSERVER:\SQL\$($node1)\DEFAULT" `
      -AvailabilityReplica @($primaryReplica,$secondaryReplica) `
      -Database $db_name

    # Join the secondary replica to the availability group.
    Write-Host "$(Get-Date) $hostname - Join $node2 to the Availability Group"
    Join-SqlAvailabilityGroup `
      -Path "SQLSERVER:\SQL\$($node2)\DEFAULT" `
      -Name $name_ag

    # Join the secondary database to the availability group.
    Write-Host "$(Get-Date) $hostname - Join DB in $node2 to Availability Group"
    Add-SqlAvailabilityDatabase `
      -Path "SQLSERVER:\SQL\$($node2)\DEFAULT\AvailabilityGroups\$($name_ag)" `
      -Database $db_name

    # Create the listener
    Write-Host "$(Get-Date) $hostname - Create Listener ($ip_ag_listener1, $ip_ag_listener2)"
    New-SqlAvailabilityGroupListener `
      -Name $name_ag_listener `
      -StaticIp "$($ip_ag_listener1)/255.255.0.0","$($ip_ag_listener2)/255.255.0.0" `
      -Path SQLSERVER:\SQL\$($node1)\DEFAULT\AvailabilityGroups\$($name_ag) | 
        Format-Table
  }
  else {
    Write-Host "$(Get-Date) $hostname - Skip creation of the Availability Group."
    Write-Host "$(Get-Date) This node already member of the Availability Group"
  }
} -ArgumentList $node1, $node2, $share_name, $db_name, $domain, $name_ag, `
  $name_ag_listener, $ip_ag_listener1, $ip_ag_listener2

Write-Host "$(Get-Date) SQL Server Availability Group $name_ag is now created"



################################################################################
# Remove the shared folder in Node 2 as it is no longer needed
################################################################################
Invoke-Command -ComputerName $node2_fqdn -Credential $cred -ScriptBlock { 
  param($sql_backup, $share_name)

  $hostname = [System.Net.Dns]::GetHostName()
  
  # Create a Windows share for the folder
  Write-Host "$(Get-Date) $hostname - Removing Windows Share: $share_name"
  Get-SmbShare | Where-Object -Property Name -eq $share_name | 
    Remove-SmbShare -Force

  Write-Host "$(Get-Date) $hostname - Removing folder: $sql_backup"
  if ((Test-Path -Path $sql_backup )) { 
    Remove-Item -Recurse -Force $sql_backup | Out-Null }

} -ArgumentList $sql_backup, $share_name, $domain, $svc_acct, $node1



# Disable CredSSP Server in remote nodes
Invoke-Command `
  -ComputerName $node1, $node2 `
  -Authentication Credssp `
  -Credential $cred `
  -ScriptBlock { Disable-WSManCredSSP -Role Server 
} | Format-Table

# Disable CredSSP in local computer
Disable-WSManCredSSP -Role Client | Format-Table


# Get-WSManCredSSP
# Use this command to destroy the WSFC if you are re-doing the creation again
# Invoke-Command -ComputerName $node1 -Authentication Credssp -Credential $cred -ScriptBlock { Remove-Cluster -Force -CleanupAD }