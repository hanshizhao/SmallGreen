using System.Data;
using Microsoft.Data.SqlClient;

namespace SmallGreen.Entity.Basic
{
    public class ErpDbHelper
    {
        private readonly string _connectionString;

        public ErpDbHelper(string connectionString)
        {
            _connectionString = connectionString
                ?? throw new ArgumentNullException(nameof(connectionString));
        }

        /// <summary>
        /// 执行查询类存储过程，返回 DataTable
        /// </summary>
        public DataTable RunProcedure(string procName, SqlParameter[] parameters)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(procName, conn)
            {
                CommandType = CommandType.StoredProcedure,
                CommandTimeout = 30
            };
            if (parameters != null)
            {
                cmd.Parameters.AddRange(parameters);
            }

            var dt = new DataTable();
            using var adapter = new SqlDataAdapter(cmd);
            adapter.Fill(dt);
            return dt;
        }

        /// <summary>
        /// 执行更新类存储过程，返回影响行数
        /// </summary>
        public int UpdateByProcedure(string procName, SqlParameter[] parameters)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(procName, conn)
            {
                CommandType = CommandType.StoredProcedure,
                CommandTimeout = 30
            };
            if (parameters != null)
            {
                cmd.Parameters.AddRange(parameters);
            }

            conn.Open();
            return cmd.ExecuteNonQuery();
        }
    }
}
