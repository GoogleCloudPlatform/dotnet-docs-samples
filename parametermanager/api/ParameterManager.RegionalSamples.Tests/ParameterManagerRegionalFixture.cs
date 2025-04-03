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
    public string LocationId = "us-central1";

    public ParameterName ParameterName { get; }

    public CryptoKeyVersionName CryptoKeyVersionName1 { get; }
    public CryptoKeyVersionName CryptoKeyVersionName2 { get; }

    public string ParameterId { get; }
    public string KeyId { get; }
    public string KeyId1 { get; }

    public Parameter UpdateParamKmsKey { get; }
    public Parameter RemoveParamKmsKey { get; }
    public CryptoKey cryptoKey { get; }
    public CryptoKey cryptoKey1 { get; }

    public ParameterManagerFixture()
    {
        ProjectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
        if (String.IsNullOrEmpty(ProjectId))
        {
            throw new Exception("missing GOOGLE_PROJECT_ID");
        }

        ParameterName = new ParameterName(ProjectId, LocationId, RandomId());

        KeyRing keyRing = CreateKeyRing(ProjectId, "csharp-test-key-ring");

        KeyId = RandomId();
        KeyId1 = RandomId();

        cryptoKey = CreateHsmKey(ProjectId, KeyId, "csharp-test-key-ring");
        cryptoKey1 = CreateHsmKey(ProjectId, KeyId1, "csharp-test-key-ring");

        ParameterId = RandomId();
        UpdateParamKmsKey = CreateParameterWithKmsKey(ParameterId, ParameterFormat.Unformatted, cryptoKey.Name);

        ParameterId = RandomId();
        RemoveParamKmsKey = CreateParameterWithKmsKey(ParameterId, ParameterFormat.Unformatted, cryptoKey.Name);

        CryptoKeyVersionName1 = new CryptoKeyVersionName(ProjectId, LocationId, "csharp-test-key-ring", KeyId, "1");
        CryptoKeyVersionName2 = new CryptoKeyVersionName(ProjectId, LocationId, "csharp-test-key-ring", KeyId1, "1");
    }

    public void Dispose()
    {
        DeleteParameter(ParameterName);
        DeleteParameter(UpdateParamKmsKey.ParameterName);
        DeleteParameter(RemoveParamKmsKey.ParameterName);
        DeleteKeyVersion(CryptoKeyVersionName1);
        DeleteKeyVersion(CryptoKeyVersionName2);
    }

    public String RandomId()
    {
        return $"csharp-{System.Guid.NewGuid()}";
    }

    public Parameter CreateParameterWithKmsKey(string parameterId, ParameterFormat format, string kmsKey)
    {
        // Define the regional endpoint
        string regionalEndpoint = $"parametermanager.{LocationId}.rep.googleapis.com";

        // Create the client with the regional endpoint
        ParameterManagerClient client = new ParameterManagerClientBuilder
        {
            Endpoint = regionalEndpoint
        }.Build();
        LocationName projectName = new LocationName(ProjectId, LocationId);

        Parameter parameter = new Parameter
        {
            Format = format,
            KmsKey = kmsKey
        };

        return client.CreateParameter(projectName, parameter, parameterId);
    }

    private void DeleteParameter(ParameterName name)
    {
        // Define the regional endpoint
        string regionalEndpoint = $"parametermanager.{LocationId}.rep.googleapis.com";

        // Create the client with the regional endpoint
        ParameterManagerClient client = new ParameterManagerClientBuilder
        {
            Endpoint = regionalEndpoint
        }.Build();
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
        KeyManagementServiceClient client = KeyManagementServiceClient.Create();
        string name = $"projects/{projectId}/locations/{LocationId}/keyRings/{keyRingId}";
        LocationName parent = new LocationName(projectId, LocationId);
        try
        {
            KeyRing resp = client.GetKeyRing(name);
            return resp;
        }
        catch (Grpc.Core.RpcException e) when (e.StatusCode == Grpc.Core.StatusCode.NotFound)
        {
            return client.CreateKeyRing(new CreateKeyRingRequest
            {
                ParentAsLocationName = parent,
                KeyRingId = keyRingId,
            });
        }
    }

    public CryptoKey CreateHsmKey(string projectId, string keyId, string keyRingId)
    {
        KeyManagementServiceClient client = KeyManagementServiceClient.Create();
        KeyRingName parent = new KeyRingName(projectId, LocationId, keyRingId);

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
            string name = $"projects/{projectId}/locations/{LocationId}/keyRings/{keyRingId}/cryptoKeys/{keyId}";
            CryptoKey resp = client.GetCryptoKey(name);
            return resp;
        }
        catch (Grpc.Core.RpcException e) when (e.StatusCode == Grpc.Core.StatusCode.NotFound)
        {
            return client.CreateCryptoKey(request);
        }
    }

    public void DeleteKeyVersion(CryptoKeyVersionName name)
    {
        KeyManagementServiceClient client = KeyManagementServiceClient.Create();
        try
        {
            client.DestroyCryptoKeyVersion(name);
        }
        catch (Grpc.Core.RpcException e) when (e.StatusCode == Grpc.Core.StatusCode.NotFound)
        {
            // Ignore error - Parameter was already deleted
        }
    }
}
