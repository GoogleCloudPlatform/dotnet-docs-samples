// Copyright(c) 2018 Google Inc.
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
using Google.Cloud.Firestore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GoogleCloudSamples
{
    public class TransactionsAndBatchedWrites
    {
        public static string Usage = @"Usage:
C:\> dotnet run command YOUR_PROJECT_ID

Where command is one of
    run-simple-transaction
    return-info-transaction
    batch-write
";
        private static async Task RunSimpleTransaction(string project)
        {
            FirestoreDb db = FirestoreDb.Create(project);
            // [START fs_run_simple_transaction]
            DocumentReference cityRef = db.Collection("cities").Document("SF");
            await db.RunTransactionAsync(async transaction =>
            {
                DocumentSnapshot snapshot = await transaction.GetSnapshotAsync(cityRef);
                long newPopulation = snapshot.GetValue<long>("Population") + 1;
                Dictionary<string, object> updates = new Dictionary<string, object>
                {
                    { "Population", newPopulation}
                };
                transaction.Update(cityRef, updates);
            });
            // [END fs_run_simple_transaction]
            Console.WriteLine("Ran a simple transaction to update the population field in the SF document in the cities collection.");
        }

        private static async Task ReturnInfoTransaction(string project)
        {
            FirestoreDb db = FirestoreDb.Create(project);
            // [START fs_return_info_transaction]
            DocumentReference cityRef = db.Collection("cities").Document("SF");
            bool transactionResult = await db.RunTransactionAsync(async transaction =>
            {
                DocumentSnapshot snapshot = await transaction.GetSnapshotAsync(cityRef);
                long newPopulation = snapshot.GetValue<long>("Population") + 1;
                if (newPopulation <= 1000000)
                {
                    Dictionary<string, object> updates = new Dictionary<string, object>
                    {
                        { "Population", newPopulation}
                    };
                    transaction.Update(cityRef, updates);
                    return true;
                }
                else
                {
                    return false;
                }
            });

            if (transactionResult)
            {
                Console.WriteLine("Population updated successfully.");
            }
            else
            {
                Console.WriteLine("Sorry! Population is too big.");
            }
            // [END fs_return_info_transaction]
        }

        private static async Task BatchWrite(string project)
        {
            FirestoreDb db = FirestoreDb.Create(project);
            // [START fs_batch_write]
            WriteBatch batch = db.StartBatch();

            // Set the data for NYC
            DocumentReference nycRef = db.Collection("cities").Document("NYC");
            Dictionary<string, object> nycData = new Dictionary<string, object>
            {
                { "name", "New York City" }
            };
            batch.Set(nycRef, nycData);

            // Update the population for SF
            DocumentReference sfRef = db.Collection("cities").Document("SF");
            Dictionary<string, object> updates = new Dictionary<string, object>
            {
                { "Population", 1000000}
            };
            batch.Update(sfRef, updates);

            // Delete LA
            DocumentReference laRef = db.Collection("cities").Document("LA");
            batch.Delete(laRef);

            // Commit the batch
            await batch.CommitAsync();
            // [END fs_batch_write]
            Console.WriteLine("Batch write successfully completed.");
        }

        public static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.Write(Usage);
                return;
            }
            string command = args[0].ToLower();
            string project = string.Join(" ",
                new ArraySegment<string>(args, 1, args.Length - 1));
            switch (command)
            {
                case "run-simple-transaction":
                    RunSimpleTransaction(project).Wait();
                    break;

                case "return-info-transaction":
                    ReturnInfoTransaction(project).Wait();
                    break;

                case "batch-write":
                    BatchWrite(project).Wait();
                    break;

                default:
                    Console.Write(Usage);
                    return;
            }
        }
    }
}
