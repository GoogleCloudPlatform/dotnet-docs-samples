Google Home Meets .NET Containers on Google Cloud
=================================================
This is a demo to show how to connect a Google Assistant enabled device (eg. Google Home) to a .NET Container running on Google Cloud.

This server can run locally and also on a Google Cloud Platform. It's only fully tested under Google App Engine (Flex); and these instructions will assume it's running on App Engine. However, there's no reason why it cannot run on Google Kubernetes Engine (GKE).

## ASP.NET Server Configuration

### Google Cloud configuration

You must have a GCP project with the following APIs enabled:

* Custom Search API
* Cloud Vision API
* BigQuery API
* Translation API
* Container Builder API

### Custom Search configuration

Visit [https://cse.google.co.uk/cse](https://cse.google.co.uk/cse) and create a new search engine under `Custom Search` following these steps:
1. Go to `New search engine`
2. Enter a valid site for `Sites to search`. The actual site itself does not matter 
3. Give your search engine a name under `Name of the search engine` and create
4. Edit the created search engine and go to `Setup` section. 
5. Make sure `Image Search` is on, under `Sites to search`, select `Search the entire web..` and update

You will need the `Search engine ID`, which can be found by clicking the
`Search engine ID` button in the the basic settings.
 
### Server code

The server side of this code is in the `GoogleHomeAspNetCoreDemoServer` project. We tested this project with Visual Studio 2017 Version 15.6.0. Compiling in earlier version might not work.

### Configuration before deployment

You need to change three settings in the `appsettings.json` file in this project.

* `ProjectId` for your Google Cloud project.
* `ApiKey` must be a valid API key for your
  project. An API key can be created in the Google Developer Console under
  the `APIs & Services` -> `Credentials` menu.
* `EngineId` must be the Custom Search Engine ID, as discussed above in the *Custom Search configuration* section.

### Deployment

It's easiest to deploy to App Engine Flex using the Google Cloud VS extension
([https://cloud.google.com/visual-studio/](https://cloud.google.com/visual-studio/)):
* Right click on the project -> `Publish to Google Cloud` -> `App Engine`  
* Pick something meaninful for version. Make sure you only use 1 VM, as the server keeps all state in memory only, so all requests must be served by the same instance. 

Alternatively, you can also deploy using `gcloud` command line tool:
* Publish the app with `dotnet publish -c Release` command.
* Copy `app.yaml` and `Dockerfile` to `publish` folder.
* Inside `publish` folder, `gcloud app deploy --version v0` and follow the prompts. 

### A note on DialogFlow Fullfillment

We will use DialogFlow's WebHook to fullfill requests. DialogFlow requires an HTTPS URL for its fulfillment URL. 

By default, App Engine Flex provides a public domain name with HTTPS and certs already setup, so no additional work is needed.

If you want to deploy to GKE, you need to make the service available externally via HTTPS at a public IP and you need to point a domain name to this public IP with a proper cert. 

### Check it works

Once the external access is configured, there is a test URL at `http://<your domain here>/Test`.

## DialogFlow Configuration

Create a DialogFlow project at [console.dialogflow.com](https://console.dialogflow.com/), and in the settings use the `Export and Import` section to `restore from zip` using the `GoogleHomeMeetsDotNetAgent.zip` file included here.

Ensure the `DialogFlow V2 API` option is enabled.

Update the `Fulfillment` `Webhook` with the correct URL for your project.

This must be an HTTPS URL, and will be in the format: `https://<your domain here>/Conversation`.

If you want to test the server locally, you can use [ngrok](https://ngrok.com/) which can expose your local server behind a public HTTP/HTTPS endpoint. Then, you can use the ngrok HTTPS endpoint as fullfillment URL in Dialogflow. However, ASP.NET apps get angry when they see a different host header than expected, so you need to start ngrok as follows:

`ngrok http [port] -host-header="localhost:[port]"`

### Testing in DialogFlow

The DialogFlow console has a "Try it now" tab on the right. This can be used at any time to test that the DialogFlow/backend-server is working correctly.

Typing text here is treated exactly the same as if the text had arrived from the Google Home/Assistant.

You can use the following phrases to test.
* Say hi to everyone
* What platform are you running on?
* I want to use Vision API
  * Search pictures of London
  * Describe the picture
  * Is picture safe?
  * Are there landmarks in the picture?
  * Go back to all pictures
* I want to use Big Query
  * What was the top hacker news on May 1 2018? 
  * In 2015, what was the hottest temperature in France?
* Translate how are you to German
* Throw an exception

### Google Home/Assistant configuration

Once all the above is configured and running, there is nothing much to do to make the Google Home/Assistant work.

Because the DialogFlow app is not public, and is a "test app", you always need to ask the assistant to let it talk to your test app, by saying: *"Let me talk to my test app"*

Home/Assistant will then confirm you are talking to the test app (i.e. this demo) and all further speech is routed directly to the DialogFlow project.

To stop talking to the test app, say: *"Goodbye"*.

## Run

When demoing, first check the server is deployed, then point a web browser to: `https://<your domain here>/`. This updates automatically during the demo in responce to the spoken queries.

This web page will auto-reconnect to the server, so does not need refreshing during the demo, even if you re-deploy the server as part of the demo. Although refreshing the page won't do any harm.

Then simply ask your Google Home/Assistant to ***"Let me talk to my test app"*** and away you go...

Good luck.
-------

This is not an official Google product.