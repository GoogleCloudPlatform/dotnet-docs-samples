/*
 * Copyright (c) 2021 Google Inc.
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

using Google.Apis.Auth.OAuth2;
using Google.Api.Gax;
using Google.Cloud.Channel.V1;
using Google.Type;
using System;
using System.IO;
using System.Linq;

namespace CodeLab
{
    class Program
    {
        // [START credentials]

        /***************** REPLACE WITH YOUR OWN VALUES ********************************/
        private static readonly string jsonPrivateKeyFile = "path/to/json_key_file.json";
        private static readonly string resellerAdminUser = "admin@yourresellerdomain.com";
        private static readonly string accountId = "C012345";
        private static readonly string customerDomain = "example.com";
        /*******************************************************************************/

        private static readonly string accountParent = "accounts/" + accountId;

        private static CloudChannelServiceClient cloudChannelServiceClient;

        static void Main(string[] args)
        {
            GoogleCredential credential;

            using (var stream = new FileStream(jsonPrivateKeyFile, FileMode.Open, FileAccess.Read))
            {
                credential =
                    GoogleCredential.FromStream(stream)
                        .CreateScoped(CloudChannelServiceClient.DefaultScopes)
                        .CreateWithUser(resellerAdminUser);
            }

            var client =
                new CloudChannelServiceClientBuilder
                    {
                        TokenAccessMethod = ((ITokenAccess) credential).GetAccessTokenForRequestAsync
                    }
                    .Build();
            // [END credentials]

            // [START pickOffer]
            // For the purpose of this codelab, the code lists all offers and picks
            // the first offer for Google Workspace Business Standard on an Annual
            // plan. This is needed because offerIds vary from one account to another,
            // but this is not a recommended model for your production integration
            PagedEnumerable<ListOffersResponse, Offer> listOffersResponse =
                client.ListOffers(new ListOffersRequest
                {
                    Parent = accountParent
                });

            string sampleOffer = "Google Workspace Business Standard";
            PaymentPlan samplePlan = PaymentPlan.Commitment;

            Offer selectedOffer = listOffersResponse.FirstOrDefault(o =>
                o.Sku.MarketingInfo.DisplayName == sampleOffer && o.Plan.PaymentPlan == samplePlan);
            // [END pickOffer]

            // [START checkExists]
            // Determine if customer already has a cloud identity
            CheckCloudIdentityAccountsExistRequest checkCloudIdentityAccountsExistRequest =
                new CheckCloudIdentityAccountsExistRequest
                {
                    Parent = accountParent,
                    Domain = customerDomain
                };

            CheckCloudIdentityAccountsExistResponse checkIdentityResponse =
                client.CheckCloudIdentityAccountsExist(checkCloudIdentityAccountsExistRequest);

            if (checkIdentityResponse.CloudIdentityAccounts.Count > 0)
            {
                throw new Exception(
                    "Cloud identity already exists. Customer must be transferred. Out of scope for this codelab");
            }
            // [END checkExists]

            // [START createCustomer]
            // Create the Customer resource
            PostalAddress postalAddress = new PostalAddress
            {
                AddressLines =
                {
                    "123 Main St"
                },
                PostalCode = "97224",
                RegionCode = "US"
            };

            Customer customer = new Customer
            {
                OrgDisplayName = "Acme Corp",
                OrgPostalAddress = postalAddress,
                Domain = customerDomain
            };

            CreateCustomerRequest createCustomerRequest =
                new CreateCustomerRequest
                {
                    Parent = accountParent,
                    Customer = customer
                };

            Customer createCustomerResponse = client.CreateCustomer(createCustomerRequest);
            // [END createCustomer]

            // [START provisionCloudIdentity]
            CloudIdentityInfo cloudIdentityInfo = new CloudIdentityInfo
            {
                AlternateEmail = "marty.mcfly@gmail.com",
                LanguageCode = "en-US"
            };

            AdminUser adminUser = new AdminUser
            {
                GivenName = "Marty",
                FamilyName = "McFly",
                Email = "admin@" + createCustomerResponse.Domain
            };

            ProvisionCloudIdentityRequest provisionCloudIdentityRequest =
                new ProvisionCloudIdentityRequest
                {
                    Customer = createCustomerResponse.Name,
                    CloudIdentityInfo = cloudIdentityInfo,
                    User = adminUser
                };

            var provisionCloudIdentityOp = client.ProvisionCloudIdentity(provisionCloudIdentityRequest);

            provisionCloudIdentityOp.PollUntilCompleted();
            // [END provisionCloudIdentity]

            // [START createEntitlement]
            RenewalSettings renewalSettings = new RenewalSettings
            {
                EnableRenewal = true,
                PaymentPlan = PaymentPlan.Commitment,
                PaymentCycle = new Period
                {
                    PeriodType = PeriodType.Year,
                    Duration = 1
                }
            };

            CommitmentSettings commitmentSettings = new CommitmentSettings
            {
                RenewalSettings = renewalSettings
            };

            Entitlement entitlement = new Entitlement
            {
                Offer = selectedOffer.Name,
                Parameters =
                {
                    new Parameter
                    {
                        Name = "num_units",
                        Value = new Value
                        {
                            Int64Value = 5
                        }
                    }
                },
                CommitmentSettings = commitmentSettings,
            };

            CreateEntitlementRequest createEntitlementRequest =
                new CreateEntitlementRequest
                {
                    Parent = createCustomerResponse.Name,
                    Entitlement = entitlement
                };

            var createEntitlementOp = client.CreateEntitlement(createEntitlementRequest);

            createEntitlementOp.PollUntilCompleted();
            // [END createEntitlement]

            Console.WriteLine("Successfully created customer " + customerDomain);
        }
    }
}