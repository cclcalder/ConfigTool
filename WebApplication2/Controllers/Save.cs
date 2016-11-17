using ConfigTool;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Web;
using static WebApplication2.Controllers.HomeController;

namespace WebApplication2.Controllers
{
    public class Save
    {
        public static string Main(string[] changes, Tuple<string, Type> table)
        {
            using (var contextObj = new DataClasses1DataContext())
            {
                if (changes != null)
                {
                    var tableName = table.Item1;
                    var tableType = table.Item2;
                    var aTables = new List<AssociatedTable>();
                    var headers = new List<Header>(GetHeaders(tableName, aTables));
                    var children = aTables.Where(a => a.relationToCurrent == "Child").ToList();
                    var nonIdCols = headers.Where(h => !h.type.Contains("IDENTITY")).Select(s => s.name);
                    var idCols = nonIdCols as string[] ?? nonIdCols.ToArray();
                    var stringCols = string.Join(", ", idCols.ToArray());

                    // Get pKey, if identity exists get that otherwise get pKey single or cluster
                    var pKeyId =
                           new Tuple<List<string>, string>(
                               contextObj.Mapping.GetTable(tableType)
                                   .RowType.DataMembers.Where(m => m.DbType != null && m.DbType.Contains("IDENTITY"))
                                   .ToArray()
                                   .Select(p => p.MappedName)
                                   .ToList(), "ID");
                    if (!pKeyId.Item1.Any())
                    {
                        pKeyId =
                            new Tuple<List<string>, string>(
                                contextObj.Mapping.GetTable(tableType)
                                    .RowType.DataMembers.Where(m => m.IsPrimaryKey)
                                    .ToArray()
                                    .Select(p => p.MappedName)
                                    .ToList(), "PK");
                        if (pKeyId.Item1.Count > 1)
                        {
                            pKeyId =
                                new Tuple<List<string>, string>(
                                    contextObj.Mapping.GetTable(tableType)
                                        .RowType.DataMembers.Where(m => m.IsPrimaryKey)
                                        .ToArray()
                                        .Select(p => p.MappedName)
                                        .ToList(), "PKMULTI");
                        }
                    }
                    var conditionType = pKeyId.Item2;
                    var identityCol = conditionType == "ID" ? pKeyId.Item1[0] : null;

                    // Group changes
                    var toInsert = changes.Where(c => (string)JsonConvert.DeserializeObject<JObject>(c.Replace("\\", ""))["hasChanges"] == "1").ToArray();
                    var toDelete = changes.Where(c => (string)JsonConvert.DeserializeObject<JObject>(c.Replace("\\", ""))["hasChanges"] == "2").ToArray();
                    var toUpdate = changes.Where(c => (string)JsonConvert.DeserializeObject<JObject>(c.Replace("\\", ""))["hasChanges"] == "3").ToArray();
                    Debug.Assert(toDelete.Length + toInsert.Length + toUpdate.Length == changes.Length);

                    var deleteQuery = "";
                    var updateQuery = "";
                    var insertQuery = "";

                    // Two types of query build
                    if (!children.Any())
                    {
                        deleteQuery = BuildDelete(toDelete, pKeyId);
                        updateQuery = BuildUpdate(toUpdate, pKeyId);
                        insertQuery = BuildInsert(toInsert, pKeyId);
                    }
                    else if (children.Any())
                    {
                        deleteQuery = BuildDelete(toDelete, pKeyId);
                        updateQuery = BuildUpdate(toUpdate, pKeyId);
                        insertQuery = BuildInsert(toInsert, pKeyId);

                        deleteQuery = CascadeDelete(deleteQuery, children);
                        updateQuery = CascadeUpdate(updateQuery, children);
                        insertQuery = CascadeInsert(insertQuery, children);
                        // Add original queries at bottom
                        deleteQuery += BuildDelete(toDelete, pKeyId);
                        updateQuery += BuildUpdate(toUpdate, pKeyId);
                        insertQuery += BuildInsert(toInsert, pKeyId);
                    }

                    // Iterate thorugh and group changes 
                    foreach (var change in changes)
                    {
                        var jsonChange = JsonConvert.DeserializeObject<JObject>(change.Replace("\\", ""));
                        var action = (string)jsonChange["hasChanges"] == "2" ? "delete" : "upsert";
                        var idValue = (string)jsonChange[pKeyId.Item1[0]];
                        var idCol = pKeyId.Item1[0];
                        if (conditionType == "MULTI")
                        {
                            idValue = "";
                            foreach (var item in pKeyId.Item1)
                            {
                                idValue += (string)jsonChange[item] + ",";
                            }
                            idValue = idValue.TrimEnd(',');
                        }
                        //idValue and idCol are now singles or csv arrays of values (depending on whether idType is ID, PK or MULTI)

                        /* NEW METHOD FOR SAVE..
                        *  GET TABLE KEYS AND ASSOCIATED (REUSE CODE!)
                        *  1 GET QUERIES
                        *  2 GROUP TO DELETE/INSERT/UPDATE
                        *  3 WRITE MULTI QUERIES
                        *  4 EXECUTE QUERIES
                        *  IF SUCCEED RETURN SUCCESS TO JAVASCRIPT, AND WRITE TO MERGE SCRIPT 
                        */
                        // GROUP TO DELETE/INSERT/UPDATE

                        //switch (action)
                        //{
                        //    case "delete":                 
                        //        if (idType == "ID" || idType == "PK")
                        //        {
                        //            //DELETE *
                        //            //FROM    app.Dim_Users
                        //            //WHERE   User_Idx IN (1, 2, 3) 

                        //            var condition = idCol + " = '" + idValue + "'";
                        //            string deleteQuery = "DELETE FROM app." + tableName + " WHERE " + condition;
                        //            if (children.Any())
                        //            {
                        //                deleteQuery = CascadeDelete(deleteQuery, children);
                        //            }
                        //            try
                        //            {
                        //                var execute = contextObj.ExecuteCommand(deleteQuery);
                        //            }
                        //            catch (SqlException sqlex)
                        //            {
                        //                return sqlex.Message;
                        //            }
                        //        }
                        //        else  
                        //        {
                        //            //new method bbi USING PRIMARY KEYS AND STUFF
                        //            string condition = "";
                        //            //idCol + " = '" + idValue + "'"
                        //            foreach (var i in pKeyId.Item1)
                        //            {
                        //                var couple = i + " = '" + (string)jsonChange[i] + "'";
                        //                condition += couple + " AND ";
                        //            }
                        //            condition = condition.TrimEnd(" AND ".ToCharArray());
                        //            string deleteQuery = "DELETE FROM app." + tableName + " WHERE " + condition;
                        //            try
                        //            {
                        //                var record = contextObj.ExecuteCommand(deleteQuery);
                        //            }
                        //            catch (SqlException sqlex)
                        //            {
                        //                return sqlex.Message;
                        //            }
                        //        }
                        //        break;

                        //    case "upsert":
                        //        // Check if insert or update
                        //        if (idType == "ID") //if contains identity col
                        //        {
                        //            if (idValue == null) //identitycolumn value doesnt exist
                        //            {
                        //                // -- INSERT
                        //                var record = idCols.ToList().Aggregate("", (current, head) => current + ("'" + (string) jsonChange[head] + "',"));
                        //                record = record.TrimEnd(',');
                        //                var insertQuery = "INSERT INTO app." + tableName + " ( " + stringCols +
                        //                                  " ) SELECT " +
                        //                                  record;
                        //                //insertForMerge.Add("( " + stringCols + " ) VALUES " + record);

                        //                try
                        //                {
                        //                    var result = contextObj.ExecuteCommand(insertQuery);
                        //                }
                        //                catch (SqlException sqlex)
                        //                {
                        //                    return sqlex.Message;
                        //                }
                        //                //new method bbi USING PRIMARY KEYS AND STUFF
                        //            }
                        //            else
                        //            {
                        //                // -- UPDATE
                        //                var newValues = idCol + " = '" + idValue;
                        //                string updateQuery = "UPDATE app." + tableName + " SET " + newValues + "' WHERE " +
                        //                                     idCol +
                        //                                     " = '" + idValue + "'";
                        //                //updateForMerge.Add(" SET " + idCol + " = '" + idValue + "'");
                        //                try
                        //                {
                        //                    var record = contextObj.ExecuteCommand(updateQuery);
                        //                }
                        //                catch (SqlException sqlex)
                        //                {
                        //                    return sqlex.Message;
                        //                }
                        //                //new method bbi USING PRIMARY KEYS AND STUFF
                        //            }
                        //        }
                        //        break;
                        //}
                    }
                    var matchKeys = "TGT.OptionItemDetail = SRC.OptionItemDetail AND TGT.OptionItem = SRC.OptionItem";
                    //JUST AN EXAMPLE
                    //notes:
                    //not all tables are app. - this is easy fix
                    var merge = @"MERGE INTO app." + tableName + " TARGET " +
                                   "USING ( SELECT " + stringCols + " FROM app." + tableName + " ) SOURCE " +
                                   "ON " + matchKeys +
                                   "WHEN MATCHED AND " + matchKeys + //NOT RIGHT
                                                                     //"THEN UPDATE" + updateForMerge.ToString() +
                                                                     //"WHEN  NOT MATCHED BY TARGET THEN INSERT " + insertForMerge.ToString() +
                                   "WHEN NOT MATCHED BY SOURCE THEN DELETE";

                    //TRY TO EXCUTE TO DATACONTEXT


                    //IF SUCCESS WRITE QUERY TO FILE
                }
            }
            return "Success";
        }

