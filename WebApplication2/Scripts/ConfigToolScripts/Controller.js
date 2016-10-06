/* ----- ANGULAR CONTROLLERS ----- */
/* ----- agrGrid CRUD ----- */
// angular js module
app.controller("TableCtrl", function ($scope, $routeParams, $timeout, crudAJService) {
    console.log("TableCtrl");
    $scope.tablename = $routeParams.tablename;
    console.log("Loading table: " + $scope.tablename);

    LoadTableContent();
    function LoadTableContent() {
        var loadMethod = crudAJService.loadTableContent($scope.tablename);
        loadMethod.then(function (TableContent) {
            console.log(TableContent.data.Item2);
            console.log(TableContent.data.Item1);
            $scope.tableheaders = TableContent.data.Item1;
            $scope.tabledata = TableContent.data.Item2;
        });
    };

    // data for listing
    var data = $scope.tabledata;
    // listed data formatted for angularjs column defination
    var columnDefs = $scope.tableheaders;


    // drop down options for customEditorUsingAngular and customEditorNoAngular
    var setSelectionOptions = ['AAA', 'BBB', 'CCC', 'DDD', 'EEE', 'FFF', 'GGG'];
    // insert new record
    $scope.addRecord = function () {
        // a record initialization
        var newRecord = { defaultString: ' ', upperCaseOnly: ' ', number: 0 , setAngular: ' ', setNoAngular: ' ' };
        // push new record in row data
        $scope.gridOptions.rowData.push(newRecord);
        // overwrite row data
        $scope.gridOptions.api.setRowData($scope.gridOptions.rowData);
    }
    // delete record from data
    function deleteRecord(params) {
        var html = '<a title="Remove" href="javascript:;" class="align-center btn-link btn-sm" ng-click="RemoveRecord(' + params.rowIndex + ')">Delete</a>';
        return html;
    }
    // remove record
    $scope.RemoveRecord = function (index) {
        // one index splice
        $scope.gridOptions.rowData.splice(index, 1);
        // overwrite row data
        $scope.gridOptions.api.setRowData($scope.gridOptions.rowData);
    }
    // initialization of grid options          
    $scope.gridOptions = {
        columnDefs: columnDefs,
        rowData: data,
        angularCompileRows: true
    };
    // convert value to upper case
    function upperCaseNewValueHandler(params) {
        params.data[params.colDef.field] = params.newValue.toUpperCase();
    }
    function numberNewValueHandler(params) {
        var valueAsNumber = parseInt(params.newValue);
        if (isNaN(valueAsNumber)) {
            window.alert("Invalid value " + params.newValue + ", must be a number");
        } else {
            params.data.number = valueAsNumber;
        }
    }
    // custom editable using angular
    function customEditorUsingAngular(params) {
        params.$scope.setSelectionOptions = setSelectionOptions;
        var html = '<span ng-show="!editing" ng-click="startEditing()">{{data.' + params.colDef.field + '}}</span> ' +
            '<select ng-blur="editing=false" ng-change="editing=false" ng-show="editing" ng-options="item for item in setSelectionOptions" ng-model="data.' + params.colDef.field + '">';
        // we could return the html as a string, however we want to add a 'onfocus' listener, which is no possible in AngularJS
        var domElement = document.createElement("span");
        domElement.innerHTML = html;
        params.$scope.startEditing = function () {
            params.$scope.editing = true; // set to true, to show dropdown
            // put this into $timeout, so it happens AFTER the digest cycle,
            // otherwise the item we are trying to focus is not visible
            $timeout(function () {
                var select = domElement.querySelector('select');
                select.focus();
            }, 0);
        };
        return domElement;
    }
    // custom editable without angular
    function customEditorNoAngular(params) {
        var editing = false;
        var eCell = document.createElement('span');
        var eLabel = document.createTextNode(params.value);
        eCell.appendChild(eLabel);
        var eSelect = document.createElement("select");
        setSelectionOptions.forEach(function (item) {
            var eOption = document.createElement("option");
            eOption.setAttribute("value", item);
            eOption.innerHTML = item;
            eSelect.appendChild(eOption);
        });
        eSelect.value = params.value;
        // add click event
        eCell.addEventListener('click', function () {
            if (!editing) {
                eCell.removeChild(eLabel);
                eCell.appendChild(eSelect);
                eSelect.focus();
                editing = true;
            }
        });
        // add blur event
        eSelect.addEventListener('blur', function () {
            if (editing) {
                editing = false;
                eCell.removeChild(eSelect);
                eCell.appendChild(eLabel);
            }
        });
        // add change event
        eSelect.addEventListener('change', function () {
            if (editing) {
                editing = false;
                var newValue = eSelect.value;
                params.data[params.colDef.field] = newValue;
                eLabel.nodeValue = newValue;
                eCell.removeChild(eSelect);
                eCell.appendChild(eLabel);
            }
        });
        return eCell;
    }
});

