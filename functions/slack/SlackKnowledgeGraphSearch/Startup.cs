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

// [START functions_slack_setup]
using Google.Apis.Kgsearch.v1;
using Google.Apis.Services;
using Google.Cloud.Functions.Hosting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;

// Specify a startup class to use for dependency injection.
// This can also be specified on the function type.
[assembly: FunctionsStartup(typeof(SlackKnowledgeGraphSearch.Startup))]

namespace SlackKnowledgeGraphSearch;

public class Startup : FunctionsStartup
{
    private static readonly TimeSpan SlackTimestampTolerance = TimeSpan.FromMinutes(5);

    public override void ConfigureServices(WebHostBuilderContext context, IServiceCollection services)
    {
        // These can come from an environment variable or a configuration file.
        string kgApiKey = context.Configuration["KG_API_KEY"];
        string slackSigningSecret = context.Configuration["SLACK_SECRET"];

        services.AddSingleton(new KgsearchService(new BaseClientService.Initializer
        {
            ApiKey = kgApiKey
        }));
        services.AddSingleton(new SlackRequestVerifier(slackSigningSecret, SlackTimestampTolerance));
        base.ConfigureServices(context, services);
    }
}
// [END functions_slack_setup]
