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

###############################################################################
#.SYNOPSIS
# Create two Windows instances and set up a SQL Server Availability Group
# using Alias IPs
#
#.DESCRIPTION
# This script will automate the steps described on:
# "Configuring SQL Server Availability Groups"
# https://cloud.google.com/compute/docs/instances/sql-server/configure-availability
#
# PREREQUISITES:
# 1. Before running the script, create the three subnetworks:
#    wsfcsubnet1, wsfcsubnet2, wsfcsubnet3 (Optional)
# 2. A domain controller for a domain called "dbeng.com" must also exist.
#    The IP for the domain controller is 10.2.0.100 and resides in wsfcsubnet3 (optional)
#
# Quick summary of the configuration:
# + Custom network "wsfcnet" with three subnetworks: wsfcsubnet1, wsfcsubnet2, wsfcsubnet3
#   - wsfcsubnet1: 10.0.0.0/24
#   - wsfcsubnet1: 10.1.0.0/24
#   - wsfcsubnet3: 10.2.0.0/24
#
# + Two Nodes
#   - cluster-sql1: 10.0.0.4 in wsfcsubnet1
#     This node has two extra IP Aliases (10.0.0.5, 10.0.0.6)
#      
#   - cluster-sql2: 10.1.0.4 in wsfcsubnet1
#     This node has two extra IP Aliases (10.1.0.5, 10.1.0.6)
#
# + Windows Server Failover Cluster (WSFC)
#   - Name: cluster-dbclus
#   - Two IPs from the IP Aliases: 10.0.0.5, 10.1.0.5
#
# + Availability Group & Listener
#   - Availability Group Name : cluster-ag
#   - Listener Name           : ag-listener
#   - Two IPs from the IP Aliases: 10.0.0.6, 10.1.0.6
#
#
# You must have the 'Google Cloud SDK' which includes the Powershell 
# CmdLets for Google Cloud. 
# See https://cloud.google.com/tools/powershell/docs/
#
# The script expects the files parameters-config.ps1 and
# create-sql-instance-availability-group.ps1 to exist in the same directory.
#
#.NOTES
# AUTHOR: Anibal Santiago - @SQLThinker
#
#.EXAMPLE
# create-sql-instance-availability-group.ps1
# Before running the script make you need to modify the file 
# parameters-config.ps1 with the parameters specific to your configuration
###############################################################################

cls
Set-Location $PSScriptRoot

################################################################################
# Read the parameters for this script. They are found in the file 
# parameters-config.ps1. We assume it is in the same folder as this script
################################################################################
. "$PSScriptRoot\parameters-config.ps1"


################################################################################
# Remove the instances and boot disks if they exists 
################################################################################
$ErrorActionPreference = 'Stop'

