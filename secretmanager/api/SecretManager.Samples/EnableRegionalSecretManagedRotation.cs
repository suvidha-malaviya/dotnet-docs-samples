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

// [START secretmanager_enable_regional_secret_managed_rotation]

using Google.Cloud.SecretManager.V1;
using System;

public class EnableRegionalSecretManagedRotationSample
{
    /// <summary>
    /// Enables managed rotation for a Cloud SQL DB credentials secret. This links the
    /// secret to a Cloud SQL instance and database user, and can only be called once
    /// per secret. It adds the secret's first version and sets the matching password
    /// on the Cloud SQL user, taking the place of a manually added secret version.
    /// Afterwards, use RotateRegionalSecret to trigger further rotations.
    ///
    /// instanceId is the bare Cloud SQL instance ID (e.g. "my-instance"), not a
    /// connection name. Neither the project nor the region should be included: the
    /// service already knows the project from the secret's own path.
    /// </summary>
    public SecretVersion EnableRegionalSecretManagedRotation(
      string projectId = "my-project",
      string locationId = "my-location",
      string secretId = "my-secret",
      string instanceId = "my-instance",
      string username = "my-user"
    )
    {
        // Create the Regional Secret Manager Client.
        SecretManagerServiceClient client = new SecretManagerServiceClientBuilder
        {
            Endpoint = $"secretmanager.{locationId}.rep.googleapis.com"
        }.Build();

        // Build the request. Parent holds the full secret resource name, not a
        // collection parent. Leaving Password unset lets Secret Manager generate a
        // secure password itself.
        EnableManagedRotationRequest request = new EnableManagedRotationRequest
        {
            ParentAsSecretName = SecretName.FromProjectLocationSecret(projectId, locationId, secretId),
            CloudSqlSingleUserCredentials = new EnableManagedRotationRequest.Types.CloudSQLSingleUserCredentials
            {
                InstanceId = instanceId,
                Username = username,
            },
        };

        // Call the API.
        SecretVersion version = client.EnableManagedRotation(request);
        Console.WriteLine($"Enabled managed rotation, created secret version: {version.Name}");
        return version;
    }
}
// [END secretmanager_enable_regional_secret_managed_rotation]
