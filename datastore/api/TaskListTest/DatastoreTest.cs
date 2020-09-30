// Copyright 2016 Google Inc.
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

using Google.Cloud.Datastore.V1;
using Google.Protobuf;
using System;
using System.Linq;
using System.Collections.Generic;
using Xunit;
using System.Threading;

namespace GoogleCloudSamples
{
    public class DatastoreTest : IDisposable
    {
        private readonly string _projectId;
        private readonly DatastoreDb _db;
        private readonly Entity _sampleTask;
        private readonly KeyFactory _keyFactory;
        private readonly DateTime _includedDate =
            new DateTime(1999, 12, 31, 0, 0, 0, DateTimeKind.Utc);
        private readonly DateTime _startDate =
            new DateTime(1998, 4, 18, 0, 0, 0, DateTimeKind.Utc);
        private readonly DateTime _endDate =
            new DateTime(2013, 4, 18, 0, 0, 0, DateTimeKind.Utc);
        private readonly int _retryCount = 3;
        private readonly int _retryDelayMs = 500;
        private readonly RetryRobot _retryRobot = new RetryRobot()
        {
            RetryWhenExceptions = new[] { typeof(Xunit.Sdk.XunitException) },
            // Eventually consistency can take a long time.
            MaxTryCount = 8
        };

        public DatastoreTest()
        {
            _projectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
            _db = DatastoreDb.Create(_projectId, TestUtil.RandomName());
            _keyFactory = _db.CreateKeyFactory("Task");
            _sampleTask = new Entity()
            {
                Key = _keyFactory.CreateKey("sampleTask"),
            };
        }

        public void Dispose()
        {
            ClearTasks();
        }

        /// <summary>
        /// Retry action.
        /// Datastore guarantees only eventual consistency.  Many tests write
        /// an entity and then query it afterward, but may not find it immediately.
        /// </summary>
        /// <param name="action"></param>
        private void Eventually(Action action) => _retryRobot.Eventually(action);


        private bool IsValidKey(Key key)
        {
            foreach (var element in key.Path)
            {
                if (element.Id == 0 && string.IsNullOrEmpty(element.Name))
                    return false;
                if (string.IsNullOrEmpty(element.Kind))
                    return false;
            }
            return true;
        }

        [Fact]
        public void TestIncompleteKey()
        {
            // [START datastore_incomplete_key]
            Key incompleteKey = _db.CreateKeyFactory("Task").CreateIncompleteKey();
            Key key = _db.AllocateId(incompleteKey);
            // [END datastore_incomplete_key]
            Assert.True(IsValidKey(key));
        }

        [Fact]
        public void TestNamedKey()
        {
            // [START datastore_named_key]
            Key key = _db.CreateKeyFactory("Task").CreateKey("sampleTask");
            // [END datastore_named_key]
            Assert.True(IsValidKey(key));
        }

        [Fact]
        public void TestKeyWithParent()
        {
            // [START datastore_key_with_parent]
            Key rootKey = _db.CreateKeyFactory("TaskList").CreateKey("default");
            Key key = new KeyFactory(rootKey, "Task").CreateKey("sampleTask");
            // [END datastore_key_with_parent]
            Assert.True(IsValidKey(key));
        }

        [Fact]
        public void TestKeyWithMultilevelParent()
        {
            // [START datastore_key_with_multilevel_parent]
            Key rootKey = _db.CreateKeyFactory("User").CreateKey("Alice");
            Key taskListKey = new KeyFactory(rootKey, "TaskList").CreateKey("default");
            Key key = new KeyFactory(taskListKey, "Task").CreateKey("sampleTask");
            // [END datastore_key_with_multilevel_parent]
            Assert.True(IsValidKey(key));
        }

        private void AssertValidEntity(Entity original)
        {
            _db.Upsert(original);
            Assert.Equal(original, _db.Lookup(original.Key));
        }

        [Fact]
        public void TestEntityWithParent()
        {
            // [START datastore_entity_with_parent]
            Key taskListKey = _db.CreateKeyFactory("TaskList").CreateKey(TestUtil.RandomName());
            Key taskKey = new KeyFactory(taskListKey, "Task").CreateKey("sampleTask");
            Entity task = new Entity()
            {
                Key = taskKey,
                ["category"] = "Personal",
                ["done"] = false,
                ["priority"] = 4,
                ["description"] = "Learn Cloud Datastore"
            };
            // [END datastore_entity_with_parent]
            AssertValidEntity(task);
        }

