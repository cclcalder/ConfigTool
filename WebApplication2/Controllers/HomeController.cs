using ConfigTool;
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
            public List<string> descriptions;
        }

        public JsonResult LoadTableContent(string table)
        {
            using (DataClasses1DataContext contextObj = new DataClasses1DataContext())
            {
                if (table != null)
                {
                    var tableType = GetType(table);
                    var tableData = contextObj.GetTable(tableType).AsQueryable();

                    //get keys
                    var pKeyNames = contextObj.Mapping.GetTable(tableType).RowType.DataMembers.Where(m => m.IsPrimaryKey).ToArray().Select(p => p.MappedName).ToList();
                    //var fKeyNames = contextObj.Mapping.GetTable(tableType).RowType.DataMembers.Where(m => m.IsForeignKey).ToArray().Select(p => p.MappedName).ToList();

                    //get data
                    JArray dataArr = JArray.Parse(JsonConvert.SerializeObject(tableData, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }));

                    //initialise values
                    var jsonResult = new Result();
                    JArray headArr = new JArray();
                    string[] headers;
                    var associationTables = new List<Tuple<string, string>>(); //list of fKey tables and data for dropdown (foreign key col data), is the table a parent or a child?
                    var descriptions = new List<string>();


                    //check if can get header data (if there is data in table)
                    headers = GetHeaders(dataArr);

                    foreach (string cell in headers)
                    {
                        var tableList = contextObj.Mapping.GetTables();
                        var tabList = new List<Tuple<string, string>>();
                        var headerName = cell.Split(':')[0].Trim(new char[] { '"', '{', '\r', '\n', '\"', '\"', ' ' });


                        //get datacontext table names
                        foreach (MetaTable tab in tableList)
                        {
                            tabList.Add(new Tuple<string, string>(tab.TableName.TrimStart("app.".ToCharArray()), tab.RowType.ToString()));
                        }
                        //if header is ?not name of current table? && other table name, list as associated table
                        if (tabList.Select(l => l.Item1).ToList().Contains(headerName) /*&& headerName != tableToLoad*/)
                        {
                            var typeName = tabList.Where(n => n.Item1 == headerName).ToList()[0].Item2;

                            associationTables.Add(new Tuple<string, string>("app." + headerName, typeName));
                        }
                        //if description pass in as description not in table editor
                        else if (headerName == "Description")
                        {
                            descriptions.Add(headerName);
                        }
                        else
                        {
                            var info = GetProps(tableType, headerName);
                            System.Data.Linq.Mapping.ColumnAttribute[] cellInfo = (System.Data.Linq.Mapping.ColumnAttribute[])info;
                            string typeEditor = ParseDbType(cellInfo[0].DbType);

                            var obj = new JObject();
                            var prop = new JProperty("headerName", headerName);
                            var fieldprop = new JProperty("field", headerName);
                            var widthprop = new JProperty("width", 150);
                            var editprop = new JProperty("editable", true);
                            var styleprop = new JProperty("cellStyle", "{}");
                            var typeprop = new JProperty("cellEditor", typeEditor);
                            var nullprop = new JProperty("canBeNull", cellInfo[0].CanBeNull);
                            //if primary key -- not editable and underlined?
                            if (pKeyNames.Contains(headerName)) { editprop = new JProperty("editable", false); styleprop = new JProperty("cellStyle", "{text-decoration: underline;}"); }
                            obj.Add(prop);
                            obj.Add(fieldprop);
                            obj.Add(widthprop);
                            obj.Add(editprop);
                            obj.Add(styleprop);
                            obj.Add(nullprop);
                            //here create json of foreign key data col
                            if (headerName == "FK") { typeprop = new JProperty("cellEditor", "popupSelect"); obj.Add(new JProperty("cellEditorParams", GetFKeyData(tableData, headerName))); }
                            obj.Add(typeprop);
                            headArr.Add(obj);
                        }

                    }

                    jsonResult.headerArr = headArr.ToString();
                    jsonResult.dataArr = dataArr.ToString();
                    jsonResult.pKey = pKeyNames;
                    jsonResult.fKeyTables = associationTables;
                    jsonResult.descriptions = descriptions;

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
            else if (dbType.Contains("VarChar"))
            {
                if (int.Parse(Regex.Match(dbType, @"\d+").Value) > 100) { return "largeText"; }
                else return "";
            }
            else if (dbType.Contains("Xml")) { return "XmlEditor"; }
            else if (dbType.Contains("DateTime")) { return "DateEditor"; }
            else if (dbType.Contains("Bit")) { return "CheckBoxEditor"; }
            else return "New type: " + dbType;
        }

        public string[] GetHeaders(JArray data)
        {
            try
            {
                return data.Root[0].ToString().Split(',');
            }
            catch
            {
                //if nothing in dataArr, cannot get headers from there.. BUT WHERE FROM
                return new string[] { "Empty Table" };
            }
        }

        public string GetFKeyData(IQueryable data, string fKey)
        {
            //return json of fk col data
            return "{ values: ['English', 'Spanish', 'French', 'Portuguese', '(other)'] }";
        }

        public Type GetType(string table)
        {
            //get data from table name
            var tableToLoad = table.Trim('/', '"');
            var t = typeof(SYS_Lock).AssemblyQualifiedName; //example of whole random table name, should refactor to use this to generate type
            string tableName = "ConfigTool." + tableToLoad + ", ConfigTool, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
            return Type.GetType(tableName);
        }

        //for now CRUD not multi select (or copy/paste data etc)
        //why can't have generic methods in mapmvcattributeroutes http://stackoverflow.com/questions/35800049/mvc-app-doesnt-run-with-a-generic-method
        public string InsertRecord(string[] row)
        {
            if (row != null)
            {
                using (DataClasses1DataContext contextObj = new DataClasses1DataContext())
                {

                    return "Row inserted successfully";
                }
            }
            else
            {
                return "Invalid record";
            }
        }
        public string UpdateRecord(string record)
        {
            var toUpdate = record;
            if (record != null)
            {
                using (DataClasses1DataContext contextObj = new DataClasses1DataContext())
                {
                    //rplace these all with actuall pKey identified
                    var parseJson = toUpdate.Split(',')[0];
                    var pkeyName = parseJson.Split(':')[0].Trim(new char[] { '{', '"', ':', ',' });
                    var pkeyValue = parseJson.Split(':')[1].Trim(new char[] { '{', '"', ':', ',', '}' });

                    var _newRecord = contextObj.SYS_Configs.Where(b => b.OptionItem_ID.ToString() == pkeyValue).FirstOrDefault();

                    //parse json to each 
                    _newRecord.IsEditable = true;
                    _newRecord.MenuItem_Code = "true";
                    _newRecord.OptionItem = "true";
                    _newRecord.OptionItemDetail = "true";
                    _newRecord.OptionItemDetail_Value = "true";

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
        public string DeleteRecord(string record)
        {
            //method needs to know what table we're in!
            var toDelete = record;
            if (!String.IsNullOrEmpty(toDelete))
            {
                //THIS NEEDS TO COME FROM KNOWN PKEY
                //here just assuming its first value
                //also only working for single deleting
                var parseJson = toDelete.Split(',')[0];
                var pkeyName = parseJson.Split(':')[0].Trim(new char[] { '{', '"', ':', ',' });
                var pkeyValue = parseJson.Split(':')[1].Trim(new char[] { '{', '"', ':', ',', '}' });
                try
                {
                    using (DataClasses1DataContext contextObj = new DataClasses1DataContext())
                    {
                        //NOT YET WOKRING FOR ANYTHING OTHER THAN SYS_CONFIG, CLASSIC
                        var _Record = contextObj.SYS_Configs.FirstOrDefault(s => s.OptionItem_ID.ToString() == pkeyValue); //only one?
                        contextObj.SYS_Configs.DeleteOnSubmit(_Record);
                        //contextObj.Log = new System.IO.StreamWriter("linqtosql.log", true) { AutoFlush = true };
                        contextObj.SubmitChanges();
                        return "Selected SYS_Config record deleted sucessfully";
                    }
                }
                catch (Exception)
                {
                    return "Record details not found";
                }
            }
            else
            {
                return "Invalid operation";
            }
        }
        public string GenericGetByID(string id)
        {
            using (DataClasses1DataContext contextObj = new DataClasses1DataContext())
            {
                var recordId = Convert.ToInt32(id);
            }
            return "Don't think this is needed for new methods";
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

