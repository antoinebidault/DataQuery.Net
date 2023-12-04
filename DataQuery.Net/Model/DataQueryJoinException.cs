
using System;
using System.Collections.Generic;
using System.Text;

namespace DataQuery.Net.Model
{
    public class DataQueryJoinException : Exception
    {
        public DataQueryJoinException(string message) : base(message)
        {
        }
    }

}
