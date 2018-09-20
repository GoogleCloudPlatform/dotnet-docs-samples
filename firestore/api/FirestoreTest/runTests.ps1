# Copyright(c) 2018 Google Inc.
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


Import-Module -DisableNameChecking ..\..\..\BuildTools.psm1

dotnet build
if ($LASTEXITCODE) {
	# Failed to build.  Report it.
} else {
	Skip-Test
}
break  # TODO: Fix and run the test below.  It times out.

$randomString = ""
1..10 | foreach {
	$char = [char] (65 + (Get-Random 26) )
	$randomString = $randomString+$char
}
$usersCollection = "kokoro-test-users-"+$randomString
$citiesCollection = "kokoro-test-cities-"+$randomString
$dataCollection = "kokoro-test-data-"+$randomString

BackupAndEdit-TextFile "FirestoreTest.cs","..\Quickstart\Program.cs","..\AddData\Program.cs","..\DataModel\Program.cs","..\DeleteData\Program.cs","..\GetData\Program.cs","..\ManageIndexes\Program.cs","..\OrderLimitData\Program.cs","..\PaginateData\Program.cs","..\QueryData\Program.cs","..\TransactionsAndBatchedWrites\Program.cs" `
	@{'db.Collection("users")' = 'db.Collection("'+$usersCollection+'")';
	  'db.Collection("cities")' = 'db.Collection("'+$citiesCollection+'")';
	  'db.Collection("data")' = 'db.Collection("'+$dataCollection+'")';
	  'DeleteCollection("users")' = 'DeleteCollection("'+$usersCollection+'")';
	  'DeleteCollection("cities")' = 'DeleteCollection("'+$citiesCollection+'")';
	  'DeleteCollection("data")' = 'DeleteCollection("'+$dataCollection+'")';
	  'DeleteCollection("cities/SF/neighborhoods")' = 'DeleteCollection("'+$citiesCollection+'/SF/neighborhoods")';
	  'YOUR_COLLECTION_NAME' = $citiesCollection;
	  } `
{
    dotnet restore
    dotnet test
}