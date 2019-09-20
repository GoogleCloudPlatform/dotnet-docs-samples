# Running tests locally in the same environment as Kokoro

## Audience

This document contains instructions for Google employees only.  If you are
not a Google employee, these instructions will fail because you don't have
permission to access secret keys.

## Background

The kokoro tests depend on:

- [chocolatey](https://chocolatey.org/) packages like `microsoft-build-tools`.
- other, downloadable packages like [casperjs](http://casperjs.org/) and  
[phantomjs](http://phantomjs.org/).
- secret keys for accessing resources in Google Cloud.  For example, for the  
   CloudSql tests to pass, there must be an environment variable set containing
   a connection string.

The scripts in this directory install those dependencies.

## Prerequisites

1. [Chocolatey](https://chocolatey.org/).
2. [Google Cloud SDK](http://cloud.google.com/sdk).  Sign into gcloud with
    your corp credentials.

    ```.ps1
    PS > gcloud auth login
    ```

## Loading the kokoro environment

1. Run Powershell **as an Administrator.**
2. In Powershell, run

    ```ps1
    PS C:\...\dotnet-docs-samples> .kokoro-windows\New-BuildEnv.ps1
    ```

3. Close the Powershell Window.
4. Run Powershell, this time **not** as an Administrator.
5. Run these commands:

    ```ps1
    PS C:\...\dotnet-docs-samples> env\Activate.ps1
    # Now the kokoro build environment is loaded.  Run any test.
    PS C:\...\dotnet-docs-samples> .\buildAndRunTests.ps1
    ```
