﻿using ConfigTool;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Data.SqlClient;
using System.Linq.Dynamic;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Contexts;
using System.Web;
using System.Web.Mvc;
using System.Text.RegularExpressions;

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

        public struct Result
        {
            public string headerArr;
            public string dataArr;
            public List<string> pKey;
            public List<Tuple<string, string>> fKeyTables; //headername, typename, and data for dropdowns
        }

        public JsonResult LoadTableContent(string table)
        {
            using (DataClasses1DataContext contextObj = new DataClasses1DataContext())
            {
                if (table != null)
                {
                    var name = table.Trim('/', '"');
                    var tableType = Type.GetType("ConfigTool." + name + ", ConfigTool, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null");
                    var tableData = contextObj.GetTable(tableType).AsQueryable();
                    JArray dataArr = JArray.Parse(JsonConvert.SerializeObject(tableData, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }));

                    var pKeyNames = contextObj.Mapping.GetTable(tableType).RowType.DataMembers.Where(m => m.IsPrimaryKey).ToArray().Select(p => p.MappedName).ToList();
                    //var fKeyNames = contextObj.Mapping.GetTable(tableType).RowType.DataMembers.Where(m => m.IsForeignKey).ToArray().Select(p => p.MappedName).ToList();
                    var associationTables = new List<Tuple<string, string>>(); //list of fKey tables and data for dropdown (foreign key col data), is the table a parent or a child?

                    //initialise values
                    var jsonResult = new Result();
                    JArray headArr = new JArray();
                    var datamodel = contextObj.Mapping;
                    var headers = GetHeaders(name, associationTables);

                    foreach (string cell in headers)
                    {
                        var headerName = cell.Split(':')[0].Trim(new char[] { '"', '{', '\r', '\n', '\"', '\"', ' ' });
                        if (!associationTables.Select(l => l.Item2).ToList().Contains(headerName))
                        {
                            string typeEditor;
                            try
                            {
                                var info = GetProps(tableType, headerName);
                                System.Data.Linq.Mapping.ColumnAttribute[] cellInfo = (System.Data.Linq.Mapping.ColumnAttribute[])info;
                                typeEditor = ParseDbType(cellInfo[0].DbType);
                            }
                            catch
                            {
                                //this meas if no data all cells are text boxes - doesnt matter right this second but needs changing
                                typeEditor = "text";
                            }

                            var obj = new JObject();
                            var nameProp = new JProperty("headerName", headerName);
                            var fieldProp = new JProperty("field", headerName);
                            var widthProp = new JProperty("width", 150);
                            var editProp = new JProperty("editable", true);
                            var editorProp = new JProperty("cellEditor", typeEditor);
                            var typeProp = new JProperty("type", typeEditor);
                            //var nullProp = new JProperty("canBeNull", cellInfo[0].CanBeNull);
                            //foreign key drop down data
                            if (headerName == "FK")
                            {
                                editorProp = new JProperty("cellEditor", "popupSelect");
                                obj.Add(new JProperty("cellEditorParams", GetFKeyData(tableData, headerName)));
                            }
                            //if primary key -- not editable and underlined?
                            if (pKeyNames.Contains(headerName))
                            {
                                typeProp = new JProperty("type", "pKey");
                                editProp = new JProperty("editable", false);
                            }
                            obj.Add(nameProp);
                            obj.Add(fieldProp);
                            obj.Add(widthProp);
                            obj.Add(editProp);
                            if (!pKeyNames.Contains(headerName))
                            {
                                obj.Add(editorProp);
                            }
                            //obj.Add(nullProp);
                            obj.Add(typeProp);
                            headArr.Add(obj);
                        }
                    }

                    jsonResult.headerArr = headArr.ToString();
                    jsonResult.dataArr = dataArr.ToString();
                    jsonResult.pKey = pKeyNames;
                    jsonResult.fKeyTables = associationTables;

                    return Json(jsonResult, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("Error: Getting table data from db.");
                }
            }
        }

        public object GetProps(Type type, string col)
        {
            PropertyInfo prop = type.GetProperty(col);
            return prop.GetCustomAttributes(typeof(System.Data.Linq.Mapping.ColumnAttribute), true);

        }
        public string ParseDbType(string dbType)
        {
            if (dbType.Contains("Int")) { return "NumericCellEditor"; }
            else if (dbType.Contains("VarChar")) {
                if (int.Parse(Regex.Match(dbType, @"\d+").Value) <= 100 ) { return "text"; }
                else return "largeText";
            }
            else if (dbType.Contains("Xml")) { return "XmlEditor"; }
            else if (dbType.Contains("DateTime")) { return "DateEditor"; }
            else if (dbType.Contains("Bit")) { return "CheckBoxEditor"; }
            else if (dbType.Contains("Unique")) { return "text"; }
            else return "New type: " + dbType;
        }
        public List<string> GetHeaders(string name, List<Tuple<string, string>> ascTables)
        {
            using (DataClasses1DataContext contextObj = new DataClasses1DataContext())
            {
                var tableList = contextObj.Mapping.GetTables();
                var tabList = new List<Tuple<string, string>>();

                List<string> headers = new List<string>();
                foreach (MetaTable tab in tableList)
                {
                    var t = tab.TableName.TrimStart("app.".ToCharArray());
                    var rt = tab.RowType.ToString();
                    tabList.Add(new Tuple<string, string>(t, rt));

                    if (rt.Equals(name, StringComparison.InvariantCultureIgnoreCase))
                    {
                        foreach (var r1 in tab.RowType.DataMembers)
                        {
                            headers.Add(r1.MappedName);
                        }
                    }
                }
                if (tabList.Select(l => l.Item1).ToList().Contains(name))
                {
                    var typeName = tabList.Where(n => n.Item1 == name).ToList()[0].Item2;

                    ascTables.Add(new Tuple<string, string>("app." + name, typeName));
                }
                return headers;
            }
        }


        public string GetFKeyData(IQueryable data, string fKey)
        {
            //return json of fk col data
            return "{ values: ['English', 'Spanish', 'French', 'Portuguese', '(other)'] }";
        }


        //submit all changes on click and generate script
        public void SubmitChanges()
        {

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
                var tableInfo = new List<Tuple<string, string>>();
                var tableList = contextObj.Mapping.GetTables();
                foreach (MetaTable table in tableList)
                {
                    tableInfo.Add(new Tuple<string, string>(table.TableName, table.RowType.ToString()));
                }
                return Json(tableInfo, JsonRequestBehavior.AllowGet);
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

