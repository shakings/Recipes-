﻿using Recipes.DataAccessLayer.Models;
using System.Data;
using System.Data.SqlClient;

namespace Recipes.DataAccessLayer.Interfaces
{
    public abstract class DALBase
    {
        protected virtual async Task<TResult> WrapQuery<TResult>(QueryParams queryParams, Func<SqlConnection, IDbTransaction, QueryParams, Task<TResult>> queryFunc)
        {
            using (SqlConnection connection = new SqlConnection(queryParams.ConnectionString))
            {
                if (queryParams.UseTransaction)
                {
                    using (SqlTransaction tran = connection.BeginTransaction())
                    {
                        try
                        {

                            TResult rows = await queryFunc(connection, tran, queryParams);
                            tran.Commit();
                            return rows;
                        }
                        catch
                        {
                            tran.Rollback();
                            throw;
                        }
                    }
                }
                else
                {
                    return await queryFunc(connection, null, queryParams);
                }
            }
        }
        protected virtual async Task WrapQuery(QueryParams queryParams, Func<SqlConnection, IDbTransaction, QueryParams, Task> queryFunc)
        {
            using (SqlConnection connection = new SqlConnection(queryParams.ConnectionString))
            {
                if (queryParams.UseTransaction)
                {
                    using (SqlTransaction tran = connection.BeginTransaction())
                    {
                        try
                        {
                            await queryFunc(connection, tran, queryParams);
                            tran.Commit();
                            return;
                        }
                        catch
                        {
                            tran.Rollback();
                            throw;
                        }
                    }

                }
                else
                {
                    await queryFunc(connection, null, queryParams);
                }
            }
        }
    }
}
