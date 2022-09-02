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

using Google.Api.Gax;
using Google.Cloud.Retail.V2;
using System;
using System.Linq;

/// <summary>
/// Search with next page token sample class.
/// </summary>
public class SearchWithNextPageTokenSample
{
    /// <summary>
    /// Get search request.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <param name="nextPageToken">The next page token.</param>
    /// <param name="pageSize">The size of the page.</param>
    /// <param name="projectId">The current project id.</param>
    /// <returns>The search request.</returns>
    private static SearchRequest GetSearchRequest(string query, string nextPageToken, int pageSize, string projectId)
    {
        string defaultSearchPlacement = $"projects/{projectId}/locations/global/catalogs/default_catalog/placements/default_search";

        var searchRequest = new SearchRequest()
        {
            Placement = defaultSearchPlacement, // Placement is used to identify the Serving Config name
            VisitorId = "123456", // A unique identifier to track visitors
            Query = query,
            PageSize = pageSize,
            PageToken = nextPageToken
        };

        Console.WriteLine("Search request:");
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
    /// <param name="projectId">Current project id.</param>
    /// <returns>First page with results.</returns>
    public Page<SearchResponse.Types.SearchResult> Search(string projectId)
    {
        // Call with empty next page token the first time:
        string nextPageToken = "";
        Page<SearchResponse.Types.SearchResult> firstPage = SearchWithNextPageToken(nextPageToken, projectId);

        // Call with valid next page token:
        nextPageToken = firstPage.NextPageToken;
        Page<SearchResponse.Types.SearchResult> secondPage = SearchWithNextPageToken(nextPageToken, projectId);

        return firstPage;
    }

    /// <summary>
    /// Call the retail search with nextPageToken.
    /// </summary>
    /// <param name="nextPageToken">The next page token.</param>
    /// <param name="projectId">The current project id.</param>
    /// <returns>First page with results.</returns>
    private Page<SearchResponse.Types.SearchResult> SearchWithNextPageToken(string nextPageToken, string projectId)
    {
        int pageSize = 10;
        string query = "Hoodie";

        SearchServiceClient client = SearchServiceClient.Create();
        SearchRequest searchRequest = GetSearchRequest(query, nextPageToken, pageSize, projectId);
        PagedEnumerable<SearchResponse, SearchResponse.Types.SearchResult> searchResultPages = client.Search(searchRequest);
        Page<SearchResponse.Types.SearchResult> singlePage = searchResultPages.ReadPage(pageSize);

        if (!singlePage.Any())
        {
            Console.WriteLine("The search operation returned no matching results.");
            Console.WriteLine();
        }
        else
        {
            Console.WriteLine($"NextPageToken: {singlePage.NextPageToken}");
            Console.WriteLine("Items found in page:");

            int itemCount = 0;
            foreach (SearchResponse.Types.SearchResult item in singlePage)
            {
                itemCount++;
                Console.WriteLine($"Item {itemCount}: ");
                Console.WriteLine(item);
                Console.WriteLine();
            }
        }

        return singlePage;
    }
}
// [END retail_search_for_products_with_next_page_token]

/// <summary>
/// Search with next page token tutorial.
/// </summary>
public static class SearchWithNextPageTokenTutorial
{
    [Runner.Attributes.Example]
    public static Page<SearchResponse.Types.SearchResult> Search()
    {
        var projectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
        var sample = new SearchWithNextPageTokenSample();
        return sample.Search(projectId);
    }
}