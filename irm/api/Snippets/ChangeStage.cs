using Google.Api.Gax.ResourceNames;
using Google.Cloud.Irm.V1Alpha2;
using Google.Protobuf.WellKnownTypes;
using static Google.Cloud.Irm.V1Alpha2.Incident.Types;
using System;
using Google.Api.Gax.Grpc;

class IrmChangeStage {
    public void ChangeStageAtomically(
        string projectId = "YOUR-PROJECT-ID",
        string incidentId = "What does this look like?")
    {
        // Create client
        IncidentServiceClient incidentServiceClient =
            IncidentServiceClient.Create();        
        // Update the severity.
        Incident incident = incidentServiceClient.GetIncident(
            new IncidentName(projectId, incidentId).ToString());
        Incident incidentChange = new Incident()
        {
            Name = incident.Name,
            Stage = Stage.Detected
        };
        // Tell the API which fields to update.
        FieldMask mask = new FieldMask() { Paths = {"stage"} };
        // Call the API to update the incident.
        // Use the "If-Match" header to prevent race conditions.
        incident = incidentServiceClient.UpdateIncident(incidentChange, mask,
            CallSettings.FromHeaderMutation(metadata => metadata.Add(
                "If-Match" , incident.Etag)));
        Console.WriteLine("Changed stage of {0}.", incident.Name);
    }
}