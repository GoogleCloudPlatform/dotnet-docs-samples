/*
 * Copyright (c) 2017 Google Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not
 * use this file except in compliance with the License. You may obtain a copy of
 * the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
 * License for the specific language governing permissions and limitations under
 * the License.
 */

using System.Net;

public class Program
{
    private WebApplication App {get; set;}

    private Program(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        App = builder.Build();
        App.MapGet("/", HandleGetAsync);
    }

    private async Task HandleGetAsync(HttpContext context, IConfiguration configuration)
    {
        // [START gae_flex_analytics_track_event]
        var trackingId = configuration["GA_TRACKING_ID"];

        var client = new HttpClient()
        {
            BaseAddress = new Uri("http://www.google-analytics.com/")
        };
        var content = new FormUrlEncodedContent(
            new Dictionary<string, string>() {
                { "v" , "1" },  // API Version.
                { "tid" , trackingId },  // Tracking ID / Property ID.
                // Anonymous Client Identifier. Ideally, this should be a UUID that
                // is associated with particular user, device, or browser instance.
                { "cid" , "555" },
                { "t" , "event" },  // Event hit type.
                { "ec" , "Poker" },  // Event category.
                { "ea" , "Royal Flush" },  // Event action.
                { "el" , "Hearts" },  // Event label.
                { "ev" , "0" },  // Event value, must be an integer
            });
        var postResponse = await client.PostAsync("collect", content);
        // [END gae_flex_analytics_track_event]
        await context.Response.WriteAsync(string.Format(
            "Analytics response code: {0}",
	    postResponse.StatusCode));
    }

    public static void Main(string[] args)
    {
        new Program(args).App.Run();
    }
}

