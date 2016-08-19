using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
//using WPFPoC.ViewModels;
using System.Xml.Linq;

namespace WPF.Test.DynamicRows
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void CheckVerticalGridDropdownData()
        {

            var r = XElement.Parse(@"  <Results>
                    <DropdownItem>
                      <Item_Idx>1</Item_Idx>
                      <Item_Name>a</Item_Name>
                      <Item_IsSelected>true</Item_IsSelected>
                    </DropdownItem>
                    <DropdownItem>
                      <Item_Idx>2</Item_Idx>
                      <Item_Name>b</Item_Name>
                      <Item_IsSelected>true</Item_IsSelected>
                    </DropdownItem>
                    <DropdownItem>
                      <Item_Idx>3</Item_Idx>
                      <Item_Name>c</Item_Name>
                      <Item_IsSelected>true</Item_IsSelected>
                    </DropdownItem>
                  </Results>");


            //var res = Option.Get(r,"");


        }
    }
}
