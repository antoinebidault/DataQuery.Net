namespace DataQuery.Net
{
  public class DataQuerySqlDataMapper : IDataQueryDataMapper
  {
    public DataQuerySqlDataMapper()
    {
    }

    /// <summary>
    /// Build an ad hoc query based on dimensions, filters provided in the params
    /// </summary>
    /// <param name="conf"></param>
    /// <param name="param"></param>
    /// <returns></returns>
    public QueryResult Query(DataQueryConfig conf, DataQueryFilterParam param)
    {
      var builder = new DataQuerySqlServerBuilder();
      return builder.Query(conf, param);
    }

  }

}
