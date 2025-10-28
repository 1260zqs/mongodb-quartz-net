using System;
using System.Threading.Tasks;
using MongoDB.Driver;
using Quartz.Spi.MongoDbJobStore.Models;
using Quartz.Spi.MongoDbJobStore.Models.Id;

namespace Quartz.Spi.MongoDbJobStore.Repositories
{
    [CollectionName("locks")]
    internal class LockRepository : BaseRepository<Lock>
    {
        public LockRepository(IMongoDatabase database, string instanceName, string collectionPrefix = null)
            : base(database, instanceName, collectionPrefix)
        {
        }

        public async Task<bool> TryAcquireLock(LockType lockType, string instanceId)
        {
            var lockId = new LockId(lockType, InstanceName);
            Log.Verbose("Trying to acquire lock {0} on {1}", lockId, instanceId);
            try
            {
                await Collection.InsertOneAsync(new Lock
                {
                    Id = lockId,
                    InstanceId = instanceId,
                    AquiredAt = DateTime.Now
                }).ConfigureAwait(false);
                Log.Verbose("Acquired lock {0} on {1}", lockId, instanceId);
                return true;
            }
            catch (MongoWriteException)
            {
                Log.Verbose("Failed to acquire lock {0} on {1}", lockId, instanceId);
                return false;
            }
        }

        public async Task<bool> ReleaseLock(LockType lockType, string instanceId)
        {
            var lockId = new LockId(lockType, InstanceName);
            Log.Verbose("Releasing lock {0} on {1}", lockId, instanceId);
            var result =
                await Collection.DeleteOneAsync(
                    FilterBuilder.Where(@lock => @lock.Id == lockId && @lock.InstanceId == instanceId)).ConfigureAwait(false);
            if (result.DeletedCount > 0)
            {
                Log.Verbose("Released lock {0} on {1}", lockId, instanceId);
                return true;
            }
            Log.Warning("Failed to release lock {0} on {1}. You do not own the lock.", lockId, instanceId);
            return false;
        }

        public override async Task EnsureIndex()
        {
            await Collection.Indexes.CreateOneAsync(IndexBuilder.Ascending(@lock => @lock.AquiredAt),
                new CreateIndexOptions() { ExpireAfter = TimeSpan.FromSeconds(30) }).ConfigureAwait(false);
        }
    }
}