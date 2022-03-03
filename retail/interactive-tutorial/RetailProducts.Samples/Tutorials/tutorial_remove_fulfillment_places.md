<walkthrough-metadata>
  <meta name="title" content="Update inventory: remove fulfillment places tutorial" />
  <meta name="description" content="Removing fulfillment places." />
  <meta name="component_id" content="593554" />
</walkthrough-metadata>

# Update inventory: remove fulfillment places tutorial

## Introduction

Changes to a product's inventory information might occur more frequently than changes to its catalog information.

After the fulfillment availability for some specific places has changed, you can push updates instead of using the `UpdateRetailProduct` method to respecify all of the product fulfillment information.

In such cases `AddFulfillment` and `RemoveFulfillment` methods can be used to incrementally update product fulfillment. The place IDs are added to or removed from a given fulfillment type based on the fulfillment changes.

These methods are asynchronous because of downstream optimizations that support hundreds of concurrent updates per product without sacrificing performance.

For more information about managing catalog information, see the [Retail API documentation](https://cloud.google.com/retail/docs/inventory-updates#inventory-update-methods).

<walkthrough-tutorial-duration duration="5"></walkthrough-tutorial-duration>

## Get started with Google Cloud Retail

This step is required if this is the first Retail API tutorial you run.
Otherwise, you can skip it.

### Select your project and enable the Retail API

Google Cloud organizes resources into projects. This lets you
collect all the related resources for a single application in one place.

If you don't have a Google Cloud project yet or you're not the owner of an existing one, you can
[create a new project](https://console.cloud.google.com/projectcreate).

After the project is created, set your PROJECT_ID to a ```project``` variable.
1. Run the following command in Terminal:
    ```bash
    gcloud config set project <YOUR_PROJECT_ID>
    ```

1. Check that the Retail API is enabled for your project in the [Admin Console](https://console.cloud.google.com/ai/retail/).

### Set up authentication

To run a code sample from the Cloud Shell, you need to be authenticated using the service account credentials.

1. Login with your user credentials.
    ```bash
    gcloud auth login
    ```

1. Type `Y` and press **Enter**. Click the link in Terminal. A browser window should appear asking you to log in using your Gmail account.

1. Provide the Google Auth Library with access to your credentials and paste the code from the browser to the Terminal.

1. Upload your service account key JSON file and use it to activate the service account:

    ```bash
    gcloud iam service-accounts keys create ~/key.json --iam-account <YOUR_SERVICE_ACCOUNT_EMAIL>
    ```

    ```bash
    gcloud auth activate-service-account --key-file  ~/key.json
    ```

1. Set key as the GOOGLE_APPLICATION_CREDENTIALS environment variable to be used for requesting the Retail API:
    ```bash
    export GOOGLE_APPLICATION_CREDENTIALS= ~/key.json
    ```

**Note**: Click the copy button on the side of the code box to paste the command in the Cloud Shell terminal and run it.

### Set the PROJECT_NUMBER and PROJECT_ID environment variables

Because you are going to run the code samples in your own Google Cloud project, you should specify the **project_number** and **project_id** as environment variables. It will be used in every request to the Retail API.

1. Find the project number and project ID in the Project Info card displayed on **Home/Dashboard**.

1. Set **project_number** with the following command:
    ```bash
    export PROJECT_NUMBER=<YOUR_PROJECT_NUMBER>
    ```
1. Set **project_id** with the following command:
    ```bash
    export PROJECT_ID=<YOUR_PROJECT_ID>
    ```

### Install Google Cloud Retail libraries

1. Next, install Google packages:
    ```bash
    dotnet add package Google.Cloud.Retail.V2

    ```

## Clone the Retail code samples

This step is required if this is the first Retail API tutorial you run.
Otherwise, you can skip it.

Clone the Git repository with all the code samples to learn the Retail features and check them in action.

<!-- TODO(ianan): change the repository link -->
1. Run the following command in the Terminal:
    ```bash
    git clone https://github.com/GoogleCloudPlatform/dotnet-docs-samples.git
    ```

    The code samples for each of the Retail services are stored in different directories.

1. Go to the ```interactive-tutorial``` directory. This is our starting point to run more commands.
    ```bash
    cd retail/interactive-tutorial/RetailProduct.Samples
    ```

## Remove fulfillment places

The `RemoveFulfillmentPlacesRequest` and `AddFulfillmentPlacesRequest` requests are similar.

1. Go to the <walkthrough-editor-select-regex filePath=""cloudshell_open/interactive-tutorial/RetailProducts.Samples/RemoveFulfillmentPlacesSample.cs" regex="# remove fulfillment request">`GetRemoveFulfillmentRequest()`</walkthrough-editor-select-regex> method and check the `RemoveFulfillmentPlacesRequest` request.

1. Run the code sample in the Terminal with the following command:
    ```bash
    dotnet run -- RemoveFulfillmentPlacesTutorial
    ```

1. Check the response in the Terminal. The product was initially created with fulfillment places `store0` and `store1`.

1. Check the ```GetRetailProduct()``` response. The `store0`  place has been removed from the list of fulfillment places for the `pickup-in-store` type.

## Congratulations

<walkthrough-conclusion-trophy></walkthrough-conclusion-trophy>

You have completed the tutorial! We encourage you to test removing the product fulfillment places by yourself.

<walkthrough-inline-feedback></walkthrough-inline-feedback>

### Do more with the Retail API

<walkthrough-tutorial-card id="retail__retail_api_v2_set_invenory_dotnet" icon="LOGO_DOTNET" title="Set inventory tutorial" keepPrevious=true>
Try to get a product via the Retail API</walkthrough-tutorial-card>

<walkthrough-tutorial-card id="retail__retail_api_v2_add_fulfillment_places_dotnet" icon="LOGO_DOTNET" title="Add fulfillment tutorial" keepPrevious=true>Try to update a product via the Retail API</walkthrough-tutorial-card>