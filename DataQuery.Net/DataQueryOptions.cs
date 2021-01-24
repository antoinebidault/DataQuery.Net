namespace Microsoft.Extensions.DependencyInjection
{
    public class DataQueryOptions
    {

        public static DataQueryOptions Defaults
        {
                get{
                    return new DataQueryOptions()
                    {

                    };
                }
            }

        public string ConnectionString { get;  set; }
    }

    }