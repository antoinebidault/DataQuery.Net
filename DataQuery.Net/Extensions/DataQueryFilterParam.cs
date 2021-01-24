using System.Text.RegularExpressions;

namespace DataQuery.Net
{
    public static class DataQueryFilterParamExtensions
    {

        /// <summary>
        /// Remove a filter in filters
        /// </summary>
        /// <param name="val"></param>
        public static void RemoveFilter(this DataQueryFilterParam param, string val)
        {
            if (!string.IsNullOrEmpty(param.Filters))
                param.Filters = Regex.Replace(param.Filters, @"[;,(]+?" + val + @"[!=~><+]+[a-z0-9-_~\/\\]+[)]?", "");
        }

        /// <summary>
        /// Add a filter dynamically to the filters string
        /// </summary>
        /// <param name="val"></param>
        /// <param name="sep"></param>
        public static void AppendFilter(this DataQueryFilterParam param, string val, ConditionSeparator sep = ConditionSeparator.And)
        {
            if (!string.IsNullOrEmpty(param.Filters) && !param.Filters.EndsWith("("))
            {
                param.Filters = param.Filters.TrimEnd(',').TrimEnd(';');
                switch (sep)
                {
                    case ConditionSeparator.And:
                        param.Filters += ";";
                        break;
                    case ConditionSeparator.Or:
                        param.Filters += ",";
                        break;
                }
            }

            param.Filters += val;
        }
    }
}
