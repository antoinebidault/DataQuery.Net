﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataQuery.Net
{
    /// <summary>
    /// Represents a single property
    /// </summary>
    public class Column
    {
        public Column()
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
        /// SQL column name, e.g. USER.Name, SUM(USER.NbConnexions)
        /// </summary>
        public string ColumnName { get; set; }


        /// <summary>
        /// A simple metadata dictionnary
        /// </summary>
        public IDictionary<string,object> MetaDatas { get; set; }

        /// <summary>
        /// SQL Type
        /// </summary>
        public SqlDbType SqlType { get; set; }

        /// <summary>
        /// SQL Alias used to call this prop in the front channel (using the dataqueryfilterparams)
        /// </summary>
        public string Alias { get; set; }

        /// <summary>
        /// User friendly label
        /// </summary>
        public string Label { get; set; }

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
    }
}