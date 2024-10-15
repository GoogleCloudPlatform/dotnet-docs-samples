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

// Calls the Retail API to search for products in a catalog and reranks the
// results boosting or burying the products that matched a given condition.

using Google.Cloud.Retail.V2;
using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Search with boot spec sample class.
/// </summary>
public class SearchWithBoostSpecSample
{
    /// <summary>
    /// Get search request.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <param name="condition">The required condition.</param>
    /// <param name="boostStrength">The boost strength.</param>
    /// <param name="projectId">The current project id.</param>
    /// <returns>The search request.</returns>
    private SearchRequest GetSearchRequest(string query, string condition, float boostStrength, string projectId)
    {
        string defaultSearchPlacement = $"projects/{projectId}/locations/global/catalogs/default_catalog/placements/default_search";

        var searchRequest = new SearchRequest
        {
            Placement = defaultSearchPlacement, // Placement is used to identify the Serving Config name
            Query = query,
            VisitorId = "123456", // A unique identifier to track visitors
            PageSize = 10,
            BoostSpec = new SearchRequest.Types.BoostSpec
            {
                ConditionBoostSpecs =
                {
                    new SearchRequest.Types.BoostSpec.Types.ConditionBoostSpec
                    {
                        Condition = condition,
                        Boost = boostStrength
                    }
                }
            }
        };

        Console.WriteLine("Search request:");
        Console.WriteLine($"Placement: {searchRequest.Placement}");
        Console.WriteLine($"Query: {searchRequest.Query}");
        Console.WriteLine($"VisitorId: {searchRequest.VisitorId}");
        Console.WriteLine($"PageSize: {searchRequest.PageSize}");
        Console.WriteLine($"BoostSpec: {searchRequest.BoostSpec}");
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
        // Try different conditions here:
        string condition = "colorFamilies: ANY(\"Blue\")";
        float boost = 0.0f;
        string query = "Tee";

        SearchServiceClient client = SearchServiceClient.Create();
        SearchRequest searchRequest = GetSearchRequest(query, condition, boost, projectId);
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

/// <summary>
/// Search with boost spec tutorial.
/// </summary>
public static class SearchWithBoostSpecTutorial
{
    [Runner.Attributes.Example]
    public static IEnumerable<SearchResponse> Search()
    {
        var projectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
        var sample = new SearchWithBoostSpecSample();
        return sample.Search(projectId);
    }
}
