//using System;
//using System.Collections.Generic;
//using System.Data.Linq;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Web.UI.WebControls;

//namespace ConfigTool.Script
//{
//    class Merge
//    {
//        //already knows target and source databases (only ones connected)
//        public static void UpsertToMerge(string tgtName, string srcName, Array values, string queryType, string pKey)
//        {
//            //this is JUST a SQL script, not more complex than a text file.
//            //where get primary keys from?
//            string template = @"MERGE INTO [sometargettable] TGT
//                            USING (
//                                         [somesourcetable]
//                                  ) SRC
//                            ON [PRIMARY KEY]
//                            WHEN MATCHED AND [STUFF UPDATING]
//                            THEN UPDATE
//                                 SET[STUFF UPDATING]
//                            WHEN NOT MATCHED BY TARGET
//                            THEN INSERT
//                                 (
//                                   [STUFF UPDATING]
//                                 ) 
//                                 VALUES
//                                 (
//                                   [STUFF UPDATING]
//                                 )
//                            WHEN NOT MATCHED BY SOURCE THEN DELETE;";

//            string[] query = template;

//            System.IO.File.WriteAllLines(@"C:\Users\CosimaCalder\Documents\Visual Studio 2015\Projects\ConfigTool\ConfigTool\Script", query);
//        }
//    }
//}

///*
 
//MERGE INTO [sometable] TGT 
//USING      (
//             [sometable] 
//           ) SRC
//ON         [PRIMARY KEY]
//WHEN MATCHED AND [STUFF I'M UPDATING]
//THEN UPDATE 
//     SET [STUFF I'M UPDATING]
//WHEN NOT MATCHED BY TARGET 
//THEN INSERT
//     (
//       [STUFF I'M UPDATING]
//     ) 
//     VALUES 
//     (
//       [STUFF I'M UPDATING]
//     )
//WHEN NOT MATCHED BY SOURCE THEN DELETE;
     
     
//     */
