﻿namespace DataQuery.Net
{
    public class DefaultDataQuery : IDataQuery
    {
        private IDataQueryProvider _provider;
        private IDataQueryRepository _repo;

        public DefaultDataQuery(IDataQueryRepository repo, IDataQueryProvider provider)
        {
            _provider = provider;
            _repo = repo;
        }
        public DataQueryResult Query(DataQueryFilterParams filter)
        {
            return _repo.Query(_provider.Provide(), filter);
        }
    }




}
