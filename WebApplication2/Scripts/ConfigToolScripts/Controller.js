/* ----- ANGULAR CONTROLLERS ----- */

/* ----- agGrid CRUD ----- */
//THIS NEEDS TO BE SPLIT INTO LOTS OF DIFFERENT CONTROLS - WAY TOO MUCH HERE AT THE MOMENT
app.controller("TableCtrl", function ($scope, $routeParams, $timeout, $mdDialog, crudAJService) {
    console.log("TableCtrl, " + $routeParams.tablename);
    $scope.tablename = $routeParams.tablename;

    $scope.loadingIsDone = false;
    $scope.associatedTablesExist = false;
    $scope.unsavedChanges = true;

    function LoadTableContent() {
        $scope.dataLoaded = false;
        $scope.isLoading = true;

        //call service
        var loadMethod = crudAJService.loadTableContent($scope.tablename);
        loadMethod.then(function (TableContent) {

            //check data present
            if (TableContent.data.dataArr == null) {
                var data = null;
            }
            else {
                var data = JSON.parse(TableContent.data.dataArr);

            }

            $scope.columnHeaders = JSON.parse(TableContent.data.headerArr);
            $scope.dataReturn = data;
            $scope.associatedTables = TableContent.data.fKeyTables;

            //check for associated tables
            if ($scope.associatedTables == 0) {
                $scope.associatedTablesExist = false;
            }
            else {
                $scope.associatedTablesExist = true;
            }

            var gridOptions = {
                columnDefs: $scope.columnHeaders,
                rowData: data,
                singleClickEdit: false,
                rowHeight: 35,
                enableSorting: true,
                enableFilter: true,
                rowSelection: 'multiple',
                debug: true,
                enableColResize: true,
            };

            $scope.gridOptions = gridOptions;

            $scope.dataLoaded = true;
            $scope.isLoading = false;
            $scope.loadingIsDone = true;

        }, function () {
            alert('Error in getting data');
            $scope.isLoading = false;
        });
    };

    //cell editors

    function DateEditor() { console.log("Numeric Cell Editor"); }
    function LargeTextEditor() { console.log("Numeric Cell Editor"); }
    function TextEditor() { console.log("Numeric Cell Editor"); }
    function XmlEditor() { console.log("Numeric Cell Editor"); }
    function CheckBoxEditor() { console.log("Numeric Cell Editor"); }

    // function to act as a class
    function NumericCellEditor() { console.log("Numeric Cell Editor"); }

    // gets called once before the renderer is used
    NumericCellEditor.prototype.init = function (params) {
        // create the cell
        this.eInput = document.createElement('input');
        this.eInput.value = isCharNumeric(params.charPress) ? params.charPress : params.value;

        var that = this;
        this.eInput.addEventListener('keypress', function (event) {
            if (!isKeyPressedNumeric(event)) {
                that.eInput.focus();
                if (event.preventDefault) event.preventDefault();
            }
        });

        // only start edit if key pressed is a number, not a letter
        var charPressIsNotANumber = params.charPress && ('1234567890'.indexOf(params.charPress) < 0);
        this.cancelBeforeStart = charPressIsNotANumber;
    };

    // gets called once when grid ready to insert the element
    NumericCellEditor.prototype.getGui = function () {
        return this.eInput;
    };

    // focus and select can be done after the gui is attached
    NumericCellEditor.prototype.afterGuiAttached = function () {
        this.eInput.focus();
    };

    // returns the new value after editing
    NumericCellEditor.prototype.isCancelBeforeStart = function () {
        return this.cancelBeforeStart;
    };

    // example - will reject the number if it contains the value 007
    // - not very practical, but demonstrates the method.
    NumericCellEditor.prototype.isCancelAfterEnd = function () {
        var value = this.getValue();
        return value.indexOf('007') >= 0;
    };

    // returns the new value after editing
    NumericCellEditor.prototype.getValue = function () {
        return this.eInput.value;
    };

    // any cleanup we need to be done here
    NumericCellEditor.prototype.destroy = function () {
        // but this example is simple, no cleanup, we could  even leave this method out as it's optional
    };

    // if true, then this editor will appear in a popup 
    NumericCellEditor.prototype.isPopup = function () {
        // and we could leave this method out also, false is the default
        return false;
    };

    LoadTableContent();

    //CRUD..

    $scope.onRemoveSelected = function (record) {
        $scope.unsavedChanges = true;
        console.log("onRemoveSelected");
        $scope.selection = $scope.gridOptions.api.getSelectedNodes();
        $scope.gridOptions.api.removeItems($scope.selection);
        //console.log(JSON.stringify($scope.selection[0].data)); //need to do this for more than one if multi row delete
        //var getData = crudAJService.DeleteRecord(JSON.stringify($scope.selection[0].data)); //pass record back to service
        //getData.then(function (msg) {
        //    alert(msg.data);
        //    LoadTableContent();
        //}, function () {
        //    alert('Error in deleting record');
        //});
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

    //save changes push to DB and write script
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

    function pushToDB() {
        console.log("Pushing changes to DB, write merge script..");
    };


});

/* ----- Open Script ----- */

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

    //data for wizard tree -- turn into .json file
    $scope.tree_data = [
        {
            Task: 'Master Data Setup',
            Script: ' ',

            children: [
                {
                    Task: 'Setup Sales Orgs and Customer Hierarchy / Levels',
                    Script: ' ',
                    children: [
                        {
                            Task: 'Review @Html.ActionLink("SYS_Config", "Index", "Home")',
                            Script: ' '
                        }
                    ]
                },
            {
                Task: 'Setup Product Hierarchy / Levels',
                Script: ' ',
                children: [
                    {
                        Task: 'Review setup.ETL_UK_Load_Products',
                        Script: ' '
                    }
                ]
            },
            {
                Task: 'Setup Measures and Attributes',
                Script: ' ',
                children: [
                    {
                        Task: 'app.Dim_Product_Cust_Measures',
                        Script: ' '
                    },
                    {
                        Task: 'app.Dim_Product_Sku_Measures',
                        Script: ' '
                    },
                    {
                        Task: 'app.Dim_Product_Sku_Cust_Measures',
                        Script: ' '
                    },
                    {
                        Task: 'app.Dim_Product_Cust_Attributes',
                        Script: ' '
                    },
                    {
                        Task: 'app.Dim_Product_Sku_Attributes',
                        Script: ' '
                    },
                    {
                        Task: 'app.Dim_Product_Sku_Cust_Attribute',
                        Script: ' '
                    }
                ]
            },
            ]
        },
        {
            Task: 'General Setup',
            Script: ' ',

            children: [
                {
                    Task: 'Set Base Unit Of Measure',
                    Script: ' ',
                    children: [
                        {
                            Task: 'app.SYS_Config',
                            Script: ' '
                        }
                    ]
                },
                {
                    Task: 'Set Deleting Policy',
                    Script: ' ',
                    children: [
                        {
                            Task: 'app.SYS_Config',
                            Script: ' '
                        }
                    ]
                },
                {
                    Task: 'Set Password Policy',
                    Script: ' ',
                    children: [
                        {
                            Task: 'app.SYS_Config',
                            Script: ' '
                        }
                    ]
                },
                {
                    Task: 'Config client Calendar View',
                    Script: ' ',
                    children: [
                        {
                            Task: 'clnt.vw_Dim_Calendar',
                            Script: ' '
                        }
                    ]
                },
                {
                    Task: 'Review References to “Demo”',
                    Script: ' ',
                    children: [
                        {
                            Task: 'app.SYS_Config',
                            Script: ' '
                        }
                    ]
                },
                {
                    Task: 'Remove any screens and tabs not in scope',
                    Script: ' ',
                    children: [
                        {
                            Task: 'app.SYS_Screens',
                            Script: ' '
                        },
                        {
                            Task: 'app.SYS_ScreenTabs',
                            Script: ' '
                        },
                        {
                            Task: 'app.Fact_Screen_ScreenGroup',
                            Script: ' '
                        }
                    ]
                },
                {
                    Task: 'Update languages settings for any renaming of screens, tabs and controls',
                    Script: ' ',
                    children: [
                        {
                            Task: 'app.Dim_Language_AppLabels',
                            Script: ' '
                        }
                    ]
                },

            ]
        },
        {
            Task: 'Planning Screen Configuration',
            Script: ' ',
            children: [
               {
                   Task: 'Setup Master Data',
                   Script: ' ',
                   children: [
                       {
                           Task: 'Review and modify as appropriate app.Dim_Planning_Volume_MeasureGroups',
                           Script: ' '
                       },
                       {
                           Task: 'Review and modify as appropriate app.Dim_Planning_Volume_Measures',
                           Script: ' '
                       },
                       {
                           Task: 'Review and modify as appropriate app.Dim_Planning_Time_Range',
                           Script: ' '
                       },
                       {
                           Task: 'Review and modify as appropriate app.Dim_Planning_Time_Levels',
                           Script: ' '
                       }
                   ]
               },
               {
                   Task: 'Test',
                   Script: ' ',
                   children: [
                       {
                           Task: 'Check volume saves and reloads, and unit of measure correctly set',
                           Script: ' '
                       }
                   ]
               }
            ]
        },
        {
            Task: 'Promotions Configuration',
            Script: ' ',
        },
        {
            Task: 'Terms Configuration',
            Script: ' ',
        },
        {
            Task: 'Management Adjustment Configuration',
            Script: ' ',

        },
        {
            Task: 'Risk And Ops Configuration',
            Script: ' ',

        },
        {
            Task: 'Funds',
            Script: ' ',

        }
    ];
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