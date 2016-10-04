﻿using ConfigTool;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Linq.Mapping;
using System.Data.SqlClient;
using System.Linq.Dynamic;
using System.Linq;
using System.Web.Mvc;


namespace WebApplication2.Controllers
{
    #region Action Views
    //does this mean you have to authorize every controller below?
    [Authorize]
    public class HomeController : Controller
    {
        // GET: SYS_Config
        public ActionResult Index()
        {
            return PartialView();
        }

        public ActionResult ModeSetup(string process)
        {
            ViewBag.Process = process;
            return PartialView();
        }

        //All tables
        public ActionResult Table(string tablename)
        {
            ViewBag.TableName = tablename;
            return PartialView();
        }

        //public ActionResult Table(string table)
        //{
        //    ViewBag.TableName = table;
        //    return PartialView();
        //}

        public ActionResult HomeSetup()
        {
            return PartialView();
        }
        public ActionResult ngView()
        {
            return View();
        }
        #endregion

        #region DataContext Info
        /* --------------------------------------- */
        //GET USEFUL INFO FROM DATACONTEXT - MAYBE DON'T NEED THIS

        //Create object containing all info html needs from db - via controller and javascript
        public struct ColDataToHtml
        {
            public List<string> names;                              // name of col, mainly for testing
            public Tuple<List<string>, List<bool>> inputTypes;      // input type for col - for html edit/add
            public List<string> children;                           // key contraints
            public List<string> parents;                            // key constraints
            public string namesJson;
        }

        //GET: Input type from column data type
        static List<string> ColumnMapping<T>(T item, string info) where T : new()
        {
            // Get type name
            var type = item.GetType();

            // Get the PropertyInfo object:
            var properties = item.GetType().GetProperties();
            var listMaps = new List<string>();
            Console.WriteLine("Finding properties for {0} ...", type.Name);
            foreach (var property in properties)
            {
                var attributes = property.GetCustomAttributes(false);
                //string msg = "the {0} property maps to the {1} database column";
                var columnMapping = attributes
                    .FirstOrDefault(a => a.GetType() == typeof(System.Data.Linq.Mapping.ColumnAttribute));

                if (columnMapping != null)
                {
                    var mapsto = columnMapping as System.Data.Linq.Mapping.ColumnAttribute;
                    if (info == "type")
                    {
                        //listMaps.Add(Convert.ToString(mapsto.DbType));

                        if (mapsto.DbType.Contains("VarChar") || mapsto.DbType.Contains("NVarChar"))
                        {
                            listMaps.Add("text");
                        }
                        else if (mapsto.DbType.Contains("Int"))
                        {
                            listMaps.Add("number");
                        }
                        else if (mapsto.DbType.Contains("Bit"))
                        {
                            listMaps.Add("checkbox");
                        }

                    }
                    else if (info == "name")
                    {
                        listMaps.Add(Convert.ToString(mapsto.Storage));
                    }
                    else if (info == "null")
                    {
                        if (!mapsto.DbType.Contains("NOT NULL"))
                        {
                            listMaps.Add("true");
                        }
                        else
                        {
                            listMaps.Add("false");
                        }
                    }
                    else if (info == "relations")
                    {
                        listMaps.Add("dunnoyet");
                    }
                }

            }
            return listMaps;
        }

        #endregion

        #region agGrid
        /* --------------------------------------- */
        //NEW AGGRID GENERIC TABLE EDITOR STUFF

        public JsonResult LoadTableContent(string table)
        {
            using (DataClasses1DataContext contextObj = new DataClasses1DataContext())
            {
                //got table
                if (table != null)
                {
                    //correct form
                    var tableToLoad = table.Trim('/', '"');
                    var t = typeof(SYS_Lock).AssemblyQualifiedName; //<-- example of whole random table name, should refactor to use this to generate type
                    string tableName = "ConfigTool." + tableToLoad + ", ConfigTool, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
                    Type tableType = Type.GetType(tableName);
                    var tableData = contextObj.GetTable(tableType).AsQueryable();
                    var pKeyName = contextObj.Mapping.GetTable(tableType).RowType.DataMembers.SingleOrDefault(m => m.IsPrimaryKey).MappedName;
                    //got selected table info, pass to web

                    try
                    { 
                        //pass table in json data form - parse js side?
                        JArray dataArr = JArray.Parse(JsonConvert.SerializeObject(tableData));
                        
                        //another method to pass data to front end
                        var headers = dataArr.Root[0].ToString().Split(',');
                        var headerList = new List<string>();
                        JArray headArr = new JArray();
                        foreach (string cell in headers)
                        {
                            var headerName = cell.Split(':')[0].Trim(new Char[] { '"', '{', '\r', '\n', '\"', '\"', ' ' });
                            var obj = new JObject();
                            var prop = new JProperty("headerName", headerName);
                            var fieldprop = new JProperty("field", headerName);
                            obj.Add(prop);
                            obj.Add(fieldprop);
                            headArr.Add(obj);
                            headerList.Add(headerName);
                        }
                        string bla = headArr.ToString().TrimStart('{').TrimEnd('}');

                        //uncomment different methods for testing
                        var jsonData = new Tuple<string, JArray>(bla, dataArr);
                        //var jsonData = new Tuple<List<string>, JArray>(headerList, dataArr);
                        var json = Json(jsonData, JsonRequestBehavior.AllowGet);
                        return json;
                    }
                    catch
                    {
                        return Json("Error getting table data from db.");
                    }
                }
                else
                {
                    return Json("Error getting table data from db.");
                }
            }
        }

