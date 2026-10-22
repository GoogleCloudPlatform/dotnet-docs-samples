/*
 * Copyright 2026 Google LLC
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

using Google.Cloud.SecretManager.V1;
using Google.Protobuf.WellKnownTypes;
using System;
using Xunit;

[Collection(nameof(RegionalSecretManagerFixture))]
public class UpdateRegionalSecretWithManagedRotationScheduleTests
{
    private readonly RegionalSecretManagerFixture _fixture;
    private readonly EnableRegionalSecretManagedRotationSample _enableSample;
    private readonly UpdateRegionalSecretWithManagedRotationScheduleSample _sample;

    public UpdateRegionalSecretWithManagedRotationScheduleTests(RegionalSecretManagerFixture fixture)
    {
        _fixture = fixture;
        _enableSample = new EnableRegionalSecretManagedRotationSample();
        _sample = new UpdateRegionalSecretWithManagedRotationScheduleSample();
    }

    [Fact]
    public void UpdatesRegionalSecretWithManagedRotationSchedule()
    {
        Secret secret = null;
        string member = null;
        try
        {
            secret = _fixture.CreateCloudSqlCredentialsSecret(_fixture.RandomId());
            member = secret.PolicyMember.IamPolicyUidPrincipal;

            _enableSample.EnableRegionalSecretManagedRotation(
              secret.SecretName.ProjectId, secret.SecretName.LocationId, secret.SecretName.SecretId,
              _fixture.CloudSqlInstanceId, _fixture.CloudSqlUsername);

            long rotationPeriodSeconds = 3600;
            DateTimeOffset before = DateTimeOffset.UtcNow;

            // Run the sample code.
            Secret updated = _sample.UpdateRegionalSecretWithManagedRotationSchedule(
              projectId: secret.SecretName.ProjectId,
              locationId: secret.SecretName.LocationId,
              secretId: secret.SecretName.SecretId,
              rotationPeriodSeconds: rotationPeriodSeconds);

            // Assert that the schedule was applied.
            Assert.Equal(rotationPeriodSeconds, updated.Rotation.RotationPeriod.Seconds);
            Assert.True(
              updated.Rotation.NextRotationTime.ToDateTimeOffset() >= before.AddSeconds(rotationPeriodSeconds).AddSeconds(-1));
        }
        finally
        {
            if (secret != null)
            {
                _fixture.DeleteSecret(secret.SecretName);
            }
            if (member != null)
            {
                _fixture.RevokeCloudSqlRole(member);
            }
        }
    }
}
