using Google.Cloud.Bigtable.V2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

[Collection(nameof(BigtableClientFixture))]
public class WriteIncrementTest
{
    private readonly BigtableClientFixture _fixture;
    public WriteIncrementTest(BigtableClientFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void WriteIncrement()
    {
        var writeIncrement = new WriteIncrementSample();
        var output = writeIncrement.WriteIncrement(_fixture.projectId, _fixture.instanceId, _fixture.tableId);
        Assert.True(false);
    }
}
