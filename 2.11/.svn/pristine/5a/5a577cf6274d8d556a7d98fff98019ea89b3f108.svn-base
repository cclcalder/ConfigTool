using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Exceedra.Controls.DynamicGrid.ViewModels;

namespace Exceedra.Web.api
{
    public class DynamicGridController : ApiController
    {
        // GET: api/DynamicGrid
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/DynamicGrid/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/DynamicGrid
        public RecordViewModel Get([FromBody]string value)
        {
            var iString = value;
            return new RecordViewModel();
        }

        // PUT: api/DynamicGrid/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/DynamicGrid/5
        public void Delete(int id)
        {
        }
    }
}
