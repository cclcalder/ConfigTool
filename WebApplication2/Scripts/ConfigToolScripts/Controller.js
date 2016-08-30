app.controller("mvcCRUDCtrl", function ($scope, crudAJService){
    $scope.divSYS_Config = false;

    //equivalent of get SYS_Config etc but now generic, can be any na
    //GetAllTableData();
    //function GetAllTableData() {
    //    console.log("Load data from DB table");

    //    var getTableData = crudAJService.getTable();
    //    getTable.then(function (Data) {
    //        $scope.tableData = Data.data;
    //    }, function () {
    //        alert('Error in getting records');
    //    });
    //}

    GetAllSYS_Configs();
    //To Get all SYS_Config records  
    function GetAllSYS_Configs() {
        console.log("get SYS_Configs");
        //debugger;
        //var self = this; //this
        var getSYS_ConfigData = crudAJService.getSYS_Configs();
        console.log("getSYS_ConfigData = " + getSYS_ConfigData);
        //self.tableParams = new NgTableParams({}, { dataset: getSYS_ConfigData }); //and this
        getSYS_ConfigData.then(function (SYS_Config) {
            $scope.SYS_Configs = SYS_Config.data.Item1; //now returns tuple of data (item1) and header info . . 
        }, function () {
            alert('Error in getting SYS_Config records');
        });
    }

    //need to get this form the wizard now? I think
    //easier - will just be json object to fill nav . . . .
    GetAllTableNames();
    function GetAllTableNames() {
        console.log("get table names");
        var getTableNameData = crudAJService.getTables();
        console.log("getTableNameData = "+getTableNameData);
        getTableNameData.then(function (TableList) {
            $scope.Tables = TableList.data;
        }, function () {
            alert('Error in getting Table Names');
        });
    }

    $scope.LoadTableData = function (table) {
        console.log("Selected table to load:" + table);

        var loadTablemethod = crudAJService.LoadTable(table);
        loadTablemethod.then(function (msg) {
            GetAllSYS_Configs();
            alert(msg.data);
            $scope.divSYS_Config = false;
        }, function () {
            alert('Error in getting table?');
        });
    }




    $scope.editSYS_Config = function (SYS_Config) {
        console.log("edit");
        var getSYS_ConfigData = crudAJService.getSYS_Config(SYS_Config.Id);
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

    
    
    $scope.predicate = 'naOptime';
    $scope.reverse = true;
    $scope.currentPage = 1;
    $scope.order = function (predicate) {
        console.log("order");

        $scope.reverse = ($scope.predicate === predicate) ? !$scope.reverse : false;
        $scope.predicate = predicate;
    };

    $scope.totalItems = 177;
    //console.log($scope.SYS_Configs.Count());
    $scope.numPerPage = 5;
    console.log("(total, num/page)" + ": " + $scope.totalItems + "," + $scope.numPerPage);
    $scope.paginate = function (value) {
        console.log("paginate");
            console.log("paginate");
            var begin, end, index;
            begin = ($scope.currentPage - 1) * $scope.numPerPage;
            end = begin + $scope.numPerPage;
            index = $scope.students.indexOf(value);
            return (begin <= index && index < end);
        };
    


});

//angular.module('MyApp', ['ngMaterial', 'ngMessages', 'material.svgAssetsCache'])
//    .controller('DemoCtrl', function ($interval) {
//        this.elevation = 1;
//        this.showFrame = 3;

//        this.nextElevation = function () {
//            if (++this.elevation == 25) {
//                this.elevation = 1;
//            }
//        };

//        $interval(this.nextElevation.bind(this), 500);

//        this.toggleFrame = function () {
//            this.showFrame = this.showFrame == 3 ? -1 : 3;
//        };
//    });

app.controller('SwitchDemoCtrl', function ($scope) {
    $scope.data = {
        cb1: true,
        cb4: true,
        cb5: false
    };
    $scope.message = 'false';
    $scope.onChange = function (cbState) {
        $scope.message = cbState;
    };
});

/**
Copyright 2016 Google Inc. All Rights Reserved. 
Use of this source code is governed by an MIT-style license that can be foundin the LICENSE file at http://material.angularjs.org/HEAD/license.
**/


