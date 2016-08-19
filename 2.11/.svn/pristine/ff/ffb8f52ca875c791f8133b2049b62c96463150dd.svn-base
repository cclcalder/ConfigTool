using System;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
 
using System.Collections.Generic;

namespace WPF.Test.XML
{
    [TestClass]
    public class DynXmlTemplating
    {
        private const string _simple = @"<GetItems>
                                            <User_Idx />
                                            <MenuItem_Idx />
                                            <GridItem_Code />
                                            <SelectedItem_Idx />
                                        </GetItems>";


           private const string _complex = @"
	                <DataSourceInput>
			                <User_Idx />
			                <Promo_Idx />
			                <ControlsWithInput>
				                <Column>
				                  <ColumnCode />
				                  <Values/>
				                </Column>	
				                <Column>
				                  <ColumnCode>AssignedProfile_Idx</ColumnCode>
				                  <Values/>
				                </Column>	
			                </ControlsWithInput>
		                </DataSourceInput>";



           #region Simple




           //[TestMethod]
           //public void SimpleTest()
           //{
           //    var rip = XElement.Parse(_simple);

           //    foreach (var node in rip.Elements())
           //    {
           //        var to = new SimpleObj() { MenuItem_Idx = "1", User_Idx = "3" };

           //        var t = node.Name;
           //        var obj = FindReflect.GetPropValue(to, t.ToString());

           //    }

           //}


           public class SimpleObj
           {

               public string User_Idx { get; set; }
               public string MenuItem_Idx { get; set; }

           }

           #endregion




        [TestMethod]
        public void ComplexTest()
        {
            var rip = XElement.Parse(_complex);

            foreach (var node in rip.Elements())
            {
                switch (node.Name.ToString())
                {
                    case "User_Idx":
                        node.Value = "1";//Model.User.CurrentUser.ID;
                        break;
                    case "Promo_Idx":
                        node.Value = "230";
                        break;
                    case "ControlsWithInput":

                        var columns = node.Elements("Column");

                        foreach (var c in columns)
                        {
                            var code = c.Element("ColumnCode").Value;

                            if (!string.IsNullOrEmpty(code))
                            {
                                // get property based on column code and find the selected items


                                //set selected column values into XML
                                for (int i = 1; i <= 5; i++)
                                {
                                    c.Element("Values").Add(new XElement("Value",i));
                                }
                               

                            }

                        }

                        break;
                   

                }

            }

            Console.Write(rip.ToString());

        }


        public class ComplexObj
        {
            public string User_Idx { get; set; }
            public string Promo_Idx { get; set; }

            public CWI ControlWithInput { get; set; }
            
        }


      public  class CWI
        {
          public List<Column> Columns { get; set; }
        


        }


        public class Column
        {
            public string ColumnCode { get; set; }
            public List<string> Values{ get; set; }
        }
    }
}
