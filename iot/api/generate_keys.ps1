# Copyright 2019 Google Inc.
#
# Licensed under the Apache License, Version 2.0 (the "License");
# you may not use this file except in compliance with the License.
# You may obtain a copy of the License at
#
#     http://www.apache.org/licenses/LICENSE-2.0
#
# Unless required by applicable law or agreed to in writing, software
# distributed under the License is distributed on an "AS IS" BASIS,
# WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
# See the License for the specific language governing permissions and
# limitations under the License.

$opensslurl = "http://wiki.overbyte.eu/arch/openssl-1.1.1a-win64.zip"

Add-Type -AssemblyName System.IO.Compression.FileSystem
function Unzip
{
    param([string]$zipfile, [string]$outpath)

    [System.IO.Compression.ZipFile]::ExtractToDirectory($zipfile, $outpath)
}

$client = new-object System.Net.WebClient
$client.DownloadFile($opensslurl,".\temp.zip")

$pwd = (Get-Item -Path ".\").FullName

Unzip ".\temp.zip" $pwd



./openssl.exe req -x509 -newkey rsa:2048 -days 365 -keyout rsa_private.pem -nodes -out rsa_cert.pem -subj "/CN=unused"
./openssl.exe pkcs8 -topk8 -inform PEM -outform DER -in rsa_private.pem
./openssl.exe ecparam -genkey -name prime256v1 -noout -out ec_private.pem
./openssl.exe ec -in ec_private.pem -pubout -out ec_public.pem