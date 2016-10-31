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
using System.Web.Script.Serialization;
using Newtonsoft.Json.Serialization;

namespace WebApplication2.Controllers
{

    //does this mean you have to authorize every controller below?
    [Authorize]
    public class HomeController : Controller
    {
        #region Action Views
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
        public struct Result
        {
            public string headerArr;
            public string dataArr;
            public List<string> pKey;
            public List<AssociatedTable> fKeyTables; //headername, typename, and data for dropdowns
            public string emptyRow;
        }
        public struct AssociatedTable
        {
            public string name;                                 //name 
            public string typeName;                             //and typename of associated table
            public string relationshipStr;                      //string to print to screen
            public string currentKey;                           //key in current table
            public string foreignKey;                           //corresponding key in associated, if table is parent of current, store data for fKey dropdowns
            public string fKeyData;
            public string relationToCurrent;                    //relation to current - PARENT OR CHILD - if child - drop down input where headername = currentKey           
        }
        public struct Header
        {
            public string name;
            public string type;
        }

        //Get Result data
        public JsonResult LoadTableContent(string table)
        {
            using (DataClasses1DataContext contextObj = new DataClasses1DataContext())
            {
                if (table != null)
                {
                    var name = table.Trim('/', '"');
                    var tableType = Type.GetType("ConfigTool." + name + ", ConfigTool, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null");
                    var tableData = contextObj.GetTable(tableType).AsQueryable();

                    JArray dataArr = GetJsonData(tableData);

                    var associationTables = new List<AssociatedTable>();
                    var jsonResult = new Result();
                    JArray headArr = new JArray();
                    var pKeyNames = contextObj.Mapping.GetTable(tableType).RowType.DataMembers.Where(m => m.IsPrimaryKey).ToArray().Select(p => p.MappedName).ToList();
                    var headers = GetHeaders(name, associationTables);

                    foreach (Header header in headers)
                    {
                        //general properties
                        var obj = new JObject();
                        var nameProp = new JProperty("headerName", header.name);
                        var fieldProp = new JProperty("field", header.name);
                        var widthProp = new JProperty("width", 150);
                        obj.Add(nameProp);
                        obj.Add(fieldProp);
                        obj.Add(widthProp);

                        var parent = false;
                        //if there are associated tables
                        if (associationTables.Count() != 0)
                        {
                            //foreach connection
                            foreach (AssociatedTable aTable in associationTables)
                            {
                                if (aTable.relationToCurrent == "Parent" && header.name == aTable.currentKey)
                                {
                                    obj.Add(new JProperty("editable", true));
                                    obj.Add(new JProperty("type", "fKey"));
                                    obj.Add(new JProperty("cellEditor", "select"));
                                    obj.Add(new JProperty("editorParams", GetFKeyData(aTable)));
                                    parent = true;
                                }
                            }
                        }
                        //properties specific to type
                        if (pKeyNames.Contains(header.name) && !parent)
                        {
                            //if primary key: key renderer and non editable
                            obj.Add(new JProperty("editable", false));
                            obj.Add(new JProperty("type", "pKey"));
                        }

                        else if (!parent)
                        {
                            ParseDbType(header.type, obj);
                        }


                        headArr.Add(obj);
                    }
                    jsonResult.headerArr = headArr.ToString();
                    jsonResult.dataArr = dataArr.ToString();
                    jsonResult.pKey = pKeyNames;
                    jsonResult.fKeyTables = associationTables;
                    jsonResult.emptyRow = NewTempRow(headers).ToString();

                    return Json(jsonResult, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("Error: Getting table data from db.");
                }
            }
        }
        public void ParseDbType(string dbType, JObject obj)
        {
            //**should restrain char count and add not null 
            if (dbType.Contains("Int"))
            {
                //if numeric: editable, cell style align right and numeric editor
                obj.Add(new JProperty("editable", true));
                obj.Add(new JProperty("type", "numeric"));
            }
            else if (dbType.Contains("VarChar"))
            {
                if (dbType.Contains("MAX"))
                {
                    obj.Add(new JProperty("editable", true));
                    obj.Add(new JProperty("cellEditor", "largeText"));
                }
                else if (int.Parse(Regex.Match(dbType, @"\d+").Value) <= 100)
                {
                    obj.Add(new JProperty("editable", true));
                    obj.Add(new JProperty("cellEditor", "text"));
                }
                else
                {
                    obj.Add(new JProperty("editable", true));
                    obj.Add(new JProperty("cellEditor", "largeText"));
                }
            }
            else if (dbType.Contains("Xml"))
            {
                obj.Add(new JProperty("editable", true));
                obj.Add(new JProperty("cellEditor", "largeText"));
            }
            else if (dbType.Contains("DateTime"))
            {
                obj.Add(new JProperty("editable", false));
            }
            else if (dbType.Contains("Date") && !dbType.Contains("DateTime"))
            {
                obj.Add(new JProperty("editable", true));
                obj.Add(new JProperty("type", "date"));
            }
            else if (dbType.Contains("Bit"))
            {
                //if checkbox (boolean), editable, renderer and editor??
                obj.Add(new JProperty("editable", true));
                obj.Add(new JProperty("type", "bool"));
            }
            else if (dbType.Contains("Unique"))
            {
                obj.Add(new JProperty("editable", true));
                obj.Add(new JProperty("cellEditor", "text"));
            }
        }
        public List<Header> GetHeaders(string name, List<AssociatedTable> ascTables)
        {
            using (DataClasses1DataContext contextObj = new DataClasses1DataContext())
            {
                var tableList = contextObj.Mapping.GetTables();
                var tabList = new List<Tuple<string, string>>();
                var checkTabList = new List<Tuple<string, string>>();
                foreach (MetaTable tab in tableList)
                {
                    var t = tab.TableName.TrimStart("app.".ToCharArray());
                    var rt = tab.RowType.ToString();
                    checkTabList.Add(new Tuple<string, string>(t, rt));
                }

                List<Header> headers = new List<Header>();
                foreach (MetaTable tab in tableList)
                {
                    if (tab.RowType.ToString().Equals(name, StringComparison.InvariantCultureIgnoreCase))
                    {
                        foreach (var r1 in tab.RowType.DataMembers)
                        {
                            //if there exists an associated table, test: "Dim_Admin_MenuItem"
                            if (r1.Association != null)
                            {
                                var ascTable = new AssociatedTable();
                                ascTable.relationshipStr = r1.Association.ToString();

                                if (checkTabList.FirstOrDefault(n => n.Item1 == r1.Name || n.Item2 == r1.Name) == null)
                                {
                                    //try remove s? TERRIBLE STUPID HACK THERE SHOULD BE A NORMAL SANE WAY AROUND THIS.....
                                    ascTable.typeName = checkTabList.FirstOrDefault(n => n.Item1 == r1.Name.TrimEnd('s')).Item2;
                                }
                                else
                                {
                                    ascTable.typeName = checkTabList.FirstOrDefault(n => n.Item1 == r1.Name || n.Item2 == r1.Name).Item2;
                                }
                                ascTable.foreignKey = r1.Association.OtherKey[0].MappedName;
                                ascTable.currentKey = r1.Association.ThisKey[0].MappedName;
                                //ascTable.name = "app." + name;
                                if (r1.Association.IsForeignKey)
                                {
                                    //if has foreign key must be the child of the current table..
                                    ascTable.relationToCurrent = "Parent";
                                    ascTable.fKeyData = GetFKeyData(ascTable);
                                }
                                else
                                {
                                    //if does not have foreign key -> is parent 
                                    ascTable.relationToCurrent = "Child";
                                    ascTable.fKeyData = null;
                                }
                                ascTables.Add(ascTable);
                            }
                            else
                            {
                                var h = new Header();
                                var fk = new List<string>();
                                h.name = r1.MappedName;
                                h.type = r1.DbType.ToString();
                                //h.fKeys = fk.Add(r1.Association.IsForeignKey.ToString());
                                headers.Add(h);
                            }
                        }
                    }
                }

                return headers;
            }
        }
        public string GetFKeyData(AssociatedTable getDataTable)
        {
            using (DataClasses1DataContext contextObj = new DataClasses1DataContext())
            {
                JArray data = new JArray();
                var getCol = getDataTable.foreignKey; //this is header of colData we want 
                var tableType = Type.GetType("ConfigTool." + getDataTable.typeName + ", ConfigTool, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null");
                var item = contextObj.GetTable(tableType);
                JArray arr = GetJsonData(contextObj.GetTable(tableType));

                foreach (JObject element in arr)
                {
                    data.Add(element[getDataTable.foreignKey]);
                }
                string dataStr = "{ 'values' :" + data.ToString() + "}";

                //    dataStr = "{ 'values' : ['error','getting','fKey', 'data', 'prob out of mem'] }";

                JObject json = JObject.Parse(dataStr);
                return json.ToString();
            }
        }
        public JArray GetJsonData(IQueryable tableData)
        {
            var size = tableData.Count(); //if table size becomes issue do iteratively 
            JArray dataArr = new JArray();

            dataArr = JArray.Parse(JsonConvert.SerializeObject(tableData, new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    PreserveReferencesHandling = PreserveReferencesHandling.None,
                    ContractResolver = new CustomResolver()
            }));
            return dataArr;
        }
        public JObject NewTempRow(List<Header> headers)
        {
            var emptyRecord = new JObject();
            foreach (Header header in headers)
            {
                emptyRecord.Add(new JProperty(header.name, ""));
            }
            return emptyRecord;
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
class CustomResolver : DefaultContractResolver
{
    protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
    {
        JsonProperty prop = base.CreateProperty(member, memberSerialization);

        if (prop.DeclaringType != typeof(DataClasses1DataContext) &&
            prop.PropertyType.IsClass &&
            prop.PropertyType != typeof(string))
        {
            prop.ShouldSerialize = obj => false;
        }

        return prop;
    }
}

