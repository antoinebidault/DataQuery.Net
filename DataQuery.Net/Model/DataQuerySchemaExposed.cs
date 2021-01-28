using System.Collections.Generic;
using System.Linq;

namespace DataQuery.Net
{
    public class DataQuerySchemaExposed
    {
        public DataQuerySchemaExposed(DataQuerySchema schema)
        {
            this.Dimensions = schema.Dimensions.Select(m => new DataQuerySchemaExposedColumn(m.Value));
            this.Metrics = schema.Metrics.Select(m => new DataQuerySchemaExposedColumn(m.Value));
        }

        public IEnumerable<DataQuerySchemaExposedColumn> Metrics { get; set; }
        public IEnumerable<DataQuerySchemaExposedColumn> Dimensions { get; set; }

    }

    public class DataQuerySchemaExposedColumn
    {
        public DataQuerySchemaExposedColumn(Column col)
        {
            this.Id = col.Alias;
            this.Label = col.Label;
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
