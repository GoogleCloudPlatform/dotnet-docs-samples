// Copyright (c) 2018 Google LLC.
// 
// Licensed under the Apache License, Version 2.0 (the "License"); you may not
// use this file except in compliance with the License. You may obtain a copy of
// the License at
// 
// http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
// License for the specific language governing permissions and limitations under
// the License.

using Google.Cloud.Datastore.V1;
using Microsoft.AspNetCore.Identity;
using System;
using System.Threading;
using System.Threading.Tasks;
using Google.Api.Gax.Grpc;
using System.Linq;
using Microsoft.Extensions.Options;
using System.Collections.Generic;

namespace Sudokumb
{
    public class DatastoreRoleStore<R> : IRoleStore<R> where R : IdentityRole, new()
    {
        readonly         DatastoreDb _datastore;
        readonly         KeyFactory _roleKeyFactory;

        const string
            KIND = "webuserrole",            
            NORMALIZED_NAME = "normalized-name",            
            ROLE_NAME = "name",            
            CONCURRENCY_STAMP = "concurrency-stamp";

        public DatastoreRoleStore(DatastoreDb datastore)
        {
            _datastore = datastore;
            _roleKeyFactory = new KeyFactory(_datastore.ProjectId, _datastore.NamespaceId, KIND);
        }

        Key KeyFromRoleId(string roleId) => _roleKeyFactory.CreateKey(roleId);

        Entity RoleToEntity(R role)
        {
            var entity = new Entity()
            {
                [NORMALIZED_NAME] = role.NormalizedName,
                [ROLE_NAME] = role.Name,
                [CONCURRENCY_STAMP] = role.ConcurrencyStamp,
                Key = KeyFromRoleId(role.Id)
            };
            entity[CONCURRENCY_STAMP].ExcludeFromIndexes = true;
            return entity;
        }

        R EntityToRole(Entity entity)
        {
            if (null == entity)
            {
                return null;
            }
            R role = new R()
            {
                NormalizedName = (string)entity[NORMALIZED_NAME],
                Name = (string)entity[ROLE_NAME],
                ConcurrencyStamp = (string)entity[CONCURRENCY_STAMP]
            };
            return role;
        }
        public async Task<IdentityResult> CreateAsync(R role, CancellationToken cancellationToken)
        {
            return await Rpc.TranslateExceptionsAsync(() =>
                _datastore.InsertAsync(RoleToEntity(role), CallSettings.FromCancellationToken(cancellationToken)));
        }

        public async Task<IdentityResult> DeleteAsync(R role, CancellationToken cancellationToken)
        {
            return await Rpc.TranslateExceptionsAsync(() =>
                _datastore.DeleteAsync(KeyFromRoleId(role.Id), CallSettings.FromCancellationToken(cancellationToken)));
        }

        public void Dispose()
        {
        }

        public async Task<R> FindByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            return EntityToRole(await _datastore.LookupAsync(KeyFromRoleId(roleId),
                callSettings: CallSettings.FromCancellationToken(cancellationToken)));
        }

        public async Task<R> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            var result = await _datastore.RunQueryAsync(new Query(KIND)
            {
                Filter = Filter.Equal(NORMALIZED_NAME, normalizedRoleName)
            });
            return EntityToRole(result.Entities.FirstOrDefault());
        }

        public Task<string> GetNormalizedRoleNameAsync(R role, CancellationToken cancellationToken)
        {
            return Task.FromResult(role.NormalizedName);
        }

        public Task<string> GetRoleIdAsync(R role, CancellationToken cancellationToken)
        {
            return Task.FromResult(role.Id);
        }

        public Task<string> GetRoleNameAsync(R role, CancellationToken cancellationToken)
        {
            return Task.FromResult(role.Name);
        }

        public Task SetNormalizedRoleNameAsync(R role, string normalizedName, CancellationToken cancellationToken)
        {
            role.NormalizedName = normalizedName;
            return Task.CompletedTask;
        }

        public Task SetRoleNameAsync(R role, string roleName, CancellationToken cancellationToken)
        {
            role.Name = roleName;
            return Task.CompletedTask;
        }

        public async Task<IdentityResult> UpdateAsync(R role, CancellationToken cancellationToken)
        {
            return await Rpc.TranslateExceptionsAsync(() =>
                _datastore.UpsertAsync(RoleToEntity(role), CallSettings.FromCancellationToken(cancellationToken)));
        }
    }
}

