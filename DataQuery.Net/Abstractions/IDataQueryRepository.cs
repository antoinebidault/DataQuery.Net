using System.Threading.Tasks;

namespace DataQuery.Net
{
    public interface IDataQueryRepository
    {
        Task<DataQueryResult> QueryAsync(DataQuerySchema config, DataQueryFilterParams filter);
    }
}
