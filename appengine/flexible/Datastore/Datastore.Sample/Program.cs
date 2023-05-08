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

using Google.Cloud.Datastore.V1;
using System.Net;

public class Program
{
    private WebApplication App {get; set;}

    private Program(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var datastore = DatastoreDb.Create(builder.Configuration["GOOGLE_PROJECT_ID"]);
        builder.Services.AddSingleton(datastore);
        builder.Services.AddSingleton(datastore.CreateKeyFactory("visit"));
        App = builder.Build();
        App.MapGet("/", HandleGetAsync);
    }

    private async Task HandleGetAsync(HttpContext context, DatastoreDb datastore, KeyFactory visitKeyFactory)
    {
        var newVisit = new Entity();
        newVisit.Key = visitKeyFactory.CreateIncompleteKey();
        newVisit["time_stamp"] = DateTime.UtcNow;
        newVisit["ip_address"] = context.Connection.RemoteIpAddress?.ToString() ?? "bad_ip";
        try
    {
            await datastore.InsertAsync(newVisit);
        }
        catch
    {
            // Datastore not setup properly.
            await context.Response.WriteAsync("Datastore connection failed, ensure project id is set.\n");
            return;
        }

        // Look up the last 10 visits.
        var results = await datastore.RunQueryAsync(new Query("visit")
        {
            Order = { { "time_stamp", PropertyOrder.Types.Direction.Descending } },
            Limit = 10
        });
        await context.Response.WriteAsync(@"<html>
            <head><title>Visitor Log</title></head>
            <body>Last 10 visits:<br>");
        foreach (var visit in results.Entities)
        {
            await context.Response.WriteAsync(string.Format("{0} {1}<br>",
                visit["time_stamp"].TimestampValue,
                visit["ip_address"].StringValue));
        }
        await context.Response.WriteAsync(@"</body></html>");
    }

    public static void Main(string[] args)
    {
        new Program(args).App.Run();
    }
}

