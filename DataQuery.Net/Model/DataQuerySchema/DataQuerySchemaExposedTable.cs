using System.Collections.Generic;

namespace DataQuery.Net
{
    public class DataQuerySchemaExposedTable
    {
        public string Id { get; set; }
        public bool Root { get; set; }
        public string Name { get; set; }
        public IEnumerable<DataQuerySchemaExposedColumn> Metrics { get; set; }
        public IEnumerable<DataQuerySchemaExposedColumn> Dimensions { get; set; }
        public IEnumerable<string> Children { get; set; }
        public string DisplayName { get; internal set; }
    }
}
