using Google.Cloud.Dlp.V2;
using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.IO;

namespace GoogleCloudSamples
{
    public class DeIdentify : DlpSampleBase
    {
        /// <summary>
        /// Deidentify primitive information using a mask character & number.
        /// </summary>
        /// <param name="projectId">Google cloud project-id.</param>
        /// <param name="value">String in which to mask content.</param>
        /// <param name="maskingCharacter">Character to use to mask sensitive content.</param>
        /// <param name="numberToMask">Number of sequential characters to mask.</param>
        /// <returns>Status code (0 if success).</returns>
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
            return DeId(request);
        }

        public static object DeidFpe(
            string projectId,
            string value,
            string keyName,
            string wrappedKeyFile,
            string alphabet,
            bool reidentify)
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
                            }
                        }
                    }
                });

            if (reidentify)
            {
                return ReId(new ReidentifyContentRequest
                {
                    Parent = $"projects/{projectId}",
                    ReidentifyConfig = deidConfig,
                    Item = new ContentItem { Value = value }
                });
            }
            else
            {
                return DeId(new DeidentifyContentRequest
                {
                    Parent = $"projects/{projectId}",
                    DeidentifyConfig = deidConfig,
                    Item = new ContentItem { Value = value }
                });
            }
        }

        static object DeId(DeidentifyContentRequest request)
        {
            DlpServiceClient dlp = DlpServiceClient.Create();
            var response = dlp.DeidentifyContent(request);
            Console.WriteLine($"Deidentified content: {response.Item.Value}");
            return 0;
        }

        static object ReId(ReidentifyContentRequest request)
        {
            DlpServiceClient dlp = DlpServiceClient.Create();
            var response = dlp.ReidentifyContent(request);
            Console.WriteLine($"Reidentified content: {response.Item.Value}");
            return 0;
        }
    }
}
