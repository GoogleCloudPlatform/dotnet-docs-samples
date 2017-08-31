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
using Google.Api.Gax;
using Google.Apis.CloudRuntimeConfig.v1beta1;
using Google.Apis.Services;

namespace TwelveFactor.Services.GoogleCloudPlatform {
    class Configurator : IConfigurationSource
    {
        // Configuration Sources are loaded before logging is configured,
        // because logging is controlled by configuration.  Therefore,
        // we have to queue all our log messages and then log them later.
        private DelayedLogger _logger = new DelayedLogger();
        public ILogger Logger
        {
            get { return _logger; }
            set { _logger.InnerLogger = value; }
        }        

        public ConfiguratorOptions Options { get; set; } 

        public Configurator(ConfiguratorOptions options = null) {
            Options = options ?? new ConfiguratorOptions();
        }        

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            var gaeDetails = Google.Api.Gax.Platform.Instance()?.GaeDetails;
            var options = Options ?? new ConfiguratorOptions();
            string projectId = options?.ProjectId ?? gaeDetails?.ProjectId;
            string configName = options?.ConfigName ?? gaeDetails?.VersionId;
            
            if (projectId == null) {
                _logger.LogWarning("No configuration variables will be "
                    + "added because ProjectId is null.");
                return new NoopConfigurationProvider();
            } else if (configName == null) {
                _logger.LogWarning("No configuration variables will be "
                    + "added because ConfigName is null.");
                return new NoopConfigurationProvider();
            }

            GoogleCredential credential =
                GoogleCredential.GetApplicationDefaultAsync().Result;
            // Inject the Cloud Storage scope if required.
            if (credential.IsCreateScopedRequired)
            {
                credential = credential.CreateScoped(new[]
                {
                    CloudRuntimeConfigService.Scope.Cloudruntimeconfig
                });
            }
            var runtimeConfig = new CloudRuntimeConfigService (new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "TwelveFactor",
            });
            string configParent = string.Format("projects/{0}/configs/{1}",
                projectId, configName); 
            _logger.LogInformation("Adding configuration variables from {0}{1}",
                    runtimeConfig.BaseUri, configParent);
            return new ConfiguratorProvider(runtimeConfig, configParent, _logger);
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
        readonly CloudRuntimeConfigService _runtimeConfig;
        readonly string _configParent;
        readonly ILogger _logger;
        public ConfiguratorProvider(CloudRuntimeConfigService runtimeConfig, 
            string configParent, ILogger logger)
        {
            _runtimeConfig = runtimeConfig;
            _configParent = configParent;
            _logger = logger;
        }

        public override void Load() {
            var request = new ProjectsResource.ConfigsResource
                .VariablesResource.ListRequest(_runtimeConfig, _configParent)
                {
                    ReturnValues = true
                };
            try {
                var result = request.Execute();
                foreach (var variable in result.Variables) {                    
                    _logger.LogDebug("{0}: {1}", variable.Name,
                        variable.Text);
                }
            } catch (Exception e) {
                _logger.LogError(0, e, "Failed to load config variables.");
            }
        }
    }
}
