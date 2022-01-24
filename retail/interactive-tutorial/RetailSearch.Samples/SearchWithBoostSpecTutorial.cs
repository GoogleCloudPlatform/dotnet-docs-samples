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

// [START retail_search_product_with_boost_spec]
// Call Retail API to search for a products in a catalog, rerank the
// results boosting or burying the products that match defined condition.

using Google.Cloud.Retail.V2;
using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Search with boot spec sample class.
/// </summary>
public class SearchWithBoostSpecSample
{
    /// <summary>Get search request.</summary>
    private SearchRequest GetSearchRequest(string query, string condition, float boostStrength, string projectNumber)
    {
        string defaultSearchPlacement = $"projects/{projectNumber}/locations/global/catalogs/default_catalog/placements/default_search";

        var conditionBoostSpec = new SearchRequest.Types.BoostSpec.Types.ConditionBoostSpec()
        {
            Condition = condition,
            Boost = boostStrength
        };

        var bootSpec = new SearchRequest.Types.BoostSpec();
        bootSpec.ConditionBoostSpecs.Add(conditionBoostSpec);

        var searchRequest = new SearchRequest()
        {
            Placement = defaultSearchPlacement, // Placement is used to identify the Serving Config name
            Query = query,
            BoostSpec = bootSpec,
            VisitorId = "123456", // A unique identifier to track visitors
            PageSize = 10
        };

        Console.WriteLine("\nSearch. request:\n");
        Console.WriteLine($"Placement: {searchRequest.Placement}");
        Console.WriteLine($"Query: {searchRequest.Query}");
        Console.WriteLine($"VisitorId: {searchRequest.VisitorId}");
        Console.WriteLine($"PageSize: {searchRequest.PageSize}");
        Console.WriteLine($"BoostSpec: {searchRequest.BoostSpec}");

        return searchRequest;
    }

    /// <summary>Call the Retail Search.</summary>
    [RetailSearch.Samples.Attributes.Example]
    public IEnumerable<SearchResponse> Search(string projectNumber)
    {
        // Try different conditions here:
        string condition = "colorFamilies: ANY(\"Blue\")";
        float boost = 0.0f;
        string query = "Tee";

        var client = SearchServiceClient.Create();
        var searchRequest = GetSearchRequest(query, condition, boost, projectNumber);
        var searchResponses = client.Search(searchRequest).AsRawResponses();

        var firstSearchResponse = searchResponses.FirstOrDefault();

        Console.WriteLine("\nSearch. response: \n");

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
// [END retail_search_product_with_boost_spec]

/// <summary>
/// Search with boost spec tutorial.
/// </summary>
public static class SearchWithBoostSpecTutorial
{
    [RetailSearch.Samples.Attributes.Example]
    public static IEnumerable<SearchResponse> Search()
    {
        var projectNumber = Environment.GetEnvironmentVariable("PROJECT_NUMBER");
        var sample = new SearchWithBoostSpecSample();
        return sample.Search(projectNumber);
    }
}