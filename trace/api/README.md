# Stackdriver Trace Sample for ASP.NET Framework

A sample demonstrating how to create traces using the ASP.NET Framework and Google Cloud Stackdriver Trace.

## Build and Run

1.  **Follow the instructions in the [root README](../../README.md)**.

2.  Open [Trace.sln](Trace.sln) with Microsoft Visual Studio version 2015 or
later.

3. In [Web.config](Web.config) change the placeholder value of
"YOUR-GOOGLE-PROJECT-ID" to be your project id.

4. Build and Run the solution.

5. Visual Studio will open a web browser loading the sample web app which will
display a confirmation that trace is configured along with a link to initiate
a trace like:

  ```
  Trace has been configured for use with this application.
  See the Stackdriver Trace dashboard in the Google Cloud Console
  to view traces for your application.

    •Initiate a trace
  ```

6. Click the "Initiate a trace" link to create a sample trace.

7. Go back to Visual Studio and stop the program which will still be running.

8. The sample trace is viewable on the
[Stackdriver Trace](https://console.cloud.google.com/traces) page in the
Cloud Platform Console.

## Deploy to a Google Cloud Platform Windows VM

For instructions on how to deploy this sample application to a Google Cloud based Windows VM instance, follow the same steps used to deploy the hello-world sample application in the
[Hello World tutorial](https://cloud.google.com/dotnet/docs/getting-started/hello-world#running_hello_world_on_google_cloud_platform).

## Contributing changes

* See [CONTRIBUTING.md](../../CONTRIBUTING.md)

## Licensing

* See [LICENSE](../../LICENSE)
