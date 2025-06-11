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
    private readonly List<TemplateName> _dlpTemplatesToCleanup = new List<TemplateName>();

    public ModelArmorFixture()
    {
        ProjectId = GetRequiredEnvVar(EnvProjectId);
        LocationId = Environment.GetEnvironmentVariable(EnvLocation) ?? "us-central1";

        // Create the Model Armor client.
        Client = new ModelArmorClientBuilder
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
        TemplateName templateName = CreateTemplateName();
        RegisterDlpTemplateForCleanup(templateName);

        string templateId = templateName.TemplateId;

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

        return response.Name;
    }

    // Creates a DLP Deidentify Template and returns its name.
    public string CreateDeidentifyTemplate(string displayName = "Test Deidentify Template")
    {
        var parent = new LocationName(ProjectId, LocationId).ToString();
        TemplateName templateName = CreateTemplateName();
        RegisterDlpTemplateForCleanup(templateName);

        string templateId = templateName.TemplateId;

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
                if (dlpTemplateName.ToString().Contains("inspectTemplates/"))
                {
                    DlpClient.DeleteInspectTemplate(
                        new DeleteInspectTemplateRequest { Name = dlpTemplateName.ToString() }
                    );
                }
                else if (dlpTemplateName.ToString().Contains("deidentifyTemplates/"))
                {
                    DlpClient.DeleteDeidentifyTemplate(
                        new DeleteDeidentifyTemplateRequest { Name = dlpTemplateName.ToString() }
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

    public void RegisterDlpTemplateForCleanup(TemplateName templateName)
    {
        if (templateName != null && !string.IsNullOrEmpty(templateName.ToString()))
        {
            _dlpTemplatesToCleanup.Add(templateName);
        }
    }

    public TemplateName CreateTemplateName(string prefix = "test-dotnet")
    {
        string templateId = $"{prefix}-{GenerateUniqueId()}";
        return new TemplateName(ProjectId, LocationId, templateId);
    }

    // Base method to create a template with basic RAI settings
    public Template ConfigureBaseTemplate()
    {
        RaiFilterSettings raiFilterSettings = new RaiFilterSettings();
        raiFilterSettings.RaiFilters.Add(
            new RaiFilterSettings.Types.RaiFilter
            {
                FilterType = RaiFilterType.Dangerous,
                ConfidenceLevel = DetectionConfidenceLevel.High,
            }
        );
        raiFilterSettings.RaiFilters.Add(
            new RaiFilterSettings.Types.RaiFilter
            {
                FilterType = RaiFilterType.HateSpeech,
                ConfidenceLevel = DetectionConfidenceLevel.MediumAndAbove,
            }
        );
        raiFilterSettings.RaiFilters.Add(
            new RaiFilterSettings.Types.RaiFilter
            {
                FilterType = RaiFilterType.SexuallyExplicit,
                ConfidenceLevel = DetectionConfidenceLevel.MediumAndAbove,
            }
        );
        raiFilterSettings.RaiFilters.Add(
            new RaiFilterSettings.Types.RaiFilter
            {
                FilterType = RaiFilterType.Harassment,
                ConfidenceLevel = DetectionConfidenceLevel.MediumAndAbove,
            }
        );

        // Create the filter config with RAI settings
        FilterConfig modelArmorFilter = new FilterConfig { RaiSettings = raiFilterSettings };

        // Create the template
        Template template = new Template { FilterConfig = modelArmorFilter };
        return template;
    }

    // Create a template with Basic SDP configuration
    public Template ConfigureBasicSdpTemplate()
    {
        // First create a base template
        Template template = ConfigureBaseTemplate();

        // Add Basic SDP configuration
        SdpBasicConfig basicSdpConfig = new SdpBasicConfig
        {
            FilterEnforcement = SdpBasicConfig.Types.SdpBasicConfigEnforcement.Enabled,
        };

        SdpFilterSettings sdpSettings = new SdpFilterSettings { BasicConfig = basicSdpConfig };
        template.FilterConfig.SdpSettings = sdpSettings;

        return template;
    }

    public Template ConfigureAdvancedSdpTemplate()
    {
        // First create a base template
        Template template = ConfigureBaseTemplate();

        string inspectTemplateName = CreateInspectTemplate();
        string deidentifyTemplateName = CreateDeidentifyTemplate();

        // Add Advanced SDP configuration
        SdpAdvancedConfig advancedSdpConfig = new SdpAdvancedConfig
        {
            InspectTemplate = inspectTemplateName,
            DeidentifyTemplate = deidentifyTemplateName,
        };

        SdpFilterSettings sdpSettings = new SdpFilterSettings
        {
            AdvancedConfig = advancedSdpConfig,
        };

        template.FilterConfig.SdpSettings = sdpSettings;

        return template;
    }

    public Template ConfigureTemplateWithMaliciousUri()
    {
        Template template = ConfigureBaseTemplate();

        template.FilterConfig.MaliciousUriFilterSettings = new MaliciousUriFilterSettings
        {
            FilterEnforcement = MaliciousUriFilterSettings
                .Types
                .MaliciousUriFilterEnforcement
                .Enabled,
        };

        return template;
    }

    public Template ConfigureTemplateWithPiAndJailbreak()
    {
        Template template = ConfigureBaseTemplate();

        template.FilterConfig.PiAndJailbreakFilterSettings = new PiAndJailbreakFilterSettings
        {
            ConfidenceLevel = DetectionConfidenceLevel.MediumAndAbove,
            FilterEnforcement = PiAndJailbreakFilterSettings
                .Types
                .PiAndJailbreakFilterEnforcement
                .Enabled,
        };
        return template;
    }

    // Create a template on GCP and register it for cleanup
    public Template CreateTemplate(Template templateConfig, string templateId = null)
    {
        // Generate a unique template ID if none provided
        templateId ??= $"test-{GenerateUniqueId()}";

        // Create the parent resource name
        LocationName parent = LocationName.FromProjectLocation(ProjectId, LocationId);

        // Create the template
        Template createdTemplate = Client.CreateTemplate(
            new CreateTemplateRequest
            {
                ParentAsLocationName = parent,
                Template = templateConfig,
                TemplateId = templateId,
            }
        );

        // Register the template for cleanup
        RegisterTemplateForCleanup(TemplateName.Parse(createdTemplate.Name));

        return createdTemplate;
    }

    // Create a base template on GCP
    public Template CreateBaseTemplate(string templateId = null)
    {
        Template templateConfig = ConfigureBaseTemplate();
        return CreateTemplate(templateConfig, templateId);
    }

    // Create a template with Basic SDP on GCP
    public Template CreateBasicSdpTemplate(string templateId = null)
    {
        Template templateConfig = ConfigureBasicSdpTemplate();
        return CreateTemplate(templateConfig, templateId);
    }

    // Create a template with Advanced SDP on GCP
    public Template CreateAdvancedSdpTemplate(string templateId = null)
    {
        Template templateConfig = ConfigureAdvancedSdpTemplate();
        return CreateTemplate(templateConfig, templateId);
    }

    public Template CreateTemplateWithMaliciousUri(string templateId = null)
    {
        Template templateConfig = ConfigureTemplateWithMaliciousUri();
        return CreateTemplate(templateConfig, templateId);
    }

    public Template CreateTemplateWithPiAndJailbreak(string templateId = null)
    {
        Template templateConfig = ConfigureTemplateWithPiAndJailbreak();
        return CreateTemplate(templateConfig, templateId);
    }
}
