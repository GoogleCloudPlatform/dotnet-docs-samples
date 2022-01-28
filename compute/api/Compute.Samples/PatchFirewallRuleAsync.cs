/*
 * Copyright 2021 Google LLC
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

// [START compute_firewall_patch]

using Google.Cloud.Compute.V1;
using System.Threading.Tasks;

public class PatchFirewallRuleAsyncSample
{
    public async Task PatchFirewallRuleAsync(
        // TODO(developer): Set your own default values for these parameters or pass different values when calling this method.
        string projectId = "your-project-id",
        string firewallRuleName = "my-test-firewall-rule",
        int newPriority = 10)
    {
        // The patch operation doesn't require the full definition of a Firewall object.
        // It will only update the values that were set in it,
        // in this case it will only change the priority.
        Firewall firewallRule = new Firewall
        {
            Priority = newPriority
        };

        // Initialize client that will be used to send requests. This client only needs to be created
        // once, and can be reused for multiple requests.
        FirewallsClient client = await FirewallsClient.CreateAsync();

        // Patch the firewall rule in the specified project.
        var firewallRulePatching = await client.PatchAsync(projectId, firewallRuleName, firewallRule);

        // Wait for the operation to complete using client-side polling.
        await firewallRulePatching.PollUntilCompletedAsync();
    }
}

// [END compute_firewall_patch]
