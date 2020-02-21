#Requires -Version 5
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

##############################################################################
#.SYNOPSIS
# Set up a two node SQL Server Availability Group
#
#.DESCRIPTION
# This script will set up a two node Availability Group in SQL Server.
# It would normally be called by the script
# create-sql-instance-availability-group.ps1, but it can also be called by 
# itself. Before running this script you must install 'Google Cloud SDK' 
# which includes the Powershell CmdLets for Google Cloud.
# See https://cloud.google.com/tools/powershell/docs/
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
#
# Note: May need to run this command if server blocks PowerShell scripts from running
#   Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass -Force
##############################################################################

$ErrorActionPreference = 'Stop'

# If we are not able to figure out $PSScriptRoot, assume the scripts are in C:\Scripts
if ($PSScriptRoot -eq "") {
  $script_path = "C:\Scripts"
} else {
  $script_path = $PSScriptRoot
}

################################################################################
# Read the parameters for this script. They are found in the file 
# parameters-config.ps1. We assume it is in the same folder as this script
################################################################################
if (!($cred)) {
  . "$script_path\parameters-config.ps1"
}


################################################################################
# Create a file share witness
################################################################################
$session_option = New-CimSessionOption -ProxyAuthentication Kerberos -SkipCACheck -SkipCNCheck -UseSsl

# Find the domain controller name
$logonserver = $env:LOGONSERVER -replace "\\",""

# If unable to figure out the logon server, use the name of the domain controller.
if ($logonserver -eq "") {
  $logonserver = $domain_cntr
}
$dcsession = New-CimSession -ComputerName $logonserver -SessionOption $session_option

# Create shared folder if it does not exist
if (!( Get-SmbShare -CimSession $dcsession | Where-Object Name -eq "QWitness" )) {
  New-SmbShare -Name "QWitness" -Path "C:\QWitness" -Description "SQL File Share Witness" -CimSession $dcsession -FullAccess "$domain\Administrator"
}

# Grant access to the shared folder to both nodes
Grant-SmbShareAccess -Name "QWitness" -AccessRight Full -CimSession $dcsession -Force -AccountName "$node1`$","$node2`$"

# Remove-SmbShare -Name "QWitness" -CimSession $dcsession -Force


################################################################################
# Create the test database in Node1
################################################################################
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


################################################################################
# Create a shared folder for initial backup used by Availability Groups
# We choose Node 2, but it can be any network share accesible by both nodes
################################################################################
Invoke-Command -ComputerName $node2 -ScriptBlock { 
  param($sql_backup, $share_name, $node1)

  # Create folder for the backup in Node2.
  # For this demo script we just use one of the nodes for simplicity
  if (!(Test-Path -Path $sql_backup )) { 
    New-item -ItemType Directory $sql_backup | Out-Null }

  # Create a Windows share for the folder
  Write-Host "$(Get-Date) $hostname - Creating Windows Share: $share_name"
  Get-SmbShare | Where-Object -Property Name -eq $share_name | 
    Remove-SmbShare -Force
  New-SMBShare –Name $share_name –Path $sql_backup –FullAccess "$($node1)`$","NT SERVICE\MSSQLSERVER" | Out-Null
} -ArgumentList $sql_backup, $share_name, $node1


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

  # Enable clustering (just in case is  not already done)
  Install-WindowsFeature Failover-Clustering -IncludeManagementTools | 
    Format-Table | Out-String

  # Create the cluster
  New-Cluster -Name $name_wsfc -Node $node1,$node2 -NoStorage `
    -StaticAddress $ip_wsfc1,$ip_wsfc2 | Format-Table | Out-String

  # It is recommended to have a Witness in a 2-node cluster
  # We use a file share called "quorum" in the domain controller
  # Uncomment this command if using a fileshare witness
  #Get-Cluster | Set-ClusterQuorum -NodeAndFileShareMajority "\\$ip_domain_cntr\quorum"

  # Enable Always-On on both nodes
  Enable-SqlAlwaysOn -ServerInstance $node1 -Force
  Enable-SqlAlwaysOn -ServerInstance $node2 -Force
}
else {
  Write-Host "$(Get-Date) $hostname - Skipping the creation of the cluster"
}


################################################################################
# Create the SQL Server endpoints for the Availability Groups
################################################################################
# First remove the endpoints if they exist
if (Test-Path -Path "SQLSERVER:SQL\$node1\DEFAULT\Endpoints\Hadr_endpoint")  {
  Remove-Item -Path "SQLSERVER:SQL\$node1\DEFAULT\Endpoints\Hadr_endpoint"
}

if (Test-Path -Path "SQLSERVER:SQL\$node2\DEFAULT\Endpoints\Hadr_endpoint")  {
  Remove-Item -Path "SQLSERVER:SQL\$node2\DEFAULT\Endpoints\Hadr_endpoint"
}

# Create the endpoints in both nodes
Invoke-Command -ComputerName $node1,$node2 -ScriptBlock { 

  param($domain_netbios, $node1, $node2)

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
} -ArgumentList $domain_netbios, $node1, $node2


################################################################################
# Create the Availability Group
################################################################################
Write-Host "$(Get-Date) Creating the SQL Server Availability Group"
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
  #Add-SqlAvailabilityDatabase `
  #  -Path "SQLSERVER:\SQL\$($node2)\DEFAULT\AvailabilityGroups\$($name_ag)" `
  #  -Database $db_name

  # In SQL Server 2017 the Add-SqlAvailabilityDatabase was failing so decided to run it as a query instead
  Invoke-Command -ComputerName $node2 -ScriptBlock { 
    param($db_name, $name_ag)
  
    $hostname = [System.Net.Dns]::GetHostName()
    Write-Host "$(Get-Date) $hostname - Adding database [$db_name] to Availability Group [$name_ag]"
  
    $query = "ALTER DATABASE [$db_name] SET HADR AVAILABILITY GROUP = [$name_ag]"
    Write-Host $query
  
    Invoke-Sqlcmd -Query $query
  } -ArgumentList $db_name, $name_ag


  # Create the listener
  # Notice we use /24 netmask. It should match the netmask of the IP address of the nodes.
  Write-Host "$(Get-Date) $hostname - Create Listener ($ip_ag_listener1, $ip_ag_listener2)"
  New-SqlAvailabilityGroupListener `
    -Name $name_ag_listener `
    -StaticIp "$($ip_ag_listener1)/255.255.255.0","$($ip_ag_listener2)/255.255.255.0" `
    -Path SQLSERVER:\SQL\$($node1)\DEFAULT\AvailabilityGroups\$($name_ag) | 
      Format-Table | Out-String
}
else {
  Write-Host "$(Get-Date) $hostname - Skip creation of the Availability Group."
  Write-Host "$(Get-Date) This node already member of the Availability Group"
}

Write-Host "$(Get-Date) SQL Server Availability Group $name_ag is now ready"


################################################################################
# Remove the shared folder in Node 2 as it is no longer needed
################################################################################
Invoke-Command -ComputerName $node2 -ScriptBlock { 
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


# Get-WSManCredSSP
# Use these commands to destroy the AG and WSFC if you are re-doing the creation again
# Remove-SqlAvailabilityGroup -Path "SQLSERVER:\SQL\$($node1)\DEFAULT\AvailabilityGroups\$($name_ag)"
# Remove-Cluster -Force -CleanupAD
# Disable-SqlAlwaysOn -ServerInstance $node1 -Force
# Disable-SqlAlwaysOn -ServerInstance $node2 -Force
