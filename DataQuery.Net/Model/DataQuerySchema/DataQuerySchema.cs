using DataQuery.Net.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataQuery.Net
{


    /// <summary>
    /// Contains all informations about the database
    /// </summary>
    public class DataQuerySchema
    {
        public DataQuerySchema()
        {
            Tables = new Dictionary<string, Table>(StringComparer.InvariantCultureIgnoreCase);
        }

        public void AddTable(Table table)
        {
            // Check if table has not been already added
            if (this.Tables.ContainsKey(table.Alias))
            {
                throw new DataQueryInvalidConfigException($"Table ALIAS conflict : There is already a table with this alias : {table.Alias}. " +
                                                        $"You try to insert the '{table.DisplayName}' table, " +
                                                        $"conflicted table : '{Tables[table.Alias].DisplayName}'");
            }

            this.Tables.Add(table.Alias, table);
        }

        public void RemoveTable(string tableName)
        {
            this.Tables.Remove(tableName);
        }

        public Dictionary<string, Table> Tables { get; }


        public Dictionary<string, Column> Metrics
        {
            get
            {
                var dic = new Dictionary<string, Column>();
                foreach (var item in MetricsAndDimensions)
                {
                    if (item.Value.IsMetric)
                    {
                        dic[item.Key] = item.Value;
                    }
                }
                return dic;
            }
        }
        public Dictionary<string, Column> Dimensions
        {
            get
            {
                var dic = new Dictionary<string, Column>();
                foreach (var item in MetricsAndDimensions)
                {
                    if (!item.Value.IsMetric)
                    {
                        dic[item.Key] = item.Value;
                    }
                }
                return dic;
            }
        }

        public Dictionary<string, Column> MetricsAndDimensions
        {
            get
            {
                var dic = new Dictionary<string, Column>();
                foreach (var table in Tables.Values)
                {
                    foreach (var prop in table.Columns)
                    {
                        dic[prop.Alias] = prop;
                    }
                }
                return dic;
            }
        }
    }
}
