using Google.Api.Gax.ResourceNames;
using Google.Cloud.Irm.V1Alpha2;
using Google.Protobuf.WellKnownTypes;
using static Google.Cloud.Irm.V1Alpha2.Incident.Types;
using System;

class IrmCreateIncident {
    public Signal CreateIncidentWithSignal(string projectId = "YOUR-PROJECT-ID")
    {
        // Create client
        IncidentServiceClient incidentServiceClient =
            IncidentServiceClient.Create();
        // Describe the incident.
        Incident newIncident = new Incident()
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
            Severity = Severity.Major,
            Stage = Stage.Unspecified
        };
        string parent = new ProjectName(projectId).ToString();
        // Call the API to create the incident.
        Incident incident =
            incidentServiceClient.CreateIncident(newIncident, parent);
        Console.WriteLine("Created incident {0}.", incident.Name);

        // Describe the signal.
        Signal newSignal = new Signal()
        {
            Incident = incident.Name,
            Title = "Red button pushed.",
            Content = "Somebody pushed the red button!",
            ContentType = "text/plain",
        };

        // Call the API to create the signal.
        Signal signal = incidentServiceClient.CreateSignal(parent, newSignal);
        Console.WriteLine("Created signal {0}.", signal.Name);

        return signal;
    }
}