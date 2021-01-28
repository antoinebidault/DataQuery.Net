using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataQuery.Net
{
  public class Table
  {
    public Table()
    {
      Props = new List<Column>();
      TypeJoin = TypeJoin.INNER;
      Size = DataBaseSize.Standard;
    }
    public bool SupportFreeText { get; set; }
    public string Name { get; set; }
    public int Order { get; set; }
    public TypeJoin TypeJoin { get; set; }
    public string Alias { get; set; }
    public List<Column> Props { get; set; }
    public string DefaultFilterUsedIfTableUsed { get; set; }
    public DataBaseSize Size { get; set; }
    
  }

  public enum DataBaseSize
  {
    Small = 0,
    Standard,
    Big,
    VeryBig
  }

  public enum TypeJoin
  {
    INNER,
    LEFTOUTER
  }


}