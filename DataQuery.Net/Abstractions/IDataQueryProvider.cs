namespace DataQuery.Net
{
    /// <summary>
    /// Provides the tables and props definitions
    /// </summary>
    public interface IDataQueryProvider
    {
        /// <summary>
        /// Return a data query collections
        /// </summary>
         DataQueryCollections Provide();
    }
}
