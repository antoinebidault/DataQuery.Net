using DataQuery.Net.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataQuery.Net
{
    public class DataQuerySchemaExposed
    {
        public DataQuerySchemaExposed(DataQuerySchema schema)
        {
            foreach (var table in schema.Tables.Where(m => m.Value.Root && !m.Value.Implicit).Select(m=>m.Value))
            {
                var dataTable = GetTable(table, new List<string>(), schema.Tables.Values);
                this.Tables.Add(dataTable);
            }

        }

        private DataQuerySchemaExposedTable GetTable(Table table, IList<string> tableToExcludeInChild, IEnumerable<Table> tables)
        {
            tableToExcludeInChild.Add(table.Name);
            return new DataQuerySchemaExposedTable()
            {
                Id = table.Name,
                Name = table.DisplayName,
                Root = table.Root,
                Children = table.GetConnectedTables(tables).Where(m => !tableToExcludeInChild.Contains(m.Name)).Select(m=>m.Name),
                Dimensions = table.Columns.Where(m => !m.IsMetric && m.Displayed).Select(m => new DataQuerySchemaExposedColumn(m, table)),
                Metrics = table.Columns.Where(m => m.IsMetric && m.Displayed).Select(m => new DataQuerySchemaExposedColumn(m, table))
            };
        }

        /// <summary>
        /// List of tables included in the model
        /// Every table contains a list of potential childs
        /// </summary>
        public IList<DataQuerySchemaExposedTable> Tables { get; set; } = new List<DataQuerySchemaExposedTable>();
    }
}
