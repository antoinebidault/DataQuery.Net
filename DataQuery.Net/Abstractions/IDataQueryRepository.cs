namespace DataQuery.Net
{
  public interface IDataQueryRepository
  {
    DataQueryResult Query(DataQuerySchema config, DataQueryFilterParams filter);
  }
}
