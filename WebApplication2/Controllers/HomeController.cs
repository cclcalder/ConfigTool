using ConfigTool;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Linq.Mapping;
using System.Diagnostics;
using System.Linq.Dynamic;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Serialization;
using System.Linq.Expressions;


namespace WebApplication2.Controllers
{

    //does this mean you have to authorize every controller below?
    [Authorize]
    public class HomeController : Controller //this file needs to be split up TOO BIG
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

        #region agGrid

        public struct CurrentTable
        {
            public static Type type;
            public static string name;
        }
        public struct Result
        {
            public string headerArr;
            public int overLoad;
            public string[] dataStringArr;
            public List<string> pKey;
            public List<AssociatedTable> fKeyTables;            // headername, typename, and data for dropdowns
            public string emptyRow;
        }
        public struct AssociatedTable
        {
            public string name;                                 // name 
            public string typeName;                             // and typename of associated table
            public string relationshipStr;                      // string to print to screen
            public string currentKey;                           // key in current table
            public string foreignKey;                           // corresponding key in associated, if table is parent of current, store data for fKey dropdowns
            public string fKeyData;                                
            public string relationToCurrent;                    // relation to current - PARENT OR CHILD - if child - drop down input where headername = currentKey           
        }
        public struct Header
        {
            public string name;
            public string type;
        }

