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
        string userEmail = "230835935096-8io28ro0tvbbv612p5k6nstlaucmhnrq@developer.gserviceaccount.com";

        // add file owner
        var result = addFileOwnerSample.AddFileOwner(_bucketFixture.BucketName, _bucketFixture.FileName, userEmail);
        Assert.Contains(result.Acl, c => c.Role == "OWNER" && c.Email == userEmail);

        // remove file owner
        removeFileOwnerSample.RemoveFileOwner(_bucketFixture.BucketName, _bucketFixture.FileName, userEmail);

        result = getMetadataSample.GetMetadata(_bucketFixture.BucketName, _bucketFixture.FileName);
        Assert.Null(result.Acl);
    }
}
