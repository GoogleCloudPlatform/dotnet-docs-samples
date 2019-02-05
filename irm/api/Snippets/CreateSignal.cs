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
