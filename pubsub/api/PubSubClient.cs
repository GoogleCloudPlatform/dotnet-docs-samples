using Google.Apis.Services;
using Google.Apis.Pubsub.v1;

class PubSubClient
{
  public static PubsubService Create()
  {
    var credentials = Google.Apis.Auth.OAuth2.GoogleCredential.GetApplicationDefaultAsync().Result;
    credentials = credentials.CreateScoped(new[] { PubsubService.Scope.Pubsub });

    var serviceInitializer = new BaseClientService.Initializer()
    {
      ApplicationName = "PubSub Sample",
      HttpClientInitializer = credentials
    };

    return new PubsubService(serviceInitializer);
  }
}