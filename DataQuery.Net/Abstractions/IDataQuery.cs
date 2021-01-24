namespace DataQuery.Net
{
    public interface IDataQuery
    {
        DataQueryResult Query(DataQueryFilterParams filter);
    }
}
