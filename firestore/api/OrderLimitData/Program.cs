// Copyright(c) 2017 Google Inc.
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
using System.Threading;
using System.Linq;

namespace GoogleCloudSamples
{
    public class OrderLimitData
    {
        public static string Usage = @"Usage:
C:\> dotnet run command YOUR_PROJECT_ID

Where command is one of
    order-by-name-limit-query
    order-by-name-desc-limit-query
    order-by-state-and-population-query
    where-order-by-limit-query
    range-order-by-query
    invalid-range-order-by-query
";

        private static async Task OrderByNameLimitQuery(string project)
        {
            FirestoreDb db = FirestoreDb.Create(project);
            CollectionReference citiesRef = db.Collection("cities");
            // [START fs_order_by_name_limit_query]
            Query query = citiesRef.OrderBy("Name").Limit(3);
            // [END fs_order_by_name_limit_query]
            QuerySnapshot querySnapshot = await query.SnapshotAsync();
            foreach (DocumentSnapshot documentSnapshot in querySnapshot.Documents)
            {
                Console.WriteLine("Document {0} returned by order by name with limit query", documentSnapshot.Id);
            }
        }

        private static async Task OrderByNameDescLimitQuery(string project)
        {
            FirestoreDb db = FirestoreDb.Create(project);
            CollectionReference citiesRef = db.Collection("cities");
            // [START fs_order_by_name_desc_limit_query]
            Query query = citiesRef.OrderByDescending("Name").Limit(3);
            // [END fs_order_by_name_desc_limit_query]
            QuerySnapshot querySnapshot = await query.SnapshotAsync();
            foreach (DocumentSnapshot documentSnapshot in querySnapshot.Documents)
            {
                Console.WriteLine("Document {0} returned by order by name descending with limit query", documentSnapshot.Id);
            }
        }

        private static async Task OrderByStateAndPopulationQuery(string project)
        {
            FirestoreDb db = FirestoreDb.Create(project);
            CollectionReference citiesRef = db.Collection("cities");
            // [START fs_order_by_state_and_population_query]
            Query query = citiesRef.OrderBy("State").OrderByDescending("Population");
            // [END fs_order_by_state_and_population_query]
            QuerySnapshot querySnapshot = await query.SnapshotAsync();
            foreach (DocumentSnapshot documentSnapshot in querySnapshot.Documents)
            {
                Console.WriteLine("Document {0} returned by order by state and descending population query", documentSnapshot.Id);
            }
        }

        private static async Task WhereOrderByLimitQuery(string project)
        {
            FirestoreDb db = FirestoreDb.Create(project);
            CollectionReference citiesRef = db.Collection("cities");
            // [START fs_where_order_by_limit_query]
            Query query = citiesRef
                .Where("Population", QueryOperator.GreaterThan, 2500000)
                .OrderBy("Population")
                .Limit(2);
            // [END fs_where_order_by_limit_query]
            QuerySnapshot querySnapshot = await query.SnapshotAsync();
            foreach (DocumentSnapshot documentSnapshot in querySnapshot.Documents)
            {
                Console.WriteLine("Document {0} returned by where order by limit query", documentSnapshot.Id);
            }
        }

        private static async Task RangeOrderByQuery(string project)
        {
            FirestoreDb db = FirestoreDb.Create(project);
            CollectionReference citiesRef = db.Collection("cities");
            // [START fs_range_order_by_query]
            Query query = citiesRef
                .Where("Population", QueryOperator.GreaterThan, 2500000)
                .OrderBy("Population");
            // [END fs_range_order_by_query]
            QuerySnapshot querySnapshot = await query.SnapshotAsync();
            foreach (DocumentSnapshot documentSnapshot in querySnapshot.Documents)
            {
                Console.WriteLine("Document {0} returned by range with order by query", documentSnapshot.Id);
            }
        }

        private static void InvalidRangeOrderByQuery(string project)
        {
            FirestoreDb db = FirestoreDb.Create(project);
            CollectionReference citiesRef = db.Collection("cities");
            // [START fs_invalid_range_order_by_query]
            Query query = citiesRef
                .Where("Population", QueryOperator.GreaterThan, 2500000)
                .OrderBy("Country");
            // [END fs_invalid_range_order_by_query]
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
                case "order-by-name-limit-query":
                    OrderByNameLimitQuery(project).Wait();
                    break;

                case "order-by-name-desc-limit-query":
                    OrderByNameDescLimitQuery(project).Wait();
                    break;

                case "order-by-state-and-population-query":
                    OrderByStateAndPopulationQuery(project).Wait();
                    break;

                case "where-order-by-limit-query":
                    WhereOrderByLimitQuery(project).Wait();
                    break;

                case "range-order-by-query":
                    RangeOrderByQuery(project).Wait();
                    break;

                case "invalid-range-order-by-query":
                    InvalidRangeOrderByQuery(project);
                    break;

                default:
                    Console.Write(Usage);
                    return;
            }
        }
    }
}