if ( $force_delete -eq $true ) {

  Write-Host "$(Get-Date) Removing instance $node1 if it exists"
  Get-GceInstance -Zone $zone | Where Name -EQ $node1 | Remove-GceInstance
  Get-GceDisk     -Zone $zone | Where Name -EQ $node1 | Remove-GceDisk

  Write-Host "$(Get-Date) Removing instance $node2 if it exists"
  Get-GceInstance -Zone $zone | Where Name -EQ $node2 | Remove-GceInstance
  Get-GceDisk     -Zone $zone | Where Name -EQ $node2 | Remove-GceDisk

  # Remove the routes for these nodes if they exist
  Write-Host "$(Get-Date) Removing routes for those instances"
  Get-GceRoute | 
    Where Name -In $node1-route, $node1-route-listener, `
      $node2-route, $node2-route-listener | 
    Remove-GceRoute
}


################################################################################
# Create a boot disk based on the image family that we want to create
################################################################################
$image_disk = Get-GceImage -Family $image_family

Write-Host "$(Get-Date) Creating boot disk for instance $node1"
$boot_disk1 = New-GceDisk -DiskName $node1 -DiskType $disk_type -Image $image_disk `
  -SizeGb $size_gb -Zone $zone

Write-Host "$(Get-Date) Creating boot disk for instance $node2"
$boot_disk2 = New-GceDisk -DiskName $node2 -DiskType $disk_type -Image $image_disk `
  -SizeGb $size_gb -Zone $zone


################################################################################
# Create the instances
# Add an IP alias with the Listener IP
################################################################################
# Continue on errors as the gcloud command is reported as failed even when it 
# was successful
$ErrorActionPreference = 'continue'
Write-Host "$(Get-Date) Creating instance $node1"
Invoke-Command -ScriptBlock {
    gcloud compute instances create $node1 --machine-type $machine_type `
      --zone $zone `
      --can-ip-forward `
      --disk="name=$node1,boot=yes,auto-delete=yes" `
      --network-interface subnet=$subnet1_name,private-network-ip=$ip_node1,aliases="$ip_wsfc1;$ip_ag_listener1" --quiet 2> $null
}

Write-Host "$(Get-Date) Creating instance $node2"
Invoke-Command -ScriptBlock {
    gcloud compute instances create $node2 --machine-type $machine_type `
      --zone $zone `
      --can-ip-forward `
      --disk="name=$node2,boot=yes,auto-delete=yes" `
      --network-interface subnet=$subnet2_name,private-network-ip=$ip_node2,aliases="$ip_wsfc2;$ip_ag_listener2" --quiet 2> $null
}
$ErrorActionPreference = 'stop'


################################################################################
# Wait until the instances are configured
# Note: We wait until the text 'Instance setup finished' shows in serial console
#       If this text changes in the future we need to modify this script
################################################################################
$creation_status = Get-GceInstance -zone $zone -Name $node1 -SerialPortOutput | 
    Select-String -Pattern 'Instance setup finished' -Quiet
$n = 20
while (!($creation_status)) {
  Write-Host "$(Get-Date) Waiting for instance $node1 to be ready"
  Start-Sleep -s 30
  $n -= 1
  if ($n -eq 0) {break}

  $creation_status = Get-GceInstance -zone $zone -Name $node1 -SerialPortOutput |
      Select-String -Pattern 'Instance setup finished' -Quiet
}
Write-Host "$(Get-Date) Instance $node1 is now ready"

$creation_status = Get-GceInstance -zone $zone -Name $node2 -SerialPortOutput | 
    Select-String -Pattern 'Instance setup finished' -Quiet
$n = 20
while (!($creation_status)) {
  Write-Host "$(Get-Date) Waiting for instance $node2 to be ready"
  Start-Sleep -s 30
  $n -= 1
  if ($n -eq 0) {break}

  $creation_status = Get-GceInstance -zone $zone -Name $node2 -SerialPortOutput | 
      Select-String -Pattern 'Instance setup finished' -Quiet
}
Write-Host "$(Get-Date) Instance $node2 is now ready"


# Wait 2 minutes before trying to connect to make sure the server is ready for connections
Start-Sleep -s 120


################################################################################
# Generate a password for the account that will connect to the remote servers. 
# The 'gcloud' command apparently generates an error in Powershell but we ignore
# it as it is not a real error.
################################################################################
$ErrorActionPreference = 'continue'

# Generate the password for Node 1. We get a false error when running gcloud.exe
# from PowerShell, so we ignore it.
$results = $null
$results = Invoke-Command -ScriptBlock { 
  gcloud compute reset-windows-password $node1 --zone $zone `
    --user $config_user --quiet 2> $null
} | ConvertFrom-String

