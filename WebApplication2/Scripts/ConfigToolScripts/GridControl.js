/* ----- agGrid CRUD ----- */
// ---  Split this into multiple controls - getting out of hand
app.controller("TableCtrl", function ($scope, $routeParams, $timeout, $mdDialog, crudAJService, sharedService) {
    console.log("TableCtrl, " + $routeParams.tablename);
    $scope.tablename = $routeParams.tablename;

    var crudArray = new Array();

    // Flags 
    $scope.loadingIsDone = false;
    $scope.rowOverload = false;
    $scope.associatedTablesExist = false;
    $scope.showAssociatedTables = false;
    $scope.unsavedChanges = false;
    $scope.wizardMode = true;
    $scope.error = false;
    if ($scope.wizardMode) {
        $scope.task = sharedService.getTask();
    }
    console.log($scope.task);

    function LoadTableContent() {

        $scope.dataLoaded = false;
        $scope.isLoading = true;

        var loadMethod = crudAJService.loadTableContent($scope.tablename);
        loadMethod.then(function (TableContent) {
            if (TableContent.data.overLoad !== 0) {
                alert('Overload: ' + TableContent.data.overLoad + " rows!");
            }
            if (TableContent.data.dataStringArr != null) {
                $scope.data = JSON.parse("[" + TableContent.data.dataStringArr + "]");
            }
            else {
                $scope.data = null;
            }

            //setup scopes
            $scope.columnHeaders = JSON.parse(TableContent.data.headerArr);
            //$scope.emptyRow = JSON.parse(crudAJService.getEmptyRow($scope.tablename));
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
                            if (head.type === "numeric") {
                                head.cellEditor = numericEditor;
                                return { 'text-align': 'right', 'padding-right': '5px' };
                            }
                            else if (head.type === "bool") {
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
                                //if multiple tables
                                for (var i = 0; i < $scope.associatedTables.length; i++) {
                                    if ($scope.associatedTables[i].currentKey === head.headerName) {
                                        head.cellEditorParams = JSON.parse($scope.associatedTables[i].fKeyData);
                                    }
                                }
                                return { 'padding-left': '5px' };
                            }
                            else if (head.type == "date") {
                                //head.cellRenderer = dateRenderer;
                                return { 'padding-left': '5px' };
                            }
                            else if (head.type == "changes") {
                                head.cellRenderer = changesRenderer;
                                return { 'background-color': 'rgba(0, 0, 0, 0.05)', 'padding-left': '5px' };
                            }
                            else return { 'padding-left': '5px' };
                        }
                    })
                };

                //NUMERIC EDITOR
                var numericEditor = function () { };
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
                //Used when have time to implement properly
                //var checkBoxRenderer = function (params) {
                //    //return '<div style="text-align:center;"><md-checkbox ng-model="params.value" aria-label="addWhenBound" type="checkbox"><div>';
                //    //return '<input type="checkbox">';
                //};

                //var dateRenderer = function (params) {
                //    console.log("date renderer");
                //    //date renderer FIX - doesnt have ngModel at the moment
                //    //return '<md-datepicker ' + params.value + '"></md-datepicker>';
                //    return '<input type="date">';
                //}

                var changesRenderer = function (params) {
                    if (params.value == null || params.value == "") { return '<span style="color:rgb(63,81,181)"> - <span>'; }
                    if (params.value == 1) { return '<span style="color:#45B25D"> Edited <span>'; };
                    if (params.value == 2) { return '<span style="color:#EA4E4E"> Deleted <span>'; };
                    if (params.value == 3) { return '<span style="color:#FF8C00"> Added <span>'; };
                    //else { return '<span class="rag-element">' + params.value + '<span>' };
                };

                //init gridOptions
                var gridOptions = {
                    columnDefs: $scope.columnHeaders,
                    rowData: $scope.data,
                    angularCompileRows: true,
                    singleClickEdit: true,
                    editType: "fullRow",
                    cellStyle: getStyle(),
                    onCellValueChanged: function (event) {
                        $scope.unsaved = true;
                        //event.colDef.field.cellStyle = { 'background-color': 'green' };
                        var updateString;
                        //not 100% working but do in a min
                        if (event.oldValue === "") {
                            updateString = 'add: ' + event.colDef.field + event.newValue + ", ";
                        }
                        else {
                            updateString = 'update: ' + event.colDef.field + ' from ' + event.oldValue + ' to ' + event.newValue + ", ";
                        }

                        crudArray.push(updateString);
                        console.log('onCellValueChanged: ' + event.colDef.field + ' from ' + event.oldValue + ' to ' + event.newValue);
                        if (event.oldValue !== event.newValue && event.data['hasChanges'] !== 3) {
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
                    var strArr = [];
                    rowData.forEach(function (obj) {
                        if (obj['hasChanges'] != null) {
                            strArr.push(JSON.stringify(obj));
                        }
                    });
                    var savingTable = crudAJService.saveNewTable(strArr);
                    savingTable.then(function (success) {
                        gridOptions.api.refreshView();
                        if (success) {
                            gridOptions.api.refreshView(); //THIS DOESNT REFERESH PROPERLY RIGHT NOW?
                            LoadTableContent(); //shouldnt have to do this - want to just refresh data in grid
                            console.log('success' + success);
                            $mdDialog.show(
                               $mdDialog.alert()
                                 .clickOutsideToClose(true)
                                 .title('Success')
                                 .textContent('Changes submitted to database.')
                                 .ok('Ok')
                             );
                            //alert("Success: Changes submitted to database.");
                            $scope.unsavedChanges = false;
                        }
                        else {
                            $scope.unsavedChanges = true;
                            alert("Error: Invalid operation");
                        }
                    }, function () {
                     $mdDialog.show(
                               $mdDialog.alert()
                                 .clickOutsideToClose(true)
                                 .title('Error')
                                 .textContent('Save unsuccessful.')
                                 .ok('Ok')
                             );
                        //alert('Error: Save unsuccessful');
                        $scope.unsavedChanges = true;
                    });
                };

                $scope.gridOptions = gridOptions;

                $scope.onRemoveSelected = function () {
                    $scope.unsavedChanges = true;
                    var selection = $scope.gridOptions.api.getSelectedNodes();
                    //change this to turn rows red when provisionally deleted
                    //$scope.gridOptions.api.removeItems(selection);
                    selection.forEach(function (node) {
                        node.data['hasChanges'] = 2;
                        gridOptions.api.refreshView();
                        crudArray.push("delete: " + JSON.stringify(node.data) + ", ");
                    });

                };

                $scope.onSave = function () {
                    var confirm = $mdDialog.confirm()
                         .title('Are you sure you want to save changes, this cannot be undone.' + crudArray)
                         .ok('Save')
                         .cancel('Cancel');
                    $mdDialog.show(confirm).then(function () {
                        //crudArray could be used for audit - but more likely newValueHandler..
                        //console.log("Changes" + crudArray);
                        getRowData();
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
                        //reset all has changes col
                        gridOptions.api.forEachNode(function (node) {
                            node.data['hasChanges'] = 0;
                        });
                        $scope.gridOptions.api.refreshView();
                        //$scope.gridOptions.api.setRowData($scope.data);
                        $scope.unsavedChanges = false;

                    }, function () {
                        console.log('Cancel');
                    });
                };
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

    // Call table load
    LoadTableContent();

    // Add row
    var newCount = 0;
    $scope.onAddRow = function () {
        console.log("onAddRow");
        $scope.unsavedChanges = true;
        var newItem = tempNewRowData();
        $scope.gridOptions.api.insertItemsAtIndex(0, [newItem]);
    }
    function tempNewRowData() {
        var newRow = $scope.emptyRow;
        newCount++;
        return newRow;
    }

    // Associated Tables
    $scope.openAssociatedTables = function () {
        if (!$scope.showAssociatedTables) {
            $scope.showAssociatedTables = true;
        }
        else if ($scope.showAssociatedTables) {
            $scope.showAssociatedTables = false;
        }
    }

    // Sizing
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

    // Scripts
    $scope.openCurrentScript = function () {
        console.log("Create insert all script");
        var confirm = $mdDialog.confirm()
            .title('Current table insert script.')
            .ok('Copy insert script')
            .cancel('Cancel');
        $mdDialog.show(confirm).then(function () {
            console.log("Creating insert script");
        }, function () {
            console.log('Cancel');
        });
    }

});

