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

public class Program
{
    private WebApplication App {get; set;}

    private Program(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        App = builder.Build();
        App.MapGet("/", HandleGetAsync);
    }

    private async Task HandleGetAsync(HttpContext context)
    {
        string response;
        try {
            // [START gae_flex_metadata]
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Metadata-Flavor", new[] { "Google" });
            response = await client.GetStringAsync(
                "http://metadata.google.internal/computeMetadata/v1/instance/network-interfaces/0/access-configs/0/external-ip");
            // [END gae_flex_metadata]
        }
        catch (System.Net.Http.HttpRequestException e)
        {
            response = "<not set>";
        }
        await context.Response.WriteAsync(string.Format(
            "Public IP address: {0}",
	    response));
    }

    public static void Main(string[] args)
    {
        new Program(args).App.Run();
    }
}

