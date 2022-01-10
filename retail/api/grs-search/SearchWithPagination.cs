// Copyright 2021 Google Inc. All Rights Reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

// [START retail_search_for_products_with_pagination]
// Call Retail API to search for a products in a catalog,
// limit the number of the products per page and go to the next page using "next_page_token"
// or jump to chosen page using "offset".

using Google.Cloud.Retail.V2;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace grs_search.search
{
    public static class SearchWithPagination
    {
        private const string Endpoint = "retail.googleapis.com";
        private static readonly string ProjectNumber = Environment.GetEnvironmentVariable("PROJECT_NUMBER");
        private static readonly string DefaultSearchPlacement = $"projects/{ProjectNumber}/locations/global/catalogs/default_catalog/placements/default_search";

        // Get search service client
        private static SearchServiceClient GetSearchServiceClient()
        {
            var searchServiceClientBuilder = new SearchServiceClientBuilder
            {
                Endpoint = Endpoint
            };

            var searchServiceClient = searchServiceClientBuilder.Build();
            return searchServiceClient;
        }

        // Get search service request
        private static SearchRequest GetSearchRequest(string query, int pageSize, int offset, string nextPageToken)
        {
            var searchRequest = new SearchRequest()
            {
                Placement = DefaultSearchPlacement, // Placement is used to identify the Serving Config name
                VisitorId = "123456", // A unique identifier to track visitors
                Query = query,
                PageSize = pageSize,
                Offset = offset,
                PageToken = nextPageToken
            };

            var jsonSerializeSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Formatting = Formatting.Indented
            };

            var searchRequestJson = JsonConvert.SerializeObject(searchRequest, jsonSerializeSettings);

            Console.WriteLine("Search. request: \n\n" + searchRequestJson);

            return searchRequest;
        }

        // Call the Retail Search:
        [Attributes.Example]
        public static IEnumerable<SearchResponse> Search()
        {
            // TRY DIFFERENT PAGINATION PARAMETERS HERE:
            int pageSize = 6;
            int offset = 0;
            string nextPageToken = "";
            string query = "Hoodie";

            var searchRequest = GetSearchRequest(query, pageSize, offset, nextPageToken);
            var searchResponse = GetSearchServiceClient().Search(searchRequest).AsRawResponses();

            Console.WriteLine("\nSearch. response: \n");

            var jsonSerializeSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Formatting = Formatting.Indented
            };

            var firstSearchResponse = searchResponse.FirstOrDefault();

            if (firstSearchResponse != null)
            {
                var objectToSerialize = new
                {
                    results = firstSearchResponse.Results,
                    totalSize = firstSearchResponse.TotalSize,
                    attributionToken = firstSearchResponse.AttributionToken,
                    nextPageToken = firstSearchResponse.NextPageToken,
                    facets = firstSearchResponse.Facets,
                    queryExpansionInfo = firstSearchResponse.QueryExpansionInfo
                };

                var serializedJson = JsonConvert.SerializeObject(objectToSerialize, jsonSerializeSettings);

                Console.WriteLine(serializedJson + "\n");
            }

            // PASTE CALL WITH NEXT PAGE TOKEN HERE:
            // var searchResponse = GetSearchServiceClient().Search(searchRequest);

            // PASTE CALL WITH OFFSET HERE:
            // var searchResponse = GetSearchServiceClient().Search(searchRequest);

            return searchResponse;
        }
    }
}
// [END retail_search_for_products_with_pagination]