//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;

//namespace WebApplication2.Controllers
//{
//    public class OldTableMethod
//    {
//        #region Old Table Stuff
//        /* --------------------------------------- */
//        //OLD TABLE FORMAT STUFF

//        // GET: All SYS_Configs
//        public JsonResult GetAllSYS_Configs()
//        {
//            using (DataClasses1DataContext contextObj = new DataClasses1DataContext())
//            {
//                var SYS_ConfigList = contextObj.SYS_Config.ToList();

//                var data = new ColDataToHtml();
//                data.names = ColumnMapping(SYS_ConfigList[0], "name");
//                data.inputTypes = new Tuple<List<string>, List<bool>>((ColumnMapping(SYS_ConfigList[0], "type")), ColumnMapping(SYS_ConfigList[0], "null").Select(b => Convert.ToBoolean(b)).ToList());
//                data.parents = ColumnMapping(SYS_ConfigList[0], "relations");
//                data.children = ColumnMapping(SYS_ConfigList[0], "relations");

//                var jsonReturn = new Tuple<List<SYS_Config>, ColDataToHtml>(SYS_ConfigList, data);

//                return Json(jsonReturn, JsonRequestBehavior.AllowGet);
//            }
//        }

//        //GET: SYS_Config by Id
//        public JsonResult GetSYS_ConfigById(string id)
//        {
//            using (DataClasses1DataContext contextObj = new DataClasses1DataContext())
//            {
//                var SYS_ConfigId = Convert.ToInt32(id);

//                var getSYS_ConfigById = contextObj.SYS_Config.FirstOrDefault(s => s.OptionItem_ID == SYS_ConfigId);
//                return Json(getSYS_ConfigById, JsonRequestBehavior.AllowGet);
//            }
//        }

//        //Update SYS_Config
//        public string UpdateSYS_Config(SYS_Config SYS_ConfigRecord)
//        {
//            if (SYS_ConfigRecord != null)
//            {
//                using (DataClasses1DataContext contextObj = new DataClasses1DataContext())
//                {
//                    int SYS_ConfigId = Convert.ToInt32(SYS_ConfigRecord.OptionItem_ID);
//                    SYS_Config _SYS_Configs = contextObj.SYS_Config.Where(b => b.OptionItem_ID == SYS_ConfigId).FirstOrDefault();

//                    ////alot easier to make generic for each table? - just have type as variable...
//                    //MetaTable tableMeta = contextObj.Mapping.GetTable(typeof(SYS_Config));

//                    //var dataMembers = tableMeta.RowType.PersistentDataMembers; //information on all rows in SYS_Config

//                    //var listofSumin = new List<MetaDataMember>();
//                    //for(int i = 0; i < dataMembers.Count(); i++)
//                    //{
//                    //    var columnName = dataMembers[i].Name;
//                    //    _SYS_Configs.columnName = SYS_ConfigRecord.columnName;
//                    //}

//                    _SYS_Configs.IsEditable = SYS_ConfigRecord.IsEditable;
//                    _SYS_Configs.MenuItem_Code = SYS_ConfigRecord.MenuItem_Code;
//                    _SYS_Configs.OptionItem = SYS_ConfigRecord.OptionItem;
//                    _SYS_Configs.OptionItemDetail = SYS_ConfigRecord.OptionItemDetail;
//                    _SYS_Configs.OptionItemDetail_Value = SYS_ConfigRecord.OptionItemDetail_Value;

//                    try
//                    {
//                        contextObj.SubmitChanges();
//                    }
//                    catch (SqlException sqlex)
//                    {
//                        return sqlex.Message;
//                    }
//                    catch (Exception ex)
//                    {
//                        return ex.Message; //This will trap any other 'type' of exception incase a sqlex is not thrown.
//                    }

//                    //contextObj.SubmitChanges();
//                    return "SYS_Config record updated successfully";
//                }
//            }
//            else
//            {
//                return "Invalid SYS_Config record";
//            }
//        }

//        // Add SYS_Config
//        public string AddSYS_Config(SYS_Config SYS_ConfigRecord)
//        {
//            if (SYS_ConfigRecord != null)
//            {
//                //find dbinfo of each row?
//                using (DataClasses1DataContext contextObj = new DataClasses1DataContext())
//                {
//                    contextObj.SYS_Config.InsertOnSubmit(SYS_ConfigRecord); //is this the same as add?
//                    try
//                    {
//                        contextObj.SubmitChanges();
//                    }
//                    catch (SqlException sqlex)
//                    {
//                        return sqlex.Message;
//                    }
//                    catch (Exception ex)
//                    {
//                        return ex.Message; //This will trap any other 'type' of exception incase a sqlex is not thrown.
//                    }
//                    return "SYS_Config record added successfully";
//                }
//            }
//            else
//            {
//                return "Invalid SYS_Config record";
//            }
//        }

//        // Delete SYS_Config
//        public string DeleteSYS_Config(string SYS_ConfigId)
//        {
//            if (!String.IsNullOrEmpty(SYS_ConfigId))
//            {
//                try
//                {
//                    int _SYS_ConfigId = Int32.Parse(SYS_ConfigId);
//                    using (DataClasses1DataContext contextObj = new DataClasses1DataContext())
//                    {
//                        var _SYS_Config = contextObj.SYS_Config.FirstOrDefault(s => s.OptionItem_ID == _SYS_ConfigId); //only one?
//                        contextObj.SYS_Config.DeleteOnSubmit(_SYS_Config);//only one element?
//                                                                          //https://damieng.com/blog/2008/07/30/linq-to-sql-log-to-debug-window-file-memory-or-multiple-writers
//                                                                          //
//                                                                          //contextObj.Log = new ConfigTool.Log();
//                                                                          //f you wish to not overwrite the existing log file then change the constructor to include the parameter true after the filename. 
//                        contextObj.Log = new System.IO.StreamWriter("linqtosql.log", true) { AutoFlush = true };

//                        contextObj.SubmitChanges();

//                        return "Selected SYS_Config record deleted sucessfully";
//                    }
//                }
//                catch (Exception)
//                {
//                    return "SYS_Config details not found";
//                }
//            }
//            else
//            {
//                return "Invalid operation";
//            }
//        }

//        #endregion
//    }
//}