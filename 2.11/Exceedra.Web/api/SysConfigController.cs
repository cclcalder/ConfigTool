using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Exceedra.Web.Models;

namespace Exceedra.Web.api
{
    public class SysConfigController : ApiController
    {
        // GET: api/SysConfig
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/SysConfig/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/SysConfig
        //public GateWayData Post(person value)
        //{
        //    return new LoadGateKeeper(value).Current;
        //}

        // PUT: api/SysConfig/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/SysConfig/5
        public void Delete(int id)
        {
        }
    }
}
