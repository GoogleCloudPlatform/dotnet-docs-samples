﻿/**
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
* https://cloud.google.com/error-reporting/docs/reference/libraries
*/

// [START error_reporting_quickstart]

using Google.Api.Gax.ResourceNames;
using Google.Cloud.ErrorReporting.V1Beta1;
using System;

public class ErrorReportingSample
{
    public static void Main(string[] args)
    {
        try
        {
            throw new Exception("Something went wrong");
        }
        catch (Exception e)
        {
            report(e);
            Console.WriteLine("Stackdriver Error Report Sent");
        }
    }


    /// <summary>
    /// Report an exception to the Error Reporting service.
    /// </summary>
    private static void report(Exception e)
    {
        // Create the report and execute the request.
        var reporter = ReportErrorsServiceClient.Create();

        // Get the project ID from the environement variables.
        var projectName = new ProjectName(
            Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID"));

        // Add a service context to the report. For more details see:
        // https://cloud.google.com/error-reporting/reference/rest/v1beta1/ServiceContext
        var assemblyName = System.Reflection.Assembly.GetExecutingAssembly().GetName();
        var serviceContext = new ServiceContext()
        {
            Service = assemblyName.Name,
            Version = assemblyName.Version.ToString(),
        };
        // Add an error context to the report. For more details see:
        // https://cloud.google.com/error-reporting/reference/rest/v1beta1/ErrorContext
        var stackTrace = new System.Diagnostics.StackTrace(e, true);
        var stackFrame = stackTrace.GetFrame(stackTrace.FrameCount - 1);
        var errorContext = new ErrorContext()
        {
            ReportLocation = new SourceLocation()
            {
                FilePath = stackFrame.GetFileName(),
                LineNumber = stackFrame.GetFileLineNumber(),
                FunctionName = stackFrame.GetMethod().Name,
            },
        };
        var errorEvent = new ReportedErrorEvent()
        {
            Message = e.ToString(),
            ServiceContext = serviceContext,
            Context = errorContext,
        };
        reporter.ReportErrorEvent(projectName, errorEvent);
    }
}
// [END error_reporting_quickstart]
