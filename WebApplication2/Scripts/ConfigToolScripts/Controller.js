﻿//ANGULAR CONTROLLERS

/* ----- Generic table controller -------------------------------- */
app.controller("mvcCRUDCtrl", function ($scope, $routeParams, crudAJService) {
    console.log("Ctrl = mvcCRUDCtrl");
    $scope.tablename = $routeParams.tablename;
    console.log("Loading table: " + $scope.tablename);

    LoadTable();
    function LoadTable() {
        var loadTableMethod = crudAJService.loadTable($scope.tablename);
        //calls function when response is available

        loadTableMethod.then(function (TabData) {
            //headers for old grid
            $scope.TableHeaders = TabData.data.Item2.names;
            //json headers for new grid
            $scope.TableHeadersJson = JSON.parse(TabData.data.Item2.namesJson);
            //$scope.TableHeadersJson.push(TabData.data.Item2.namesJson);
            //data (in json)
            $scope.TableData = TabData.data.Item1;

            console.log("Headers: " + TabData.data.Item2.names);
            console.log("Json Headers: " + TabData.data.Item2.namesJson);
            console.log("Data: " + TabData.data.Item1);
        }, function () {
            alert('Error in getting table.');
        });

        var gridDiv = document.querySelector('#myGrid');
        //gridOptions.datasource = TabData.data.Item1;
        //console.log(gridOptions.datasource);
        var colHeaders = $scope.TableHeadersJson;
        var gridOptions = {
            //columnDefs:  colHeaders ,
            columnDefs: [
            {
                "headerName": "StoredProcedure",
                "field": "StoredProcedure"
            },
            {
                "headerName": "RunLogID",
                "field": "RunLogID"
            },
            {
                "headerName": "Locked",
                "field": "Locked"
            },
            {
                "headerName": "LockedDate",
                "field": "LockedDate"
            }
            ],
            rowData: $scope.TableData,
            //rowData: [
            //    { "StoredProcedure": "Procast_SP_AccountPlanBuild", "RunLogID": "3ee152b7-13c4-41d9-ab38-4279b9090d82", "Locked": "false", "LockedDate": "null" },
            //    { "StoredProcedure": "Procast_SP_AccountPlanBuild", "RunLogID": "3ee152b7-13c4-41d9-ab38-4279b9090d82", "Locked": "false", "LockedDate": "null" },
            //    { "StoredProcedure": "Procast_SP_AccountPlanBuild", "RunLogID": "3ee152b7-13c4-41d9-ab38-4279b9090d82", "Locked": "false", "LockedDate": "null" },
            //    { "StoredProcedure": "Procast_SP_AccountPlanBuild", "RunLogID": "3ee152b7-13c4-41d9-ab38-4279b9090d82", "Locked": "false", "LockedDate": "null" }
            //],

            enableSorting: true,
            enableFilter: true,
            debug: true,
            //paginationPageSize: 500,
            rowSelection: 'multiple',
            enableColResize: true
            //rowModelType: 'pagination'

        };
        new agGrid.Grid(gridDiv, gridOptions);

        //function onPageSizeChanged(newPageSize) {
        //    this.gridOptions.paginationPageSize = new Number(newPageSize);
        //    createNewDatasource();
        //}

        //// when json gets loaded, it's put here, and  the datasource reads in from here.
        //// in a real application, the page will be got from the server.
        //var allOfTheData;

        //function createNewDatasource() {
        //    if (!allOfTheData) {
        //        // in case user selected 'onPageSizeChanged()' before the json was loaded
        //        return;
        //    }

        //    var dataSource = {
        //        //rowCount: ???, - not setting the row count, infinite paging will be used
        //        getRows: function (params) {
        //            // this code should contact the server for rows. however for the purposes of the demo,
        //            // the data is generated locally, a timer is used to give the experience of
        //            // an asynchronous call
        //            console.log('asking for ' + params.startRow + ' to ' + params.endRow);
        //            setTimeout(function () {
        //                // take a chunk of the array, matching the start and finish times
        //                var rowsThisPage = allOfTheData.slice(params.startRow, params.endRow);
        //                // see if we have come to the last page. if we have, set lastRow to
        //                // the very last row of the last page. if you are getting data from
        //                // a server, lastRow could be returned separately if the lastRow
        //                // is not in the current page.
        //                var lastRow = -1;
        //                if (allOfTheData.length <= params.endRow) {
        //                    lastRow = allOfTheData.length;
        //                }
        //                params.successCallback(rowsThisPage, lastRow);
        //            }, 500);
        //        }
        //    };

        //    gridOptions.api.setDatasource(dataSource);
        //}

        //function setRowData(rowData) {
        //    allOfTheData = rowData;
        //    createNewDatasource();
        //}

        //// setup the grid after the page has finished loading
        //document.addEventListener('DOMContentLoaded', function () {
        //    var gridDiv = document.querySelector('#myGrid');
        //    new agGrid.Grid(gridDiv, gridOptions);

        //            setRowData($scope.TableData);

        //});

    }
});

/*-------- Side Nav ----------------------------------------*/
app.controller('NavCtrl', function ($scope, $timeout, $mdSidenav) {
      $scope.toggleLeft = buildToggler('left');
      $scope.toggleRight = buildToggler('right');

      function buildToggler(componentId) {
          return function () {
              $mdSidenav(componentId).toggle();
          }
      }
  });

/* ------- Side Nav controller -------------------------------- */
/**
 * Add two numbers.
 * @param {number} num The first number.
 * @returns The sum of the two numbers.
 */
