/*
 * Copyright (c) 2019 Google Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not
 * use this file except in compliance with the License. You may obtain a copy of
 * the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
 * License for the specific language governing permissions and limitations under
 * the License.
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using CommandLine;
using Google.Cloud.Iam.V1;
using Google.Cloud.Secrets.V1Beta1;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace GoogleCloudSamples
{

    class SecretOptions {
        [Value(0, HelpText = "Full resource name of the secret", Required = true)]
        public string Name { get; set; }
    }

    class SecretVersionOptions {
        [Value(0, HelpText = "Full resource name of the version", Required = true)]
        public string Name { get; set; }
    }

    [Verb("create", HelpText = "Create a secret")]
    class CreateSecretOptions
    {
        [Value(0, HelpText = "Full resource name of project", Required = true)]
        public string ProjectId { get; set; }

        [Value(1, HelpText = "Name of the secret", Required = true)]
        public string Id { get; set; }
    }

    [Verb("delete", HelpText = "Delete secret")]
    class DeleteSecretOptions : SecretOptions { }

    [Verb("get", HelpText = "Get secret")]
    class GetSecretOptions : SecretOptions { }

    [Verb("list", HelpText = "List secrets")]
    class ListSecretsOptions : SecretOptions { }

    [Verb("update", HelpText = "Update secret")]
    class UpdateSecretOptions : SecretOptions { }

    [Verb("access-version", HelpText = "Access the provided secret version")]
    class AccessSecretVersionOptions : SecretVersionOptions { }

    [Verb("add-version", HelpText = "Add a new version")]
    class AddSecretVersionOptions : SecretVersionOptions { }

    [Verb("destroy-version", HelpText = "Destroy secret version")]
    class DestroySecretVersionOptions : SecretVersionOptions { }

    [Verb("disable-version", HelpText = "Disable secret version")]
    class DisableSecretVersionOptions : SecretVersionOptions { }

    [Verb("enable-version", HelpText = "Enable secret version")]
    class EnableSecretVersionOptions : SecretVersionOptions { }

    [Verb("get-version", HelpText = "Get secret version")]
    class GetSecretVersionOptions : SecretVersionOptions { }

    [Verb("list-versions", HelpText = "List secret versions")]
    class ListSecretVersionsOptions : SecretVersionOptions { }


    public class SecretManagerSample
    {
        // [START secretmanager_access_secret_version]
        /// <summary>
        /// Accesses a secret with provided version.
        /// </summary>
        /// <param name="name">Secret version name to access.</param>
        /// <example>
        /// With a specific version.
        /// <code>AccessSecretVersion("projects/my-project/secrets/my-secret/versions/5")</code>
        /// </example>
        /// <example>
        /// With an alias version.
        /// <code>AccessSecretVersion("projects/my-project/secrets/my-secret/versions/latest")</code>
        /// </example>
        public static void AccessSecretVersion(string name)
        {
            SecretManagerServiceClient client = SecretManagerServiceClient.Create();

            // Create the request.
            var request = new AccessSecretVersionRequest
            {
                Name = name,
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
        /// <param name="parent">Secret in which to add the version.</param>
        /// <example>
        /// Add a secret version.
        /// <code>AddSecretVersion("projects/my-project/secrets/my-secret")</code>
        /// </example>
        public static void AddSecretVersion(string parent)
        {
            SecretManagerServiceClient client = SecretManagerServiceClient.Create();

            // Create the secret payload.
            var payload = "my super secret data";

            // Create the request.
            var request = new AddSecretVersionRequest
            {
                Parent = parent,
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
        /// <param name="parent">Project in which to create the secret.</param>
        /// <param name="id">ID to use for the secret.</param>
        /// <example>
        /// Create a secret.
        /// <code>CreateSecret("projects/my-project", "my-secret")</code>
        /// </example>
        public static void CreateSecret(string parent, string id)
        {
            SecretManagerServiceClient client = SecretManagerServiceClient.Create();

            // Create the request.
            var request = new CreateSecretRequest
            {
                Parent = parent,
                SecretId = id,
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
        /// <param name="name">Name of the secret to delete.</param>
        /// <example>
        /// Delete a secret.
        /// <code>DeleteSecret("projects/my-project/secrets/my-secret")</code>
        /// </example>
        public static void DeleteSecret(string name)
        {
            SecretManagerServiceClient client = SecretManagerServiceClient.Create();

            // Create the request.
            var request = new DeleteSecretRequest
            {
                Name = name,
            };

            // Delete the secret.
            client.DeleteSecret(request);
            Console.WriteLine($"Deleted secret {name}");
        }
        // [END secretmanager_delete_secret]

        // [START secretmanager_destroy_secret_version]
        /// <summary>
        /// Destroy an existing secret version.
        /// </summary>
        /// <param name="name">Name of the secret version to destroy.</param>
        /// <example>
        /// Destroy a secret version.
        /// <code>DestroySecretVersion("projects/my-project/secrets/my-secret/versions/5")</code>
        /// </example>
        public static void DestroySecretVersion(string name)
        {
            SecretManagerServiceClient client = SecretManagerServiceClient.Create();

            // Create the request.
            var request = new DestroySecretVersionRequest
            {
                Name = name,
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
        /// <param name="name">Name of the secret to disable.</param>
        /// <example>
        /// Disable an existing secret version.
        /// <code>DisableSecretVersion("projects/my-project/secrets/my-secret/versions/5")</code>
        /// </example>
        public static void DisableSecretVersion(string name)
        {
            SecretManagerServiceClient client = SecretManagerServiceClient.Create();

            // Create the request.
            var request = new DisableSecretVersionRequest
            {
                Name = name,
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
        /// <param name="name">Name of the secret to enable.</param>
        /// <example>
        /// Enable an existing secret version.
        /// <code>EnableSecretVersion("projects/my-project/secrets/my-secret/versions/5")</code>
        /// </example>
        public static void EnableSecretVersion(string name)
        {
            SecretManagerServiceClient client = SecretManagerServiceClient.Create();

            // Create the request.
            var request = new EnableSecretVersionRequest
            {
                Name = name,
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
        /// <param name="name">Name of the secret version to get.</param>
        /// <example>
        /// Get an existing secret version.
        /// <code>GetSecretVersion("projects/my-project/secrets/my-secret/versions/5")</code>
        /// </example>
        /// <example>
        /// With an alias version.
        /// <code>GetSecretVersion("projects/my-project/secrets/my-secret/versions/latest")</code>
        /// </example>
        public static void GetSecretVersion(string name)
        {
            SecretManagerServiceClient client = SecretManagerServiceClient.Create();

            // Create the request.
            var request = new GetSecretVersionRequest
            {
                Name = name,
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
        /// <param name="name">Name of the secret to get.</param>
        /// <example>
        /// Get an existing secret.
        /// <code>GetSecret("projects/my-project/secrets/my-secret")</code>
        /// </example>
        public static void GetSecret(string name)
        {
            SecretManagerServiceClient client = SecretManagerServiceClient.Create();

            // Create the request.
            var request = new GetSecretRequest
            {
                Name = name,
            };

            // Get the secret.
            var secret = client.GetSecret(request);
            Console.WriteLine($"Secret {secret.Name}, replication {secret.Replication.ReplicationCase}");
        }
        // [END secretmanager_get_secret]

        // [START secretmanager_list_secret_versions]
        /// <summary>
        /// List all secret versions for a secret.
        /// </summary>
        /// <param name="parent">Parent secret for which to list secret versions.</param>
        /// <example>
        /// List all secret versions.
        /// <code>ListSecretVersions("projects/my-project/secrets/my-secret")</code>
        /// </example>
        public static void ListSecretVersions(string parent)
        {
            SecretManagerServiceClient client = SecretManagerServiceClient.Create();

            // Create the request.
            var request = new ListSecretVersionsRequest
            {
                Parent = parent,
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
        /// <param name="parent">Parent project for which to secrets.</param>
        /// <example>
        /// List all secrets.
        /// <code>ListSecrets("projects/my-project")</code>
        /// </example>
        public static void ListSecrets(string parent)
        {
            SecretManagerServiceClient client = SecretManagerServiceClient.Create();

            // Create the request.
            var request = new ListSecretsRequest
            {
                Parent = parent,
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
        /// <param name="name">Name of the secret to update.</param>
        /// <example>
        /// Update an existing secret.
        /// <code>UpdateSecret("projects/my-project/secrets/my-secret")</code>
        /// </example>
        public static void UpdateSecret(string name)
        {
            SecretManagerServiceClient client = SecretManagerServiceClient.Create();

            // Create the secret to update.
            var secret = new Secret
            {
                Name = name,
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
                typeof(ListSecretVersionsOptions),
                typeof(ListSecretsOptions),
                typeof(UpdateSecretOptions))
            .WithParsed<AccessSecretVersionOptions>(opts => AccessSecretVersion(opts.Name))
            .WithParsed<AddSecretVersionOptions>(opts => AddSecretVersion(opts.Name))
            .WithParsed<CreateSecretOptions>(opts => CreateSecret(opts.ProjectId, opts.Id))
            .WithParsed<DeleteSecretOptions>(opts => DeleteSecret(opts.Name))
            .WithParsed<DestroySecretVersionOptions>(opts => DestroySecretVersion(opts.Name))
            .WithParsed<DisableSecretVersionOptions>(opts => DisableSecretVersion(opts.Name))
            .WithParsed<EnableSecretVersionOptions>(opts => EnableSecretVersion(opts.Name))
            .WithParsed<GetSecretVersionOptions>(opts => GetSecretVersion(opts.Name))
            .WithParsed<GetSecretOptions>(opts => GetSecret(opts.Name))
            .WithParsed<ListSecretVersionsOptions>(opts => ListSecretVersions(opts.Name))
            .WithParsed<ListSecretsOptions>(opts => ListSecrets(opts.Name))
            .WithParsed<UpdateSecretOptions>(opts => UpdateSecret(opts.Name));
        }
    }
}
