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
# Specify parameters for these PowerShell scripts: 
#   create-sql-instance-availability-group.ps1, create-availability-group.ps1
#
#.DESCRIPTION
# This script is used to import parameter definitions into the scripts:
# create-sql-instance-availability-group.ps1 and create-availability-group.ps1
# Normally it will never be run by itself, but imported from another script.
#
#.NOTES
# AUTHOR: Anibal Santiago - @SQLThinker
##############################################################################
cls
Set-Location $PSScriptRoot

#####################################################################################
# We assume you have created a network in Google Cloud called wsfcnet
# with two subnetworks wsfcsubnet1 and wsfcsubnet2
#####################################################################################
$force_delete   = $False                 # If $True delete the nodes if they exist

# Node and subnet parameters
$node1          = 'cluster-sql1'         # Name of node 1
$node2          = 'cluster-sql2'         # Name of node 2
$network        = 'wsfcnet'              # Name of the Custom network in GCP. Must already be created in Google Cloud
$subnet1_name   = 'wsfcsubnet1'          # Name of Subnet 1. Must already be created in Google Cloud. (Ex. 10.0.0.0/16)
$subnet2_name   = 'wsfcsubnet2'          # Name of Subnet 2. Must already be created in Google Cloud. (Ex. 10.1.0.0/16)
$size_gb        = '200'                  # Size in GB for the boot disk of the nodes
$disk_type      = 'pd-standard'          # Type of disk: pd-ssd (recommended) or pd-standard (only option in free trial account)

# Windows Cluster parameters
$name_wsfc       = 'cluster-dbclus'      # Name for the Windows Failover Cluster (WSFC)

$ip_node1        = '10.0.0.4'            # IP address of node 1 in Subnet 1
$ip_wsfc1        = '10.0.0.5'            # IP address of WSFC in Subnet 1        (choose from $ip_alias1 address range)
$ip_ag_listener1 = '10.0.0.6'            # IP address of AG listener in Subnet 1 (choose from $ip_alias1 address range)

$ip_node2        = '10.1.0.4'            # IP address of node 2 in Subnet 2
$ip_wsfc2        = '10.1.0.5'            # IP address of WSFC in Subnet 2        (choose from $ip_alias2 address range)
$ip_ag_listener2 = '10.1.0.6'            # IP address of AG listener in Subnet 2 (choose from $ip_alias2 address range)

$name_ag         = 'cluster-ag'          # Name of the SQL Server Availability Group
$name_ag_listener= 'ag-listener'         # Name of the SQL Server Availability Group Listener (15 characters maximum)


# Domain/user parameters
$config_user     = 'Config.User'          # Local user to create in the instance. It will be used for the initial remote connection.
$ip_domain_cntr  = '10.2.0.100'           # IP address of the Windows Domain controller. This will be used as the local DNS server.
$domain_cntr     = 'dc-windows'           # Hostname of the domain controller. Will be used when creating a shared folder for Quorum.
$domain          = 'dbeng.com'            # Name of the Windows Active Directory domain
$domain_netbios  = 'dbeng'                # Name of the Windows Active Directory domain in Netbios (15 characters max)
$domain_cred     = 'dbeng\Administrator'  # Account to use to add computers into the domain
#$domain_pwd      = 'MyPassword'          # Password for the $domain_cred account. Leave commented to prompt the user for a password.


# Database parameters
$db_name        = 'TestDB'               # Name of the database to create
$sql_data       = 'C:\SQLData'           # Directory to store the database data files
$sql_log        = 'C:\SQLLog'            # Directory to store the database transaction log files
$sql_backup     = 'C:\SQLBackup'         # Directory to store the backup of the database for the initial synchronization of Availability Groups
$share_name     = 'SQLBackup'            # Share name for the backup folder (\\node2\SQLBackup. Points to the $sql_backup variable)
$data_size      = 1024                   # Initial size of the database in MB
$data_growth    = 256                    # Auto growth size of the database in MB
$log_size       = 1024                   # Initial size of the transaction log in MB
$log_growth     = 256                    # Auto growth size of the transaction log in MB


