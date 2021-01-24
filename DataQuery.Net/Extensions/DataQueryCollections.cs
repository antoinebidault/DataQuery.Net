using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataQuery.Net.Extensions
{
    public static class DataQueryCollectionsExtensions
    {
        public static IEnumerable<DatabaseProp> GetReportableChamp(this DataQueryCollections collections, List<string> tables = null)
        {
            return collections.Tables.Where(m => (tables == null || tables.Contains(m.Key)))
                .SelectMany(m => m.Value.Props)
                .Where(m => m.AllowedToReport && m.Displayed);
        }

        public static IEnumerable<DatabaseProp> GetExportableChamps(this DataQueryCollections collections, List<string> tables = null)
        {
            return collections.Tables.Where(m => (tables == null || tables.Contains(m.Key)))
                .SelectMany(m => m.Value.Props)
                .Where(m => m.AllowedToExport && m.Displayed);
        }

        public static IEnumerable<DatabaseProp> GetQueryableChamps(this DataQueryCollections collections, List<string> tables = null)
        {
            return collections.Tables
                .Where(m => (tables == null || tables.Contains(m.Key)))
                .SelectMany(m => m.Value.Props)
                .Where(m => m.AllowedToRequest && m.Displayed);
        }
    }
}
