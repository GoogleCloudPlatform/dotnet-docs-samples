using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

[Collection(nameof(BigtableClientFixture))]
public class TestWriteSimpleIncrementConditional
{
    private readonly BigtableClientFixture _fixture;
    public TestWriteSimpleIncrementConditional(BigtableClientFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void WriteSimpleIncrementConditional()
    {
        //WriteSimple writeSimple = new WriteSimple();
        //var output=writeSimple.Write(_fixture.projectId, _fixture.instanceId, _fixture.tableId)
        //Assert.Contains(2, output.);

        //WriteIncrementSample writeIncrement = new WriteIncrementSample();
        //Assert.Contains("Successfully updated row", writeIncrement.writeIncrement(_fixture.projectId, _fixture.instanceId, _fixture.tableId));

        //Writes.WriteConditional writeConditional = new Writes.WriteConditional();
        //Assert.Contains("Successfully updated row's os_name: True", writeConditional.writeConditional(_fixture.projectId, _fixture.instanceId, _fixture.tableId));
    }
}