# If the command fails, run it again but this time show the error message
if ( !($lastexitcode -eq 0)) {
  Write-Host "An error ocurred when trying to generate a password for $node1"
  Invoke-Command -ScriptBlock { 
    gcloud compute reset-windows-password $node1 --zone $zone `
      --user $config_user --quiet
  }
}

# Grab the IP and Password from the results of the 'gcloud.exe' command
$ip_address1 = $results.P2[0]
$password1 = ConvertTo-SecureString -String $results.P2[1] -AsPlainText -Force


# Generate the password for Node 2. We get a false error when running gcloud.exe
# from PowerShell, so we ignore it.
$results = $null
$results = Invoke-Command -ScriptBlock { 
  gcloud compute reset-windows-password $node2 --zone $zone `
    --user $config_user --quiet 2> $null
} | ConvertFrom-String

# If the command fails, run it again but this time show the error message
if ( !($lastexitcode -eq 0)) {
  Write-Host "An error ocurred when trying to generate a password for $node2"
  Invoke-Command -ScriptBlock { 
    gcloud compute reset-windows-password $node2 --zone $zone `
      --user $config_user --quiet
  }
}

# Grab the IP and Password from the results of the 'gcloud.exe' command
$ip_address2 = $results.P2[0]
$password2 = ConvertTo-SecureString -String $results.P2[1] -AsPlainText -Force

$results = $null
$ErrorActionPreference = 'stop'


################################################################################
# In the following sections we do remote connections to nodes to configure them
################################################################################
$session_options = New-PSSessionOption -SkipCACheck -SkipCNCheck `
  -SkipRevocationCheck

$credential1 = New-Object System.Management.Automation.PSCredential($config_user, $password1)
$credential2 = New-Object System.Management.Automation.PSCredential($config_user, $password2)

Write-Host "  Using PowerShell remoting to connect to both nodes"
Write-Host "  If you get errors verify that WinRM port 5986 is open in Firewall"

# Create a remote session in each server.
Write-Host "$(Get-Date) Creating remote session to $node1 - $ip_address1"
$session1 = New-PSSession -ComputerName $ip_address1 -UseSSL `
  -Credential $credential1 -SessionOption $session_options

Write-Host "$(Get-Date) Creating remote session to $node2 - $ip_address2"
$session2 = New-PSSession -ComputerName $ip_address2 -UseSSL `
  -Credential $credential2 -SessionOption $session_options


# The following commands run in both nodes.
# Change the DNS server to be the domain controller.
# This makes it easier to add the nodes to the AD domain.
Invoke-Command -Session $session1,$session2 -ScriptBlock {
  param($ip_domain_cntr)

  $hostname = [System.Net.Dns]::GetHostName()

  Write-Host "$(Get-Date) $hostname - Changing DNS to domain controller: $ip_domain_cntr"

  # Change the Dns server
  $adapter = Get-NetAdapter -Name Ethernet
  $adapter | Set-DnsClientServerAddress -ServerAddresses $ip_domain_cntr

} -ArgumentList $ip_domain_cntr 2>&1 | Out-Null


# The following commands run in both nodes. 
# They are needed in preparation to create the database.
Invoke-Command -Session $session1,$session2 -ScriptBlock {
  param($sql_data, $sql_log)

  $hostname = [System.Net.Dns]::GetHostName()

  # Create directories for data and transaction log
  Write-Host "$(Get-Date) $hostname - Creating folders: $sql_data; $sql_log"
  if (!(Test-Path -Path $sql_data )) { New-item -ItemType Directory $sql_data | 
    Out-Null }
  if (!(Test-Path -Path $sql_log  )) { New-item -ItemType Directory $sql_log | 
    Out-Null }
} -ArgumentList $sql_data, $sql_log


# The following commands run in node 1. 
# Create a directory to run a PowerShell script locally.
Invoke-Command -Session $session1 -ScriptBlock {

  $hostname = [System.Net.Dns]::GetHostName()

  # Create directory C:\Scripts
  Write-Host "$(Get-Date) $hostname - Creating folder: C:\Scripts"
  if (!(Test-Path -Path "C:\Scripts" )) { New-item -ItemType Directory "C:\Scripts" | 
    Out-Null }
}


# The following code will run in both nodes. 
# It has all other configurations that we want to do in the servers.
Invoke-Command -Session $session1,$session2 -ScriptBlock {
  param($node1, $node2, $ip_domain_cntr, $domain, $cred, $sql_data, $sql_log)

  $hostname = [System.Net.Dns]::GetHostName()
  
  # Rename the SQL Server instance
  $query = "exec sp_dropserver @@servername; exec sp_addserver '$hostname', local"
  Invoke-Sqlcmd -Query $query

  # Open firewall ports. Display output as table to save vertical space in the output.
  Write-Host "$(Get-Date) $hostname - Open firewall ports in $hostname"
  New-NetFirewallRule -DisplayName 'Open Port 5022 for Availability Groups' `
    -Direction Inbound -Protocol TCP -LocalPort 5022 -Action allow | 
	  SELECT DisplayName, Enabled, Direction, Action
  New-NetFirewallRule -DisplayName 'Open Port 1433 for SQL Server' `
    -Direction Inbound -Protocol TCP -LocalPort 1433 -Action allow | 
	  SELECT DisplayName, Enabled, Direction, Action

  # Enable clustering
  Write-Host "$(Get-Date) $hostname - Enable clustering"
  Install-WindowsFeature Failover-Clustering -IncludeManagementTools | 
    Format-Table | Out-String

  # Add computer to domain
  Write-Host "$(Get-Date) $hostname - Adding the server to the Domain: $domain"
  Add-Computer -DomainName $domain -Credential $cred -ComputerName $hostname -Restart -Force

  Write-Host "$(Get-Date) Restarting the server $hostname"

} -ArgumentList $node1, $node2, $ip_domain_cntr, $domain, $cred, $sql_data, $sql_log


# Close the remote sessions. They need to be re-created after the restarts.
Remove-PSSession $session1
Remove-Variable session1
Remove-PSSession $session2
Remove-Variable session2


##################################################################################
# Wait until the instances are restarted after joining them to the domain
# Note: We wait until the text 'Starting shutdown scripts' shows in serial console
#       If this text changes in the future we need to modify this script
##################################################################################
$creation_status = Get-GceInstance -zone $zone -Name $node1 -SerialPortOutput | 
    Select-String -Pattern 'Starting shutdown scripts' -Quiet
$n = 20
while (!($creation_status)) {
  Write-Host "$(Get-Date) Waiting for instance $node1 to restart"
  Start-Sleep -s 30
  $n -= 1
  if ($n -eq 0) {break}

  $creation_status = Get-GceInstance -zone $zone -Name $node1 -SerialPortOutput | 
      Select-String -Pattern 'Starting shutdown scripts' -Quiet
}

$creation_status = Get-GceInstance -zone $zone -Name $node2 -SerialPortOutput | 
    Select-String -Pattern 'Starting shutdown scripts' -Quiet
$n = 20
while (!($creation_status)) {
  Write-Host "$(Get-Date) Waiting for instance $node2 to restart"
  Start-Sleep -s 30
  $n -= 1
  if ($n -eq 0) {break}

  $creation_status = Get-GceInstance -zone $zone -Name $node2 -SerialPortOutput | 
      Select-String -Pattern 'Starting shutdown scripts' -Quiet
}

# Wait 2 minutes to make sure all services have started
Start-Sleep -s 120

Write-Host "$(Get-Date) Ready now to create a Windows Failover Cluster (WSFC)"
Write-Host "                    $node1 - $ip_address1"
Write-Host "                    $node2 - $ip_address2"


# Create a remote session only to Node 1
# If we are running the script from a computer in the same domain, we need to specify the FQDN
# Note: If run from a computer in a different domain you may still get an error
#       In that case just run create-availability-group.ps1 manually from one of the nodes
Write-Host "$(Get-Date) Create a remote session to each server again"

if ( (gwmi win32_computersystem).Domain.ToLower() -eq $domain.ToLower() ) {
  $fqdn1="$node1.$env:USERDNSDOMAIN".ToLower()

  Write-Host "$(Get-Date) Creating remote session to $fqdn1"
  $session1 = New-PSSession -ComputerName $fqdn1 -UseSSL -SessionOption $session_options
}
else {
  Write-Host "$(Get-Date) Creating remote session to $node1 - $ip_address1"
  $session1 = New-PSSession -ComputerName $ip_address1 -UseSSL `
    -Credential $cred -SessionOption $session_options
}

##################################################################################
# Create a Windows Failover Cluster (WSFC) and Availability Group
##################################################################################
# Copy the files create-availability-group.ps1 and parameters-config.ps1 to Node 1
Write-Host "$(Get-Date) Copy PowerShell scripts to $node1"
Copy-Item ".\create-availability-group.ps1" `
  -Destination "C:\Scripts\" `
  -ToSession $session1

Copy-Item ".\parameters-config.ps1" `
  -Destination "C:\Scripts\" `
  -ToSession $session1

# Do a remote session to Node 1 to create a scheduled job to setup the cluster.
# Seting up the cluster requires connecting to more than one server which creates
# a double-hop issue. We create a scheduled job to setup the cluster outside of
# our WinRM connection to avoid this issue.
Invoke-Command -Session $session1 -ScriptBlock {
  param($cred)

  $hostname = [System.Net.Dns]::GetHostName()
  Write-Host "$(Get-Date) $hostname - Creating scheduled job to create Availability Group" -BackgroundColor Green -ForegroundColor Yellow

  # Delete job if it exists
  Get-ScheduledJob  | Where Name -eq 'Create-Availability-Group' | Unregister-ScheduledJob  -Confirm:$false

  # Create a scheduled job to create the cluster and AG
  # Just for this session bypass the execution policy to allow this Powershell script to run
  Register-ScheduledJob -Name Create-Availability-Group `
    -ScriptBlock { Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass -Force; C:\Scripts\create-availability-group.ps1 -Verbose *> 'C:\Scripts\create-availability-group.log' } `
    -Credential $Cred -RunNow

  # Wait 30 seconds before checking if the job is running
  Start-Sleep -s 30

  # Wait until the scheduled job finish
  $status = Get-Job | 
    Where-Object { ($_.Name -eq 'Create-Availability-Group') -and ($_.State -eq 'Running') } |
    SELECT State
  $n = 40

  while ($status) {
    Write-Host "$(Get-Date) $hostname - Waiting for Availability Group to be ready"
    Start-Sleep -s 30
    $n -= 1
    if ($n -eq 0) {break}

    $status = Get-Job |
      Where-Object { ($_.Name -eq 'Create-Availability-Group') -and ($_.State -eq 'Running') } |
      SELECT State
  }

  Write-Host "$(Get-Date) $hostname - Scheduled job to create Availability Group finished"

  # Display the contents of the log file created by the scheduled job
  Write-Host "$(Get-Date) $hostname - Displaying the content of scheduled job log file" -BackgroundColor Green -ForegroundColor Yellow

  Get-Content -Path 'C:\Scripts\create-availability-group.log'

  Write-Host "$(Get-Date) $hostname - End of log file" -BackgroundColor Green -ForegroundColor Yellow

  # Delete job if it exists
#  Get-ScheduledJob  | Where Name -eq 'Create-Availability-Group' | Unregister-ScheduledJob  -Confirm:$false
} -ArgumentList $cred


# Verify that the AG was created. This only works if ran from a local node.
# Left commented as an example for reference
#Invoke-Command -Session $session1 -ScriptBlock {
#  param($node1, $name_ag)

  # Verify if the AG exists
#  Import-Module SQLPS -DisableNameChecking
#  if (Test-Path -Path "SQLSERVER:\SQL\$($node1)\DEFAULT\AvailabilityGroups\$($name_ag)")  {
#    Write-Host "$(Get-Date) Verified that the Availability Group exists"
#  } else {
#    Throw "Unable to verify that the Availability Group exists"
#  }

#} -ArgumentList $node1, $name_ag


# Close the remote sessions
Remove-PSSession $session1
Remove-Variable session1

Write-Host "$(Get-Date) End of create-sql-instance-availability-group.ps1"


# Get-PSSession
