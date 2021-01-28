namespace DataQuery.Net
{
    /// <summary>
    /// Provides a single interface to query data using the DataQueryFilterParams
    /// </summary>
    public interface IDataQuery
    {
        DataQueryResult Query(DataQueryFilterParams filter);
    }
}
