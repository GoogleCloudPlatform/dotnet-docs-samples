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

using Google.Cloud.PubSub.V1;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Linq;
using System.Net;

namespace Pubsub
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
            services.AddOptions();
            services.Configure<PubsubOptions>(
                Configuration.GetSection("Pubsub"));
            services.AddMvc();
            // Add Pubsub publisher.
            services.AddSingleton((provider) =>
            {
                var options = provider.GetService<IOptions<PubsubOptions>>()
                    .Value;
                var logger = provider.GetService<ILogger<Startup>>();
                CreateTopicAndSubscription(options, logger);
                return PublisherClient.CreateAsync(
                    new TopicName(options.ProjectId, options.TopicId)).Result;
            });
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

        /// <summary>
        /// Create a topic and subscription if they don't already exist.
        /// </summary>
        static void CreateTopicAndSubscription(PubsubOptions options,
            ILogger logger)
        {
            var topicName = new TopicName(options.ProjectId, options.TopicId);
            var publisher = PublisherServiceApiClient.Create();
            try
            {
                publisher.CreateTopic(topicName);
            }
            catch (Grpc.Core.RpcException e)
            when (e.Status.StatusCode == Grpc.Core.StatusCode.AlreadyExists ||
                e.Status.StatusCode == Grpc.Core.StatusCode.PermissionDenied)
            {
                logger.LogWarning(0, e, "Could not create topic {0}", topicName);
            }
            // Create push subscription for /Push endpoint.
            var subscriptionName = new SubscriptionName(
                    options.ProjectId, options.SubscriptionId);
            var pushConfig = new PushConfig()
            {
                PushEndpoint = $"https://{options.ProjectId}.appspot.com/Push"
            };
            CreateOrModifySubscription(subscriptionName, topicName, pushConfig, logger);
            
            // Create push subscription for /AuthPush endpoint.
            var authSubscriptionName = new SubscriptionName(
                options.ProjectId, options.AuthSubscriptionId);
            // To enable authentication in subscription.
            var oidcToken = new PushConfig.Types.OidcToken();
            oidcToken.ServiceAccountEmail = options.ServiceAccountEmail;
            var authPushConfig = new PushConfig
            {
                PushEndpoint = $"https://{options.ProjectId}.appspot.com/AuthPush",
                OidcToken = oidcToken
            };
            CreateOrModifySubscription(authSubscriptionName, topicName, authPushConfig, logger);
        }
        /// <summary>
        /// Create a subscription if it not exits otherwise update the existing one.
        /// </summary>
        static void CreateOrModifySubscription(SubscriptionName subscriptionName,TopicName topicName,PushConfig pushConfig,ILogger logger)
        {
            var subscriber = SubscriberServiceApiClient.Create();
            try
            {
                subscriber.CreateSubscription(subscriptionName, topicName,
                    pushConfig, 20);
                return;
            }
            catch (Grpc.Core.RpcException e)
            when (e.Status.StatusCode == Grpc.Core.StatusCode.AlreadyExists ||
                e.Status.StatusCode == Grpc.Core.StatusCode.PermissionDenied)
            {
                logger.LogWarning(1, e, "Could not create subscription {0}",
                    subscriptionName);
            }
            catch (Grpc.Core.RpcException e)
            {
                logger.LogWarning(2, e, "Could not create subscription {0}",
                    subscriptionName);
                return;
            }
            try
            {
                subscriber.ModifyPushConfig(subscriptionName, pushConfig);
            }
            catch (Grpc.Core.RpcException e)
            when (e.Status.StatusCode == Grpc.Core.StatusCode.PermissionDenied)
            {
                logger.LogWarning(3, e, "Could not modify subscription {0}",
                    subscriptionName);
            }
            return;
        }

    }
}
