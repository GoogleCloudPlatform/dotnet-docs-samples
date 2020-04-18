// Copyright(c) 2020 Google LLC.
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

using CommandLine;
using Google.Api.Gax.ResourceNames;
using Google.Cloud.Gaming.V1Beta;
using System;
namespace GoogleCloudSamples
{

    public class RealmOptions : BaseOptions
    {
        [Value(2, HelpText = "ID for the realm")]
        public string RealmId { get; set; }
    }

    [Verb("create_realm", HelpText = "Create a realm")]
    public class CreateRealmOptions : RealmOptions
    { }

    [Verb("list_realms", HelpText = "List realms in project")]
    public class ListRealmsOptions : BaseOptions
    { };

    [Verb("get_realm", HelpText = "Get realm in project")]
    public class GetRealmOptions : RealmOptions
    { };

    [Verb("delete_realm", HelpText = "Delete a realm")]
    public class DeleteRealmOptions : RealmOptions
    { }

    public class Realms
    {
        public static void RegisterCommands(VerbMap<object> verbMap)
        {
            verbMap
                .Add((CreateRealmOptions opts) => CreateRealm(opts.ProjectID, opts.Location, opts.RealmId))
                .Add((ListRealmsOptions opts) => ListRealms(opts.ProjectID, opts.Location))
                .Add((GetRealmOptions opts) => GetRealms(opts.ProjectID, opts.Location, opts.RealmId))
                .Add((DeleteRealmOptions opts) => DeleteRealm(opts.ProjectID, opts.Location, opts.RealmId));
        }

        public static object CreateRealm(string projectId, string location, string realmId)
        {
            var client = RealmsServiceClient.Create();
            var request = new CreateRealmRequest
            {
                ParentAsLocationName = new LocationName(projectId, location),
                RealmId = realmId,
                Realm = new Realm
                {
                    Description = "My Game Server Realm",
                    // Must use a supported time zone name.
                    // See https://cloud.google.com/dataprep/docs/html/Supported-Time-Zone-Values_66194188
                    TimeZone = "US/Pacific"
                }
            };

            var response = client.CreateRealm(request);

            // Synchronous check of operation status
            var completedResponse = response.PollUntilCompleted();

            if (completedResponse.IsCompleted)
            {
                var result = completedResponse.Result;

                Console.WriteLine($"Realm name: {result.Name}");
                Console.WriteLine($"Realm description: {result.Description}");
                Console.WriteLine($"Realm time zone: {result.TimeZone}");
            }
            return 0;
        }

        public static object ListRealms(string projectId, string location)
        {
            var client = RealmsServiceClient.Create();
            var request = new ListRealmsRequest
            {
                ParentAsLocationName = new LocationName(projectId, location)
            };
            var response = client.ListRealms(request);

            foreach (var realm in response)
            {
                Console.WriteLine($"Realm name: {realm.Name}");
                Console.WriteLine($"Realm description: {realm.Description}"
                    + Environment.NewLine);
            }
            return 0;
        }

        private static int GetRealms(object projectId, object location, object realmId)
        {
            var client = RealmsServiceClient.Create();
            var request = new GetRealmRequest
            {
                Name = $"projects/{projectId}/locations/{location}/realms/{realmId}"
            };
            var realm = client.GetRealm(request);

            Console.WriteLine($"Realm name: {realm.Name}");
            Console.WriteLine($"Realm description: {realm.Description}");
            Console.WriteLine($"Realm time zone: {realm.TimeZone}");

            return 0;

        }

        public static object DeleteRealm(string projectId, string location, string realmId)
        {
            var client = RealmsServiceClient.Create();
            var request = new DeleteRealmRequest
            {
                Name = $"projects/{projectId}/locations/{location}/realms/{realmId}"
            };
            var response = client.DeleteRealm(request);

            // Synchronous check of operation status
            var completedResponse = response.PollUntilCompleted();

            if (completedResponse.IsCompleted)
            {
                Console.WriteLine("Realm deleted");
            }
            return 0;
        }
    }
}
