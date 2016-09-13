/**
 * Copyright 2016 Google Inc. All Rights Reserved.
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

/**
* Sample code to issue a report to Stackdriver Error Reporting
* using the Google Client Libraries.
* For more information, see documentation for Stackdriver Error Reporting
* https://cloud.google.com/error-reporting/reference/rest/
*/

using Google.Apis.Auth.OAuth2;
using Google.Apis.Clouderrorreporting.v1beta1;
using Google.Apis.Clouderrorreporting.v1beta1.Data;
using Google.Apis.Services;
using System;
using System.Threading.Tasks;
using static Google.Apis.Clouderrorreporting.v1beta1.ProjectsResource.EventsResource;

namespace GoogleCloudSamples
{
    // [START example]
    public class ErrorReportingSample
    {
        public static void Main(string[] args)
        {
            try
            {
                throw new Exception("Generic exception for testing Stackdriver Error Reporting");
            }
            catch (Exception e)
            {
                report(e);
                Console.WriteLine("Stackdriver Error Report Sent");
            }
        }

        private static ClouderrorreportingService CreateErrorReportingClient()
        {
            GoogleCredential credential = Task.Run(() => GoogleCredential.GetApplicationDefaultAsync()).Result;
            credential.CreateScoped(ClouderrorreportingService.Scope.CloudPlatform);
            ClouderrorreportingService service = new ClouderrorreportingService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
            });
            return service;
        }

        private static ReportRequest CreateReportRequest(Exception e)
        {
            ClouderrorreportingService service = CreateErrorReportingClient();
            string projectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
            string formattedProjectId = string.Format("projects/{0}", "error-reporting-c-sharp-test");
            ServiceContext serviceContext = new ServiceContext()
            {
                Service = "myapp",
                Version = "8c1917a9eca3475b5a3686d1d44b52908463b989",
            };
            ReportedErrorEvent errorEvent = new ReportedErrorEvent()
            {
                Message = e.ToString(),
                ServiceContext = serviceContext,
            };
            return new ReportRequest(service, errorEvent, formattedProjectId);
        }

        private static void report(Exception e)
        {
            ReportRequest request = CreateReportRequest(e);
            request.Execute();
        }
    }
    // [END example]
}