        [Fact]
        public void TestProperties()
        {
            // [START datastore_properties]
            Entity task = new Entity()
            {
                Key = _db.CreateKeyFactory("Task").CreateKey("sampleTask"),
                ["category"] = "Personal",
                ["created"] = new DateTime(1999, 01, 01, 0, 0, 0, DateTimeKind.Utc),
                ["done"] = false,
                ["priority"] = 4,
                ["percent_complete"] = 10.0,
                ["description"] = new Value()
                {
                    StringValue = "Learn Cloud Datastore",
                    ExcludeFromIndexes = true
                },
            };
            // [END datastore_properties]
            AssertValidEntity(task);
        }

        [Fact]
        public void TestArrayValue()
        {
            // [START datastore_array_value]
            Entity task = new Entity()
            {
                Key = _db.CreateKeyFactory("Task").CreateKey("sampleTask"),
                ["collaborators"] = new ArrayValue() { Values = { "alice", "bob" } },
                ["tags"] = new ArrayValue() { Values = { "fun", "programming" } }
            };
            // [END datastore_array_value]
            AssertValidEntity(task);
        }

        [Fact]
        public void TestBasicEntity()
        {
            // [START datastore_basic_entity]
            Entity task = new Entity()
            {
                Key = _db.CreateKeyFactory("Task").CreateKey("sampleTask"),
                ["category"] = "Personal",
                ["done"] = false,
                ["priority"] = 4,
                ["description"] = "Learn Cloud Datastore"
            };
            // [END datastore_basic_entity]
            AssertValidEntity(task);
        }

        [Fact]
        public void TestUpsert()
        {
            // [START datastore_upsert]
            _db.Upsert(_sampleTask);
            // [END datastore_upsert]
            Assert.Equal(_sampleTask, _db.Lookup(_sampleTask.Key));
            // Make sure a second upsert doesn't throw an exception.
            _db.Upsert(_sampleTask);
        }

        [Fact]
        public void TestInsert()
        {
            // [START datastore_insert]
            Entity task = new Entity()
            {
                Key = _keyFactory.CreateIncompleteKey()
            };
            task.Key = _db.Insert(task);
            // [END datastore_insert]
            Assert.Equal(task, _db.Lookup(task.Key));
            // Make sure a second insert throws an exception.
            Grpc.Core.RpcException e = Assert.Throws<Grpc.Core.RpcException>(() =>
                _db.Insert(task));
        }

        [Fact]
        public void TestInsertCompleteKey()
        {
            Entity task = new Entity()
            {
                Key = _keyFactory.CreateKey(new Random().Next())
            };
            task.Key = _db.Insert(task);
            // This assertion should fail!
            Assert.Null(task.Key);
            // Instead, this assertion should pass:
            // Assert.Equal(task, _db.Lookup(task.Key));
        }

        [Fact]
        public void TestLookup()
        {
            _db.Upsert(_sampleTask);
            // [START datastore_lookup]
            Entity task = _db.Lookup(_sampleTask.Key);
            // [END datastore_lookup]
            Assert.Equal(_sampleTask, task);
        }


        [Fact]
        public void TestUpdate()
        {
            _db.Upsert(_sampleTask);
            // [START datastore_update]
            _sampleTask["priority"] = 5;
            _db.Update(_sampleTask);
            // [END datastore_update]
            Assert.Equal(_sampleTask, _db.Lookup(_sampleTask.Key));
        }

        [Fact]
        public void TestDelete()
        {
            _db.Upsert(_sampleTask);
            // [START datastore_delete]
            _db.Delete(_sampleTask.Key);
            // [END datastore_delete]
            Assert.Null(_db.Lookup(_sampleTask.Key));
        }

        private Entity[] UpsertBatch(Key taskKey1, Key taskKey2)
        {
            var taskList = new[]
            {
                new Entity()
                {
                    Key = taskKey1,
                    ["category"] = "Personal",
                    ["done"] = false,
                    ["priority"] = 4,
                    ["description"] = "Learn Cloud Datastore"
                },
                new Entity()
                {
                    Key = taskKey2,
                    ["category"] = "Personal",
                    ["done"] = "false",
                    ["priority"] = 5,
                    ["description"] = "Integrate Cloud Datastore"
                }
            };
            _db.Upsert(taskList);
            return taskList;
        }

