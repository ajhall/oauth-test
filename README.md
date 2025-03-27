# Cross-project log writing via OAuth sign-in

OAuth project needs:
- `iamcredentials.googleapis.com` API enabled
- No additional scopes added to the OAuth consent screen

Log project needs:
- Logging API enabled
- `log-writer` service account with the `roles/logging.logWriter` role
- Grant the `roles/iam.serviceAccountTokenCreator` role to users on `log-writer` service account

Make sure to include these scopes when you sign in:
- `openid`
- `https://www.googleapis.com/auth/cloud-platform`

The user logs in via the OAuth project, then uses their token to call the IAM Credentials API to get a service account token with the `https://www.googleapis.com/auth/logging.write` scope. This token is used to call the Logging API to write logs.
