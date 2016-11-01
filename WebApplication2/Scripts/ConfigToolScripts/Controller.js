/* ----- ANGULAR CONTROLLERS ----- */
// need to split these up into controllers

/* ----- agGrid CRUD ----- */
// --- THIS NEEDS TO BE SPLIT INTO LOTS OF DIFFERENT CONTROLS - WAY TOO MUCH HERE AT THE MOMENT
app.controller("TableCtrl", function ($scope, $routeParams, $timeout, $mdDialog, crudAJService, sharedService) {
    console.log("TableCtrl, " + $routeParams.tablename);
    $scope.tablename = $routeParams.tablename;

    //crud array init
    var crudArray = new Array();

    // --- INITIATE FLAGS
    //flags 
    $scope.loadingIsDone = false;
    $scope.associatedTablesExist = false;
    $scope.showAssociatedTables = false;
    $scope.unsavedChanges = false;
    $scope.wizardMode = true;
    $scope.error = false;
    if ($scope.wizardMode) {
        $scope.task = sharedService.getTask();
    }
    console.log($scope.task);

    //load data and init grid (called below)
    function LoadTableContent() {

        $scope.dataLoaded = false;
        $scope.isLoading = true;

        //get data from service
        var loadMethod = crudAJService.loadTableContent($scope.tablename);
        loadMethod.then(function (TableContent) {

            //check data present
            if (TableContent.data.dataArr == null) {
                if (TableContent.data.dataStringArr != null) {
                    console.log(TableContent.data.dataStringArr);
                    $scope.data = TableContent.data.dataStringArr;
                }
                else { $scope.data = null; }
            }
            else {
                $scope.data = JSON.parse(TableContent.data.dataArr);
            }

            $scope.columnHeaders = JSON.parse(TableContent.data.headerArr);
            $scope.tempRow = JSON.parse(TableContent.data.emptyRow);
            $scope.dataReturn = $scope.data;
            $scope.associatedTables = TableContent.data.fKeyTables;

            //check for 'associated' tables
            if ($scope.associatedTables == 0) {
                $scope.associatedTablesExist = false;
            }
            else {
                $scope.associatedTablesExist = true;
            }

            //setup ag-Grid
            (function () {
                var rowData = $scope.data;
                var columnDefs = $scope.columnHeaders;

                function getStyle(params) {
                    $scope.columnHeaders.forEach(function (head) {
                        head.cellStyle = function (params) {
                            if (head.type == "numeric") {
                                head.cellEditor = numericEditor;
                                return { 'text-align': 'right', 'padding-right': '5px' };
                            }
                            else if (head.type == "bool") {
                                //head.cellRenderer = checkBoxRenderer;
                                head.cellEditor = 'select';
                                head.cellEditorParams = { values: ['true', 'false'] };
                                return { 'text-align': 'center' };
                            }
                            else if (head.type == "pKey") {
                                head.cellRenderer = pKeyRenderer;
                                return { 'padding-left': '5px', 'font-weight': 'bold' };
                            }
                            else if (head.type == "fKey") {
                                head.cellRenderer = pKeyRenderer;
                                head.cellEditor = 'select';
                                var table = $scope.associatedTables[0];
                                //MASSIVE ISSUE THAT ONLY WORKS IF ONE FOREIGH KEY TABLE AT THE MOMENT
                                head.cellEditorParams = JSON.parse(table.fKeyData);
                                return { 'padding-left': '5px' };
                            }
                            else if (head.type == "date") {
                                head.cellRenderer = dateRenderer;
                                return { 'padding-left': '5px' };
                            }
                            else if (head.type == "changes") {
                                head.cellRenderer = changesRenderer;
                                return { 'text-align': 'center', 'background-color': 'rgba(0, 0, 0, 0.05)' };
                            }
                            else return { 'padding-left': '5px' };
                        }
                    })
                };

                //NUMERIC EDITOR
                var numericEditor = function (params) { };
                function getCharCodeFromEvent(event) {
                    event = event || window.event;
                    return (typeof event.which == "undefined") ? event.keyCode : event.which;
                }
                function isCharNumeric(charStr) {
                    return !!/\d/.test(charStr);
                }
                function isKeyPressedNumeric(event) {
                    var charCode = getCharCodeFromEvent(event);
                    var charStr = String.fromCharCode(charCode);
                    return isCharNumeric(charStr);
                }
                // gets called once before the renderer is used
                numericEditor.prototype.init = function (params) {
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
                numericEditor.prototype.getGui = function () {
                    return this.eInput;
                };
                // focus and select can be done after the gui is attached
                numericEditor.prototype.afterGuiAttached = function () {
                    this.eInput.focus();
                };
                // returns the new value after editing
                numericEditor.prototype.isCancelBeforeStart = function () {
                    return this.cancelBeforeStart;
                };
                numericEditor.prototype.getValue = function () {
                    return this.eInput.value;
                };

                //CELL RENDERERS
                var pKeyRenderer = function (params) {
                    return '<span title="Primary Key"><i class="fa fa-key" aria-hidden="true"></i> &nbsp;' + params.value + '</span>';
                };

                var checkBoxRenderer = function (params) {
                    //return '<div style="text-align:center;"><md-checkbox ng-model="params.value" aria-label="addWhenBound" type="checkbox"><div>';
                    //return '<input type="checkbox">';
                };

                var dateRenderer = function (params) {
                    var datetime = params.value.split("T");
                    var date = datetime[0];
                    console.log("date renderer");
                    console.log(date);
                    return '<md-datepicker ng-model="' + date + '"></md-datepicker>';
                    return '<input type="date">';
                }

                var changesRenderer = function (params) {
                    if (params.value == null || params.value == "") { return '<span>0<span>'; }
                    else { return '<span class="rag-element">' + params.value + '<span>' };
                };

                //init gridOptions
                var gridOptions = {
                    columnDefs: $scope.columnHeaders,
                    rowData: $scope.data,
                    angularCompileRows: true,
                    singleClickEdit: true,
                    editType: 'fullRow',
                    cellStyle: getStyle(),
                    //cellClassRules: {
                    //    //
                    //    'rag-green-outer': function (params) { event.data['hasChanges'] === 1 },
                    //    'rag-red-outer': function (params) { event.data['hasChanges'] === 2 }
                    //},
                    onCellValueChanged: function (event) {
                        $scope.unsaved = true;
                        //this isnt working currently but should be soon (cahnge colour of edited field)
                        event.colDef.field.cellStyle = { 'background-color': 'green' };
                        var updateString = 'update:' + event.colDef.field + ' from ' + event.oldValue + ' to ' + event.newValue;
                        crudArray.push(updateString);
                        console.log('onCellValueChanged: ' + event.colDef.field + ' from ' + event.oldValue + ' to ' + event.newValue);
                        if (event.oldValue != event.newValue) {
                            event.data['hasChanges'] = 1;
                            gridOptions.api.refreshView();
                        };
                    },
                    onGridReady: function (event) {
                        event.api.sizeColumnsToFit();
                    },
                    rowHeight: 30,
                    enableSorting: true,
                    enableFilter: true,
                    rowSelection: 'multiple',
                    debug: true,
                    enableColResize: true,

                };

                function getRowData() {
                    var rowData = [];
                    gridOptions.api.forEachNode(function (node) {
                        rowData.push(node.data);
                    });
                    console.log('Row Data:');
                    console.log(rowData);
                };

                $scope.gridOptions = gridOptions;

                $scope.onRemoveSelected = function (params) {
                    $scope.unsavedChanges = true;
                    var selection = $scope.gridOptions.api.getSelectedNodes();
                    //change this to turn rows red when provisionally deleted
                    //$scope.gridOptions.api.removeItems(selection);
                    selection.forEach(function (node) {
                        console.log("node.data:" + node.data['hasChanges']);
                        node.data['hasChanges'] = 2;
                        gridOptions.api.refreshView();
                        crudArray.push("delete:" + JSON.stringify(node.data));
                    });
                    
                };



                $scope.onSave = function () {
                    var confirm = $mdDialog.confirm()
                         .title('Are you sure you want to save changes, this cannot be undone.')
                         .ok('Save')
                         .cancel('Cancel');
                    $mdDialog.show(confirm).then(function () {
                        //crudArray could be used for audit - but more likely newValueHandler..
                        console.log("Changes" + crudArray);
                        var newData = getRowData();
                        $scope.unsavedChanges = false;
                    }, function () {
                        console.log('Cancel');
                    });
                };

                $scope.onRevert = function () {
                    var confirm = $mdDialog.confirm()
                         .title('Are you sure you want to undo changes.')
                         .ok('Revert')
                         .cancel('Cancel');
                    $mdDialog.show(confirm).then(function () {
                        console.log("Revert changes");
                        //refresh data, remove unsaved changes
                        $scope.gridOptions.api.refreshView();
                        $scope.gridOptions.api.setRowData($scope.data);
                        $scope.unsavedChanges = false;
                        //$scope.unsavedChanges = false;
                    }, function () {
                        console.log('Cancel');
                    });
                };
                //$scope.gridOptions.api.refreshView();


            })();

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

    //call load
    LoadTableContent();


    //these are 'onclick' functions
    //local grid CRUD


    var newCount = 0;
    $scope.onAddRow = function () {
        console.log("onAddRow");
        $scope.unsavedChanges = true;
        var newItem = tempNewRowData();
        $scope.gridOptions.api.insertItemsAtIndex(0, [newItem]);
    }
    function tempNewRowData() {
        var newRow = $scope.tempRow;
        newCount++;
        return newRow;
    }

    $scope.openAssociatedTables = function () {
        if (!$scope.showAssociatedTables) {
            $scope.showAssociatedTables = true;
        }
        else if ($scope.showAssociatedTables) {
            $scope.showAssociatedTables = false;
        }
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
app.controller("GetTablesCtrl", function ($scope, crudAJService, sharedService, $log) {
    console.log("Ctrl = GetTablesCtrl");
    //Loads all table names from the data base into navbar
    $scope.excludedTable = false;
    GetAllTableNames();
    function GetAllTableNames() {
        console.log("get table names");
        var getTableNameData = crudAJService.getTables();
        getTableNameData.then(function (Table) {
            $scope.TableList = Table.data;
            $scope.TableList.forEach(function (table) {
                if (table == app.SYS_Telemetry) {
                    $scope.excludedTable = true;
                }
            });
        }, function () {
            alert('Error in getting Table Names');
        });
    }

    (function () {
        //-- into .json file
        var rowData = [
            {
                folder: true,
                open: true,
                name: 'Master Data Setup',
                done: 'false',
                children: [
                    {
                        folder: true,
                        open: true,
                        name: 'Setup Sales Orgs and Customer Hierarchy / Levels',
                        done: 'false',
                        children: [
                            {
                                name: 'Review app.SYS_Config',
                                table: 'app.SYS_Config',
                                dateChanged: '25/10/2016',
                                done: 'false'
                            }
                        ]
                    },
                    {
                        folder: true,
                        name: 'Setup Product Hierarchy / Levels',
                        done: 'false',
                        open: true,
                        children: [
                            {
                                name: 'Review setup.ETL_UK_Load_Products',
                                table: 'setup.ETL_UK_Load_Products',
                                dateChanged: '25/10/2016',
                                done: 'false'
                            }
                        ]
                    },
                    {
                        folder: true,
                        open: true,
                        name: 'Setup Measures and Attributes',
                        done: 'false',
                        children: [
                            {
                                name: 'Setup app.Dim_Product_Cust_Measures',
                                table: 'app.Dim_Product_Cust_Measures',
                                dateChanged: '25/10/2016',
                                done: 'false'
                            },
                            {
                                name: 'Setup app.Dim_Product_Sku_Measures',
                                table: 'app.Dim_Product_Sku_Measures',
                                dateChanged: '25/10/2016',
                                done: 'false'
                            },
                            {
                                name: 'Setup app.Dim_Product_Sku_Cust_Measures',
                                table: 'app.Dim_Product_Sku_Cust_Measures',
                                dateChanged: '25/10/2016',
                                done: 'false'
                            },
                            {
                                name: 'Setup app.Dim_Product_Cust_Attributes',
                                table: 'app.Dim_Product_Cust_Attributes',
                                dateChanged: '25/10/2016',
                                done: 'false'
                            },
                            {
                                name: 'Setup app.Dim_Product_Sku_Attributes',
                                table: 'app.Dim_Product_Sku_Attributes',
                                dateChanged: '25/10/2016',
                                done: 'false'
                            },
                            {
                                name: 'Setup app.Dim_Product_Sku_Cust_Attribute',
                                table: 'app.Dim_Product_Sku_Cust_Attribute',
                                dateChanged: '25/10/2016',
                                done: 'false'
                            }
                        ]
                    },
                ]
            },
            {
                folder: true,
                open: true,
                name: 'General Setup',
                done: 'false',
                children: [
                    {
                        name: 'Set Base Unit Of Measure',
                        table: 'app.SYS_Config',
                        dateChanged: '25/10/2016',
                        done: 'false'
                    },
                    {
                        name: 'Set Deleting Policy',
                        table: 'app.SYS_Config',
                        dateChanged: '25/10/2016',
                        done: 'false'
                    },
                    {
                        name: 'Set Password Policy',
                        table: 'app.SYS_Config',
                        dateChanged: '25/10/2016',
                        done: 'false'
                    },
                    {
                        name: 'Config client Calendar View',
                        table: 'clnt.vw_Dim_Calendar',
                        dateChanged: '25/10/2016',
                        done: 'false'
                    },
                    {
                        name: 'Review References to “Demo”',
                        table: 'app.SYS_Config',
                        dateChanged: '25/10/2016',
                        done: 'false'
                    },
                    {
                        folder: true,
                        name: 'Remove any screens and tabs not in scope',
                        done: 'false',
                        children: [
                            {
                                name: 'Remove Screens',
                                table: 'app.SYS_Screens',
                                dateChanged: '25/10/2016',
                                done: 'false'
                            },
                            {
                                name: 'Remove ScreenTabs',
                                table: 'app.SYS_ScreenTabs',
                                dateChanged: '25/10/2016',
                                done: 'false'
                            },
                            {
                                name: 'Remove ScreenGroups',
                                table: 'app.Fact_Screen_ScreenGroup',
                                dateChanged: '25/10/2016',
                                done: 'false'
                            }
                        ]
                    },
                    {
                        name: 'Update languages settings for any renaming of screens, tabs and controls',
                        table: 'app.Dim_Language_AppLabels',
                        dateChanged: '25/10/2016',
                        done: 'false'
                    },

                ]
            },
            {

                folder: true,
                name: 'Planning Screen Configuration',
                done: 'false',
                children: [
                   {
                       folder: true,
                       name: 'Setup Master Data',
                       done: 'false',
                       children: [
                           {
                               name: 'Review and modify as appropriate',
                               table: 'app.Dim_Planning_Volume_MeasureGroups',
                               dateChanged: '25/10/2016',
                               done: 'false'
                           },
                           {
                               name: 'Review and modify as appropriate',
                               table: 'app.Dim_Planning_Volume_Measures',
                               dateChanged: '25/10/2016',
                               done: 'false'
                           },
                           {
                               name: 'Review and modify as appropriate',
                               table: 'app.Dim_Planning_Time_Range',
                               dateChanged: '25/10/2016',
                               done: 'false'
                           },
                           {
                               name: 'Review and modify as appropriate',
                               table: 'app.Dim_Planning_Time_Levels',
                               dateChanged: '25/10/2016',
                               done: 'false'
                           }
                       ]
                   },
                   {
                       folder: true,
                       name: 'Test',
                       done: 'false',
                       children: [
                           {
                               name: 'Check volume saves and reloads, and unit of measure correctly set',
                               dateChanged: '25/10/2016',
                               done: 'false'
                           }
                       ]
                   }
                ]
            },
             {
                 folder: true,
                 name: 'Promotions Configuration',
                 done: 'false',
                 children: [
                     {
                         folder: true,
                         name: 'Overall Promotions Behaviour',
                         done: 'false',
                         children: [
                                     {
                                         folder: true,
                                         name: 'Review Sys Config Settings',
                                         done: 'false',
                                         children: [
                                             {
                                                 name: 'Phasing',
                                                 table: 'app.SYS_Config',
                                                 dateChanged: '25/10/2016',
                                                 done: 'false'
                                             },
                                             {
                                                 name: 'PostPromo',
                                                 table: 'app.SYS_Config',
                                                 dateChanged: '25/10/2016',
                                                 done: 'false'
                                             },
                                             {
                                                 name: 'PromotionScenario',
                                                 table: 'app.SYS_Config',
                                                 dateChanged: '25/10/2016',
                                                 done: 'false'
                                             },
                                             {
                                                 name: 'EnablePromotionProductsReferenceData',
                                                 table: 'app.SYS_Config',
                                                 dateChanged: '25/10/2016',
                                                 done: 'false'
                                             },
                                             {
                                                 name: 'EnableTemplateConstraints',
                                                 table: 'app.SYS_Config',
                                                 dateChanged: '25/10/2016',
                                                 done: 'false'
                                             },
                                             {
                                                 name: 'Scenario_IsExportActive',
                                                 table: 'app.SYS_Config',
                                                 dateChanged: '25/10/2016',
                                                 done: 'false'
                                             },
                                             {
                                                 name: 'PromoPowerEditor',
                                                 table: 'app.SYS_Config',
                                                 dateChanged: '25/10/2016',
                                                 done: 'false'
                                             },
                                             {
                                                 name: 'Promotion_FileSelector',
                                                 table: 'app.SYS_Config',
                                                 dateChanged: '25/10/2016',
                                                 done: 'false'
                                             },
                                             {
                                                 name: 'ListingsProductSelectedByDefault CanEditPromoFinancialScreenProductData',
                                                 table: 'app.SYS_Config',
                                                 dateChanged: '25/10/2016',
                                                 done: 'false'
                                             },
                                             {
                                                 name: 'Phasing.Daily',
                                                 table: 'app.SYS_Config',
                                                 dateChanged: '25/10/2016',
                                                 done: 'false'
                                             },
                                             {
                                                 name: 'TriggerPromoOrProduct',
                                                 table: 'app.SYS_Config',
                                                 dateChanged: '25/10/2016',
                                                 done: 'false'
                                             },
                                             {
                                                 name: 'TriggerUnitOrDeal',
                                                 table: 'app.SYS_Config',
                                                 dateChanged: '25/10/2016',
                                                 done: 'false'
                                             },
                                             {
                                                 name: 'PromoPhasing_PostPromo',
                                                 table: 'app.SYS_Config',
                                                 dateChanged: '25/10/2016',
                                                 done: 'false'
                                             },
                                             {
                                                 name: 'AllowOverlappingPromotions',
                                                 table: 'app.SYS_Config',
                                                 dateChanged: '25/10/2016',
                                                 done: 'false'
                                             },
                                             {
                                                 name: 'PostPromoNumDays',
                                                 table: 'app.SYS_Config',
                                                 dateChanged: '25/10/2016',
                                                 done: 'false'
                                             },
                                             {
                                                 name: 'PromoUseRRpc',
                                                 table: 'app.SYS_Config',
                                                 dateChanged: '25/10/2016',
                                                 done: 'false'
                                             },

                                         ]
                                     }
                         ]
                     },
               {
                   folder: true,
                   open: true,
                   name: 'Terms Configuration',
                   done: 'false',
               },
               {
                   folder: true,
                   open: true,
                   name: 'Management Adjustment Configuration',
                   done: 'false',

               },
               {
                   folder: true,
                   open: true,
                   name: 'Risk And Ops Configuration',
                   done: 'false',

               },
               {
                   folder: true,
                   open: true,
                   name: 'Funds',
                   done: 'false',

               }
                 ]
             }
        ];

        var columnDefs = [
            {
                headerName: "Task", field: "name", cellRenderer: 'group', width: 350, cellStyle: infoStyle,
                cellRendererParams: {
                    innerRenderer: innerCellRenderer
                }
            },
            { headerName: "Go To Table", field: "table", cellRenderer: tableLink, width: 250 },
            { headerName: "Complete", field: "done", cellRenderer: checkBoxRenderer, width: 60, editable: true },
            { headerName: "Date Modified", field: "dateChanged", cellStyle: centerStyle, width: 100 }
        ];

        var gridOptions = {
            columnDefs: columnDefs,
            rowData: rowData,
            rowSelection: 'multiple',
            rowHeight: 35,
            //getNodeChildDetails: function (task) {
            //    if (task.folder) {
            //        return {
            //            group: true,
            //            children: task.children,
            //            expanded: task.open
            //        };
            //    } else {
            //        return null;
            //    }
            //},
            icons: {
                groupExpanded: '<i class="fa fa-minus-square-o"/>',
                groupContracted: '<i class="fa fa-plus-square-o"/>'
            },
            onRowClicked: rowClicked

        };

        function expandAll(expand) {
            var columnApi = gridOptions.columnApi;
            var groupNames = ['GroupA', 'GroupB', 'GroupC', 'GroupD', 'GroupE', 'GroupF', 'GroupG'];

            groupNames.forEach(function (groupId) {
                columnApi.setColumnGroupOpened(groupId, expand);
            });
        }

        function rowClicked(params) {
            sharedService.setTask(params.data.name);
            var node = params.node;
            var path = node.data.name;
            while (node.parent) {
                node = node.parent;
                path = node.data.name + '\\' + path;

            }
            //document.querySelector('#selectedFile').innerHTML = path;
        }

        $scope.parents = rowData.data;

        function centerStyle() {
            return { 'text-align': 'center' };
        }

        function infoStyle() {
            return { 'white-space': 'normal' };
        }

        function innerCellRenderer(params) {
            var image;
            if (params.node.group) {
                image = params.node.level === 0 ? '<i class="fa fa-tasks" aria-hidden="true"></i> &nbsp;&nbsp;' : ' <i class="fa fa-cubes" aria-hidden="true"></i> &nbsp;&nbsp;';
            } else {
                image = "<i  style='color:#696969'class='fa fa-arrow-right' aria-hidden='true'></i> &nbsp;&nbsp;";
            }
            return image + params.data.name;
        }

        function tableExists(table) {
            console.log("check table exists in connected database");
            return false;
        }

        function tableLink(params) {
            if (params.data.table != null) {

                //console.log(params.data.name + params.data);
                //return "<div  style='color:#0000FF'> "+ params.data.table+"</div>";
                return "&nbsp; <a style='color:#696969;' ng-disabled = tableExists(" + params.data.table + ") href='#Table/" + params.data.table.replace('app.', '') + "'>" + params.data.table + "<a>";
            }
            else return ' ';

        };

        function checkBoxRenderer(params) {
            //return '<div style="text-align:center;"><md-checkbox ng-model="'+params.data.done+'"class="orange" aria-label="addWhenBound" type="checkbox"></div>';
            return '<div style="text-align:center;"><input type="checkbox"><div>';
        };

        $scope.gridOptions = gridOptions;
    })();

    //console.log($scope.parents + '!!!');

    $scope.masterLabels = ["Download Sales", "In-Store Sales", "Mail-Order Sales", "test", "t2"];
    $scope.masterData = [300, 500, 100, 80, 200];

    $scope.doughnutLabels = ["Download Sales", "In-Store Sales", "Mail-Order Sales"];
    $scope.doughnutData = [300, 500, 100];

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