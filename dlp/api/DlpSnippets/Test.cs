using System;
using Xunit;

public class DlpSnippetsTest
{
    readonly string _projectId = 
        Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
    DlpSnippets _snippets = new DlpSnippets();

    [Fact]
    public void TestDeidMask()
    {
        Assert.Equal("My SSN is *****9127", 
            _snippets.DeidentiyMasking(_projectId));
    }
}
