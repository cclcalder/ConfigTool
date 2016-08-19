using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Exceedra.Website.api
{
    public class ApiPromotionsController : ApiController
    {
        // GET: api/ApiPromotions
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/ApiPromotions/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/ApiPromotions
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/ApiPromotions/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/ApiPromotions/5
        public void Delete(int id)
        {
        }
    }
}
