/* ----- ANGULAR CONTROLLERS ----- */

/* ----- agGrid CRUD ----- */
// --- THIS NEEDS TO BE SPLIT INTO LOTS OF DIFFERENT CONTROLS - WAY TOO MUCH HERE AT THE MOMENT
app.controller("TableCtrl", function ($scope, $routeParams, $timeout, $mdDialog, crudAJService) {
    console.log("TableCtrl, " + $routeParams.tablename);
    $scope.tablename = $routeParams.tablename;

    // --- INITIATE FLAGS
    //flags 
    $scope.loadingIsDone = false;
    $scope.associatedTablesExist = false;
    $scope.unsavedChanges = false;
    $scope.wizardMode = false;
    $scope.error = false;

    // --- CALLED BELOW
    //load data and init grid
    function LoadTableContent() {

        $scope.dataLoaded = false;
        $scope.isLoading = true;

        //get data from service
        var loadMethod = crudAJService.loadTableContent($scope.tablename);
        loadMethod.then(function (TableContent) {
            //check data present
            if (TableContent.data.dataArr == null) {
                $scope.data = null;
            }
            else {
                $scope.data = JSON.parse(TableContent.data.dataArr);
            }

            $scope.columnHeaders = JSON.parse(TableContent.data.headerArr);
            $scope.dataReturn = $scope.data;
            $scope.associatedTables = TableContent.data.fKeyTables;
            var typeRenderer = function (params) {
                $scope.columnHeaders.forEach(function (head) {
                    head.cellRenderer = function (params) {
                        switch (head.renderer) {
                            case "CheckBoxEditor":
                                return '<md-checkbox aria-label="addWhenBound" type="checkbox">'
                                return '<input type="checkbox">'
                                break;
                            case "pKey":
                                return '<span title="Primary Key"><i class="fa fa-key" aria-hidden="true"></i> &nbsp;' + params.value + '</span>'
                                break;
                            case "fkRenderer":
                                return '<span title="Primary Key"><i class="fa fa-key" aria-hidden="true"></i> &nbsp;' + params.value + '</span>'
                                break;
                            case "text":
                                return '<span>' + params.value + '</span>'
                                break;
                            case "largeText":
                                return '<span>' + params.value + '</span>'
                                break;
                            case "XmlEditor":
                                return '<span>' + params.value + '</span>'
                                break;
                            case "NumericCellEditor":
                                return '<span>' + params.value + '</span>'
                                break;
                            case "DateEditor":
                                return '<md-datepicker ng-model="params.input"></md-datepicker>'
                                break;
                        }
                    }

                });
            };

            //check for 'associated' tables
            if ($scope.associatedTables == 0) {
                $scope.associatedTablesExist = false;
            }
            else {
                $scope.associatedTablesExist = true;
            }

            //init gridOptions
            var gridOptions = {
                columnDefs: $scope.columnHeaders,
                rowData: $scope.data,
                angularCompileRows: true,
                singleClickEdit: false,
                rowHeight: 30,
                enableSorting: true,
                enableFilter: true,
                rowSelection: 'multiple',
                debug: true,
                enableColResize: true,
                cellRenderer: typeRenderer(),
            };

            //init grid
            $scope.gridOptions = gridOptions;

            $scope.dataLoaded = true;
            $scope.isLoading = false;
            $scope.loadingIsDone = true;

        }, function () {
            alert('Error in getting data');
            $scope.error = true;
            $scope.isLoading = false;
            $scope.unsavedChanges = false;
        });
    };

    // --- CALLS ABOVE METHOD TO LOAD ALL DATA 
    //call load
    LoadTableContent();

    // --- THESE ARE ALL 'ONLICK' FUNCTIONS, HENCE $SCOPE
    //local grid CRUD
    $scope.onRemoveSelected = function (record) {
        $scope.unsavedChanges = true;
        console.log("onRemoveSelected");
        $scope.selection = $scope.gridOptions.api.getSelectedNodes();
        //$scope.selection.rowStyle = { 'background-color': 'yellow' };
        $scope.gridOptions.api.removeItems($scope.selection);
    }

    var newCount = 1;
    $scope.onAddRow = function () {
        console.log("onAddRow");
        $scope.unsavedChanges = true;
        var newItem = tempNewRowData();
        $scope.gridOptions.api.insertItemsAtIndex(0, [newItem]);
    }

    function tempNewRowData() {
        console.log("Insert Record: " + $scope.dataReturn[0]); //this just copies last row of data instead of empty record..
        var newData = $scope.dataReturn[0];
        newCount++;
        return newData;
    }

    $scope.getRowData = function () {
        console.log("getRowData");
        var rowData = [];
        $scope.gridOptions.api.forEachNode(function (node) {
            rowData.push(node.data);
        });
        console.log('Row Data:');
        console.log(rowData);
    }

    //sizing
    $scope.sizeToFit = function () {
        console.log("sizeToFit");
        $scope.gridOptions.api.sizeColumnsToFit();
    }

    $scope.autoSizeAll = function () {
        console.log("autoSizeAll");
        var allColumnIds = [];
        $scope.columnHeaders.forEach(function (columnDef) {
            allColumnIds.push(columnDef.field);
        });
        $scope.gridOptions.columnApi.autoSizeColumns(allColumnIds);
    }

    //save/revert changes, push to DB, and write script
    $scope.onSave = function () {
        var confirm = $mdDialog.confirm()
             .title('Are you sure you want to save changes, this cannot be undone.')
             .ok('Save')
             .cancel('Cancel');
        $mdDialog.show(confirm).then(function () {
            console.log("Saving changes");
            $scope.unsavedChanges = false;
            pushToDB();

        }, function () {
            console.log('Cancel');
        });
    }

    function pushToDB() {
        console.log("Pushing changes to DB, write merge script..");
    };

    $scope.onRevert = function () {
        var confirm = $mdDialog.confirm()
             .title('Are you sure you want to undo changes.')
             .ok('Revert')
             .cancel('Cancel');
        $mdDialog.show(confirm).then(function () {
            console.log("Revert changes");
            //refresh data, remove unsaved changes
            $scope.gridOptions.api.setRowData($scope.data);
            $scope.unsavedChanges = false;
            //$scope.unsavedChanges = false;
        }, function () {
            console.log('Cancel');
        });
    }

    $scope.openCurrentScript = function () {
        console.log("Open current script");
        var confirm = $mdDialog.confirm()
            .title('Current change script.')
            .ok('Copy')
            .cancel('Cancel');
        $mdDialog.show(confirm).then(function () {
            console.log("Copying script");
        }, function () {
            console.log('Cancel');
        });
    }

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
            $location.path('/Table/SYS_Config');
            //url: '/tables';

        }, function () {
            console.log('Back to mode setup');
        });
    };
});

