# Copyright (c) 2018 Google LLC.
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

# Build the application.
dotnet publish -c Release

# Generate the source code information context so Stackdriver knows where
# to find the source files.
# See https://cloud.google.com/debugger/docs/source-context for details.
gcloud debug source gen-repo-info-file --output-directory=bin/Release/netcoreapp2.0/publish/

# Deploy to app engine.
gcloud -q beta app deploy bin/Release/netcoreapp2.0/publish/app.yaml
