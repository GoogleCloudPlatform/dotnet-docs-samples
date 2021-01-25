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
        /***************** REPLACE WITH YOUR OWN VALUES ********************************/
        private static readonly String jsonPrivateKeyFile = "path/to/json_key_file.json";
        private static readonly String resellerAdminUser = "admin@yourresellerdomain.com";
        private static readonly String accountId = "C012345";
        private static readonly String customerDomain = "example.com";
        /*******************************************************************************/

        private static readonly String accountParent = "accounts/" + accountId;

        private static CloudChannelServiceClient cloudChannelServiceClient;

        static void Main(string[] args)
        {
            GoogleCredential credential;

            using (var stream = new FileStream(jsonPrivateKeyFile, FileMode.Open, FileAccess.Read))
            {
                credential =
                    GoogleCredential.FromStream(stream)
                        .CreateScoped(new string[] {"https://www.googleapis.com/auth/apps.order"})
                        .CreateWithUser(resellerAdminUser);
            }

            var client =
                new CloudChannelServiceClientBuilder
                    {
                        TokenAccessMethod = ((ITokenAccess) credential).GetAccessTokenForRequestAsync
                    }
                    .Build();

            // For the purpose of this codelab, the code lists all offers and picks
            // the first offer for Google Workspace Business Standard on an Annual
            // plan. This is needed because offerIds vary from one account to another,
            // but this is not a recommended model for your production integration
            PagedEnumerable<ListOffersResponse, Offer> listOffersResponse =
                client.ListOffers(new ListOffersRequest 
                {
                    Parent = accountParent
                });
            
            Offer selectedOffer = new Offer();
            String sampleOffer = "Google Workspace Business Standard";
            String samplePlan = "Commitment";

            foreach (Offer offer in listOffersResponse.ToList())
            {
                String offerName = offer.Sku.MarketingInfo.DisplayName;
                String offerPlan = offer.Plan.PaymentPlan.ToString();
                if (offerName == sampleOffer && offerPlan == samplePlan)
                {
                    selectedOffer = offer;
                    break;
                }
            }

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

            PostalAddress postalAddress = new PostalAddress
            {
                AddressLines = {
                    "123 Main St"
                },
                PostalCode = "97224", RegionCode = "US"
            };

            Customer customer = new Customer
            {
                OrgDisplayName = "Acme Corp", OrgPostalAddress = postalAddress, Domain = customerDomain
            };

            CreateCustomerRequest createCustomerRequest =
                new CreateCustomerRequest {
                    Parent = accountParent, 
                    Customer = customer
                };

            Customer createCustomerResponse = client.CreateCustomer(createCustomerRequest);

            CloudIdentityInfo cloudIdentityInfo =
                new CloudIdentityInfo
                {
                    AlternateEmail = "marty.mcfly@gmail.com",
                    LanguageCode = "en-US"
                };

            AdminUser adminUser = new AdminUser
            {
                GivenName = "Marty", FamilyName = "McFly",
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

            PollSettings pollSettings = new PollSettings(Expiration.FromTimeout(TimeSpan.FromSeconds(30)),
                TimeSpan.FromSeconds(1));

            provisionCloudIdentityOp.PollUntilCompleted(pollSettings);

            RenewalSettings renewalSettings =
                new RenewalSettings
                {
                    EnableRenewal = true, PaymentPlan = PaymentPlan.Commitment,
                    PaymentCycle =
                        new Period {PeriodType = PeriodType.Year, Duration = 1}
                };

            CommitmentSettings commitmentSettings =
                new CommitmentSettings {
                    RenewalSettings = renewalSettings
                };

            Entitlement entitlement = new Entitlement
            {
                Offer = selectedOffer.Name,
                Parameters = {
                    new Parameter {
                        Name = "num_units", 
                        Value = new Value {
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

            createEntitlementOp.PollUntilCompleted(pollSettings);

            Console.WriteLine("Successfully created customer " + customerDomain);
        }
    }
}