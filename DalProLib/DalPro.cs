using System.Data;
using System.Reflection;
using Microsoft.Data.SqlClient;

namespace DalProLib
{
    public class DalPro
    {
        public static string ConnectionString; 

        private static readonly Dictionary<Type, PropertyInfo[]> _cacheProps =
            new Dictionary<Type, PropertyInfo[]>();
        

        public static SqlConnection GetConnection() 
        {
            return new SqlConnection(ConnectionString); 
        }

        // --------------------------------------------------
        // CREATE COMMAND - método para criar comando
        // Parametros: sql - string de comando sql
        //             trans - transaction
        //             parameters - parametros do comando sql (se houverem)
        // Parametros trans e parameters são nulls por default, caso não exista simolesmente assume-se que são null
        // --------------------------------------------------
        private static SqlCommand CreateCommand(
            string sql,
            SqlTransaction trans = null,
            Dictionary<string, object> parameters = null) 
        {
            SqlCommand cmd;

            if (trans != null) 

                cmd = new SqlCommand(sql, trans.Connection, trans);
            else
            {  
                SqlConnection cn = GetConnection();
                cn.Open(); 
                cmd = new SqlCommand(sql, cn);
            }

            if (parameters != null)  
            {
                foreach (var p in parameters)
                    cmd.Parameters.AddWithValue(p.Key, p.Value ?? DBNull.Value);
            }

            return cmd;
        }

        // --------------------------------------------------
        // EXECUTE NON QUERY
        //Usado para INSERT, UPDATE ou DELETE. Ele retorna o número de linhas afetadas.
        //Uso de using, que garante que o comando seja destruído da memória após o uso.
        //São comandos de modificação, ou seja, não são uma busca (não há query), portanto se tutiliza o comando ExecuteNonQuery()
        // --------------------------------------------------
        public static int Execute(
            string sql,
            Dictionary<string, object> parameters = null,
            SqlTransaction trans = null)
        {
            using SqlCommand cmd = CreateCommand(sql, trans, parameters); 

            int result = cmd.ExecuteNonQuery(); 

            if (trans == null)
                cmd.Connection.Close(); 

            return result;
        }

        // --------------------------------------------------
        // EXECUTE SCALAR
        //Usado quando se quer apenas um valor (ex: SELECT COUNT(*) ... ou SELECT MAX(Id) ...).
        //Ele retorna apenas a primeira coluna da primeira linha.
        // --------------------------------------------------
        public static object ExecuteScalar(
            string sql,
            Dictionary<string, object> parameters = null,
            SqlTransaction trans = null)
        {
            using SqlCommand cmd = CreateCommand(sql, trans, parameters);

            object result = cmd.ExecuteScalar(); 

            if (trans == null)
                cmd.Connection.Close();

            return result;
        }

        // --------------------------------------------------
        // QUERY GENERIC
        // Método para realizar queries para qualquer tipo de classe
        // --------------------------------------------------
        public static List<T> Query<T>(
            string sql,
            Dictionary<string, object> parameters = null,
            SqlTransaction trans = null) where T : new() 
        {
            List<T> list = new(); 

            using SqlCommand cmd = CreateCommand(sql, trans, parameters); 
            using SqlDataReader dr = cmd.ExecuteReader(); 

            PropertyInfo[] props; 

            if (!_cacheProps.TryGetValue(typeof(T), out props)) 
            {
                props = typeof(T).GetProperties();
                _cacheProps[typeof(T)] = props; 
            }

           
            while (dr.Read())
            {
                T obj = new T();

                foreach (var prop in props)
                {
                    try 
                    {
                        int idx = dr.GetOrdinal(prop.Name); 

                        if (!dr.IsDBNull(idx)) 
                            prop.SetValue(obj, dr[idx]); 
                    }
                    catch { }
                }

                list.Add(obj);
            }

            if (trans == null)
                cmd.Connection.Close(); 

            return list;
        }

        // --------------------------------------------------
        // DATATABLE FOR UPDATE
        // Carrega dados em uma datatable para depois salvar as alterações.
        // --------------------------------------------------
        public static DataTable DataTableForUpdate(
            string sql,
            ref SqlDataAdapter da,
            SqlTransaction trans = null)
        {
            SqlConnection cn;

            if (trans != null)
                cn = trans.Connection;
            else
            {
                cn = GetConnection();
                cn.Open();
            }

            da = new SqlDataAdapter(sql, cn); 

            if (trans != null)
                da.SelectCommand.Transaction = trans; 

            da.MissingSchemaAction = MissingSchemaAction.AddWithKey; 

            SqlCommandBuilder cb = new SqlCommandBuilder(da);

            cb.QuotePrefix = "[";
            cb.QuoteSuffix = "]";

            DataTable dt = new DataTable();
            da.Fill(dt);

            if (trans == null)
                cn.Close();

            return dt;
        }

        // --------------------------------------------------
        // STORED PROCEDURE - Método usado para chamar funções que já estão salvas dentro do servidor SQL,
        //                    retornando os resultados em um DataTable.
        // Parametros : spName - nome sa SP no SqlServer
        //              parameters - dicionário com parametros
        //              trans - transaction
        // --------------------------------------------------
        public static DataTable ExecuteSP(
            string spName,
            Dictionary<string, object> parameters = null,
            SqlTransaction trans = null)
        {
            SqlConnection cn;

            if (trans != null)
                cn = trans.Connection;
            else
            {
                cn = GetConnection();
                cn.Open();
            }

            SqlCommand cmd = new SqlCommand(spName, cn);
            cmd.CommandType = CommandType.StoredProcedure;

            if (trans != null)
                cmd.Transaction = trans;

            if (parameters != null) 
            {
                foreach (var p in parameters)
                    cmd.Parameters.AddWithValue(p.Key, p.Value ?? DBNull.Value);
            }

            SqlDataAdapter da = new SqlDataAdapter(cmd);

            DataTable dt = new DataTable();
            da.Fill(dt); 

            if (trans == null)
                cn.Close(); 

            return dt; 
        }

        
        public static object ExecuteScalarSP(
           string spName,
           Dictionary<string, object> parameters = null,
           SqlTransaction trans = null)
        {
            SqlConnection cn;

            if (trans != null)
                cn = trans.Connection; 
            else
            {
                cn = GetConnection(); 
                cn.Open();
            }

            SqlCommand cmd = new SqlCommand(spName, cn);
            cmd.CommandType = CommandType.StoredProcedure; 
            if (trans != null)
                cmd.Transaction = trans; 

            if (parameters != null) 
            {
                foreach (var p in parameters)
                    cmd.Parameters.AddWithValue(p.Key, p.Value ?? DBNull.Value);
            }

            object result = cmd.ExecuteScalar();

            if (trans == null)
                cn.Close(); 

            return result;
        }



        // --------------------------------------------------
        // TRANSACTIONS
        // --------------------------------------------------
        public static SqlTransaction BeginTransaction()
        {
            SqlConnection cn = GetConnection();
            cn.Open();
            return cn.BeginTransaction(); 
        }

        public static void Commit(SqlTransaction trans)
        {
            SqlConnection cn = trans.Connection;

            trans.Commit();

            if (cn.State == ConnectionState.Open) 
                cn.Close();
        }

        public static void Rollback(SqlTransaction trans) 
        {
            SqlConnection cn = trans.Connection;

            trans.Rollback();

            if (cn.State == ConnectionState.Open)
                cn.Close();
        }
    }
}
