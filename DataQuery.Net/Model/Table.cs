using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataQuery.Net
{
    public class Table
    {
        /// <param name="alias">Programatic name of the table (e.g. DP). It must be as short as possible and in uppercase if possible</param>
        /// <param name="sqlName">Sql name of the table (e.g. DataTreatments)</param>
        public Table(string alias, string sqlName = null)
        {
            this.DisplayName = alias;
            this.Alias = alias;
            this.SqlName = sqlName ?? alias;
            Columns = new List<Dimension>();
            TypeJoin = TypeJoin.INNER;
            Size = DataBaseSize.Standard;
        }
        public string DisplayName { get; set; }

        public bool SupportFreeText { get; set; }

        /// <summary>
        /// Programatic name of the table used as alias in the SQL Query
        /// </summary>
        public string Alias { get; set; }

        /// <summary>
        /// The real name of the table. You can use schema prefix if necessary (e.g. dbo.Actors)
        /// </summary>
        public string SqlName { get; }
        public int Order { get; set; }


        public TypeJoin TypeJoin { get; set; }

        /// <summary>
        /// List of metrics and dimensions contained in the table
        /// </summary>
        public List<Dimension> Columns { get; set; }

        /// <summary>
        /// A filter that is automatically appened to the query (SQL)
        /// e.g. <table>.TenantId = 1 AND <table>.WorkspaceId = 1 
        /// </summary>
        public string DefaultFilterUsedIfTableUsed { get; set; } 
        public DataBaseSize Size { get; set; }

        /// <summary>
        /// Root table are main tables 
        /// </summary>
        public bool Root { get; set; }

        /// <summary>
        /// An abstract table not exposed
        /// </summary>
        public bool Implicit { get; set; }
        public bool NotDiscoverable { get; set; }

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