        [Fact]
        public void TestBatchUpsert()
        {
            // [START datastore_batch_upsert]
            var taskList = new[]
            {
                new Entity()
                {
                    Key = _keyFactory.CreateIncompleteKey(),
                    ["category"] = "Personal",
                    ["done"] = false,
                    ["priority"] = 4,
                    ["description"] = "Learn Cloud Datastore"
                },
                new Entity()
                {
                    Key = _keyFactory.CreateIncompleteKey(),
                    ["category"] = "Personal",
                    ["done"] = "false",
                    ["priority"] = 5,
                    ["description"] = "Integrate Cloud Datastore"
                }
            };
            var keyList = _db.Upsert(taskList[0], taskList[1]);
            // [END datastore_batch_upsert]
            taskList[0].Key = keyList[0];
            taskList[1].Key = keyList[1];
            Assert.Equal(taskList[0], _db.Lookup(keyList[0]));
            Assert.Equal(taskList[1], _db.Lookup(keyList[1]));
        }

        [Fact]
        public void TestBatchLookup()
        {
            // [START datastore_batch_lookup]
            var keys = new Key[] { _keyFactory.CreateKey(1), _keyFactory.CreateKey(2) };
            // [END datastore_batch_lookup]
            var expectedTasks = UpsertBatch(keys[0], keys[1]);
            // [START datastore_batch_lookup]
            var tasks = _db.Lookup(keys[0], keys[1]);
            // [END datastore_batch_lookup]
            Assert.Equal(expectedTasks[0], tasks[0]);
            Assert.Equal(expectedTasks[1], tasks[1]);
        }

        [Fact]
        public void TestBatchDelete()
        {
            // [START datastore_batch_delete]
            var keys = new Key[] { _keyFactory.CreateKey(1), _keyFactory.CreateKey(2) };
            // [END datastore_batch_delete]
            UpsertBatch(keys[0], keys[1]);
            var lookups = _db.Lookup(keys);
            Assert.NotNull(lookups[0]);
            Assert.NotNull(lookups[1]);
            // [START datastore_batch_delete]
            _db.Delete(keys);
            // [END datastore_batch_delete]
            lookups = _db.Lookup(keys[0], keys[1]);
            Assert.Null(lookups[0]);
            Assert.Null(lookups[1]);
        }

        private void ClearTasks()
        {
            var deadEntities = _db.RunQuery(new Query("Task"));
            _db.Delete(deadEntities.Entities);
        }

        private string UpsertTaskList()
        {
            string taskListKeyName = TestUtil.RandomName();
            Key taskListKey = _db.CreateKeyFactory("TaskList").CreateKey(taskListKeyName);
            Key taskKey = new KeyFactory(taskListKey, "Task").CreateKey("someTask");
            Entity task = new Entity()
            {
                Key = taskKey,
                ["category"] = "Personal",
                ["done"] = false,
                ["completed"] = false,
                ["priority"] = 4,
                ["created"] = _includedDate,
                ["percent_complete"] = 10.0,
                ["description"] = new Value()
                {
                    StringValue = "Learn Cloud Datastore",
                    ExcludeFromIndexes = true
                },
                ["tag"] = new ArrayValue() { Values = { "fun", "l", "programming" } }
            };
            _db.Upsert(task);
            // Datastore is, after all, eventually consistent.
            System.Threading.Thread.Sleep(1000);
            return taskListKeyName;
        }

        private static bool IsEmpty(DatastoreQueryResults results) =>
            results.Entities.Count == 0;

        [Fact(Skip = "https://github.com/GoogleCloudPlatform/google-cloud-dotnet/issues/304")]
        public void TestBasicQuery()
        {
            UpsertTaskList();
            // [START datastore_basic_query]
            Query query = new Query("Task")
            {
                Filter = Filter.And(Filter.Equal("done", false),
                    Filter.GreaterThanOrEqual("priority", 4)),
                Order = { { "priority", PropertyOrder.Types.Direction.Descending } }
            };
            // [END datastore_basic_query]
            Eventually(() => Assert.False(IsEmpty(_db.RunQuery(query))));
        }

        [Fact]
        public void TestRunQuery()
        {
            UpsertTaskList();
            Eventually(() =>
            {
                // [START datastore_run_query]
                Query query = new Query("Task");
                DatastoreQueryResults tasks = _db.RunQuery(query);
                // [END datastore_run_query]
                Assert.False(IsEmpty(tasks));
            });
        }

        [Fact]
        public void TestPropertyFilter()
        {
            UpsertTaskList();
            Eventually(() =>
            {
                // [START datastore_property_filter]
                Query query = new Query("Task")
                {
                    Filter = Filter.Equal("done", false)
                };
                // [END datastore_property_filter]
                var tasks = _db.RunQuery(query);
                Assert.False(IsEmpty(tasks));
            });
        }

