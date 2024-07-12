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

using System.Threading.Tasks;
using Xunit;

namespace Compute.Samples.Tests
{
    [Collection(nameof(ComputeFixture))]
    public class PatchFirewallRuleAsyncTest
    {
        private readonly ComputeFixture _fixture;
        private readonly CreateFirewallRuleAsyncSample _createSample = new CreateFirewallRuleAsyncSample();
        private readonly PatchFirewallRuleAsyncSample _sample = new PatchFirewallRuleAsyncSample();

        public PatchFirewallRuleAsyncTest(ComputeFixture fixture) => _fixture = fixture;

        [Fact]
        public async Task PatchFirewallRule()
        {
            string firewallRuleName = _fixture.GenerateFirewallRuleName();

            await _fixture.FirewallRuleCreated.Eventually(async () =>
                await _createSample.CreateFirewallRuleAsync(
                    _fixture.ProjectId, firewallRuleName, _fixture.NetworkResourceUri));

            await _fixture.FirewallRuleReady.Eventually(async () =>
            {
                var rule = await _fixture.FirewallsClient.GetAsync(_fixture.ProjectId, firewallRuleName);
                Assert.NotEqual(10, rule.Priority);

                await _sample.PatchFirewallRuleAsync(_fixture.ProjectId, firewallRuleName, 10);

                rule = await _fixture.FirewallsClient.GetAsync(_fixture.ProjectId, firewallRuleName);
                Assert.Equal(10, rule.Priority);
            });
        }
    }
}
