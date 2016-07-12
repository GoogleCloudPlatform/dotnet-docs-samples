# Powershell Storage Sample

Demonstrates how to copy files to and from Google Cloud Storage using powershell.

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
