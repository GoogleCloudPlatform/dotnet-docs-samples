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
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Google.Apis.Auth.OAuth2;

namespace TwelveFactor.Services.GoogleCloudPlatform {
    class Configurator : IConfigurationSource
    {
        readonly HttpClient _http;
        readonly ILoggerFactory _loggerFactory;
        public Configurator(ConfiguratorOptions options, 
            ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
            var gaeDetails = Google.Api.Gax.Platform.Instance()?.GaeDetails;
            string projectId = options?.ProjectId ?? gaeDetails?.ProjectId;
            string configName = options?.ConfigName ?? gaeDetails?.VersionId;
            var logger = loggerFactory.CreateLogger<Configurator>();
            if (projectId == null) {
                logger.LogWarning("No configuration variables will be added "
                    + "because ProjectId is null.");
                return;
            } else if (configName == null) {
                logger.LogWarning("No configuration variables will be added "
                    + "because ConfigName is null.");
                return;
            }

            logger.LogInformation("Adding configuration variables from "
                + "projects/{0}/configs/{1}/", projectId, configName);

            GoogleCredential credential =
                GoogleCredential.GetApplicationDefaultAsync().Result;
            // Inject the Cloud Storage scope if required.
            if (credential.IsCreateScopedRequired)
            {
                credential = credential.CreateScoped(new[]
                {
                    "https://www.googleapis.com/auth/runtimeconfig.variables.get",
                    "https://www.googleapis.com/auth/runtimeconfig.variables.list",
                    "https://www.googleapis.com/auth/runtimeconfig.variables.watch"
                });
            }
            _http = new Google.Apis.Http.HttpClientFactory()
                .CreateHttpClient(
                new Google.Apis.Http.CreateHttpClientArgs()
                {
                    ApplicationName = "TwelveFactor",
                    GZipEnabled = true,
                    Initializers = { credential },
                });
            _http.BaseAddress = new Uri(string.Format(
                "https://runtimeconfig.googleapis.com/projects/{0}/configs/{1}/",
                projectId, configName));
        }

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            if (_http == null) {
                return new NoopConfigurationProvider();
            } else {
                return new ConfiguratorProvider(_http, _loggerFactory);
            }
        }
    }

    public class ConfiguratorOptions
    {
        /// <summary>
        /// When copying fields to configuration variables, replace
        /// -s with :s in the variable name.  Useful because configurator field
        /// names cannot contain :s.
        /// </summary>
        public bool ReplaceHyphensWithColons { get; set; } = true;

        // Not necessary to set when running on app engine.
        public string ProjectId { get; set; }

        // Not necessary to set when running on app engine.
        public string ConfigName { get; set; }
    }

    public class NoopConfigurationProvider : ConfigurationProvider {}

    public class ConfiguratorProvider : ConfigurationProvider {
        readonly HttpClient _http;
        readonly ILogger _logger;        
        public ConfiguratorProvider(HttpClient http, 
            ILoggerFactory loggerFactory)
        {
            _http = http;
            _logger = loggerFactory.CreateLogger<ConfiguratorProvider>();
        }

        public override void Load() {
            _logger.LogTrace(
                _http.GetStringAsync("variables?returnValues=True").Result);
        }

    }
}
