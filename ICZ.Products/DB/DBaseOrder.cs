﻿using ICZ.Products.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace ICZ.Products.DB
{
	public class DBaseOrder : DBaseBasic
	{
		SqlConnection mConnection;
		IConfiguration config;
		public DBaseOrder()
		{ }
		public DBaseOrder(IConfiguration config)
		{
			this.config = config;
		}
		public async Task<List<OrderFull>> getOrderDetail(Guid idOrder, string ConnectionStringMSSQL)
		{
			List<OrderFull> lReturn = new List<OrderFull>();
			SqlDataReader lReader;

			try
			{
				using (mConnection = new SqlConnection(ConnectionStringMSSQL))
				{
					SqlCommand lCmd;
					string command = string.Empty;
					command = @"SELECT o.order_id as IdOrder, o.status as Status, o.order_created as OrderCreated, oi.item_id as ItemId, oi.product_id as IdProduct, pr.product_name as ProductName,
								pr.category_id as IdCategory, c.category_name as CategoryName, oi.quantity as Quantity, oi.unit_price as UnitCost
								FROM [OrderAndProduct].[dbo].[Order] o
								left join OrderItem oi on o.order_id = oi.order_id
								left join Product pr on oi.product_id = pr.product_id
								left join Category c on pr.category_id = c.category_id
								WHERE item_id is not null AND o.order_id = @IdOrder";
					using (lCmd = new SqlCommand(command, mConnection))
					{
						lCmd.Parameters.AddWithValue("@IdOrder", idOrder);

						lReader = AsyncExeReaderMethod(mConnection, lCmd).Result;
						if (lReader.HasRows)
						{
							using (DataTable lTable = new DataTable())
							{
								lTable.Load(lReader);
								int lSentinel = lTable.Rows.Count;
								for (int i = 0; i < lSentinel; i++)
									lReturn.Add(ReflectPropertyInfo.ReflectType<OrderFull>(lTable.Rows[i]));
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

		public async Task<bool> createOrder(OrderCreate orderCreate, string ConnectionStringMSSQL)
		{
			bool lReturn = false;
			try
			{
				using (mConnection = new SqlConnection(ConnectionStringMSSQL))
				{
					SqlCommand lCmd;
					SqlDataReader lReader;
					string command = string.Empty;
					int iAffectedRows = 0;
					Guid orderId = Guid.Empty;

					command = @"INSERT INTO [Order] (order_id, status, order_created) output inserted.order_id VALUES (NEWID(), @Status, getDate())";
					using (lCmd = new SqlCommand(command, mConnection))
					{
						lCmd.Parameters.AddWithValue("@Status", orderCreate.Status);

						lReader = AsyncExeReaderMethod(mConnection, lCmd).Result;
						if (lReader.HasRows)
						{
							while (lReader.Read())
							{
								orderId = lReader.GetGuid(0);
							}
						}
						lReader.Close();
					}

                    if (orderId != Guid.Empty)
                    {
						foreach (OrderItemCreate orderItem in orderCreate.Items)
						{
							command = @"INSERT INTO [OrderItem] (item_id, order_id, product_id, quantity, unit_price) SELECT NEWID(), @IdOrder, product_id, @Quantity, product_cost FROM Product WHERE product_id = @IdProduct";
							using (lCmd = new SqlCommand(command, mConnection))
							{
								lCmd.Parameters.AddWithValue("@IdOrder", orderId);
								lCmd.Parameters.AddWithValue("@IdProduct", orderItem.IdProduct);
								lCmd.Parameters.AddWithValue("@Quantity", orderItem.Quantity);


								iAffectedRows = AsyncExeExecuteMethod(mConnection, lCmd).Result;
                                switch (iAffectedRows)
                                {
									case 0:
										//_logger.LogError($"PROBLEM: OrderItem {orderId}:{orderItem.IdProduct}:{orderItem.Quantity} Not Created");
										lReturn = false;
										break;
									case 1:
										//_logger.LogInformation($"OrderItem {orderId}:{orderItem.IdProduct}:{orderItem.Quantity} Created");
										lReturn = true;
										break;
									case -1:
										//_logger.LogError($"OrderIteam {orderId}:{orderItem.IdProduct}:{orderItem.Quantity} doesnt work");
										lReturn = false;
										break;
								}
                                if (!lReturn)
                                {
									break;
									// Pokud selze jedno pridani polozky v objednavce, tak metoda prestava vkladat ostatni polozky z objednavky a metoda konci.
									// TODO bylo by lepsi pouzit SQL transakci (SqlTransaction), kdy mohu pouzit metody Commit() nebo RollBack v pripade neuspesneho vlozeni dat
									// Bohuzel nemam zkusenosti s SqlTransaction a nevim zda se tato praktika pouziva, posledni zkusenost v JAVE na VS
								}
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
				//_logger.LogError(string.Format("InvalidOperationException: Message:{0} StackTrace{1}", iOpEx.Message, iOpEx.StKCackTrace));
			}
			catch (Exception ex) // this will catch all exceptions
			{
				//_logger.LogError(string.Format("Exception: Message:{0} StackTrace{1}", ex.Message, ex.StackTrace));
			}
			return lReturn;
		}

		//public async Task<bool> updateProduct(Product product, string ConnectionStringMSSQL)
		//{
		//	bool lReturn = false;
		//	SqlConnection mConnection;
		//	try
		//	{
		//		using (mConnection = new SqlConnection(ConnectionStringMSSQL))
		//		{
		//			SqlCommand lCmd;
		//			string command = string.Empty;
		//			int iAffectedRows = 0;

		//			command = @"UPDATE Product SET product_name = @ProductName, product_cost = @ProductCost, category_id = @IdCategory WHERE product_id = @IdProduct";
		//			using (lCmd = new SqlCommand(command, mConnection))
		//			{
		//				lCmd.Parameters.AddWithValue("@ProductName", product.ProductName);
		//				lCmd.Parameters.AddWithValue("@ProductCost", product.ProductCost);
		//				lCmd.Parameters.AddWithValue("@IdCategory", product.IdCategory);
		//				lCmd.Parameters.AddWithValue("@IdProduct", product.IdProduct);

		//				iAffectedRows = AsyncExeExecuteMethod(mConnection, lCmd).Result;
		//				if (iAffectedRows == 1)
		//					lReturn = true;
		//			}
		//		}
		//		mConnection.Close();
		//	}
		//	catch (SqlException sqlEx) // This will catch all SQL exceptions
		//	{
		//		lReturn = false;
		//		//_logger.LogError(string.Format("SqlException: Message:{0} StackTrace{1}", sqlEx.Message, sqlEx.StackTrace));
		//	}
		//	catch (InvalidOperationException iOpEx) // This will catch SqlConnection Exception
		//	{
		//		lReturn = false;
		//		//_logger.LogError(string.Format("InvalidOperationException: Message:{0} StackTrace{1}", iOpEx.Message, iOpEx.StackTrace));
		//	}
		//	catch (Exception ex) // this will catch all exceptions
		//	{
		//		lReturn = false;
		//		//_logger.LogError(string.Format("Exception: Message:{0} StackTrace{1}", ex.Message, ex.StackTrace));
		//	}
		//	return lReturn;
		//}

		//public async Task<bool> deleteProduct(Guid idProduct, string ConnectionStringMSSQL)
		//{
		//	bool lReturn = false;
		//	SqlConnection mConnection;
		//	try
		//	{
		//		using (mConnection = new SqlConnection(ConnectionStringMSSQL))
		//		{
		//			SqlCommand lCmd;
		//			string command = string.Empty;
		//			int iAffectedRows = 0;

		//			command = @"DELETE FROM Product WHERE product_id = @IdProduct";
		//			using (lCmd = new SqlCommand(command, mConnection))
		//			{
		//				lCmd.Parameters.AddWithValue("@IdProduct", idProduct);

		//				iAffectedRows = AsyncExeExecuteMethod(mConnection, lCmd).Result;
		//				if (iAffectedRows == 1)
		//					lReturn = true;
		//			}
		//		}
		//		mConnection.Close();
		//	}
		//	catch (SqlException sqlEx) // This will catch all SQL exceptions
		//	{
		//		lReturn = false;
		//		//_logger.LogError(string.Format("SqlException: Message:{0} StackTrace{1}", sqlEx.Message, sqlEx.StackTrace));
		//	}
		//	catch (InvalidOperationException iOpEx) // This will catch SqlConnection Exception
		//	{
		//		lReturn = false;
		//		//_logger.LogError(string.Format("InvalidOperationException: Message:{0} StackTrace{1}", iOpEx.Message, iOpEx.StackTrace));
		//	}
		//	catch (Exception ex) // this will catch all exceptions
		//	{
		//		lReturn = false;
		//		//_logger.LogError(string.Format("Exception: Message:{0} StackTrace{1}", ex.Message, ex.StackTrace));
		//	}
		//	return lReturn;
		//}

		//public async Task<List<Product>> getProducts(string ConnectionStringMSSQL)
		//{
		//	List<Product> lReturn = new List<Product>();
		//	SqlDataReader lReader;

		//	try
		//	{
		//		using (mConnection = new SqlConnection(ConnectionStringMSSQL))
		//		{
		//			SqlCommand lCmd;
		//			string command = string.Empty;
		//			command = @"SELECT pd.product_id as IdProduct, pd.product_name as ProductName, ct.category_id as IdCategory, ct.category_name as CategoryName, pd.product_cost as ProductCost FROM Product pd LEFT JOIN Category ct on pd.category_id = ct.category_id";
		//			using (lCmd = new SqlCommand(command, mConnection))
		//			{
		//				lReader = AsyncExeReaderMethod(mConnection, lCmd).Result;
		//				if (lReader.HasRows)
		//				{
		//					using (DataTable lTable = new DataTable())
		//					{
		//						lTable.Load(lReader);
		//						int lSentinel = lTable.Rows.Count;
		//						for (int i = 0; i < lSentinel; i++)
		//							lReturn.Add(ReflectPropertyInfo.ReflectType<Product>(lTable.Rows[i]));
		//					}
		//				}
		//			}
		//		}
		//		mConnection.Close();
		//	}
		//	catch (SqlException sqlEx) // This will catch all SQL exceptions
		//	{
		//		//_logger.LogError(string.Format("SqlException: Message:{0} StackTrace{1}", sqlEx.Message, sqlEx.StackTrace));
		//	}
		//	catch (InvalidOperationException iOpEx) // This will catch SqlConnection Exception
		//	{
		//		//_logger.LogError(string.Format("InvalidOperationException: Message:{0} StackTrace{1}", iOpEx.Message, iOpEx.StackTrace));
		//	}
		//	catch (Exception ex) // this will catch all exceptions
		//	{
		//		//_logger.LogError(string.Format("Exception: Message:{0} StackTrace{1}", ex.Message, ex.StackTrace));
		//	}
		//	return lReturn;
		//}

	}
}