# Image Family for the instance. Uncomment only one of the variables below
# Check the command: gcloud compute images list --format='table(family)' | findstr sql
$image_family = 'sql-std-2017-win-2019'
  # 'sql-ent-2012-win-2012-r2'
  # 'sql-std-2012-win-2012-r2'
  # 'sql-web-2012-win-2012-r2'
  # 'sql-ent-2014-win-2012-r2'
  # 'sql-ent-2014-win-2016'
  # 'sql-std-2014-win-2012-r2'
  # 'sql-web-2014-win-2012-r2'
  # 'sql-ent-2016-win-2012-r2'
  # 'sql-ent-2016-win-2016'
  # 'sql-ent-2016-win-2019'
  # 'sql-std-2016-win-2012-r2'
  # 'sql-std-2016-win-2016'
  # 'sql-std-2016-win-2019'
  # 'sql-web-2016-win-2012-r2'
  # 'sql-web-2016-win-2016'
  # 'sql-web-2016-win-2019'
  # 'sql-ent-2017-win-2016'
  # 'sql-ent-2017-win-2019'
  # 'sql-exp-2017-win-2012-r2'
  # 'sql-exp-2017-win-2016'
  # 'sql-exp-2017-win-2019'
  # 'sql-std-2017-win-2016'
  # 'sql-std-2017-win-2019'
  # 'sql-web-2017-win-2016'
  # 'sql-web-2017-win-2019'


# Machine Type. Uncomment only one of the variables below.
# Check the command: gcloud compute machine-types list --filter="ZONE:us-central1-f" --format='table(name, description)'
$machine_type = 'n1-standard-4'
  # 'f1-micro'       # 1 vCPU (shared physical core) and 0.6 GB RAM
  # 'g1-small'       # 1 vCPU (shared physical core) and 1.7 GB RAM
  # 'n1-highcpu-16'  # 16 vCPUs, 14.4 GB RAM
  # 'n1-highcpu-2'   # 2 vCPUs, 1.8 GB RAM
  # 'n1-highcpu-32'  # 32 vCPUs, 28.8 GB RAM
  # 'n1-highcpu-4'   # 4 vCPUs, 3.6 GB RAM
  # 'n1-highcpu-64'  # 64 vCPUs, 57.6 GB RAM
  # 'n1-highcpu-8'   # 8 vCPUs, 7.2 GB RAM
  # 'n1-highcpu-96'  # 96 vCPUs, 86 GB RAM
  # 'n1-highmem-16'  # 16 vCPUs, 104 GB RAM
  # 'n1-highmem-2'   # 2 vCPUs, 13 GB RAM
  # 'n1-highmem-32'  # 32 vCPUs, 208 GB RAM
  # 'n1-highmem-4'   # 4 vCPUs, 26 GB RAM
  # 'n1-highmem-64'  # 64 vCPUs, 416 GB RAM
  # 'n1-highmem-8'   # 8 vCPUs, 52 GB RAM
  # 'n1-highmem-96'  # 96 vCPUs, 624 GB RAM
  # 'n1-standard-1'  # 1 vCPU, 3.75 GB RAM
  # 'n1-standard-16' # 16 vCPUs, 60 GB RAM
  # 'n1-standard-2'  # 2 vCPUs, 7.5 GB RAM
  # 'n1-standard-32' # 32 vCPUs, 120 GB RAM
  # 'n1-standard-4'  # 4 vCPUs, 15 GB RAM
  # 'n1-standard-64' # 64 vCPUs, 240 GB RAM
  # 'n1-standard-8'  # 8 vCPUs, 30 GB RAM
  # 'n1-standard-96' # 96 vCPUs, 360 GB RAM

# Zone where the instance will be located. Uncomment only one of the variables below.
# Check the command: gcloud compute zones list --format='table(name)'
$zone = 'us-east1-b'
  # 'asia-east1-a'
  # 'asia-east1-b'
  # 'asia-east1-c'
  # 'europe-west1-b'
  # 'europe-west1-c'
  # 'europe-west1-d'
  # 'us-central1-a'
  # 'us-central1-b'
  # 'us-central1-c'
  # 'us-central1-f'
  # 'us-east1-b'
  # 'us-east1-c'
  # 'us-east1-d'
  # 'us-west1-a'
  # 'us-west1-b'


# Create a credential object for the domain admin account. It will be used to add the instances to the domain later on.
# Skip if running in Node 1 as we copy this file to Node 1 to create the WSFC locally. No need to create the Credential
# if running locally.
$hostname = [System.Net.Dns]::GetHostName()

if ( $hostname.ToLower() -ne $node1.ToLower() ) {
  if (!($domain_pwd)) {
    $cred = Get-Credential -Credential $domain_cred }
  else {
    $Pwd = ConvertTo-SecureString $domain_pwd -AsPlainText -Force 
    $cred = New-Object System.Management.Automation.PSCredential $domain_cred, $Pwd
  }
}
