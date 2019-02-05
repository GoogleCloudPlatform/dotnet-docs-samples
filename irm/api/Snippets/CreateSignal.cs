using Google.Api.Gax.ResourceNames;
using Google.Cloud.Irm.V1Alpha2;
using System;

class IrmCreateSignal 
{
    public string CreateSignal(string projectId = "YOUR-PROJECT-ID")
    {
        // Create client
        IncidentServiceClient incidentServiceClient = IncidentServiceClient.Create();
        // Describe the signal.
        Signal signal = new Signal()
        {
            Title = "Red button pushed.",
            Content = "Somebody pushed the red button!",
            ContentType = "text/plain"
        };
        // Call the API to create the signal.
        string parent = new ProjectName(projectId).ToString();
        Signal response = incidentServiceClient.CreateSignal(parent, signal);
        Console.WriteLine("Created signal {0}.", response.Name);
        return response.Name;
    }
}