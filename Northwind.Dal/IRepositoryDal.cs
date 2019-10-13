using Northwind.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Northwind.Dal
{
    public interface IRepositoryDal
    {

        IQueryable<T> GetQuery<T>(string sql);

        int ExecuteQuery(string sql);

        IQueryable<Admin> GetAdmin();

    }
}
