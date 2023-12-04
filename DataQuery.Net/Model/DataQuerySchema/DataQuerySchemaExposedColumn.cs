using System.Collections.Generic;

namespace DataQuery.Net
{
    public class DataQuerySchemaExposedColumn
    {
        public DataQuerySchemaExposedColumn(Column col, Table table)
        {
            this.Id = table.Name + "." + col.Alias;
            this.DisplayName = col.DisplayName;
            this.Description = col.Description;
            this.Type = col.SqlType.ToString();
            this.Group = col.Group;
            this.Unit = col.Unit;
            this.Color = col.Color;
            this.MetaDatas = col.MetaDatas;
        }

        public string Id { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public string Group { get; set; }
        public string Unit { get; }
        public string Type { get; set; }
        public string Color { get; }
        public IDictionary<string, object> MetaDatas { get; }
    }
}
