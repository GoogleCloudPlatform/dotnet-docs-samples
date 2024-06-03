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

// [START functions_tips_scopes]
using Google.Cloud.Functions.Framework;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Threading.Tasks;

namespace Scopes;

public class Function : IHttpFunction
{
    // Global (server-wide) scope.
    // This computation runs at server cold-start.
    // Warning: Class variables used in functions code must be thread-safe.
    private static readonly int GlobalVariable = HeavyComputation();

    // Note that one instance of this class (Function) is created per invocation,
    // so calling HeavyComputation in the constructor would not have the same
    // benefit.

    public async Task HandleAsync(HttpContext context)
    {
        // Per-function-invocation scope.
        // This computation runs every time this function is called.
        int functionVariable = LightComputation();

        await context.Response.WriteAsync(
            $"Global: {GlobalVariable}; function: {functionVariable}",
            context.RequestAborted);
    }

    private static int LightComputation()
    {
        int[] numbers = { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        return numbers.Sum();
    }

    private static int HeavyComputation()
    {
        int[] numbers = { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        return numbers.Aggregate((current, next) => current * next);
    }
}
// [END functions_tips_scopes]
