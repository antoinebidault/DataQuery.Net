using System.Threading.Tasks;

namespace DataQuery.Net
{
    public class DefaultDataQuery : IDataQuery
    {
        private IDataQueryProvider _provider;
        private readonly IDataQueryRepository _repo;
        private DataQuerySchema _schema;
        public DefaultDataQuery(IDataQueryRepository repo, IDataQueryProvider provider)
        {
            _provider = provider;
            _repo = repo;
        }
        public async Task<DataQueryResult> QueryAsync(DataQueryFilterParams filter)
        {
            return await _repo.QueryAsync(_provider.Provide(), filter);
        }

        public DataQuerySchemaExposed GetSchemaExposed()
        {
            return new DataQuerySchemaExposed(GetSchema());
        }

        public DataQuerySchema GetSchema()
        {
            if (_schema != null)
            {
                return _schema;
            }

            _schema = _provider.Provide();
            return _schema;
        }
    }


}
