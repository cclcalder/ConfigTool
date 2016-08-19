using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml.Linq;
using System.Extensions;
using System.Collections.Generic;
using System.Xml.Serialization;
using Model;
using WPF.UserControls.Listings;


namespace WPF.Test.XML
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void ReadXML()
        {
            XElement el = XElement.Parse(@"<RemoteCalc>
	                                    <TargetGrid>Promotion</TargetGrid>
	                                    <Action>MUL</Action>
		                                    <Products>
			                                    <Product>
				                                    <ID>101</ID>
				                                    <Factor>100</Factor>
			                                    </Product>
		                                    </Products>
                                    </RemoteCalc>");


           var TargetGrid = el.Element("TargetGrid").Value;
           var Action = el.Element("Action").Value;

           var ProductFactors = new Dictionary<int, decimal>();
            var products = el.Element("Products");

            if (products != null)
            {
                foreach (var product in products.Elements())
                {
                    var ID = Convert.ToInt32(product.Element("ID").Value);
                    var Factor = Convert.ToDecimal(product.Element("Factor").Value);

                   // ProductFactors.Add(Int32.Parse(product.Element("Product").Element("ID").Value), Decimal.Parse(product.Element("Product").Element("Factor").Value));
                }
            }


        }

        [TestMethod]
        public void OptionConversion()
        {
            var attr = XElement.Parse(@"<Root>
            <DataSource>dbo.VerticalGetDropdownDataData</DataSource>
                <DataSourceInput>
                    <ColumnCode>Support</ColumnCode>
                    <User_Idx>1</User_Idx>
                    <Input>
                        <ColumnCode>Mechanic</ColumnCode>
                        <Values>
                            <SelectedItem_Idx>1</SelectedItem_Idx>
                            <SelectedItem_Idx>2</SelectedItem_Idx>
                            <SelectedItem_Idx>3</SelectedItem_Idx>
                        </Values>
                    </Input>
                    <Input>
                        <ColumnCode>Feature</ColumnCode>
                        <Values>
                            <SelectedItem_Idx>1</SelectedItem_Idx>
                            <SelectedItem_Idx>2</SelectedItem_Idx>
                            <SelectedItem_Idx>3</SelectedItem_Idx>
                        </Values>
                    </Input>
                </DataSourceInput>
            </Root>");
            if (attr != null)
            {
                //we have some Data input, get dataset from WS



                //convert loaded XML to XMLin format
                var argument = new XElement("GetDropdownItems");

                var DataSourceInputXML = attr.Element("DataSourceInput");

                argument.Add(new XElement("ColumnCode", DataSourceInputXML.Element("ColumnCode").Value));
                argument.Add(new XElement("User_Idx", DataSourceInputXML.Element("User_Idx").Value));

                var ParentItems = new XElement("ParentItems");
                foreach (var x in DataSourceInputXML.Elements("Input"))
                {
                    var item = new XElement("Item");
                    item.Add(new XElement("ColumnCode", x.Element("ColumnCode").Value));

                    var values = new XElement("Values");

                    var ValuesXML = x.Element("Values");

                    foreach (var i in ValuesXML.Elements("SelectedItem_Idx"))
                    {
                        values.Add(new XElement("SelectedItem", i.Value));
                    }
                    item.Add(values);

                    ParentItems.Add(item);
                }

                argument.Add(ParentItems);


                Console.WriteLine(argument.ToString());


            }
        }



        //[TestMethod]
        //public void UserDefault()
        //{
        //    var res = new UserSelectedDefaults();
        //    res.ScreenCode = "test";
        //    res.Products= new List<Idx>()
        //    {
        //        new Idx("r"),new Idx("t"),new Idx("l")
        //    };

        //    res.Customers = new List<Idx>()
        //    {
        //        new Idx("a"),new Idx("b"),new Idx("c")
        //    };

        //    var x = Model.XMLExtension.Serialize(res);


        //}

       
        
    }
}
