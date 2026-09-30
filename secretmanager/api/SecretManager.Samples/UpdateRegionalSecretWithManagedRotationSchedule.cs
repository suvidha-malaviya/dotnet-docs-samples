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

// [START secretmanager_update_regional_secret_with_managed_rotation_schedule]

using Google.Cloud.SecretManager.V1;
using Google.Protobuf.WellKnownTypes;
using System;

public class UpdateRegionalSecretWithManagedRotationScheduleSample
{
    /// <summary>
    /// Reconfigures the recurring rotation schedule on a secret that already has
    /// Cloud SQL managed rotation enabled (see EnableRegionalSecretManagedRotation).
    /// This only applies to regional secrets of the CLOUD_SQL_DB_CREDENTIALS type.
    ///
    /// rotationPeriodSeconds is the interval between rotations, in whole seconds. The
    /// service requires it to be at least 3600 (1 hour), and the derived next rotation
    /// time (now + rotationPeriodSeconds) must be at least 300 seconds in the future.
    /// Both are enforced by the API, not checked client-side here.
    /// </summary>
    public Secret UpdateRegionalSecretWithManagedRotationSchedule(
      string projectId = "my-project",
      string locationId = "my-location",
      string secretId = "my-secret",
      long rotationPeriodSeconds = 3600
    )
    {
        // Create the Regional Secret Manager Client.
        SecretManagerServiceClient client = new SecretManagerServiceClientBuilder
        {
            Endpoint = $"secretmanager.{locationId}.rep.googleapis.com"
        }.Build();

        // NextRotationTime and RotationPeriod must be set together.
        Timestamp nextRotationTime = Timestamp.FromDateTimeOffset(
          DateTimeOffset.UtcNow.AddSeconds(rotationPeriodSeconds));

        // Build the secret with updated fields.
        Secret secret = new Secret
        {
            SecretName = SecretName.FromProjectLocationSecret(projectId, locationId, secretId),
            Rotation = new Rotation
            {
                NextRotationTime = nextRotationTime,
                RotationPeriod = new Duration { Seconds = rotationPeriodSeconds },
            },
        };

        // Build the field mask. Mask only the two subfields being set, not the whole
        // "rotation" submessage: that would also include the output-only
        // managed_rotation_status, and the API rejects it as immutable.
        FieldMask fieldMask = new FieldMask
        {
            Paths = { "rotation.next_rotation_time", "rotation.rotation_period" }
        };

        // Call the API.
        Secret updatedSecret = client.UpdateSecret(secret, fieldMask);
        Console.WriteLine($"Updated regional secret rotation schedule: {updatedSecret.Name}");
        return updatedSecret;
    }
}
// [END secretmanager_update_regional_secret_with_managed_rotation_schedule]
