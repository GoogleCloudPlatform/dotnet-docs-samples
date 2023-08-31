// Copyright 2023 Google LLC
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

// [START functions_typed_greeting]
using Google.Cloud.Functions.Framework;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Greeting;

public class Function : ITypedFunction<GreetingRequest, GreetingResponse>
{
    public Task<GreetingResponse> HandleAsync(GreetingRequest request, CancellationToken cancellationToken) =>
        Task.FromResult(new GreetingResponse
        {
            Message = $"Hello {request.FirstName} {request.LastName}!",
        });
}

public class GreetingRequest
{
    [JsonPropertyName("first_name")]
    public string FirstName { get; set; }

    [JsonPropertyName("last_name")]
    public string LastName { get; set; }
}

public class GreetingResponse
{
    [JsonPropertyName("message")]
    public string Message { get; set; }
}

// [END functions_typed_greeting]
