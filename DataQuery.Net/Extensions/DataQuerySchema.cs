using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace DataQuery.Net.Extensions
{
    public static class DataQuerySchemaExtensions
    {
        /// <summary>
        /// Add data query entity to schema
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="schema"></param>
        public static void IncludeDataQuerySchema<T>(this DataQuery.Net.DataQuerySchema schema) where T : class
        {


        }


    }
}
