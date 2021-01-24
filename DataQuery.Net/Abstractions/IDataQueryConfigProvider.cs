namespace DataQuery.Net
{
    /// <summary>
    /// Configuration du DataQuery branché à l'entrepôt de données
    /// Définition des tables
    /// </summary>
    public interface IDataQueryConfigProvider
    {
        /// <summary>
        /// Return configuration
        /// </summary>
        DataQueryConfig Get();
    }
}
