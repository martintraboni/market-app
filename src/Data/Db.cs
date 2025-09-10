using System.Data;
using System.Data.SqlClient;

namespace Minimarket.Data
{
    public static class Db
    {
        public static SqlConnection GetConnection()
        {
            return new SqlConnection(Config.ConnectionString);
        }

        public static DataTable Query(string sql, params SqlParameter[] parameters)
        {
            using var cn = GetConnection();
            using var cmd = new SqlCommand(sql, cn);
            if (parameters != null)
                cmd.Parameters.AddRange(parameters);
            using var da = new SqlDataAdapter(cmd);
            var dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        public static int Execute(string sql, params SqlParameter[] parameters)
        {
            using var cn = GetConnection();
            using var cmd = new SqlCommand(sql, cn);
            if (parameters != null)
                cmd.Parameters.AddRange(parameters);
            cn.Open();
            return cmd.ExecuteNonQuery();
        }

        public static object? Scalar(string sql, params SqlParameter[] parameters)
        {
            using var cn = GetConnection();
            using var cmd = new SqlCommand(sql, cn);
            if (parameters != null)
                cmd.Parameters.AddRange(parameters);
            cn.Open();
            return cmd.ExecuteScalar();
        }
    }
}
