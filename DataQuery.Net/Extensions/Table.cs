using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataQuery.Net.Extensions
{
    public static class TableExtensions
    {
        public static IEnumerable<Table> GetConnectedTables(this Table table, IEnumerable<Table> tables)
        {
            var connectedTables = new List<Table>();
            foreach (var col in table.Columns)
            {
                if (col.SqlJoins != null && col.SqlJoins.Count > 0)
                {
                    foreach (var sql in col.SqlJoins)
                    {
                        var found = tables.FirstOrDefault(m => m.Alias == sql.Key);
                        if (found != null)
                        {
                            connectedTables.Add(found);
                        }
                    }

                }
            }

            return connectedTables;

        }
    }
}
