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

// [START functions_firebase_auth]
using CloudNative.CloudEvents;
using Google.Cloud.Functions.Framework;
using Google.Events.Protobuf.Firebase.Auth.V1;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace FirebaseAuth;

public class Function : ICloudEventFunction<AuthEventData>
{
    private readonly ILogger _logger;

    public Function(ILogger<Function> logger) =>
        _logger = logger;

    public Task HandleAsync(CloudEvent cloudEvent, AuthEventData data, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Function triggered by change to user: {uid}", data.Uid);
        if (data.Metadata is UserMetadata metadata)
        {
            _logger.LogInformation("User created at: {created:s}", metadata.CreateTime.ToDateTimeOffset());
        }
        if (!string.IsNullOrEmpty(data.Email))
        {
            _logger.LogInformation("Email: {email}", data.Email);
        }

        // In this example, we don't need to perform any asynchronous operations, so the
        // method doesn't need to be declared async.
        return Task.CompletedTask;
    }
}
// [END functions_firebase_auth]
