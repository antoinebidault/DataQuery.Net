namespace DataQuery.Net
{
  public interface IDataQueryDataMapper
  {
    QueryResult Query(DataQueryConfig config, DataQueryFilterParam param);
  }

}
