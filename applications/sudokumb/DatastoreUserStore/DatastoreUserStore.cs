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
using static Google.Cloud.Datastore.V1.Key.Types;
using Grpc.Core;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;

namespace Sudokumb
{
    public class DatastoreUserStore<U> : IUserPasswordStore<U>, IUserRoleStore<U>, IUserStore<U>
        where U : IdentityUser<string>, new()
    {
        readonly DatastoreDb _datastore;
        readonly KeyFactory _userKeyFactory;
        readonly KeyFactory _nnindexKeyFactory;
        readonly Microsoft.Extensions.Logging.ILogger _logger;

        // Additional properties we store for each User instance
        // in a ConditionalWeakTable. 
        public class UserAddendum
        {
            public string NormalizedUserName { get; set; }
            public List<string> Roles { get; set; } = new List<string>();
        };

        static readonly ConditionalWeakTable<U, UserAddendum> s_userAddendums
            = new ConditionalWeakTable<U, UserAddendum>();

        const string
            USER_KIND = "webuser",
            NORMALIZED_EMAIL = "normalized-email",
            NORMALIZED_NAME = "normalized-name",
            USER_NAME = "user-name",
            CONCURRENCY_STAMP = "concurrency-stamp",
            PASSWORD_HASH = "password-hash",
            ROLES = "roles",
            NORMALIZED_NAME_INDEX_KIND = "webuser-nnindex",
            USER_KEY = "key";

        public DatastoreUserStore(DatastoreDb datastore,
            ILogger<DatastoreUserStore<U>> logger)
        {
            _datastore = datastore;
            _userKeyFactory = new KeyFactory(_datastore.ProjectId,
                _datastore.NamespaceId, USER_KIND);
            _nnindexKeyFactory = new KeyFactory(_datastore.ProjectId,
                _datastore.NamespaceId, NORMALIZED_NAME_INDEX_KIND);
            _logger = logger;
        }

        Key KeyFromUserId(string userId) => _userKeyFactory.CreateKey(userId);

        Entity UserToEntity(U user)
        {
            var entity = new Entity()
            {
                [NORMALIZED_EMAIL] = user.NormalizedEmail,
                [NORMALIZED_NAME] = user.NormalizedUserName,
                [USER_NAME] = user.UserName,
                [CONCURRENCY_STAMP] = user.ConcurrencyStamp,
                [PASSWORD_HASH] = user.PasswordHash,
                [ROLES] = s_userAddendums.GetOrCreateValue(user).Roles.ToArray(),
                Key = KeyFromUserId(user.Id)
            };
            entity[CONCURRENCY_STAMP].ExcludeFromIndexes = true;
            entity[PASSWORD_HASH].ExcludeFromIndexes = true;
            // Normalized name has its own separate index.
            entity[NORMALIZED_NAME].ExcludeFromIndexes = true;
            return entity;
        }

        U EntityToUser(Entity entity)
        {
            if (null == entity)
            {
                return null;
            }
            U user = new U()
            {
                Id = entity.Key.Path.First().Name,
                NormalizedUserName = (string)entity[NORMALIZED_NAME],
                NormalizedEmail = (string)entity[NORMALIZED_EMAIL],
                UserName = (string)entity[USER_NAME],
                PasswordHash = (string)entity[PASSWORD_HASH],
                ConcurrencyStamp = (string)entity[CONCURRENCY_STAMP],
            };
            var addendum = s_userAddendums.GetOrCreateValue(user);
            var roles = (string[])entity[ROLES];
            if (roles != null && roles.Count() > 0)
            {
                addendum.Roles.AddRange(roles);
            }
            addendum.NormalizedUserName = user.NormalizedUserName;
            return user;
        }

        public async Task<IdentityResult> CreateAsync(U user,
            CancellationToken cancellationToken)
        {
            _logger.LogDebug("CreateAsync({0})", user.NormalizedEmail);
            user.Id = Guid.NewGuid().ToString();
            var entity = UserToEntity(user);
            Entity indexEntity = new Entity()
            {
                Key = _nnindexKeyFactory.CreateKey(user.NormalizedUserName),
                [USER_KEY] = entity.Key
            };
            indexEntity[USER_KEY].ExcludeFromIndexes = true;
            var result = await InTransactionAsync(
                cancellationToken, async (transaction, callSettings) =>
            {
                transaction.Insert(new[] { entity, indexEntity });
                await transaction.CommitAsync(callSettings);
            });
            if (result.Succeeded)
            {
                s_userAddendums.GetOrCreateValue(user).NormalizedUserName
                    = user.NormalizedUserName;
            }
            return result;
        }

        public Task<IdentityResult> DeleteAsync(U user,
            CancellationToken cancellationToken)
        {
            _logger.LogDebug("DeleteAsync({0})", user.NormalizedEmail);
            return InTransactionAsync(
                cancellationToken, async (transaction, callSettings) =>
            {
                transaction.Delete(new[] { KeyFromUserId(user.Id),
                    _nnindexKeyFactory.CreateKey(user.NormalizedUserName) });
                await transaction.CommitAsync(callSettings);
            });
        }

        public async Task<U> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            _logger.LogDebug("FindByIdAsync({0})", userId);
            return EntityToUser(await _datastore.LookupAsync(KeyFromUserId(userId),
                callSettings: CallSettings.FromCancellationToken(cancellationToken)));
        }