        [Fact]
        public void TestCompositeFilter()
        {
            UpsertTaskList();
            // [START datastore_composite_filter]
            Query query = new Query("Task")
            {
                Filter = Filter.And(Filter.Equal("done", false),
                    Filter.Equal("priority", 4)),
            };
            // [END datastore_composite_filter]
            Eventually(() => Assert.False(IsEmpty(_db.RunQuery(query))));
        }

        [Fact]
        public void TestKeyFilter()
        {
            UpsertTaskList();
            // [START datastore_key_filter]
            Query query = new Query("Task")
            {
                Filter = Filter.GreaterThan("__key__", _keyFactory.CreateKey("aTask"))
            };
            // [END datastore_key_filter]
            Eventually(() => Assert.False(IsEmpty(_db.RunQuery(query))));
        }

        [Fact]
        public void TestAscendingSort()
        {
            UpsertTaskList();
            // [START datastore_ascending_sort]
            Query query = new Query("Task")
            {
                Order = { { "created", PropertyOrder.Types.Direction.Ascending } }
            };
            // [END datastore_ascending_sort]
            Eventually(() => Assert.False(IsEmpty(_db.RunQuery(query))));
        }

        [Fact]
        public void TestDescendingSort()
        {
            UpsertTaskList();
            // [START datastore_descending_sort]
            Query query = new Query("Task")
            {
                Order = { { "created", PropertyOrder.Types.Direction.Descending } }
            };
            // [END datastore_descending_sort]
            Eventually(() => Assert.False(IsEmpty(_db.RunQuery(query))));
        }

        [Fact(Skip = "https://github.com/GoogleCloudPlatform/google-cloud-dotnet/issues/304")]
        public void TestMultiSort()
        {
            UpsertTaskList();
            // [START datastore_multi_sort]
            Query query = new Query("Task")
            {
                Order = { { "priority", PropertyOrder.Types.Direction.Descending },
                    { "created", PropertyOrder.Types.Direction.Ascending } }
            };
            // [END datastore_multi_sort]
            Eventually(() => Assert.False(IsEmpty(_db.RunQuery(query))));
        }

        [Fact]
        public void TestKindlessQuery()
        {
            UpsertTaskList();
            // [START datastore_kindless_query]
            Query query = new Query()
            {
                Filter = Filter.GreaterThan("__key__",
                    _keyFactory.CreateKey("aTask"))
            };
            // [END datastore_kindless_query]
            Eventually(() => Assert.False(IsEmpty(_db.RunQuery(query))));
        }

        [Fact]
        public void TestAncestorQuery()
        {
            string keyName = UpsertTaskList();
            // [START datastore_ancestor_query]
            Query query = new Query("Task")
            {
                Filter = Filter.HasAncestor(_db.CreateKeyFactory("TaskList")
                    .CreateKey(keyName))
            };
            // [END datastore_ancestor_query]
            Eventually(() => Assert.False(IsEmpty(_db.RunQuery(query))));
        }

        [Fact(Skip = "https://github.com/GoogleCloudPlatform/google-cloud-dotnet/issues/304")]
        public void TestProjectionQuery()
        {
            UpsertTaskList();
            // [START datastore_projection_query]
            Query query = new Query("Task")
            {
                Projection = { "priority", "percent_complete" }
            };
            // [END datastore_projection_query]
            Eventually(() => Assert.False(IsEmpty(_db.RunQuery(query))));
        }

        [Fact(Skip = "https://github.com/GoogleCloudPlatform/google-cloud-dotnet/issues/304")]
        public void TestRunProjectionQuery()
        {
            UpsertTaskList();
            // [START datastore_run_query_projection]
            Query query = new Query("Task")
            {
                Projection = { "priority", "percent_complete" }
            };
            List<long> priorities = new List<long>();
            List<double> percentCompletes = new List<double>();
            foreach (var entity in _db.RunQuery(query).Entities)
            {
                priorities.Add((long)entity["priority"]);
                percentCompletes.Add((double)entity["percent_complete"]);
            }
            // [END datastore_run_query_projection]
            Eventually(() =>
            {
                Assert.Equal(new long[] { 4L }, priorities.ToArray());
                Assert.Equal(new double[] { 10.0 }, percentCompletes.ToArray());
            });
        }

