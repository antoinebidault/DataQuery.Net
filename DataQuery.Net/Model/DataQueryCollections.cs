using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataQuery.Net
{
    /// <summary>
    /// Returned informations
    /// </summary>
    public class DataQueryCollections
    {
        public DataQueryCollections()
        {
            Tables = new Dictionary<string, Table>(StringComparer.InvariantCultureIgnoreCase);
        }

        public Dictionary<string, Table> Tables { get; set; }

        public Dictionary<string, DatabaseProp> Metrics
        {
            get
            {
                var dic = new Dictionary<string, DatabaseProp>();
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
        public Dictionary<string, DatabaseProp> Dimensions
        {
            get
            {
                var dic = new Dictionary<string, DatabaseProp>();
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

        public Dictionary<string, DatabaseProp> MetricsAndDimensions
        {
            get
            {
                var dic = new Dictionary<string, DatabaseProp>();
                foreach (var table in Tables.Values)
                {
                    foreach (var prop in table.Props)
                    {
                        dic[prop.Alias] = prop;
                    }
                }
                return dic;
            }
        }

     

    }
}
