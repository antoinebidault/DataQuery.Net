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
            /*
            this.Dimensions = schema.Dimensions
                .Where(m=>m.Value.Displayed)
                .Select(m => new DataQuerySchemaExposedColumn(m.Value));

            this.Metrics = schema.Metrics
                .Where(m => m.Value.Displayed)
                .Select(m => new DataQuerySchemaExposedColumn(m.Value));
            */


            foreach (var table in schema.Tables)
            {
                if (table.Value.IsRoot)
                {
                    var dataTable = GetTable(table.Value, new List<string>(), schema.Tables.Values);
                    this.Tables.Add(dataTable);
                }
            }

        }

        private DataQuerySchemaExposedTable GetTable(Table table, IList<string> tableToExcludeInChild, IEnumerable<Table> tables)
        {
            tableToExcludeInChild.Add(table.Name);
            return new DataQuerySchemaExposedTable()
            {
                Id = table.Name,
                Root = table.IsRoot,
                Name = table.Name,
                Children = table.GetConnectedTables(tables).Where(m => !tableToExcludeInChild.Contains(m.Name)).Select(m => GetTable(m, tableToExcludeInChild, tables)),
                Dimensions = table.Columns.Where(m => !m.IsMetric && m.Displayed).Select(m => new DataQuerySchemaExposedColumn(m)),
                Metrics = table.Columns.Where(m => m.IsMetric && m.Displayed).Select(m => new DataQuerySchemaExposedColumn(m))
            };

        }

        public IList<DataQuerySchemaExposedTable> Tables { get; set; }

    }

    public class DataQuerySchemaExposedTable
    {
        public string Id { get; set; }
        public bool Root { get; set; }
        public string Name { get; set; }
        public IEnumerable<DataQuerySchemaExposedColumn> Metrics { get; set; }
        public IEnumerable<DataQuerySchemaExposedColumn> Dimensions { get; set; }
        public IEnumerable<DataQuerySchemaExposedTable> Children { get; set; }
        public string DisplayName { get; internal set; }
    }

    public class DataQuerySchemaExposedColumn
    {
        public DataQuerySchemaExposedColumn(Column col)
        {
            this.Id = col.Alias;
            this.Label = col.DisplayName;
            this.Description = col.Description;
            this.Type = col.SqlType.ToString();
            this.Group = col.Group;
            this.Unit = col.Unit;
            this.Color = col.Color;
            this.MetaDatas = col.MetaDatas;
        }

        public string Id { get; set; }
        public string Label { get; set; }
        public string Description { get; set; }
        public string Group { get; set; }
        public string Unit { get; }
        public string Type { get; set; }
        public string Color { get; }
        public IDictionary<string, object> MetaDatas { get; }
    }
}
