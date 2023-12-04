
using System;

namespace DataQuery.Net.Model
{
    public class DataQueryInvalidConfigException : Exception
    {
        public DataQueryInvalidConfigException(string message) : base(message)
        {
        }
    }

}
