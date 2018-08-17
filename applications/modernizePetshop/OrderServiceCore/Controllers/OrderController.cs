using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Npgsql;
using NpgsqlTypes;
using PetOrder.Configuration;
using ServiceSharedCore.Utilities;
using ServiceSharedCore.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderServiceCore.Controllers
{
    public class OrderController : Controller
    {
        private const string SQL_GET_HIGHEST_ORDERID = "SELECT OrderId FROM Orders Order by OrderId DESC LIMIT 1";
        //"SELECT OrderId FROM \"MSPETSHOP4ORDERS\".Orders Order by OrderId DESC LIMIT 1";
        private const string SQL_SELECT_ORDER = "SELECT o.OrderDate, o.UserId, o.CardType, o.CreditCard, o.ExprDate, o.BillToFirstName, o.BillToLastName, o.BillAddr1, o.BillAddr2, o.BillCity, o.BillState, o.BillZip, o.BillCountry, o.ShipToFirstName, o.ShipToLastName, o.ShipAddr1, o.ShipAddr2, o.ShipCity, o.ShipState, o.ShipZip, o.ShipCountry, o.TotalPrice, l.ItemId, l.LineNum, l.Quantity, l.UnitPrice FROM Orders o, lineitem l WHERE o.OrderId = :OrderId AND o.orderid = l.orderid";
        //"SELECT o.OrderDate, o.UserId, o.CardType, o.CreditCard, o.ExprDate, o.BillToFirstName, o.BillToLastName, o.BillAddr1, o.BillAddr2, o.BillCity, o.BillState, o.BillZip, o.BillCountry, o.ShipToFirstName, o.ShipToLastName, o.ShipAddr1, o.ShipAddr2, o.ShipCity, o.ShipState, o.ShipZip, o.ShipCountry, o.TotalPrice, l.ItemId, l.LineNum, l.Quantity, l.UnitPrice FROM \"MSPETSHOP4ORDERS\".Orders o, \"MSPETSHOP4ORDERS\".lineitem l WHERE o.OrderId = :OrderId AND o.orderid = l.orderid";

        private string PostgreSQLConnectionString { get; set; }

        public OrderController(IOptions<ConnectionSettings> settings)
        {
            PostgreSQLConnectionString = settings.Value.PostgreSQLConnectionString;
        }
        // GET api/products
        [HttpPost]
        [Route("order")]
        public void PostOrder([FromBody]OrderInfo orderInfo)
        {
            int? highestOrderId = DBFacilitator.GetInteger(PostgreSQLConnectionString, SQL_GET_HIGHEST_ORDERID, new List<Tuple<string, string, NpgsqlDbType>>());
            highestOrderId = highestOrderId.HasValue ? highestOrderId+1 : 0;

            var sb = new StringBuilder("");
            sb.Append("INSERT INTO Orders VALUES(");
            sb.Append("'" + highestOrderId + "', ");
            sb.Append("'" + orderInfo.UserId + "', ");
            sb.Append("'" + orderInfo.Date + "', ");
            sb.Append("'" + orderInfo.ShippingAddress.Address1 + "', ");
            sb.Append("'" + orderInfo.ShippingAddress.Address2 + "', ");
            sb.Append("'" + orderInfo.ShippingAddress.City + "', ");
            sb.Append("'" + orderInfo.ShippingAddress.State + "', ");
            sb.Append("'" + orderInfo.ShippingAddress.Zip + "', ");
            sb.Append("'" + orderInfo.ShippingAddress.Country + "', ");
            sb.Append("'" + orderInfo.BillingAddress.Address1 + "', ");
            sb.Append("'" + orderInfo.BillingAddress.Address2 + "', ");
            sb.Append("'" + orderInfo.BillingAddress.City + "', ");
            sb.Append("'" + orderInfo.BillingAddress.State + "', ");
            sb.Append("'" + orderInfo.BillingAddress.Zip + "', ");
            sb.Append("'" + orderInfo.BillingAddress.Country + "', ");
            sb.Append("'" + " UPS',");
            sb.Append("'" + orderInfo.OrderTotal + "', ");
            sb.Append("'" + orderInfo.BillingAddress.FirstName + "', ");
            sb.Append("'" + orderInfo.BillingAddress.LastName + "', ");
            sb.Append("'" + orderInfo.ShippingAddress.FirstName + "', ");
            sb.Append("'" + orderInfo.ShippingAddress.LastName + "', ");
            sb.Append("'" + orderInfo.AuthorizationNumber + "', ");
            sb.Append("'US-en');\n");


            sb.Append("INSERT INTO OrderStatus VALUES(");
            sb.Append("'" + highestOrderId + "', ");
            sb.Append("'" + "0', ");
            sb.Append("'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', ");
            sb.Append("'P'");
            sb.Append(");\n");

            foreach(LineItemInfo info in orderInfo.LineItems)
            {
                sb.Append("INSERT INTO LineItem VALUES(");
                sb.Append("'" + highestOrderId + "', ");
                sb.Append("'" + info.Line + "', ");
                sb.Append("'" + info.ItemId + "', ");
                sb.Append("'" + info.Quantity + "', ");
                sb.Append("'" + info.Price);
                sb.Append("');\n");

                sb.Append("UPDATE Inventory SET Qty = Qty - " + info.Quantity + " WHERE ItemId = '" + info.ItemId + "';\n");
            }

            foreach(LineItemInfo info in orderInfo.LineItems)
            {
                ServiceSharedCore.RedisCacheManager.Invalidate("item_" + info.ItemId);
                ServiceSharedCore.RedisCacheManager.Invalidate("itembyproduct_" + info.ProductId);
                ServiceSharedCore.RedisCacheManager.Invalidate("inventoryitem_" + info.ItemId);
            }

            DBFacilitator.ExecuteCommand(PostgreSQLConnectionString, sb.ToString(), new List<Tuple<string, string, NpgsqlDbType>>());
        }
        [HttpGet]
        [Route("order")]
        public OrderInfo GetOrder(int orderId)
        {
            using (var conn = new NpgsqlConnection(PostgreSQLConnectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(SQL_SELECT_ORDER, conn))
                {
                    cmd.Parameters.AddWithValue(":OrderId", orderId);

                    using (var rdr = cmd.ExecuteReader())
                    {
                        if (rdr.Read())
                        {
                            //Generate an order header from the first row
                            AddressInfo billingAddress = new AddressInfo(rdr.GetString(5), rdr.GetString(6), rdr.GetString(7), rdr.GetString(8), rdr.GetString(9), rdr.GetString(10), rdr.GetString(11), rdr.GetString(12), null, "email");
                            AddressInfo shippingAddress = new AddressInfo(rdr.GetString(13), rdr.GetString(14), rdr.GetString(15), rdr.GetString(16), rdr.GetString(17), rdr.GetString(18), rdr.GetString(19), rdr.GetString(20), null, "email");

                            var order = new OrderInfo(orderId, rdr.GetDateTime(0), rdr.GetString(1), null, billingAddress, shippingAddress, rdr.GetDecimal(21), null, null);

                            var lineItems = new List<LineItemInfo>();
                            //Create the lineitems from the first row and subsequent rows
                            do
                            {
                                lineItems.Add(new LineItemInfo(rdr.GetString(22), string.Empty, rdr.GetInt32(23), rdr.GetInt32(24), rdr.GetDecimal(25)));
                            } while (rdr.Read());

                            order.LineItems = new LineItemInfo[lineItems.Count];
                            lineItems.CopyTo(order.LineItems, 0);

                            return order;
                        }
                    }
                }
            }

            return null;
        }
    }
}