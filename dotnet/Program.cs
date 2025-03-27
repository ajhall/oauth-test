using Google.Api;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Cloud.Iam.Credentials.V1;
using Google.Cloud.Logging.V2;

const string ClientSecretsFile = "oauth_client_secret.json";
const string LogProjectId = "ajhall-logsink";
const string ImpersonatedServiceAccount = "log-writer@ajhall-logsink.iam.gserviceaccount.com";

var flow = new GoogleAuthorizationCodeFlow(
    new GoogleAuthorizationCodeFlow.Initializer
    {
        ClientSecrets = GoogleClientSecrets.FromFile(ClientSecretsFile).Secrets,
        Scopes = ["https://www.googleapis.com/auth/cloud-platform"],
    }
);
var authApp = new AuthorizationCodeInstalledApp(flow, new LocalServerCodeReceiver());

var userCredential = await authApp.AuthorizeAsync("user", CancellationToken.None);
Console.WriteLine("Signed in");

var credsClient = new IAMCredentialsClientBuilder { Credential = userCredential }.Build();
var accessTokenResponse = credsClient.GenerateAccessToken(
    new GenerateAccessTokenRequest
    {
        Name = ImpersonatedServiceAccount,
        Scope = { "https://www.googleapis.com/auth/logging.write" },
    }
);
Console.WriteLine($"Impersonated service account: {ImpersonatedServiceAccount}");

var loggingServiceV2ClientBuilder = new LoggingServiceV2ClientBuilder()
{
  GoogleCredential = GoogleCredential.FromAccessToken(accessTokenResponse.AccessToken),
};

var loggingClient = loggingServiceV2ClientBuilder.Build();

var logName = new LogName(LogProjectId, "user_logs");
var logEntry = new LogEntry
{
    LogName = logName.ToString(),
    TextPayload = "Test log entry",
    Resource = new MonitoredResource { Type = "global" },
};

loggingClient.WriteLogEntries(logName, null, null, [logEntry]);
Console.WriteLine($"Log entry created in project: {LogProjectId}");
