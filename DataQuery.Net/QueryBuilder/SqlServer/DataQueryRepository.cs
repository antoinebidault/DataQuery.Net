using DataQuery.Net.Extensions;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace DataQuery.Net
{

    public class DataQuerySqlServerRepository : IDataQueryRepository
    {
        private readonly IMemoryCache _cache;
        private readonly IDistributedCache _distributed;
        private readonly DataQueryOptions _options;

        public DataQuerySqlServerRepository(DataQueryOptions options, IMemoryCache cache, IDistributedCache distributed)
        {
            this._cache = cache;
            this._distributed = distributed;
            this._options = options;
        }

        public async Task<DataQueryResult> QueryAsync(DataQuerySchema config, DataQueryFilterParams filter)
        {
            string uniqueKey = _options.CacheKeyPrefix + "_" + (JsonConvert.SerializeObject(filter)).ToGuid().ToString();

            if (_options.CacheEnabled)
            {
                if (_options.DistributedCache)
                {
                    var result = _distributed.GetString(uniqueKey);
                    if (!string.IsNullOrEmpty(result))
                    {
                        return JsonConvert.DeserializeObject<DataQueryResult>(result);
                    }

                   var data = await QueryBaseAsync(config, filter);

                    _distributed.SetString(uniqueKey,JsonConvert.SerializeObject(data, Formatting.None), new DistributedCacheEntryOptions()
                    {
                         AbsoluteExpiration = DateTime.Now.AddSeconds(_options.CacheDuration)
                    });
                }
                else
                {
                    return await _cache.GetOrCreateAsync(uniqueKey, async (e) =>
                    {
                        e.AbsoluteExpiration = DateTime.Now.AddSeconds(_options.CacheDuration);
                        return await QueryBaseAsync(config, filter);
                    });
                }
            }

            return await QueryBaseAsync(config, filter);
        }

        private async Task<DataQueryResult> QueryBaseAsync(DataQuerySchema config, DataQueryFilterParams filterParams)
        {
            var builder = new DataQuerySqlServerBuilder(_options);

            // Transform input parameted to clean dataQueryFilter object
            var filters = filterParams.Parse(config);

            var query = await builder.QueryAsync(config, filters);

            query.Filter = filterParams;

            return query;
        }
    }
}
