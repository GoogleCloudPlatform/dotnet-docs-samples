// Copyright(c) 2025 Google Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License"); you may not
// use this file except in compliance with the License. You may obtain a copy of
// the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
// License for the specific language governing permissions and limitations under
// the License.

using Google.Api.Gax.ResourceNames;
using Google.Cloud.Kms.V1;
using Google.Cloud.ParameterManager.V1;
using Google.Protobuf;
using System.Text;

[CollectionDefinition(nameof(ParameterManagerFixture))]
public class ParameterManagerFixture : IDisposable, ICollectionFixture<ParameterManagerFixture>
{
    public string ProjectId { get; }
    public const string LocationId = "global";

    public ParameterManagerClient client { get; }
    public KeyManagementServiceClient kmsClient { get; }
    internal List<ParameterName> ParametersToDelete { get; } = new List<ParameterName>();
    internal List<CryptoKeyVersionName> CryptoKeyVersionsToDelete { get; } = new List<CryptoKeyVersionName>();

    public ParameterManagerFixture()
    {
        ProjectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
        if (String.IsNullOrEmpty(ProjectId))
        {
            throw new Exception("missing GOOGLE_PROJECT_ID");
        }

        client = ParameterManagerClient.Create();
        kmsClient = KeyManagementServiceClient.Create();
        KeyRing keyRing = CreateKeyRing(ProjectId, "csharp-test-key-ring");
    }

    public void Dispose()
    {
        foreach (var cryptoKeyVersion in CryptoKeyVersionsToDelete)
        {
            DeleteKeyVersion(cryptoKeyVersion);
        }
        foreach (var parameter in ParametersToDelete)
        {
            DeleteParameter(parameter);
        }
    }

    public String RandomId()
    {
        return $"csharp-{System.Guid.NewGuid()}";
    }

    public Parameter CreateParameterWithKmsKey(string parameterId, ParameterFormat format, string kmsKey)
    {
        LocationName projectName = new LocationName(ProjectId, LocationId);

        Parameter parameter = new Parameter
        {
            Format = format,
            KmsKey = kmsKey
        };

        Parameter Parameter = client.CreateParameter(projectName, parameter, parameterId);
        ParametersToDelete.Add(Parameter.ParameterName);
        return Parameter;
    }

    private void DeleteParameter(ParameterName name)
    {
        try
        {
            client.DeleteParameter(name);
        }
        catch (Grpc.Core.RpcException e) when (e.StatusCode == Grpc.Core.StatusCode.NotFound)
        {
            // Ignore error - Parameter was already deleted
        }
    }

    public KeyRing CreateKeyRing(string projectId, string keyRingId)
    {
        string name = $"projects/{projectId}/locations/global/keyRings/{keyRingId}";
        LocationName parent = new LocationName(projectId, "global");
        try
        {
            KeyRing resp = kmsClient.GetKeyRing(name);
            return resp;
        }
        catch (Grpc.Core.RpcException e) when (e.StatusCode == Grpc.Core.StatusCode.NotFound)
        {
            return kmsClient.CreateKeyRing(new CreateKeyRingRequest
            {
                ParentAsLocationName = parent,
                KeyRingId = keyRingId,
            });
        }
    }

    public CryptoKey CreateHsmKey(string projectId, string keyId, string keyRingId)
    {
        KeyRingName parent = new KeyRingName(projectId, "global", keyRingId);

        CreateCryptoKeyRequest request = new CreateCryptoKeyRequest
        {
            ParentAsKeyRingName = parent,
            CryptoKeyId = keyId,
            CryptoKey = new CryptoKey
            {
                Purpose = CryptoKey.Types.CryptoKeyPurpose.EncryptDecrypt,
                VersionTemplate = new CryptoKeyVersionTemplate
                {
                    Algorithm = CryptoKeyVersion.Types.CryptoKeyVersionAlgorithm.GoogleSymmetricEncryption,
                    ProtectionLevel = ProtectionLevel.Hsm,
                },
            },
        };

        try
        {
            string name = $"projects/{projectId}/locations/global/keyRings/{keyRingId}/cryptoKeys/{keyId}";
            CryptoKey resp = kmsClient.GetCryptoKey(name);
            return resp;
        }
        catch (Grpc.Core.RpcException e) when (e.StatusCode == Grpc.Core.StatusCode.NotFound)
        {
            return kmsClient.CreateCryptoKey(request);
        }
    }

    public void DeleteKeyVersion(CryptoKeyVersionName name)
    {
        try
        {
            kmsClient.DestroyCryptoKeyVersion(name);
        }
        catch (Grpc.Core.RpcException e) when (e.StatusCode == Grpc.Core.StatusCode.NotFound)
        {
            // Ignore error - Parameter was already deleted
        }
    }
}
