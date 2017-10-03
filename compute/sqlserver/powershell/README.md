# PowerShell examples for SQL Server

In this folder you will find some examples on how to use PowerShell to manage SQL Server in Google Cloud Platform (GCP)

## Creating a SQL Server Availability Group

The following PowerShell scripts will help you setup a SQL Server Availability Group. 

  * [parameters-config.ps1](parameters-config.ps1) - Specify the parameters that are used by the other scripts.
  * [create-sql-instance-availability-group.ps1](create-sql-instance-availability-group.ps1) - **Usually this is the only script that needs to be run**. It does the following steps:
      1. Creates two SQL Server instances
      2. Adds the two instances to a Windows AD domain
      3. Creates a Windows Server Failover Cluster (WSFC)
      4. Creates a SQL Server Availability Group
  * [create-availability-group.ps1](create-availability-group.ps1) - Script to create the Availability Group. It is called by the script *create-sql-instance-availability-group.ps1*.
  * [create-sql-instance-availability-group-prerequisites.ps1](create-sql-instance-availability-group-prerequisites.ps1) - (Optional) Set up the pre-requisites needed to create an Availability Group:
     1. Creates a Custom Network in GCP
     2. Creates firewall rules for WinRM and RDP
     3. Creates a Windows AD domain with one domain controller
  * [create-sql-instance-availability-group-cleanup.ps1](create-sql-instance-availability-group-cleanup.ps1) - (Optional) Delete the SQL Server instances, the domain controller and the custom network. *WARNING: Use with caution. Use in cases where you are just testing the scripts*.
  * [create-sql-instance-availability-group.Tests.ps1](create-sql-instance-availability-group.Tests.ps1) - (Optional) Use Pester to unit test the scripts.
