using System;
using Google.Apis.Auth.OAuth2;
using Xunit;

public class Test
{
    [Fact]
    public void TestCreateIncident()
    {
        new IrmCreateIncident().CreateIncidentWithSignal(ProjectId);
    }

    [Fact]
    public void TestCreateSignal()
    {
        new IrmCreateSignal().CreateSignal(ProjectId);
    }

    static readonly string ProjectId = GetProjectId();

    static string GetProjectId()
    {
        GoogleCredential googleCredential = Google.Apis.Auth.OAuth2
            .GoogleCredential.GetApplicationDefault();
        if (googleCredential != null)
        {
            ICredential credential = googleCredential.UnderlyingCredential;
            ServiceAccountCredential serviceAccountCredential =
                credential as ServiceAccountCredential;
            if (serviceAccountCredential != null)
            {
                return serviceAccountCredential.ProjectId;
            }
        }
        return Google.Api.Gax.Platform.Instance().ProjectId;
    }
}
