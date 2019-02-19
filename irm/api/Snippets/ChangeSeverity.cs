// Copyright (c) 2019 Google LLC.
// 
// Licensed under the Apache License, Version 2.0 (the "License"); you may not
// use this file except in compliance with the License. You may obtain a copy of
// the License at
// 
// http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
// License for the specific language governing permissions and limitations under
// the License.

// [START monitoring_irm_change_severity]

using Google.Api.Gax.ResourceNames;
using Google.Cloud.Irm.V1Alpha2;
using Google.Protobuf.WellKnownTypes;
using static Google.Cloud.Irm.V1Alpha2.Incident.Types;
using System;
using Google.Api.Gax.Grpc;

class IrmChangeSeverity
{
    public void ChangeSeverity(
        string projectId = "YOUR-PROJECT-ID",
        string incidentId = "an.opaque.random.id")
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
        FieldMask mask = new FieldMask() { Paths = { "severity" } };
        // Call the API to update the incident.
        var incident =
            incidentServiceClient.UpdateIncident(incidentChange, mask);
        // How can the caller prevent race conditions updating the incident?
        Console.WriteLine("Changed severity of {0}.", incident.Name);
    }
}
// [END monitoring_irm_change_severity]