        [Fact]
        public void TestKeysOnlyQuery()
        {
            UpsertTaskList();
            // [START datastore_keys_only_query]
            Query query = new Query("Task")
            {
                Projection = { "__key__" }
            };
            // [END datastore_keys_only_query]
            Eventually(() =>
            {
                foreach (Entity task in _db.RunQuery(query).Entities)
                {
                    Assert.False(string.IsNullOrEmpty(task.Key.Path[0].Name));
                    Assert.Empty(task.Properties);
                    break;
                }
            });
        }

        [Fact]
        public void TestNamespaceRunQuery()
        {
            UpsertTaskList();
            Eventually(() =>
            {
                // [START datastore_namespace_run_query]
                KeyFactory keyFactory = _db.CreateKeyFactory("__namespace__");
                // List all the namespaces between a lower and upper bound.
                string lowerBound = _db.NamespaceId.Substring(0,
                    _db.NamespaceId.Length - 1);
                string upperBound = _db.NamespaceId + "z";
                Key startNamespace = keyFactory.CreateKey(lowerBound);
                Key endNamespace = keyFactory.CreateKey(upperBound);
                Query query = new Query("__namespace__")
                {
                    Filter = Filter.And(
                        Filter.GreaterThan("__key__", startNamespace),
                        Filter.LessThan("__key__", endNamespace))
                };
                var namespaces = new List<string>();
                foreach (Entity entity in _db.RunQuery(query).Entities)
                {
                    namespaces.Add(entity.Key.Path[0].Name);
                };
                // [END datastore_namespace_run_query]
                Assert.Contains(_db.NamespaceId, namespaces.ToArray());
            });
        }

        [Fact]
        public void TestKindRunQuery()
        {
            UpsertTaskList();
            Eventually(() =>
            {
                // [START datastore_kind_run_query]
                Query query = new Query("__kind__");
                var kinds = new List<string>();
                foreach (Entity entity in _db.RunQuery(query).Entities)
                {
                    kinds.Add(entity.Key.Path[0].Name);
                };
                // [END datastore_kind_run_query]
                Assert.Contains("Task", kinds);
            });
        }

        [Fact]
        public void TestPropertyRunQuery()
        {
            UpsertTaskList();
            Eventually(() =>
            {
                // [START datastore_property_run_query]
                Query query = new Query("__property__");
                var properties = new List<string>();
                foreach (Entity entity in _db.RunQuery(query).Entities)
                {
                    string kind = entity.Key.Path[0].Name;
                    string property = entity.Key.Path[1].Name;
                    if (kind == "Task")
                        properties.Add(property);
                };
                // [END datastore_property_run_query]
                properties.Sort();
                Assert.Equal(new[] { "category", "completed", "created",
                    "done", "percent_complete", "priority", "tag" },
                    properties.ToArray());
            });
        }

        [Fact]
        public void TestPropertyByKindRunQuery()
        {
            ClearTasks();
            UpsertTaskList();
            Eventually(() =>
            {
                // [START datastore_property_by_kind_run_query]
                Key key = _db.CreateKeyFactory("__kind__").CreateKey("Task");
                Query query = new Query("__property__")
                {
                    Filter = Filter.HasAncestor(key)
                };
                var properties = new List<string>();
                foreach (Entity entity in _db.RunQuery(query).Entities)
                {
                    string kind = entity.Key.Path[0].Name;
                    string property = entity.Key.Path[1].Name;
                    var representations = entity["property_representation"]
                        .ArrayValue.Values.Select(x => x.StringValue)
                        .OrderBy(x => x);
                    properties.Add($"{property}:" +
                        string.Join(",", representations));
                };
                // [END datastore_property_by_kind_run_query]
                properties.Sort();
                Assert.Equal(new[] {
                    "category:STRING",
                    "completed:BOOLEAN",
                    "created:INT64",
                    "done:BOOLEAN",
                    "percent_complete:DOUBLE",
                    "priority:INT64",
                    "tag:STRING" },
                    properties.ToArray());
            });
        }

        [Fact]
        public void TestPropertyFilteringRunQuery()
        {
            UpsertTaskList();
            Eventually(() =>
            {
                // [START datastore_property_filtering_run_query]
                Key key = _db.CreateKeyFactory("__kind__").CreateKey("Task");
                Key startKey = new KeyFactory(key, "__property__")
                    .CreateKey("priority");
                Query query = new Query("__property__")
                {
                    Filter = Filter.GreaterThanOrEqual("__key__", startKey)
                };
                var properties = new List<string>();
                foreach (Entity entity in _db.RunQuery(query).Entities)
                {
                    string kind = entity.Key.Path[0].Name;
                    string property = entity.Key.Path[1].Name;
                    properties.Add($"{kind}.{property}");
                };
                // [END datastore_property_filtering_run_query]
                properties.Sort();
                Assert.NotEmpty(properties);
            });
        }

