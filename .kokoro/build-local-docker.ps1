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

# Builds a docker image that resembles the environment when 
# tests are run on kokoro.
Push-Location
try {
    # Build the parent docker image.
    Set-Location ./docker
    docker build -t gcr.io/cloud-devrel-kokoro-resources/dotnet .
} finally {
    Pop-Location
}
Push-Location
try {
    # Copy secrets from google cloud storage to a local directory
    Set-Location ./local-docker
    gsutil cp -r gs://cloud-devrel-kokoro-resources/dotnet-docs-samples/ .

    # Build the local docker image.
    docker build --no-cache --build-arg KOKORO_GFILE_DIR=dotnet-docs-samples -t dotnet-docs-samples/kokoro .
} finally {
    Pop-Location
}