namespace DataQuery.Net
{
  public interface IDataQueryRepository
  {
    QueryResult Query(DataQueryConfig config, DataQueryFilterParam filter);
    QueryResult QueryFromCache(DataQueryConfig config, DataQueryFilterParam filter);
  }
}
