# Powershell Storage Sample

The [Windows PowerShell cmdlets for the Google Cloud Platform]
(http://googlecloudplatform.github.io/google-cloud-powershell/#/)
let you upload, download, and delete objects from [Google Cloud Storage]
[GCS].

This sample is a larger application that recursively copies
directories to and from [Google Cloud Storage][GCS].  In addition to
showing you how to call cmdlets like `New-GcsObject`, it's also a
really handy command line utility.

## Prerequisites

Download and install the [Google Cloud SDK](https://cloud.google.com/sdk).

## Run
```
NAME
    Copy-GcsObject.ps1

SYNOPSIS
    Copies files to or from Google Cloud Storage.


SYNTAX
    Copy-GcsObject.ps1 [-SourcePath] <String> [-DestPath] <String> [-Force] [-Recurse] [<CommonParameters>]


DESCRIPTION
    Requires the Google Cloud SDK be installed.

    Google Cloud Storage paths look like:
    gs://bucket/a/b/c.txt

    Does not support wildcards like * or ?.
    Does not support paths with . or ..
```

## Contributing changes

* See [CONTRIBUTING.md](../../CONTRIBUTING.md)

## Licensing

* See [LICENSE](../../LICENSE)

## Testing

* See [TESTING.md](../../TESTING.md)

[GCS]: https://cloud.google.com/storage/docs/