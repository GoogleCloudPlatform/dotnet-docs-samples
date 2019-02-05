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
