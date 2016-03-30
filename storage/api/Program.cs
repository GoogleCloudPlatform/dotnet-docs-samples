/*
 * Copyright (c) 2015 Google Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not
 * use this file except in compliance with the License. You may obtain a copy of
 * the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
 * License for the specific language governing permissions and limitations under
 * the License.
 */

/**
 * Sample code to issue several basic Google Cloud Store (GCS) operations
 * using the Google Client Libraries.
 * For more information, see documentation for Compute Storage .NET client
 * https://cloud.google.com/storage/docs/json_api/v1/json-api-dotnet-samples
 */

using System;
using System.Linq;

namespace GoogleCloudSamples
{
    public class StorageProgram
    {
        public static void Main(string[] args)
        {
            if (args.Length == 0)
                PrintUsage();
            else
                RunCommand(args.First(), args.Skip(1).ToArray());
        }

        private static void PrintUsage()
        {
            Console.WriteLine(@"Usage: StorageSample.exe [command] [args]
       ListBuckets
       ...........                 [name]
");
        }

        private static void RunCommand(string command, string[] args)
        {
            var projectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");

            if (string.IsNullOrEmpty(projectId))
            {
                Console.WriteLine("GOOGLE_PROJECT_ID environment variable needs to be set");
                return;
            }

            switch (command)
            {
                case "ListBuckets":
                    new StorageSample().ListBuckets(projectId);
                    break;
                default:
                    Console.WriteLine($"Command not found: {command}");
                    break;
            }
        }
    }
}