        public static string BuildDelete(string[] toDelete, Tuple<List<string>, string>pKeyId)
        {
            return "test";
        }
        public static string BuildUpdate(string[] toDelete, Tuple<List<string>, string> pKeyId)
        {
            return "test";
        }
        public static string BuildInsert(string[] toDelete, Tuple<List<string>, string> pKeyId)
        {
            return "test";
        }
        public static string CascadeDelete(string deleteQuery, List<AssociatedTable> children)
        {
            //dont need to get values - the point is that theyre the same as initial delete
            //just need to get 
            //need to check this will write query in the right order
            foreach (var child in children)
            {
                //check whether child has children
                var grandchildren = GetChildren(child.name);
                if (grandchildren.Any())
                {
                    CascadeDelete(deleteQuery, grandchildren);
                }
                // Find name and condition and call multiDelete method
                var childDelete = "";
            }

            return deleteQuery;

        }

        public static string CascadeUpdate(string updateQuery, List<AssociatedTable> children)
        {
            var updateChildren = "";
            foreach (var child in children)
            {
                updateChildren = "ble";

            }

            updateQuery = updateChildren + updateQuery;
            return "hi";
        }

        public static string CascadeInsert(string updateQuery, List<AssociatedTable> children)
        {
            var updateChildren = "";
            foreach (var child in children)
            {
                updateChildren = "ble";

            }

            updateQuery = updateChildren + updateQuery;
            return "hi";
        }

