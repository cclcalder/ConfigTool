app.service("crudAJService", function ($http) {
        ////get Tables Names 
    //this.getTableNames = function (){
    //    return $http.get("Home/GetTableNames");
    //}
    //get All SYS_Configs
    this.getSYS_Configs = function () {
        return $http.get("Home/GetAllSYS_Configs");
    };

    this.getTables = function () {
        return $http.get("Home/GetAllTableNames");
    };

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

    this.LoadTable = function (table) {
        var response = $http({
            method: "post",
            url: "Home/LoadTable",
            params: {
                table: JSON.stringify(table)
            }
        });
        return response;

        //return $http.get("Home/LoadTableData");
    };

});