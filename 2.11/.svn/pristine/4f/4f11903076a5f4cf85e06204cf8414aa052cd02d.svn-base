using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Configuration;
using System.Web;
using System.Xml.Linq;
using Model;

namespace Exceedra.Web.Models
{
    public class ModelAccess
    {
        private const string GridDataProc = StoredProcedure.Dummy.GetHorizontalGrid;
        private const string GridDataArgs =@"<GetData><Columns>{0}</Columns><Rows>{1}</Rows></GetData>";


        public XElement GetGridXML(int columns, int rows)
        {
            var x = Model.DataAccess.WebServiceProxy.Call(GridDataProc,XElement.Parse(string.Format(GridDataArgs, columns, rows)));

            return x;
        }

    }
}