app.controller('SideNavCtrl', function ($scope, $timeout, $mdSidenav, $log) {
    $scope.toggleLeft = buildDelayedToggler('left');
    $scope.toggleRight = buildToggler('right');
    $scope.isOpenRight = function () {
        return ( $mdSidenav('right').isOpen() );
    };

    /**
     * Supplies a function that will continue to operate until the
     * time is up.
     */
    function debounce(func, wait, context) {
        var timer;

        return function debounced() {
            var context = $scope,
                args = Array.prototype.slice.call(arguments);
            $timeout.cancel(timer);
            timer = $timeout(function () {
                timer = undefined;
                func.apply(context, args);
            }, wait || 10);
        };
    }

    /**
     * Build handler to open/close a SideNav; when animation finishes
     * report completion in console
     */
    function buildDelayedToggler(navID) {
        return debounce(function () {
            // Component lookup should always be available since we are not using `ng-if`
            $mdSidenav(navID)
              .toggle()
              .then(function () {
                  $log.debug("toggle " + navID + " is done");
              });
        }, 200);
    }

    function buildToggler(navID) {
        return function () {
            // Component lookup should always be available since we are not using `ng-if`
            $mdSidenav(navID)
              .toggle()
              .then(function () {
                  $log.debug("toggle " + navID + " is done");
              });
        }
    }
})
app.controller('LeftCtrl', function ($scope, $timeout, $mdSidenav, $log) {
    $scope.close = function () {
        // Component lookup should always be available since we are not using `ng-if`
        $mdSidenav('left').close()
          .then(function () {
              $log.debug("close LEFT is done");
          });

    };
})
//app.controller('RightCtrl', function ($scope, $timeout, $mdSidenav, $log) {
//    $scope.close = function () {
//        // Component lookup should always be available since we are not using `ng-if`
//        $mdSidenav('right').close()
//          .then(function () {
//              $log.debug("close RIGHT is done");
//          });
//    };
//});
//app.controller('MyController', function ($scope, $mdSidenav) {
//    $scope.openLeftMenu = function () {
//        $mdSidenav('left').toggle();
//    };
//});


/* ------- Angular Material Setup -------------------------------- */
app.controller('SetupCtrl', function ($scope, $routeParams) {
    //instead send type of setup as routeParam?
    $scope.setup = {
        username: '',
        project: '',
        client: '',
        database: '',
        company: 'Exceedra',
        startdate: '',
        leveloption: '',
        notes: ' '
    };

    $scope.leveloptions = ('Demo Setup All DataManipulation').split(' ').map(function (leveloption) {
        return { abbrev: leveloption };
    });

    $scope.process = $routeParams.process;

    $scope.start = function (process) {
        console.log("start process: " + process);

    };

    $scope.autofill = function () {
        //var date = now();
        //console.log('date' + date);
        console.log("autofill");
        console.log("autofill");
        $scope.setup = {
            username: 'Name',
            project: 'Project',
            client: 'Exceedra',
            database: 'Default',
            company: 'Exceedra',
            startdate: '',
            leveloption: 'All',
            notes: ''

        }
    }
    $scope.clear = function () {
        console.log("clear");
        $scope.setup = {
            username: '',
            project: '',
            client: '',
            database: '',
            company: 'Exceedra',
            startdate: '',
            leveloption: '',
            notes: ' '
        }
    }
})
app.config(function ($mdThemingProvider) {
    $mdThemingProvider.theme('docs-dark', 'default')
    .primaryPalette('yellow')
    .dark();

});

/* ----------- Main controller -------------------------------------- */
//This will be loaded AFTER setup - only then does the tool know what database it is connecting to
//Login -> Setup (what database, whether wizard(if option), etc) -> loads tables into menu and starts whatever selected
app.controller("mainCtrl", function ($scope, crudAJService) {
    console.log("Ctrl = mainCtrl");
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

});

/* ----------- Test table controller ------------------------------------------ */
app.controller("getdataCtrl", function ($scope, crudAJService) {
    console.log("Ctrl = getdataCtrl");
    LoadDefaultTable();
    function LoadDefaultTable() {
        console.log("Load default table");
        var getSYS_ConfigData = crudAJService.getSYS_Configs();
        //Console.log("data:" + getSYS_ConfigData);
        getSYS_ConfigData.then(function (SYS_Config) {
            //Now returns tuple of data (item1) and header info..
            $scope.SYS_Configs = SYS_Config.data.Item1;
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
                LoadDefaultTable();
                alert(msg.data);
                $scope.divSYS_Config = false;
            }, function () {
                alert('Error in updating record');
            });
        } else {
            getSYS_ConfigData = crudAJService.AddSYS_Config(SYS_Config);
            getSYS_ConfigData.then(function (msg) {
                LoadDefaultTable();
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
            LoadDefaultTable();
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

/* ------------- Angular routing ------------------------------------ */
app.config(function ($routeProvider,
    $locationProvider) {
    $routeProvider
        .when("/", {
            templateUrl: function () { console.log("HomeView2"); return "/Home/Home"; }
        })
        .when("/Index", {
            templateUrl: function () { console.log("IndexView"); return "/Home/Index"; }
        })
        .when("/Setup", {
            templateUrl: function () { console.log("SetupView"); return "/Home/Setup"; }
        })
        .when("/Login", {
            templateUrl: function () { console.log("Login"); return "Account/Login"; }
        })
        .when('/Table/:tablename', {
            //tablename is a 'route parameter'
            templateUrl: function (params) { console.log("params.tablename: " + params.tablename); return "/Home/Table?tablename=" + params.tablename; },
            controller: 'mvcCRUDCtrl'
        })
        .when("/Contact", {
            templateUrl: function (params) { console.log("HomeView3" + params.process); return "/Home/Home?process=" + params.process; },

        })
        .otherwise({ redirectTo: '/Home/Contact' });

});

