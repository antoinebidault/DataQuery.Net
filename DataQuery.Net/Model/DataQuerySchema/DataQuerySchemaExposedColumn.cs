using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;

namespace DataQuery.Net
{
    public class DataQuerySchemaExposedColumn
    {
        public DataQuerySchemaExposedColumn(Dimension col)
        {
            this.Id = col.Alias;
            this.Name = col.Name;
            this.DisplayName = col.DisplayName;
            this.Description = col.Description;
            this.SqlType = col.SqlType;
            this.Type = GetType(col.PropertyType);
            this.EnumType = col.PropertyType.IsEnum ? col.PropertyType.Name : null;
            this.Group = col.Group;
            this.Unit = col.Unit;
            this.Color = col.Color;
            this.MetaDatas = col.MetaDatas;
            this.PropertyType = col.PropertyType;
            this.Values = col.Values;
        }

       
        public string Id { get; set; }
        public string Name { get; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public SqlDbType? SqlType { get; }
        public string Group { get; set; }
        public string Unit { get; }
        public string Type { get; set; }
        public string EnumType { get; }
        public string Color { get; }
        public IDictionary<string, object> MetaDatas { get; }

        [JsonIgnore]
        public Type PropertyType { get; set; }

        public IEnumerable<ColumnValue> Values { get; }

        private string GetType(Type type)
        {
            type = Nullable.GetUnderlyingType(type) ?? type;

            if (type == typeof(string))
            {
                return "string";
            }
            else if (type.IsEnum)
            {
                return "enum";
            }
            else if (type == typeof(int)
             || type == typeof(double)
             || type == typeof(float)
             || type == typeof(decimal))
            {
                return "number";
            }
            else if (type == typeof(DateTime))
            {
                return "datetime";
            }
            else if (type == typeof(Guid))
            {
                return "guid";
            }
            return "string";
        }

    }
}
