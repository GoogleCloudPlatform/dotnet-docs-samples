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

###############################################################################
#.SYNOPSIS
# Create two Windows instances and set up a SQL Server Availability Group
#
#.DESCRIPTION
# This script will automate the steps described on:
# https://cloud.google.com/compute/docs/instances/sql-server/configure-availability
#
# PREREQUISITES:
# 1. Before running the script, create the two subnetworks as described in the
#    document: "wsfcsubnet1", "wsfcsubnet2"
# 2. A domain controller for a domain called "dbeng.com" must also exist.
#    The IP for the domain controller is 10.2.0.100
#
# Quick summary of the setup steps:
# + Custom network "wsfcnet" with two subnetworks: "wsfcsubnet1", "wsfcsubnet2"
#   - wsfcsubnet1: 10.0.0.0/24  -  Hosts Min/Max (10.0.0.1 - 10.0.0.254)
#   - wsfcsubnet1: 10.1.0.0/24  -  Hosts Min/Max (10.1.0.1 - 10.1.0.254)
#
# + Two Nodes
#   - cluster-sql1: 10.0.0.4 in wsfcsubnet1
#   - cluster-sql2: 10.1.0.4 in wsfcsubnet1
#
# + The netmask in both nodes is later changed to /16 (255.255.0.0)
#   This is critical when we choose IPs for WSFC and Listener
#   - cluster-sql1 address range - Hosts Min/Max (10.0.0.1 - 10.0.255.254)
#   - cluster-sql2 address range - Hosts Min/Max (10.1.0.1 - 10.1.255.254)
#
# + Windows Server Failover Cluster (WSFC)
#   - Name: cluster-dbclus
#   - Two IPs: 10.0.1.4, 10.1.1.4 - They are outside the two subnetworks but
#     inside the two nodes address range.
#
# + Availability Group & Listener
#   - Availability Group Name : cluster-ag
#   - Listener Name           : cluster-listene
#   - Listener IPs - 10.0.1.5, 10.1.1.5 - They are outside the two subnetworks
#     but inside the two nodes address range.
#
# + Create routes in GCP for the 2 IPs for the WSFC and the 2 IPs for the 
#   listener. They are needed as those 4 IPs are outside the 2 subnetworks.
#
# You must have the 'Cloud SDK for Windows' which includes the Powershell 
# CmdLets for Google Cloud. 
# See https://googlecloudplatform.github.io/google-cloud-powershell
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
. ".\parameters-config.ps1"


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
$image_disk = Get-GceImage $image_project -Family $image_family

Write-Host "$(Get-Date) Creating boot disk for instance $node1"
$boot_disk1 = New-GceDisk -DiskName $node1 -DiskType pd-ssd -Image $image_disk `
  -SizeGb $size_gb -Zone $zone

Write-Host "$(Get-Date) Creating boot disk for instance $node2"
$boot_disk2 = New-GceDisk -DiskName $node2 -DiskType pd-ssd -Image $image_disk `
  -SizeGb $size_gb -Zone $zone


################################################################################
# Create the instances
################################################################################
$ErrorActionPreference = 'continue'
Write-Host "$(Get-Date) Creating instance $node1"
Invoke-Command -ScriptBlock {
    gcloud compute instances create $node1 --machine-type $machine_type `
      --zone $zone --subnet $subnet1_name --private-network-ip=$ip_node1 `
      --can-ip-forward --disk="name=$node1,boot=yes,auto-delete=yes" --quiet 2> $null
}

Write-Host "$(Get-Date) Creating instance $node2"
Invoke-Command -ScriptBlock {
    gcloud compute instances create $node2 --machine-type $machine_type `
      --zone $zone --subnet $subnet2_name --private-network-ip=$ip_node2 `
      --can-ip-forward --disk="name=$node2,boot=yes,auto-delete=yes" --quiet 2> $null
}


################################################################################
# Wait until the instances are configured
################################################################################
$creation_status = Get-GceInstance -zone $zone -Name $node1 -SerialPortOutput | 
  Select-String -Pattern 'Finished running startup scripts' -Quiet
$n = 20
while (!($creation_status)) {
  Write-Host "$(Get-Date) Waiting for instance $node1 to be created"
  Start-Sleep 15
  $n -= 1
  if ($n -eq 0) {break}

  $creation_status = Get-GceInstance -zone $zone -Name $node1 -SerialPortOutput |
    Select-String -Pattern 'Finished running startup scripts' -Quiet
}
Write-Host "$(Get-Date) Instance $node1 is now created"

