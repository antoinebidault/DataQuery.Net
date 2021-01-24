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
        public double QueryDelay { get; set; }
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
            this.Columns = new List<DataQueryColumn>();
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
        public List<DataQueryColumn> Columns { get; set; }
        public List<object[]> Rows { get; set; }

        /// <summary>
        /// Convert to a standard datatable
        /// </summary>
        /// <returns></returns>
        public DataTable ToDataTable()
        {
            var dt = new DataTable();
            dt.TableName = "Data query result";
            foreach (DataQueryColumn col in Columns)
            {
                dt.Columns.Add(new DataColumn() { ColumnName = col.Name, Caption = col.Label });
            }

            foreach (object[] row in Rows)
            {
                dt.Rows.Add(row);
            }

            return dt;
        }

        /*
        /// <summary>
        /// Convert to a csv file (on disk file
        /// </summary>
        /// <param name="tempFilePath">L'emplacement physique du fichier temporaire :  c:\temp\test.csv</param>
        /// <param name="separator">Le séparateur</param>
        /// <param name="nullValue">La valeur si null</param>
        public void ToFileCSV(string tempFilePath, string separator = ";", string nullValue = "")
        {
            string line = "";
            using (var writer = new StreamWriter(tempFilePath, false, Encoding.Default))
            {
                line = string.Empty;

                //Entêtes de colonnes
                for (int i = 0; i < this.Columns.Count; i++)
                {
                    line += "\"";
                    line += this.Columns[i].Label.Replace("\"", "\"\"");
                    line += "\"";
                    line += (i == this.Columns.Count - 1 ? "\r\n" : separator);
                }
                writer.Write(line);

                string val = "";

                //Ecriture des lignes
                foreach (object[] row in this.Rows)
                {
                    line = string.Empty;

                    //Ecriture des cellules
                    for (int i = 0; i < this.Columns.Count; i++)
                    {
                        val = row[i].ToString();
                        line += "\"";
                        {
                            if (!string.IsNullOrEmpty(val))
                                line += (string.IsNullOrEmpty(val) ? nullValue : val.Replace("\"", "\"\""));
                        }
                        line += "\"";
                        line += (i == this.Columns.Count - 1 ? "\r\n" : separator);
                    }
                    writer.Write(line);
                }
            }

        }

        public string ToCsv(string separator = ";", string nullValue = "")
        {
            var result = new StringBuilder();
            for (int i = 0; i < this.Columns.Count; i++)
            {
                result.Append("\"");
                result.Append(this.Columns[i].Label.Replace("\"", "\"\""));
                result.Append("\"");
                result.Append(i == this.Columns.Count - 1 ? "\r\n" : separator);
            }

            foreach (var row in this.Rows)
            {
                for (int i = 0; i < Columns.Count; i++)
                {
                    var val = row[i].ToString();
                    result.Append("\"");
                    result.Append((string.IsNullOrEmpty(val) ? nullValue : val).Replace("\"", "\"\""));
                    result.Append("\"");
                    result.Append(i == Columns.Count - 1 ? "\r\n" : separator);
                }
            }

            return result.ToString();

        }*/

        public void Dispose()
        {
            this.Columns.Clear();
            this.Rows.Clear();
            this.TotalRows = 0;
            this.Count = 0;
        }
    }
}
