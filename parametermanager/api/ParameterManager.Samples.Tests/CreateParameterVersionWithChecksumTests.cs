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

using Force.Crc32;
using Google.Cloud.ParameterManager.V1;
using Google.Protobuf;
using Google.Rpc;
using System.Text;

[Collection(nameof(ParameterManagerFixture))]
public class CreateParameterVersionWithChecksumTests
{
    private readonly ParameterManagerFixture _fixture;
    private readonly CreateParameterVersionWithChecksumSample _sample;

    public CreateParameterVersionWithChecksumTests(ParameterManagerFixture fixture)
    {
        _fixture = fixture;
        _sample = new CreateParameterVersionWithChecksumSample();
    }

    [Fact]
    public void CreateParameterVersionWithChecksum()
    {
        Parameter parameter = _fixture.CreateParameter(_fixture.RandomId(), ParameterFormat.Unformatted);
        ParameterVersionName parameterVersionName = new ParameterVersionName(_fixture.ProjectId, ParameterManagerFixture.LocationId, parameter.ParameterName.ParameterId, _fixture.RandomId());

        ParameterVersion result = _sample.CreateParameterVersionWithChecksum(projectId: _fixture.ProjectId, parameterId: parameterVersionName.ParameterId, versionId: parameterVersionName.ParameterVersionId);
        _fixture.ParameterVersionsToDelete.Add(parameterVersionName);

        Assert.NotNull(result);
        Assert.Equal(parameterVersionName.ParameterVersionId, result.ParameterVersionName.ParameterVersionId);
        Assert.Equal(ParameterVersion.Types.ChecksumSource.UserSpecified, result.ChecksumSource);

        GetParameterVersionRequest request = new GetParameterVersionRequest
        {
            ParameterVersionName = parameterVersionName,
            View = View.Full
        };
        ParameterVersion fetched = _fixture.Client.GetParameterVersion(request);
        Assert.Equal(Crc32CAlgorithm.Compute(fetched.Payload.Data.ToByteArray()), fetched.Payload.DataCrc32C);
    }

    [Fact]
    public void CreateParameterVersionWithChecksumWithMismatchedChecksum()
    {
        Parameter parameter = _fixture.CreateParameter(_fixture.RandomId(), ParameterFormat.Unformatted);
        byte[] payload = Encoding.UTF8.GetBytes("hello world!");
        uint wrongCrc32c = Crc32CAlgorithm.Compute(payload) + 1;

        ParameterVersion parameterVersion = new ParameterVersion
        {
            Payload = new ParameterVersionPayload
            {
                Data = ByteString.CopyFrom(payload),
                DataCrc32C = wrongCrc32c
            }
        };

        var exception = Assert.Throws<Grpc.Core.RpcException>(() => _fixture.Client.CreateParameterVersion(parameter.ParameterName, parameterVersion, _fixture.RandomId()));
        Assert.Equal(Grpc.Core.StatusCode.InvalidArgument, exception.Status.StatusCode);
        Google.Rpc.Status status = Google.Rpc.Status.Parser.ParseFrom(exception.Trailers.GetValueBytes("grpc-status-details-bin"));
        Assert.Equal("CHECKSUM_MISMATCH", status.Details[0].Unpack<ErrorInfo>().Reason);
    }
}
