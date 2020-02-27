// Copyright 2020 Google LLC
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     https://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Google.Cloud.Redis.V1;
using System;

namespace Redis
{
    public class Redis
    {
        public static readonly string s_projectId = "YOUR-PROJECT-ID";
        public static readonly string s_locationId = "LOCATION-ID";
        private static readonly string s_usage =
            "Usage: \n" +
            "  Redis create [instance-name]\n" +
            "  Redis get [instance-name]\n" +
            "  Redis list\n";

        public static int Main(string[] args)
        {
            Redis redis = new Redis();
            return redis.Run(args);
        }

        public int Run(string[] args)
        {
            if (s_projectId == "YOUR-PROJECT-ID" || s_locationId == "LOCATION-ID")
            {
                Console.WriteLine("Update Redis.cs and replace YOUR-PROJECT" +
                    "-ID with your project id and LOCATION-ID with location id, and recompile.");
                return -1;
            }
            if (args.Length < 1 && PrintUsage()) return -1;
            try
            {
                switch (args[0].ToLower())
                {
                    case "create":
                        if (args.Length < 2 && PrintUsage()) return -1;
                        CreateInstance.Create(s_projectId, s_locationId, args[1]);
                        break;
                    case "get":
                        if (args.Length < 2 && PrintUsage()) return -1;
                        GetInstance.Get(s_projectId, s_locationId, args[1]);
                        break;
                    case "list":
                        ListInstance.List(s_projectId, s_locationId);
                        break;
                    default:
                        PrintUsage();
                        return -1;
                }
                return 0;
            }
            catch (Google.GoogleApiException e)
            {
                Console.WriteLine(e.Message);
                return null == e.Error ? -1 : e.Error.Code;
            }
        }

        /// <summary>
        /// Prints the basic information of Redis instance.
        /// </summary>
        public static void PrintInstanceInfo(Instance instance)
        {
            Console.WriteLine($"InstanceId:\t{instance.InstanceName.InstanceId}");
            Console.WriteLine($"State:\t{instance.State}");
            Console.WriteLine($"Tier:\t{instance.Tier}");
            Console.WriteLine($"Host:\t{instance.Host}");
            Console.WriteLine($"Location:\t{instance.LocationId}");
            Console.WriteLine($"Memory Size(GB):\t{instance.MemorySizeGb}");
            Console.WriteLine($"Version:\t{instance.RedisVersion}");
            Console.WriteLine($"Reserved IP Range:\t{instance.ReservedIpRange}");
        }

        private bool PrintUsage()
        {
            Console.WriteLine(s_usage);
            return true;
        }
    }
}
