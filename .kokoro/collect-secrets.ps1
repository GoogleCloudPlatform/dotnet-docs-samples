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

function Collect-Secrets() {
    $envVars = Get-ChildItem env:
    foreach ($var in $envVars) {
        $name = $var.Name
        if ($name -like 'TEST_*' -or $name -like 'GOOGLE*') {
            $value = $var.Value
            echo "`$env:$name = '$value'"
        }
    }
}

Collect-Secrets