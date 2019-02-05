using Google.Api.Gax.ResourceNames;
using Google.Cloud.Irm.V1Alpha2;
using Google.Protobuf.WellKnownTypes;
using static Google.Cloud.Irm.V1Alpha2.Incident.Types;
using System;

class IrmCreateIncident {
    public void CreateIncident(string projectId = "YOUR-PROJECT-ID")
    {
        // Create client
        IncidentServiceClient incidentServiceClient = IncidentServiceClient.Create();
        // Describe the incident.
        Incident incident = new Incident()
        {
            Title = "Somebody pushed the red button!",
            Synopsis = new Synopsis() 
            {
                Author = new User() 
                {
                    Email = "janedoe@example.com",                    
                },
                Content = "Nobody should ever push the red button.",
                ContentType = "text/plain",
                UpdateTime = DateTime.UtcNow.ToTimestamp()
            },
            StartTime = DateTime.UtcNow.ToTimestamp(),
            Severity = Severity.Major,
            Stage = Stage.Unspecified
        };
        string parent = new ProjectName(projectId).ToString();
        // Call the API to create the incident.
        Incident response = incidentServiceClient.CreateIncident(incident, parent);
        Console.WriteLine("Created incident {0}.", response.Name);
    }
}