using DataQuery.Net.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataQuery.Net
{
    public static class TableExtensions
    {
        public static bool IsConnected(this IEnumerable<Table> tables, IEnumerable<Table> tablesConfigured)
        {
            if (!tables.Any())
            {
                throw new ArgumentException("A tables argument is required");
            }

            if (tables.Count() == 1)
            {
                return true;
            }

            var tableNames = tables.Select(m => m.Alias).ToList();

            var currentTable = tables.First();
            HashSet<string> tableProcessed = new HashSet<string>();

            IsConnectedRecursive(currentTable, tableNames, tablesConfigured, tableProcessed);

            return !tableNames.Any();
        }

        private static void IsConnectedRecursive(Table currentTable, List<string> tableNames, IEnumerable<Table> tablesConfigured, HashSet<string> tableProcessed)
        {

            foreach (var childTable in currentTable.GetConnectedTables(tablesConfigured))
            {
                if (tableNames.Contains(childTable.Alias))
                {
                    tableNames.Remove(childTable.Alias);
                }


                if (!tableProcessed.Contains(childTable.Alias))
                {
                    tableProcessed.Add(childTable.Alias);
                    IsConnectedRecursive(childTable, tableNames, tablesConfigured, tableProcessed);
                }
            }
        }


        public static IEnumerable<Table> GetConnectedTables(this Table table, IEnumerable<Table> tables)
        {
            var connectedTables = new List<Table>();

            if (table.NotDiscoverable)
            {
                return connectedTables;
            }

            foreach (var col in table.Columns)
            {
                if (col.SqlJoins != null && col.SqlJoins.Count > 0)
                {
                    foreach (var sql in col.SqlJoins)
                    {
                        var found = tables.FirstOrDefault(m => m.Alias == sql.Key);

                        if (found != null)
                        {
                            if (found.Implicit)
                            {
                                connectedTables.AddRange(GetConnectedTables(found, tables));
                            }
                            else
                            {
                                connectedTables.Add(found);
                            }
                        }
                    }

                }
            }

            return connectedTables;

        }


        public static void AddDimension(this Table table, Dimension column)
        {
            column.TableAlias = table.Alias;


            if (table.Columns.Any(m => m.Alias == column.Alias))
            {
                throw new DataQueryInvalidConfigException($"This dimension has already been added on the table {table.DisplayName}, field name : {column.Alias}");
            }
            table.Columns.Add(column);
        }

        public static void AddMetric(this Table table, Metric metric)
        {
            metric.TableAlias = table.Alias;
            metric.IsMetric = true;

            if (table.Columns.Any(m => m.Alias == metric.Alias))
            {
                throw new DataQueryInvalidConfigException($"This metric has already been added on the table {table.DisplayName}, field name : {metric.Alias}");
            }
            table.Columns.Add(metric);
        }


        public static void OneToManyJoin(this Table from, Table to, string columnTo = null, string columnFrom = "Id")
        {
            if (from.Alias == to.Alias)
            {
                throw new DataQueryJoinException($"OneToManyJoin Error : unable to self join a table");
            }

            if (columnTo == null)
            {
                columnTo = from.Alias + "Id";
            }

            var fromColumn = from.Columns.FirstOrDefault(m => m.Alias.Equals(from.Alias + "_" + columnFrom, StringComparison.InvariantCultureIgnoreCase));

            if (fromColumn == null)
            {
                throw new DataQueryJoinException($"OneToManyJoin Error : from column : {columnFrom} not found in {from.Alias}");
            }

            fromColumn.SqlJoins.Add(to.Alias, columnTo);


            var toColumn = to.Columns.FirstOrDefault(m => m.Alias.Equals(to.Alias + "_" + columnTo, StringComparison.InvariantCultureIgnoreCase));

            if (toColumn == null)
            {
                throw new DataQueryJoinException($"OneToManyJoin Error :Invalid to column : {columnTo} not found in {to.Alias}");
            }

            toColumn.SqlJoins.Add(from.Alias, columnFrom);
        }

        /*
        private static void ManyToManyJoin(this Table from, string columnFromn, string intermediateTable, Table to, string columnTo, string fromIntermediateTable = null, string toIntermediateTable = null)
        {
            from.Columns.FirstOrDefault(m => m.Name == columnFrom).SqlJoins.Add(to.Name, columnTo);
            to.Columns.FirstOrDefault(m => m.Name == columnTo).SqlJoins.Add(to.Name, columnFrom);
        }
        */
    }
}
