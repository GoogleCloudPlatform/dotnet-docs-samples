/*
 * Copyright 2018 Google LLC
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.IO;
using Google.Apis.JobService.v2;
using Google.Apis.JobService.v2.Data;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;

namespace Google.Samples
{
    /// <summary>
    /// This Util helps to generate credentials and create the Google JobService.
    /// </summary>
    public static class JobServiceUtils
    {
        private const string SCOPES = "https://www.googleapis.com/autho/jobs";

        private static readonly JobServiceService jobService = CreateJobService(GenerateCredential());

        public static JobServiceService JobService
        {
            get
            {
                return jobService;
            }
        }

        private static JobServiceService CreateJobService(GoogleCredential credential)
        {
            JobServiceService.Initializer initializer = new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                GZipEnabled = true,
            };
            return new JobServiceService(initializer);
        }

        private static GoogleCredential GenerateCredential()
        {
            // Authorize the client using Application Default Credentials.
            // See: https://developers.google.com/identity/protocols/application-default-credentials
            GoogleCredential credential = GoogleCredential.GetApplicationDefaultAsync().Result;
            // Specify the Service scope.
            if (credential.IsCreateScopedRequired)
            {
                credential = credential.CreateScoped(new[]
                {
                   Google.Apis.JobService.v2.JobServiceService.Scope.Jobs
                });
            }
            return credential;
        }
    }
}

// TODO(acasale): Need this?
//   private static HttpRequestInitializer setHttpTimeout(
//       final HttpRequestInitializer requestInitializer) {
//     return request -> {
//       requestInitializer.initialize(request);
//       request.setHeaders(new HttpHeaders().set("X-GFE-SSL", "yes"));
//       request.setConnectTimeout(1 * 60000); // 1 minute connect timeout
//       request.setReadTimeout(1 * 60000); // 1 minute read timeout
//     };
//   }
