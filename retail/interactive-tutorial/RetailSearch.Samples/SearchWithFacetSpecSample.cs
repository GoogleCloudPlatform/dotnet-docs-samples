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

// [START retail_search_product_with_facet_spec]

using Google.Api.Gax;
using Google.Cloud.Retail.V2;
using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Search with facet spec sample class.
/// </summary>
public class SearchWithFacetSpecSample
{
    /// <summary>Get search request.</summary>
    private SearchRequest GetSearchRequest(string query, string facetKeyParam, string projectNumber)
    {
        string defaultSearchPlacement = $"projects/{projectNumber}/locations/global/catalogs/default_catalog/placements/default_search";

        var facetKey = new SearchRequest.Types.FacetSpec.Types.FacetKey
        {
            Key = facetKeyParam
        };

        // Put the intervals here:
        
        var searchRequest = new SearchRequest
        {
            Placement = defaultSearchPlacement, // Placement is used to identify the Serving Config name
            Query = query,
            VisitorId = "123456", // A unique identifier to track visitors
            PageSize = 10,
            FacetSpecs =
            {
                new SearchRequest.Types.FacetSpec
                {
                    FacetKey = facetKey
                }
            }
        };

        Console.WriteLine("Search. request:");
        Console.WriteLine($"Placement: {searchRequest.Placement}");
        Console.WriteLine($"Query: {searchRequest.Query}");
        Console.WriteLine($"VisitorId: {searchRequest.VisitorId}");
        Console.WriteLine($"PageSize: {searchRequest.PageSize}");
        Console.WriteLine($"FacetSpecs: {searchRequest.FacetSpecs}");
        Console.WriteLine();

        return searchRequest;
    }

    public IEnumerable<SearchResponse> Search(string projectNumber)
    {
        // Try different facets here:
        string facetKey = "colorFamilies";
        string query = "Tee";

        SearchServiceClient client = SearchServiceClient.Create();
        SearchRequest searchRequest = GetSearchRequest(query, facetKey, projectNumber);
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
            Console.WriteLine($"Facets: {firstPage.Facets}");
            Console.WriteLine($"TotalSize: {firstPage.TotalSize},");
            Console.WriteLine("Items found in first page:");
            foreach (SearchResponse.Types.SearchResult item in firstPage)
            {
                Console.WriteLine(item);
            }
        }

        return searchResultPages;
    }
}
// [END retail_search_product_with_facet_spec]

/// <summary>
/// Search with facet spec tutorial.
/// </summary>
public static class SearchWithFacetSpecTutorial
{
    [Runner.Attributes.Example]
    public static IEnumerable<SearchResponse> Search()
    {
        var projectNumber = Environment.GetEnvironmentVariable("PROJECT_NUMBER");
        var sample = new SearchWithFacetSpecSample();
        return sample.Search(projectNumber);
    }
}