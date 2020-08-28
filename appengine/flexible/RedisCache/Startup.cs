/*
 * Copyright (c) 2017 Google Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not
 * use this file except in compliance with the License. You may obtain a copy of
 * the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
 * License for the specific language governing permissions and limitations under
 * the License.
 */

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Redis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Net;

namespace RedisCache
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc();
            string redisEndpoint = Configuration["RedisEndpoint"];
            redisEndpoint = new string[] { null, "", "your-redis-endpoint" }
                .Contains(redisEndpoint) ?
                "" : WorkAroundIssue463(redisEndpoint);
            // [START redis_startup]
            services.AddDistributedRedisCache(options =>
            {
                options.Configuration = redisEndpoint;
                options.InstanceName = "master";
            });
            // [END redis_startup]
        }

        /// <summary>
        /// See https://github.com/StackExchange/StackExchange.Redis/issues/463
        /// When that issue is fixed, this function is no longer necessary.
        /// </summary>
        string WorkAroundIssue463(string redisConfig)
        {
            // Resolve all the dns names to IP addresses.
            string[] chunks = redisConfig.Split(',');
            var newChunks = chunks.Select(chunk =>
            {
                if (chunk.Contains('='))
                {
                    return chunk;
                }
                string[] hostport = chunk.Split(':');
                string port = hostport.Length > 1 ? ":" + hostport[1] : "";
                string host = hostport.Length > 1 ? hostport[0] : chunk;
                var addresses = Dns.GetHostAddressesAsync(host).Result;
                if (addresses.Count() == 0)
                {
                    return chunk;
                }
                return string.Join(",", addresses.Select(x => x.MapToIPv4().ToString() + port));
            });
            return string.Join(",", newChunks);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
