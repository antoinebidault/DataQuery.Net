using DataQuery.Net;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Extensions.DependencyInjection
{
    public class DataQueryOptions
    {
        /// <summary>
        /// Max size of a page
        /// </summary>
        public int MaxRecordsetSize { get; set; } = 1000;

        /// <summary>
        /// Connectionstring used to connect to database
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// Cache duration in seconds
        /// </summary>
        public double CacheDuration { get; set; }

        /// <summary>
        /// A unique key hash defined for caching purpose in DataQueryRepo
        /// </summary>
        public string CacheKeyPrefix { get; set; } = "dataquery_";

        /// <summary>
        /// Wether the cache is anabled
        /// </summary>
        public bool CacheEnabled { get; set; } = true;

        /// <summary>
        /// Enable distributed caching
        /// </summary>
        public bool DistributedCache { get; set; } = false;
        public string InputDateFormat { get; set; } = "dd/MM/yyyy";

        public static DataQueryOptions Defaults
        {
            get
            {
                return new DataQueryOptions()
                {
                    ConnectionString = ""
                };
            }
        }

        public bool IncludeQueryProfiling { get; set; } 
    }

}