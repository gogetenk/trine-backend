using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Assistance.Operational.Dal.MongoImpl.Entities;
using AutoMapper;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using Sogetrel.Sinapse.Framework.Dal.MongoDb;
using Sogetrel.Sinapse.Framework.Dal.MongoDb.Repositories;
using Sogetrel.Sinapse.Framework.Exceptions;

namespace Assistance.Operational.Dal.Impl.Repositories
{
    public abstract class CrudRepositoryBase<T> : RepositoryBase where T : MongoEntityBase
    {
        protected CrudRepositoryBase(IMongoDbContext mongoDbContext, ILogger<RepositoryBase> logger, IMapper mapper) : base(mongoDbContext, logger, mapper)
        {
        }

        /// <summary>
        /// Create entity
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual T Insert(T entity)
        {
            var collection = GetCollection<T>();
            collection.InsertOne(entity);
            return entity;
        }

        /// <summary>
        /// Get an entity from a predicate
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual List<T> FindBy(Expression<Func<T, bool>> predicate)
        {
            var collection = GetCollection<T>();
            var results = collection
                .AsQueryable()
                .Where(predicate)
                .ToList();

            return Mapper.Map<List<T>>(results);
        }

        /// <summary>
        /// Get an entity from its id
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual T GetById(string id)
        {
            if (!ObjectId.TryParse(id, out ObjectId objectId))
                throw new DalException("The object id is not well formated");

            var collection = GetCollection<T>();
            var results = collection
                .AsQueryable()
                .Where(x => x.Id == objectId)
                .FirstOrDefault();

            return Mapper.Map<T>(results);
        }

        /// <summary>
        /// Get an entity from a predicate
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual List<T> GetByIds(List<string> ids)
        {
            var objectIds = ids.Select(x => new ObjectId(x)).ToList();
            var collection = GetCollection<T>();
            var results = collection
                .AsQueryable()
                .Where(x => objectIds.Contains(x.Id))
                .ToList();

            return Mapper.Map<List<T>>(results);
        }

        /// <summary>
        /// Update entity
        /// </summary>
        /// <param name="entity"></param>
        public virtual bool Update(string id, T entity)
        {

            var collection = GetCollection<T>();
            if (!ObjectId.TryParse(id, out ObjectId objectId))
                throw new DalException("The object id is not well formated");

            if (!FindBy(x => x.Id == objectId).Any())
                return false;

            entity.Id = objectId;

            var result = collection.ReplaceOne<T>(x => x.Id == objectId, entity);
            if (result == null)
                return false;

            return result.IsAcknowledged;
        }

        /// <summary>
        /// Delete entity
        /// </summary>
        /// <param name="entityId"></param>
        public virtual void Delete(string entityId)
        {
            var collection = GetCollection<T>();

            var builder = Builders<T>.Filter;
            var filter = builder.Eq(x => x.Id, new ObjectId(entityId));

            collection.DeleteOne(filter);
        }
    }

}
