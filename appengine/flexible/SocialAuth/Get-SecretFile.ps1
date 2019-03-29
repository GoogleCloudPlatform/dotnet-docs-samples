# Copyright (c) 2019 Google LLC.
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
# Decode a Kubernetes secret to a file.
#
#.DESCRIPTION
# Invokes kubectl.
#
#.PARAMETER SecretName
# The Kubernetes secret name.
#
#.PARAMETER FileName
# The name of the file in the Kubernetes secret.
#
#.PARAMETER OutputFileName
# Optional.  The name of the file to write.  Clobbers any existing file.
# If none, then dumps the file contents to stdout.
#
#.EXAMPLE
# .\Get-SecretFile.ps1 my-kubernetes-secret super-secrets.json super-secrets.json
##############################################################################

param(
    [Parameter(Mandatory=$true)][string]$SecretName,
    [Parameter(Mandatory=$true)][string]$FileName,
    [string]$OutputFileName)

$base64text = (kubectl get secret $SecretName -o json | ConvertFrom-Json).data.$FileName
$bytes = [System.Convert]::FromBase64String($base64text)
if ($OutputFileName) {
    [io.file]::WriteAllBytes($OutputFileName, $bytes)
} else {
    [System.Text.Encoding]::UTF8.GetString($bytes)
}