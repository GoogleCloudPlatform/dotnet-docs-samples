# How to set up/ tear down the test resources

## Required environment variables

To successfully import the catalog data for tests, the following environment variables should be set:
 - GOOGLE_CLOUD_PROJECT_NUMBER
 - GOOGLE_CLOUD_PROJECT_ID 
 - BUCKET_NAME
 - EVENTS_BUCKET_NAME

These values are stored in the Secret Manager and will be submitted as 
   docker environment variables before the test run.

The Secret Manager name is set in .kokoro/presubmit/common.cfg file, SECRET_MANAGER_KEYS variable.

## Import catalog data

There is a JSON file with valid products prepared in the `TestResourcesSetupCleanup` directory:
`TestResourcesSetupCleanup/resources/products.json`.

Run the `CreateTestResources.cs` to perform the following actions:
    - create the GCS bucket <BUCKET_NAME>, 
        - upload the product data from `TestResourcesSetupCleanup/resources/products.json` file to products bucket,
    - import products to the default branch of the Retail catalog,
    - create the GCS bucket <EVENTS_BUCKET_NAME>, 
        - upload the product data from `TestResourcesSetupCleanup/resources/user_events.json` file to events bucket,
    - create a BigQuery dataset and table `products`,
        - insert products from TestResourcesSetupCleanup/resources/products.json to the created products table,
    - create a BigQuery dataset and table `events`,
        - insert user events from `TestResourcesSetupCleanup/resources/user_events.json` to the created events table

```
$ dotnet run -- CreateTestResources.cs
```

In the result 316 products should be created in the test project catalog.


## Remove catalog data

Run the `RemoveTestResources.cs` to perform the following actions:
    - remove all objects from the GCS bucket <BUCKET_NAME>, 
    - remove the <BUCKET_NAME> bucket,
    - delete all products from the Retail catalog.
    - remove all objects from the GCS bucket <EVENTS_BUCKET_NAME>, 
    - remove the <EVENTS_BUCKET_NAME> bucket,
    - remove dataset `products` along with tables
    - remove dataset `user_events` along with tables 

```
$ dotnet run -- RemoveTestResources.cs
```