/*
 * Copyright 2020 Google LLC
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     https://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

// [START kms_verify_asymmetric_signature_ec]
using Google.Cloud.Kms.V1;

public class VerifyAsymmetricSignatureEcSample
{
    public bool VerifyAsymmetricSignatureEc(
      string projectId = "my-project", string locationId = "us-east1", string keyRingId = "my-key-ring", string keyId = "my-key", string keyVersionId = "123",
      string message = "my message",
      byte[] signature = null)
    {
        // Cloud KMS returns signatures in a DER-encoded format. .NET requires
        // signatures to be in IEEE 1363 format, and converting between these
        // formats is a few hundred lines of code.
        //
        // https://github.com/dotnet/runtime/pull/1612 exposes these helpers,
        // but will not be available until .NET 5. Until then, you will need to
        // use an external library or package to validate signatures.

        return false;
    }
}
// [END kms_verify_asymmetric_signature_ec]
