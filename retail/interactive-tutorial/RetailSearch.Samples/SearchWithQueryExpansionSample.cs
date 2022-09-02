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

// [START retail_search_for_products_with_query_expansion_specification]
// Call Retail API to search for a products in a catalog,
// enabling the query expansion feature to let the Google Retail Search build an automatic query expansion.

using Google.Cloud.Retail.V2;
using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Search with query expansion sample class.
/// </summary>
public class SearchWithQueryExpansionSample 
{
    /// <summary>
    /// Get search request.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <param name="condition">The query expansion condition.</param>
    /// <param name="projectId">The current project id.</param>
    /// <returns>The search request.</returns>
    private static SearchRequest GetSearchRequest(string query, SearchRequest.Types.QueryExpansionSpec.Types.Condition condition, string projectId)
    { 
        string defaultSearchPlacement = $"projects/{projectId}/locations/global/catalogs/default_catalog/placements/default_search";

        var searchRequest = new SearchRequest()
        {
            Placement = defaultSearchPlacement, // Placement is used to identify the Serving Config name
            Query = query,
            QueryExpansionSpec = new SearchRequest.Types.QueryExpansionSpec { Condition = condition },
            VisitorId = "123456", // A unique identifier to track visitors
            PageSize = 10
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
    /// <returns>Search result pages.</returns>
    public IEnumerable<SearchResponse> Search(string projectId)
    {
        // Try different query expansion condition here:
        var condition = SearchRequest.Types.QueryExpansionSpec.Types.Condition.Auto;
        string query = "Google Youth Hero Tee Grey";

        SearchServiceClient client = SearchServiceClient.Create();
        SearchRequest searchRequest = GetSearchRequest(query, condition, projectId);
        IEnumerable<SearchResponse> searchResultPages = client.Search(searchRequest).AsRawResponses();
        SearchResponse firstPage = searchResultPages.First();

        if (firstPage.TotalSize == 0)
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

            int itemCount = 0;
            foreach (SearchResponse.Types.SearchResult item in firstPage)
            {
                itemCount++;
                Console.WriteLine($"Item {itemCount}: ");
                Console.WriteLine(item);
                Console.WriteLine();
            }
        }

        return searchResultPages;
    }
}
// [END retail_search_for_products_with_query_expansion_specification]

/// <summary>
/// Search with query expansion tutorial.
/// </summary>
public static class SearchWithQueryExpansionTutorial
{
    [Runner.Attributes.Example]
    public static IEnumerable<SearchResponse> Search()
    {
        var projectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
        var sample = new SearchWithQueryExpansionSample();
        return sample.Search(projectId);
    }
}