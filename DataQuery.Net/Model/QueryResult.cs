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
  public class QueryResult
  {
    public QueryResult()
    {
    }

    public string SqlQuery { get; set; }
    public double QueryDelay { get; set; }
    public DataQueryFilterParam Filter { get; set; }
    public QueryTable Data { get; set; }

  }

  public class Column
  {
    public string Name { get; set; }
    public string Label { get; set; }
    public string Type { get; set; }
    public bool IsMetric { get; set; }
    public string Unite { get; set; }
    public string Color { get; set; }

  }



  public class QueryTable : IDisposable
  {
    public QueryTable()
    {
      this.Columns = new List<Column>();
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
    public List<Column> Columns { get; set; }
    public List<object[]> Rows { get; set; }

    /// <summary>
    /// Convertit la table en datatable pour plus de comodité
    /// </summary>
    /// <returns></returns>
    public DataTable ToDataTable()
    {
      var dt = new DataTable();
      dt.TableName = "Résultat Data Query";
      foreach (Column col in Columns)
      {
        dt.Columns.Add(new DataColumn() { ColumnName = col.Name, Caption = col.Label });
      }

      foreach (object[] row in Rows)
      {
        dt.Rows.Add(row);
      }

      return dt;
    }

    /// <summary>
    /// Si vous voulez faire un CSV > 200 mégas à partir d'un datatable, 
    /// cette méthode est nécessaire pour éviter les outofmemoryexception de la méthode ci-dessus
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
