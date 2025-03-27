from google.auth import impersonated_credentials
from google.cloud import logging
from google_auth_oauthlib.flow import InstalledAppFlow

LOG_PROJECT_ID = "ajhall-logsink"
CLIENT_SECRETS_FILE = "oauth_client_secret.json"
IMPERSONATED_SERVICE_ACCOUNT = "log-writer@ajhall-logsink.iam.gserviceaccount.com"
SCOPES = [
    "openid",
    "https://www.googleapis.com/auth/cloud-platform",
]

flow = InstalledAppFlow.from_client_secrets_file(CLIENT_SECRETS_FILE, SCOPES)
creds = flow.run_local_server(port=8080)
print("Signed in")

impersonated_creds = impersonated_credentials.Credentials(
    source_credentials=creds,
    target_principal=IMPERSONATED_SERVICE_ACCOUNT,
    target_scopes=["https://www.googleapis.com/auth/logging.write"],
)
print("Impersonated service account:", IMPERSONATED_SERVICE_ACCOUNT)

client = logging.Client(project=LOG_PROJECT_ID, credentials=impersonated_creds)
logger = client.logger("user_logs")
logger.log_text("Test log entry")
print("Log entry created in project:", LOG_PROJECT_ID)