        public async Task<U> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            _logger.LogDebug("FindByNameAsync({0})", normalizedUserName);
            try
            {
                CallSettings callSettings =
                    CallSettings.FromCancellationToken(cancellationToken);
                using (var transaction = await _datastore.BeginTransactionAsync(
                    callSettings))
                {
                    var indexEntity = await transaction.LookupAsync(
                        _nnindexKeyFactory.CreateKey(normalizedUserName),
                        callSettings);
                    if (null == indexEntity)
                    {
                        return null;
                    }
                    return EntityToUser(await transaction.LookupAsync(
                        (Key)indexEntity[USER_KEY], callSettings));
                }
            }
            catch (Grpc.Core.RpcException e)
            when (e.Status.StatusCode == StatusCode.NotFound)
            {
                return null;
            }
        }

        public Task<string> GetNormalizedUserNameAsync(U user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.NormalizedUserName);
        }

        public Task<string> GetUserIdAsync(U user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Id == null ? null : user.Id.ToString());
        }

        public Task<string> GetUserNameAsync(U user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.UserName);
        }

        public Task SetNormalizedUserNameAsync(U user, string normalizedName, CancellationToken cancellationToken)
        {
            user.NormalizedUserName = normalizedName;
            return Task.CompletedTask;
        }

        public Task SetUserNameAsync(U user, string userName, CancellationToken cancellationToken)
        {
            user.UserName = userName;
            return Task.CompletedTask;
        }
        void IDisposable.Dispose()
        {
        }

        public async Task<IdentityResult> UpdateAsync(U user,
            CancellationToken cancellationToken)
        {
            _logger.LogDebug("UpdateAsync({0})", user.NormalizedEmail);
            // Was the NormalizedUserName modified?
            UserAddendum addendum = s_userAddendums.GetOrCreateValue(user);
            if (user.NormalizedUserName == addendum.NormalizedUserName)
            {
                // NormalizedUserName was not modified.  The common and efficient case.
                return await Rpc.TranslateExceptionsAsync(() =>
                    _datastore.UpsertAsync(UserToEntity(user),
                    CallSettings.FromCancellationToken(cancellationToken)));
            }
            Entity entity = UserToEntity(user);
            Entity indexEntity = new Entity()
            {
                Key = _nnindexKeyFactory.CreateKey(user.NormalizedUserName),
                [USER_KEY] = entity.Key
            };
            var result = await InTransactionAsync(cancellationToken,
                async (transaction, callSettings) =>
            {
                // NormalizedUserName was modified.  Have to update the
                // index too.
                if (!string.IsNullOrEmpty(addendum.NormalizedUserName))
                {
                    transaction.Delete(_nnindexKeyFactory
                        .CreateKey(addendum.NormalizedUserName));
                }
                indexEntity[USER_KEY].ExcludeFromIndexes = true;
                transaction.Upsert(entity);
                transaction.Upsert(indexEntity);
                await transaction.CommitAsync(callSettings);
            });
            if (result.Succeeded)
            {
                addendum.NormalizedUserName = user.NormalizedUserName;
            }
            return result;
        }

        public Task AddToRoleAsync(U user, string roleName, CancellationToken cancellationToken)
        {
            UserAddendum addendum = s_userAddendums.GetOrCreateValue(user);
            addendum.Roles.Add(roleName);
            return Task.CompletedTask;
        }

        public Task RemoveFromRoleAsync(U user, string roleName, CancellationToken cancellationToken)
        {
            UserAddendum addendum = s_userAddendums.GetOrCreateValue(user);
            addendum.Roles.Remove(roleName);
            return Task.CompletedTask;
        }

        public Task<IList<string>> GetRolesAsync(U user, CancellationToken cancellationToken)
        {
            UserAddendum addendum = s_userAddendums.GetOrCreateValue(user);
            return Task.FromResult((IList<string>)addendum.Roles);
        }

        public Task<bool> IsInRoleAsync(U user, string roleName, CancellationToken cancellationToken)
        {
            UserAddendum addendum = s_userAddendums.GetOrCreateValue(user);
            return Task.FromResult(addendum.Roles.Contains(roleName));
        }

        public async Task<IList<U>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
        {
            var result = await _datastore.RunQueryAsync(new Query(USER_KIND)
            {
                Filter = Filter.Equal(ROLES, roleName)
            });
            return result.Entities.Select(e => EntityToUser(e)).ToList();
        }

        public Task SetPasswordHashAsync(U user, string passwordHash, CancellationToken cancellationToken)
        {
            user.PasswordHash = passwordHash;
            return Task.CompletedTask;
        }

        public Task<string> GetPasswordHashAsync(U user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PasswordHash);
        }

        public Task<bool> HasPasswordAsync(U user, CancellationToken cancellationToken)
        {
            return Task.FromResult(!string.IsNullOrWhiteSpace(user.PasswordHash));
        }

        async Task<IdentityResult> InTransactionAsync(

            CancellationToken cancellationToken,
            Func<DatastoreTransaction, CallSettings, Task> f)
        {
            try
            {
                CallSettings callSettings =
                    CallSettings.FromCancellationToken(cancellationToken);
                using (var transaction = await _datastore.BeginTransactionAsync(
                    callSettings))
                {
                    await f(transaction, callSettings);
                }
                return IdentityResult.Success;
            }
            catch (Grpc.Core.RpcException e)
            {
                return IdentityResult.Failed(new IdentityError()
                {
                    Code = e.Status.Detail,
                    Description = e.Message
                });
            }
        }
    }
}
