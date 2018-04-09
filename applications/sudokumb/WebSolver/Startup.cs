// Copyright (c) 2018 Google LLC.
//
// Licensed under the Apache License, Version 2.0 (the "License"); you may not
// use this file except in compliance with the License. You may obtain a copy of
// the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
// License for the specific language governing permissions and limitations under
// the License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Cloud.Datastore.V1;
using Google.Cloud.Diagnostics.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Sudokumb;

namespace WebSolver
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            // Add exception logging.
            services.AddGoogleExceptionLogging(options =>
            {
                options.ProjectId = Configuration["Google:ProjectId"];
                options.ServiceName = Configuration["Google:AppEngine:ServiceName"] ?? "WebSolver";
                options.Version = Configuration["Google:AppEngine:Version"] ?? "0.0";
            });
            // Add trace service.
            services.AddGoogleTrace(options =>
            {
                options.ProjectId = Configuration["Google:ProjectId"];
                options.Options = Google.Cloud.Diagnostics.Common.TraceOptions.Create(
                    bufferOptions: Google.Cloud.Diagnostics.Common.BufferOptions.NoBuffer());
            });

            services.AddOptions();
            services.Configure<PubsubGameBoardQueueOptions>(
                Configuration.GetSection("Google"));
            services.AddSingleton<DatastoreDb>(provider => DatastoreDb.Create(
                Configuration["Google:ProjectId"],
                Configuration["Google:NamespaceId"] ?? ""));
            services.AddSingleton<SolveStateStore>();
            services.AddDatastoreCounter();
            services.AddSingleton<Solver>();
            services.AddSingleton<InMemoryGameBoardStackImpl>();
            services.AddPubsubGameBoardQueueSolver();
            services.AddAdminSettings();
            services.AddMemoryCache();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env,
            ILoggerFactory loggerFactory)
        {
            // Configure logging service.
            loggerFactory.AddGoogle(Configuration["Google:ProjectId"]);

            // Configure error reporting service.
            app.UseGoogleExceptionLogging();

            // Configure trace service.
            app.UseGoogleTrace();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.Run(async (context) =>
            {
                long count = app.ApplicationServices
                    .GetService<SolveStateStore>().LocallyExaminedBoardCount;
                await context.Response.WriteAsync(string.Format(
                    "Examined {0} sudoku boards.", count));
            });
        }
    }
}