        [Fact(Skip = "https://github.com/GoogleCloudPlatform/google-cloud-dotnet/issues/346")]
        public void TestDistinctOnQuery()
        {
            UpsertTaskList();
            Eventually(() =>
            {
                // [START datastore_distinct_on_query]
                Query query = new Query("Task")
                {
                    Projection = { "category", "priority" },
                    DistinctOn = { "category" },
                    Order = {
                        { "category", PropertyOrder.Types.Direction.Ascending},
                        {"priority", PropertyOrder.Types.Direction.Ascending }
                    }
                };
                // [END datastore_distinct_on_query]
                Assert.False(IsEmpty(_db.RunQuery(query)));
            });
        }

        [Fact]
        public void TestArrayValueInequalityRange()
        {
            UpsertTaskList();
            // [START datastore_array_value_inequality_range]
            Query query = new Query("Task")
            {
                Filter = Filter.And(Filter.GreaterThan("tag", "learn"),
                    Filter.LessThan("tag", "math"))
            };
            // [END datastore_array_value_inequality_range]
            Eventually(() => Assert.True(IsEmpty(_db.RunQuery(query))));
        }

        [Fact]
        public void TestArrayValueEquality()
        {
            UpsertTaskList();
            // [START datastore_array_value_equality]
            Query query = new Query("Task")
            {
                Filter = Filter.And(Filter.Equal("tag", "fun"),
                    Filter.Equal("tag", "programming"))
            };
            // [END datastore_array_value_equality]
            Eventually(() => Assert.False(IsEmpty(_db.RunQuery(query))));
        }

        [Fact]
        public void TestInequalityRange()
        {
            UpsertTaskList();
            // [START datastore_inequality_range]
            Query query = new Query("Task")
            {
                Filter = Filter.And(Filter.GreaterThan("created", _startDate),
                    Filter.LessThan("created", _endDate))
            };
            // [END datastore_inequality_range]
            Eventually(() => Assert.False(IsEmpty(_db.RunQuery(query))));
        }

        [Fact]
        public void TestInequalityInvalid()
        {
            UpsertTaskList();
            // [START datastore_inequality_invalid]
            Query query = new Query("Task")
            {
                Filter = Filter.And(Filter.GreaterThan("created", _startDate),
                    Filter.GreaterThan("priority", 3))
            };
            // [END datastore_inequality_invalid]
            Exception e = Assert.Throws<Grpc.Core.RpcException>(() =>
                IsEmpty(_db.RunQuery(query)));
        }

        [Fact(Skip = "https://github.com/GoogleCloudPlatform/google-cloud-dotnet/issues/304")]
        public void TestEqualAndInequalityRange()
        {
            UpsertTaskList();
            // [START datastore_equal_and_inequality_range]
            Query query = new Query("Task")
            {
                Filter = Filter.And(Filter.Equal("priority", 4),
                    Filter.GreaterThan("created", _startDate),
                    Filter.LessThan("created", _endDate))
            };
            // [END datastore_equal_and_inequality_range]
            Eventually(() => Assert.False(IsEmpty(_db.RunQuery(query))));
        }

        [Fact(Skip = "https://github.com/GoogleCloudPlatform/google-cloud-dotnet/issues/304")]
        public void TestInequalitySort()
        {
            UpsertTaskList();
            // [START datastore_inequality_sort]
            Query query = new Query("Task")
            {
                Filter = Filter.GreaterThan("priority", 3),
                Order = { { "priority", PropertyOrder.Types.Direction.Ascending},
                    {"created", PropertyOrder.Types.Direction.Ascending } }
            };
            // [END datastore_inequality_sort]
            Eventually(() => Assert.False(IsEmpty(_db.RunQuery(query))));
        }

        [Fact]
        public void TestInequalitySortInvalidNotSame()
        {
            UpsertTaskList();
            // [START datastore_inequality_sort_invalid_not_same]
            Query query = new Query("Task")
            {
                Filter = Filter.GreaterThan("priority", 3),
                Order = { { "created", PropertyOrder.Types.Direction.Ascending } }
            };
            // [END datastore_inequality_sort_invalid_not_same]
            Exception e = Assert.Throws<Grpc.Core.RpcException>(() =>
                IsEmpty(_db.RunQuery(query)));
        }

