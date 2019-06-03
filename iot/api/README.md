# .NET Cloud IoT Core Sample

This sample demonstrates how to manage registries and devices for the
[Google Cloud IoT Core](https://cloud.google.com/iot-core/docs) product.

This sample requires [.NET Core 2.0](https://www.microsoft.com/net/core) or
later.  That means using [Visual Studio 2017](https://www.visualstudio.com/),
or the command line.

## Build and Run

1.  **Follow the set-up instructions in [the documentation](https://cloud.google.com/dotnet/docs/setup).**

2.  Enable APIs for your project.
    [Click here](https://console.cloud.google.com/flows/enableapi?apiid=cloudiot.googleapis.com&showconfirmation=true)
    to visit Cloud Platform Console and enable the Google Cloud IoT Core API.

3. Set the `GOOGLE_APPLICATION_CREDENTIALS` environment variable to the
service account JSON from the Google Cloud Console:

```powershell
    PS > $env:GOOGLE_APPLICATION_CREDENTIALS="creds.json"
```

4. Set the `GOOGLE_PROJECT_ID` environment variable:

```powershell
    PS > $env:GOOGLE_PROJECT_ID="your-project-id"
```

5.  From a Powershell command line, restore the NuGet packages and call
`run` to see a list of subcommands like getRegistry or getDevice:
    ```powershell
    PS C:\...\dotnet-docs-samples\iot\api\CloudIotSample> dotnet restore
    PS C:\...\dotnet-docs-samples\iot\api\CloudIotSample> dotnet run

    CloudIotSample 1.0.0
    Copyright (C) 2019 CloudIotSample

      createRegistry        Create a new Device registry.
      clearRegistry         Removes all devices and then deletes a device Registry.
      deleteRegistry        Delete a registry.
      getRegistry           Retrieve a registry.
      listRegistries        List the registry IDs.
      bindDeviceToGateway   Binds a device to a gateway.
      createDeviceNoAuth    Create a device without associated credentials.
      createDeviceEs        Create device with ES encryption type.
      createDeviceRsa       Create device with RSA encryption type.
      createGateway         Create a gateway to bind devices to.
      deleteDevice          Delete a device from a registry.
      getDevice             Retrieve information for a specific device.
      getDeviceConfigs      Retrieve configurations for a specific device.
      getIamPolicy          Get the IAM policy for a device registry.
      listDevices           List devices in the provided Cloud IoT Core Registry.
      listDevicesForGateway List devices bound to a gateway.
      listGateways          List gateways in a registry.
      unbindAllDevices      Unbinds all devices in a given registry. Mainly for cleaing registry.
      unbindDeviceFromGateway   Unbinds a device from a gateway.
      patchDeviceEs         Patch device with ES encryption type.
      patchDeviceRsa        Create device with RSA encryption type.
      setDeviceConfig       Set the configuration data for a device.
      setIamPolicy          Set the IAM policy for a device registry.
      sendCommand           Send a command to a device.
      help                  Display more information on a specific command.
      version               Display version information.
    ```
# .NET Cloud IoT Core MQTT example

This sample app publishes data to Cloud Pub/Sub using the MQTT bridge provided
as part of Google Cloud IoT Core.

Note that before you can run the sample, you must configure a Google Cloud
PubSub topic for Cloud IoT Core and register a device as described in the
the following setup.

## MQTT Example Setup

1. From the [Google Cloud IoT Core section](https://console.cloud.google.com/iot/)
   of the Google Cloud console, create a device registry.
2. Use the [`generate_keys.sh`](generate_keys.sh) script to generate your signing keys:

```git bash
   $ > ./generate_keys.ps1
```

3. Add a device using the file `rsa_cert.pem`, specifying RS256_X509 and using the
  text copy of the public key starting with the ----START---- block of the certificate.

```git bash
   $ > cat rsa_cert.pem
```

4. Connect a device using the HTTP or MQTT device samples in the [CloudIotMqttExample](./CloudIotMqttExample) folder.

5. Programmattically control device configuration and using the device manager sample in the [CloudIotMqttExample](./CloudIotMqttExample) folder.

## Running the sample

The following command summarizes the sample usage:

From a Powershell command line, restore the NuGet packages and call
`run` to see a list of subcommands like getRegistry or getDevice:

```powershell
    PS C:\...\dotnet-docs-samples\iot\api\CloudIotMqttExample> dotnet restore
    PS C:\...\dotnet-docs-samples\iot\api\CloudIotMqttExample> dotnet run
```

For example, if your project ID is `blue-jet-123`, your device registry is
located in the `asia-east1` region, number of messages you want to publish is `100`,
expiration time on your token is `15 min`, and you have generated your
credentials using the [`generate_keys.ps1`](./generate_keys.ps1) script
provided in the parent folder, you can run the sample as:

```powershell
    PS C:\...\dotnet-docs-samples\iot\api\CloudIotMqttExample> dotnet restore
    PS C:\...\dotnet-docs-samples\iot\api\CloudIotMqttExample> dotnet run startMqtt blue-jet-123 asia-east1 device-registry-id device-id    ../rsa_private.pem RS256 ../roots.pem 100 event mqtt.googleapis.com 8883 15
```

## Reading the messages written by the sample client

1. Create a subscription to your topic.

    gcloud beta pubsub subscriptions create \
        projects/your-project-id/subscriptions/my-subscription \
        --topic device-events

2. Read messages published to the topic

    gcloud beta pubsub subscriptions pull --auto-ack \
        projects/my-iot-project/subscriptions/my-subscription

## Contributing changes

* See [CONTRIBUTING.md](../../CONTRIBUTING.md)

## Licensing

* See [LICENSE](../../LICENSE)

## Testing

* See [TESTING.md](../../TESTING.md)
