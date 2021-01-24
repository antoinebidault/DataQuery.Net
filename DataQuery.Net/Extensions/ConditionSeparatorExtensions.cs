using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataQuery.Net
{
  public static class ConditionSeparatorExtensions
  {
    public static string GetSqlLabel(this ConditionSeparator type)
    {
      switch (type)
      {
        case ConditionSeparator.Or:
          return "OR";
        case ConditionSeparator.And:
          return "AND";
      }
      return "";
    }
  }
}
