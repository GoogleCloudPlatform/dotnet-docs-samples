using Xunit;

[Collection(nameof(StorageFixture))]
public class RemoveFileOwnerTest
{
    private readonly StorageFixture _fixture;

    public RemoveFileOwnerTest(StorageFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void TestRemoveFileOwner()
    {
        AddFileOwnerSample addFileOwnerSample = new AddFileOwnerSample();
        RemoveFileOwnerSample removeFileOwnerSample = new RemoveFileOwnerSample();
        GetMetadataSample getMetadataSample = new GetMetadataSample();

        // Add file owner.
        addFileOwnerSample.AddFileOwner(_fixture.BucketNameGeneric, _fixture.FileName, _fixture.ServiceAccountEmail);

        // Remove file owner.
        removeFileOwnerSample.RemoveFileOwner(_fixture.BucketNameGeneric, _fixture.FileName, _fixture.ServiceAccountEmail);

        var metadata = getMetadataSample.GetMetadata(_fixture.BucketNameGeneric, _fixture.FileName);
        Assert.DoesNotContain(metadata.Acl, acl => acl.Email == _fixture.ServiceAccountEmail && acl.Role == "OWNER");
    }
}
