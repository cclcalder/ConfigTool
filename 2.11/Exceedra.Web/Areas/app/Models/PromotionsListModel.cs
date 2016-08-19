using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using Exceedra.Controls.DynamicGrid.ViewModels;
using Exceedra.Web.Models;
using Model;
using Website.Webservices;

namespace Exceedra.Web.Areas.app.Models
{
    public class PromotionsListModel
    {
        private ModelAccess _access = new ModelAccess();
        public string UserName { get; set; }
        public DataTable DT1 { get; set; }
        public RecordViewModel GridXML { get; set; }

      

        public PromotionsListModel()
        {
            var proc = StoredProcedure.Promotion.GetPromotions;
            var XML_In = XElement.Parse(@"<GetPromotions>
  <User_Idx>10</User_Idx>
  <SalesOrg_Idx>1</SalesOrg_Idx>
  <Screen_Code>PROMOTION</Screen_Code>
  <Statuses>
    <Idx>1</Idx>
    <Idx>-1</Idx>
    <Idx>10</Idx>
    <Idx>1000</Idx>
    <Idx>1002</Idx>
    <Idx>1003</Idx>
    <Idx>12</Idx>
    <Idx>14</Idx>
    <Idx>-2</Idx>
    <Idx>3</Idx>
    <Idx>4</Idx>
    <Idx>5</Idx>
    <Idx>6</Idx>
    <Idx>7</Idx>
    <Idx>8</Idx>
    <Idx>9</Idx>
  </Statuses>
  <Customers>
    <Idx>1</Idx>
    <Idx>20042</Idx>
    <Idx>20043</Idx>
    <Idx>20073</Idx>
    <Idx>20088</Idx>
    <Idx>4</Idx>
    <Idx>5</Idx>
    <Idx>6</Idx>
  </Customers>
  <Products>
    <Idx>1</Idx>
    <Idx>10</Idx>
    <Idx>15006</Idx>
    <Idx>15044</Idx>
    <Idx>15048</Idx>
    <Idx>15088</Idx>
    <Idx>15095</Idx>
    <Idx>15119</Idx>
    <Idx>15125</Idx>
    <Idx>15129</Idx>
    <Idx>15134</Idx>
    <Idx>15135</Idx>
    <Idx>15138</Idx>
    <Idx>15171</Idx>
    <Idx>15175</Idx>
    <Idx>15184</Idx>
    <Idx>15185</Idx>
    <Idx>15233</Idx>
    <Idx>15248</Idx>
    <Idx>15260</Idx>
    <Idx>15272</Idx>
    <Idx>15273</Idx>
    <Idx>15318</Idx>
    <Idx>15331</Idx>
    <Idx>15354</Idx>
    <Idx>15355</Idx>
    <Idx>15377</Idx>
    <Idx>15386</Idx>
    <Idx>15393</Idx>
    <Idx>15397</Idx>
    <Idx>15406</Idx>
    <Idx>15416</Idx>
    <Idx>2</Idx>
    <Idx>3</Idx>
    <Idx>4</Idx>
    <Idx>5</Idx>
    <Idx>50000</Idx>
    <Idx>50001</Idx>
    <Idx>6</Idx>
    <Idx>60000</Idx>
    <Idx>60001</Idx>
    <Idx>7</Idx>
    <Idx>8</Idx>
    <Idx>9</Idx>
  </Products>
  <ListingsGroup_Idx>1</ListingsGroup_Idx>
  <Start>2015-01-01</Start>
  <End>2017-01-01</End>
  <Graph_Idx></Graph_Idx>
  <UsePowerEditor>0</UsePowerEditor>
</GetPromotions>");

            var res = Model.DataAccess.WebServiceProxy.ParseXml(proc, XML_In, StaticWS.Run(proc, XML_In.ToString(), "ExceedraConn_v2_10"));


            //var GridXMl = new RecordViewModel(_access.GetGridXML(10, 10));
            GridXML  = new RecordViewModel(res);
            //return GridXMl.Records;

            // var sd = XElement.Parse(StaticData.RobGrid);

            // //UserName = Model.User.CurrentUser.DisplayName;
            // GridXML = new RecordViewModel(sd) ;// new RecordViewModel(_access.GetGridXML(10, 10));

            // //DataTable dt = new DataTable("MyTable");
            // //foreach (var p in GridXML.Records[0].Properties)
            // //{
            // //    dt.Columns.Add(new DataColumn(p.HeaderText, typeof(string)));
            // //}

            // //foreach (var r in GridXML.Records)
            // //{
            // //    DataRow row = dt.NewRow();
            // //    foreach (var prop in r.Properties)
            // //    {
            // //        row[prop.HeaderText] = prop.Value;
            // //    }

            // //    dt.Rows.Add(row);
            // //}

            //// DT1 = dt;
        }

    }
}