/* ----- Generic Table Editor ----- */
app.controller("TableEditorCtrl", function ($scope, $routeParams, crudAJService) {
    console.log("Ctrl = TableEditorCtrl");
    //gets table names from route - user clicked table of choice
    $scope.tablename = $routeParams.tablename;
    console.log("Loading table: " + $scope.tablename);

    LoadTableContent();
    function LoadTableContent() {
        console.log("Load new table");

        var loadMethod = crudAJService.loadTableContent($scope.tablename);
        loadMethod.then(function (TableContent) {
            if (typeof TableContent.data.Item1 == "string") {
                TableContent.data.Item1 = JSON.parse(TableContent.data.Item1);
            }
            if (typeof TableContent.data.Item2 == "string") {
                TableContent.data.Item2 = JSON.parse(TableContent.data.Item2);
            }
            console.log("Headers: " + TableContent.data.Item1);
            console.log("Data: " + TableContent.data.Item2);

            for (var i = 0; i < Student.length; i++) {
                $scope.customColumns.push(
                    {
                        headerName: Student[i].Name,
                        field: "Mark",
                        headerClass: 'grid-halign-left'

                    }
                );

                $scope.gridOptions.columnDefs = $scope.customColumns;
                $scope.gridOptions.rowData = Student;
                $scope.gridOptions.api.setColumnDefs();

            }
        }, function () {
            alert('Error in getting table from database.');
        });



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
            //rowData: $scope.TableData,
            rowData: [
                { "StoredProcedure": "Grocast_SP_AccountPlanBuild", "RunLogID": "3ee152b7-13c4-41d9-ab38-4279b9090d82", "Locked": "false", "LockedDate": "null" },
                { "StoredProcedure": "Procast_SP_AccountPlanBuild", "RunLogID": "3ee152b7-13c4-41d9-ab38-4279b9090d82", "Locked": "false", "LockedDate": "null" },
                { "StoredProcedure": "Procast_SP_AccountPlanBuild", "RunLogID": "3ee152b7-13c4-41d9-ab38-4279b9090d82", "Locked": "false", "LockedDate": "null" },
                { "StoredProcedure": "Procast_SP_AccountPlanBuild", "RunLogID": "3ee152b7-13c4-41d9-ab38-4279b9090d82", "Locked": "false", "LockedDate": "null" }
            ],

            enableSorting: true,
            enableFilter: true,
            debug: true,

            rowSelection: 'multiple',
            enableColResize: true
        };
        new agGrid.Grid(gridDiv, gridOptions);


        //new agGrid.Grid(gridDiv, gridOptions);
        ////Init and fill agGrid
        //$scope.gridOptions.api.setRowData(jsonString);


        //http://stackoverflow.com/questions/31743534/angular-grid-ag-grid-columndefs-dynamically-change


    };
});

/* ----- Side Nav ----- */
app.controller('NavCtrl', function ($scope, $timeout, $mdSidenav) {
    $scope.toggleLeft = buildToggler('left');
    $scope.toggleRight = buildToggler('right');

    function buildToggler(componentId) {
        return function () {
            $mdSidenav(componentId).toggle();
        }
    }
});