        [Fact]
        public void TestInequalitySortInvalidNotFirst()
        {
            UpsertTaskList();
            // [START datastore_inequality_sort_invalid_not_first]
            Query query = new Query("Task")
            {
                Filter = Filter.GreaterThan("priority", 3),
                Order = { {"created", PropertyOrder.Types.Direction.Ascending },
                    { "priority", PropertyOrder.Types.Direction.Ascending} }
            };
            // [END datastore_inequality_sort_invalid_not_first]
            Exception e = Assert.Throws<Grpc.Core.RpcException>(() =>
                IsEmpty(_db.RunQuery(query)));
        }

        [Fact]
        public void TestLimit()
        {
            UpsertTaskList();
            // [START datastore_limit]
            Query query = new Query("Task")
            {
                Limit = 5,
            };
            // [END datastore_limit]
            Eventually(() => Assert.InRange(_db.RunQuery(query).Entities.Count(), 1, 5));
        }

        [Fact]
        public void TestCursorPaging()
        {
            UpsertTaskList();
            _db.Upsert(_sampleTask);
            Eventually(() =>
            {
                var pageOneCursor = CursorPaging(1, null);
                Assert.NotNull(pageOneCursor);
                var pageTwoCursor = CursorPaging(1, pageOneCursor);
                Assert.NotNull(pageTwoCursor);
                Assert.NotEqual(pageOneCursor, pageTwoCursor);
            });
        }

        private string CursorPaging(int pageSize, string pageCursor)
        {
            // [START datastore_cursor_paging]
            Query query = new Query("Task")
            {
                Limit = pageSize,
            };
            if (!string.IsNullOrEmpty(pageCursor))
                query.StartCursor = ByteString.FromBase64(pageCursor);

            return _db.RunQuery(query).EndCursor?.ToBase64();
            // [END datastore_cursor_paging]
        }

        private IReadOnlyList<Key> UpsertBalances()
        {
            KeyFactory keyFactory = _db.CreateKeyFactory("People");
            Entity from = new Entity()
            {
                Key = keyFactory.CreateKey("from"),
                ["balance"] = 100
            };
            Entity to = new Entity()
            {
                Key = keyFactory.CreateKey("to"),
                ["balance"] = 0
            };
            var keys = _db.Upsert(from, to);
            // TODO: return keys; when following bug is fixed:
            // https://github.com/GoogleCloudPlatform/google-cloud-dotnet/issues/308
            return new[] { from.Key, to.Key };
        }

        // [START datastore_transactional_update]
        private void TransferFunds(Key fromKey, Key toKey, long amount)
        {
            using (var transaction = _db.BeginTransaction())
            {
                var entities = transaction.Lookup(fromKey, toKey);
                entities[0]["balance"].IntegerValue -= amount;
                entities[1]["balance"].IntegerValue += amount;
                transaction.Update(entities);
                transaction.Commit();
            }
        }
        // [END datastore_transactional_update]

        private void TransferFunds(Key fromKey, Key toKey, long amount,
            DatastoreTransaction transaction)
        {
            var entities = transaction.Lookup(fromKey, toKey);
            entities[0]["balance"].IntegerValue -= amount;
            entities[1]["balance"].IntegerValue += amount;
            transaction.Update(entities);
        }

        [Fact]
        public void TestTransactionalUpdate()
        {
            var keys = UpsertBalances();
            using (var transaction = _db.BeginTransaction())
            {
                TransferFunds(keys[0], keys[1], 10, transaction);
                transaction.Commit();
            }
            var entities = _db.Lookup(keys);
            Assert.Equal(90, entities[0]["balance"]);
            Assert.Equal(10, entities[1]["balance"]);
        }

        [Fact]
        public void TestConflictingTransactionalUpdate()
        {
            var keys = UpsertBalances();
            using (var transaction = _db.BeginTransaction())
            {
                TransferFunds(keys[0], keys[1], 10, transaction);
                TransferFunds(keys[1], keys[0], 5);
                Exception e = Assert.Throws<Grpc.Core.RpcException>(() =>
                    transaction.Commit());
            }
        }

