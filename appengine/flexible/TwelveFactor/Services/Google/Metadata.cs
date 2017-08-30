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
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace TwelveFactor.Services.Google {
    class Metadata {
        public HttpClient Http;
        public string ProjectId { get; set; }
        public string Version { get; set; }
        private Metadata(HttpClient httpClient, string projectId)
        {
            Http = httpClient;
            ProjectId = projectId;
        }

        public static async Task<Metadata> Create(ILogger logger) 
        {
            try
            {
                var http = new HttpClient()
                {
                    BaseAddress = new Uri(
                        "http://metadata.google.internal/computeMetadata/v1/")
                };
                http.DefaultRequestHeaders.Add("Metadata-Flavor", "Google");
                var response = await http.GetAsync("project/project-id");
                string projectId = await response.Content.ReadAsStringAsync();
                return new Metadata(http, projectId);
            }
            catch (HttpRequestException e) {
                if (logger != null) {
                    logger.LogWarning(0, e, "No Google metadata because not "
                    + "running on Google Cloud Platform.");
                }
                return null;
            }
        }
    }
}