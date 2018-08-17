using PetShop.IBLLStrategy;
using PetShop.Model;
using System.Configuration;
using Newtonsoft.Json;
using PetShop.IDAL;

namespace PetShop.BLL
{
    class OrderWS : IOrderStrategy, IOrder
    {
        string orderURL = ConfigurationManager.AppSettings["OrderBaseURL"] + "order";

        public OrderInfo GetOrder(int orderId)
        {
            var requestURL = orderURL;
            var result = Utilities.WebAPICall(requestURL);
            return JsonConvert.DeserializeObject<OrderInfo>(result);
        }

        public void Insert(OrderInfo order)
        {
            var requestURL = orderURL;
            var result = Utilities.WebAPICallWithData(requestURL, JsonConvert.SerializeObject(order));
        }

    }
}
