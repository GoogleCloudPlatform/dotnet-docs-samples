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
    public readonly string ProjectId;
    public readonly ProjectName ProjectName;

    public readonly string LocationId;
    public readonly LocationName LocationName;

    public readonly string KeyRingId;
    public readonly KeyRingName KeyRingName;

    public readonly string AsymmetricDecryptKeyId;
    public readonly CryptoKeyName AsymmetricDecryptKeyName;

    public readonly string AsymmetricSignEcKeyId;
    public readonly CryptoKeyName AsymmetricSignEcKeyName;

    public readonly string AsymmetricSignRsaKeyId;
    public readonly CryptoKeyName AsymmetricSignRsaKeyName;

    public readonly string SymmetricKeyId;
    public readonly CryptoKeyName SymmetricKeyName;

    public KmsFixture()
    {
        this.ProjectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
        if (string.IsNullOrEmpty(this.ProjectId))
        {
            throw new Exception("missing GOOGLE_PROJECT_ID");
        }
        this.ProjectName = new ProjectName(this.ProjectId);

        this.LocationId = "us-east1";
        this.LocationName = new LocationName(this.ProjectId, this.LocationId);

        this.KeyRingId = this.RandomId();
        this.KeyRingName = new KeyRingName(this.ProjectId, this.LocationId, this.KeyRingId);
        this.CreateKeyRing(this.KeyRingId);

        this.AsymmetricDecryptKeyId = this.RandomId();
        this.AsymmetricDecryptKeyName = new CryptoKeyName(this.ProjectId, this.LocationId, this.KeyRingId, this.AsymmetricDecryptKeyId);
        this.CreateAsymmetricDecryptKey(this.AsymmetricDecryptKeyId);

        this.AsymmetricSignEcKeyId = this.RandomId();
        this.AsymmetricSignEcKeyName = new CryptoKeyName(this.ProjectId, this.LocationId, this.KeyRingId, this.AsymmetricSignEcKeyId);
        this.CreateAsymmetricSignEcKey(this.AsymmetricSignEcKeyId);

        this.AsymmetricSignRsaKeyId = this.RandomId();
        this.AsymmetricSignRsaKeyName = new CryptoKeyName(this.ProjectId, this.LocationId, this.KeyRingId, this.AsymmetricSignRsaKeyId);
        this.CreateAsymmetricSignRsaKey(this.AsymmetricSignRsaKeyId);

        this.SymmetricKeyId = this.RandomId();
        this.SymmetricKeyName = new CryptoKeyName(this.ProjectId, this.LocationId, this.KeyRingId, this.SymmetricKeyId);
        this.CreateSymmetricKey(this.SymmetricKeyId);
    }

    public void Dispose()
    {
        DisposeKeyRing(this.KeyRingId);
    }

    public void DisposeKeyRing(string keyRingId)
    {
        var client = KeyManagementServiceClient.Create();

        var listKeysRequest = new ListCryptoKeysRequest
        {
            ParentAsKeyRingName = new KeyRingName(this.ProjectId, this.LocationId, keyRingId),
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
        var client = KeyManagementServiceClient.Create();
        return client.CreateKeyRing(new CreateKeyRingRequest
        {
            ParentAsLocationName = this.LocationName,
            KeyRingId = keyRingId,
        });
    }

    public CryptoKey CreateAsymmetricDecryptKey(string keyId)
    {
        var client = KeyManagementServiceClient.Create();

        var request = new CreateCryptoKeyRequest
        {
            ParentAsKeyRingName = this.KeyRingName,
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
        var client = KeyManagementServiceClient.Create();

        var request = new CreateCryptoKeyRequest
        {
            ParentAsKeyRingName = this.KeyRingName,
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
        var client = KeyManagementServiceClient.Create();

        var request = new CreateCryptoKeyRequest
        {
            ParentAsKeyRingName = this.KeyRingName,
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

    public CryptoKey CreateSymmetricKey(string keyId)
    {
        var client = KeyManagementServiceClient.Create();

        var request = new CreateCryptoKeyRequest
        {
            ParentAsKeyRingName = this.KeyRingName,
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
        var client = KeyManagementServiceClient.Create();

        var result = client.CreateCryptoKeyVersion(new CreateCryptoKeyVersionRequest
        {
            ParentAsCryptoKeyName = new CryptoKeyName(this.ProjectId, this.LocationId, this.KeyRingId, keyId),
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
