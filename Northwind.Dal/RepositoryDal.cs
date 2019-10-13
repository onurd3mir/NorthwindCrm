using Dapper;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Northwind.Entities;

namespace Northwind.Dal
{
    public class RepositoryDal : IRepositoryDal
    {

        private readonly IDbConnection connection;
    
        public RepositoryDal(IConfiguration configuration)
        {
            connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        }

        public int ExecuteQuery(string sql)
        {
            try
            {
                connection.Open();

                return connection.Execute(sql);
            }
            catch (Exception)
            {
                throw;
            }
            finally { connection.Close(); }
        }

        public IQueryable<Admin> GetAdmin()
        {
            try
            {
                connection.Open();

                return connection.Query<Admin>("SELECT * FROM admins (nolock)").AsQueryable();
            }
            catch (Exception)
            {
                throw;
            }
            finally { connection.Close(); }
        }

        public IQueryable<T> GetQuery<T>(string sql)
        {
            try
            {
                connection.Open();

                return connection.Query<T>(sql).AsQueryable();
            }
            catch (Exception)
            {
                throw;
            }
            finally { connection.Close(); }
        } 
    }
}
