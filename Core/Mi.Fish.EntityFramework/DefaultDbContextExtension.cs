using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace Mi.Fish.EntityFramework
{
    public static class DbContextExtension
    {
        private static readonly MethodInfo GetEntitiesMethod =
            typeof(DbContextExtension).GetMethod(nameof(DbContextExtension.GetEntities), BindingFlags.Static | BindingFlags.NonPublic);

        public static Task<TResult> ExecuteFunctionAsync<TResult>(this DbContext context, string sql, object @params) where TResult : class, new()
        {
            return ExecuteFunctionAsync<TResult>(context, sql, @params, CancellationToken.None);
        }

        public static async Task<TResult> ExecuteFunctionAsync<TResult>(this DbContext context, string sql, object @params, CancellationToken cancellationToken) where TResult : class, new()
        {
            var conn = context.Database.GetDbConnection();

            DbTransaction transaction = null;
            if (context.Database.GetService<IDbContextTransactionManager>() is RelationalConnection)
            {
                transaction = context.Database.CurrentTransaction?.GetDbTransaction();
            }

            bool needClosed = transaction == null;

            cancellationToken.ThrowIfCancellationRequested();

            DataTable table;
            DbCommand cmd = null;
            try
            {
                if (conn.State != ConnectionState.Open)
                {
                    await conn.OpenAsync(cancellationToken);
                }

                cmd = CreateCommand(conn, transaction, sql, @params);

                var reader = await cmd.ExecuteReaderAsync(cancellationToken);
                
                table = new DataTable();
                table.Load(reader);

                reader.Close();

                if (needClosed)
                {
                    cmd.Transaction.Commit();
                }
            }
            catch (Exception e)
            {
                //context.Logger.Error("ExecuteFunctionAsync has errors.", e);

                if (needClosed)
                {
                    cmd?.Transaction?.Rollback();
                }

                throw;
            }
            finally
            {
                if (needClosed)
                {
                    cmd?.Transaction?.Dispose();
                    conn.Close();
                }
                //connection closed by UOW, comment this operation
                //conn.Close();
            }

            var resultType = typeof(TResult);
            if (resultType.IsGenericType)
            {
                //if result is List<T>
                if (typeof(IEnumerable).IsAssignableFrom(resultType.GetGenericTypeDefinition()))
                {
                    //make generic method
                    var itemType = resultType.GetGenericArguments()[0];

                    var items = GetEntitiesMethod.MakeGenericMethod(itemType).Invoke(null, new object[] { table });

                    var result = (TResult)Activator.CreateInstance(resultType, items);

                    return result;
                }
                else
                {
                    throw new NotImplementedException("other generic type.");
                }
            }
            else
            {
                var properties = GetTypeProperties<TResult>(table.Columns);

                return table.Rows.Count > 0 ? GetEntity<TResult>(table.Rows[0], properties) : default(TResult);
            }
        }

        /// <summary>
        /// Creates the command.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="transaction">The transaction.</param>
        /// <param name="sql">The SQL.</param>
        /// <param name="params">The parameters.</param>
        /// <returns></returns>
        private static DbCommand CreateCommand(DbConnection connection, DbTransaction transaction, string sql, object @params)
        {
            var cmd = connection.CreateCommand();

            cmd.CommandText = sql;

            cmd.Transaction = transaction ?? connection.BeginTransaction();

            foreach (var prop in @params.GetType().GetProperties())
            {
                var para = Activator.CreateInstance(typeof(SqlParameter), $"@{prop.Name}", prop.GetValue(@params) ?? DBNull.Value);

                cmd.Parameters.Add(para);
            }

            return cmd;
        }

        /// <summary>
        /// Get entities from DataTable
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="dt">The dt.</param>
        /// <returns></returns>
        private static IEnumerable<TEntity> GetEntities<TEntity>(DataTable dt)
        {
            List<TEntity> returnValue = new List<TEntity>();

            var properties = GetTypeProperties<TEntity>(dt.Columns);

            foreach (DataRow row in dt.Rows)
            {
                returnValue.Add(GetEntity<TEntity>(row, properties));
            }

            return returnValue.AsEnumerable();
        }

        /// <summary>
        /// Gets the entity.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="row">The row.</param>
        /// <param name="properties">The properties.</param>
        /// <returns></returns>
        private static TEntity GetEntity<TEntity>(DataRow row, List<string> properties)
        {
            TEntity entity = Activator.CreateInstance<TEntity>();

            foreach (var propertyName in properties)
            {
                var prop = entity.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public);

                if (row[propertyName] != DBNull.Value)
                {
                    if (prop.PropertyType == typeof(String))
                    {
                        //todo: CultureInfo
                        prop.SetValue(entity, row[propertyName].ToString());
                    }
                    else if (prop.PropertyType == typeof(Guid))
                    {
                        prop.SetValue(entity, Guid.Parse(row[propertyName].ToString()));
                    }
                    else
                    {
                        prop.SetValue(entity, row[propertyName]);
                    }
                }
                else
                {
                    prop.SetValue(entity, null);
                }
            }

            return entity;
        }

        /// <summary>
        /// Gets the type properties.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="columns">The columns.</param>
        /// <returns></returns>
        private static List<string> GetTypeProperties<TEntity>(DataColumnCollection columns)
        {
            var entityType = typeof(TEntity);

            List<string> typeProperties = new List<string>();
            foreach (DataColumn column in columns)
            {
                var prop = entityType.GetProperty(column.ColumnName, BindingFlags.Instance | BindingFlags.Public);
                if (prop != null)
                {
                    typeProperties.Add(column.ColumnName);
                }
            }

            return typeProperties;
        }

        /// <summary>
        /// ExecuteNonQueryAsync
        /// </summary>
        /// <param name="context"></param>
        /// <param name="sql"></param>
        /// <param name="params"></param>
        /// <returns></returns>
        public static Task<int> ExecuteNonQueryAsync(this DbContext context, string sql, object @params)
        {
            return context.Database.ExecuteSqlCommandAsync(sql, @params);

            //var conn = context.Database.GetDbConnection();

            //DbTransaction transaction = null;
            //if (context.Database.GetService<IDbContextTransactionManager>() is RelationalConnection)
            //{
            //    transaction = context.Database.CurrentTransaction?.GetDbTransaction();
            //}

            //bool newTransaction = transaction == null;
            //var cmd = CreateCommand(conn, transaction, sql, @params);

            //try
            //{
            //    if (conn.State != ConnectionState.Open)
            //    {
            //        await conn.OpenAsync();
            //    }

            //    return await cmd.ExecuteNonQueryAsync();
            //}
            //catch (DbException exc)
            //{
            //    context.Logger.Error("ExecuteNonQueryAsync has errors.", exc);
            //    throw;
            //}
            //finally
            //{
            //    if (newTransaction)
            //    {
            //        cmd.Transaction.Commit();
            //        cmd.Transaction.Dispose();
            //    }
            //}
        }

        /// <summary>
        /// ExecuteNonAsync
        /// </summary>
        /// <param name="context"></param>
        /// <param name="sql"></param>
        /// <param name="params"></param>
        /// <returns></returns>
        public static async Task<int> ExecuteNonAsync(this DbContext context, string sql, object @params)
        {
            var conn = context.Database.GetDbConnection();

            DbTransaction transaction = null;
            DbCommand cmd = null;
            var result = 0;

            if (context.Database.GetService<IDbContextTransactionManager>() is RelationalConnection)
            {
                transaction = context.Database.CurrentTransaction?.GetDbTransaction();
            }

            bool newTransaction = transaction == null;
            try
            {
                if (conn.State != ConnectionState.Open)
                {
                    await conn.OpenAsync();
                }

                cmd = CreateCommand(conn, transaction, sql, @params);
                result = await cmd.ExecuteNonQueryAsync();
            }
            catch (DbException exc)
            {
                if (newTransaction)
                {
                   cmd?.Transaction?.Rollback();
                }
                throw exc;
            }
            finally
            {
                if (newTransaction)
                {
                    cmd?.Transaction?.Commit();
                    cmd?.Transaction?.Dispose();
                }
            }

            return result;
        }

        /// <summary>
        /// 获取DataSet
        /// </summary>
        /// <param name="context"></param>
        /// <param name="sql"></param>
        /// <param name="params"></param>
        /// <returns></returns>
        public static DataSet GetDataSet(this DbContext context, string sql, object @params)
        {
            var conn = context.Database.GetDbConnection();
            SqlConnection sqlConnection = (SqlConnection) conn;
            using (SqlCommand cmd = sqlConnection.CreateCommand())
            {
                cmd.CommandText = sql; 
                foreach (var prop in @params.GetType().GetProperties())
                {
                    var para = Activator.CreateInstance(typeof(SqlParameter), $"@{prop.Name}",
                        prop.GetValue(@params) ?? DBNull.Value);
                    cmd.Parameters.Add(para);
                }

                DbDataAdapter adapter = null;
                try
                {
                    if (conn.State != ConnectionState.Open)
                    {
                        conn.Open();
                    }

                    adapter = new SqlDataAdapter((SqlCommand) cmd);
                    DataSet ds = new DataSet();
                    adapter.Fill(ds);
                    return ds;
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    conn.Close();
                    IDisposable id = (IDisposable) adapter;
                    id?.Dispose();
                }
            }
        }

        /// <summary>
        /// GetDataTableAsync
        /// </summary>
        /// <param name="context"></param>
        /// <param name="sql"></param>
        /// <param name="params"></param>
        /// <returns></returns>
         public static async Task<DataTable> GetDataTableAsync(this DbContext context, string sql, object @params)
        {
            var conn = context.Database.GetDbConnection();
            SqlConnection sqlConnection = (SqlConnection)conn;
         
            using (SqlCommand cmd = sqlConnection.CreateCommand())
            {
                try
                {
                    cmd.CommandText = sql;
                    if (@params != null)
                    {
                        foreach (var prop in @params.GetType().GetProperties())
                        {
                            var para = Activator.CreateInstance(typeof(SqlParameter), $"@{prop.Name}", prop.GetValue(@params) ?? DBNull.Value);
                            cmd.Parameters.Add(para);
                        }
                    }
                    if (conn.State != ConnectionState.Open)
                    {
                        await conn.OpenAsync(CancellationToken.None);
                    }
                    
                    var reader = await cmd.ExecuteReaderAsync(CancellationToken.None);
                    DataTable table= new DataTable();
                    table.Load(reader);
                    reader.Close();
                    return table;
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    sqlConnection.Close();
                }

            }

        }
        /// <summary>
        /// ExecuteScalarAsync
        /// </summary>
        /// <param name="context"></param>
        /// <param name="sql"></param>
        /// <param name="params"></param>
        /// <returns></returns>
        public static async Task<object> ExecuteScalarAsync(this DbContext context, string sql, object @params)
        {
            var conn = context.Database.GetDbConnection();
            SqlConnection sqlConnection = (SqlConnection)conn;

            using (SqlCommand cmd = sqlConnection.CreateCommand())
            {
                try
                {
                    cmd.CommandText = sql;
                    if (@params != null)
                    {
                        foreach (var prop in @params.GetType().GetProperties())
                        {
                            var para = Activator.CreateInstance(typeof(SqlParameter), $"@{prop.Name}", prop.GetValue(@params) ?? DBNull.Value);
                            cmd.Parameters.Add(para);
                        }
                    }
                    if (conn.State != ConnectionState.Open)
                    {
                        await conn.OpenAsync(CancellationToken.None);
                    }

                    object obj = await cmd.ExecuteScalarAsync(CancellationToken.None);
                    return obj;
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    sqlConnection.Close();
                }

            }

        }
    }
}
