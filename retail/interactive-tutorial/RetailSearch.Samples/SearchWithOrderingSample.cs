// Copyright 2022 Google LLC.
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

// Call Retail API to search for a products in a catalog, order the results by different product fields.
// [START retail_search_for_products_with_ordering]

using Google.Cloud.Retail.V2;
using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Search with ordering sample class.
/// </summary>
public class SearchWithOrderingSample
{
    /// <summary>
    /// Get search request.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <param name="order">The order expression.</param>
    /// <param name="projectId">The current project id.</param>
    /// <returns>The search request.</returns>
    private static SearchRequest GetSearchRequest(string query, string order, string projectId)
    {
        string defaultSearchPlacement = $"projects/{projectId}/locations/global/catalogs/default_catalog/placements/default_search";

        var searchRequest = new SearchRequest()
        {
            Placement = defaultSearchPlacement, // Placement is used to identify the Serving Config name
            Query = query,
            OrderBy = order,
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
        // Try different ordering expressions here:
        string order = "price desc";
        string query = "Hoodie";

        SearchServiceClient client = SearchServiceClient.Create();
        SearchRequest searchRequest = GetSearchRequest(query, order, projectId);
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

// [END retail_search_for_products_with_ordering]

/// <summary>
/// Search with ordering tutorial.
/// </summary>
public static class SearchWithOrderingTutorial
{
    [Runner.Attributes.Example]
    public static IEnumerable<SearchResponse> Search()
    {
        var projectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
        var sample = new SearchWithOrderingSample();
        return sample.Search(projectId);
    }
}
