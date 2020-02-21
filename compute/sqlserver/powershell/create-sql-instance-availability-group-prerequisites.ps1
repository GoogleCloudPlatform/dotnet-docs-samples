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
#
##############################################################################
#.SYNOPSIS
# Run the prerequisites before running the script: 
#   create-sql-instance-availability-group.ps1
#
#.DESCRIPTION
# The script create-sql-instance-availability-group.ps1 assumes you have a
# custom network with 3 subnetworks. It also expects a Windows AD domain.
# This script can be used to create all of those prerequisites.
# 
# For more information see "Configuring SQL Server Availability Groups" at:
# https://cloud.google.com/compute/docs/instances/sql-server/configure-availability
#
# You should run this script one section at a time since not all sections
# may apply in your case
#
#.NOTES
# AUTHOR: Anibal Santiago - @SQLThinker
##############################################################################

Set-Location $PSScriptRoot
################################################################################
# Specify the parameters:
# You can change these parameters to match your requirements.
################################################################################
# Network names
$network      = 'wsfcnet'      # Name of the Custom network
$subnet1_name = 'wsfcsubnet1'  # Name of Subnet 1. Node 1 will reside here.
$subnet2_name = 'wsfcsubnet2'  # Name of Subnet 2. Node 2 will reside here.
$subnet3_name = 'wsfcsubnet3'  # Name of Subnet 3. The domain controller will reside here but it can also be one of the other 2 subnets.

# Create 3 subnets. One for each node and the third one for the domain controller. Change as appropiate to match your network requirements.
$subnet1 = "10.0.0.0/24" # Subnet1 netmask
$subnet2 = "10.1.0.0/24" # Subnet2 netmask
$subnet3 = "10.2.0.0/24" # Subnet3 netmask

$region1 = "us-east1"    # Subnet1 region
$region2 = "us-east1"    # Subnet2 region
$region3 = "us-east4"    # Subnet3 region


# AD Domain configuration
$DomainName   = "dbeng.com";
$DomainMode   = "Win2012R2";
$ForestMode   = "Win2012R2";
$DatabasePath = "C:\Windows\NTDS";
$LogPath      = "C:\Windows\NTDS";
$SysvolPath   = "C:\Windows\SYSVOL";

# Domain controller configuration
$domain_cntr                = "dc-windows"      # Name of the Windows Domain controller
$ip_domain_cntr             = '10.2.0.100'      # IP of the Windows Domain controller
$zone_domain_cntr           = "us-east4-b"      # Zone for domain controller (needs to be within $region3)
$machine_type_domain_cntr   = "n1-standard-1"   # Machine Type
$boot_disk_type_domain_cntr = "pd-standard"     # Boot disk type (pd-ssd, pd-standard)
$image_family_domain_cntr   = "windows-2012-r2" # Windows Server image family (windows-2012-r2, windows-2016)
$boot_disk_size_domain_cntr = "200GB"           # Size of the boot disk

# Get the password for Domain Admin
if (!($domain_pwd)) {
  $admin_pwd = Read-Host -AsSecureString "Password for Domain Administrator" }
else {
  $admin_pwd = ConvertTo-SecureString $domain_pwd -AsPlainText -Force
}


