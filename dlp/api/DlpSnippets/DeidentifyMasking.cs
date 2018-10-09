// [START dlp_deidentify_masking]
using System;
using System.Collections.Generic;
using Google.Api.Gax.ResourceNames;
using Google.Cloud.Dlp.V2;

partial class DlpSnippets
{
    public string DeidentiyMasking(string projectId = "YOUR-PROJECT-ID")
    {
        var transformation = new InfoTypeTransformations.Types.InfoTypeTransformation
        {
            PrimitiveTransformation = new PrimitiveTransformation
            {
                CharacterMaskConfig = new CharacterMaskConfig
                {
                    MaskingCharacter = "*",
                    NumberToMask = 5,
                    ReverseOrder = false,
                }
            }
        };
        var request = new DeidentifyContentRequest
        {
            ParentAsProjectName = new ProjectName(projectId),
            InspectConfig = new InspectConfig
            {
                InfoTypes = 
                {
                    new InfoType { Name = "US_SOCIAL_SECURITY_NUMBER" }
                }
            },
            DeidentifyConfig = new DeidentifyConfig
            {
                InfoTypeTransformations = new InfoTypeTransformations
                {
                    Transformations = { transformation }
                }
            },
            Item = new ContentItem { Value = "'My SSN is 372819127.'" }
        };

        DlpServiceClient dlp = DlpServiceClient.Create();
        var response = dlp.DeidentifyContent(request);

        Console.WriteLine($"Deidentified content: {response.Item.Value}");
        return response.Item.Value;
    }
}

// [END dlp_deidentify_masking]