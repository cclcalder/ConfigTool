using ConfigTool;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ConfigTool
{
    public static class LinqToSQL
    {
        public static void tableAccess()
        {
            var DB = new DataClasses1DataContext(); //DataContext object used to query database

            //Test table access (SYS_)
            var testSYS_Config = DB.SYS_Configs.Where(d => d.OptionItemDetail_Value == "1").ToList();
            //var testSYS_Screens = DB.SYS_Screens.Where(s => s.Customer_Hierarchy_Idx != 0).ToList();
            //var testSYS_ScreenTabs = DB.SYS_ScreenTabs.Where(t => t.IsDisplayed == true).ToList();
            var newSYS_Config = new SYS_Config { OptionItem = "CosTest", OptionItemDetail_Value = "3", MenuItem_Code = "123" };
                //DB.InsertSYS_Config(newSYS_Config)

            //Test table access (Products)
           //var testProdHierachies = DB.Dim_Product_Hierarchies.Select(h => h.Dim_Product_Levels.Select(l => l.ProdLevel_Code)).ToList();
           //var testProdLevels = DB.Dim_Product_Levels.Select(l => l.ProdLevel_Code).ToList();
           //
           //
           //IEnumerable<Dim_Product_Sku_Measure> ProductSkuMeasureNames = from p in DB.Dim_Product_Sku_Measures
           //                                                      where p.SkuMeasure_Name != null
           //                                                      orderby p.SkuMeasure_Idx
           //                                                      select p; //Retrieve all present sku measure names and order by idx
           ////Inserting and deleting tests
           //Dim_Product_Sku_Measure newMeasure = new Dim_Product_Sku_Measure { SkuMeasure_Name = "CosTest", SkuMeasure_Idx = 1 };
           //
           //Dim_Product_Sku_Measure newMeasureDelete = new Dim_Product_Sku_Measure { SkuMeasure_Name = "CosTestDelete", SkuMeasure_Idx = 2 };
           //    if (newMeasureDelete != null)
           //        DB.Dim_Product_Sku_Measures.DeleteOnSubmit(newMeasureDelete);
           //        DB.SubmitChanges();
           //
            //This will all be done on the web ^^
        }
    }
}
