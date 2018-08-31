# Log4Net Logging Sample

A sample demonstrating how create a custom log entry using Log4Net and Google Cloud Stackdriver Logging.

## Links

- [Log4Net](https://logging.apache.org/log4net/)
- [Stackdriver Logging](https://cloud.google.com/logging/)

## Build and Run

1.  **Follow the set-up instructions in [the documentation](https://cloud.google.com/dotnet/docs/setup).**

2.  Open [Log4NetSample.sln](Log4NetSample.sln) with Microsoft Visual Studio version 2015 or later.

4. In [Web.config](Web.config) change the placeholder value of "YOUR-PROJECT-ID" to be your project id.

5. Build the Solution.

6. Run the solution.

7. Visual Studio will open a web browser loading the sample web app which will create a sample log entry in your project and display a message like:

  ```
  Log Entry created in project: sample-project-78557
  ```

8. Go back to Visual Studio and stop the program which will still be running.

9. The sample log entry is viewable on the [Stackdriver Logging](https://console.cloud.google.com/logs/viewer) page in the Cloud Platform Console.


## Contributing changes

* See [CONTRIBUTING.md](../../../CONTRIBUTING.md)

## Licensing

* See [LICENSE](../../../LICENSE)
