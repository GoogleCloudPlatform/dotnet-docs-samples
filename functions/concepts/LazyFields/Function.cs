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

// [START functions_tips_lazy_globals]
using Google.Cloud.Functions.Framework;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LazyFields;

public class Function : IHttpFunction
{
    // This computation runs at server cold-start.
    // Warning: Class variables used in functions code must be thread-safe.
    private static readonly int NonLazyGlobal = FileWideComputation();

    // This variable is initialized at server cold-start, but the
    // computation is only performed when the function needs the result.
    private static readonly Lazy<int> LazyGlobal = new Lazy<int>(
        FunctionSpecificComputation,
        LazyThreadSafetyMode.ExecutionAndPublication);

    public async Task HandleAsync(HttpContext context)
    {
        // In a more complex function, there might be some paths that use LazyGlobal.Value,
        // and others that don't. The computation is only performed when necessary, and
        // only once per server.
        await context.Response.WriteAsync(
            $"Lazy global: {LazyGlobal.Value}; non-lazy global: {NonLazyGlobal}",
            context.RequestAborted);
    }

    private static int FunctionSpecificComputation()
    {
        int[] numbers = { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        return numbers.Sum();
    }

    private static int FileWideComputation()
    {
        int[] numbers = { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        return numbers.Aggregate((current, next) => current * next);
    }
}
// [END functions_tips_lazy_globals]
