/*
 * Copyright (c) 2019 Google LLC.
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

using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace SolutionCounter.Classes
{
    /// <summary>
    /// Defines the <see cref="Startup" />
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// The Start
        /// </summary>
        public static void Start()
        {
            try
            {
                var api = new ApiFacade();

                api.InitFireStoreDb();

                api.InitShardsCounter(2);

                var docRef = api.FireStoreDb.Collection("counter_samples1").Document("DCounter");

                Task.Run(async () =>
                {
                    Console.WriteLine("Application start ...");

                    await api.InitCounterAsync(docRef);

                }).ContinueWith(async t =>
                {
                    await api.IncrementCounterAsync(docRef);

                }).ContinueWith(async r =>
                {
                    await Task.Delay(5000);

                    var countTotal = api.GetCount(docRef);

                    Console.WriteLine(countTotal);

                    Console.WriteLine("Application stopped ...");
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
    }
}
