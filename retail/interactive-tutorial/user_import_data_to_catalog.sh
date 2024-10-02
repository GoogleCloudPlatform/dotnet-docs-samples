#!/bin/bash

# Copyright 2022 Google Inc.
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

failure() {
    echo "========================================="
    echo "The Google Cloud setup was not completed."
    echo "Please fix the errors above!"
    echo "========================================="
    exit 0
}

# catch any error that happened during execution
trap 'failure' ERR

# Create a GCS bucket and upload the product data to the bucket.
cd ~/cloudshell_open/dotnet-docs-samples/retail/interactive-tutorial/RetailProducts.Samples
output=$(dotnet run -- ProductsCreateGcsBucket)

# Get the bucket name and store it in the env variable RETAIL_BUCKET_NAME.
temp="${output#*Bucket name: }"
bucket_name="${temp%$'\n\n\n'Created*}"
export RETAIL_BUCKET_NAME=$bucket_name

# Import products to the Retail catalog.
dotnet run -- ImportProductsGcsTutorial

echo "====================================="
echo "Your Retail catalog is ready to use!"
echo "====================================="
