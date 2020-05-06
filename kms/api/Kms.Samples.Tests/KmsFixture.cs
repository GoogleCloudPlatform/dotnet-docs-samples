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


using Google.Api.Gax.ResourceNames;
using Google.Cloud.Kms.V1;
using Google.Protobuf.WellKnownTypes;
using System;
using System.Threading;
using Xunit;

[CollectionDefinition(nameof(KmsFixture))]
public class KmsFixture : IDisposable, ICollectionFixture<KmsFixture>
{
    public string ProjectId { get; }
    public ProjectName ProjectName { get; }

    public string LocationId { get; }
    public LocationName LocationName { get; }

    public string KeyRingId { get; }
    public KeyRingName KeyRingName { get; }

    public string AsymmetricDecryptKeyId { get; }
    public CryptoKeyName AsymmetricDecryptKeyName { get; }

    public string AsymmetricSignEcKeyId { get; }
    public CryptoKeyName AsymmetricSignEcKeyName { get; }

    public string AsymmetricSignRsaKeyId { get; }
    public CryptoKeyName AsymmetricSignRsaKeyName { get; }

    public string HsmKeyId { get; }
    public CryptoKeyName HsmKeyName { get; }

    public string SymmetricKeyId { get; }
    public CryptoKeyName SymmetricKeyName { get; }

    public KmsFixture()
    {
        ProjectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
        if (string.IsNullOrEmpty(ProjectId))
        {
            throw new Exception("missing GOOGLE_PROJECT_ID");
        }
        ProjectName = new ProjectName(ProjectId);

        LocationId = "us-east1";
        LocationName = new LocationName(ProjectId, LocationId);

        KeyRingId = RandomId();
        KeyRingName = new KeyRingName(ProjectId, LocationId, KeyRingId);
        CreateKeyRing(KeyRingId);

        AsymmetricDecryptKeyId = RandomId();
        AsymmetricDecryptKeyName = new CryptoKeyName(ProjectId, LocationId, KeyRingId, AsymmetricDecryptKeyId);
        CreateAsymmetricDecryptKey(AsymmetricDecryptKeyId);

        AsymmetricSignEcKeyId = RandomId();
        AsymmetricSignEcKeyName = new CryptoKeyName(ProjectId, LocationId, KeyRingId, AsymmetricSignEcKeyId);
        CreateAsymmetricSignEcKey(AsymmetricSignEcKeyId);

        AsymmetricSignRsaKeyId = RandomId();
        AsymmetricSignRsaKeyName = new CryptoKeyName(ProjectId, LocationId, KeyRingId, AsymmetricSignRsaKeyId);
        CreateAsymmetricSignRsaKey(AsymmetricSignRsaKeyId);

        HsmKeyId = RandomId();
        HsmKeyName = new CryptoKeyName(ProjectId, LocationId, KeyRingId, HsmKeyId);
        CreateHsmKey(HsmKeyId);

        SymmetricKeyId = RandomId();
        SymmetricKeyName = new CryptoKeyName(ProjectId, LocationId, KeyRingId, SymmetricKeyId);
        CreateSymmetricKey(SymmetricKeyId);
    }

    public void Dispose()
    {
        DisposeKeyRing(KeyRingId);
    }

    public void DisposeKeyRing(string keyRingId)
    {
        KeyManagementServiceClient client = KeyManagementServiceClient.Create();

        var listKeysRequest = new ListCryptoKeysRequest
        {
            ParentAsKeyRingName = new KeyRingName(ProjectId, LocationId, keyRingId),
        };

        foreach (var key in client.ListCryptoKeys(listKeysRequest))
        {
            if (key.RotationPeriod != null || key.NextRotationTime != null)
            {
                client.UpdateCryptoKey(new UpdateCryptoKeyRequest
                {
                    CryptoKey = new CryptoKey
                    {
                        CryptoKeyName = key.CryptoKeyName,
                        RotationPeriod = null,
                        NextRotationTime = null,
                    },
                    UpdateMask = new FieldMask
                    {
                        Paths = { "rotation_period", "next_rotation_time" }
                    },
                });
            }

            var listKeyVersionsRequest = new ListCryptoKeyVersionsRequest
            {
                ParentAsCryptoKeyName = key.CryptoKeyName,
                Filter = "state != DESTROYED AND state != DESTROY_SCHEDULED",
            };

            foreach (var keyVersion in client.ListCryptoKeyVersions(listKeyVersionsRequest))
            {
                client.DestroyCryptoKeyVersion(new DestroyCryptoKeyVersionRequest
                {
                    CryptoKeyVersionName = keyVersion.CryptoKeyVersionName,
                });
            }
        }
    }

    public string RandomId()
    {
        return $"csharp-{System.Guid.NewGuid()}";
    }

