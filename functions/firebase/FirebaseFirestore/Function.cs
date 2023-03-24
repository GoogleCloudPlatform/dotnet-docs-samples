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

// [START functions_cloudevent_firebase_firestore]
// [START functions_firebase_firestore]
using CloudNative.CloudEvents;
using Google.Cloud.Functions.Framework;
using Google.Events.Protobuf.Cloud.Firestore.V1;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FirebaseFirestore;

public class Function : ICloudEventFunction<DocumentEventData>
{
    private readonly ILogger _logger;

    public Function(ILogger<Function> logger) =>
        _logger = logger;

    public Task HandleAsync(CloudEvent cloudEvent, DocumentEventData data, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Function triggered by event on {subject}", cloudEvent.Subject);
        _logger.LogInformation("Event type: {type}", cloudEvent.Type);
        MaybeLogDocument("Old value", data.OldValue);
        MaybeLogDocument("New value", data.Value);

        // In this example, we don't need to perform any asynchronous operations, so the
        // method doesn't need to be declared async.
        return Task.CompletedTask;
    }

    /// <summary>
    /// Logs the names and values of the fields in a document in a very simplistic way.
    /// </summary>
    private void MaybeLogDocument(string message, Document document)
    {
        if (document is null)
        {
            return;
        }

        // ConvertFields converts the Firestore representation into a .NET-friendly
        // representation.
        IReadOnlyDictionary<string, object> fields = document.ConvertFields();
        var fieldNamesAndTypes = fields
            .OrderBy(pair => pair.Key)
            .Select(pair => $"{pair.Key}: {pair.Value}");
        _logger.LogInformation(message + ": {fields}", string.Join(", ", fieldNamesAndTypes));
    }
}
// [END functions_firebase_firestore]
// [END functions_cloudevent_firebase_firestore]