################################################################################
# Create the custom network with 3 subnetworks
################################################################################
Write-Host "$(Get-Date) Create custom network with 3 subnetworks"
$ErrorActionPreference = 'continue' #To skip non-error reported by PowerShell
Invoke-Command -ScriptBlock {

  if ( !( Get-GceNetwork | Where Name -eq $network ) ) {
    gcloud compute networks create $network --subnet-mode custom
    # Remove-GceNetwork -Name $network
    # New-GceNetwork -Name $network -IPv4Range 10.0.0.0/22
  } else {
    Write-Host "  The network: $network already exist"
  }
  
  if ( !( Get-GceNetwork | Where {$_.Name -eq $network -and `
    $_.Subnetworks -like "*$subnet1_name" } ) ) {

    gcloud compute networks subnets create $subnet1_name `
      --network $network `
      --region $region1 `
      --range $subnet1
  } else {
     Write-Host "  The subnetwork: $subnet1_name already exist"
  }
  
  if ( !( Get-GceNetwork | Where {$_.Name -eq $network -and `
    $_.Subnetworks -like "*$subnet2_name" } ) ) {

    gcloud compute networks subnets create $subnet2_name `
      --network $network `
      --region $region2 `
      --range $subnet2
  } else {
     Write-Host "  The subnetwork: $subnet2_name already exist"
  }
  
  if ( !( Get-GceNetwork | Where {$_.Name -eq $network -and `
    $_.Subnetworks -like "*$subnet3_name" } ) ) {

    gcloud compute networks subnets create $subnet3_name `
      --network $network `
      --region $region3 `
      --range $subnet3
  } else {
     Write-Host "  The subnetwork: $subnet3_name already exist"
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
# Then create the domain and assign a password for the domain administrator
################################################################################
Write-Host "$(Get-Date) Create Instance: $domain_cntr"
$ErrorActionPreference = 'continue' #To skip non-error reported by PowerShell

if ( !( Get-GceInstance | Where {$_.Name -eq $domain_cntr} ) ) {

  Invoke-Command -ScriptBlock {
    gcloud compute instances create $domain_cntr `
      --machine-type $machine_type_domain_cntr `
      --boot-disk-type $boot_disk_type_domain_cntr `
      --image-project windows-cloud `
      --image-family $image_family_domain_cntr `
      --boot-disk-size $boot_disk_size_domain_cntr `
      --zone $zone_domain_cntr `
      --subnet $subnet3_name `
      --private-network-ip $ip_domain_cntr;
  } 2> $null

  # Wait until the instance is configured
  # We wait for the text 'Instance setup finished' to show in the Serial Port
  $creation_status = Get-GceInstance -zone $zone_domain_cntr -Name $domain_cntr -SerialPortOutput | 
    Select-String -Pattern 'Instance setup finished' -Quiet

  while (!($creation_status)) {
    Write-Host "$(Get-Date) Waiting for instance $domain_cntr to be ready"
    Start-Sleep -s 30
    $creation_status = Get-GceInstance -zone $zone_domain_cntr -Name $domain_cntr -SerialPortOutput |
      Select-String -Pattern 'Instance setup finished' -Quiet
  }

  Write-Host "$(Get-Date) Instance $domain_cntr is now ready"


  ################################################################################
  # Generate a password in the domain controller for username config.user
  ################################################################################
  Write-Host "$(Get-Date) Create/reset password for config.user in $domain_cntr"
  $ErrorActionPreference = 'continue' #To skip non-error reported by PowerShell

  $results = Invoke-Command -ScriptBlock { 
    gcloud compute reset-windows-password $domain_cntr `
      --zone $zone_domain_cntr `
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
  # Enable the local Administrator account. It is needed to create a domain.
  ################################################################################
  Invoke-Command -Session $session -ScriptBlock {
    Write-Host "$(Get-Date) Enabling the Administrator account"

    $admin_user = ([ADSI]'WinNT://localhost/Administrator,user')
    if ($admin_user.userflags.value -band "0x0002"){
      $admin_user.userflags.value = $admin_user.userflags.value -bxor "0x0002"
    }

    # Generate a 36 characters random password
    $password = -join(33..122 | %{[char]$_} | Get-Random -C 36)
    $admin_user.SetPassword($password)
    $admin_user.SetInfo()
  }


  ################################################################################ 
  # Create the Windows domain
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
  # Wait until the instance is restarted
  # Notice we wait for the text 'Starting shutdown scripts' in Serial Console
  ################################################################################
  $creation_status = Get-GceInstance -zone $zone_domain_cntr -Name $domain_cntr -SerialPortOutput | 
    Select-String -Pattern 'Starting shutdown scripts' -Quiet
  while (!($creation_status)) {
    Write-Host "$(Get-Date) Waiting for instance $domain_cntr to restart"
    Start-Sleep -s 30

    $creation_status = Get-GceInstance -zone $zone_domain_cntr -Name $domain_cntr -SerialPortOutput | 
      Select-String -Pattern 'Starting shutdown scripts' -Quiet
  }

  # Wait 2 minutes to make sure all AD services are running before we continue
  Start-Sleep -s 120

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
  Write-Host "$(Get-Date) Change password for Administrator"
  Invoke-Command -Session $session -ScriptBlock {
    param($admin_pwd)
  
    Set-ADAccountPassword `
      -Identity "Administrator" `
      -Reset `
      -NewPassword $admin_pwd

  } -ArgumentList $admin_pwd


  # Remove the session object
  Remove-PSSession $session
  Remove-Variable session

} else {
  Write-Host "  The instance: $domain_cntr already exist"
}


Write-Host " "
Write-Host "IMPORTANT: You are now ready to run the script: create-sql-instance-availability-group.ps1"


# Remove firewall rules that allow current IP to RDP and WinRM
# Uncomment if you don't want to keep those firewall rules
<#
if ( ( Get-GceFirewall | Where {$_.Name -eq "allow-winrm-current-ip"} ) ) {
  Remove-GceFirewall -FirewallName "allow-winrm-current-ip"
}

if ( ( Get-GceFirewall | Where {$_.Name -eq "allow-rdp-current-ip"} ) ) {
  Remove-GceFirewall -FirewallName "allow-rdp-current-ip"
}
#>
