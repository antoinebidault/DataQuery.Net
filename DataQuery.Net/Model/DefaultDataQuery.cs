using System.Threading.Tasks;

namespace DataQuery.Net
{
    public class DefaultDataQuery : IDataQuery
    {
        private IDataQueryProvider _provider;
        private readonly IDataQueryRepository _repo;

        public DefaultDataQuery(IDataQueryRepository repo, IDataQueryProvider provider)
        {
            _provider = provider;
            _repo = repo;
        }
        public async Task<DataQueryResult> QueryAsync(DataQueryFilterParams filter)
        {
            return await _repo.QueryAsync(_provider.Provide(), filter);
        }

        public DataQuerySchemaExposed GetSchema()
        {
            return new DataQuerySchemaExposed(_provider.Provide());
        }
    }


}
