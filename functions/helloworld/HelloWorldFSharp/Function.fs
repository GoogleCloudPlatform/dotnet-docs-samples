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

// [START functions_helloworld_get]
namespace HelloWorldFSharp

open Google.Cloud.Functions.Framework
open Microsoft.AspNetCore.Http

type Function() =
    interface IHttpFunction with
        member this.HandleAsync context =
            async {
                do! context.Response.WriteAsync "Hello World!" |> Async.AwaitTask
            } |> Async.StartAsTask :> _
// [END functions_helloworld_get]
