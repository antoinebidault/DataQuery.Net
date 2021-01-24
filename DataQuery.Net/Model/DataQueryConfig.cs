using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataQuery.Net
{

    /// <summary>
    /// Returned informations
    /// </summary>
    public class DataQueryConfig
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cnxReader">The plain connectionstring to the DataBase</param>
        public DataQueryConfig(string cnxReader)
        {
            this.ConnectionString = cnxReader;
            Tables = new Dictionary<string, Table>(StringComparer.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// A unique key hash defined for caching purpose in DataQueryRepo
        /// </summary>
        public string UniqueKey { get; set; }

        public bool CacheEnabled { get; set; }

        [JsonIgnore]
        internal string ConnectionString { get; set; }

        [JsonIgnore]
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

        [JsonIgnore]
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
        /// <summary>
        /// Cache duration in seconds
        /// </summary>
        public double CacheDuration { get; set; }

        public IEnumerable<DatabaseProp> GetReportableChamp(List<string> tables = null)
        {
            return Tables.Where(m => (tables == null || tables.Contains(m.Key))).SelectMany(m => m.Value.Props).Where(m => m.AllowedToReport && m.Aff);
        }
        public string GetReportableChampAsCommaSeparatedList(List<string> tables = null)
        {
            return string.Join(",", GetReportableChamp(tables).Select(m => m.Alias));
        }

        public IEnumerable<DatabaseProp> GetQueryableChamps(List<string> tables = null)
        {
            return Tables.Where(m => (tables == null || tables.Contains(m.Key))).SelectMany(m => m.Value.Props).Where(m => m.AllowedToRequest && m.Aff);
        }

        public string GetQueryableChampsAsCommaSeparatedList(List<string> tables = null)
        {
            return string.Join(",", GetQueryableChamps(tables).Select(m => m.Alias));
        }

        public IEnumerable<DatabaseProp> GetExportableChamps(List<string> tables = null)
        {
            return Tables.Where(m => (tables == null || tables.Contains(m.Key))).SelectMany(m => m.Value.Props).Where(m => m.AllowedToExport && m.Aff);
        }

        public string GetExportableChampsAsCommaSeparatedList(List<string> tables = null)
        {
            return string.Join(",", GetExportableChamps(tables).Select(m => m.Alias));
        }

        /// <summary>
        /// Build des stats
        /// Not more useful
        /// </summary>
        [Obsolete]
        public void Build()
        {
            /*
            foreach (var table in Tables.Values)
            {
              foreach (var prop in table.Props)
              {
                MetricsAndDimensions[prop.Alias] = prop;

                if (prop.IsMetric)
                  Metrics[prop.Alias] = prop;
                else
                  Dimensions[prop.Alias] = prop;
              }
            }*/
        }
    }
}
