# Copyright(c) 2017 Google Inc.
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

$secrets = @('Authentication:Google:ClientID',
	'Authentication:Google:ClientSecret',
	'ConnectionStrings:DefaultConnection',
	'Authentication:Facebook:AppId',
	'Authentication:Facebook:AppSecret') -join '|'

$secretLine = "info: ($secrets) = (.*)"

foreach ($line in (dotnet user-secrets list )) {
	if ($line -match $secretLine ) {
		$key = $matches[1].replace(':', '-')
		$value = $matches[2]
		echo "gcloud compute project-info add-metadata --metadata=$key=..."
		gcloud compute project-info add-metadata --metadata=$key=$value
	}
}
