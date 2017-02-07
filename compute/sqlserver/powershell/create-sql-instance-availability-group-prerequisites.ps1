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
# Run the prerequisites before running the script: 
#   create-sql-instance-availability-group.ps1
#
#.DESCRIPTION
# The script create-sql-instance-availability-group.ps1 assumes you have 
# custom network with 3 subnetworks. It also expects a Windows AD domain.
# 
# This script can be used to create all of those prerequisites
#
# You should run this script one section at a time since not all sections
# may apply in your case
#
#.NOTES
# AUTHOR: Anibal Santiago - @SQLThinker
##############################################################################


################################################################################
# Specify the parameters
################################################################################
# Network configuration
$network      = 'wsfcnet'      # Name of the Custom network
$subnet1_name = 'wsfcsubnet1'  # Name of SubNet 1
$subnet2_name = 'wsfcsubnet2'  # Name of SubNet 2
$subnet3_name = 'wsfcsubnet3'  # Name of SubNet 3 (used for domain controller)
$subnet1 = "10.0.0.0/24"
$subnet2 = "10.1.0.0/24"
$subnet3 = "10.2.0.0/24"

# AD Domain configuration
$DomainName = "dbeng.com";
$DomainMode = "Win2012R2";
$ForestMode = "Win2012R2";
$DatabasePath = "C:\Windows\NTDS";
$LogPath      = "C:\Windows\NTDS";
$SysvolPath   = "C:\Windows\SYSVOL";

# Domain controller/Zone/Region
$domain_cntr    = "dc-windows" # Name of the Windows Domain controller
$ip_domain_cntr = '10.2.0.100' # IP of the Windows Domain controller
$zone   = "us-central1-f"
$region = "us-central1"


# Get the passwords for Domain Admin and SQL Server service account
if (!($admin_pwd)) {
  $admin_pwd = Read-Host -AsSecureString "Password for Domain Administrator"
}


