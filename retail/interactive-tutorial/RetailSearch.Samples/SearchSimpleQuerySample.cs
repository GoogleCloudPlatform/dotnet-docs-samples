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

using Google.Api.Gax;
using Google.Cloud.Retail.V2;
using System;
using System.Collections.Generic;
using System.Linq;

// [START retail_search_for_products_with_query_parameter]
// Call Retail API to search for a products in a catalog using only search query.

/// <summary>
/// Search simple query sample class.
/// </summary>
public class SearchSimpleQuerySample
{
    /// <summary>Get search request.</summary>
    private SearchRequest GetSearchRequest(string query, string projectNumber)
    {
        string defaultSearchPlacement = $"projects/{projectNumber}/locations/global/catalogs/default_catalog/placements/default_search";

        var searchRequest = new SearchRequest
        {
            Placement = defaultSearchPlacement, // Placement is used to identify the Serving Config name
            Query = query,
            VisitorId = "123456", // A unique identifier to track visitors
            PageSize = 10
        };

        Console.WriteLine("Search. request:");
        Console.WriteLine($"Placement: {searchRequest.Placement}");
        Console.WriteLine($"Query: {searchRequest.Query}");
        Console.WriteLine($"VisitorId: {searchRequest.VisitorId}");
        Console.WriteLine($"PageSize: {searchRequest.PageSize}");

        return searchRequest;
    }

    public IEnumerable<SearchResponse> Search(string projectNumber)
    {
        // Try different query phrases here:
        var query = "Hoodie";

        SearchServiceClient client = SearchServiceClient.Create();
        SearchRequest searchRequest = GetSearchRequest(query, projectNumber);
        var searchResponses = client.Search(searchRequest).AsRawResponses();

        var firstSearchResponse = searchResponses.FirstOrDefault();

        Console.WriteLine("Search. response:");

        if (firstSearchResponse != null)
        {
            Console.WriteLine($"Results: {firstSearchResponse.Results}");
            Console.WriteLine($"TotalSize: {firstSearchResponse.TotalSize},");
            Console.WriteLine($"AttributionToken: {firstSearchResponse.AttributionToken},");
            Console.WriteLine($"NextPageToken: {firstSearchResponse.NextPageToken},");
        }

        return searchResponses;
    }
}
// [END retail_search_for_products_with_query_parameter]

/// <summary>
/// Search simple query tutorial.
/// </summary>
public static class SearchSimpleQueryTutorial
{
    [RetailSearch.Samples.Attributes.Example]
    public static IEnumerable<SearchResponse> Search()
    {
        var projectNumber = Environment.GetEnvironmentVariable("PROJECT_NUMBER");
        var sample = new SearchSimpleQuerySample();
        return sample.Search(projectNumber);
    }
}