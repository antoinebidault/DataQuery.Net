using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataQuery.Net
{

    /// <summary>
    /// Les informations récupérés du dataquery
    /// </summary>
    public class DataQueryResult
    {
        public DataQueryResult()
        {
        }

        public string SqlQuery { get; set; }
        public double Delay { get; set; }
        public DataQueryFilterParams Filter { get; set; }
        public DataQueryTable Data { get; set; }

    }

    /// <summary>
    /// A basic data column
    /// </summary>
    public class DataQueryColumn
    {
        public string Name { get; set; }
        public string Label { get; set; }
        public string Type { get; set; }
        public bool IsMetric { get; set; }
        public string Unite { get; set; }
        public string Color { get; set; }

    }


    /// <summary>
    /// The results returned by the data query
    /// </summary>
    public class DataQueryTable : IDisposable
    {
        public DataQueryTable()
        {
            this.Columns = new List<DataQuerySchemaExposedColumn>();
            this.Rows = new List<object[]>();
        }

        public int? PageIndex { get; set; }
        public int? PageSize { get; set; }
        public int PageCount
        {
            get
            {
                if (PageIndex == null || TotalRows == 0 || PageSize == null || PageSize == 0)
                    return 0;

                return (int)Math.Ceiling(this.TotalRows / (double)this.PageSize);
            }
        }
        public int Count { get; set; }
        public int TotalRows { get; set; }
        public List<DataQuerySchemaExposedColumn> Columns { get; set; }
        public List<object[]> Rows { get; set; }

        
        /// <summary>
        /// Convert to a standard datatable
        /// </summary>
        /// <returns></returns>
        public DataTable ToDataTable()
        {
            var dt = new DataTable
            {
                TableName = "Data query result"
            };

            foreach (var col in Columns)
            {
                dt.Columns.Add(new DataColumn(col.DisplayName, col.PropertyType) );
            }

            foreach (object[] row in Rows)
            {
                dt.Rows.Add(row);
            }

            return dt;
        }
        

        public void Dispose()
        {
            this.Columns.Clear();
            this.Rows.Clear();
            this.TotalRows = 0;
            this.Count = 0;
        }
    }
}
