# .NET Cloud Job Discovery Sample

A sample that demonstrates how to call the
[Google Cloud Job Discovery API](https://cloud.google.com/job-discovery/docs) from C#.

This sample requires [.NET Core 2.0](
    https://www.microsoft.com/net/core) or later.

## Build and Run

1. **Follow the set-up instructions in [the documentation](https://cloud.google.com/dotnet/docs/setup).**

2. Ensure that your environment variables are set for both `GOOGLE_APPLICATION_CREDENTIALS` and `GOOGLE_PROJECT_ID`.

3. Enable APIs for your project.
   [Click here](https://console.cloud.google.com/flows/enableapi?apiid=jobs.googleapis.com&showconfirmation=true)
   to visit Cloud Platform Console and enable the Google Cloud Key Management Service API.

4. Enable data logging. 
   [Click here](https://cloud.google.com/job-discovery/docs/before-you-begin)
   to visit Cloud Platform Console and enable data logging..

5. From a Powershell command line:
   ```
   PS C:\...\dotnet-docs-samples\jobs\api\v3\QuickStart> dotnet restore
   PS C:\...\dotnet-docs-samples\jobs\api\v3\QuickStart> dotnet run
   ```

## Contributing changes

* See [CONTRIBUTING.md](../../../../CONTRIBUTING.md)

## Licensing

* See [LICENSE](../../../../LICENSE)

## Testing

1. Navigate to the JobsTest directory.

2. Ensure that your environment variables are set for both `GOOGLE_APPLICATION_CREDENTIALS` and `GOOGLE_PROJECT_ID`.

3. From a Powershell command line:

   ```
   PS C:\...\dotnet-docs-samples\jobs\api\v3\JobsTest> dotnet restore
   PS C:\...\dotnet-docs-samples\jobs\api\v3\JobsTest> dotnet test --test-adapter-path:. --logger:junit
   ```
4. See [TESTING.md](../../../../TESTING.md)
