// Copyright 2022 Google LLC. All Rights Reserved.
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
using System;
using System.Collections.Generic;
using System.Linq;


/// <summary>
/// Search with pagination sample class.
/// </summary>
public class SearchWithPaginationSample
{
    /// <summary>Get search request.</summary>
    private static SearchRequest GetSearchRequest(string query, int pageSize, int offset, string nextPageToken, string projectNumber)
    {
        string defaultSearchPlacement = $"projects/{projectNumber}/locations/global/catalogs/default_catalog/placements/default_search";

        var searchRequest = new SearchRequest()
        {
            Placement = defaultSearchPlacement, // Placement is used to identify the Serving Config name
            VisitorId = "123456", // A unique identifier to track visitors
            Query = query,
            PageSize = pageSize,
            Offset = offset,
            PageToken = nextPageToken
        };

        Console.WriteLine("Search. request:");
        Console.WriteLine($"Placement: {searchRequest.Placement}");
        Console.WriteLine($"Query: {searchRequest.Query}");
        Console.WriteLine($"VisitorId: {searchRequest.VisitorId}");
        Console.WriteLine($"PageSize: {searchRequest.PageSize}");
        Console.WriteLine();

        return searchRequest;
    }

    /// <summary>
    /// Call the retail search.
    /// </summary>
    /// <param name="projectNumber">Current project number.</param>
    /// <returns></returns>
    public IEnumerable<SearchResponse> Search(string projectNumber)
    {
        // Try different pagination parameters here:
        int pageSize = 6;
        int offset = 0;
        string nextPageToken = "";
        string query = "Hoodie";

        SearchServiceClient client = SearchServiceClient.Create();
        SearchRequest searchRequest = GetSearchRequest(query, pageSize, offset, nextPageToken, projectNumber);
        IEnumerable<SearchResponse> searchResultPages = client.Search(searchRequest).AsRawResponses();
        SearchResponse firstPage = searchResultPages.FirstOrDefault();

        if (firstPage is null)
        {
            Console.WriteLine("The search operation returned no matching results.");
        }
        else
        {
            Console.WriteLine("Search results:");
            Console.WriteLine($"AttributionToken: {firstPage.AttributionToken},");
            Console.WriteLine($"NextPageToken: {firstPage.NextPageToken},");
            Console.WriteLine($"TotalSize: {firstPage.TotalSize},");
            Console.WriteLine("Items found in first page:");

            foreach (SearchResponse.Types.SearchResult item in firstPage)
            {
                Console.WriteLine(item);
            }
        }

        // Paste call with next page token here:

        //nextPageToken = firstPage.NextPageToken;
        //searchRequest = GetSearchRequest(query, pageSize, offset, nextPageToken, projectNumber);
        //searchResultPages = client.Search(searchRequest).AsRawResponses();

        //var secondSearchResponse = searchResultPages.FirstOrDefault();

        //if (secondSearchResponse is null)
        //{
        //    Console.WriteLine("The search operation returned no matching results.");
        //}
        //else
        //{
        //    Console.WriteLine("Search results:");
        //    Console.WriteLine($"AttributionToken: {firstPage.AttributionToken},");
        //    Console.WriteLine($"NextPageToken: {firstPage.NextPageToken},");
        //    Console.WriteLine($"TotalSize: {firstPage.TotalSize},");
        //    Console.WriteLine("Items found in first page:");

        //    foreach (SearchResponse.Types.SearchResult item in firstPage)
        //    {
        //        Console.WriteLine(item);
        //    }
        //}

        // Paste call with offset here:

        //offset = 9;
        //searchRequest = GetSearchRequest(query, pageSize, offset, nextPageToken, projectNumber);
        //searchResultPages = client.Search(searchRequest).AsRawResponses();

        //var thirdSearchResponse = searchResultPages.FirstOrDefault();

        //if (thirdSearchResponse is null)
        //{
        //    Console.WriteLine("The search operation returned no matching results.");
        //}
        //else
        //{
        //    Console.WriteLine("Search results:");
        //    Console.WriteLine($"AttributionToken: {firstPage.AttributionToken},");
        //    Console.WriteLine($"NextPageToken: {firstPage.NextPageToken},");
        //    Console.WriteLine($"TotalSize: {firstPage.TotalSize},");
        //    Console.WriteLine("Items found in first page:");

        //    foreach (SearchResponse.Types.SearchResult item in firstPage)
        //    {
        //        Console.WriteLine(item);
        //    }
        //}

        return searchResultPages;
    }
}
// [END retail_search_for_products_with_pagination]

/// <summary>
/// Search with pagination tutorial.
/// </summary>
public static class SearchWithPaginationTutorial
{
    [Runner.Attributes.Example]
    public static IEnumerable<SearchResponse> Search()
    {
        var projectNumber = Environment.GetEnvironmentVariable("PROJECT_NUMBER");
        var sample = new SearchWithPaginationSample();
        return sample.Search(projectNumber);
    }
}