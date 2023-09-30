using System;
using System.Collections.Generic;
using System.Text;

namespace DataQuery.Net.Attributes
{
    public class DataQueryColumnAttribute : Attribute
    {
        public string SqlName { get; set; }
        public DataQueryColumnAttribute(string sqlName = null, bool tableName = false)
        {
            this.SqlName = sqlName;
            // this.Implicit = impl;
        }
    }
}
