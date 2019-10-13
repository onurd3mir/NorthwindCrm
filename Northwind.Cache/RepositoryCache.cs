using Dapper;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Northwind.Cache
{
    public class RepositoryCache : IRepositoryCache
    {
        
        private readonly IDbConnection connection;
        private readonly IMemoryCache _memoryCache;

        public RepositoryCache(IMemoryCache memoryCache, IConfiguration configuration)
        {
            _memoryCache = memoryCache;
            connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        }

        public List<T> AddCache<T>(string key, int time, string sql)
        {

            if (!_memoryCache.TryGetValue(key, out List<T> response))
            {
                try
                {
                    connection.Open();

                    response = connection.Query<T>(sql).AsQueryable().ToList();

                    var cacheExpirationOptions =
                    new MemoryCacheEntryOptions
                    {
                        AbsoluteExpiration = DateTime.Now.AddMinutes(time),
                        Priority = CacheItemPriority.Normal
                    };
                    _memoryCache.Set(key, response, cacheExpirationOptions);

                }
                catch (Exception)
                {
                    throw;
                }
                finally { connection.Close(); }

            }

            return response;
        }
    }
}
