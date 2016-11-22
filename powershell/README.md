# Powershell Samples

The [Windows PowerShell cmdlets for the Google Cloud Platform](
http://googlecloudplatform.github.io/google-cloud-powershell/#/)
let you manage your Google Cloud resources via powershell.

## Prerequisites

Download and install the [Google Cloud SDK](https://cloud.google.com/sdk).

## Samples:

### In other directories:
* **[`Copy-GcsObject.ps1`](../storage/powershell/)**
    Recursively copy directories to and from Cloud Storage.

### In this directory:
* **[`Find-GcResource.ps1`](Find-GcResource.ps1)**
    ```
    NAME
        Find-GcResource.ps1
        
    SYNOPSIS
        List all the resources visible to Google Cloud Tools for PowerShell.
        
    SYNTAX
        .\Find-GcResource.ps1 [[-ProjectId] <String>] 
        [<CommonParameters>]
                
    DESCRIPTION
        This doesn't list everything.  For example, it tells you nothing about logs
        or datastore.  As additional commands are added to
        Google Cloud Tools for PowerShell, they'll be added here.
    ```

## Contributing changes

* See [CONTRIBUTING.md](../CONTRIBUTING.md)

## Licensing

* See [LICENSE](../LICENSE)

## Testing

* See [TESTING.md](../TESTING.md)
