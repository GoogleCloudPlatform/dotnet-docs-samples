using PetShop.IDAL;
using System.Collections.Generic;
using PetShop.Model;
using System.Configuration;
using Newtonsoft.Json;

namespace PetShop.BLL
{
    public class ProductWS : IProduct
    {
        string orderURL = ConfigurationManager.AppSettings["ProductBaseURL"] + "product";
        public ProductInfo GetProduct(string productId)
        {
            var requestURL = orderURL;
            var result = Utilities.WebAPICall(requestURL + "/" + productId);
            return JsonConvert.DeserializeObject<ProductInfo>(result);
        }

        public IList<ProductInfo> GetProductsByCategory(string category)
        {
            var requestURL = orderURL;
            var result = Utilities.WebAPICall(requestURL + "/productbycategory/" + category);
            return JsonConvert.DeserializeObject<IList<ProductInfo>>(result);
        }

        public IList<ProductInfo> GetProductsBySearch(string[] keywords)
        {
            var requestURL = orderURL + "/productbykeyword/";
            foreach(var keyword in keywords)
            {
                requestURL += keyword + ",";
            }
            requestURL = requestURL.Remove(requestURL.Length - 2);
            var result = Utilities.WebAPICall(requestURL);
            return JsonConvert.DeserializeObject<IList<ProductInfo>>(result);
        }
    }
}
