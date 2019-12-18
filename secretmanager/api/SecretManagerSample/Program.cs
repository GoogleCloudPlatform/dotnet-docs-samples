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
        public string name { get; set; }
    }

    class SecretVersionOptions {
        [Value(0, HelpText = "Full resource name of the version", Required = true)]
        public string name { get; set; }
    }

    [Verb("create", HelpText = "Create a secret")]
    class CreateSecretOptions
    {
        [Value(0, HelpText = "Full resource name of project", Required = true)]
        public string projectId { get; set; }

        [Value(1, HelpText = "Name of the secret", Required = true)]
        public string id { get; set; }
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
        public static void AccessSecretVersion(string name)
        {
            // string name = "projects/my-project/secrets/my-secret/versions/5";
            // string name = "projects/my-project/secrets/my-secret/versions/latest";

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
        public static void AddSecretVersion(string parent)
        {
            // string parent = "projects/my-project/secrets/my-secret";

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
        public static void CreateSecret(string parent, string id)
        {
            // string parent = "projects/my-project";
            // string id = "my-secret";

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
        public static void DeleteSecret(string name)
        {
            // string name = "projects/my-project/secrets/my-secret";

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
        public static void DestroySecretVersion(string name)
        {
            // string name = "projects/my-project/secrets/my-secret/versions/5";

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
        public static void DisableSecretVersion(string name)
        {
            // string name = "projects/my-project/secrets/my-secret/versions/5";

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
        public static void EnableSecretVersion(string name)
        {
            // string name = "projects/my-project/secrets/my-secret/versions/5";

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
        public static void GetSecretVersion(string name)
        {
            // string name = "projects/my-project/secrets/my-secret/versions/5";

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
        public static void GetSecret(string name)
        {
            // string name = "projects/my-project/secrets/my-secret";

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
        public static void ListSecretVersions(string parent)
        {
            // string parent = "projects/my-project/secrets/my-secret";

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
        public static void ListSecrets(string parent)
        {
            // string parent = "projects/my-project";

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
        public static void UpdateSecret(string name)
        {
            // string name = "projects/my-project/secrets/my-secret";

            SecretManagerServiceClient client = SecretManagerServiceClient.Create();

            // Create the request.
            var request = new UpdateSecretRequest
            {
                Secret = new Secret
                {
                  Name = name,
                  // Labels = new Dictionary<string, string>
                  // {
                  //   {"secretmanager", "rocks"},
                  // },
                },
                UpdateMask = FieldMask.FromString("labels"),
            };

            // Update the secret
            var secret = client.UpdateSecret(request);
            Console.WriteLine($"Updated secret {secret.Name}");
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
            .WithParsed<AccessSecretVersionOptions>(opts => AccessSecretVersion(opts.name))
            .WithParsed<AddSecretVersionOptions>(opts => AddSecretVersion(opts.name))
            .WithParsed<CreateSecretOptions>(opts => CreateSecret(opts.projectId, opts.id))
            .WithParsed<DeleteSecretOptions>(opts => DeleteSecret(opts.name))
            .WithParsed<DestroySecretVersionOptions>(opts => DestroySecretVersion(opts.name))
            .WithParsed<DisableSecretVersionOptions>(opts => DisableSecretVersion(opts.name))
            .WithParsed<EnableSecretVersionOptions>(opts => EnableSecretVersion(opts.name))
            .WithParsed<GetSecretVersionOptions>(opts => GetSecretVersion(opts.name))
            .WithParsed<GetSecretOptions>(opts => GetSecret(opts.name))
            .WithParsed<ListSecretVersionsOptions>(opts => ListSecretVersions(opts.name))
            .WithParsed<ListSecretsOptions>(opts => ListSecrets(opts.name))
            .WithParsed<UpdateSecretOptions>(opts => UpdateSecret(opts.name));
        }
    }
}
