# Copyright 2020 Google LLC
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

Require-Platform Win*

& nuget.exe restore 2>&1 | %{ "$_" }
& msbuild /t:clean /t:build "CloudDemo.Mvc\CloudDemo.Mvc.csproj" /p:Configuration=Release /p:DeployOnBuild=true /p:PublishProfile=FolderProfile /p:OutputPath=bin 2>&1 | %{ "$_" }
