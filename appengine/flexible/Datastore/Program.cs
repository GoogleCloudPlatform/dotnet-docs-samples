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
using Microsoft.AspNetCore.Builder;
//using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Logging;
//using System;
//using System.Linq;
using System.Net;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

async Task handleGet(HttpContext context) {
  var datastore = DatastoreDb.Create(config["GoogleProjectId"]);
  var visitKeyFactory = datastore.CreateKeyFactory("visit");
  var newVisit = new Entity();
  newVisit.Key = visitKeyFactory.CreateIncompleteKey();
  newVisit["time_stamp"] = DateTime.UtcNow;
  newVisit["ip_address"] = FormatAddress(
    context.Connection.RemoteIpAddress);
  await datastore.InsertAsync(newVisit);

  // Look up the last 10 visits.
  var results = await datastore.RunQueryAsync(new Query("visit")
  {
    Order = { { "time_stamp", PropertyOrder.Types.Direction.Descending } },
      Limit = 10
    });
    await context.Response.WriteAsync(@"<html>
        <head><title>Visitor Log</title></head>
        <body>Last 10 visits:<br>");
    foreach (Entity visit in results.Entities)
    {
      await context.Response.WriteAsync(string.Format("{0} {1}<br>",
          visit["time_stamp"].TimestampValue,
          visit["ip_address"].StringValue));
    }
    await context.Response.WriteAsync(@"</body></html>");
}

string FormatAddress(IPAddress address)
{
  if (address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
  {
    var bytes = address.GetAddressBytes();
    return string.Format("{0:X2}{1:X2}:{2:X2}{3:X2}", bytes[0], bytes[1],
        bytes[2], bytes[3]);
  }
  else if (address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
  {
    var bytes = address.GetAddressBytes();
    return string.Format("{0}.{1}", bytes[0], bytes[1]);
  }
  else
  {
    return "bad.address";
  }
}

var app = builder.Build();
app.MapGet("/", handleGet);
app.Run();

