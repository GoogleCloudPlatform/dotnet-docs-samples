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

namespace SocialAuth
{
    /// <summary>
    /// A ConfigurationSource that populates copies Google Compute Engine
    /// metadata into the configuration.
    /// </summary>
    public class MetadataConfigurationSource : IConfigurationSource
    {
        readonly MetadataConfigurationOptions _options;
        public MetadataConfigurationSource(
            MetadataConfigurationOptions options = null)
        {
            _options = options ?? new MetadataConfigurationOptions();
        }
        IConfigurationProvider IConfigurationSource.Build(
            IConfigurationBuilder builder)
        {
            return new MetadataConfigurationProvider(_options);
        }
    }

    public class MetadataConfigurationOptions
    {
        /// <summary>
        /// When copying metadata fields to configuration variables, replace
        /// -s with :s in the variable name.  Useful because metadata field
        /// names cannot contain :s.
        /// </summary>
        public bool ReplaceHyphensWithColons { get; set; } = true;
    }

    /// <summary>
    /// A ConfigurationProvider that populates copies Google Compute Engine
    /// metadata into the configuration.
    /// 
    /// TODO: Implement OnReload() and GetReloadToken().
    /// </summary>
    public class MetadataConfigurationProvider : ConfigurationProvider
    {
        /// <summary>
        /// The HttpClient used to fetch metadata.
        /// </summary>
        readonly HttpClient _http;
        readonly MetadataConfigurationOptions _options;

        public MetadataConfigurationProvider(
            MetadataConfigurationOptions options)
        {
            _options = options;
            // Initialize the HttpClient we use to fetch metadata.
            _http = new HttpClient()
            {
                BaseAddress = new Uri(
                    "http://metadata.google.internal/computeMetadata/v1/")
            };
            _http.DefaultRequestHeaders.Add("Metadata-Flavor", "Google");
        }

        public override void Load()
        {
            try
            {
                // Fetch the metadata and parse the json response.
                Dictionary<string, string> attributes =
                    JsonConvert.DeserializeObject<Dictionary<string, string>>(
                    _http.GetAsync("project/attributes/?recursive=true")
                    .Result.Content.ReadAsStringAsync().Result);
                // Instance attributes clobber project attributes.
                Dictionary<string, string> instanceAttributes =
                    JsonConvert.DeserializeObject<Dictionary<string, string>>(
                    _http.GetAsync("instance/attributes/?recursive=true")
                    .Result.Content.ReadAsStringAsync().Result);
                foreach (var instanceAttribute in instanceAttributes)
                {
                    attributes[instanceAttribute.Key] = 
                        instanceAttribute.Value;
                }
                // Replace hyphens with colons.
                if (_options.ReplaceHyphensWithColons)
                {
                    var newData = new Dictionary<string, string>();
                    foreach (var attribute in attributes)
                    {
                        newData[attribute.Key.Replace('-', ':')] = 
                            attribute.Value;
                    }
                    Data = newData;
                }
                else
                {
                    Data = attributes;
                }
                Data["IAmRunningInGoogleCloud"] = "true";
            }
            catch (AggregateException ae)
            {
                ae.Handle((e) =>
                {
                    if (e is HttpRequestException)
                    {
                        Debug.WriteLine(
                            "Failed to load attributes from Google metadata. "
                            + "I assume I'm not running in Google Cloud.");
                        return true;
                    }
                    return false;
                });                 
            }
        }
    }
}