/* ----- Mode Compare ----- */

/* ----- Table List ----- */
app.controller("GetTablesCtrl", function ($scope, crudAJService) {
    console.log("Ctrl = GetTablesCtrl");
    //Loads all table names from the data base into navbar
    $scope.excludedTable = false;
    GetAllTableNames();
    function GetAllTableNames() {
        console.log("get table names");
        var getTableNameData = crudAJService.getTables();
        getTableNameData.then(function (Table) {
            $scope.TableList = Table.data;
            $scope.TableList.forEach(function(table) {
                if (table == app.SYS_Telemetry) {
                    $scope.excludedTable = true;
                }
                });
        }, function () {
            alert('Error in getting Table Names');
        });
    }

    //$scope.prevTable = function (current) {
    //    var n = tableN(current);
    //    console.log("n:" + n);
    //    console.log("going to prev table bbi" + $scope.TableList[n - 1]);
    //    $scope.prevTab = $scope.TableList[n - 1];
    //}
    //$scope.nextTable = function (current) {
    //    var n = tableN(current);
    //    console.log("going to next table bbi");
    //    $scope.nextTab = $scope.TableList[n + 1];
    //}

    //function tableN(current) {
    //    var tabNo = 0;
    //    $scope.TableList.forEach(function (table) {
    //        tabNo++;
    //        if (table == current) {
    //            $scope.n = tabNo;
    //        };
    //    });
    //    $scope.prevTab = $scope.TableList[$scope.n - 1];
    //    $scope.nextTab = $scope.TableList[$scope.n + 1];
    //    console.log($scope.prevTab);
    //    console.log(current);
    //    console.log($scope.nextTab);
    //}


    //data for wizard tree -- turn into .json file
    $scope.tree_data = [
        {
            Task: 'Master Data Setup',
            Done: ' ',

            children: [
                {
                    Task: 'Setup Sales Orgs and Customer Hierarchy / Levels',
                    Done: ' ',
                    children: [
                        {
                            Task: 'Review app.SYS_Config',
                            Done: ' '
                        }
                    ]
                },
            {
                Task: 'Setup Product Hierarchy / Levels',
                Done: ' ',
                children: [
                    {
                        Task: 'Review setup.ETL_UK_Load_Products',
                        Done: ' '
                    }
                ]
            },
            {
                Task: 'Setup Measures and Attributes',
                Done: ' ',
                children: [
                    {
                        Task: 'app.Dim_Product_Cust_Measures',
                        Done: ' '
                    },
                    {
                        Task: 'app.Dim_Product_Sku_Measures',
                        Done: ' '
                    },
                    {
                        Task: 'app.Dim_Product_Sku_Cust_Measures',
                        Done: ' '
                    },
                    {
                        Task: 'app.Dim_Product_Cust_Attributes',
                        Done: ' '
                    },
                    {
                        Task: 'app.Dim_Product_Sku_Attributes',
                        Done: ' '
                    },
                    {
                        Task: 'app.Dim_Product_Sku_Cust_Attribute',
                        Done: ' '
                    }
                ]
            },
            ]
        },
        {
            Task: 'General Setup',
            Done: ' ',

            children: [
                {
                    Task: 'Set Base Unit Of Measure',
                    Done: ' ',
                    children: [
                        {
                            Task: 'app.SYS_Config',
                            Done: ' '
                        }
                    ]
                },
                {
                    Task: 'Set Deleting Policy',
                    Done: ' ',
                    children: [
                        {
                            Task: 'app.SYS_Config',
                            Done: ' '
                        }
                    ]
                },
                {
                    Task: 'Set Password Policy',
                    Done: ' ',
                    children: [
                        {
                            Task: 'app.SYS_Config',
                            Done: ' '
                        }
                    ]
                },
                {
                    Task: 'Config client Calendar View',
                    Done: ' ',
                    children: [
                        {
                            Task: 'clnt.vw_Dim_Calendar',
                            Done: ' '
                        }
                    ]
                },
                {
                    Task: 'Review References to “Demo”',
                    Done: ' ',
                    children: [
                        {
                            Task: 'app.SYS_Config',
                            Done: ' '
                        }
                    ]
                },
                {
                    Task: 'Remove any screens and tabs not in scope',
                    Done: ' ',
                    children: [
                        {
                            Task: 'app.SYS_Screens',
                            Done: ' '
                        },
                        {
                            Task: 'app.SYS_ScreenTabs',
                            Done: ' '
                        },
                        {
                            Task: 'app.Fact_Screen_ScreenGroup',
                            Done: ' '
                        }
                    ]
                },
                {
                    Task: 'Update languages settings for any renaming of screens, tabs and controls',
                    Done: ' ',
                    children: [
                        {
                            Task: 'app.Dim_Language_AppLabels',
                            Done: ' '
                        }
                    ]
                },

            ]
        },
        {
            Task: 'Planning Screen Configuration',
            Done: ' ',
            children: [
               {
                   Task: 'Setup Master Data',
                   Done: ' ',
                   children: [
                       {
                           Task: 'Review and modify as appropriate app.Dim_Planning_Volume_MeasureGroups',
                           Done: ' '
                       },
                       {
                           Task: 'Review and modify as appropriate app.Dim_Planning_Volume_Measures',
                           Done: ' '
                       },
                       {
                           Task: 'Review and modify as appropriate app.Dim_Planning_Time_Range',
                           Done: ' '
                       },
                       {
                           Task: 'Review and modify as appropriate app.Dim_Planning_Time_Levels',
                           Done: ' '
                       }
                   ]
               },
               {
                   Task: 'Test',
                   Done: ' ',
                   children: [
                       {
                           Task: 'Check volume saves and reloads, and unit of measure correctly set',
                           Done: ' '
                       }
                   ]
               }
            ]
        },
        {
            Task: 'Promotions Configuration',
            Done: ' ',
        },
        {
            Task: 'Terms Configuration',
            Done: ' ',
        },
        {
            Task: 'Management Adjustment Configuration',
            Done: ' ',

        },
        {
            Task: 'Risk And Ops Configuration',
            Done: ' ',

        },
        {
            Task: 'Funds',
            Done: ' ',

        }
    ];

    //if cell name is done renderer = checkbox
    //save to local app database? along with projects and progress etc

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