        // Get Result data
        public JsonResult LoadTableContent(string table)
        {
            using (DataClasses1DataContext contextObj = new DataClasses1DataContext())
            {
                if (table != null)
                {
                    var tableName = table.Trim('/', '"');
                    var tableType = Type.GetType("ConfigTool." + tableName + ", ConfigTool, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null");
                    Debug.Assert(tableType != null, "tableType != null");
                    var tableData = contextObj.GetTable(tableType).AsQueryable();
                    var overload = tableData.Count();
                    var dataArr = new JArray();
                    CurrentTable.type = tableType;
                    CurrentTable.name = tableName;

                    // If bigger than 100,000 rows dont bother getting the data
                    if (overload < 10000)
                    {
                        dataArr = GetJsonData(tableData);
                    }
                    else { }

                    var associationTables = new List<AssociatedTable>();
                    var jsonResult = new Result();
                    var headArr = new JArray();
                    var pKeyNames = contextObj.Mapping.GetTable(tableType).RowType.DataMembers.Where(m => m.IsPrimaryKey).ToArray().Select(p => p.MappedName).ToList();

                    var headers = GetHeaders(tableName, associationTables);
                    var hasChanges = new JObject
                    {
                        new JProperty("headerName", "Has Changes"),
                        new JProperty("field", "hasChanges"),
                        new JProperty("width", 150),
                        new JProperty("type", "changes"),
                        new JProperty("editable", false)
                    };
                    headArr.Add(hasChanges);

                    foreach (var header in headers)
                    {
                        // Common properties
                        var obj = new JObject();
                        var nameProp = new JProperty("headerName", header.name);
                        var fieldProp = new JProperty("field", header.name);
                        var widthProp = new JProperty("width", 150);
                        obj.Add(nameProp);
                        obj.Add(fieldProp);
                        obj.Add(widthProp);
                        if (header.type.Contains("NOT NULL"))
                        {
                            obj.Add(new JProperty("null", false));
                        }

                        var parent = false;
                        if (associationTables.Count() != 0)
                        {
                            foreach (var aTable in associationTables)
                            {
                                if (aTable.relationToCurrent == "Parent" && header.name == aTable.currentKey)
                                {
                                    obj.Add(new JProperty("editable", true));
                                    obj.Add(new JProperty("type", "fKey"));
                                    obj.Add(new JProperty("cellEditor", "select"));
                                    parent = true;
                                }
                            }
                        }

                        // Properties specific to type
                        if (pKeyNames.Contains(header.name) && !parent)
                        {
                            obj.Add(header.type.Contains("IDENTITY")
                                ? new JProperty("editable", false)
                                : new JProperty("editable", true));
                            obj.Add(new JProperty("type", "pKey"));
                        }

                        else if (!parent)
                        {
                            ParseDbType(header.type, obj);
                        }
                        headArr.Add(obj);
                    }

                    jsonResult.headerArr = headArr.ToString();
                    jsonResult.pKey = pKeyNames;
                    jsonResult.fKeyTables = associationTables;
                    jsonResult.emptyRow = NewTempRow(headers).ToString();
                    jsonResult.overLoad = 0;

                    if (overload > 10000)
                    {
                        jsonResult.overLoad = overload;
                        //return Json(jsonResult, JsonRequestBehavior.AllowGet);
                    }

                    var array = dataArr.Select(data => data.ToString()).ToArray();
                    jsonResult.dataStringArr = array;

                    return Json(jsonResult, JsonRequestBehavior.AllowGet);
                }
                return Json("Error: Getting table data from db.");
            }
        }
        public void ParseDbType(string dbType, JObject obj)
        {
            var editable = !dbType.Contains("IDENTITY");
            //**should restrain char count and add not null 
            if (dbType.Contains("Int"))
            {
                //if numeric: editable, cell style align right and numeric editor
                obj.Add(new JProperty("type", "numeric"));
            }
            else if (dbType.Contains("VarChar"))
            {
                if (dbType.Contains("MAX"))
                {
                    obj.Add(new JProperty("cellEditor", "largeText"));
                }
                else
                {
                    var limit = int.Parse(Regex.Match(dbType, @"\d+").Value);
                    if (limit >= 100)
                    {
                        obj.Add(new JProperty("cellEditor", "largeText"));
                        obj.Add(new JProperty("charLimit", limit));
                    }

                    else
                    {
                        obj.Add(new JProperty("cellEditor", "text"));
                        obj.Add(new JProperty("charLimit", limit));
                    }
                }

            }
            else if (dbType.Contains("Xml"))
            {
                obj.Add(new JProperty("cellEditor", "largeText"));
            }
            else if (dbType.Contains("DateTime"))
            {
                editable = false;
            }
            else if (dbType.Contains("Date") && !dbType.Contains("DateTime"))
            {
                obj.Add(new JProperty("type", "date"));
            }
            else if (dbType.Contains("Bit"))
            {
                obj.Add(new JProperty("type", "bool"));
            }
            else if (dbType.Contains("Unique"))
            {
                obj.Add(new JProperty("cellEditor", "text"));
            }
            obj.Add(new JProperty("editable", editable));
        }
        public static List<Header> GetHeaders(string tableName, List<AssociatedTable> ascTables)
        {
            using (var contextObj = new DataClasses1DataContext())
            {
                var tableList = contextObj.Mapping.GetTables();
                var metaTables = tableList as IList<MetaTable> ?? tableList.ToList();
                var checkTabList = metaTables.Select(tab => tab.RowType.ToString()).ToList();

                var headers = new List<Header>();
                foreach (var tab in metaTables)
                {
                    if (!tab.RowType.ToString().Equals(tableName, StringComparison.InvariantCultureIgnoreCase))
                        continue;
                    foreach (var r1 in tab.RowType.DataMembers)
                    {
                        //if there exists an associated table, test: "Dim_Admin_MenuItem"
                        if (r1.Association != null)
                        {
                            var ascTable = new AssociatedTable();
                            ascTable.relationshipStr = r1.Association.ToString();

                            if (checkTabList.FirstOrDefault(n => n == r1.Name) == null)
                            {
                                //Name = "Dim_Event_Status1"
                                //try add s? TERRIBLE STUPID HACK THERE SHOULD BE A NORMAL SANE WAY AROUND THIS.....
                                ascTable.typeName = checkTabList.FirstOrDefault(n => n + '1' == r1.Name);
                            }
                            else
                            {
                                ascTable.typeName = checkTabList.FirstOrDefault(n => n == r1.Name);
                            }
                            ascTable.foreignKey = r1.Association.OtherKey[0].MappedName;
                            ascTable.currentKey = r1.Association.ThisKey[0].MappedName;
                            //ascTable.name = "app." + name;
                            if (r1.Association.IsForeignKey)
                            {
                                ascTable.relationToCurrent = "Parent";
                                //does this need to be called here?
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
                            h.name = r1.MappedName;
                            if (r1.DbType != null) h.type = r1.DbType;
                            //h.fKeys = fk.Add(r1.Association.IsForeignKey.ToString());
                            headers.Add(h);
                        }
                    }
                }

                return headers;
            }
        }
        public static string GetFKeyData(AssociatedTable getDataTable)
        {
            using (DataClasses1DataContext contextObj = new DataClasses1DataContext())
            {
                var data = new JArray();
                var tableType = Type.GetType("ConfigTool." + getDataTable.typeName + ", ConfigTool, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null");
                contextObj.GetTable(tableType);
                var arr = GetJsonData(contextObj.GetTable(tableType));

                foreach (var element in arr)
                {
                    data.Add(element[getDataTable.foreignKey]);
                }
                var dataStr = "{ 'values' :" + data + "}";

                var json = JObject.Parse(dataStr);
                return json.ToString();
            }
        }
        public static JArray GetJsonData(IQueryable tableData)
        {
            var dataArr = JArray.Parse(JsonConvert.SerializeObject(tableData, new JsonSerializerSettings()
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
        #endregion

        //new method of saving to datacontext and writing script
        public string SaveTable(string[] changes)//RENAME TO EXECUTE IF WORKING
        {

                var table = new Tuple<string, Type>(CurrentTable.name, CurrentTable.type);
                Save.Main(changes, table);
                WriteScript();
            return "true";
        }
        public void WriteScript()
        {
            //in here take changes pushed to database and write merge script
        }

        //public virtual bool GenericDelete<T>(T item)
        //{
        //    using (DataClasses1DataContext db = new DataClasses1DataContext())
        //    {
        //        db.GetTable<T>().DeleteOnSubmit(item);
        //        db.SubmitChanges();
        //        return true;
        //    }
        //}


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

public interface IEntity<T> where T : class
{
    IList<T> List();
    IList<T> List(int? page, int? pageSize, Expression<Func<T, bool>> predicate, Expression<Func<T, object>> sort);
    void Add(T item);
    T Get(Int64 Id);
    void Update(T item);
    bool Delete(T item);
}





