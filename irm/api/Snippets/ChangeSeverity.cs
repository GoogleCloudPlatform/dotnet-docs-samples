using Google.Api.Gax.ResourceNames;
using Google.Cloud.Irm.V1Alpha2;
using Google.Protobuf.WellKnownTypes;
using static Google.Cloud.Irm.V1Alpha2.Incident.Types;
using System;
using Google.Api.Gax.Grpc;

class IrmChangeSeverity {
    public void ChangeSeverity(
        string projectId = "YOUR-PROJECT-ID",
        string incidentId = "What does this look like?")
    {
        // Create client
        IncidentServiceClient incidentServiceClient =
            IncidentServiceClient.Create();        
        // Update the severity.
        Incident incidentChange = new Incident()
        {
            Name = new IncidentName(projectId, incidentId).ToString(),
            Severity = Severity.Minor,
        };
        // Tell the API which fields to update.
        FieldMask mask = new FieldMask() { Paths = {"severity"} };
        // Call the API to update the incident.
        var incident =
            incidentServiceClient.UpdateIncident(incidentChange, mask);
        // How can the caller prevent race conditions updating the incident?
        Console.WriteLine("Changed severity of {0}.", incident.Name);
    }
}