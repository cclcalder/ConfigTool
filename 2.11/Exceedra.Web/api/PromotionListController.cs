using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Xml.Linq;
using Exceedra.Common.Logging;
using Exceedra.Controls.DynamicGrid.ViewModels;
using Exceedra.Web.Models;
using Model.DataAccess;
using Model.DataAccess.Generic;
using Website.Webservices;

namespace Exceedra.Web.api
{
    public class PromotionListController : ApiController
    {
        private ModelAccess _access = new ModelAccess();
        private PromotionAccess _promoAccess = new PromotionAccess();

        // GET: api/PromotionList
        //public IEnumerable<Exceedra.Controls.DynamicGrid.Models.Record> Get()
        //{
        
        //}

        // GET: api/PromotionList/5
        public string Get(int id)
        {
            return _access.GetGridXML(10, 100).ToString();
        }

        // POST: api/PromotionList
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/PromotionList/5
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/PromotionList/5
        public void Delete(int id)
        {
        }


       
    }
}