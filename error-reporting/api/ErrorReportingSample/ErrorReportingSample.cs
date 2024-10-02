/**
 * Copyright 2016 Google Inc.
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
* https://cloud.google.com/error-reporting/docs/reference/libraries
*/

// [START error_reporting_quickstart]

using Google.Api.Gax.ResourceNames;
using Google.Cloud.ErrorReporting.V1Beta1;
using System;

public class ErrorReportingSample
{
    static String projectId;

    public static void Main(string[] args)
    {
	    // Set your Google Cloud Platform project ID via environment or explicitly
        if (args != null && args.Length > 0 && !String.IsNullOrEmpty(args[0]))
        {
            projectId = args[0];
        }
        else
        {
            projectId = Environment.GetEnvironmentVariable("GOOGLE_CLOUD_PROJECT");
        }

        try
        {
            throw new Exception("Something went wrong");
        }
        catch (Exception e)
        {
            ReportError(e);
            Console.WriteLine("Stackdriver Error Report Sent");
        }
    }


    /// <summary>
    /// Report an exception to the Error Reporting service.
    /// </summary>
    private static void ReportError(Exception e)
    {
        // Create the report and execute the request.
        var reporter = ReportErrorsServiceClient.Create();
        var projectName = new ProjectName(projectId);

        // Optionally add a service context to the report. For more details see:
        // https://cloud.google.com/error-reporting/reference/rest/v1beta1/ServiceContext
        var assemblyName = System.Reflection.Assembly.GetEntryAssembly().GetName();
        var serviceContext = new ServiceContext()
        {
            Service = assemblyName.Name,
            Version = assemblyName.Version.ToString(),
        };
        var errorEvent = new ReportedErrorEvent()
        {
            Message = e.ToString(),
            ServiceContext = serviceContext,
        };
        reporter.ReportErrorEvent(projectName, errorEvent);
    }
}
// [END error_reporting_quickstart]
