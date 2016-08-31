app.controller("mvcCRUDCtrl", function ($scope, crudAJService) {

    //Loads all table names from the data base into navbar
    GetAllTableNames();
    function GetAllTableNames() {
        console.log("get table names");
        var getTableNameData = crudAJService.getTables();
        getTableNameData.then(function (Table) {
            $scope.TableList = Table.data;
        }, function () {
            alert('Error in getting Table Names');
        });
    }

    $scope.LoadTableData = function (table) {
        console.log("Selected table to load:" + table);

        var loadTableMethod = crudAJService.loadTable(table);

        console.log("loadTableMethod from service.js:" + loadTableMethod);
        //calls function when response is available
        loadTableMethod.then(function (TabData) {
            console.log("in loadtablemethod");
            $scope.TableHeaders = TabData.data.Item2;
            $scope.TableData = TabData.data.Item1;
            console.log("TabData.data.Item2.names:" + TabData.data.Item2.names);
            console.log("TabData.data.Item1:" + TabData.data.Item1);
        }, function () {
            alert('Error in getting table.');
        });
        //Now put this data into 'Table'
    }

    $scope.divSYS_Config = false;

    //This is SYS_Config at the moment
    //LoadDefaultTable();
    //function LoadDefaultTable() {
    //    console.log("Load default data");
    //    var getSYS_ConfigData = crudAJService.getSYS_Configs();
    //    getSYS_ConfigData.then(function (SYS_Config) {
    //        $scope.SYS_Configs = SYS_Config.data.Item1; //now returns tuple of data (item1) and header info . . 
    //    }, function () {
    //        alert('Error in getting SYS_Config records');
    //    });
    //}

    /* ------------------------------------------------------------------------- */
    //Replace this
    $scope.GetAllSYS_Configs = function () {
        console.log("Get SYS_Config data");
        var getSYS_ConfigData = crudAJService.getSYS_Configs();
        getSYS_ConfigData.then(function (SYS_Config) {
            console.log("SYS_Config.data.Item1:" + SYS_Config.data.Item1);
            $scope.SYS_Configs = SYS_Config.data.Item1; //now returns tuple of data (item1) and header info . . 
        }, function () {
            alert('Error in getting SYS_Config records');
        });
    }

    $scope.editSYS_Config = function (SYS_Config) {
        console.log("edit");
        var getSYS_ConfigData = crudAJService.getSYS_Config(SYS_Config.Id);
        console.log("getSYS_ConfigData from service.js:" + getSYS_ConfigData);
        getSYS_ConfigData.then(function (_SYS_Config) {
            $scope.SYS_Config = _SYS_Config.data.Item1;
            $scope.SYS_ConfigId = SYS_Config.OptionItem_ID;
            $scope.SYS_ConfigIsEditable = SYS_Config.IsEditable;
            $scope.SYS_ConfigMenuItem_Code = SYS_Config.MenuItem_Code;
            $scope.SYS_ConfigOptionItem = SYS_Config.OptionItem;
            $scope.SYS_ConfigOptionItemDetail = SYS_Config.OptionItemDetail;
            $scope.SYS_ConfigOptionItemDetail_Value = SYS_Config.OptionItemDetail_Value;
            $scope.Action = "Update";
            $scope.divSYS_Config = true;

        }, function () {
            alert('Error in getting SYS_Config records');
        });
    };

    $scope.AddUpdateSYS_Config = function () {
        console.log("update");
        var SYS_Config = {

            OptionItem_ID: $scope.SYS_ConfigId,
            IsEditable: $scope.SYS_ConfigIsEditable,
            MenuItem_Code: $scope.SYS_ConfigMenuItem_Code,
            OptionItem: $scope.SYS_ConfigOptionItem,
            OptionItemDetail: $scope.SYS_ConfigOptionItemDetail,
            OptionItemDetail_Value: $scope.SYS_ConfigOptionItemDetail_Value

        };
        var getSYS_ConfigAction = $scope.Action;

        if (getSYS_ConfigAction === "Update") {
            SYS_Config.Id = $scope.SYS_ConfigId;
            var getSYS_ConfigData = crudAJService.updateSYS_Config(SYS_Config);
            getSYS_ConfigData.then(function (msg) {
                GetAllSYS_Configs();
                alert(msg.data);
                $scope.divSYS_Config = false;
            }, function () {
                alert('Error in updating record');
            });
        } else {
            getSYS_ConfigData = crudAJService.AddSYS_Config(SYS_Config);
            getSYS_ConfigData.then(function (msg) {
                GetAllSYS_Configs();
                alert(msg.data);
                $scope.divSYS_Config = false;
            }, function () {
                alert('Error in adding record');
            });
        }
    };

    $scope.AddSYS_ConfigDiv = function () {
        console.log("add");
        ClearFields();
        $scope.Action = "Add";
        $scope.divSYS_Config = true;
    };

    $scope.deleteSYS_Config = function (SYS_Config) {
        console.log("delete");
        var getSYS_ConfigData = crudAJService.DeleteSYS_Config(SYS_Config.OptionItem_ID);
        getSYS_ConfigData.then(function (msg) {
            alert(msg.data);
            GetAllSYS_Configs();
        }, function () {
            alert('Error in deleting record');
        });
    };

    function ClearFields() {
        console.log("clear");
        $scope.SYS_Config = "";
        $scope.SYS_ConfigId = "";
        $scope.SYS_ConfigIsEditable = "";
        $scope.SYS_ConfigMenuItem_Code = "";
        $scope.SYS_ConfigOptionItem = "";
        $scope.SYS_ConfigOptionItemDetail = "";
        $scope.SYS_ConfigOptionItemDetail_Value = "";
    }
    $scope.Cancel = function () {
        console.log("cancel");
        $scope.divSYS_Config = false;
    };

});
/* ------------------------------------------------------------------------- */

