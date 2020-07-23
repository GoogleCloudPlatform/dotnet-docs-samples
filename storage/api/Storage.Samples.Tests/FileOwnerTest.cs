using Xunit;

[Collection(nameof(BucketFixture))]
public class FileOwnerTest
{
    private readonly BucketFixture _bucketFixture;

    public FileOwnerTest(BucketFixture bucketFixture)
    {
        _bucketFixture = bucketFixture;
    }

    [Fact]
    public void AddRemoveFileOwner()
    {
        AddFileOwnerSample addFileOwnerSample = new AddFileOwnerSample();
        RemoveFileOwnerSample removeFileOwnerSample = new RemoveFileOwnerSample();
        GetMetadataSample getMetadataSample = new GetMetadataSample();

        // add file owner
        var result = addFileOwnerSample.AddFileOwner(_bucketFixture.BucketName, _bucketFixture.FileName, _bucketFixture.ServiceAccountEmail);
        Assert.Contains(result.Acl, c => c.Role == "OWNER" && c.Email == _bucketFixture.ServiceAccountEmail);

        // remove file owner
        removeFileOwnerSample.RemoveFileOwner(_bucketFixture.BucketName, _bucketFixture.FileName, _bucketFixture.ServiceAccountEmail);

        result = getMetadataSample.GetMetadata(_bucketFixture.BucketName, _bucketFixture.FileName);
        Assert.Null(result.Acl);
    }
}
