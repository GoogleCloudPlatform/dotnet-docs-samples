#!/bin/bash

# Copyright 2022 Google Inc. All Rights Reserved.
#
# Licensed under the Apache License, Version 2.0 (the "License");
# you may not use this file except in compliance with the License.
# You may obtain a copy of the License at
#
#      http://www.apache.org/licenses/LICENSE-2.0
#
# Unless required by applicable law or agreed to in writing, software
# distributed under the License is distributed on an "AS IS" BASIS,
# WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
# See the License for the specific language governing permissions and
# limitations under the License.

# set the key as GOOGLE_APPLICATION_CREDENTIALS
export GOOGLE_APPLICATION_CREDENTIALS=~/key.json

# Run the sample for creating the GCS bucket and extract the output of that execution
cd ~/cloudshell_open/dotnet-docs-samples/retail/interactive-tutorial/RetailProducts.Samples
output=$(dotnet run -- ProductsCreateGcsBucket)

# Get the bucket name and store it in the env variable BUCKET_NAME
temp="${output#*Created bucket }"
bucket_name="${temp% in*}"
export BUCKET_NAME=$bucket_name

# Import products to the Retail catalog
dotnet run -- ImportProductsGcsSample

echo "====================================="
echo "Your Retail catalog is ready to use!"
echo "====================================="
