using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Exceedra.Common;

namespace Model.Entity.DemandPlanning
{
//    const string dummy @"               
//             <UpliftResponse >
//                    <Parameters />
//                     <Uplifts>
//                           <UpliftRow>
//                               <Measures>
//                                   <UpliftMeasure>
//                                      <Code>SysUpliftPc</Code>
//                                      <Value>150</Value>
//                                   </UpliftMeasure>
//                                   <UpliftMeasure>
//                                        <Code>SysUpliftMin</Code>.
//                                        <Value>-233067.80799627936</Value>
//                                   </UpliftMeasure>
//                                   <UpliftMeasure>
//                                       <Code>SysUpliftMax</Code>
//                                       <Value>233367.80799627936</Value>
//                                   </UpliftMeasure>
//                              </Measures>
//                              <Promo_Idx>1</Promo_Idx>
//                              <Sku_Idx>15095</Sku_Idx>
//                           </UpliftRow>                     
//                      </Uplifts>
//              </UpliftResponse>";

    public  class UpliftResponse
    {


        public List<string> Parameters { get; set; }
        public List<UpliftRow> Uplifts { get; set; }



        public UpliftResponse(XElement el)
        {
           
         var rows = el.Element("Uplifts");

         Uplifts = rows != null
                ? rows.Elements().Select(m => new UpliftRow(m)).ToList()
                : new List<UpliftRow>();
        }


    }

    public class UpliftRow
    {
        public List<UpliftMeasure> Measures { get; set; }
        public string Promo_Idx { get; set; }
        public string Sku_Idx { get; set; } // product sku to look for (ie the row of the grid)


        public UpliftRow(XElement el)
        {
            Promo_Idx = el.GetValueOrDefault<string>("Promo_Idx");
            Sku_Idx = el.GetValueOrDefault<string>("Sku_Idx");

            var rows = el.Element("Measures");
            Measures = rows != null
                ? rows.Elements().Select(m => new UpliftMeasure(m)).ToList()
                : new List<UpliftMeasure>();

        }

    }

    public class UpliftMeasure
    {
        public string Code { get; set; } //column code to look for in the grid
        public string Value { get; set; } // value to set in this column / product sku combo

         public UpliftMeasure(XElement el)
         {
              Code = el.GetValueOrDefault<string>("Code");
              Value = el.GetValueOrDefault<string>("Value");
         }

    }
}
