using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataQuery.Net
{
    public class Table
    {
        public Table(string alias, string sqlName = null)
        {
            this.DisplayName = alias;
            this.Alias = alias;
            this.SqlName = sqlName ?? alias;
            Columns = new List<Dimension>();
            TypeJoin = TypeJoin.INNER;
            Size = DataBaseSize.Standard;
        }
        public bool SupportFreeText { get; set; }
        public string Alias { get; set; }
        public string SqlName { get; }
        public int Order { get; set; }
        public TypeJoin TypeJoin { get; set; }
        public List<Dimension> Columns { get; set; }
        public string DefaultFilterUsedIfTableUsed { get; set; }
        public DataBaseSize Size { get; set; }
        public bool Root { get; set; }
        public bool Implicit { get; set; }
        public bool NotDiscoverable { get; set; }
        public string DisplayName { get; set; }

        /// <summary>
        /// A sample dic for storing things
        /// </summary>
        public IDictionary<string,object> Items { get; set; }

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