// Copyright 2022 Google Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

// [START cloudrun_pubsub_server]
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var port = Environment.GetEnvironmentVariable("PORT");
if (port != null)
{
    app.Urls.Add($"http://0.0.0.0:{port}");
}
// [END cloudrun_pubsub_server]

// [START cloudrun_pubsub_handler]
app.MapPost("/", (Envelope envelope) =>
{
    if (envelope?.Message?.Data == null)
    {
        app.Logger.LogWarning("Bad Request: Invalid Pub/Sub message format.");
        return Results.BadRequest();
    }

    var data = Convert.FromBase64String(envelope.Message.Data);
    var target = System.Text.Encoding.UTF8.GetString(data);

    app.Logger.LogInformation($"Hello {target}!");

    return Results.NoContent();
});
// [END cloudrun_pubsub_handler]

app.Run();

record Message(string MessageId, string Data, DateTime PublishTime);

record Envelope(Message Message);

// Expose partial class for testing.
public partial class Program { }
