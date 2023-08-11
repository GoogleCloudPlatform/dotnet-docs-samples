// Copyright 2023 Google Inc.
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

// [START dlp_inspect_datastore_send_to_scc]

using System.Collections.Generic;
using System.Linq;
using Google.Api.Gax.ResourceNames;
using Google.Cloud.Dlp.V2;
using static Google.Cloud.Dlp.V2.InspectConfig.Types;

public class InspectDataStoreJobWithSCCIntegration
{
    public static DlpJob SendInspectDatastoreToSCC(
        string projectId,
        string kindName,
        string namespaceId,
        Likelihood minLikelihood = Likelihood.Unlikely,
        IEnumerable<InfoType> infoTypes = null)
    {
        // Instantiate the dlp client.
        var dlp = DlpServiceClient.Create();

        // Specify the Datastore entity to be inspected and construct the storage
        // config. The NamespaceId is to be used for partition entity and the datastore kind defining
        // a data set.
        var storageConfig = new StorageConfig
        {
            DatastoreOptions = new DatastoreOptions
            {
                Kind = new KindExpression { Name = kindName },
                PartitionId = new PartitionId
                {
                    NamespaceId = namespaceId,
                    ProjectId = projectId
                }
            }
        };

        // Specify the type of info to be inspected and construct the inspect config.
        var inspectConfig = new InspectConfig
        {
            InfoTypes =
            {
                infoTypes ?? new InfoType[]
                {
                    new InfoType { Name = "EMAIL_ADDRESS" },
                    new InfoType { Name = "PERSON_NAME" },
                    new InfoType { Name = "LOCATION" },
                    new InfoType { Name = "PHONE_NUMBER" }
                }
            },
            IncludeQuote = true,
            MinLikelihood = minLikelihood,
            Limits = new FindingLimits
            {
                MaxFindingsPerRequest = 100
            }
        };

        // Construct the SCC action which will be performed after inspecting the datastore.
        var actions = new Action[]
        {
            new Action
            {
                PublishSummaryToCscc = new Action.Types.PublishSummaryToCscc()
            }
        };

        // Construct the inspect job config using storage config, inspect config and action.
        var inspectJob = new InspectJobConfig
        {
            StorageConfig = storageConfig,
            InspectConfig = inspectConfig,
            Actions = { actions }
        };

        // Construct the request.
        var request = new CreateDlpJobRequest
        {
            ParentAsLocationName = new LocationName(projectId, "global"),
            InspectJob = inspectJob
        };

        // Call the API.
        DlpJob response = dlp.CreateDlpJob(request);

        return response;
    }
}

// [END dlp_inspect_datastore_send_to_scc]
