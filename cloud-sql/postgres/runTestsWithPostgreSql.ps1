# Copyright (c) 2019 Google LLC.
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

Import-Module -DisableNameChecking ..\..\BuildTools.psm1

if ($IsRunningOnWindows) { # b/352406852
    # Download and run the sql proxy.
    if (-not (Test-Path cloud_sql_proxy.exe)) {
    	if ($PSVersionTable.Platform -eq 'Unix') {
    		Invoke-WebRequest -Uri 'https://dl.google.com/cloudsql/cloud_sql_proxy.linux.amd64' -OutFile cloud_sql_proxy.exe
    		chmod +x cloud_sql_proxy.exe
    	} else {
    		Invoke-WebRequest -Uri 'https://dl.google.com/cloudsql/cloud_sql_proxy_x64.exe' -OutFile cloud_sql_proxy.exe
    	}
    }
    $proxy = Start-Job -ArgumentList (Get-Location) -ScriptBlock {
    	Set-Location $args[0]
    	./cloud_sql_proxy.exe --instances=$env:TEST_CLOUDSQL2_POSTGRESQL_INSTANCE=tcp:5432 `
    		--credential_file=$GOOGLE_APPLICATION_CREDENTIALS
    }
    $env:INSTANCE_HOST=$env:TEST_CLOUDSQL2_HOST
    $env:DB_USER=$env:TEST_CLOUDSQL2_USER
    $env:DB_PASS=$env:TEST_CLOUDSQL2_VOTES_PASS
    $env:DB_NAME=$env:TEST_CLOUDSQL2_VOTES_NAME
    
    try {
    	dotnet restore --force
    	Receive-Job $proxy -ErrorAction 'Continue'
    	dotnet build --no-restore
    	Receive-Job $proxy -ErrorAction 'Continue'
    	try {
    		Run-KestrelTest 5567 -CasperJs11
    	} finally {
    		Move-TestResults PostgreSql
    	}
    
    } finally {
    	Stop-Job $proxy
    	Receive-Job $proxy -ErrorAction 'Continue'
    	Remove-Job $proxy
    }
}