        public JsonResult LoadTableData(string table)
        {
            //string tableName
            var tableToLoad = table.Trim('/', '"');

            using (DataClasses1DataContext contextObj = new DataClasses1DataContext())
            {
                //this shouldn't exist - if no match show dialog alert and don't reload page
                if (String.IsNullOrEmpty(table))
                {
                    //load SYS_Config example
                    var SYS_ConfigList = contextObj.SYS_Configs.ToList();
                    //perhaps redundant v
                    var data = new ColDataToHtml();
                    data.names = ColumnMapping(SYS_ConfigList[0], "name");
                    data.inputTypes = new Tuple<List<string>, List<bool>>((ColumnMapping(SYS_ConfigList[0], "type")), ColumnMapping(SYS_ConfigList[0], "null").Select(b => Convert.ToBoolean(b)).ToList());
                    data.parents = ColumnMapping(SYS_ConfigList[0], "relations");
                    data.children = ColumnMapping(SYS_ConfigList[0], "relations");
                    //json parsing
                    JArray jArr = new JArray();
                    foreach (string name in data.names)
                    {
                        var obj = new JObject();
                        var prop = new JProperty("headerName", name.Trim('_'));
                        var fieldprop = new JProperty("field", name.Trim('_'));
                        obj.Add(prop);
                        obj.Add(fieldprop);
                        jArr.Add(obj);
                    }
                    data.namesJson = jArr.ToString();
                    var jsonReturn = new Tuple<List<SYS_Config>, ColDataToHtml>(SYS_ConfigList, data);
                    return Json(jsonReturn, JsonRequestBehavior.AllowGet);
                }
                /* THIS IS FIRST ISSUE - dynamically get tables from tableNames */
                /* SECOND ISSUE = TROUBLE GETTING DATA INTO AGGRID */
                else
                {
                    try
                    {
                        //non generic method, using SYS_Locks at the moment
                        var test1 = contextObj.SYS_Locks; //this is Table<SYS_Lock>
                        var returnTable = contextObj.SYS_Locks.ToList();
                        var data = new ColDataToHtml();
                        data.names = ColumnMapping(returnTable[0], "name");
                        data.inputTypes = new Tuple<List<string>, List<bool>>((ColumnMapping(returnTable[0], "type")), ColumnMapping(returnTable[0], "null").Select(b => Convert.ToBoolean(b)).ToList());
                        data.parents = ColumnMapping(returnTable[0], "relations");
                        data.children = ColumnMapping(returnTable[0], "relations");
                        JArray jArr = new JArray();
                        foreach (string name in data.names)
                        {
                            var obj = new JObject();
                            var prop = new JProperty("headerName", name.Trim('_'));
                            var fieldprop = new JProperty("field", name.Trim('_'));
                            obj.Add(prop);
                            obj.Add(fieldprop);
                            jArr.Add(obj);
                        }
                        data.namesJson = jArr.ToString();
                        var jsonReturn = new Tuple<List<SYS_Lock>, ColDataToHtml>(returnTable, data);
                        return Json(jsonReturn, JsonRequestBehavior.AllowGet);
                    }
                    catch
                    {
                        return Json("Error getting table data from db.");
                    }
                }
            }
        }

        #endregion

        #region Get Info on Tables in DB
        /* --------------------------------------- */
        //GET TABLES AND INFO FOR SIDE NAV AND SETUP

        public JsonResult CompareSourceAndTarget()
        {
            //
            var r = new JsonResult();
            return r;
        }

