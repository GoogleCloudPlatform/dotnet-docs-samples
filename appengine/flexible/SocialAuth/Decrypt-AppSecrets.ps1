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

##############################################################################
#.SYNOPSIS
# Decrypts appsecrets.json.encrypted.
#
#.DESCRIPTION
# Uses the kms key named by appsecrets.json.keyname to decrypt 
# appsecrets.json.encrypted.  Overwrites file appsecrets.json.
#
#.EXAMPLE
# .\Decrypt-AppSecrets.ps1
##############################################################################
gcloud kms decrypt --plaintext-file appsecrets.json --ciphertext-file appsecrets.json.encrypted --key (Get-Content appsecrets.json.keyname) 
