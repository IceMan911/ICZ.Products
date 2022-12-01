using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ICZ.Products.DB
{
	public class DBaseBasic
	{

		public static async Task<SqlDataReader> AsyncExeReaderMethod(SqlConnection conn, SqlCommand cmd)
		{
			if (conn != null && conn.State != ConnectionState.Open)
			{ await conn.OpenAsync(); }
			return await cmd.ExecuteReaderAsync();
		}
		public static async Task<int> AsyncExeExecuteMethod(SqlConnection conn, SqlCommand cmd)
		{
			if (conn != null && conn.State != ConnectionState.Open)
			{ await conn.OpenAsync(); }
			return await cmd.ExecuteNonQueryAsync();
		}
		public static async Task<object> AsyncExeScalarMethod(SqlConnection conn, SqlCommand cmd)
		{
			if (conn != null && conn.State != ConnectionState.Open)
			{ await conn.OpenAsync(); }
			return await cmd.ExecuteScalarAsync();
		}

	}

}
