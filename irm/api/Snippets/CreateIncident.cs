using static Google.Cloud.Irm.V1Alpha2.IncidentService;

class IrmCreateIncident {
    public void CreateIncident()
    {
        // No create method!
        IncidentServiceClient incidents = IncidentServiceClient.Create();
    }
}