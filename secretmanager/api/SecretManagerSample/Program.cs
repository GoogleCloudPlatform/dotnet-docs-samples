// Copyright(c) 2020 Google Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License"); you may not
// use this file except in compliance with the License. You may obtain a copy of
// the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
// License for the specific language governing permissions and limitations under
// the License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using CommandLine;
using Google.Api.Gax.ResourceNames;
using Google.Cloud.Iam.V1;
using Google.Cloud.SecretManager.V1Beta1;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GoogleCloudSamples
{
    class ProjectOptions
    {
        [Value(0, HelpText = "Project in which the secrets reside", Required = true)]
        public string ProjectId { get; set; }
    }

    class SecretOptions
    {
        [Value(0, HelpText = "Project for the secret", Required = true)]
        public string ProjectId { get; set; }

        [Value(1, HelpText = "ID of the secret", Required = true)]
        public string SecretId { get; set; }
    }

    class SecretIAMOptions
    {
        [Value(0, HelpText = "Project for the secret", Required = true)]
        public string ProjectId { get; set; }

        [Value(1, HelpText = "ID of the secret", Required = true)]
        public string SecretId { get; set; }

        [Value(2, HelpText = "Name of the member", Required = true)]
        public string Member { get; set; }
    }

    class SecretVersionOptions
    {
        [Value(0, HelpText = "Project for the secret", Required = true)]
        public string ProjectId { get; set; }

        [Value(1, HelpText = "ID of the secret", Required = true)]
        public string SecretId { get; set; }

        [Value(2, HelpText = "Version of the secret", Required = true)]
        public string SecretVersion { get; set; }
    }

    [Verb("create", HelpText = "Create a secret")]
    class CreateSecretOptions : SecretOptions { }

    [Verb("delete", HelpText = "Delete secret")]
    class DeleteSecretOptions : SecretOptions { }

    [Verb("get", HelpText = "Get secret")]
    class GetSecretOptions : SecretOptions { }

    [Verb("list", HelpText = "List secrets")]
    class ListSecretsOptions : ProjectOptions { }

    [Verb("update", HelpText = "Update secret")]
    class UpdateSecretOptions : SecretOptions { }

    [Verb("access-version", HelpText = "Access the provided secret version")]
    class AccessSecretVersionOptions : SecretVersionOptions { }

    [Verb("add-version", HelpText = "Add a new version")]
    class AddSecretVersionOptions : SecretOptions { }

    [Verb("destroy-version", HelpText = "Destroy secret version")]
    class DestroySecretVersionOptions : SecretVersionOptions { }

    [Verb("disable-version", HelpText = "Disable secret version")]
    class DisableSecretVersionOptions : SecretVersionOptions { }

    [Verb("enable-version", HelpText = "Enable secret version")]
    class EnableSecretVersionOptions : SecretVersionOptions { }

    [Verb("get-version", HelpText = "Get secret version")]
    class GetSecretVersionOptions : SecretVersionOptions { }

    [Verb("iam-grant-access", HelpText = "Grant IAM access")]
    class IAMGrantAccessOptions : SecretIAMOptions { }

    [Verb("iam-revoke-access", HelpText = "Revoke IAM access")]
    class IAMRevokeAccessOptions : SecretIAMOptions { }

    [Verb("list-versions", HelpText = "List secret versions")]
    class ListSecretVersionsOptions : SecretOptions { }


    public class SecretManagerSample
    {
        // [START secretmanager_access_secret_version]
        /// <summary>
        /// Accesses a secret with provided version.
        /// </summary>
        /// <param name="projectId">ID of the project where the secret resides.</param>
        /// <param name="secretId">ID of the secret.</param>
        /// <param name="secretVersion">Version of the secret.</param>
        /// <example>
        /// With a specific version.
        /// <code>AccessSecretVersion("my-project", "my-secret", "5")</code>
        /// </example>
        /// <example>
        /// With an alias version.
        /// <code>AccessSecretVersion("my-project", "my-secret", "latest")</code>
        /// </example>
        public static void AccessSecretVersion(string projectId, string secretId, string secretVersion)
        {
            SecretManagerServiceClient client = SecretManagerServiceClient.Create();

            // Create the request.
            var request = new AccessSecretVersionRequest
            {
                SecretVersionName = new SecretVersionName(projectId, secretId, secretVersion),
            };

            // Access the secret and print the result.
            //
            // WARNING: Do not print secrets in production environments. This
            // snippet is for demonstration purposes only.
            var version = client.AccessSecretVersion(request);
            string payload = version.Payload.Data.ToStringUtf8();
            Console.WriteLine($"Payload: {payload}");
        }
        // [END secretmanager_access_secret_version]

        // [START secretmanager_add_secret_version]
        /// <summary>
        /// Add a secret version to the given secret. The given secret must
        /// already exist.
        /// </summary>
        /// <param name="projectId">ID of the project where the secret resides.</param>
        /// <param name="secretId">ID of the secret.</param>
        /// <example>
        /// Add a secret version.
        /// <code>AddSecretVersion("my-project", "my-secret")</code>
        /// </example>
        public static void AddSecretVersion(string projectId, string secretId)
        {
            SecretManagerServiceClient client = SecretManagerServiceClient.Create();

            // Create the secret payload.
            var payload = "my super secret data";

            // Create the request.
            var request = new AddSecretVersionRequest
            {
                ParentAsSecretName = new SecretName(projectId, secretId),
                Payload = new SecretPayload
                {
                    Data = ByteString.CopyFrom(payload, Encoding.UTF8),
                },
            };

            // Add the secret version.
            var version = client.AddSecretVersion(request);
            Console.WriteLine($"Added secret version {version.Name}");
        }
        // [END secretmanager_add_secret_version]

        // [START secretmanager_create_secret]
        /// <summary>
        /// Create a new secret in the given project with the given name.
        /// </summary>
        /// <param name="projectId">ID of the project in which to create the secret.</param>
        /// <param name="secretId">ID of the secret.</param>
        /// <example>
        /// Create a secret.
        /// <code>CreateSecret("my-project", "my-secret")</code>
        /// </example>
        public static void CreateSecret(string projectId, string secretId)
        {
            SecretManagerServiceClient client = SecretManagerServiceClient.Create();

            // Create the request.
            var request = new CreateSecretRequest
            {
                ParentAsProjectName = new ProjectName(projectId),
                SecretId = secretId,
                Secret = new Secret
                {
                    Replication = new Replication
                    {
                        Automatic = new Replication.Types.Automatic(),
                    },
                },
            };

            // Create the secret.
            var secret = client.CreateSecret(request);
            Console.WriteLine($"Created secret {secret.Name}");
        }
        // [END secretmanager_create_secret]

        // [START secretmanager_delete_secret]
        /// <summary>
        /// Delete an existing secret with the given name.
        /// </summary>
        /// <param name="projectId">ID of the project where the secret resides.</param>
        /// <param name="secretId">ID of the secret.</param>
        /// <example>
        /// Delete a secret.
        /// <code>DeleteSecret("my-project", "my-secret")</code>
        /// </example>
        public static void DeleteSecret(string projectId, string secretId)
        {
            SecretManagerServiceClient client = SecretManagerServiceClient.Create();

            // Create the request.
            var request = new DeleteSecretRequest
            {
                SecretName = new SecretName(projectId, secretId),
            };

            // Delete the secret.
            client.DeleteSecret(request);
            Console.WriteLine($"Deleted secret {secretId}");
        }
        // [END secretmanager_delete_secret]

        // [START secretmanager_destroy_secret_version]
        /// <summary>
        /// Destroy an existing secret version.
        /// </summary>
        /// <param name="projectId">ID of the project where the secret resides.</param>
        /// <param name="secretId">ID of the secret.</param>
        /// <param name="secretVersion">Version of the secret.</param>
        /// <example>
        /// Destroy a secret version.
        /// <code>DestroySecretVersion("my-project", "my-secret", "5")</code>
        /// </example>
        public static void DestroySecretVersion(string projectId, string secretId, string secretVersion)
        {
            SecretManagerServiceClient client = SecretManagerServiceClient.Create();

            // Create the request.
            var request = new DestroySecretVersionRequest
            {
                SecretVersionName = new SecretVersionName(projectId, secretId, secretVersion),
            };

            // Destroy the secret version.
            var version = client.DestroySecretVersion(request);
            Console.WriteLine($"Destroyed secret version {version.Name}");
        }
        // [END secretmanager_destroy_secret_version]

        // [START secretmanager_disable_secret_version]
        /// <summary>
        /// Disable an existing secret version.
        /// </summary>
        /// <param name="projectId">ID of the project where the secret resides.</param>
        /// <param name="secretId">ID of the secret.</param>
        /// <param name="secretVersion">Version of the secret.</param>
        /// <example>
        /// Disable an existing secret version.
        /// <code>DisableSecretVersion("my-project", "my-secret", "5")</code>
        /// </example>
        public static void DisableSecretVersion(string projectId, string secretId, string secretVersion)
        {
            SecretManagerServiceClient client = SecretManagerServiceClient.Create();

            // Create the request.
            var request = new DisableSecretVersionRequest
            {
                SecretVersionName = new SecretVersionName(projectId, secretId, secretVersion),
            };

            // Disable the secret version.
            var version = client.DisableSecretVersion(request);
            Console.WriteLine($"Disabled secret version {version.Name}");
        }
        // [END secretmanager_disable_secret_version]

        // [START secretmanager_enable_secret_version]
        /// <summary>
        /// Enable an existing secret version.
        /// </summary>
        /// <param name="projectId">ID of the project where the secret resides.</param>
        /// <param name="secretId">ID of the secret.</param>
        /// <param name="secretVersion">Version of the secret.</param>
        /// <example>1
        /// Enable an existing secret version.
        /// <code>EnableSecretVersion("my-project", "my-secret", "5")</code>
        /// </example>
        public static void EnableSecretVersion(string projectId, string secretId, string secretVersion)
        {
            SecretManagerServiceClient client = SecretManagerServiceClient.Create();

            // Create the request.
            var request = new EnableSecretVersionRequest
            {
                SecretVersionName = new SecretVersionName(projectId, secretId, secretVersion),
            };

            // Enable the secret version.
            var version = client.EnableSecretVersion(request);
            Console.WriteLine($"Enabled secret version {version.Name}");
        }
        // [END secretmanager_enable_secret_version]

        // [START secretmanager_get_secret_version]
        /// <summary>
        /// Get an existing secret version.
        /// </summary>
        /// <param name="projectId">ID of the project where the secret resides.</param>
        /// <param name="secretId">ID of the secret.</param>
        /// <param name="secretVersion">Version of the secret.</param>
        /// <example>
        /// Get an existing secret version.
        /// <code>GetSecretVersion("my-project", "my-secret", "5")</code>
        /// </example>
        /// <example>
        /// With an alias version.
        /// <code>GetSecretVersion("my-project", "my-secret", "latest")</code>
        /// </example>
        public static void GetSecretVersion(string projectId, string secretId, string secretVersion)
        {
            SecretManagerServiceClient client = SecretManagerServiceClient.Create();

            // Create the request.
            var request = new GetSecretVersionRequest
            {
                SecretVersionName = new SecretVersionName(projectId, secretId, secretVersion),
            };

            // Get the secret version.
            var version = client.GetSecretVersion(request);
            Console.WriteLine($"Secret version {version.Name}, state {version.State}");
        }
        // [END secretmanager_get_secret_version]

        // [START secretmanager_get_secret]
        /// <summary>
        /// Get an existing secret.
        /// </summary>
        /// <param name="projectId">ID of the project where the secret resides.</param>
        /// <param name="secretId">ID of the secret.</param>
        /// <example>
        /// Get an existing secret.
        /// <code>GetSecret("my-project", "my-secret")</code>
        /// </example>
        public static void GetSecret(string projectId, string secretId)
        {
            SecretManagerServiceClient client = SecretManagerServiceClient.Create();

            // Create the request.
            var request = new GetSecretRequest
            {
                SecretName = new SecretName(projectId, secretId),
            };

            // Get the secret.
            var secret = client.GetSecret(request);
            Console.WriteLine($"Secret {secret.Name}, replication {secret.Replication.ReplicationCase}");
        }
        // [END secretmanager_get_secret]

        // [START secretmanager_iam_grant_access]
        /// <summary>
        /// Grant a user or account access to the secret.
        /// </summary>
        /// <param name="projectId">ID of the project where the secret resides.</param>
        /// <param name="secretId">ID of the secret.</param>
        /// <param name="member">IAM member to grant with user: or serviceAccount: prefix</param>
        /// <example>
        /// Grant a user or account access to the secret.
        /// <code>IAMGrantAccess("my-project", "my-secret", "user:foo@example.com")</code>
        /// </example>
        public static void IAMGrantAccess(string projectId, string secretId, string member)
        {
            SecretManagerServiceClient client = SecretManagerServiceClient.Create();

            // Create the request to get the current IAM policy.
            var getRequest = new GetIamPolicyRequest
            {
                ResourceAsResourceName = new SecretName(projectId, secretId),
            };

            // Get the current IAM policy.
            var policy = client.GetIamPolicy(getRequest);

            // Add the user to the list of bindings.
            policy.AddRoleMember("roles/secretmanager.secretAccessor", member);

            // Create the request to update the IAM policy.
            var setRequest = new SetIamPolicyRequest
            {
                ResourceAsResourceName = new SecretName(projectId, secretId),
                Policy = policy,
            };

            // Save the updated IAM policy.
            client.SetIamPolicy(setRequest);

            Console.WriteLine($"Updated IAM policy for {secretId}");
        }
        // [END secretmanager_iam_grant_access]

        // [START secretmanager_iam_revoke_access]
        /// <summary>
        /// Revoke a user or account access to the secret.
        /// </summary>
        /// <param name="projectId">ID of the project where the secret resides.</param>
        /// <param name="secretId">ID of the secret.</param>
        /// <param name="member">IAM member to revoke with user: or serviceAccount: prefix</param>
        /// <example>
        /// Revoke a user or account access to the secret.
        /// <code>IAMRevokeAccess("my-project", "my-secret", "user:foo@example.com")</code>
        /// </example>
        public static void IAMRevokeAccess(string projectId, string secretId, string member)
        {
            SecretManagerServiceClient client = SecretManagerServiceClient.Create();

            // Create the request to get the current IAM policy.
            var getRequest = new GetIamPolicyRequest
            {
                ResourceAsResourceName = new SecretName(projectId, secretId),
            };

            // Get the current IAM policy.
            var policy = client.GetIamPolicy(getRequest);

            // Remove the user to the list of bindings.
            policy.RemoveRoleMember("roles/secretmanager.secretAccessor", member);

            // Create the request to update the IAM policy.
            var setRequest = new SetIamPolicyRequest
            {
                ResourceAsResourceName = new SecretName(projectId, secretId),
                Policy = policy,
            };

            // Save the updated IAM policy.
            client.SetIamPolicy(setRequest);

            Console.WriteLine($"Updated IAM policy for {secretId}");
        }
        // [END secretmanager_iam_revoke_access]

        // [START secretmanager_list_secret_versions]
        /// <summary>
        /// List all secret versions for a secret.
        /// </summary>
        /// <param name="projectId">ID of the project where the secret resides.</param>
        /// <param name="secretId">ID of the secret.</param>
        /// <example>
        /// List all secret versions.
        /// <code>ListSecretVersions("my-project", "my-secret")</code>
        /// </example>
        public static void ListSecretVersions(string projectId, string secretId)
        {
            SecretManagerServiceClient client = SecretManagerServiceClient.Create();

            // Create the request.
            var request = new ListSecretVersionsRequest
            {
                ParentAsSecretName = new SecretName(projectId, secretId),
            };

            // List all versions and their state.
            foreach (var version in client.ListSecretVersions(request))
            {
                Console.WriteLine($"Secret version {version.Name}, {version.State}");
            }
        }
        // [END secretmanager_list_secret_versions]

        // [START secretmanager_list_secrets]
        /// <summary>
        /// List all secret for a project
        /// </summary>
        /// <param name="projectId">ID of the project where secrets reside.</param>
        /// <example>
        /// List all secrets.
        /// <code>ListSecrets("my-project")</code>
        /// </example>
        public static void ListSecrets(string projectId)
        {
            SecretManagerServiceClient client = SecretManagerServiceClient.Create();

            // Create the request.
            var request = new ListSecretsRequest
            {
                ParentAsProjectName = new ProjectName(projectId),
            };

            // List all secrets in the project.
            foreach (var secret in client.ListSecrets(request))
            {
                Console.WriteLine($"Secret {secret.Name}");
            }
        }
        // [END secretmanager_list_secrets]

        // [START secretmanager_update_secret]
        /// <summary>
        /// Update an existing secret.
        /// </summary>
        /// <param name="projectId">ID of the project where the secret resides.</param>
        /// <param name="secretId">ID of the secret.</param>
        /// <example>
        /// Update an existing secret.
        /// <code>UpdateSecret("my-project", "my-secret")</code>
        /// </example>
        public static void UpdateSecret(string projectId, string secretId)
        {
            SecretManagerServiceClient client = SecretManagerServiceClient.Create();

            // Create the secret to update.
            var secret = new Secret
            {
                SecretName = new SecretName(projectId, secretId),
            };
            secret.Labels["secretmanager"] = "rocks";

            // Create the request.
            var request = new UpdateSecretRequest
            {
                Secret = secret,
                UpdateMask = FieldMask.FromString("labels"),
            };

            // Update the secret
            var updatedSecret = client.UpdateSecret(request);
            Console.WriteLine($"Updated secret {updatedSecret.Name}");
        }
        // [END secretmanager_update_secret]


        public static void Main(string[] args)
        {
            Parser.Default.ParseArguments(args,
                    typeof(AccessSecretVersionOptions),
                    typeof(AddSecretVersionOptions),
                    typeof(CreateSecretOptions),
                    typeof(DestroySecretVersionOptions),
                    typeof(DeleteSecretOptions),
                    typeof(EnableSecretVersionOptions),
                    typeof(DisableSecretVersionOptions),
                    typeof(GetSecretVersionOptions),
                    typeof(GetSecretOptions),
                    typeof(IAMGrantAccessOptions),
                    typeof(IAMRevokeAccessOptions),
                    typeof(ListSecretVersionsOptions),
                    typeof(ListSecretsOptions),
                    typeof(UpdateSecretOptions))
                .WithParsed<AccessSecretVersionOptions>(opts => AccessSecretVersion(opts.ProjectId, opts.SecretId, opts.SecretVersion))
                .WithParsed<AddSecretVersionOptions>(opts => AddSecretVersion(opts.ProjectId, opts.SecretId))
                .WithParsed<CreateSecretOptions>(opts => CreateSecret(opts.ProjectId, opts.SecretId))
                .WithParsed<DeleteSecretOptions>(opts => DeleteSecret(opts.ProjectId, opts.SecretId))
                .WithParsed<DestroySecretVersionOptions>(opts => DestroySecretVersion(opts.ProjectId, opts.SecretId, opts.SecretVersion))
                .WithParsed<DisableSecretVersionOptions>(opts => DisableSecretVersion(opts.ProjectId, opts.SecretId, opts.SecretVersion))
                .WithParsed<EnableSecretVersionOptions>(opts => EnableSecretVersion(opts.ProjectId, opts.SecretId, opts.SecretVersion))
                .WithParsed<GetSecretVersionOptions>(opts => GetSecretVersion(opts.ProjectId, opts.SecretId, opts.SecretVersion))
                .WithParsed<GetSecretOptions>(opts => GetSecret(opts.ProjectId, opts.SecretId))
                .WithParsed<IAMGrantAccessOptions>(opts => IAMGrantAccess(opts.ProjectId, opts.SecretId, opts.Member))
                .WithParsed<IAMRevokeAccessOptions>(opts => IAMRevokeAccess(opts.ProjectId, opts.SecretId, opts.Member))
                .WithParsed<ListSecretVersionsOptions>(opts => ListSecretVersions(opts.ProjectId, opts.SecretId))
                .WithParsed<ListSecretsOptions>(opts => ListSecrets(opts.ProjectId))
                .WithParsed<UpdateSecretOptions>(opts => UpdateSecret(opts.ProjectId, opts.SecretId));
        }
    }
}
