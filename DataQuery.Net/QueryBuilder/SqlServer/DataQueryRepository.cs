using DataQuery.Net.Extensions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;

namespace DataQuery.Net
{

    public class DataQuerySqlServerRepository : IDataQueryRepository
    {
        private readonly IMemoryCache _cache;
        private readonly DataQueryOptions _options;

        public DataQuerySqlServerRepository(DataQueryOptions options , IMemoryCache cache)
        {
            this._cache = cache;
            this._options = options;
        }

        public DataQueryResult Query(DataQuerySchema config, DataQueryFilterParams filter)
        {
            string uniqueKey = _options.CacheKeyPrefix + "_" + (JsonConvert.SerializeObject(filter)).ToGuid().ToString();

            if (_options.CacheEnabled)
            {
                return _cache.GetOrCreate(uniqueKey, (e) =>
                {
                    e.AbsoluteExpiration = DateTime.Now.AddSeconds(_options.CacheDuration);
                    return QueryBase(config, filter);
                });
            }

            return QueryBase(config, filter);
        }

        private DataQueryResult QueryBase(DataQuerySchema config, DataQueryFilterParams filterParams)
        {
            var builder = new DataQuerySqlServerBuilder(_options);

            // Transform input parameted to clean dataQueryFilter object
            var filters = filterParams.Parse(config);

            var query = builder.Query(config, filters);

            query.Filter = filterParams;

            return query;
        }
    }
}
