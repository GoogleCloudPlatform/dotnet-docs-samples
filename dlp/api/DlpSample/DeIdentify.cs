using Google.Cloud.Dlp.V2;
using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.IO;

namespace GoogleCloudSamples
{
    public class DeIdentify : DlpSampleBase
    {
        // [START dlp_deidentify_masking]
        public static object DeidMask(
            string projectId,
            string value,
            string infoTypes,
            string maskingCharacter,
            int numberToMask,
            bool reverseOrder)
        {
            DeidentifyContentRequest request = new DeidentifyContentRequest
            {
                Parent = $"projects/{projectId}",
                DeidentifyConfig = new DeidentifyConfig
                {
                    InfoTypeTransformations = new InfoTypeTransformations()
                },
                Item = new ContentItem
                {
                    Value = value
                }
            };
            request.DeidentifyConfig.InfoTypeTransformations.Transformations.AddRange(
                new List<InfoTypeTransformations.Types.InfoTypeTransformation> {
                    new InfoTypeTransformations.Types.InfoTypeTransformation
                    {
                        PrimitiveTransformation = new PrimitiveTransformation
                        {
                            CharacterMaskConfig = new CharacterMaskConfig
                            {
                                MaskingCharacter = maskingCharacter,
                                NumberToMask = numberToMask,
                                ReverseOrder = reverseOrder
                            }
                        }
                    }
                });
            DlpServiceClient dlp = DlpServiceClient.Create();
            var response = dlp.DeidentifyContent(request);
            Console.WriteLine($"Deidentified content: {response.Item.Value}");
            return 0;
        }
        // [END dlp_deidentify_masking]

        // [START dlp_deidentify_fpe]
        public static object DeidFpe(
            string projectId,
            string value,
            string keyName,
            string wrappedKeyFile,
            string alphabet)
        {
            var alphabetType = CryptoReplaceFfxFpeConfig.Types.FfxCommonNativeAlphabet.Unspecified;
            switch (alphabet)
            {
                default:
                    alphabetType = CryptoReplaceFfxFpeConfig.Types.FfxCommonNativeAlphabet.AlphaNumeric;
                    break;
                case ("an"):
                    alphabetType = CryptoReplaceFfxFpeConfig.Types.FfxCommonNativeAlphabet.AlphaNumeric;
                    break;
                case ("hex"):
                    alphabetType = CryptoReplaceFfxFpeConfig.Types.FfxCommonNativeAlphabet.Hexadecimal;
                    break;
                case ("num"):
                    alphabetType = CryptoReplaceFfxFpeConfig.Types.FfxCommonNativeAlphabet.Numeric;
                    break;
                case ("an-uc"):
                    alphabetType = CryptoReplaceFfxFpeConfig.Types.FfxCommonNativeAlphabet.UpperCaseAlphaNumeric;
                    break;
            }

            var deidConfig = new DeidentifyConfig
            {
                InfoTypeTransformations = new InfoTypeTransformations()
            };
            deidConfig.InfoTypeTransformations.Transformations.Add(
                new InfoTypeTransformations.Types.InfoTypeTransformation
                {
                    PrimitiveTransformation = new PrimitiveTransformation
                    {
                        CryptoReplaceFfxFpeConfig = new CryptoReplaceFfxFpeConfig
                        {
                            CommonAlphabet = alphabetType,
                            CryptoKey = new CryptoKey
                            {
                                KmsWrapped = new KmsWrappedCryptoKey
                                {
                                    CryptoKeyName = keyName,
                                    WrappedKey = ByteString.CopyFrom(File.ReadAllBytes(wrappedKeyFile))
                                }
                            },
                            SurrogateInfoType = new InfoType
                            {
                                Name = "TOKEN"
                            }
                        }
                    }
                });

            DlpServiceClient dlp = DlpServiceClient.Create();
            var response = dlp.DeidentifyContent(new DeidentifyContentRequest
            {
                Parent = $"projects/{projectId}",
                DeidentifyConfig = deidConfig,
                Item = new ContentItem { Value = value }
            });
            Console.WriteLine($"Deidentified content: {response.Item.Value}");
            return 0;
        }
        // [END dlp_deidentify_fpe]

        // [START dlp_reidentify_fpe]
        public static object ReidFpe(
            string projectId,
            string value,
            string keyName,
            string wrappedKeyFile,
            string alphabet)
        {
            var alphabetType = CryptoReplaceFfxFpeConfig.Types.FfxCommonNativeAlphabet.Unspecified;
            switch (alphabet)
            {
                default:
                    alphabetType = CryptoReplaceFfxFpeConfig.Types.FfxCommonNativeAlphabet.AlphaNumeric;
                    break;
                case ("an"):
                    alphabetType = CryptoReplaceFfxFpeConfig.Types.FfxCommonNativeAlphabet.AlphaNumeric;
                    break;
                case ("hex"):
                    alphabetType = CryptoReplaceFfxFpeConfig.Types.FfxCommonNativeAlphabet.Hexadecimal;
                    break;
                case ("num"):
                    alphabetType = CryptoReplaceFfxFpeConfig.Types.FfxCommonNativeAlphabet.Numeric;
                    break;
                case ("an-uc"):
                    alphabetType = CryptoReplaceFfxFpeConfig.Types.FfxCommonNativeAlphabet.UpperCaseAlphaNumeric;
                    break;
            }

            var reidConfig = new DeidentifyConfig
            {
                InfoTypeTransformations = new InfoTypeTransformations()
            };
            var inspectConfig = new InspectConfig();
            inspectConfig.CustomInfoTypes.Add(new CustomInfoType {
                InfoType = new InfoType
                {
                    Name = "TOKEN"
                },
                SurrogateType = new CustomInfoType.Types.SurrogateType()
            });
            reidConfig.InfoTypeTransformations.Transformations.Add(
                new InfoTypeTransformations.Types.InfoTypeTransformation
                {
                    PrimitiveTransformation = new PrimitiveTransformation
                    {
                        CryptoReplaceFfxFpeConfig = new CryptoReplaceFfxFpeConfig
                        {
                            CommonAlphabet = alphabetType,
                            CryptoKey = new CryptoKey
                            {
                                KmsWrapped = new KmsWrappedCryptoKey
                                {
                                    CryptoKeyName = keyName,
                                    WrappedKey = ByteString.CopyFrom(File.ReadAllBytes(wrappedKeyFile))
                                }
                            },
                            SurrogateInfoType = new InfoType
                            {
                                Name = "TOKEN"
                            }
                        }
                    }
                });
            reidConfig.InfoTypeTransformations.Transformations[0].InfoTypes.Add(new InfoType { Name = "TOKEN" });

            DlpServiceClient dlp = DlpServiceClient.Create();
            var response = dlp.ReidentifyContent(new ReidentifyContentRequest
            {
                Parent = $"projects/{projectId}",
                InspectConfig = inspectConfig,
                ReidentifyConfig = reidConfig,
                Item = new ContentItem { Value = value }
            });
            Console.WriteLine($"Reidentified content: {response.Item.Value}");
            return 0;
        }
        // [END dlp_reidentify_fpe]
    }
}
