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

// [START compute_firewall_create]

using Google.Cloud.Compute.V1;
using System.Threading.Tasks;

public class CreateFirewallRuleAsyncSample
{
    public async Task CreateFirewallRuleAsync(
        // TODO(developer): Set your own default values for these parameters or pass different values when calling this method.
        string projectId = "your-project-id",
        string firewallRuleName = "my-test-firewall-rule",
        // Name of the network the rule will be applied to. Some available name formats:
        // projects/{project_id}/global/networks/{network}
        // global/networks/{network}
        string networkName = "global/networks/default")
    {
        Firewall firewallRule = new Firewall
        {
            Name = firewallRuleName,
            Network = networkName,
            Direction = ComputeEnumConstants.Firewall.Direction.Ingress,
            Allowed =
            {
                new Allowed
                {
                    Ports = { "80", "443" },
                    IPProtocol = "tcp"
                }
            },
            TargetTags = { "web" },
            Description = "Allows TCP traffic on port 80 and 443 from anywhere."
        };

        // Note that the default value of priority for the firewall API is 1000.
        // If you check the value of firewallRule.Priority at this point it
        // will be equal to 0, however it is not treated as "set" by the library, and thus
        // the default will be applied to the new rule. If you want to create a rule that
        // has priority == 0, you'll need to explicitly set it: firewallRule.Priority = 0.
        // You can use the firewallRule.HasPriority property to check if the priority has been set.
        // You can use the firewallRule.ClearPriority() method to unset the priority.

        // Initialize client that will be used to send requests. This client only needs to be created
        // once, and can be reused for multiple requests.
        FirewallsClient client = await FirewallsClient.CreateAsync();

        // Create the firewall rule in the specified project.
        var firewallRuleCreation = await client.InsertAsync(projectId, firewallRule);

        // Wait for the operation to complete using client-side polling.
        await firewallRuleCreation.PollUntilCompletedAsync();
    }
}

// [END compute_firewall_create]
