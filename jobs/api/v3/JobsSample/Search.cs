/*
 * Copyright (c) 2018 Google Inc.
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
using Google.Apis.Auth.OAuth2;
using Google.Apis.CloudTalentSolution.v3;
using Google.Apis.CloudTalentSolution.v3.Data;
using Google.Apis.Services;
using static Google.Apis.CloudTalentSolution.v3.ProjectsResource;

namespace GoogleCloudSamples
{
    public class Search
    {
        public static GoogleCredential credential;
        public static CloudTalentSolutionService jobServiceClient;
        public static string projectId;
        public static string parent;
        public static Company myCompany;
        public static Job myJob;

        public static void Initialize()
        {
            // Authorize the client using Application Default Credentials.
            // See: https://developers.google.com/identity/protocols/application-default-credentials
            credential = GoogleCredential.GetApplicationDefaultAsync().Result;
            // Specify the Service scope.
            if (credential.IsCreateScopedRequired)
            {
                credential = credential.CreateScoped(new[]
                {
                    Google.Apis.CloudTalentSolution.v3.CloudTalentSolutionService.Scope.Jobs
                });
            }
            // Instantiate the Cloud Key Management Service API.
            jobServiceClient = new CloudTalentSolutionService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                GZipEnabled = false
            });

            projectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
            parent = $"projects/{projectId}";
        }

        public static void Main(string[] args)
        {
            Initialize();
            SetupCompany();
            SetupJobs();

            SearchFilters();
            SearchLocations();
            CommuteSearch(null);

            Teardown();
        }

        public static void SetupJobs()
        {
            // Test create job
            ApplicationInfo info = new ApplicationInfo()
            {
                Instruction = "Application instructions here"
            };
            myJob = new Job()
            {
                Title = "Software Engineer",
                RequisitionId = generateRandom(),
                Description = "My Job Description Here",
                CompanyName = myCompany.Name,
                ApplicationInfo = info
            };

            myJob = CreateJob(myJob);
        }

        // [START commute_search]

        public static void CommuteSearch(string companyName)
        {
            RequestMetadata requestMetadata = new RequestMetadata()
            {
                // Make sure to hash your userID
                UserId = "HashedUserId",
                // Make sure to hash the sessionID
                SessionId = "HashedSessionId",
                // Domain of the website where the search is conducted
                Domain = "www.google.com"
            };

            JobQuery jobQuery = new JobQuery()
            {
                CommuteFilter = new CommuteFilter()
                {
                    RoadTraffic = "TRAFFIC_FREE",
                    CommuteMethod = "TRANSIT",
                    TravelDuration = "1000s",
                    StartCoordinates = new LatLng()
                    {
                        Latitude = 37.42208,
                        Longitude = -122.085609
                    }
                }
            };

            if (companyName != null)
            {
                jobQuery.CompanyNames = new List<string>
                {
                    companyName
                };
            }

            SearchJobsRequest searchJobRequest = new SearchJobsRequest()
            {
                RequestMetadata = requestMetadata,
                JobQuery = jobQuery,
                JobView = "JOB_VIEW_FULL",
                RequirePreciseResultSize = true
            };

            SearchJobsResponse searchJobsResponse = jobServiceClient.Projects.Jobs.Search(searchJobRequest, parent).Execute();

            Console.WriteLine("Jobs commute searched: " + ToJsonString(searchJobsResponse));
        }
        // [END commute_search]

        public static void SearchLocations()
        {
            BasicLocationSearch(null, "Mountain View", 100);
            KeywordLocationSearch(null, "Mountain View", 100, "software");
            CityLocationSearch(null, "Mountain View");
            MultiLocationSearch(null, "Mountain View", 100, "San Francisco");
            BroadeningLocationsSearch(null, "Mountain View");
        }

        // [START basic_location_search]

        public static void BasicLocationSearch(string companyName, string location, double distance)
        {
            RequestMetadata requestMetadata = new RequestMetadata()
            {
                // Make sure to hash your userID
                UserId = "HashedUserId",
                // Make sure to hash the sessionID
                SessionId = "HashedSessionId",
                // Domain of the website where the search is conducted
                Domain = "www.google.com"
            };

            LocationFilter locationFilter = new LocationFilter()
            {
                Address = location,
                DistanceInMiles = distance
            };

            JobQuery jobQuery = new JobQuery()
            {
                LocationFilters = new List<LocationFilter>()
                {
                    locationFilter
                }
            };

            if (companyName != null)
            {
                jobQuery.CompanyNames = new List<string>
                {
                    companyName
                };
            }

            SearchJobsRequest searchJobRequest = new SearchJobsRequest()
            {
                RequestMetadata = requestMetadata,
                JobQuery = jobQuery,
                SearchMode = "JOB_SEARCH"
            };

            SearchJobsResponse searchJobsResponse = jobServiceClient.Projects.Jobs.Search(searchJobRequest, parent).Execute();

            Console.WriteLine("Jobs basic location searched: " + ToJsonString(searchJobsResponse));
        }
        // [END basic_location_search]

        // [START keyword_location_search]

        public static void KeywordLocationSearch(string companyName, string location, double distance, string keyword)
        {
            RequestMetadata requestMetadata = new RequestMetadata()
            {
                // Make sure to hash your userID
                UserId = "HashedUserId",
                // Make sure to hash the sessionID
                SessionId = "HashedSessionId",
                // Domain of the website where the search is conducted
                Domain = "www.google.com"
            };

            LocationFilter locationFilter = new LocationFilter()
            {
                Address = location,
                DistanceInMiles = distance
            };

            JobQuery jobQuery = new JobQuery()
            {
                LocationFilters = new List<LocationFilter>()
                {
                    locationFilter
                },
                Query = keyword
            };

            if (companyName != null)
            {
                jobQuery.CompanyNames = new List<string>
                {
                    companyName
                };
            }

            SearchJobsRequest searchJobRequest = new SearchJobsRequest()
            {
                RequestMetadata = requestMetadata,
                JobQuery = jobQuery,
                SearchMode = "JOB_SEARCH"
            };

            SearchJobsResponse searchJobsResponse = jobServiceClient.Projects.Jobs.Search(searchJobRequest, parent).Execute();

            Console.WriteLine("Jobs location and keyword searched: " + ToJsonString(searchJobsResponse));
        }
        // [END keyword_location_search]

        // [START city_location_search]

        public static void CityLocationSearch(string companyName, string location)
        {
            RequestMetadata requestMetadata = new RequestMetadata()
            {
                // Make sure to hash your userID
                UserId = "HashedUserId",
                // Make sure to hash the sessionID
                SessionId = "HashedSessionId",
                // Domain of the website where the search is conducted
                Domain = "www.google.com"
            };

            LocationFilter locationFilter = new LocationFilter()
            {
                Address = location
            };

            JobQuery jobQuery = new JobQuery()
            {
                LocationFilters = new List<LocationFilter>()
                {
                    locationFilter
                }
            };

            if (companyName != null)
            {
                jobQuery.CompanyNames = new List<string>
                {
                    companyName
                };
            }

            SearchJobsRequest searchJobRequest = new SearchJobsRequest()
            {
                RequestMetadata = requestMetadata,
                JobQuery = jobQuery,
                SearchMode = "JOB_SEARCH"
            };

            SearchJobsResponse searchJobsResponse = jobServiceClient.Projects.Jobs.Search(searchJobRequest, parent).Execute();

            Console.WriteLine("Jobs city level searched: " + ToJsonString(searchJobsResponse));
        }
        // [END city_location_search]

        // [START multi_location_search]

        public static void MultiLocationSearch(string companyName, string location1, double distance1, string location2)
        {
            RequestMetadata requestMetadata = new RequestMetadata()
            {
                // Make sure to hash your userID
                UserId = "HashedUserId",
                // Make sure to hash the sessionID
                SessionId = "HashedSessionId",
                // Domain of the website where the search is conducted
                Domain = "www.google.com"
            };

            LocationFilter locationFilter1 = new LocationFilter()
            {
                Address = location1,
                DistanceInMiles = distance1
            };

            LocationFilter locationFilter2 = new LocationFilter()
            {
                Address = location2
            };

            JobQuery jobQuery = new JobQuery()
            {
                LocationFilters = new List<LocationFilter>()
                {
                    locationFilter1,
                    locationFilter2
                }
            };

            if (companyName != null)
            {
                jobQuery.CompanyNames = new List<string>
                {
                    companyName
                };
            }

            SearchJobsRequest searchJobRequest = new SearchJobsRequest()
            {
                RequestMetadata = requestMetadata,
                JobQuery = jobQuery,
                SearchMode = "JOB_SEARCH"
            };

            SearchJobsResponse searchJobsResponse = jobServiceClient.Projects.Jobs.Search(searchJobRequest, parent).Execute();

            Console.WriteLine("Jobs multi location searched: " + ToJsonString(searchJobsResponse));
        }
        // [END multi_location_search]

        // [START broadening_location_search]

        public static void BroadeningLocationsSearch(string companyName, string location)
        {
            RequestMetadata requestMetadata = new RequestMetadata()
            {
                // Make sure to hash your userID
                UserId = "HashedUserId",
                // Make sure to hash the sessionID
                SessionId = "HashedSessionId",
                // Domain of the website where the search is conducted
                Domain = "www.google.com"
            };

            LocationFilter locationFilter = new LocationFilter()
            {
                Address = location
            };

            JobQuery jobQuery = new JobQuery()
            {
                LocationFilters = new List<LocationFilter>()
                {
                    locationFilter
                }
            };

            if (companyName != null)
            {
                jobQuery.CompanyNames = new List<string>
                {
                    companyName
                };
            }

            SearchJobsRequest searchJobRequest = new SearchJobsRequest()
            {
                RequestMetadata = requestMetadata,
                JobQuery = jobQuery,
                EnableBroadening = true,
                SearchMode = "JOB_SEARCH"
            };

            SearchJobsResponse searchJobsResponse = jobServiceClient.Projects.Jobs.Search(searchJobRequest, parent).Execute();

            Console.WriteLine("Jobs broadening searched: " + ToJsonString(searchJobsResponse));
        }
        // [END broadening_location_search]

        public static void SearchFilters()
        {
            // Category search
            List<string> categories = new List<string>()
            {
                "SCIENCE_AND_ENGINEERING"
            };

            CategoryFilterSearch(null, categories);

            // Employment type search
            List<string> employmentTypes = new List<string>()
            {
                "FULL_TIME"
            };

            EmploymentTypesSearch(null, employmentTypes);

            // Language code search
            List<string> languageCodes = new List<string>()
            {
                "en-US"
            };

            LanguageCodeSearch(null, languageCodes);

            // Company display names search
            List<string> companyDisplayNames = new List<string>()
            {
                "Google, LLC."
            };

            CompanyDisplayNameSearch(null, companyDisplayNames);

            // Compensation search
            CompensationSearch(null);
        }

        // [START category_filter]

        public static void CategoryFilterSearch(string companyName, List<String> categories)
        {
            RequestMetadata requestMetadata = new RequestMetadata()
            {
                // Make sure to hash your userID
                UserId = "HashedUserId",
                // Make sure to hash the sessionID
                SessionId = "HashedSessionId",
                // Domain of the website where the search is conducted
                Domain = "www.google.com"
            };

            JobQuery jobQuery = new JobQuery()
            {
                JobCategories = categories
            };

            if (companyName != null)
            {
                jobQuery.CompanyNames = new List<string>
                {
                    companyName
                };
            }

            SearchJobsRequest searchJobRequest = new SearchJobsRequest()
            {
                RequestMetadata = requestMetadata,
                JobQuery = jobQuery,
                SearchMode = "JOB_SEARCH"
            };

            SearchJobsResponse searchJobsResponse = jobServiceClient.Projects.Jobs.Search(searchJobRequest, parent).Execute();

            Console.WriteLine("Jobs category searched: " + ToJsonString(searchJobsResponse));
        }
        // [END category_filter]

        // [START employment_types_filter]

        public static void EmploymentTypesSearch(string companyName, List<string> employmentTypes)
        {
            RequestMetadata requestMetadata = new RequestMetadata()
            {
                // Make sure to hash your userID
                UserId = "HashedUserId",
                // Make sure to hash the sessionID
                SessionId = "HashedSessionId",
                // Domain of the website where the search is conducted
                Domain = "www.google.com"
            };

            JobQuery jobQuery = new JobQuery()
            {
                EmploymentTypes = employmentTypes
            };

            if (companyName != null)
            {
                jobQuery.CompanyNames = new List<string>
                {
                    companyName
                };
            }

            SearchJobsRequest searchJobRequest = new SearchJobsRequest()
            {
                RequestMetadata = requestMetadata,
                JobQuery = jobQuery,
                SearchMode = "JOB_SEARCH"
            };

            SearchJobsResponse searchJobsResponse = jobServiceClient.Projects.Jobs.Search(searchJobRequest, parent).Execute();

            Console.WriteLine("Jobs employment type searched: " + ToJsonString(searchJobsResponse));
        }
        // [END employment_types_filter]

        // [START date_range_filter]

        public static void DateRangeSearch(string companyName, string startTime, string endTime)
        {
            RequestMetadata requestMetadata = new RequestMetadata()
            {
                // Make sure to hash your userID
                UserId = "HashedUserId",
                // Make sure to hash the sessionID
                SessionId = "HashedSessionId",
                // Domain of the website where the search is conducted
                Domain = "www.google.com"
            };

            TimestampRange timeStampRange = new TimestampRange()
            {
                StartTime = startTime,
                EndTime = endTime
            };

            JobQuery jobQuery = new JobQuery()
            {
                PublishTimeRange = timeStampRange
            };

            if (companyName != null)
            {
                jobQuery.CompanyNames = new List<string>
                {
                    companyName
                };
            }

            SearchJobsRequest searchJobRequest = new SearchJobsRequest()
            {
                RequestMetadata = requestMetadata,
                JobQuery = jobQuery,
                SearchMode = "JOB_SEARCH"
            };

            SearchJobsResponse searchJobsResponse = jobServiceClient.Projects.Jobs.Search(searchJobRequest, parent).Execute();

            Console.WriteLine("Jobs date range searched: " + ToJsonString(searchJobsResponse));
        }
        // [END date_range_filter]

        // [START language_code_filter]

        public static void LanguageCodeSearch(string companyName, List<string> languageCodes)
        {
            RequestMetadata requestMetadata = new RequestMetadata()
            {
                // Make sure to hash your userID
                UserId = "HashedUserId",
                // Make sure to hash the sessionID
                SessionId = "HashedSessionId",
                // Domain of the website where the search is conducted
                Domain = "www.google.com"
            };

            JobQuery jobQuery = new JobQuery()
            {
                LanguageCodes = languageCodes
            };

            if (companyName != null)
            {
                jobQuery.CompanyNames = new List<string>
                {
                    companyName
                };
            }

            SearchJobsRequest searchJobRequest = new SearchJobsRequest()
            {
                RequestMetadata = requestMetadata,
                JobQuery = jobQuery,
                SearchMode = "JOB_SEARCH"
            };

            SearchJobsResponse searchJobsResponse = jobServiceClient.Projects.Jobs.Search(searchJobRequest, parent).Execute();

            Console.WriteLine("Jobs language codes searched: " + ToJsonString(searchJobsResponse));
        }
        // [END language_code_filter]

        // [START company_display_name_filter]

        public static void CompanyDisplayNameSearch(string companyName, List<string> companyDisplayNames)
        {
            RequestMetadata requestMetadata = new RequestMetadata()
            {
                // Make sure to hash your userID
                UserId = "HashedUserId",
                // Make sure to hash the sessionID
                SessionId = "HashedSessionId",
                // Domain of the website where the search is conducted
                Domain = "www.google.com"
            };

            JobQuery jobQuery = new JobQuery()
            {
                CompanyDisplayNames = companyDisplayNames
            };

            if (companyName != null)
            {
                jobQuery.CompanyNames = new List<string>
                {
                    companyName
                };
            }

            SearchJobsRequest searchJobRequest = new SearchJobsRequest()
            {
                RequestMetadata = requestMetadata,
                JobQuery = jobQuery,
                SearchMode = "JOB_SEARCH"
            };

            SearchJobsResponse searchJobsResponse = jobServiceClient.Projects.Jobs.Search(searchJobRequest, parent).Execute();

            Console.WriteLine("Jobs company display names searched: " + ToJsonString(searchJobsResponse));
        }
        // [END company_display_name_filter]

        // [START compensation_filter]

        public static void CompensationSearch(string companyName)
        {
            RequestMetadata requestMetadata = new RequestMetadata()
            {
                // Make sure to hash your userID
                UserId = "HashedUserId",
                // Make sure to hash the sessionID
                SessionId = "HashedSessionId",
                // Domain of the website where the search is conducted
                Domain = "www.google.com"
            };

            JobQuery jobQuery = new JobQuery()
            {
                CompensationFilter = new CompensationFilter()
                {
                    Type = "UNIT_AND_AMOUNT",
                    Units = new List<string> { "HOURLY" },
                    Range = new CompensationRange()
                    {
                        MaxCompensation = new Money()
                        {
                            CurrencyCode = "USD",
                            Units = 15L
                        },
                        MinCompensation = new Money()
                        {
                            CurrencyCode = "USD",
                            Units = 10L,
                            Nanos = 500000000
                        }
                    }
                }
            };

            if (companyName != null)
            {
                jobQuery.CompanyNames = new List<string>
                {
                    companyName
                };
            }

            SearchJobsRequest searchJobRequest = new SearchJobsRequest()
            {
                RequestMetadata = requestMetadata,
                JobQuery = jobQuery,
                SearchMode = "JOB_SEARCH"
            };

            SearchJobsResponse searchJobsResponse = jobServiceClient.Projects.Jobs.Search(searchJobRequest, parent).Execute();

            Console.WriteLine("Jobs compensation searched: " + ToJsonString(searchJobsResponse));
        }
        // [END compensation_filter]

        public static void BasicSearchJobs(string companyName, string query)
        {
            RequestMetadata requestMetadata = new RequestMetadata()
            {
                // Make sure to hash your userID
                UserId = "HashedUserId",
                // Make sure to hash the sessionID
                SessionId = "HashedSessionId",
                // Domain of the website where the search is conducted
                Domain = "www.google.com"
            };

            JobQuery jobQuery = new JobQuery()
            {
                Query = query,
                CompanyNames = new List<string>
                {
                    companyName
                }
            };

            SearchJobsRequest searchJobRequest = new SearchJobsRequest()
            {
                RequestMetadata = requestMetadata,
                JobQuery = jobQuery,
                SearchMode = "JOB_SEARCH"
            };

            SearchJobsResponse searchJobsResponse = jobServiceClient.Projects.Jobs.Search(searchJobRequest, parent).Execute();

            Console.WriteLine("Jobs searched: " + ToJsonString(searchJobsResponse));
        }

        // [START histogram_search]

        public static void HistogramSearch(String companyName)
        {
            RequestMetadata requestMetadata = new RequestMetadata()
            {
                // Make sure to hash your userID
                UserId = "HashedUserId",
                // Make sure to hash the sessionID
                SessionId = "HashedSessionId",
                // Domain of the website where the search is conducted
                Domain = "www.google.com"
            };

            HistogramFacets histogramFacets = new HistogramFacets()
            {
                SimpleHistogramFacets = new List<String>
                {
                    "COMPANY_ID"
                },
                CustomAttributeHistogramFacets = new List<CustomAttributeHistogramRequest>
                {
                    new CustomAttributeHistogramRequest()
                    {
                        Key = "someFieldName1",
                        StringValueHistogram = true
                    }
                }
            };

            SearchJobsRequest searchJobsRequest = new SearchJobsRequest()
            {
                RequestMetadata = requestMetadata,
                SearchMode = "JOB_SEARCH",
                HistogramFacets = histogramFacets
            };

            if (companyName != null)
            {
                searchJobsRequest.JobQuery = new JobQuery()
                {
                    CompanyNames = new List<string>
                    {
                        companyName
                    }
                };
            }

            SearchJobsResponse searchJobsResponse = jobServiceClient.Projects.Jobs.Search(searchJobsRequest, parent).Execute();
            Console.WriteLine("Histogram search: " + ToJsonString(searchJobsResponse));
        }
        // [END histogram_search]

        // [START auto_complete_job_title]

        public static void JobTitleAutoComplete(String companyName, String query)
        {
            CompleteRequest completeRequest = new CompleteRequest(jobServiceClient, parent)
            {
                Query = query,
                LanguageCode = "en-US",
                Type = CompleteRequest.TypeEnum.JOBTITLE,
                PageSize = 10
            };

            if (companyName != null)
            {
                completeRequest.CompanyName = companyName;
            }

            CompleteQueryResponse results = completeRequest.Execute();
            Console.WriteLine("Completion results: " + ToJsonString(results));
        }
        // [END auto_complete_job_title]

        public static Job CreateJob(Job jobToBeCreated)
        {
            CreateJobRequest createJobRequest = new CreateJobRequest();
            createJobRequest.Job = jobToBeCreated;

            Job jobCreated = jobServiceClient.Projects.Jobs.Create(createJobRequest, parent).Execute();
            Console.WriteLine("Job created: " + jobCreated.Name);
            return jobCreated;
        }


        public static Job getJob(String jobName)
        {
            Job jobExisted = jobServiceClient.Projects.Jobs.Get(jobName).Execute();
            Console.WriteLine("Job exists: " + jobExisted.Name);
            return jobExisted;
        }

        public static void DeleteJob(String jobName)
        {
            jobServiceClient.Projects.Jobs.Delete(jobName).Execute();
            Console.WriteLine("Job deleted: " + jobName);
        }

        public static void SetupCompany()
        {
            myCompany = new Company()
            {
                DisplayName = "Google",
                HeadquartersAddress = "1600 Ampitheatre Parkway, Mountain View CA 94043",
                ExternalId = generateRandom()
            };
            myCompany = CreateCompany(myCompany);
        }

        public static Company CreateCompany(Company companyToBeCreated)
        {
            // Start method
            CreateCompanyRequest createCompanyRequest = new CreateCompanyRequest();
            createCompanyRequest.Company = companyToBeCreated;
            Company companyCreated = jobServiceClient.Projects.Companies.Create(createCompanyRequest, parent).Execute();
            Console.WriteLine("Created company: " + ToJsonString(companyCreated));
            return companyCreated;
        }

        public static Company GetCompany(string companyName)
        {
            try
            {
                Company companyExisted = jobServiceClient.Projects.Companies.Get(companyName).Execute();
                Console.WriteLine("Company existed: " + companyExisted.Name);
                return companyExisted;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                throw e;
            }
        }

        public static void DeleteCompany(string companyName)
        {
            jobServiceClient.Projects.Companies.Delete(companyName).Execute();
            Console.WriteLine("Deleted company: " + companyName);
        }

        public static void Teardown()
        {
            DeleteJob(myJob.Name);
            DeleteCompany(myCompany.Name);
        }

        public static String generateRandom()
        {
            Random random = new Random();
            int randomId = random.Next(1000000, 10000000);
            return randomId.ToString();
        }

        private static string ToJsonString(object obj)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(obj);
        }
    }
}