$creation_status = Get-GceInstance -zone $zone -Name $node2 -SerialPortOutput | 
  Select-String -Pattern 'Finished running startup scripts' -Quiet
$n = 20
while (!($creation_status)) {
  Write-Host "$(Get-Date) Waiting for instance $node2 to be created"
  Start-Sleep 15
  $n -= 1
  if ($n -eq 0) {break} 

  $creation_status = Get-GceInstance -zone $zone -Name $node2 -SerialPortOutput | 
    Select-String -Pattern 'Finished running startup scripts' -Quiet
}
Write-Host "$(Get-Date) Instance $node2 is now created"


################################################################################
# Generate a password for the account that will connect to the remote servers. 
# The 'gcloud' command apparently generates an error in Powershell but we ignore
# it as it is not a real error.
################################################################################
$ErrorActionPreference = 'continue'

# Generate the password for Node 1
$results = $null
$results = Invoke-Command -ScriptBlock { 
  gcloud compute reset-windows-password $node1 --zone $zone --user $config_user `
    --quiet 2> $null
} | ConvertFrom-String

# Grab the IP and Password from the results of the 'gcloud.exe' command
$ip_address1 = $results.P2[0]
$password1 = ConvertTo-SecureString -String $results.P2[1] -AsPlainText -Force

# Generate the password for Node 2
$results = $null
$results = Invoke-Command -ScriptBlock { 
  gcloud compute reset-windows-password $node2 --zone $zone --user $config_user `
    --quiet 2> $null 
} | ConvertFrom-String

# Grab the IP and Password from the results of the 'gcloud.exe' command
$ip_address2 = $results.P2[0]
$password2 = ConvertTo-SecureString -String $results.P2[1] -AsPlainText -Force

$results = $null
$ErrorActionPreference = 'stop'


################################################################################
# Create some routes for the Windows Failover Cluster and Availability Group
# These are the IPs for the WSFC and the AG listener. 2 IPs per subnet.
# The IPs are outside the range of the GCP network and that's why the routes
# need to be created.
################################################################################
Write-Host "$(Get-Date) Creating routes in GCP"
$objNetwork = Get-GceNetwork -Name $network
$objNode1 = Get-GceInstance -Name $node1
$objNode2 = Get-GceInstance -Name $node2
Add-GceRoute -Name $node1-route-listener -Network $objNetwork `
  -DestinationIpRange "$ip_ag_listener1/32" -NextHopInstance $objNode1 -Priority 1 | 
    SELECT Name, DestRange | Format-Table
Add-GceRoute -Name $node2-route-listener -Network $objNetwork `
  -DestinationIpRange "$ip_ag_listener2/32" -NextHopInstance $objNode2 -Priority 1 | 
    SELECT Name, DestRange | Format-Table
Add-GceRoute -Name $node1-route -Network $objNetwork `
  -DestinationIpRange "$ip_wsfc1/32" -NextHopInstance $objNode1 -Priority 1 | 
    SELECT Name, DestRange | Format-Table
Add-GceRoute -Name $node2-route -Network $objNetwork `
  -DestinationIpRange "$ip_wsfc2/32" -NextHopInstance $objNode2 -Priority 1 | 
    SELECT Name, DestRange | Format-Table


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


# The following commands run in both nodes. Change the IP to static and 
# also change the Netmask. We assume the domain controller is Dns server.
# It is important to change the Netmask to include a range of IPs
# wider than the subnet where the node resides.
# Expect an error after losing connection when changing to static IP.
# Purposely tell PowerShell to ignore errors.
$ErrorActionPreference = 'continue'
Invoke-Command -Session $session1,$session2 -ScriptBlock {
  param($ip_domain_cntr, $netmask)

  $hostname = [System.Net.Dns]::GetHostName()

  Write-Host "$(Get-Date) $hostname - Changing network adapter to static IP"
  Write-Host "$(Get-Date) $hostname - Network connection will be lost"

  # Get the current configuration
  $ipconfig = Get-NetIPConfiguration
  $ipv4 = $ipconfig.IPv4Address.IPAddress
  $gateway = $ipconfig.IPv4DefaultGateway.NextHop
  $adapter = Get-NetAdapter -Name Ethernet

  # Remove the IP address and gateway
  $adapter | Remove-NetIPAddress -AddressFamily "IPv4" -Confirm:$false
  $adapter | Remove-NetRoute -AddressFamily "IPv4" -Confirm:$false

  # Specify the Dns server
  $adapter | Set-DnsClientServerAddress -ServerAddresses $ip_domain_cntr

  # Setup the IP as static and with the new netmask
  $adapter | New-NetIPAddress `
    -AddressFamily "IPv4" `
    -IPAddress $ipv4 `
    -PrefixLength $netmask `
    -DefaultGateway $gateway
} -ArgumentList $ip_domain_cntr, $netmask 2>&1 | Out-Null

