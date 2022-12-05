using ICZ.Products.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace ICZ.Products.DB
{
    public class DBaseProduct : DBaseBasic
    {
        SqlConnection mConnection;
        IConfiguration config;
        public DBaseProduct()
        { }
        public DBaseProduct(IConfiguration config)
        {
            this.config = config;
        }
        public async Task<Product> getProductDetail(Guid idProduct, string ConnectionStringMSSQL)
        {
            Product lReturn = null;
            SqlDataReader lReader;

            try
            {
                using (mConnection = new SqlConnection(ConnectionStringMSSQL))
                {
                    SqlCommand lCmd;
                    string command = string.Empty;
                    command = @"SELECT pd.product_id as IdProduct, pd.product_name as ProductName, ct.category_id as IdCategory, ct.category_name as CategoryName, pd.product_cost as ProductCost FROM Product pd LEFT JOIN Category ct on pd.category_id = ct.category_id WHERE pd.product_id = @IdProduct";
                    using (lCmd = new SqlCommand(command, mConnection))
                    {
                        lCmd.Parameters.AddWithValue("@IdProduct", idProduct);

                        lReader = AsyncExeReaderMethod(mConnection, lCmd).Result;
                        if (lReader.HasRows)
                        {
                            using (DataTable lTable = new DataTable())
                            {
                                lTable.Load(lReader);
                                int lSentinel = lTable.Rows.Count;
                                for (int i = 0; i < lSentinel; i++)
                                    lReturn = ReflectPropertyInfo.ReflectType<Product>(lTable.Rows[i]);
                            }
                        }
                    }
                }
                mConnection.Close();
            }
            catch (SqlException sqlEx) // This will catch all SQL exceptions
            {
                //_logger.LogError(string.Format("SqlException: Message:{0} StackTrace{1}", sqlEx.Message, sqlEx.StackTrace));
            }
            catch (InvalidOperationException iOpEx) // This will catch SqlConnection Exception
            {
                //_logger.LogError(string.Format("InvalidOperationException: Message:{0} StackTrace{1}", iOpEx.Message, iOpEx.StackTrace));
            }
            catch (Exception ex) // this will catch all exceptions
            {
                //_logger.LogError(string.Format("Exception: Message:{0} StackTrace{1}", ex.Message, ex.StackTrace));
            }
            return lReturn;
        }

		public async Task<bool> createProduct(Product product, string ConnectionStringMSSQL)
		{
            bool lReturn = false;
			try
			{
				using (mConnection = new SqlConnection(ConnectionStringMSSQL))
				{
					SqlCommand lCmd;
					string command = string.Empty;
                    int iAffectedRows = 0;

                    command = @"INSERT INTO Product (product_id, product_name, product_cost, category_id)
							VALUES (NEWID(), @ProductName, @ProductCost, @IdCategory)";
					using (lCmd = new SqlCommand(command, mConnection))
					{
						lCmd.Parameters.AddWithValue("@ProductName", product.ProductName);
						lCmd.Parameters.AddWithValue("@ProductCost", product.ProductCost);
						lCmd.Parameters.AddWithValue("@IdCategory", product.IdCategory);

                        iAffectedRows = AsyncExeExecuteMethod(mConnection, lCmd).Result;
                        if (iAffectedRows != 1)
                            lReturn = false;
                        else
                            lReturn = true;

					}
                }
				mConnection.Close();
			}
			catch (SqlException sqlEx) // This will catch all SQL exceptions
			{
				//_logger.LogError(string.Format("SqlException: Message:{0} StackTrace{1}", sqlEx.Message, sqlEx.StackTrace));
			}
			catch (InvalidOperationException iOpEx) // This will catch SqlConnection Exception
			{
				//_logger.LogError(string.Format("InvalidOperationException: Message:{0} StackTrace{1}", iOpEx.Message, iOpEx.StKCackTrace));
			}
			catch (Exception ex) // this will catch all exceptions
			{
				//_logger.LogError(string.Format("Exception: Message:{0} StackTrace{1}", ex.Message, ex.StackTrace));
			}
			return lReturn;
		}

	}
}
