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

// [START monitoring_irm_change_stage]

using Google.Api.Gax.ResourceNames;
using Google.Cloud.Irm.V1Alpha2;
using Google.Protobuf.WellKnownTypes;
using static Google.Cloud.Irm.V1Alpha2.Incident.Types;
using System;
using Google.Api.Gax.Grpc;

class IrmChangeStage
{
    public void ChangeStageAtomically(
        string projectId = "YOUR-PROJECT-ID",
        string incidentId = "an.opaque.random.id")
    {
        // Create client
        IncidentServiceClient incidentServiceClient =
            IncidentServiceClient.Create();
        for (int retry = 0; true; ++retry)
        {
            try
            {
                // Observe the current state of the incident.
                Incident incident = incidentServiceClient.GetIncident(
                    new IncidentName(projectId, incidentId).ToString());
                Incident incidentChange = new Incident()
                {
                    // TODO: Remove the name hack.
                    Name = incident.Name.Replace("/-/", $"/{projectId}/"),
                    // Use the ETag to prevent race conditions.  Only update the
                    // incident if it hasn't changed since we observed it.
                    Etag = incident.Etag,
                    Stage = Stage.Detected
                };
                // Tell the API which fields to update.
                FieldMask mask = new FieldMask() { Paths = { "stage" } };
                // Call the API to update the incident.
                incident = incidentServiceClient.UpdateIncident(
                    incidentChange, mask);
                Console.WriteLine("Changed stage of {0}.", incident.Name);
                break;
            }
            catch (Grpc.Core.RpcException e)
            when (retry < 3 && e.StatusCode == Grpc.Core.StatusCode.Aborted)
            {
                // Somebody else must have updated the incident at the same 
                // time!  Try again.
            }
        }
    }
}
// [END monitoring_irm_change_stage]
