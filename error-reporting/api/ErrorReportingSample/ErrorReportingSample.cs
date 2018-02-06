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

        /// <summary>
        /// Create the Error Reporting service (<seealso cref="ClouderrorreportingService"/>)
        /// with the Application Default Credentials and the proper scopes.
        /// See: https://developers.google.com/identity/protocols/application-default-credentials.
        /// </summary>
        private static ClouderrorreportingService CreateErrorReportingClient()
        {
            // Get the Application Default Credentials.
            GoogleCredential credential = GoogleCredential.GetApplicationDefaultAsync().Result;

            // Add the needed scope to the credentials.
            credential = credential.CreateScoped(ClouderrorreportingService.Scope.CloudPlatform);

            // Create the Error Reporting Service.
            ClouderrorreportingService service = new ClouderrorreportingService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
            });
            return service;
        }

        /// <summary>
        /// Creates a <seealso cref="ReportRequest"/> from a given exception.
        /// </summary>
        private static ReportRequest CreateReportRequest(Exception e)
        {
            // Create the service.
            ClouderrorreportingService service = CreateErrorReportingClient();

            // Get the project ID from the environement variables.
            string projectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");

            // Format the project id to the format Error Reporting expects. See:
            // https://cloud.google.com/error-reporting/reference/rest/v1beta1/projects.events/report
            string formattedProjectId = string.Format("projects/{0}", projectId);

            // Add a service context to the report.  For more details see:
            // https://cloud.google.com/error-reporting/reference/rest/v1beta1/projects.events#ServiceContext
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

        /// <summary>
        /// Report an exception to the Error Reporting service.
        /// </summary>
        private static void report(Exception e)
        {
            // Create the report and execute the request.
            ReportRequest request = CreateReportRequest(e);
            request.Execute();
        }
    }
    // [END example]
}
