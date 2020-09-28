// Copyright 2020 Google Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Xunit;

[Collection(nameof(BucketFixture))]
public class GetRequesterPaysStatusTest
{
    private readonly BucketFixture _bucketFixture;

    public GetRequesterPaysStatusTest(BucketFixture bucketFixture)
    {
        _bucketFixture = bucketFixture;
    }

    [Fact]
    public void TestGetRequesterPaysStatus()
    {
        GetRequesterPaysStatusSample getRequesterPaysStatusSample = new GetRequesterPaysStatusSample();
        EnableRequesterPaysSample enableRequesterPaysSample = new EnableRequesterPaysSample();
        DisableRequesterPaysSample disableRequesterPaysSample = new DisableRequesterPaysSample();

        // Enable request pay.
        enableRequesterPaysSample.EnableRequesterPays(_bucketFixture.BucketNameGeneric);
        _bucketFixture.SleepAfterBucketCreateUpdateDelete();

        // Get status.
        var status = getRequesterPaysStatusSample.GetRequesterPaysStatus(_bucketFixture.ProjectId, _bucketFixture.BucketNameGeneric);
        Assert.True(status);

        // Disable request pay.
        disableRequesterPaysSample.DisableRequesterPays(_bucketFixture.ProjectId, _bucketFixture.BucketNameGeneric);
        _bucketFixture.SleepAfterBucketCreateUpdateDelete();
    }
}
