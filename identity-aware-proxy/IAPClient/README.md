# Google Cloud Identity-Aware Proxy Samples

This sample shows how to authenticate an http request when requesting a web page from
a Google App Engine application protected by the [Google Cloud Identity-Aware Proxy](http://cloud.google.com/iap).

This sample requires [.NET Core 2.0](
    https://www.microsoft.com/net/core) or later.  That means using
[Visual Studio 2017](
    https://www.visualstudio.com/), or the command line.

## Build and Run

1.  **Follow the instructions in the [root README](../../README.md)**.

2.  Follow the instructions here
    [https://cloud.google.com/iap/docs/app-engine-quickstart](https://cloud.google.com/iap/docs/app-engine-quickstart)
    to create an App Engine application and enable Identity-Aware Proxy (IAP).

3.  Follow the instructions here 
    [https://cloud.google.com/iap/docs/authentication-howto](https://cloud.google.com/iap/docs/authentication-howto#iap-make-request-csharp)
    to download service account credentials and observe your IAP client id.

9.  From a Powershell command line, run to see command line arguments.
    ```
    PS > dotnet run
    IAPClient 1.0.0
    Copyright (C) 2018 IAPClient
    ERROR(S):
    Required option 'u, uri' is missing.

    -j, --credentials    Path to your service account credentials .json file.

    -u, --uri            Required. The URI to fetch.

    -c, --iapcid         Your IAP client id listed on https://console.cloud.google.com/apis/credentials

    --help               Display this help screen.

    --version            Display version information.
    ```
    
9.  And pass your own values:
    ```
    PS > dotnet run -- -u https://my-project.appspot.com `
        -c 71377614389-vn0dmho3h2utdus0v3jq9dcf9foj1gph.apps.googleusercontent.com `
        -j /path/to/my/credentials.json
    <!--
    /*
    * Copyright (c) 2017 Google Inc.
    *
    * Licensed under the Apache License, Version 2.0 (the "License"); you may not
    * use this file except in compliance with the License. You may obtain a copy of
    * the License at
    *
    * http://www.apache.org/licenses/LICENSE-2.0
    *
    * Unless required by applicable law or agreed to in writing, software
    * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
    * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
    * License for the specific language governing permissions and limitations under
    * the License.
    */
    -->
    <!DOCTYPE html>
    <!--  [START sample] -->
    <html>
    <head>
        <meta charset="utf-8" />
        <title>Hello Static World</title>
    </head>
    <body>
        <p>This is a static html document.</p>
        <p><img src="trees.jpg" /></p>
    </body>
    </html>
    <!-- [END sample] -->
    ```

## Contributing changes

* See [CONTRIBUTING.md](../../CONTRIBUTING.md)

## Licensing

* See [LICENSE](../../LICENSE)

## Testing

* See [TESTING.md](../../TESTING.md)
