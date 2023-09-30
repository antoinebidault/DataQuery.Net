﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataQuery.Net
{
    public class Table
    {
        public Table(string tableName, string sqlName = null)
        {
            this.DisplayName = tableName;
            this.Name = tableName;
            this.SqlName = sqlName ?? tableName;
            Columns = new List<Column>();
            TypeJoin = TypeJoin.INNER;
            Size = DataBaseSize.Standard;
        }
        public bool SupportFreeText { get; set; }
        public string Name { get; }
        public string SqlName { get; }
        public int Order { get; set; }
        public TypeJoin TypeJoin { get; set; }
        public string Alias { get; set; }
        public List<Column> Columns { get; set; }
        public string DefaultFilterUsedIfTableUsed { get; set; }
        public DataBaseSize Size { get; set; }
        public bool Root { get; set; }
        public bool Implicit { get; set; }
        public bool NotDiscoverable { get; set; }
        public string DisplayName { get; set; }


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