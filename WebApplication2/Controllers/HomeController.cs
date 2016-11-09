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
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Contexts;
using System.Web;
using System.Web.Mvc;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;
using Newtonsoft.Json.Serialization;
using System.Collections;
using System.Linq.Expressions;


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

        public struct currentTable
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
                    var tableName = table.Trim('/', '"');
                    var tableType = Type.GetType("ConfigTool." + tableName + ", ConfigTool, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null");
                    var tableData = contextObj.GetTable(tableType).AsQueryable();
                    var overload = tableData.Count();
                    var dataArr = new JArray();
                    currentTable.type = tableType;
                    currentTable.name = tableName;
                    //if bigger than 100,000 rows dont bother getting the data
                    if (overload < 10000)
                    {
                        dataArr = GetJsonData(tableData);
                    }
                    else { }

                    var associationTables = new List<AssociatedTable>();
                    var jsonResult = new Result();
                    JArray headArr = new JArray();
                    var pKeyNames = contextObj.Mapping.GetTable(tableType).RowType.DataMembers.Where(m => m.IsPrimaryKey).ToArray().Select(p => p.MappedName).ToList();

                    var headers = GetHeaders(tableName, associationTables);
                    var hasChanges = new JObject();
                    hasChanges.Add(new JProperty("headerName", "Has Changes"));
                    hasChanges.Add(new JProperty("field", "hasChanges"));
                    hasChanges.Add(new JProperty("width", 150));
                    hasChanges.Add(new JProperty("type", "changes"));
                    hasChanges.Add(new JProperty("editable", false));
                    headArr.Add(hasChanges);

                    foreach (Header header in headers)
                    {
                        //common properties
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
                            //foreach connection
                            foreach (AssociatedTable aTable in associationTables)
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
                        //properties specific to type
                        if (pKeyNames.Contains(header.name) && !parent)
                        {
                            if (header.type.Contains("IDENTITY"))
                            {
                                obj.Add(new JProperty("editable", false));
                            }
                            else { obj.Add(new JProperty("editable", true)); }
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

                    var dataStrArr = new List<string>();
                    foreach (var data in dataArr)
                    {
                        dataStrArr.Add(data.ToString());
                    }
                    var array = dataStrArr.ToArray();
                    jsonResult.dataStringArr = array;

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
            bool editable = true;
            //**should restrain char count and add not null 
            if (dbType.Contains("Int"))
            {
                //if numeric: editable, cell style align right and numeric editor
                obj.Add(new JProperty("type", "numeric"));
            }
            else if (dbType.Contains("VarChar"))
            {
                var limit = int.Parse(Regex.Match(dbType, @"\d+").Value);
                if (limit >= 100)
                {
                    obj.Add(new JProperty("cellEditor", "largeText"));
                    obj.Add(new JProperty("charLimit", limit));
                }
                else if (dbType.Contains("MAX"))
                {
                    obj.Add(new JProperty("cellEditor", "largeText"));
                }
                else
                {
                    obj.Add(new JProperty("cellEditor", "text"));
                    obj.Add(new JProperty("charLimit", limit));
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
        public List<Header> GetHeaders(string tableName, List<AssociatedTable> ascTables)
        {
            using (DataClasses1DataContext contextObj = new DataClasses1DataContext())
            {
                var tableList = contextObj.Mapping.GetTables();
                var checkTabList = new List<string>();
                foreach (MetaTable tab in tableList)
                {
                    var rt = tab.RowType.ToString();
                    checkTabList.Add(rt);
                }

                List<Header> headers = new List<Header>();
                foreach (MetaTable tab in tableList)
                {
                    if (tab.RowType.ToString().Equals(tableName, StringComparison.InvariantCultureIgnoreCase))
                    {
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
        #endregion

        //new method of saving to datacontext and writing script
        public bool SaveTable(string[] changes)//RENAME TO EXECUTE IF WORKING
        {
            using (DataClasses1DataContext contextObj = new DataClasses1DataContext())
            {
                if (changes != null)
                {
                    var name = currentTable.name;
                    var stringCols = String.Join(", ", GetHeaders(name, new List<AssociatedTable>()).Select(s => s.name).ToArray());
                    string updateForMerge;
                    string insertForMerge;
                    foreach (string change in changes)//iterate thorugh changes 
                    {

                        var jsonChange = JsonConvert.DeserializeObject<JObject>(change.Replace("\\", ""));
                        var action = (string)jsonChange["hasChanges"];
                        if(action == "2")
                        {
                            //get pKey, if identity exists get that otherwise get pKey single or cluster
                            var pKeyID = new Tuple<List<string>, string>(contextObj.Mapping.GetTable(currentTable.type).RowType.DataMembers.Where(m => m.IsPrimaryKey && m.DbType.Contains("IDENTITY")).ToArray().Select(p => p.MappedName).ToList(), "ID");
                            if (pKeyID.Item1.Count() == 0)
                            {
                                pKeyID = new Tuple<List<string>, string>(contextObj.Mapping.GetTable(currentTable.type).RowType.DataMembers.Where(m => m.IsPrimaryKey).ToArray().Select(p => p.MappedName).ToList(), "SINGLE");
                                if (pKeyID.Item1.Count() > 1)
                                {
                                    pKeyID = new Tuple<List<string>, string>(contextObj.Mapping.GetTable(currentTable.type).RowType.DataMembers.Where(m => m.IsPrimaryKey).ToArray().Select(p => p.MappedName).ToList(), "MULTI");
                                }
                            }
                            var id = (string)jsonChange[pKeyID.Item1[0]];
                            var colName = pKeyID.Item1[0].ToString();
                            if (pKeyID.Item2 == "MULTI")
                            {
                                foreach (var item in pKeyID.Item1)
                                {

                                }
                                id = ;
                                colName = ;
                            }
                            // -- DELETE                     
                            //build MERGE STATEMENT ??
                            //this shouldnt be done in here?, should be done outside each change....
                            string deleteQuery = "DELETE FROM app." + name + " WHERE " + colName + " = '" + id + "'";
                            try { var record = contextObj.ExecuteCommand(deleteQuery); }
                            catch { }
                        }
                        else
                        {
                            // -- INSERT
                            string stringValues = " ";//remember to have '' around strings
                            foreach (var head in GetHeaders(name, new List<AssociatedTable>()).Select(s => s.name).ToList())
                            {
                                stringValues += "'" + (string)jsonChange["head"] + "',";
                            }
                            stringValues = stringValues.TrimEnd(',');
                            string insertQuery = "INSERT INTO app." + name + " ( " + stringCols + " ) SELECT " + stringValues;
                            updateForMerge = " ( " + stringCols + " ) VALUES " + stringValues;

                            // -- UPDATE
                            //fing changes 
                            string updateQuery = "UPDATE app." + name + " SET " + colName + " = '" + id + "'";
                            updateForMerge = " SET " + colName + " = '" + id + "'";
                        }


                    }
                    var matchKeys = "TGT.OptionItemDetail = SRC.OptionItemDetail AND TGT.OptionItem = SRC.OptionItem";//JUST AN EXAMPLE
                    //notes:
                    //not all tables are app. - this is easy fix
                    string merge = @"MERGE INTO app."+name+" TARGET "+ 
                                    "USING ( SELECT "+stringCols+ " FROM app." + name + " ) SOURCE "+
                                    "ON "+ matchKeys +
                                    "WHEN MATCHED AND " + matchKeys + //NOT RIGHT
                                    "THEN UPDATE"+ updateForMerge +
                                    "WHEN  NOT MATCHED BY TARGET THEN INSERT "+insertForMerge+
                                    "WHEN NOT MATCHED BY SOURCE THEN DELETE";

                    //TRY TO EXCUTE TO DATACONTEXT


                    //IF SUCCESS WRITE QUERY TO FILE
                }
            }
            return true;
        }


        //submit all changes on click and generate script
        public bool S(string[] changes)
        {
            using (DataClasses1DataContext contextObj = new DataClasses1DataContext())
            {
                //static struct storing current table type (and whatever else might be needed to do database stuff)
                if (changes != null)
                {
                    var tablePKey = contextObj.Mapping.GetTable(currentTable.type).RowType.DataMembers.Where(m => m.IsPrimaryKey).ToArray().Select(p => p.MappedName).ToList();
                    List<string> idToDelete = new List<string>();
                    var toUpsert = new JArray();

                    var jsonChanges = new JArray();
                    foreach (string change in changes)//iterate thorugh changes 
                    {
                        var jsonChange = JsonConvert.DeserializeObject<JObject>(change.Replace("\\", ""));
                        var action = (string)jsonChange["hasChanges"];
                        if (action == "2") //delete
                        {
                            string id = "";
                            foreach (var item in tablePKey) //primary key can be 'clustered' (multiple fields)
                            {
                                id = id + (string)jsonChange[item] + ",";
                                idToDelete.Add(id);//csv of primary key combo to delete
                            }
                        }
                        else if (action == "1") //edit or insert
                        {
                            toUpsert.Add(jsonChange);//pass whole record in json form to add or edit
                        }
                    }
                    if (DeleteRow(idToDelete) && Upsert(toUpsert))
                    {
                        //if allowed by data base, write script
                        WriteScript();
                        return true;
                    }
                    else { return false; }
                }
                else
                {
                    return false;
                }
            }
        }

        public bool DeleteRow(List<string> idsToDelete)
        //Query the database for the row to be deleted.
        //Call the DeleteOnSubmit method.
        //Submit the change to the database.
        {
            using (DataClasses1DataContext contextObj = new DataClasses1DataContext())
            {
                Type type = currentTable.type;
                //pkey of current table (however many) - correspond to list of ids set to 
                var pKey = contextObj.Mapping.GetTable(type).RowType.DataMembers.Where(m => m.IsPrimaryKey).ToArray().Select(p => p.MappedName).ToList();
                //get records
                foreach (var id in idsToDelete)
                {
                    //var record = null;
                    var colType = Type.GetType(pKey[0]);
                    if (pKey.Count() == 1)//if single primary key
                    {
                        //T instance = (T)contextObj.GetTable<T>().Where<T>(c => c == id).Single();
                        var keyCol = pKey[0];

                        //this can get element from selected record BUT I WANT THE WHOLE THING
                        var record = contextObj.GetTable(type).
                                        Where(keyCol + "=" + id).
                                        Select("");

                        //var test = contextObj.GetTable(type).SingleOrDefault(GetIDWhereExpression(id));
                        //var test1 = ( select X from contextObj.GetTable(type).Cast<object>() WHERE ;
                        //var toQuery = from col in contextObj.GetTable(type) where col.Field<int>(pKey[0]) == id select col;

                        contextObj.GetTable(type).DeleteOnSubmit(record);
                    }
                    else //otherwise split and check for key combo
                    {
                        foreach (var part in pKey)
                        {
                            //record = contextObj.GetTable(type).FirstOrDefault(s => s.OptionItem_ID == id && ....);
                        }
                    }
                    //contextObj.GetTable(type).DeleteOnSubmit(record);
                    //var record = contextObj.SYS_Config.FirstOrDefault(s => s.OptionItem_ID == _SYS_ConfigId);
                    //contextObj.SYS_Config.DeleteOnSubmit(_SYS_Config);
                }
                //deletions on submit set..
                try
                {
                    contextObj.SubmitChanges();
                    return true;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return false;
                    // Provide for exceptions.
                }

            }
        }
        public bool Upsert(JArray newRow)
        {
            //DECIDE WHETHER INSERT OR UPDATE
            using (DataClasses1DataContext contextObj = new DataClasses1DataContext())
            {
                var pKey = contextObj.Mapping.GetTable(currentTable.type).RowType.DataMembers.Where(m => m.IsPrimaryKey).ToArray().Select(p => p.MappedName).ToList();
                var newKey = new List<int>();
                foreach (var item in pKey)
                {
                    if (newRow[item] == null || newRow[item].ToString() == "")//if no primarykey -> ADD
                    {
                        newKey.Add(GenerateIdentityPKey(item));
                    }
                    else //EDIT
                    {

                    }
                }

                return true;
            }
        }

        public int GenerateIdentityPKey(string item)
        {
            using (DataClasses1DataContext contextObj = new DataClasses1DataContext())
            {
                JArray data = new JArray();
                JArray arr = GetJsonData(contextObj.GetTable(currentTable.type));
                foreach (JObject element in arr)
                {
                    data.Add(element[item]);
                }
                //var findMax = ;

                //have to generate it for script
                //find max id and add one FOR NOW
                return +1;
            }

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

        #region Old Table Stuff
        /* --------------------------------------- */
        //OLD TABLE FORMAT STUFF

        // GET: All SYS_Configs
        public JsonResult GetAllSYS_Configs()
        {
            using (DataClasses1DataContext contextObj = new DataClasses1DataContext())
            {
                var SYS_ConfigList = contextObj.SYS_Config.ToList();

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

                var getSYS_ConfigById = contextObj.SYS_Config.FirstOrDefault(s => s.OptionItem_ID == SYS_ConfigId);
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
                    SYS_Config _SYS_Configs = contextObj.SYS_Config.Where(b => b.OptionItem_ID == SYS_ConfigId).FirstOrDefault();

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
                    contextObj.SYS_Config.InsertOnSubmit(SYS_ConfigRecord); //is this the same as add?
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
                        var _SYS_Config = contextObj.SYS_Config.FirstOrDefault(s => s.OptionItem_ID == _SYS_ConfigId); //only one?
                        contextObj.SYS_Config.DeleteOnSubmit(_SYS_Config);//only one element?
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

public interface IEntity<T> where T : class
{
    IList<T> List();
    IList<T> List(int? page, int? pageSize, Expression<Func<T, bool>> predicate, Expression<Func<T, object>> sort);
    void Add(T item);
    T Get(Int64 Id);
    void Update(T item);
    bool Delete(T Item);
}





