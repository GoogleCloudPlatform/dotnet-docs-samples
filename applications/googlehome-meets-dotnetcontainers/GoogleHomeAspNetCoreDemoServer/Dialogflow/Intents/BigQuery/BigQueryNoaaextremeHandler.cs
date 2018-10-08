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
using Google.Cloud.BigQuery.V2;
using Google.Cloud.Dialogflow.V2;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace GoogleHomeAspNetCoreDemoServer.Dialogflow.Intents.BigQuery
{
    /// <summary>
    /// Handler for all "bigquery.noaaextreme" DialogFlow intent.
    /// </summary>
    [Intent("bigquery.noaaextreme")]
    public class BigQueryNoaaextremeHandler : BaseBigQueryHandler
    {
        /// <summary>
        /// Initializes class with the conversation.
        /// </summary>
        /// <param name="conversation">Conversation</param>
        public BigQueryNoaaextremeHandler(Conversation conversation) : base(conversation)
        {
        }

        /// <summary>
        /// Handle the intent.
        /// </summary>
        /// <param name="req">Webhook request</param>
        /// <returns>Webhook response</returns>
        public override async Task<WebhookResponse> HandleAsync(WebhookRequest req)
        {
            var errorMessage = ExtractAndValidateParameters(req, out string hottestOrColdest, out string year, 
                out string countryName, out string fipsCountry);

            if (errorMessage != null) 
            {
                return new WebhookResponse
                {
                    FulfillmentText = errorMessage
                };
            }

            var bigQueryClient = await BigQueryClient.CreateAsync(Program.AppSettings.GoogleCloudSettings.ProjectId);

            // Build the parameterized SQL query
            var table = bigQueryClient.GetTable("bigquery-public-data", "noaa_gsod", $"gsod*");
            var tableStations = bigQueryClient.GetTable("bigquery-public-data", "noaa_gsod", "stations");
            var sql = $"SELECT (gsod.temp - 32)*5/8 AS celcius, stations.name AS name, gsod.year AS y, gsod.mo AS m, gsod.da AS d \n" +
                $"FROM {table} AS gsod \n" +
                $"JOIN {tableStations} AS stations ON gsod.stn = stations.usaf AND gsod.wban = stations.wban \n" +
                $"WHERE stations.country=@fipsCountry and gsod.year=@year \n" +
                $"ORDER BY gsod.temp{(hottestOrColdest == "hottest" ? " DESC" : "")} \n" +
                "limit 10";

            // Create the BigQuery parameters
            var parameters = new[]
            {
                new BigQueryParameter("fipsCountry", BigQueryDbType.String, fipsCountry),
                new BigQueryParameter("year", BigQueryDbType.String, year)
            };

            var (resultList, processedMb, secs) = await RunQueryAsync(bigQueryClient, sql, parameters);

            // Check if there's data.
            if (resultList.Count == 0)
            {
                return new WebhookResponse
                {
                    FulfillmentText = "Sorry, there is no data."
                };
            }

            // Show SQL query and query results in browser
            var temperatures = resultList
                  .Select(x => $"{(double)x["celcius"]:0.0}&deg;C at {x["name"]} on {x["y"]}-{x["m"]}-{x["d"]}")
                  .ToList();

            ShowQuery(sql, parameters, (processedMb, secs, temperatures));

            // Send spoken response to DialogFlow
            var top = resultList[0];
            return new WebhookResponse
            {
                FulfillmentText = $"Scanned {processedMb} mega-bytes in {secs:0.0} seconds. " +
                $"The {hottestOrColdest} temperature in {countryName} in the year {year} was " +
                $"{(double)top["celcius"]:0.0} degrees celcius, at the {top["name"]} monitoring station."
            };
        }

        /// <summary>
        /// Given a conversation request, extracts and validates required parameters.
        /// </summary>
        /// <param name="req">Conversation request</param>
        /// <param name="hottestOrColdest">Hottest or coldest temperature indicator</param>
        /// <param name="year">Year between 1929 and 2017</param>
        /// <param name="countryName">Country name</param>
        /// <param name="fipsCountry"></param>
        /// <returns></returns>
        private static string ExtractAndValidateParameters(WebhookRequest req, out string hottestOrColdest, out string year, 
            out string countryName, out string fipsCountry)
        {
            var fields = req.QueryResult.Parameters.Fields;
            hottestOrColdest = fields["hottest-or-coldest"].StringValue.ToLowerInvariant();

            // Extract the day from DialogFlow date
            var datePeriod = fields["date"].StructValue;
            var startDate = datePeriod.Fields["startDate"].StringValue;
            var dateTime = DateTime.Parse(startDate);
            year = dateTime.ToString("yyyy");
          
            var country = fields["country"].StructValue;
            var countryCode = country.Fields["alpha-2"].StringValue;
            countryName = country.Fields["name"].StringValue;
            fipsCountry = FipsIsoCountryMap.Map(countryCode.ToUpperInvariant());

            if (!int.TryParse(year, out int yearInt) || yearInt < 1929 || yearInt > 2017)
            {
                return "Sorry, the year specified is invalid or out of range.";
            }

            if (fipsCountry == null)
            {
                return "Sorry, cannot recognize country: '{countryName}'";
            }

            return null;
        }
    }
}
