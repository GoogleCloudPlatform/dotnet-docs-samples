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

using System;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Irm.V1Alpha2;
using Xunit;

public class Test
{
    static readonly string s_projectId = GetProjectId();

    static readonly IncidentServiceClient s_incidentServiceClient =
            IncidentServiceClient.Create();

    static string GetProjectId()
    {
        GoogleCredential googleCredential = Google.Apis.Auth.OAuth2
            .GoogleCredential.GetApplicationDefault();
        if (googleCredential != null)
        {
            ICredential credential = googleCredential.UnderlyingCredential;
            ServiceAccountCredential serviceAccountCredential =
                credential as ServiceAccountCredential;
            if (serviceAccountCredential != null)
            {
                return serviceAccountCredential.ProjectId;
            }
        }
        return Google.Api.Gax.Platform.Instance().ProjectId;
    }

    [Fact]
    public void TestMostEverything()
    {
        var signal = new IrmCreateIncident().CreateIncidentWithSignal(s_projectId);
        try
        {
            string incidentId = IncidentName.Parse(signal.Incident).IncidentId;
            var annotation = new IrmAnnotateIncident().AnnotateIncident(s_projectId, incidentId);
            new IrmChangeSeverity().ChangeSeverity(s_projectId, incidentId);
            new IrmChangeStage().ChangeStageAtomically(s_projectId, incidentId);
        }
        finally
        {
            // I see now way to clean up old incidents!
        }
    }

    [Fact]
    public void TestCreateSignal()
    {
        new IrmCreateSignal().CreateSignal(s_projectId);
    }
}