    public KeyRing CreateKeyRing(string keyRingId)
    {
        KeyManagementServiceClient client = KeyManagementServiceClient.Create();
        return client.CreateKeyRing(new CreateKeyRingRequest
        {
            ParentAsLocationName = LocationName,
            KeyRingId = keyRingId,
        });
    }

    public CryptoKey CreateAsymmetricDecryptKey(string keyId)
    {
        KeyManagementServiceClient client = KeyManagementServiceClient.Create();

        var request = new CreateCryptoKeyRequest
        {
            ParentAsKeyRingName = KeyRingName,
            CryptoKeyId = keyId,
            CryptoKey = new CryptoKey
            {
                Purpose = CryptoKey.Types.CryptoKeyPurpose.AsymmetricDecrypt,
                VersionTemplate = new CryptoKeyVersionTemplate
                {
                    Algorithm = CryptoKeyVersion.Types.CryptoKeyVersionAlgorithm.RsaDecryptOaep2048Sha256,
                },
            },
        };
        request.CryptoKey.Labels["foo"] = "bar";
        request.CryptoKey.Labels["zip"] = "zap";

        return client.CreateCryptoKey(request);
    }

    public CryptoKey CreateAsymmetricSignEcKey(string keyId)
    {
        KeyManagementServiceClient client = KeyManagementServiceClient.Create();

        var request = new CreateCryptoKeyRequest
        {
            ParentAsKeyRingName = KeyRingName,
            CryptoKeyId = keyId,
            CryptoKey = new CryptoKey
            {
                Purpose = CryptoKey.Types.CryptoKeyPurpose.AsymmetricSign,
                VersionTemplate = new CryptoKeyVersionTemplate
                {
                    Algorithm = CryptoKeyVersion.Types.CryptoKeyVersionAlgorithm.EcSignP256Sha256,
                },
            },
        };
        request.CryptoKey.Labels["foo"] = "bar";
        request.CryptoKey.Labels["zip"] = "zap";

        return client.CreateCryptoKey(request);
    }

    public CryptoKey CreateAsymmetricSignRsaKey(string keyId)
    {
        KeyManagementServiceClient client = KeyManagementServiceClient.Create();

        var request = new CreateCryptoKeyRequest
        {
            ParentAsKeyRingName = KeyRingName,
            CryptoKeyId = keyId,
            CryptoKey = new CryptoKey
            {
                Purpose = CryptoKey.Types.CryptoKeyPurpose.AsymmetricSign,
                VersionTemplate = new CryptoKeyVersionTemplate
                {
                    Algorithm = CryptoKeyVersion.Types.CryptoKeyVersionAlgorithm.RsaSignPss2048Sha256,
                },
            },
        };
        request.CryptoKey.Labels["foo"] = "bar";
        request.CryptoKey.Labels["zip"] = "zap";

        return client.CreateCryptoKey(request);
    }

    public CryptoKey CreateHsmKey(string keyId)
    {
        KeyManagementServiceClient client = KeyManagementServiceClient.Create();

        var request = new CreateCryptoKeyRequest
        {
            ParentAsKeyRingName = KeyRingName,
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
        request.CryptoKey.Labels["foo"] = "bar";
        request.CryptoKey.Labels["zip"] = "zap";

        return client.CreateCryptoKey(request);
    }

    public CryptoKey CreateSymmetricKey(string keyId)
    {
        KeyManagementServiceClient client = KeyManagementServiceClient.Create();

        var request = new CreateCryptoKeyRequest
        {
            ParentAsKeyRingName = KeyRingName,
            CryptoKeyId = keyId,
            CryptoKey = new CryptoKey
            {
                Purpose = CryptoKey.Types.CryptoKeyPurpose.EncryptDecrypt,
                VersionTemplate = new CryptoKeyVersionTemplate
                {
                    Algorithm = CryptoKeyVersion.Types.CryptoKeyVersionAlgorithm.GoogleSymmetricEncryption,
                },
            },
        };
        request.CryptoKey.Labels["foo"] = "bar";
        request.CryptoKey.Labels["zip"] = "zap";

        return client.CreateCryptoKey(request);
    }

    public CryptoKeyVersion CreateKeyVersion(string keyId)
    {
        KeyManagementServiceClient client = KeyManagementServiceClient.Create();

        var result = client.CreateCryptoKeyVersion(new CreateCryptoKeyVersionRequest
        {
            ParentAsCryptoKeyName = new CryptoKeyName(ProjectId, LocationId, KeyRingId, keyId),
        });

        for (var i = 1; i <= 5; i++)
        {
            var version = client.GetCryptoKeyVersion(new GetCryptoKeyVersionRequest
            {
                CryptoKeyVersionName = result.CryptoKeyVersionName,
            });

            if (version.State == CryptoKeyVersion.Types.CryptoKeyVersionState.Enabled)
            {
                return version;
            }

            Thread.Sleep(500 * i);
        }

        throw new TimeoutException($"{result.Name} not enabled within time");
    }
}
