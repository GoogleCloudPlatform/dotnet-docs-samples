# Copyright 2017 Google Inc. All Rights Reserved.
#
# Licensed under the Apache License, Version 2.0 (the "License");
# you may not use this file except in compliance with the License.
# You may obtain a copy of the License at
#
#     http://www.apache.org/licenses/LICENSE-2.0
#
# Unless required by applicable law or agreed to in writing, software
# distributed under the License is distributed on an "AS IS" BASIS,
# WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
# See the License for the specific language governing permissions and
# limitations under the License.

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Newtonsoft.Json;

namespace prediction_client
{
    class Person
    {
        public int age { get; set; }
        public string workclass { get; set; }
        public string education { get; set; }
        public int education_num { get; set; }
        public string marital_status { get; set; }
        public string occupation { get; set; }
        public string relationship { get; set; }
        public string race { get; set; }
        public string gender { get; set; }
        public int capital_gain { get; set; }
        public int capital_loss { get; set; }
        public int hours_per_week { get; set; }
        public string native_country { get; set; }
    }

    class MainClass
    {
        public static void Main(string[] args)
        {
            RunAsync().Wait();
        }

	static string project = "YOUR_PROJECT";
	static string model = "census";  // Name of deployed model
		
        static HttpClient client = new HttpClient();

        static async Task RunAsync()
        {
            client.BaseAddress = new Uri("https://ml.googleapis.com/v1/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            try
            {
                Person person = new Person
                {
                    age = 25,
                    workclass = " Private",
                    education = " 11th",
                    education_num = 7,
                    marital_status = " Never - married",
                    occupation = " Machine - op - inspct",
                    relationship = " Own - child",
                    race = " Black",
                    gender = " Male",
                    capital_gain = 0,
                    capital_loss = 0,
                    hours_per_week = 40,
                    native_country = " United - Stats"
                };
                var instances = new List<Person> { person };

                var predictions = await Predict(project, model, instances);
                Console.WriteLine(predictions);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            Console.ReadLine();
        }

        // [START predict_json]
        static async Task<dynamic> Predict(String project, String model, List<Person> instances, String version = null)
        {
            GoogleCredential credential = await GoogleCredential.GetApplicationDefaultAsync();
            var bearer_token = await credential.UnderlyingCredential.GetAccessTokenForRequestAsync();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearer_token);

            var version_suffix = version == null ? "" : $"/version/{version}";
            var model_uri = $"projects/{project}/models/{model}{version_suffix}";
			var predict_uri = $"{model_uri}:predict";

            var request = new { instances = instances };
            var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

            var responseMessage = await client.PostAsync(predict_uri, content);
            responseMessage.EnsureSuccessStatusCode();

            var responseBody = await responseMessage.Content.ReadAsStringAsync();
            dynamic response = JsonConvert.DeserializeObject(responseBody);

            return response.predictions;
        }
        // [END predict_json]
    }
}

