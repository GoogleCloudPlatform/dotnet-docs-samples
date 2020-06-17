using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

[Collection(nameof(BigtableClientFixture))]
public class WriteSimpleTest
{
    private readonly BigtableClientFixture _fixture;
    public WriteSimpleTest(BigtableClientFixture fixture)
    {
        _fixture = fixture;
    }
    [Fact]
    public void WriteSimple()
    {
        var writeSimple = new WriteSimple();
        var output = writeSimple.Write(_fixture.projectId, _fixture.instanceId, _fixture.tableId);
        Assert.True(false);
    }
}
