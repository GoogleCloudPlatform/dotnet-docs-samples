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

        // Put the intervals here:
        var searchRequest = new SearchRequest()
        {
            Placement = defaultSearchPlacement, // Placement is used to identify the Serving Config name
            Query = query,
            VisitorId = "123456", // A unique identifier to track visitors
            PageSize = 10,
            FacetSpecs =
            {
                new SearchRequest.Types.FacetSpec
                {
                    FacetKey = new SearchRequest.Types.FacetSpec.Types.FacetKey
                    {
                         Key = facetKeyParam
                    }
                }
            }
        };

        Console.WriteLine("\nSearch. request:\n");
        Console.WriteLine($"Placement: {searchRequest.Placement}");
        Console.WriteLine($"Query: {searchRequest.Query}");
        Console.WriteLine($"VisitorId: {searchRequest.VisitorId}");
        Console.WriteLine($"PageSize: {searchRequest.PageSize}");
        Console.WriteLine($"FacetSpecs: {searchRequest.FacetSpecs}");

        return searchRequest;
    }

    /// <summary>Call the Retail Search.</summary>
    [RetailSearch.Samples.Attributes.Example]
    public IEnumerable<SearchResponse> Search(string projectNumber)
    {
        // Try different facets here:
        string facetKey = "colorFamilies";
        string query = "Tee";

        var client = SearchServiceClient.Create();
        var searchRequest = GetSearchRequest(query, facetKey, projectNumber);
        var searchResponses = client.Search(searchRequest).AsRawResponses();

        var firstSearchResponse = searchResponses.FirstOrDefault();

        Console.WriteLine("\nSearch. response: \n");

        if (firstSearchResponse != null)
        {
            Console.WriteLine($"Results: {firstSearchResponse.Results}");
            Console.WriteLine($"TotalSize: {firstSearchResponse.TotalSize},");
            Console.WriteLine($"AttributionToken: {firstSearchResponse.AttributionToken},");
            Console.WriteLine($"NextPageToken: {firstSearchResponse.NextPageToken},");
            Console.WriteLine($"Facets: {firstSearchResponse.Facets},");
        }

        return searchResponses;
    }
}
// [END retail_search_product_with_facet_spec]

/// <summary>
/// Search with facet spec tutorial.
/// </summary>
public static class SearchWithFacetSpecTutorial
{
    [RetailSearch.Samples.Attributes.Example]
    public static IEnumerable<SearchResponse> Search()
    {
        var projectNumber = Environment.GetEnvironmentVariable("PROJECT_NUMBER");
        var sample = new SearchWithFacetSpecSample();
        return sample.Search(projectNumber);
    }
}