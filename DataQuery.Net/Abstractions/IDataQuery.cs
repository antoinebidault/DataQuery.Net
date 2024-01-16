using System.Threading.Tasks;

namespace DataQuery.Net
{
    /// <summary>
    /// Provides a single interface to query data using the DataQueryFilterParams
    /// </summary>
    public interface IDataQuery
    {
        Task<DataQueryResult> QueryAsync(DataQueryFilterParams filter);
        DataQuerySchemaExposed GetSchema();
    }
}
