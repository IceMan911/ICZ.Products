using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ICZ.Products.Models;

namespace ICZ.Products.Helpers
{
    public static class Utils
    {
        public static Order getOrderDetail(List<OrderFull> orders)
        {
            Order lReturn = new Order();
            try
            {
                foreach (var item in orders)
                {
                    if (lReturn.IdOrder == Guid.Empty)
                    {
                        lReturn.IdOrder = item.IdOrder;
                        lReturn.Status = item.Status;
                        lReturn.OrderCreated = item.OrderCreated;
                    }

                    OrderItem orderItem = new OrderItem();
                    orderItem.ItemId = item.ItemId;
                    orderItem.IdProduct = item.IdProduct;
                    orderItem.ProductName = item.ProductName;
                    orderItem.IdCategory = item.IdCategory;
                    orderItem.CategoryName = item.CategoryName;
                    orderItem.Quantity = item.Quantity;
                    orderItem.UnitCost = item.UnitCost;

                    lReturn.Items.Add(orderItem);
                }

            }
            catch
            {
                // Logging error
            }

            return lReturn;
        }

    }
}
