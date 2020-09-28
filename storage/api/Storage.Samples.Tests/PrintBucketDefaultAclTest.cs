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
public class PrintBucketDefaultAclTest
{
    private readonly BucketFixture _bucketFixture;

    public PrintBucketDefaultAclTest(BucketFixture bucketFixture)
    {
        _bucketFixture = bucketFixture;
    }

    [Fact]
    public void TestPrintBucketDefaultAcl()
    {
        PrintBucketDefaultAclSample printBucketDefaultAclSample = new PrintBucketDefaultAclSample();
        AddBucketDefaultOwnerSample addBucketDefaultOwnerSample = new AddBucketDefaultOwnerSample();
        RemoveBucketDefaultOwnerSample removeBucketDefaultOwnerSample = new RemoveBucketDefaultOwnerSample();
        string userEmail = _bucketFixture.ServiceAccountEmail;

        // add default owner
        addBucketDefaultOwnerSample.AddBucketDefaultOwner(_bucketFixture.BucketNameGeneric, userEmail);
        _bucketFixture.SleepAfterBucketCreateUpdateDelete();

        // print default owner
        var defaultBucketAcl = printBucketDefaultAclSample.PrintBucketDefaultAcl(_bucketFixture.BucketNameGeneric);
        Assert.Contains(defaultBucketAcl, c => c.Role == "OWNER" && c.Email == userEmail);

        // remove default owner
        removeBucketDefaultOwnerSample.RemoveBucketDefaultOwner(_bucketFixture.BucketNameGeneric, userEmail);
        _bucketFixture.SleepAfterBucketCreateUpdateDelete();
    }
}
