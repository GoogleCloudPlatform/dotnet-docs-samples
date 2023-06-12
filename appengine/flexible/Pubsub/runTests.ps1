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

dotnet restore --force
if ($env:TEST_PROJECT_ID -eq $null) {
	$env:TEST_PROJECT_ID = 'your-project-id'
}
if ($env:TEST_VERIFICATION_TOKEN -eq $null) {
	$env:TEST_VERIFICATION_TOKEN = 'your-token'
}
if ($env:TEST_SUBSCRIPTION_ID -eq $null) {
	$env:TEST_SUBSCRIPTION_ID = 'your-sub-id'
}
if ($env:TEST_AUTH_SUBSCRIPTION_ID -eq $null) {
	$env:TEST_AUTH_SUBSCRIPTION_ID = 'your-auth-sub-id'
}
if ($env:TEST_TOPIC_ID -eq $null) {
	$env:TEST_TOPIC_ID = 'your-topic-id'
}
if ($env:TEST_SERVICE_ACCOUNT_EMAIL -eq $null) {
	$env:TEST_SERVICE_ACCOUNT_EMAIL = 'your-service-account-email'
}
dotnet test --no-restore --test-adapter-path:. --logger:junit 2>&1 | %{ "$_" }
