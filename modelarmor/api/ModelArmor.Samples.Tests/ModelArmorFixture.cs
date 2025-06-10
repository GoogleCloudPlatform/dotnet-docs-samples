/*
 * Copyright 2025 Google LLC
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

using System;
using System.Collections.Generic;
using Google.Api.Gax.ResourceNames;
using Google.Cloud.Dlp.V2;
using Google.Cloud.ModelArmor.V1;
using Xunit;

[CollectionDefinition(nameof(ModelArmorFixture))]
public class ModelArmorFixture : IDisposable, ICollectionFixture<ModelArmorFixture>
{
    // Environment variable names.
    private const string EnvProjectId = "GOOGLE_PROJECT_ID";
    private const string EnvLocation = "GOOGLE_CLOUD_LOCATION";

    public ModelArmorClient Client { get; }
    public DlpServiceClient DlpClient { get; }
    public string ProjectId { get; }
    public string LocationId { get; }
    private readonly List<TemplateName> _maTemplatesToCleanup = new List<TemplateName>();
    private readonly List<string> _dlpTemplatesToCleanup = new List<string>();

    public ModelArmorFixture()
    {
        ProjectId = GetRequiredEnvVar(EnvProjectId);
        LocationId = Environment.GetEnvironmentVariable(EnvLocation) ?? "us-central1";

        // Create the Model Armor client.
        ModelArmorClient Client = new ModelArmorClientBuilder
        {
            Endpoint = $"modelarmor.{LocationId}.rep.googleapis.com",
        }.Build();

        // Create the DLP client.
        DlpClient = new DlpServiceClientBuilder { Endpoint = $"dlp.googleapis.com" }.Build();
    }

    private string GetRequiredEnvVar(string name)
    {
        var value = Environment.GetEnvironmentVariable(name);
        if (string.IsNullOrEmpty(value))
        {
            throw new Exception($"Missing {name} environment variable");
        }
        return value;
    }

    public string GenerateUniqueId()
    {
        return Guid.NewGuid().ToString("N").Substring(0, 8);
    }

    // Creates a DLP Inspect Template and returns its name.
    public string CreateInspectTemplate(string displayName = "Test Inspect Template")
    {
        var parent = new LocationName(ProjectId, LocationId).ToString();
        var templateId = $"inspect-{GenerateUniqueId()}";
        var request = new CreateInspectTemplateRequest
        {
            Parent = parent,
            InspectTemplate = new InspectTemplate
            {
                DisplayName = displayName,
                InspectConfig = new InspectConfig
                {
                    InfoTypes =
                    {
                        new InfoType { Name = "PERSON_NAME" },
                        new InfoType { Name = "EMAIL_ADDRESS" },
                        new InfoType { Name = "US_INDIVIDUAL_TAXPAYER_IDENTIFICATION_NUMBER" },
                    },
                },
            },
            TemplateId = templateId,
        };
        var response = DlpClient.CreateInspectTemplate(request);
        RegisterDlpTemplateForCleanup(response.Name);
        return response.Name;
    }

    // Creates a DLP Deidentify Template and returns its name.
    public string CreateDeidentifyTemplate(string displayName = "Test Deidentify Template")
    {
        var parent = new LocationName(ProjectId, LocationId).ToString();
        var templateId = $"deidentify-{GenerateUniqueId()}";
        var request = new CreateDeidentifyTemplateRequest
        {
            Parent = parent,
            DeidentifyTemplate = new DeidentifyTemplate
            {
                DisplayName = displayName,
                DeidentifyConfig = new DeidentifyConfig
                {
                    InfoTypeTransformations = new InfoTypeTransformations
                    {
                        Transformations =
                        {
                            new InfoTypeTransformations.Types.InfoTypeTransformation
                            {
                                PrimitiveTransformation = new PrimitiveTransformation
                                {
                                    ReplaceConfig = new ReplaceValueConfig
                                    {
                                        NewValue = new Value { StringValue = "[REDACTED]" },
                                    },
                                },
                            },
                        },
                    },
                },
            },
            TemplateId = templateId,
        };
        var response = DlpClient.CreateDeidentifyTemplate(request);
        RegisterDlpTemplateForCleanup(response.Name);
        return response.Name;
    }

    public void Dispose()
    {
        // Clean up resources after tests.
        foreach (var resourceName in _maTemplatesToCleanup)
        {
            try
            {
                Client.DeleteTemplate(new DeleteTemplateRequest { TemplateName = resourceName });
            }
            catch (Exception)
            {
                // Ignore errors during cleanup.
            }
        }

        // Clean up DLP templates.
        foreach (var dlpTemplateName in _dlpTemplatesToCleanup)
        {
            try
            {
                if (dlpTemplateName.Contains("inspectTemplates/"))
                {
                    DlpClient.DeleteInspectTemplate(
                        new DeleteInspectTemplateRequest { Name = dlpTemplateName }
                    );
                }
                else if (dlpTemplateName.Contains("deidentifyTemplates/"))
                {
                    DlpClient.DeleteDeidentifyTemplate(
                        new DeleteDeidentifyTemplateRequest { Name = dlpTemplateName }
                    );
                }
            }
            catch (Exception)
            {
                // Ignore errors during cleanup.
            }
        }
    }

    public void RegisterTemplateForCleanup(TemplateName templateName)
    {
        if (templateName != null && !string.IsNullOrEmpty(templateName.ToString()))
        {
            _maTemplatesToCleanup.Add(templateName);
        }
    }

    public void RegisterDlpTemplateForCleanup(string templateName)
    {
        if (!string.IsNullOrEmpty(templateName))
        {
            _dlpTemplatesToCleanup.Add(templateName);
        }
    }

    public TemplateName CreateTemplateName(string prefix = "test-dotnet")
    {
        string templateId = $"{prefix}-{GenerateUniqueId()}";
        return new TemplateName(ProjectId, LocationId, templateId);
    }
}
