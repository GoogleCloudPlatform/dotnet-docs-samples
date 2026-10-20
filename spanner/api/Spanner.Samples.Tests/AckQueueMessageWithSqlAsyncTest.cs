// Copyright 2026 Google LLC
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     https://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.


using System.Threading.Tasks;
using Xunit;

[Collection(nameof(SpannerFixture))]
public class AckQueueMessageWithSqlAsyncTest
{
    private readonly SpannerFixture _spannerFixture;

    public AckQueueMessageWithSqlAsyncTest(SpannerFixture spannerFixture)
    {
        _spannerFixture = spannerFixture;
    }

    [Fact]
    public async Task TestAckQueueMessageWithSqlAsync()
    {
        await _spannerFixture.RunWithTemporaryDatabaseAsync(async databaseId =>
        {
            const long messageId = 1;

            SendToQueueWithSqlAsyncSample sendSample = new SendToQueueWithSqlAsyncSample();
            await sendSample.SendToQueueWithSqlAsync(_spannerFixture.ProjectId, _spannerFixture.InstanceId, databaseId, messageId, "Hello World");

            AckQueueMessageWithSqlAsyncSample sample = new AckQueueMessageWithSqlAsyncSample();
            await sample.AckQueueMessageWithSqlAsync(_spannerFixture.ProjectId, _spannerFixture.InstanceId, databaseId, messageId);

            Assert.Null(await _spannerFixture.GetQueueMessagePayloadAsync(databaseId, messageId));
        }, SpannerFixture.CreateQueueStatement);
    }
}
