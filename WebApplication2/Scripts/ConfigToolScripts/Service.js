﻿app.service("crudAJService", function ($http) {

    //get All SYS_Configs
    this.getSYS_Configs = function () {
        return $http.get("Home/GetAllSYS_Configs");
    };

    this.getTables = function () {
        return $http.get("Home/GetAllTableNames");

    };

    this.loadTable = function (table) {
        var response = $http({
            method: "post",
            url: "Home/LoadTableData",
            params: {
                table: JSON.stringify(table)
            }
        });
        //return above function
        return response;

        //return $http.get("Home/LoadTableData");
    };

    /* --------------------------------------- */

    // get SYS_Config by SYS_ConfigId
    this.getSYS_Config = function (SYS_ConfigId) {
        var response = $http({
            method: "post",
            url: "Home/GetSYS_ConfigById",
            params: {
                id: JSON.stringify(SYS_ConfigId)
            }
        });
        return response;
    };

    // Update SYS_Config 
    this.updateSYS_Config = function (SYS_Config) {
        var response = $http({
            method: "post",
            url: "Home/UpdateSYS_Config",
            data: JSON.stringify(SYS_Config),
            dataType: "json"
        });
        return response;
    };

    // Add SYS_Config
    this.AddSYS_Config = function (SYS_Config) {
        var response = $http({
            method: "post",
            url: "Home/AddSYS_Config",
            data: JSON.stringify(SYS_Config),
            dataType: "json"
        });
        return response;
    };

    //Delete SYS_Config
    this.DeleteSYS_Config = function (SYS_ConfigId) {
        var response = $http({
            method: "post",
            url: "Home/DeleteSYS_Config",
            params: {
                SYS_ConfigId: JSON.stringify(SYS_ConfigId)
            }
        });
        return response;
    };

    /* --------------------------------------- */
    //Generic Table view - same as above ish

    // Get record by id
    this.getRecord = function (RecordId) {
        var response = $http({
            method: "post",
            url: "Table/GetRecordById",
            params: {
                id: JSON.stringify(RecordId)
            }
        });
        return response;
    };

    // Update record 
    this.updateRecord = function (Record) {
        var response = $http({
            method: "post",
            url: "Table/UpdateRecord",
            data: JSON.stringify(Record),
            dataType: "json"
        });
        return response;
    };

    // Add record
    this.AddRecord = function (Record) {
        var response = $http({
            method: "post",
            url: "Table/AddRecord",
            data: JSON.stringify(Record),
            dataType: "json"
        });
        return response;
    };

    //Delete record
    this.DeleteRecord = function (RecordId) {
        var response = $http({
            method: "post",
            url: "Table/DeleteRecord",
            params: {
                SYS_ConfigId: JSON.stringify(Record)
            }
        });
        return response;
    };


});