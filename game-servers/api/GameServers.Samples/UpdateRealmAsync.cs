/*
 * Copyright 2021 Google LLC
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     https://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

// [START cloud_game_servers_realm_update]

using Google.Cloud.Gaming.V1;
using Google.LongRunning;
using Google.Protobuf.WellKnownTypes;
using System.Threading.Tasks;

public class UpdateRealmSample
{
    public async Task<Realm> UpdateRealmAsync(
        string projectId, string regionId, string realmId)
    {
        // Create the client.
        RealmsServiceClient client = await RealmsServiceClient.CreateAsync();

        Realm realm = new Realm
        {
            RealmName = RealmName.FromProjectLocationRealm(projectId, regionId, realmId),
        };
        realm.Labels.Add("label-key-1", "label-value-1");
        realm.Labels.Add("label-key-2", "label-value-2");

        UpdateRealmRequest request = new UpdateRealmRequest
        {
            Realm = realm,
            UpdateMask = new FieldMask { Paths = { "labels" } }
        };

        // Make the request.
        Operation<Realm, OperationMetadata> response = await client.UpdateRealmAsync(request);
        Operation<Realm, OperationMetadata> completedResponse = await response.PollUntilCompletedAsync();

        // Retrieve the operation result. This result will NOT contain the updated labels.
        // If you want to get the updated resource, use a GET request on the resource.
        return completedResponse.Result;
    }
}
// [END cloud_game_servers_realm_update]