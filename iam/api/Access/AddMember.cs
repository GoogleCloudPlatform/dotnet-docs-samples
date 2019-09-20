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

// [START iam_modify_policy_member]

using System.Linq;
using Google.Apis.CloudResourceManager.v1.Data;

public partial class AccessManager
{
    public static Policy AddMember(Policy policy, string role, string member)
    {
        var binding = policy.Bindings.First(x => x.Role == role);
        binding.Members.Add(member);
        return policy;
    }
}
// [END iam_modify_policy_member]
