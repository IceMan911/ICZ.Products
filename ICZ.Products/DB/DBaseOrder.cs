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

		/*
		 * TODO ziskani vsech objednavek
		 *
		 * implementace by byla velmi podobna metode getOrderDetail() s rozdilem, ze by SQL dotaz nemel podminku na konkretni objednavku dle ID
		 * ziskani objednavky podle ID a vsech objednavek by mohlo byt implementovano ve stylu jedne metody, ktera dle parametru provede ziskani dat z DB
		 * Rozdil by byl v post processingu, ktery vytvari konceny objekt, kde nejsou duplicitni informace o objednavce
		 * Teoreticky pri velkem mnozstvi dat se nabizi omezeni v poctu zobrazeni zaznamu a nebo pouziti logiky (offset a fetch)
		 *
		 */

		/*
		 * TODO smazani objednavky
		 *
		 * implementace by byla velmi podobna metode deleteProduct, akorat s tim, ze by se meli dodatecne smazat i polozky objednavky
		 * nicmene pri navrhu databaze byla pouzito SQL kliceove slovo CASCADE, takze v pripade smazani objednavky dle ID se z tabulky OrderItem smazou i zaznamy dane objednavky
		 *
		 *
		 */

		/*
		 * TODO uprava objednavky
		 *
		 * implementace by byla velmi podobna metode createProduct, protoze je zapotrebi upravit zaznam v tabulce Order nebo OrderItem
		 * nejdulezitejsim faktorem je jaky pristup zvolit a jak urcit upravene zaznamy z FE
		 * pouzil bych opet prichozi JSON zpravu (Objekt v c#), ktery by obsahoval u kazdeho parametru parametr s bool
		 * parametr bool by urcoval, jestli se zmenila dana polozku a podle toho by se menil zaznam v DB
		 *
		 *
		 */

		/*
		 * TODO ziskani nejprodavanejsich produktu podle kategorie (vyuzit sql)
		 *
		 *	1. varianta vyuziti WITH, pocitany pouze odeslane objednavky
		 *
		WITH ProductWithRowNumber(category_name, product_name, total, row) as
		(SELECT c.category_name, pr.product_name, sum(oi.quantity) as total, ROW_NUMBER() OVER(partition by c.category_name ORDER BY SUM(oi.quantity) DESC) AS RowNumber
		FROM [OrderAndProduct].[dbo].[Order] o
		left join OrderItem oi on o.order_id = oi.order_id
		left join Product pr on oi.product_id = pr.product_id
		left join Category c on pr.category_id = c.category_id
		WHERE item_id is not null and o.status = 'shipped'
		group by c.category_name, pr.product_name)
		SELECT  category_name, product_name, total FROM ProductWithRowNumber
		WHERE row = 1
		*
		* 2. varianta vyuziti vnoreneho dotazu, kde jsou pocitany veskere objednavky
		*
		select category_name, product_name, total from
		(SELECT c.category_name, pr.product_name, sum(oi.quantity) as total, ROW_NUMBER() OVER(partition by c.category_name ORDER BY SUM(oi.quantity) DESC) AS RowNumber
		FROM [OrderAndProduct].[dbo].[Order] o
		left join OrderItem oi on o.order_id = oi.order_id
		left join Product pr on oi.product_id = pr.product_id
		left join Category c on pr.category_id = c.category_id
		WHERE item_id is not null
		group by c.category_name, pr.product_name) as temporatyQuery
		where RowNumber = 1
		*
		*/

		/*
		 * TODO prehled poctu objednavek po je mesicich za dane obdobi (vyuzit sql)
		 *
		 * varianta, ktera vyuziva funkce year, month a PARTITION by
		 *
		SELECT distinct
		YEAR(order_created) as YEAR,
		MONTH(order_created) as Month,
		count(order_id) OVER (PARTITION BY MONTH(order_created), YEAR(order_created)) AS numberOfOrders
		FROM [OrderAndProduct].[dbo].[Order]
		where order_created BETWEEN '2022-01-01 00:00:00.000' and '2023-01-01 00:00:00.000'
		order by YEAR, Month
		*
		*
		*/

	}
}