        // [START datastore_transactional_retry]
        /// <summary>
        /// Retry the action when a Grpc.Core.RpcException is thrown.
        /// </summary>
        private T RetryRpc<T>(Func<T> action)
        {
            List<Grpc.Core.RpcException> exceptions = null;
            var delayMs = _retryDelayMs;
            for (int tryCount = 0; tryCount < _retryCount; ++tryCount)
            {
                try
                {
                    return action();
                }
                catch (Grpc.Core.RpcException e)
                {
                    if (exceptions == null)
                        exceptions = new List<Grpc.Core.RpcException>();
                    exceptions.Add(e);
                }
                System.Threading.Thread.Sleep(delayMs);
                delayMs *= 2;  // Exponential back-off.
            }
            throw new AggregateException(exceptions);
        }

        private void RetryRpc(Action action)
        {
            RetryRpc(() => { action(); return 0; });
        }

        [Fact]
        public void TestTransactionalRetry()
        {
            int tryCount = 0;
            var keys = UpsertBalances();
            RetryRpc(() =>
            {
                using (var transaction = _db.BeginTransaction())
                {
                    TransferFunds(keys[0], keys[1], 10, transaction);
                    // Insert a conflicting transaction on the first try.
                    if (tryCount++ == 0)
                        TransferFunds(keys[1], keys[0], 5);
                    transaction.Commit();
                }
            });
            Assert.Equal(2, tryCount);
        }
        // [END datastore_transactional_retry]

        [Fact]
        public void TestTransactionalGetOrCreate()
        {
            // [START datastore_transactional_get_or_create]
            Entity task;
            using (var transaction = _db.BeginTransaction())
            {
                task = transaction.Lookup(_sampleTask.Key);
                if (task == null)
                {
                    transaction.Insert(_sampleTask);
                    transaction.Commit();
                }
            }
            // [END datastore_transactional_get_or_create]
            Assert.Equal(_sampleTask, _db.Lookup(_sampleTask.Key));
        }

        [Fact]
        public void TestTransactionalSingleEntityGroupReadOnly()
        {
            string keyName = UpsertTaskList();
            Key taskListKey = _db.CreateKeyFactory("TaskList")
                .CreateKey(keyName);
            Entity taskListEntity = new Entity() { Key = taskListKey };
            _db.Upsert(taskListEntity);
            // [START datastore_transactional_single_entity_group_read_only]
            Entity taskList;
            IReadOnlyList<Entity> tasks;
            using (var transaction = _db.BeginTransaction(TransactionOptions.CreateReadOnly()))
            {
                taskList = transaction.Lookup(taskListKey);
                var query = new Query("Task")
                {
                    Filter = Filter.HasAncestor(taskListKey)
                };
                tasks = transaction.RunQuery(query).Entities;
                transaction.Commit();
            }
            // [END datastore_transactional_single_entity_group_read_only]
            Assert.Equal(taskListEntity, taskList);
            Assert.Collection(tasks, task => { });
        }

        [Fact]
        public void TestEventualConsistentQuery()
        {
            string keyName = UpsertTaskList();
            Eventually(() =>
            {
                // [START datastore_eventual_consistent_query]
                Query query = new Query("Task")
                {
                    Filter = Filter.HasAncestor(_db.CreateKeyFactory("TaskList")
                        .CreateKey(keyName))
                };
                var results = _db.RunQuery(query,
                    ReadOptions.Types.ReadConsistency.Eventual);
                // [END datastore_eventual_consistent_query]
                Assert.False(IsEmpty(results));
            });
        }

        [Fact]
        public void TestUnindexedPropertyQuery()
        {
            UpsertTaskList();
            // [START datastore_unindexed_property_query]
            Query query = new Query("Task")
            {
                Filter = Filter.Equal("description", "Learn Cloud Datastore")
            };
            // [END datastore_unindexed_property_query]
            Eventually(() =>
            {
                var tasks = _db.RunQuery(query).Entities;
                Assert.True(IsEmpty(_db.RunQuery(query)));
            });
        }

        [Fact]
        public void TestExplodingProperties()
        {
            // [START datastore_exploding_properties]
            Entity task = new Entity()
            {
                Key = _db.CreateKeyFactory("Task").CreateKey("sampleTask"),
                ["tags"] = new ArrayValue() { Values = { "fun", "programming", "learn" } },
                ["collaborators"] = new ArrayValue() { Values = { "alice", "bob", "charlie" } },
                ["created"] = DateTime.UtcNow
            };
            // [END datastore_exploding_properties]
            // Avoid test failure due to float rounding differences.
            task["created"] = new DateTime(2016, 8, 12, 9, 0, 0, DateTimeKind.Utc);
            AssertValidEntity(task);
        }
    }
}
