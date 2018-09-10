// Copyright 2018 Google Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using Google.Apis.Auth.OAuth2;
using Google.Apis.CloudResourceManager.v1;
using Google.Apis.CloudResourceManager.v1.Data;

namespace GoogleCloudSamples
{
    public class AccessManager
    {
        private CloudResourceManagerService _service;

        public AccessManager()
        {
            var credential = GoogleCredential.GetApplicationDefault()
                .CreateScoped(CloudResourceManagerService.Scope.CloudPlatform);
            _service = new CloudResourceManagerService(
                new CloudResourceManagerService.Initializer
                {
                    HttpClientInitializer = credential
                });
        }

        // [START iam_get_policy]
        public Policy GetPolicy(string projectId)
        {
            Policy policy = _service.Projects.GetIamPolicy(
                new GetIamPolicyRequest(), projectId).Execute();
            return policy;
        }
        // [END iam_get_policy]

        // [START iam_modify_policy_member]
        public Policy AddMember(Policy policy, string role, string member)
        {
            Binding binding = policy.Bindings.First(x => x.Role == role);
            binding.Members.Add(member);
            return policy;
        }
        // [END iam_modify_policy_member]

        // [START iam_modify_policy_add_role]
        public Policy AddBinding(Policy policy, string role, string member)
        {
            var binding = new Binding
            {
                Role = role,
                Members = new List<string> { member }
            };
            policy.Bindings.Add(binding);
            return policy;
        }
        // [END iam_modify_policy_add_role]

        // [START iam_set_policy]
        public Policy SetPolicy(string projectId, Policy policy)
        {
            return _service.Projects.SetIamPolicy(new SetIamPolicyRequest
            {
                Policy = policy
            }, projectId).Execute();
        }
        // [END iam_set_policy]
    }
}