        public string CreateMultiDelete(string name, string[] condition)
        {
            var deleteQuery = "";
            for (var i = 0; i < condition.Length; i++)
            {
                deleteQuery = "DELETE FROM app." + name + " WHERE " + condition[i];
                deleteQuery += deleteQuery;
            }
            return deleteQuery;
        }

        //public string CreateMultiInsert(string name, string[] idCol, string[] idValue)
        //{
        //    var deleteQuery = "";
        //    for (var i = 0; i < idCol.Count(); i++)
        //    {
        //        deleteQuery = "DELETE FROM app." + name + " WHERE " + idCol[i] + " = '" + idValue[i] + "'";
        //        deleteQuery += deleteQuery;
        //    }
        //    return deleteQuery;
        //}

        //public string CreateMultiUpdate(string name, string[] idCol, string[] idValue)
        //{
        //    var deleteQuery = "";
        //    for (var i = 0; i < idCol.Count(); i++)
        //    {
        //        deleteQuery = "DELETE FROM app." + name + " WHERE " + idCol[i] + " = '" + idValue[i] + "'";
        //        deleteQuery += deleteQuery;
        //    }
        //    return deleteQuery;
        //}

        public static List<AssociatedTable> GetChildren(string tableName)
        {
            var aTables = new List<AssociatedTable>();
            //probably better, faster, to write specific assocaited table method rather than getting all info don't need?
            GetHeaders(tableName, aTables); //use method to update aTables 
            return aTables.Where(a => a.relationToCurrent == "Child").ToList();
        }
    }
}