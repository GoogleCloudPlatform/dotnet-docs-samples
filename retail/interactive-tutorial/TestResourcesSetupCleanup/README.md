# How to set up/ tear down the test resources

## Required environment variables

To successfully import the catalog data for tests, the following environment variables should be set:
 - GOOGLE_CLOUD_PROJECT_ID 
 - RETAIL_BUCKET_NAME
 - RETAIL_EVENTS_BUCKET_NAME

## Import catalog data

There is a JSON file with valid products prepared in the `TestResourcesSetupCleanup` directory:
`TestResourcesSetupCleanup/resources/products.json`.

Run the `CreateTestResources.cs` to perform the following actions:
    - create the GCS bucket <RETAIL_BUCKET_NAME>, 
        - upload the product data from `TestResourcesSetupCleanup/resources/products.json` file to products bucket,
    - import products to the default branch of the Retail catalog,
    - create the GCS bucket <RETAIL_EVENTS_BUCKET_NAME>, 
        - upload the product data from `TestResourcesSetupCleanup/resources/user_events.json` file to events bucket,
    - create a BigQuery dataset and table `products`,
        - insert products from TestResourcesSetupCleanup/resources/products.json to the created products table,
    - create a BigQuery dataset and table `events`,
        - insert user events from `TestResourcesSetupCleanup/resources/user_events.json` to the created events table

```
$ dotnet run -- CreateTestResourcesTutorial
```


## Remove catalog data

Run the `RemoveTestResources.cs` to perform the following actions:
    - remove all objects from the GCS bucket <RETAIL_BUCKET_NAME>, 
    - remove the <RETAIL_BUCKET_NAME> bucket,
    - remove all objects from the GCS bucket <RETAIL_EVENTS_BUCKET_NAME>, 
    - remove the <RETAIL_EVENTS_BUCKET_NAME> bucket,
    - remove dataset `products` along with tables,
    - remove dataset `user_events` along with tables,
    - delete all products from the Retail catalog

```
$ dotnet run -- RemoveTestResourcesTutorial
```