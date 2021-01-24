namespace DataQuery.Net
{
  public interface IDataQueryRepository
  {
    DataQueryResult Query(DataQueryCollections config, DataQueryFilterParams filter);
  }
}
