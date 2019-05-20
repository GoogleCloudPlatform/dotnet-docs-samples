using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace TranslateWorker.Controllers
{
    class PubsubMessage {
        public string data { get; set; }
        public string messageId { get; set; }
        public Dictionary<string, string> attributes { get; set; }
    }

    public class PostMessage {
        public PubsubMessage message { get; set; }
        public string subscription { get; set; }        
    }


    [Route("api/[controller]")]
    [ApiController]
    public class TranslateController : ControllerBase
    {
        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
            JsonConvert.

        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
