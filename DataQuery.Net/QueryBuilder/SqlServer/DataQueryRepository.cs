using DataQuery.Net.Extensions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;

namespace DataQuery.Net
{

    public class DataQuerySqlServerRepository : IDataQueryRepository
    {
        private IMemoryCache _cache;
        private DataQueryOptions _options;

        public DataQuerySqlServerRepository(DataQueryOptions options , IMemoryCache cache)
        {
            this._cache = cache;
            this._options = options;
        }

        public DataQueryResult Query(DataQueryCollections config, DataQueryFilterParams filter)
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

        private DataQueryResult QueryBase(DataQueryCollections config, DataQueryFilterParams filterParams)
        {
            var builder = new DataQuerySqlServerBuilder(_options);
            return builder.Query(config, filterParams);
        }
    }
}
