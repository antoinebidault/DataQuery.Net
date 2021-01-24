using DataQuery.Net.Extensions;
using System;
using System.Linq;

namespace DataQuery.Net
{
    public class Inclusion
    {
        public Inclusion(string inclusion, DataQueryConfig config)
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

            if (!config.MetricsAndDimensions[propName].AllowedToRequest)
                throw new Exception(string.Format("Vous n'êtes malheureusement pas autorisé à requêter cette dimension : {0} ! ", propName));

            this.Prop = config.Dimensions[propName];
            this.Table = config.Tables.Values.FirstOrDefault(m => m.Props.Contains(this.Prop));
            this.KeyTable = Table.Props.FirstOrDefault(m => m.SqlJoin.Any());
            this.LinkedTable = config.Tables[KeyTable.SqlJoin.FirstOrDefault().Key];
            this.LinkedPropertyColumnName = this.LinkedTable.Alias + "." + KeyTable.SqlJoin.FirstOrDefault().Value;
        }

        public DatabaseProp Prop { get; set; }
        public DatabaseProp KeyTable { get; set; }
        public string LinkedPropertyColumnName { get; set; }
        public Table LinkedTable { get; set; }
        public Table Table { get; set; }
        public string Value { get; set; }
        public TypeInclusion Type { get; set; }
    }




}
