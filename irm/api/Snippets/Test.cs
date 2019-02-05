using System;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Irm.V1Alpha2;
using Xunit;

public class Test
{
    [Fact]
    public void TestCreateIncidentAndAnnotate()
    {
        var signal = new IrmCreateIncident().CreateIncidentWithSignal(ProjectId);
        try 
        {
            var annotation = new IrmAnnotateIncident().AnnotateIncident(ProjectId,
                IncidentName.Parse(signal.Incident).IncidentId);
        } 
        finally
        {
            // I see now way to clean up old incidents!
        }
    }

    [Fact]
    public void TestCreateSignal()
    {
        new IrmCreateSignal().CreateSignal(ProjectId);
    }

    static readonly string ProjectId = GetProjectId();

    static readonly IncidentServiceClient IncidentServiceClient =
            IncidentServiceClient.Create();

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
