using System;
using Xunit;

public class QuickStartTest
{
    [Fact]
    public void TestMain()
    {
        // Main() will throw an exception on failure
        QuickStart.Main(new string[] { });
        Assert.True(true);
    }
}
