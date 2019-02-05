using System;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Irm.V1Alpha2;
using Xunit;

public class Test
{
    [Fact]
    public void TestMostEverything()
    {
        var signal = new IrmCreateIncident().CreateIncidentWithSignal(s_projectId);
        try 
        {
            string incidentId = IncidentName.Parse(signal.Incident).IncidentId;
            var annotation = new IrmAnnotateIncident().AnnotateIncident(s_projectId, incidentId);
            new IrmChangeSeverity().ChangeSeverity(s_projectId, incidentId);
            new IrmChangeStage().ChangeStageAtomically(s_projectId, incidentId);
        } 
        finally
        {
            // I see now way to clean up old incidents!
        }
    }

    [Fact]
    public void TestCreateSignal()
    {
        new IrmCreateSignal().CreateSignal(s_projectId);
    }

    static readonly string s_projectId = GetProjectId();

    static readonly IncidentServiceClient s_incidentServiceClient =
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
