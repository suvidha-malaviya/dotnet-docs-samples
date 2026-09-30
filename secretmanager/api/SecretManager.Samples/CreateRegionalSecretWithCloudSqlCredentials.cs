/*
 * Copyright 2026 Google LLC
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     https://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

// [START secretmanager_create_regional_secret_with_cloud_sql_credentials]

using Google.Api.Gax.ResourceNames;
using Google.Cloud.SecretManager.V1;
using System;

public class CreateRegionalSecretWithCloudSqlCredentialsSample
{
    /// <summary>
    /// Creates a secret with the Cloud SQL DB credentials secret type. This type is
    /// required to enable Secret Manager's managed rotation of Cloud SQL passwords.
    /// It can only be set when the secret is created, and the secret's location must
    /// match the region of the target Cloud SQL instance.
    /// </summary>
    public Secret CreateRegionalSecretWithCloudSqlCredentials(
      string projectId = "my-project",
      string locationId = "my-location",
      string secretId = "my-secret"
    )
    {
        // Create the Regional Secret Manager Client.
        SecretManagerServiceClient client = new SecretManagerServiceClientBuilder
        {
            Endpoint = $"secretmanager.{locationId}.rep.googleapis.com"
        }.Build();

        // Build the parent resource name.
        LocationName location = new LocationName(projectId, locationId);

        // Build the secret.
        Secret secret = new Secret
        {
            SecretType = Secret.Types.SecretType.CloudSqlDbCredentials,
        };

        // Call the API.
        Secret createdSecret = client.CreateSecret(location, secretId, secret);
        Console.WriteLine($"Created secret: {createdSecret.Name}");

        // This built-in identity is what you grant Cloud SQL IAM permissions to,
        // so that Secret Manager can rotate the database password on its behalf.
        Console.WriteLine(
          "Grant this identity Cloud SQL IAM permissions to enable rotation: " +
          createdSecret.PolicyMember.IamPolicyUidPrincipal);

        return createdSecret;
    }
}
// [END secretmanager_create_regional_secret_with_cloud_sql_credentials]
