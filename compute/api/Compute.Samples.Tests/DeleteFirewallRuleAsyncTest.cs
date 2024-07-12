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

using Grpc.Core;
using System.Threading.Tasks;
using Xunit;

namespace Compute.Samples.Tests
{
    [Collection(nameof(ComputeFixture))]
    public class DeleteFirewallRuleAsyncTest
    {
        private readonly ComputeFixture _fixture;
        private readonly CreateFirewallRuleAsyncSample _createSample = new CreateFirewallRuleAsyncSample();
        private readonly DeleteFirewallRuleAsyncSample _deleteSample = new DeleteFirewallRuleAsyncSample();

        public DeleteFirewallRuleAsyncTest(ComputeFixture fixture) => _fixture = fixture;

        [Fact]
        public async Task DeletesFirewallRule()
        {
            string firewallRuleName = _fixture.GenerateFirewallRuleName();

            await _fixture.FirewallRuleCreated.Eventually(async () =>
                // Let's use the create firewall rule sample because that polls until completed.
                await _createSample.CreateFirewallRuleAsync(_fixture.ProjectId, firewallRuleName, _fixture.NetworkResourceUri));

            await _fixture.FirewallRuleReady.Eventually(async () =>
                await _deleteSample.DeleteFirewallRuleAsync(_fixture.ProjectId, firewallRuleName));

            var exception = await Assert.ThrowsAsync<RpcException>(async () => await _fixture.FirewallsClient.GetAsync(_fixture.ProjectId, firewallRuleName));
            Assert.Equal(StatusCode.NotFound, exception.StatusCode);
        }
    }
}