################################################################################
# Create the custom network with 3 subnetworks
################################################################################
Write-Host "$(Get-Date) Create custom network with 3 subnetworks"
$ErrorActionPreference = 'continue' #To skip non-error reported by PowerShell
Invoke-Command -ScriptBlock {

  if ( !( Get-GceNetwork | Where Name -eq $network ) ) {
    gcloud compute networks create $network --mode custom
  } else {
    Write-Host "  The network: $network already exist"
  }
  
  if ( !( Get-GceNetwork | Where {$_.Name -eq $network -and `
    $_.Subnetworks -like "*$subnet1_name" } ) ) {

    gcloud compute networks subnets create $subnet1_name `
      --network $network `
      --region $region `
      --range $subnet1
  } else {
     Write-Host "  The subnetwork: $subnet1_name already exist"
  }
  
  if ( !( Get-GceNetwork | Where {$_.Name -eq $network -and `
    $_.Subnetworks -like "*$subnet1_name" } ) ) {

    gcloud compute networks subnets create $subnet2_name `
      --network $network `
      --region $region `
      --range $subnet2
  } else {
     Write-Host "  The subnetwork: $subnet2_name already exist"
  }
  
  if ( !( Get-GceNetwork | Where {$_.Name -eq $network -and `
    $_.Subnetworks -like "*$subnet1_name" } ) ) {

    gcloud compute networks subnets create $subnet3_name `
      --network $network `
      --region $region `
      --range $subnet3
  } else {
     Write-Host "  The subnetwork: $subnet2_name already exist"
  }
} 2> $null
$ErrorActionPreference = 'Stop'


################################################################################
# Create the firewall rules
################################################################################
Write-Host "$(Get-Date) Create firewall: Allow RDP, WinRM and Internal Traffic"

# Find the public IP and network URL
$myip = Invoke-RestMethod http://ipinfo.io/json | Select -ExpandProperty ip
$network_url = Get-GceNetwork -Name $network | Select -ExpandProperty SelfLink

# Allow all connections between the instances
$tcp  = New-GceFirewallProtocol -IPProtocol tcp -Port 1-65535
$udp  = New-GceFirewallProtocol -IPProtocol udp -Port 1-65535
$icmp = New-GceFirewallProtocol -IPProtocol icmp

if ( !( Get-GceFirewall | Where {$_.Name -eq "allow-internal-ports"} ) ) {
  Add-GceFirewall `
    -Name "allow-internal-ports" `
    -Network $network_url `
    -SourceRange $subnet1,$subnet2,$subnet3 `
    -AllowedProtocol $tcp, $udp, $icmp 
} else {
  Write-Host "  The firewall: allow-internal-ports already exist"
}

# Allow RDP from any IP address
<#
$tcp = New-GceFirewallProtocol -IPProtocol tcp -Port 3389

if ( !( Get-GceFirewall | Where {$_.Name -eq "allow-rdp"} ) ) {
  Add-GceFirewall `
    -Name "allow-rdp" `
    -Network $network_url `
    -SourceRange "0.0.0.0/0" `
    -AllowedProtocol $tcp 
} else {
  Write-Host "  The firewall: allow-rdp already exist"
}
#>

# Allow WinRM from any IP address
<#
$tcp = New-GceFirewallProtocol -IPProtocol tcp -Port 5986

if ( !( Get-GceFirewall | Where {$_.Name -eq "allow-winrm"} ) ) {
Add-GceFirewall `
  -Name "allow-winrm" `
  -Network $network_url `
  -SourceRange "0.0.0.0/0" `
  -AllowedProtocol $tcp 
} else {
  Write-Host "  The firewall: allow-winrm already exist"
}
#>

# Allow WinRM from current IP
$tcp = New-GceFirewallProtocol -IPProtocol tcp -Port 5986

if ( !( Get-GceFirewall | Where {$_.Name -eq "allow-winrm-current-ip"} ) ) {
  #Remove-GceFirewall -FirewallName "allow-winrm-current-ip"
  Add-GceFirewall `
    -Name "allow-winrm-current-ip" `
    -Network $network_url `
    -SourceRange "$($myip)" `
    -AllowedProtocol $tcp
} else {
  Write-Host "  The firewall: allow-winrm-current-ip already exist"
}


# Allow RDP from current IP
$tcp = New-GceFirewallProtocol -IPProtocol tcp -Port 3389

if ( !( Get-GceFirewall | Where {$_.Name -eq "allow-rdp-current-ip"} ) ) {
  #Remove-GceFirewall -FirewallName "allow-rdp-current-ip"
  Add-GceFirewall `
    -Name "allow-rdp-current-ip" `
    -Network $network_url `
    -SourceRange "$($myip)" `
    -AllowedProtocol $tcp
} else {
  Write-Host "  The firewall: allow-rdp-current-ip already exist"
}




################################################################################
# Create a windows instance to be the domain controller
# Then create the domain and assign a password for the domain admin
################################################################################
Write-Host "$(Get-Date) Create Instance: $domain_cntr"
$ErrorActionPreference = 'continue' #To skip non-error reported by PowerShell

if ( !( Get-GceInstance | Where {$_.Name -eq $domain_cntr} ) ) {

  Invoke-Command -ScriptBlock {
    gcloud compute instances create $domain_cntr `
      --machine-type n1-standard-1 `
      --boot-disk-type pd-standard `
      --image-project windows-cloud `
      --image-family windows-2012-r2 `
      --boot-disk-size 200GB `
      --zone $zone `
      --subnet $subnet3_name `
      --private-network-ip=$ip_domain_cntr; 
  } 2> $null

  # Wait until the instance is configured
  $creation_status = Get-GceInstance -zone $zone -Name $domain_cntr -SerialPortOutput | 
    Select-String -Pattern 'Finished running startup scripts' -Quiet

  while (!($creation_status)) {
    Write-Host "$(Get-Date) Waiting for instance $domain_cntr to be created"
    Start-Sleep 15
    $creation_status = Get-GceInstance -zone $zone -Name $domain_cntr -SerialPortOutput |
      Select-String -Pattern 'Finished running startup scripts' -Quiet
  }

  Write-Host "$(Get-Date) Instance $domain_cntr is now created"


  ################################################################################
  # Generate a password in the domain controller for username config.user
  ################################################################################
  Write-Host "$(Get-Date) Create/reset password for config.user in $domain_cntr"
  $ErrorActionPreference = 'continue' #To skip non-error reported by PowerShell

  $results = Invoke-Command -ScriptBlock { 
    gcloud compute reset-windows-password dc-windows `
      --zone $zone `
      --user "config.user" `
      --quiet 2> $null
  } | ConvertFrom-String
  
  $ErrorActionPreference = 'Stop'

  # Grab the IP and Password from the results of the 'gcloud.exe' command
  $ip_address = $results.P2[0]
  $password = ConvertTo-SecureString -String $results.P2[1] -AsPlainText -Force
  $credential = New-Object System.Management.Automation.PSCredential("config.user", $password)


  ################################################################################
  # Create a remote session to the domain controller
  ################################################################################
  $session_options = New-PSSessionOption -SkipCACheck -SkipCNCheck `
    -SkipRevocationCheck
  $session = New-PSSession -ComputerName $ip_address -UseSSL `
    -Credential $credential -SessionOption $session_options


  ################################################################################
  # Create the Windows domain dbeng.com
  ################################################################################
  Write-Host "$(Get-Date) Promote $domain_cntr to a Domain Controller"
  Invoke-Command -Session $session -ScriptBlock {
    param($admin_pwd, $DomainName, $DomainMode, $DatabasePath, $LogPath, `
      $SysvolPath, $ForestMode)

    # Find information about the instance
    $computer_info = Get-WmiObject -Class Win32_ComputerSystem

    # Setup the computer as domain controller, only if not setup yet
    if ( $computer_info.Domain -eq "WORKGROUP" ) {
      Install-WindowsFeature -Name AD-Domain-Services -IncludeManagementTools

      Install-ADDSForest -CreateDnsDelegation:$false `
        -DatabasePath $DatabasePath `
        -LogPath $LogPath `
        -SysvolPath $SysvolPath `
        -DomainName $DomainName `
        -DomainMode $DomainMode `
        -ForestMode $ForestMode `
        -InstallDNS:$true `
        -NoRebootOnCompletion:$false `
        -SafeModeAdministratorPassword $admin_pwd `
        -Force:$true

       Write-Host "$(Get-Date) Rebooting the instance"
     }
     else {
       Write-Host "  The server instance is already a member of a domain"
     }
  } -ArgumentList $admin_pwd, $DomainName, $DomainMode, $DatabasePath, $LogPath, `
    $SysvolPath, $ForestMode


  # Remove the session object
  Remove-PSSession $session
  Remove-Variable session


  ################################################################################
  # Wait until the instances is restarted
  ################################################################################
  $creation_status = Get-GceInstance -zone $zone -Name $domain_cntr -SerialPortOutput | 
    Select-String -Pattern 'Finished running shutdown scripts' -Quiet
  while (!($creation_status)) {
    Write-Host "$(Get-Date) Waiting for instance $domain_cntr to restart"
    Start-Sleep 15

    $creation_status = Get-GceInstance -zone $zone -Name $domain_cntr -SerialPortOutput | 
      Select-String -Pattern 'Finished running shutdown scripts' -Quiet
  }

  # Wait a minute to make sure all AD services are running before we continue
  Start-Sleep 60

  ################################################################################
  # Create a remote session to the domain controller
  ################################################################################
  $session_options = New-PSSessionOption -SkipCACheck -SkipCNCheck `
    -SkipRevocationCheck
  $session = New-PSSession -ComputerName $ip_address -UseSSL `
    -Credential $credential -SessionOption $session_options


  ################################################################################
  # Change the password for the domain administrator
  ################################################################################
  Write-Host "$(Get-Date) Change password for Administrator and sql.service"
  Invoke-Command -Session $session -ScriptBlock {
    param($admin_pwd, $sql_pwd)
  
    Set-ADAccountPassword `
      -Identity "Administrator" `
      -Reset `
      -NewPassword $admin_pwd

  } -ArgumentList $admin_pwd, $sql_pwd


  # Remove the session object
  Remove-PSSession $session
  Remove-Variable session

} else {
  Write-Host "  The instance: $domain_cntr already exist"
}


Write-Host " "
Write-Host "IMPORTANT: You are now ready to run the script  create-sql-instance-availability-group.ps1"
Write-Host "           It is recommended to run it from an instance that is a member of the domain: $DomainName"
Write-Host "           if you want to automatically setup the SQL Server Availability Group."

# Remove firewall rule to allow current IP
#Get-GceFirewall -FirewallName "allow-winrm-current-ip" | Remove-GceFirewall

##### Cleanup - Run with caution as everything will be deleted #####
<#
$ErrorActionPreference = 'continue'

# Delete instance
Get-GceInstance -Zone $zone | Where Name -EQ $domain_cntr | Remove-GceInstance

# Delete the 3 subnetworks
Invoke-Command -ScriptBlock { gcloud compute networks subnets delete wsfcsubnet1 --quiet 2> $null }
Invoke-Command -ScriptBlock { gcloud compute networks subnets delete wsfcsubnet2 --quiet 2> $null }
Invoke-Command -ScriptBlock { gcloud compute networks subnets delete wsfcsubnet3 --quiet 2> $null } 

# Delete the firewall rules, routes and custom network
Get-GceFirewall | Where-Object Network -Like "*wsfcnet" | Remove-GceFirewall
Get-GceRoute | Where-Object Network -Like "*wsfcnet" | Remove-GceRoute
Invoke-Command -ScriptBlock { gcloud compute networks delete wsfcnet --quiet 2> $null } 

#>

