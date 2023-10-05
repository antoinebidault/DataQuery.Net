using System;

namespace DataQuery.Net.Attributes
{
    public class DataQueryTableAttribute : Attribute
    {
        public string SqlName { get; set; }
        public DataQueryTableAttribute(string sqlName = null, bool tableName = false)
        {
            this.SqlName = sqlName;
        }
    }
}