        public JsonResult GetAllTableNames()
        {
            using (DataClasses1DataContext contextObj = new DataClasses1DataContext())
            {
                var tableList = contextObj.Mapping.GetTables();
                var tabNames = new List<string>();
                foreach (MetaTable table in tableList)
                {
                    tabNames.Add(table.TableName.TrimStart("app.".ToCharArray()));
                }
                return Json(tabNames, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region Old Table Stuff
        /* --------------------------------------- */
        //OLD TABLE FORMAT STUFF

        // GET: All SYS_Configs
        public JsonResult GetAllSYS_Configs()
        {
            using (DataClasses1DataContext contextObj = new DataClasses1DataContext())
            {

                var SYS_ConfigList = contextObj.SYS_Configs.ToList();

                var data = new ColDataToHtml();
                data.names = ColumnMapping(SYS_ConfigList[0], "name");
                data.inputTypes = new Tuple<List<string>, List<bool>>((ColumnMapping(SYS_ConfigList[0], "type")), ColumnMapping(SYS_ConfigList[0], "null").Select(b => Convert.ToBoolean(b)).ToList());
                data.parents = ColumnMapping(SYS_ConfigList[0], "relations");
                data.children = ColumnMapping(SYS_ConfigList[0], "relations");


                var jsonReturn = new Tuple<List<SYS_Config>, ColDataToHtml>(SYS_ConfigList, data);

                return Json(jsonReturn, JsonRequestBehavior.AllowGet);
            }
        }

        //GET: SYS_Config by Id
        public JsonResult GetSYS_ConfigById(string id)
        {
            using (DataClasses1DataContext contextObj = new DataClasses1DataContext())
            {
                var SYS_ConfigId = Convert.ToInt32(id);
                var getSYS_ConfigById = contextObj.SYS_Configs.FirstOrDefault(s => s.OptionItem_ID == SYS_ConfigId);
                return Json(getSYS_ConfigById, JsonRequestBehavior.AllowGet);
            }
        }

        //Update SYS_Config
        public string UpdateSYS_Config(SYS_Config SYS_ConfigRecord)
        {
            if (SYS_ConfigRecord != null)
            {
                using (DataClasses1DataContext contextObj = new DataClasses1DataContext())
                {
                    int SYS_ConfigId = Convert.ToInt32(SYS_ConfigRecord.OptionItem_ID);
                    SYS_Config _SYS_Configs = contextObj.SYS_Configs.Where(b => b.OptionItem_ID == SYS_ConfigId).FirstOrDefault();

                    ////alot easier to make generic for each table? - just have type as variable...
                    //MetaTable tableMeta = contextObj.Mapping.GetTable(typeof(SYS_Config));

                    //var dataMembers = tableMeta.RowType.PersistentDataMembers; //information on all rows in SYS_Config

                    //var listofSumin = new List<MetaDataMember>();
                    //for(int i = 0; i < dataMembers.Count(); i++)
                    //{
                    //    var columnName = dataMembers[i].Name;
                    //    _SYS_Configs.columnName = SYS_ConfigRecord.columnName;
                    //}

                    _SYS_Configs.IsEditable = SYS_ConfigRecord.IsEditable;
                    _SYS_Configs.MenuItem_Code = SYS_ConfigRecord.MenuItem_Code;
                    _SYS_Configs.OptionItem = SYS_ConfigRecord.OptionItem;
                    _SYS_Configs.OptionItemDetail = SYS_ConfigRecord.OptionItemDetail;
                    _SYS_Configs.OptionItemDetail_Value = SYS_ConfigRecord.OptionItemDetail_Value;

                    try
                    {
                        contextObj.SubmitChanges();
                    }
                    catch (SqlException sqlex)
                    {
                        return sqlex.Message;
                    }
                    catch (Exception ex)
                    {
                        return ex.Message; //This will trap any other 'type' of exception incase a sqlex is not thrown.
                    }

                    //contextObj.SubmitChanges();
                    return "SYS_Config record updated successfully";
                }
            }
            else
            {
                return "Invalid SYS_Config record";
            }
        }

        // Add SYS_Config
        public string AddSYS_Config(SYS_Config SYS_ConfigRecord)
        {
            if (SYS_ConfigRecord != null)
            {
                //find dbinfo of each row?
                using (DataClasses1DataContext contextObj = new DataClasses1DataContext())
                {
                    contextObj.SYS_Configs.InsertOnSubmit(SYS_ConfigRecord); //is this the same as add?
                    try
                    {
                        contextObj.SubmitChanges();
                    }
                    catch (SqlException sqlex)
                    {
                        return sqlex.Message;
                    }
                    catch (Exception ex)
                    {
                        return ex.Message; //This will trap any other 'type' of exception incase a sqlex is not thrown.
                    }
                    return "SYS_Config record added successfully";
                }
            }
            else
            {
                return "Invalid SYS_Config record";
            }
        }

        // Delete SYS_Config
        public string DeleteSYS_Config(string SYS_ConfigId)
        {
            if (!String.IsNullOrEmpty(SYS_ConfigId))
            {
                try
                {
                    int _SYS_ConfigId = Int32.Parse(SYS_ConfigId);
                    using (DataClasses1DataContext contextObj = new DataClasses1DataContext())
                    {
                        var _SYS_Config = contextObj.SYS_Configs.FirstOrDefault(s => s.OptionItem_ID == _SYS_ConfigId); //only one?
                        contextObj.SYS_Configs.DeleteOnSubmit(_SYS_Config);//only one element?
                                                                           //https://damieng.com/blog/2008/07/30/linq-to-sql-log-to-debug-window-file-memory-or-multiple-writers
                                                                           //
                                                                           //contextObj.Log = new ConfigTool.Log();
                                                                           //f you wish to not overwrite the existing log file then change the constructor to include the parameter true after the filename. 
                        contextObj.Log = new System.IO.StreamWriter("linqtosql.log", true) { AutoFlush = true };

                        contextObj.SubmitChanges();

                        return "Selected SYS_Config record deleted sucessfully";
                    }
                }
                catch (Exception)
                {
                    return "SYS_Config details not found";
                }
            }
            else
            {
                return "Invalid operation";
            }
        }

        #endregion
    }

}