using Microsoft.Extensions.DependencyInjection;
using DataQuery.Net;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddDataQueryProvider<TProvider>(this IServiceCollection services) where TProvider : IDataQueryProvider
        {
            services.AddTransient<IDataQueryProvider>(c => c.GetRequiredService<TProvider>());
            return services;
        }

        public static IServiceCollection AddDataQuery(this IServiceCollection services, string connectionString)
        {
            var dataQueryOptions = DataQueryOptions.Defaults;
            dataQueryOptions.ConnectionString = connectionString;
            return services.AddDataQuery(dataQueryOptions);
        }

        public static IServiceCollection AddDataQuery(this IServiceCollection services, Action<DataQueryOptions> optionsBuilder)
        {
            var dataQueryOptions = DataQueryOptions.Defaults;
            optionsBuilder.Invoke(dataQueryOptions);
            return services.AddDataQuery(dataQueryOptions);
        }

        public static IServiceCollection AddDataQuery(this IServiceCollection services, DataQueryOptions options)
        {
            services.AddSingleton(options);
            services.AddTransient<IDataQueryRepository, DataQuerySqlServerRepository>();
            services.AddTransient<IDataQuery, DefaultDataQuery>();
            return services;
        }


    }
}
