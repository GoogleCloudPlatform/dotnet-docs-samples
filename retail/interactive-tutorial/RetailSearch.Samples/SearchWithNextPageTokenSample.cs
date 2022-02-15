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

// [START retail_search_for_products_with_next_page_token]
// Call Retail API to search for a products in a catalog,
// limit the number of the products per page and go to the next page using "next_page_token"
// or jump to chosen page using "offset".

using Google.Cloud.Retail.V2;
using System;
using System.Collections.Generic;
using System.Linq;


/// <summary>
/// Search with next page token sample class.
/// </summary>
public class SearchWithNextPageTokenSample
{
    /// <summary>Get search request.</summary>
    private static SearchRequest GetSearchRequest(string query, string nextPageToken, string projectId)
    {
        string defaultSearchPlacement = $"projects/{projectId}/locations/global/catalogs/default_catalog/placements/default_search";

        var searchRequest = new SearchRequest()
        {
            Placement = defaultSearchPlacement, // Placement is used to identify the Serving Config name
            VisitorId = "123456", // A unique identifier to track visitors
            Query = query,
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
    /// <param name="projectNumber">Current project id.</param>
    /// <returns></returns>
    public IEnumerable<SearchResponse> Search(string projectId)
    {
        // Call with empty next page token:
        string nextPageToken = "";
        IEnumerable<SearchResponse> firstSearchResultPages = SearchWithNextPageToken(nextPageToken, projectId);

        // Call with valid next page token:
        nextPageToken = firstSearchResultPages.First().NextPageToken;
        SearchWithNextPageToken(nextPageToken, projectId);

        return firstSearchResultPages;
    }

    /// <summary>
    /// Call the retail search with nextPageToken.
    /// </summary>
    /// <param name="nextPageToken">The next page token.</param>
    /// <param name="projectId">The current project id.</param>
    /// <returns></returns>
    private IEnumerable<SearchResponse> SearchWithNextPageToken(string nextPageToken, string projectId)
    {
        string query = "Hoodie";

        SearchServiceClient client = SearchServiceClient.Create();
        SearchRequest searchRequest = GetSearchRequest(query, nextPageToken, projectId);
        IEnumerable<SearchResponse> firstSearchResultPages = client.Search(searchRequest).AsRawResponses();
        SearchResponse firstPage = firstSearchResultPages.FirstOrDefault();

        if (firstPage is null)
        {
            Console.WriteLine("The search operation returned no matching results.");
        }
        else
        {
            Console.WriteLine("First page search results:");
            Console.WriteLine($"AttributionToken: {firstPage.AttributionToken},");
            Console.WriteLine($"NextPageToken: {firstPage.NextPageToken},");
            Console.WriteLine($"TotalSize: {firstPage.TotalSize},");
            Console.WriteLine("Items found in first page:");

            foreach (SearchResponse.Types.SearchResult item in firstPage)
            {
                Console.WriteLine(item);
            }
        }

        return firstSearchResultPages;
    }
}
// [END retail_search_for_products_with_next_page_token]

/// <summary>
/// Search with next page token tutorial.
/// </summary>
public static class SearchWithNextPageTokenTutorial
{
    [Runner.Attributes.Example]
    public static IEnumerable<SearchResponse> Search()
    {
        var projectId = Environment.GetEnvironmentVariable("PROJECT_ID");
        var sample = new SearchWithNextPageTokenSample();
        return sample.Search(projectId);
    }
}