using System;
using Xunit;

public class Test
{
    [Fact]
    public void TestCreateIncident()
    {   
        new IrmCreateIncident().CreateIncident();
    }
}
