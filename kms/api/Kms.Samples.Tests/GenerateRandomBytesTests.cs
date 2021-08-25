/*
 * Copyright 2021 Google LLC
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

using Xunit;

[Collection(nameof(KmsFixture))]
public class GenerateRandomBytesTest
{
    private readonly KmsFixture _fixture;
    private readonly GenerateRandomBytesSample _sample;

    public GenerateRandomBytesTest(KmsFixture fixture)
    {
        _fixture = fixture;
        _sample = new GenerateRandomBytesSample();
    }

    [Fact]
    public void GeneratesRandomBytes()
    {
        // Run the sample code.
        var result = _sample.GenerateRandomBytes(
            projectId: _fixture.ProjectId, locationId: _fixture.LocationId, numBytes: 256);

        // Verify length of returned bytes.
        Assert.Equal(256, result.Length);
    }
}
