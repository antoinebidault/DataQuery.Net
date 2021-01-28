using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataQuery.Net.Extensions
{
    public static class DataQueryCollectionsExtensions
    {
        public static IEnumerable<Column> GetViewableProperties(this DataQuerySchema collections, List<string> tables = null)
        {
            return collections.Tables.Where(m => (tables == null || tables.Contains(m.Key)))
                .SelectMany(m => m.Value.Columns)
                .Where(m => m.AllowedToView && m.Displayed);
        }

        public static IEnumerable<Column> GetQueryableProperties(this DataQuerySchema collections, List<string> tables = null)
        {
            return collections.Tables
                .Where(m => (tables == null || tables.Contains(m.Key)))
                .SelectMany(m => m.Value.Columns)
                .Where(m => m.AllowedToFilter && m.Displayed);
        }
    }
}
