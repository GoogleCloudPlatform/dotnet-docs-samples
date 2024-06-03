// Copyright 2020 Google LLC
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     https://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

// [START functions_cloudevent_firebase_reactive]
// [START functions_firebase_reactive]
using CloudNative.CloudEvents;
using Google.Cloud.Firestore;
using Google.Cloud.Functions.Framework;
using Google.Cloud.Functions.Hosting;
using Google.Events.Protobuf.Cloud.Firestore.V1;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FirestoreReactive;

public class Startup : FunctionsStartup
{
    public override void ConfigureServices(WebHostBuilderContext context, IServiceCollection services) =>
        services.AddSingleton(FirestoreDb.Create());
}

// Register the startup class to provide the Firestore dependency.
[FunctionsStartup(typeof(Startup))]
public class Function : ICloudEventFunction<DocumentEventData>
{
    private readonly ILogger _logger;
    private readonly FirestoreDb _firestoreDb;

    public Function(ILogger<Function> logger, FirestoreDb firestoreDb) =>
        (_logger, _firestoreDb) = (logger, firestoreDb);

    public async Task HandleAsync(CloudEvent cloudEvent, DocumentEventData data, CancellationToken cancellationToken)
    {
        // Get the recently-written value. This expression will result in a null value
        // if any of the following is true:
        // - The event doesn't contain a "new" document
        // - The value doesn't contain a field called "original"
        // - The "original" field isn't a string
        string currentValue = data.Value?.ConvertFields().GetValueOrDefault("original") as string;
        if (currentValue is null)
        {
            _logger.LogWarning($"Event did not contain a suitable document");
            return;
        }

        string newValue = currentValue.ToUpperInvariant();
        if (newValue == currentValue)
        {
            _logger.LogInformation("Value is already upper-cased; no replacement necessary");
            return;
        }

        // The CloudEvent subject is "documents/x/y/...".
        // The Firestore SDK FirestoreDb.Document method expects a reference relative to
        // "documents" (so just the "x/y/..." part). This may be simplified over time.
        if (cloudEvent.Subject is null || !cloudEvent.Subject.StartsWith("documents/"))
        {
            _logger.LogWarning("CloudEvent subject is not a document reference.");
            return;
        }
        string documentPath = cloudEvent.Subject.Substring("documents/".Length);

        _logger.LogInformation("Replacing '{current}' with '{new}' in '{path}'", currentValue, newValue, documentPath);
        await _firestoreDb.Document(documentPath).UpdateAsync("original", newValue, cancellationToken: cancellationToken);
    }
}
// [END functions_firebase_reactive]
// [END functions_cloudevent_firebase_reactive]
