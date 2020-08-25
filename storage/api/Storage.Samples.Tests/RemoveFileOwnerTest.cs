using Xunit;

[Collection(nameof(BucketFixture))]
public class RemoveFileOwnerTest
{
    private readonly BucketFixture _bucketFixture;

    public RemoveFileOwnerTest(BucketFixture bucketFixture)
    {
        _bucketFixture = bucketFixture;
    }

    [Fact]
    public void TestRemoveFileOwner()
    {
        AddFileOwnerSample addFileOwnerSample = new AddFileOwnerSample();
        RemoveFileOwnerSample removeFileOwnerSample = new RemoveFileOwnerSample();
        GetMetadataSample getMetadataSample = new GetMetadataSample();

        // Add file owner.
        addFileOwnerSample.AddFileOwner(_bucketFixture.BucketNameGeneric, _bucketFixture.FileName, _bucketFixture.ServiceAccountEmail);

        // Remove file owner.
        removeFileOwnerSample.RemoveFileOwner(_bucketFixture.BucketNameGeneric, _bucketFixture.FileName, _bucketFixture.ServiceAccountEmail);

        var metadata = getMetadataSample.GetMetadata(_bucketFixture.BucketNameGeneric, _bucketFixture.FileName);
        Assert.DoesNotContain(metadata.Acl, acl => acl.Email == _bucketFixture.ServiceAccountEmail && acl.Role == "OWNER");
    }
}
