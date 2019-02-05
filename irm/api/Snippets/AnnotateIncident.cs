using Google.Api.Gax.ResourceNames;
using Google.Cloud.Irm.V1Alpha2;
using Google.Protobuf.WellKnownTypes;
using static Google.Cloud.Irm.V1Alpha2.Incident.Types;
using System;

class IrmAnnotateIncident 
{
    public string AnnotateIncident(
        string projectId = "YOUR-PROJECT-ID",
        string incidentId = "What does this look like?")
    {
        // Create client
        IncidentServiceClient incidentServiceClient =
            IncidentServiceClient.Create();
        // Describe the annotation.
        Annotation newAnnotation = new Annotation()
        {
            Content = "The red button was found in a depressed state."
        };
        string parent = new IncidentName(projectId, incidentId).ToString();
        // Call the API to create the annotation.
        Annotation annotation = 
            incidentServiceClient.CreateAnnotation(parent, newAnnotation);
        Console.WriteLine("Created annotation {0}.", annotation.Name);
        return annotation.Name;
    }
}