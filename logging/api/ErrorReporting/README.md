# Stackdriver Error Reporting Sample

A sample demonstrating how to use the Error Reporting nuget package
to log all of an ASP.NET web application's errors to the Error Reporting page in the Cloud Platform Console.

## Links

- [Stackdriver Error Reporting](https://cloud.google.com/error-reporting/)

## Build and Run

1.  **Follow the instructions in the [root README](../../../README.md)**.

2.  Enable APIs for your project.
    [Click here](https://console.cloud.google.com/flows/enableapi?apiid=clouderrorreporting.googleapis.com&showconfirmation=true)
    to visit Cloud Platform Console and enable the Stackdriver Error Reporting API.

3.  Open [ErrorReporting.sln](ErrorReporting.sln) with Microsoft Visual Studio version 2015 or later.

4. In [WebApiConfig.cs](App_Start/WebApiConfig.cs) change the placeholder value of "YOUR-PROJECT-ID" to be your project id.

5. Build the Solution.

6. Run the solution.

7. Visual Studio will open a web browser and attempt to load the sample web app. Visual Studio will then take focus and highlight the generic exception error that is thrown by the sample.

8. Continue running the sample by clicking the green *Continue* button or pressing F11.

9. The browser will display an XML error with the message "An error has occurred.".

10. Go back to Visual Studio and stop the program which will still be running.

11. The generic error that was thrown will be logged and viewable in
    the [Error Reporting](https://console.cloud.google.com/errors) page
    in the Cloud Platform Console.


## Contributing changes

* See [CONTRIBUTING.md](../../../CONTRIBUTING.md)

## Licensing

* See [LICENSE](../../../LICENSE)
