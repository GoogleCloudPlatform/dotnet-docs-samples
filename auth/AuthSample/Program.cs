/*
 * Copyright (c) 2015 Google Inc.
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
// [START all]

using CommandLine;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Storage.v1;
using Google.Cloud.Language.V1;
using Google.Cloud.Storage.V1;
using Grpc.Auth;
using System;
using System.Net.Http;

namespace GoogleCloudSamples
{
    class BaseOptions
    {
        string _projectId;
        [Option('p', Default = null, HelpText = "Your Google project id.")]
        public string ProjectId
        {
            get
            {
                if (null == _projectId)
                {
                    _projectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
                    if (null == _projectId)
                    {
                        _projectId = Google.Api.Gax.Platform.Instance()?.GceDetails?.ProjectId;
                        if (null == _projectId)
                        {
                            throw new ArgumentNullException("ProjectId");
                        }
                    }
                }
                return _projectId;
            }
            set
            {
                _projectId = value;
            }
        }

        [Option('j', Default = null, HelpText = "Path to a credentials json file.")]
        public string JsonPath { get; set; }

        [Option('c', Default = false, HelpText = "Pull credentials from compute engine metadata.")]
        public bool Compute { get; set; }
    }

    [Verb("hand", HelpText = "Authenticate using the Google.Cloud.Storage library.  "
        + "The preferred way of authenticating hand-coded wrapper libraries.")]
    class HandOptions : BaseOptions { }

    [Verb("cloud", HelpText = "Authenticate using the Google.Cloud.Language library.  "
        + "The preferred way of authenticating gRPC-based libraries.")]
    class CloudOptions : BaseOptions { }

    [Verb("api", HelpText = "Authenticate using the Google.Apis.Storage library.")]
    class ApiOptions : BaseOptions { }

    [Verb("http", HelpText = "Authenticate using and make a rest HTTP call.")]
    class HttpOptions : BaseOptions { }

    /// <summary>
    /// Each library supports 3 methods of authenticating.
    /// </summary>
    interface AuthLibrary
    {
        object AuthImplicit(string projectId);
        object AuthExplicit(string projectId, string jsonPath);
        object AuthExplicitComputeEngine(string projectId);
    }

    /// <summary>
    /// Some APIs like storage have hand-coded libraries.  They auth like this.
    /// </summary>
    public class HandCodedLibrary : AuthLibrary
    {
        ///////////////////////////////////////////////
        // This is the preferred way of authenticating.
        ///////////////////////////////////////////////
        // [START auth_cloud_implicit]
        public object AuthImplicit(string projectId)
        {
            // If you don't specify credentials when constructing the client, the
            // client library will look for credentials in the environment.
            var credential = GoogleCredential.GetApplicationDefault();
            var storage = StorageClient.Create(credential);
            // Make an authenticated API request.
            var buckets = storage.ListBuckets(projectId);
            foreach (var bucket in buckets)
            {
                Console.WriteLine(bucket.Name);
            }
            return null;
        }
        // [END auth_cloud_implicit]

        // [START auth_cloud_explicit]
        // Some APIs, like Storage, accept a credential in their Create()
        // method.
        public object AuthExplicit(string projectId, string jsonPath)
        {
            // Explicitly use service account credentials by specifying 
            // the private key file.
            var credential = GoogleCredential.FromFile(jsonPath);
            var storage = StorageClient.Create(credential);
            // Make an authenticated API request.
            var buckets = storage.ListBuckets(projectId);
            foreach (var bucket in buckets)
            {
                Console.WriteLine(bucket.Name);
            }
            return null;
        }
        // [END auth_cloud_explicit]

        // [START auth_cloud_explicit_compute_engine]
        // Some APIs, like Storage, accept a credential in their Create()
        // method.
        public object AuthExplicitComputeEngine(string projectId)
        {
            // Explicitly request service account credentials from the compute
            // engine instance.
            GoogleCredential credential =
                GoogleCredential.FromComputeCredential();
            var storage = StorageClient.Create(credential);
            // Make an authenticated API request.
            var buckets = storage.ListBuckets(projectId);
            foreach (var bucket in buckets)
            {
                Console.WriteLine(bucket.Name);
            }
            return null;
        }
        // [END auth_cloud_explicit_compute_engine]
    }

    /// <summary>
    /// Authenticates with Google.Apis.* libraries.
    /// </summary>
    class ApiLibrary : AuthLibrary
    {
        // [START auth_api_implicit]
        public object AuthImplicit(string projectId)
        {
            GoogleCredential credential =
                GoogleCredential.GetApplicationDefault();
            // Inject the Cloud Storage scope if required.
            if (credential.IsCreateScopedRequired)
            {
                credential = credential.CreateScoped(new[]
                {
                    StorageService.Scope.DevstorageReadOnly
                });
            }
            var storage = new StorageService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "DotNet Google Cloud Platform Auth Sample",
            });
            var request = new BucketsResource.ListRequest(storage, projectId);
            var requestResult = request.Execute();
            foreach (var bucket in requestResult.Items)
            {
                Console.WriteLine(bucket.Name);
            }
            return null;
        }
        // [END auth_api_implicit]

        // [START auth_api_explicit]
        public object AuthExplicit(string projectId, string jsonPath)
        {
            var credential = GoogleCredential.FromFile(jsonPath);
            // Inject the Cloud Storage scope if required.
            if (credential.IsCreateScopedRequired)
            {
                credential = credential.CreateScoped(new[]
                {
                    StorageService.Scope.DevstorageReadOnly
                });
            }
            var storage = new StorageService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "DotNet Google Cloud Platform Auth Sample",
            });
            var request = new BucketsResource.ListRequest(storage, projectId);
            var requestResult = request.Execute();
            foreach (var bucket in requestResult.Items)
            {
                Console.WriteLine(bucket.Name);
            }
            return null;
        }
        // [END auth_api_explicit]

        // [START auth_api_explicit_compute_engine]
        public object AuthExplicitComputeEngine(string projectId)
        {
            // Explicitly use service account credentials by specifying the 
            // private key file.
            GoogleCredential credential =
                GoogleCredential.FromComputeCredential();
            // Inject the Cloud Storage scope if required.
            if (credential.IsCreateScopedRequired)
            {
                credential = credential.CreateScoped(new[]
                {
                    StorageService.Scope.DevstorageReadOnly
                });
            }
            var storage = new StorageService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "DotNet Google Cloud Platform Auth Sample",
            });
            var request = new BucketsResource.ListRequest(storage, projectId);
            var requestResult = request.Execute();
            foreach (var bucket in requestResult.Items)
            {
                Console.WriteLine(bucket.Name);
            }
            return null;
        }
        // [END auth_api_explicit_compute_engine]
    }

    class HttpLibrary : AuthLibrary
    {
        // [START auth_http_implicit]
        public object AuthImplicit(string projectId)
        {
            GoogleCredential credential =
                GoogleCredential.GetApplicationDefault();
            // Inject the Cloud Storage scope if required.
            if (credential.IsCreateScopedRequired)
            {
                credential = credential.CreateScoped(new[]
                {
                    "https://www.googleapis.com/auth/devstorage.read_only"
                });
            }
            HttpClient http = new Google.Apis.Http.HttpClientFactory()
                .CreateHttpClient(
                new Google.Apis.Http.CreateHttpClientArgs()
                {
                    ApplicationName = "Google Cloud Platform Auth Sample",
                    GZipEnabled = true,
                    Initializers = { credential },
                });
            UriBuilder uri = new UriBuilder(
                "https://www.googleapis.com/storage/v1/b");
            uri.Query = "project=" +
                System.Web.HttpUtility.UrlEncode(projectId);
            var resultText = http.GetAsync(uri.Uri).Result.Content
                .ReadAsStringAsync().Result;
            dynamic result = Newtonsoft.Json.JsonConvert
                .DeserializeObject(resultText);
            foreach (var bucket in result.items)
            {
                Console.WriteLine(bucket.name);
            }
            return null;
        }
        // [END auth_http_implicit]

        // [START auth_http_explicit]
        public object AuthExplicit(string projectId, string jsonPath)
        {
            var credential = GoogleCredential.FromFile(jsonPath);
            // Inject the Cloud Storage scope if required.
            if (credential.IsCreateScopedRequired)
            {
                credential = credential.CreateScoped(new[]
                {
                    "https://www.googleapis.com/auth/devstorage.read_only"
                });
            }
            HttpClient http = new Google.Apis.Http.HttpClientFactory()
                .CreateHttpClient(
                new Google.Apis.Http.CreateHttpClientArgs()
                {
                    ApplicationName = "Google Cloud Platform Auth Sample",
                    GZipEnabled = true,
                    Initializers = { credential },
                });
            UriBuilder uri = new UriBuilder(
                "https://www.googleapis.com/storage/v1/b");
            uri.Query = "project=" +
                System.Web.HttpUtility.UrlEncode(projectId);
            var resultText = http.GetAsync(uri.Uri).Result.Content
                .ReadAsStringAsync().Result;
            dynamic result = Newtonsoft.Json.JsonConvert
                .DeserializeObject(resultText);
            foreach (var bucket in result.items)
            {
                Console.WriteLine(bucket.name);
            }
            return null;
        }

        public object AuthExplicitComputeEngine(string projectId)
        {
            throw new NotImplementedException();
        }
        // [END auth_http_explicit]
    }

    /// <summary>
    /// Authenticates with Google.Cloud.* libraries.
    /// Specifically calls the language API, but all the machine learning APIs,
    /// And some other APIs like Pub/Sub also follow this pattern.
    /// </summary>
    public class CloudLibrary : AuthLibrary
    {
        // [START auth_cloud_explicit]
        // Other APIs, like Language, accept a channel in their Create()
        // method.
        public object AuthExplicit(string projectId, string jsonPath)
        {
            LanguageServiceClientBuilder builder = new LanguageServiceClientBuilder
            {
                CredentialsPath = jsonPath
            };

            LanguageServiceClient client = builder.Build();
            AnalyzeSentiment(client);
            return 0;
        }
        // [END auth_cloud_explicit]

        // [START auth_cloud_explicit_compute_engine]
        // Other APIs, like Language, accept a channel in their Create()
        // method.
        public object AuthExplicitComputeEngine(string projectId)
        {
            LanguageServiceClientBuilder builder = new LanguageServiceClientBuilder
            {
                ChannelCredentials = GoogleCredential
                .FromComputeCredential()
                .ToChannelCredentials()
            };

            LanguageServiceClient client = builder.Build();
            AnalyzeSentiment(client);
            return 0;
        }
        // [END auth_cloud_explicit_compute_engine]

        public object AuthImplicit(string projectId)
        {
            var client = LanguageServiceClient.Create();
            AnalyzeSentiment(client);
            return 0;
        }

        void AnalyzeSentiment(LanguageServiceClient client)
        {
            string text = "Hello World!";
            var response = client.AnalyzeSentiment(new Document()
            {
                Content = text,
                Type = Document.Types.Type.PlainText
            });
            var sentiment = response.DocumentSentiment;
            Console.WriteLine($"Score: {sentiment.Score}");
            Console.WriteLine($"Magnitude: {sentiment.Magnitude}");
        }
    }

    public class AuthSample
    {
        static object ChooseAuthMethodAndInvoke(BaseOptions options, AuthLibrary library)
        {
            if (!string.IsNullOrEmpty(options.JsonPath))
            {
                return library.AuthExplicit(options.ProjectId, options.JsonPath);
            }
            if (options.Compute)
            {
                return library.AuthExplicitComputeEngine(options.ProjectId);
            }
            return library.AuthImplicit(options.ProjectId);
        }
        public static void Main(string[] args)
        {
            Parser.Default.ParseArguments<CloudOptions, ApiOptions, HttpOptions, HandOptions>(args)
              .MapResult(
                (HandOptions opts) => ChooseAuthMethodAndInvoke(opts, new HandCodedLibrary()),
                (ApiOptions opts) => ChooseAuthMethodAndInvoke(opts, new ApiLibrary()),
                (HttpOptions opts) => ChooseAuthMethodAndInvoke(opts, new HttpLibrary()),
                (CloudOptions opts) => ChooseAuthMethodAndInvoke(opts, new CloudLibrary()),
                errs => 1);
        }
    }
}
// [END all]
