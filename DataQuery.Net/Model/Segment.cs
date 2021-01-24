using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataQuery.Net
{

  /// <summary>
  /// Les informations récupérés du dataquery
  /// </summary>
  public class Segment
  {
    public string Id { get; set; }
    public SqlDbType Type { get; set; }
    public string TableName { get; set; }
  }

}
