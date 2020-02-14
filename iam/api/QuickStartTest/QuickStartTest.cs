using System;
using Xunit;

public class QuickStartTest
{
    private readonly string _project;
    private readonly string _member;
    private readonly string _role;
    
    public QuickStartTest()
    {
        _project = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
        _member = "user:yaraarryn.677500@gmail.com";
        _role = "roles/logging.viewer";
    }
        
    [Fact]
    public void TestQuickStart()
    {
        // Will throw exceptions on failure
        var crmService = QuickStartAddMember.InitializeService();
        QuickStartAddMember.AddBinding(crmService, _project, _member, _role);
        QuickStartRemoveMember.RemoveMember(crmService, _project, _member, _role);
    }

    [Fact]
    public void TestMain()
    {
        // Main() will throw an exception on failure
        QuickStart.Main(new string[] { });
        Assert.True(true);
    }
}
