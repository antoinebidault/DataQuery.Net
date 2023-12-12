using DataQuery.Net.Extensions;
using DataQuery.Net.Model;
using System;
using System.Linq;

namespace DataQuery.Net
{
    public class Inclusion
    {
        public Inclusion(string inclusion, DataQuerySchema config)
        {
            string[] dims = inclusion.Split(':');
            string propName = dims[0];
            if (propName.StartsWith("-"))
            {
                propName = propName.ReplaceFirst("-", "");
                this.Type = TypeInclusion.Out;
            }
            else if (propName.StartsWith("+"))
            {
                propName = propName.ReplaceFirst("+", "");
                this.Type = TypeInclusion.Add;
            }
            else
            {
                this.Type = TypeInclusion.In;
            }
            this.Value = dims[1];

            if (!config.MetricsAndDimensions[propName].AllowedToFilter)
                throw new DataQueryException($"You are not allowed to access this dimension : {propName} ! ");

            this.Prop = config.Dimensions[propName];
            this.Table = config.Tables.Values.FirstOrDefault(m => m.Columns.Contains(this.Prop));
            this.KeyTable = Table.Columns.FirstOrDefault(m => m.SqlJoins.Any());
            this.LinkedTable = config.Tables[KeyTable.SqlJoins.FirstOrDefault().Key];
            this.LinkedPropertyColumnName = this.LinkedTable.Alias + "." + KeyTable.SqlJoins.FirstOrDefault().Value;
        }

        public Dimension Prop { get; set; }
        public Dimension KeyTable { get; set; }
        public string LinkedPropertyColumnName { get; set; }
        public Table LinkedTable { get; set; }
        public Table Table { get; set; }
        public string Value { get; set; }
        public TypeInclusion Type { get; set; }
    }


}
