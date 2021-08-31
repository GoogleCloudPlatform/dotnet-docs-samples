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

[Collection(nameof(StorageFixture))]
public class PrintBucketDefaultAclTest
{
    private readonly StorageFixture _fixture;

    public PrintBucketDefaultAclTest(StorageFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void TestPrintBucketDefaultAcl()
    {
        PrintBucketDefaultAclSample printBucketDefaultAclSample = new PrintBucketDefaultAclSample();
        AddBucketDefaultOwnerSample addBucketDefaultOwnerSample = new AddBucketDefaultOwnerSample();
        RemoveBucketDefaultOwnerSample removeBucketDefaultOwnerSample = new RemoveBucketDefaultOwnerSample();
        string userEmail = _fixture.ServiceAccountEmail;

        // add default owner
        addBucketDefaultOwnerSample.AddBucketDefaultOwner(_fixture.BucketNameGeneric, userEmail);
        _fixture.SleepAfterBucketCreateUpdateDelete();

        // print default owner
        var defaultBucketAcl = printBucketDefaultAclSample.PrintBucketDefaultAcl(_fixture.BucketNameGeneric);
        Assert.Contains(defaultBucketAcl, c => c.Role == "OWNER" && c.Email == userEmail);

        // remove default owner
        removeBucketDefaultOwnerSample.RemoveBucketDefaultOwner(_fixture.BucketNameGeneric, userEmail);
        _fixture.SleepAfterBucketCreateUpdateDelete();
    }
}
