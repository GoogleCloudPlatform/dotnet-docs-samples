using System.Linq;
using Xunit;

[Collection(nameof(BigtableClientFixture))]
public class WriteBatchTest
{
    private readonly BigtableClientFixture _fixture;
    public WriteBatchTest(BigtableClientFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void WriteBatch()
    {
        WriteBatchSample writeBatch = new WriteBatchSample();
        var result = writeBatch.WriteBatch(_fixture.projectId, _fixture.instanceId, _fixture.tableId);
        Assert.Equal(2, result.Count());
    }
}
