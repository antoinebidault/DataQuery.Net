using DataQuery.Net.Extensions;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System;

namespace DataQuery.Net
{

    public class DataQuerySqlServerRepository : IDataQueryRepository
    {
        private IDataQueryDataMapper mapper;
        private IMemoryCache _cache;
        public DataQuerySqlServerRepository(IDataQueryDataMapper mapper, IMemoryCache cache)
        {
            this.mapper = mapper;
            this._cache = cache;
        }

        public QueryResult Query(DataQueryConfig config, DataQueryFilterParam filter)
        {
            return mapper.Query(config, filter);
        }
        public QueryResult QueryFromCache(DataQueryConfig config, DataQueryFilterParam filter)
        {
            string uniqueKey = (JsonConvert.SerializeObject(filter) + config.UniqueKey).ToGuid().ToString();

            if (config.CacheEnabled)
            {
                return _cache.GetOrCreate(string.Format("DataQuery_{0}", uniqueKey), (e) =>
                {
                    e.AbsoluteExpiration = DateTime.Now.AddSeconds(config.CacheDuration);
                    return mapper.Query(config, filter);
                });
            }

            return mapper.Query(config, filter);
        }
    }
}