/* ----- Side Nav ----- */
app.controller('SideNavCtrl', function ($scope, $timeout, $mdSidenav, $log) {
    $scope.toggleLeft = buildDelayedToggler('left');
    $scope.toggleRight = buildToggler('right');
    $scope.isOpenRight = function () {
        return ($mdSidenav('right').isOpen());
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

/* ----- Home Setup ----- */
app.controller('HomeSetupCtrl', function ($scope, $routeParams) {
    console.log("Ctrl = HomeSetupCtrl");
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

/* ----- Mode Setup ----- */
app.controller('ModeSetupCtrl', function ($scope) {
    console.log("Ctrl = ModeSetupCtrl");

    $scope.mode = {
        sourceServer: '',
        sourceName: '',
        sourcePassword: '',
        sourceDatabase: '',
        sourceConnString: '',

        targetServer: '',
        targetName: '',
        targetPassword: '',
        targetDatabase: '',
        targetConnString: ''
    };

    $scope.connectTarget = 'CONNECT';
    $scope.connectSource = 'CONNECT';

    $scope.clickConnectT = function (connectState) {
        console.log('connectStateT: ' + connectState);
        if (connectState == 'CONNECTED') {

        }
        else {
            //somehow put loading icon here
            $scope.connectTarget = 'CONNECTED';
        }
    };
    $scope.clickConnectS = function (connectState) {
        console.log('connectStateS: ' + connectState);
        if (connectState == 'CONNECTED') {

        }
        else {
            //somehow put loading icon here
            $scope.connectSource = 'CONNECTED';
        }
    };

    //do an on click, turns into gif until connected and then $scope.connect = connected
    //console.log("Connecting to DB")

    //write something that disables dependant on how you choose to input OR even better! disbaled but filled in upper bit with broken up string and vice versa

    //all this source target shit can be simplified into same code
    $scope.modeSourceFormIncomplete = function (ss, sn, sp, sd, sconstr) {
        //console.log("modeSourceFormIncomplete check completion");
        //if no connection string and complete other inputs
        if (sconstr == null && (ss && sn && sp && sd) != null) {
            //IS complete so return false
            return false;
        }
        else if (sconstr != null && (ss || sn || sp || sd) == null) {
            return false;
        }
        else {
            return true;
        }
    };

    $scope.modeTargetFormIncomplete = function (ts, tn, tp, td, tconstr) {
        //console.log("modeTargetFormIncomplete check completion");
        //if no connection string and complete other inputs
        if (tconstr == null && (ts && tn && tp && td) != null) {
            //IS complete so return false
            return false;
        }
        else if (tconstr != null && (ts || tn || tp || td) == null) {
            return false;
        }
        else {
            return true;
        }
    };

});

/* ----- Mode Continue ----- */
app.controller('ModeContinueCtrl', function ($scope, $mdDialog, $location) {
    $scope.confirmContinue = function () {
        // Appending dialog to document.body to cover sidenav in docs app
        var confirm = $mdDialog.confirm()
              .title('Are you sure you want to continue with:')
              .textContent('- Source Database: ' + $scope.mode.sourceDatabase + ' , - Target Database: ' + $scope.mode.targetDatabase + ' - Compare Differences: ')
              .ok('Continue')
              .cancel('Back');

        $mdDialog.show(confirm).then(function () {
            console.log('Continue to: table editor view .... ');
            //$location.url();
            $location.path('/Home/Tables');


            //url: '/tables';

        }, function () {
            console.log('Back to mode setup');
        });
    };
});

/* ----- Main ----- */
app.controller("GetTablesCtrl", function ($scope, crudAJService) {
    console.log("Ctrl = GetTablesCtrl");
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

/* ----- Test Table ----- */
app.controller("GetDataCtrl", function ($scope, crudAJService) {
    console.log("Ctrl = GetDataCtrl");
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

/* ----- Angular Routing ----- */
app.config(function ($routeProvider,
    $locationProvider) {
    $routeProvider
        .when("/", {
            templateUrl: function () { console.log("Route: HomeSetupView"); return "/Home/HomeSetup"; }
        })
        .when("/Index", {
            templateUrl: function () { console.log("Route: IndexView"); return "/Home/Index"; }
        })
        .when("/ModeSetup", {
            templateUrl: function () { console.log("Route: ModeSetupView"); return "/Home/ModeSetup"; }
        })
        .when("/Login", {
            templateUrl: function () { console.log("Route: Login"); return "Account/Login"; }
        })
        .when('/Table/:tablename', {
            templateUrl: function (params) { console.log("Route: Table/" + params.tablename); return "/Home/Table?tablename=" + params.tablename; },
            controller: 'TableCtrl'
            //controller: 'TableEditorCtrl'
        })
        .when("/ModeSetup/:process", {
            templateUrl: function (params) { console.log("Route: ModeSetupView/" + params.process); return "/Home/ModeSetup?process=" + params.process; },

        })
        .when("/Table", {
            templateUrl: function () { console.log("Route: Home/Table/"); return "/Home/Table" },
            //this should be for table editor..
        })
        .when("/About", {
            //not sure what this does
            templateUrl: function () { console.log("Route: Home/About/"); return "/Home/About ;" },
        })
        .otherwise({ redirectTo: '/Home/HomeSetup' });

});

