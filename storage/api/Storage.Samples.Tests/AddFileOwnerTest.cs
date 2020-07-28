using Xunit;

[Collection(nameof(BucketFixture))]
public class AddFileOwnerTest
{
    private readonly BucketFixture _bucketFixture;

    public AddFileOwnerTest(BucketFixture bucketFixture)
    {
        _bucketFixture = bucketFixture;
    }

    [Fact]
    public void TestAddFileOwner()
    {
        AddFileOwnerSample addFileOwnerSample = new AddFileOwnerSample();
        RemoveFileOwnerSample removeFileOwnerSample = new RemoveFileOwnerSample();

        // Add file owner.
        var result = addFileOwnerSample.AddFileOwner(_bucketFixture.BucketNameGeneric, _bucketFixture.FileName, _bucketFixture.ServiceAccountEmail);
        Assert.Contains(result.Acl, c => c.Role == "OWNER" && c.Email == _bucketFixture.ServiceAccountEmail);

        // Remove file owner.
        removeFileOwnerSample.RemoveFileOwner(_bucketFixture.BucketNameGeneric, _bucketFixture.FileName, _bucketFixture.ServiceAccountEmail);
    }
}
