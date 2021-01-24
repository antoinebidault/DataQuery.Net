
using System;
using System.Collections.Generic;
using System.Text;

namespace DataQuery.Net.Model
{
    public class DataQueryException : Exception
    {
        public DataQueryException(string message) : base(message)
        {
        }
    }
}
