# Google Pubsub and Google App Engine Flexible Environment

This sample application demonstrates how to publish and receive
messages via Google Pubsub when running in Google App Engine Flexible Environment.

For detailed steps to deploy this app, check out
[the Google Cloud
documentation](https://cloud.google.com/appengine/docs/flexible/writing-and-responding-to-pub-sub-messages?tab=.net).

To test pushing messages to the app, you can run the following sample curl command:
`curl -X POST -H "Content-Type: application/json" -d '{"message":{"data":"SGVsbG8sIFdvcmxkIQ=="}}' localhost:5000/push?token=$TEST_VERIFICATION_TOKEN`
