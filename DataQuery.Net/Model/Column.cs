using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataQuery.Net
{

    public class Metric : Dimension
    {
        public Metric()
        {
            this.IsMetric = true;

            // By default, metric are setup as double
            this.PropertyType = typeof(double);
        }
    }

    /// <summary>
    /// Represents a single property
    /// </summary>
    public class Dimension
    {
        public Dimension()
        {
            this.SqlType = SqlDbType.NVarChar;
            this.Displayed = true;
            this.SqlJoins = new Dictionary<string, string>();
            this.AllowedToFilter = true;
            this.AllowedToView = true;
            this.Color = "#F86410";
            MetaDatas = new Dictionary<string, object>();
        }

        /// <summary>
        /// Table alias
        /// </summary>
        public string TableAlias { get; set; }


        [Obsolete]
        /// <summary>
        /// Please use "SqlName" instead
        /// </summary>
        public string ColumnName { get; set; }

        /// <summary>
        /// SQL column name, e.g. USER.Name, SUM(USER.NbConnexions)
        /// </summary>
        public string SqlName { get; set; }
        internal string SqlNameComputed
        {
            get
             {
                var sqlName = SqlName.Replace("<table>", this.TableAlias);
                if (!sqlName.Contains(this.TableAlias + "."))
                    return this.TableAlias + "." + sqlName;
                return sqlName;
            }
        }

        /// <summary>
        /// A simple metadata dictionnary
        /// </summary>
        public IDictionary<string, object> MetaDatas { get; set; }


        /// <summary>
        /// SQL Type overiden (use the native type if you don't care)
        /// </summary>
        public SqlDbType? SqlType { get; set; }

        /// <summary>
        /// Native type
        /// </summary>
        public Type PropertyType { get; set; } = typeof(string);

        public SqlDbType SqlTypeAuto
        {
            get
            {
                if (SqlType.HasValue)
                {
                    return this.SqlType.Value;
                }

                return PropertyType.GetDbType();
            }
        }

        public string Name { get; set; }
        public string Alias { get { return $"{this.TableAlias}_{this.Name}"; } }

        /// <summary>
        /// User friendly label
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Column definition
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Column group, if any
        /// </summary>
        public string Group { get; set; }

        /// <summary>
        /// Set it to true if it's a metric
        /// </summary>
        public bool IsMetric { get; set; }

        /// <summary>
        /// Set it to true if this prop is of type geography
        /// </summary>
        public bool IsGeography { get; set; }

        /// <summary>
        /// Unit, if any
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        /// Metric's color (for graph reporting)
        /// </summary>
        public string Color { get; set; }

        /// <summary>
        /// Set true to display this prop
        /// </summary>
        public bool Displayed { get; set; }

        /// <summary>
        /// Autorise filter (WHERE)
        /// </summary>
        public bool AllowedToFilter { get; set; }

        /// <summary>
        /// Autorise reporting (SELECT)
        /// </summary>
        public bool AllowedToView { get; set; }

        /// <summary>
        /// Set to true if this prop must be used to filter date or not
        /// </summary>
        public bool UsedToFilterDate { get; set; }

        /// <summary>
        /// Sql joins
        /// </summary>
        public Dictionary<string, string> SqlJoins { get; set; }
        public IEnumerable<ColumnValue> Values { get; set; }
    }

    public class ColumnValue
    {
        public string Id { get; set; }
        public string Text { get; set; }
        public string Value { get; set; }
        public string Color { get; set; }
    }
}