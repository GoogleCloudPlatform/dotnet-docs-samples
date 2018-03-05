# .NET Cloud IoT Core Sample

This sample demonstrates how to manage registries and devices for the
[Google Cloud IoT Core](https://cloud.google.com/iot-core/docs) product.

This sample requires [.NET Core 2.0](https://www.microsoft.com/net/core) or
later.  That means using [Visual Studio 2017](https://www.visualstudio.com/),
or the command line.

## Build and Run

1.  **Follow the instructions in the [root README](../../README.md)**.

2.  Enable APIs for your project.
    [Click here](https://console.cloud.google.com/flows/enableapi?apiid=cloudiot.googleapis.com&showconfirmation=true)
    to visit Cloud Platform Console and enable the Google Cloud IoT Core API.

3. Set the `GOOGLE_APPLICATION_CREDENTIALS` environment variable to the
service account JSON from the Google Cloud Console:

```
    set GOOGLE_APPLICATION_CREDENTIALS=creds.json
```

4. Set the `GOOGLE_PROJECT_ID` environment variable:

```
    set GOOGLE_PROJECT_ID=your-project-id
```

5.  From a Powershell command line, restore the NuGet packages and call
`run` to see a list of subcommands like getRegistry or getDevice:
    ```
    PS C:\...\dotnet-docs-samples\iot\api\> dotnet restore
    PS C:\...\dotnet-docs-samples\iot\api\> dotnet run

    CloudIotSample 1.0.0
    Copyright (C) 2018 CloudIotSample

      createRegistry        Create a new Device registry.
      deleteRegistry        Delete a registry.
      getRegistry           Retrieve a registry.
      listRegistries        List the registry IDs.
      createDeviceNoAuth    Create a device without associated credentials.
      createDeviceEs        Create device with ES encryption type.
      createDeviceRsa       Create device with RSA encryption type.
      deleteDevice          Delete a device from a registry.
      getDevice             Retrieve information for a specific device.
      getDeviceConfigs      Retrieve configurations for a specific device.
      getIamPolicy          Get the IAM policy for a device registry.
      listDevices           List devices in the provided Cloud IoT Core Registry.
      patchDeviceEs         Patch device with ES encryption type.
      patchDeviceRsa        Create device with RSA encryption type.
      setDeviceConfig       Set the configuration data for a device.
      setIamPolicy          Set the IAM policy for a device registry.
      help                  Display more information on a specific command.
      version               Display version information.
    ```

## Contributing changes

* See [CONTRIBUTING.md](../../CONTRIBUTING.md)

## Licensing

* See [LICENSE](../../LICENSE)

## Testing

* See [TESTING.md](../../TESTING.md)