app.controller("getdataCtrl", function ($scope, crudAJService) {
    LoadDefaultTable();
    function LoadDefaultTable() {
        console.log("Load default table");
        var getSYS_ConfigData = crudAJService.getSYS_Configs();
        console.log("data:" + getSYS_ConfigData);
        getSYS_ConfigData.then(function (SYS_Config) {
            //console.log("inmethod: " + SYS_Config.data.Item1);
            $scope.SYS_Configs = SYS_Config.data.Item1; //now returns tuple of data (item1) and header info . . 
        }, function () {
            alert('Error in getting SYS_Config records');
        });
    }
});

/* ------------------------------------------------------------------------- */

//app.controller("getableCtrl", function ($scope, crudAJService) {
//    LoadDefaultTable();
//    function LoadDefaultTable() {
//        console.log("Load default data");
//        var getSYS_ConfigData = crudAJService.getSYS_Configs();
//        console.log("data:" + getSYS_ConfigData);
//        getSYS_ConfigData.then(function (SYS_Config) {
//            //console.log("inmethod: " + SYS_Config.data.Item1);
//            $scope.SYS_Configs = SYS_Config.data.Item1; //now returns tuple of data (item1) and header info . . 
//        }, function () {
//            alert('Error in getting SYS_Config records');
//        });
//    }
//});

/* ------------------------------------------------------------------------- */

app.config(function ($routeProvider) {
    $routeProvider
        .when("#", {
            templateUrl: "/Home/Home"
        })
        .when("/Index", {
            templateUrl: "/Home/Index"
        })
        .when("/Setup", {
            templateUrl: "/Setup/Setup"
        })
        .when("/Table", {
            templateUrl: function (params) { console.log("tableview"); return "/Home/Table?tablename=" + params.tablename; }
            //templateUrl: function (params) { console.log("tableview"); return '/Home/Table?tablename=' + params.tablename; }
        })
        .when("/Home", {
            templateUrl: "Home"
        });
})