$ErrorActionPreference = 'Stop'

# Close the remote sessions
# Changing the IP to Static will cause to lose the connection
Remove-PSSession $session1
Remove-Variable session1
Remove-PSSession $session2
Remove-Variable session2

Start-Sleep 30
# Create a remote session in each server again
Write-Host "$(Get-Date) Creating remote session to $node1 - $ip_address1"
$session1 = New-PSSession -ComputerName $ip_address1 -UseSSL `
  -Credential $credential1 -SessionOption $session_options

Write-Host "$(Get-Date) Creating remote session to $node2 - $ip_address2"
$session2 = New-PSSession -ComputerName $ip_address2 -UseSSL `
  -Credential $credential2 -SessionOption $session_options


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


# The following code will run in both nodes. 
# It has all other configurations that we want to do in the servers.
Invoke-Command -Session $session1,$session2 -ScriptBlock {
  param($node1, $node2, $ip_domain_cntr, $domain, $cred, $sql_data, $sql_log)

  $hostname = [System.Net.Dns]::GetHostName()
  Write-Host "$(Get-Date) Successfull remote session to $hostname"
  
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
    Format-Table

  # Add computer do domain
  Write-Host "$(Get-Date) $hostname - Adding the server to the Domain: $domain"
  Add-Computer -DomainName $domain -Credential $cred -ComputerName $hostname -Restart -Force

  Write-Host "$(Get-Date) Restarting the server $hostname"

} -ArgumentList $node1, $node2, $ip_domain_cntr, $domain, $cred, $sql_data, $sql_log



################################################################################
# Wait until the instances are restarted after joining them to the domain
################################################################################
$creation_status = Get-GceInstance -zone $zone -Name $node1 -SerialPortOutput | 
  Select-String -Pattern 'Finished running shutdown scripts' -Quiet
$n = 20
while (!($creation_status)) {
  Write-Host "$(Get-Date) Waiting for instance $node1 to restart"
  Start-Sleep 15
  $n -= 1
  if ($n -eq 0) {break}

  $creation_status = Get-GceInstance -zone $zone -Name $node1 -SerialPortOutput | 
    Select-String -Pattern 'Finished running shutdown scripts' -Quiet
}

$creation_status = Get-GceInstance -zone $zone -Name $node2 -SerialPortOutput | 
  Select-String -Pattern 'Finished running shutdown scripts' -Quiet
$n = 20
while (!($creation_status)) {
  Write-Host "$(Get-Date) Waiting for instance $node2 to restart"
  Start-Sleep 15
  $n -= 1
  if ($n -eq 0) {break}

  $creation_status = Get-GceInstance -zone $zone -Name $node2 -SerialPortOutput | 
    Select-String -Pattern 'Finished running shutdown scripts' -Quiet
}

# Close the remote sessions
Remove-PSSession $session1
Remove-Variable session1
Remove-PSSession $session2
Remove-Variable session2

# Get-PSSession

# Wait a minute to make sure all services have started
Start-Sleep 60

Write-Host "$(Get-Date) Ready now to create a Windows Failover Cluster (WSFC)"
Write-Host "                    $node1 - $ip_address1"
Write-Host "                    $node2 - $ip_address2"


################################################################################
# Create a Windows Failover Cluster (WSFC) and Availability Group
# Needs to run from a computer member of the domain in order to use CredSSP
################################################################################
If ( Get-ADDomain | Where Forest -eq $domain ) {

  # Only continue if SQL Server is installed
  $node1_sql_server = Get-Service -ComputerName $node1 | 
    Where Name -Like 'MSSQLSERVER*'
  $node2_sql_server = Get-Service -ComputerName $node2 | 
    Where Name -Like 'MSSQLSERVER*'

  If (($node1_sql_server) -and ($node2_sql_server)) {

    ############################################################################
    # Create the Windows Cluster and Availability Group
    # All the code is in another script for portability
    ############################################################################
    #. "C:\Powershell\create-availability-group.ps1""
    . ".\create-availability-group.ps1"
  }
  else {
    Write-Host "Please install SQL Server in both nodes in order to continue."
    Write-Host "Then run the script 'create-availability-group.ps1.'"
  }
}
else {
    Write-Host "To automatically create a Windows Failover Cluster (WSFC) `
	this Powershell script must be run from a computer that is a member of `
	the domain: '$domain'. You will need to setup the WSFC manually."
}