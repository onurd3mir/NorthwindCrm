using System;
using System.Collections.Generic;
using System.Text;

namespace Northwind.Cache
{
    public interface IRepositoryCache
    {
        List<T> AddCache<T>(string key,int time,string sql);
    }
}
