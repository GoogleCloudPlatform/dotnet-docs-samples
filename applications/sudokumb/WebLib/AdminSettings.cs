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

using System;
using System.Threading.Tasks;
using Google.Cloud.Datastore.V1;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace Sudokumb
{
    public static class AdminSettingsExtensions
    {
        public static IServiceCollection AddAdminSettings(
            this IServiceCollection services)
        {
            services.AddSingleton<AdminSettings>();
            services.AddSingleton<IDumb, AdminSettings>(
                provider => provider.GetService<AdminSettings>()
            );
            return services;
        }
    }

    /// <summary>
    /// Stores settings readable by everyone, but should only be set by
    /// administrators.
    /// </summary>
    public class AdminSettings : Sudokumb.IDumb
    {
        /// <summary>
        /// Settings get stored in datastore.
        /// </summary>
        readonly DatastoreDb datastore_;
        /// <summary>
        /// The key to the one entity that contains all the settings.
        /// </summary>
        readonly Key key_;

        // Cache the datastore entity so we don't query datastore a million
        // times, which would slow us down.  Performance optimization.
        object cachedEntityLock_ = new object();
        Task<Entity> cachedEntity_;
        DateTime cachedEntityExpires_;

        public AdminSettings(DatastoreDb datastore)
        {
            cachedEntityExpires_ = DateTime.MinValue;
            datastore_ = datastore;
            key_ = new KeyFactory(datastore.ProjectId, datastore.NamespaceId,
                ENTITY_KIND).CreateKey(1);
        }

        const string ENTITY_KIND = "AdminSettings",
            DUMB_EXPIRES = "dumbExpires";

        /// <summary>
        /// Dumb means every next possible move on the sudoku board gets
        /// its own pub/sub message.
        /// </summary>
        public async Task<bool> IsDumbAsync()
        {
            var dumbExpires = await GetDumbExpiresAsync();
            return !dumbExpires.HasValue ? false :
                dumbExpires > DateTime.UtcNow;
        }

        Task<Entity> LookupEntityAsync()
        {
            lock(cachedEntityLock_)
            {
                var now = DateTime.Now;
                if (now > cachedEntityExpires_)
                {
                    cachedEntityExpires_ = now.AddSeconds(10);
                    cachedEntity_ = datastore_.LookupAsync(key_);
                }
                return cachedEntity_;
            }
        }

        public Task SetDumbExpiresAsync(DateTime when)
        {
            Entity entity = new Entity()
            {
                Key = key_,
                [DUMB_EXPIRES] = when
            };
            lock(cachedEntityLock_)
            {
                cachedEntity_ = Task.FromResult(entity);
                cachedEntityExpires_ = DateTime.Now.AddSeconds(10);
            }
            return datastore_.UpsertAsync(entity);
        }

        public async Task<DateTime?> GetDumbExpiresAsync()
        {
            var entity = await LookupEntityAsync();
            if (null == entity)
            {
                return null;
            }
            return (DateTime?) entity[DUMB_EXPIRES];
        }
    }
}
