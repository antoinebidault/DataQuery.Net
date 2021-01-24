using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataQuery.Net
{
  public static class OperatorTypeExtensions
  {
    public static string GetQueryLabel(this OperatorType type)
    {
      switch (type)
      {
        case OperatorType.Equal:
          return "==";
        case OperatorType.Superior:
          return ">=";
        case OperatorType.SuperiorStrict:
          return ">";
        case OperatorType.Inferior:
          return "<=";
        case OperatorType.InferiorStrict:
          return "<";
        case OperatorType.Different:
          return "!=";
        case OperatorType.Like:
          return "=~";
        case OperatorType.NotLike:
          return "!=~";
      }
      return "";
    }
    public static string GetSqlLabel(this OperatorType type)
    {
      switch (type)
      {
        case OperatorType.Equal:
          return "=";
        case OperatorType.Superior:
          return ">=";
        case OperatorType.SuperiorStrict:
          return ">";
        case OperatorType.Inferior:
          return "<=";
        case OperatorType.InferiorStrict:
          return "<";
        case OperatorType.Different:
          return "<>";
        case OperatorType.Like:
          return "LIKE";
        case OperatorType.NotLike:
          return "NOT LIKE";
      }
      return "";
    }
  }
}
