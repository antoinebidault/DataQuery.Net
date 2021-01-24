namespace DataQuery.Net
{
    /// <summary>
    /// Configuration du DataQuery branché à l'entrepôt de données
    /// Définition des tables
    /// </summary>
    public interface IDataQueryProvider
    {
        /// <summary>
        /// Return configuration
        /// </summary>
         DataQueryCollections Provide();
    }
}
