/* ----- Table List ----- */
app.controller("GetTablesCtrl", function ($scope, crudAJService, sharedService) {
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
               name: "Master Data Setup",
               done: "false",
               children: [
                   {
                       folder: true,
                       open: true,
                       name: "Setup Sales Orgs and Customer Hierarchy / Levels",
                       done: "false",
                       children: [
                           { name: "Review app.SYS_Config", table: 'app.SYS_Config', dateChanged: '25/10/2016', done: 'false' }
                       ]
                   },
                   {
                       folder: true,
                       name: "Setup Product Hierarchy / Levels",
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
               open: true,
               name: 'Screen Configuration',
               done: 'false',
               children: [
                   {
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
                       open: true,
                       name: 'Promotions Configuration',
                       done: 'false',
                   },
                   {
                       open: true,
                       name: 'Terms Configuration',
                       done: 'false',
                   },
                   {
                       open: true,
                       name: 'Management Adjustment Configuration',
                       done: 'false',

                   },
                   {
                       open: true,
                       name: 'Risk And Ops Configuration',
                       done: 'false',

                   },
                   {
                       open: true,
                       name: 'Funds',
                       done: 'false',
                       group: false
                   }
               ]
           }
        ];
        //var rowData = [
        //    {
        //        folder: true,
        //        open: true,
        //        name: 'Master Data Setup',
        //        done: 'false',
        //        children: [
        //            {
        //                folder: true,
        //                open: true,
        //                name: 'Setup Sales Orgs and Customer Hierarchy / Levels',
        //                done: 'false',
        //                children: [
        //                    {
        //                        name: 'Review app.SYS_Config',
        //                        table: 'app.SYS_Config',
        //                        dateChanged: '25/10/2016',
        //                        done: 'false'
        //                    }
        //                ]
        //            },
        //            {
        //                folder: true,
        //                name: 'Setup Product Hierarchy / Levels',
        //                done: 'false',
        //                open: true,
        //                children: [
        //                    {
        //                        name: 'Review setup.ETL_UK_Load_Products',
        //                        table: 'setup.ETL_UK_Load_Products',
        //                        dateChanged: '25/10/2016',
        //                        done: 'false'
        //                    }
        //                ]
        //            },
        //            {
        //                folder: true,
        //                open: true,
        //                name: 'Setup Measures and Attributes',
        //                done: 'false',
        //                children: [
        //                    {
        //                        name: 'Setup app.Dim_Product_Cust_Measures',
        //                        table: 'app.Dim_Product_Cust_Measures',
        //                        dateChanged: '25/10/2016',
        //                        done: 'false'
        //                    },
        //                    {
        //                        name: 'Setup app.Dim_Product_Sku_Measures',
        //                        table: 'app.Dim_Product_Sku_Measures',
        //                        dateChanged: '25/10/2016',
        //                        done: 'false'
        //                    },
        //                    {
        //                        name: 'Setup app.Dim_Product_Sku_Cust_Measures',
        //                        table: 'app.Dim_Product_Sku_Cust_Measures',
        //                        dateChanged: '25/10/2016',
        //                        done: 'false'
        //                    },
        //                    {
        //                        name: 'Setup app.Dim_Product_Cust_Attributes',
        //                        table: 'app.Dim_Product_Cust_Attributes',
        //                        dateChanged: '25/10/2016',
        //                        done: 'false'
        //                    },
        //                    {
        //                        name: 'Setup app.Dim_Product_Sku_Attributes',
        //                        table: 'app.Dim_Product_Sku_Attributes',
        //                        dateChanged: '25/10/2016',
        //                        done: 'false'
        //                    },
        //                    {
        //                        name: 'Setup app.Dim_Product_Sku_Cust_Attribute',
        //                        table: 'app.Dim_Product_Sku_Cust_Attribute',
        //                        dateChanged: '25/10/2016',
        //                        done: 'false'
        //                    }
        //                ]
        //            },
        //        ]
        //    },
        //    {
        //        folder: true,
        //        open: true,
        //        name: 'General Setup',
        //        done: 'false',
        //        children: [
        //            {
        //                name: 'Set Base Unit Of Measure',
        //                table: 'app.SYS_Config',
        //                dateChanged: '25/10/2016',
        //                done: 'false'
        //            },
        //            {
        //                name: 'Set Deleting Policy',
        //                table: 'app.SYS_Config',
        //                dateChanged: '25/10/2016',
        //                done: 'false'
        //            },
        //            {
        //                name: 'Set Password Policy',
        //                table: 'app.SYS_Config',
        //                dateChanged: '25/10/2016',
        //                done: 'false'
        //            },
        //            {
        //                name: 'Config client Calendar View',
        //                table: 'clnt.vw_Dim_Calendar',
        //                dateChanged: '25/10/2016',
        //                done: 'false'
        //            },
        //            {
        //                name: 'Review References to “Demo”',
        //                table: 'app.SYS_Config',
        //                dateChanged: '25/10/2016',
        //                done: 'false'
        //            },
        //            {
        //                folder: true,
        //                name: 'Remove any screens and tabs not in scope',
        //                done: 'false',
        //                children: [
        //                    {
        //                        name: 'Remove Screens',
        //                        table: 'app.SYS_Screens',
        //                        dateChanged: '25/10/2016',
        //                        done: 'false'
        //                    },
        //                    {
        //                        name: 'Remove ScreenTabs',
        //                        table: 'app.SYS_ScreenTabs',
        //                        dateChanged: '25/10/2016',
        //                        done: 'false'
        //                    },
        //                    {
        //                        name: 'Remove ScreenGroups',
        //                        table: 'app.Fact_Screen_ScreenGroup',
        //                        dateChanged: '25/10/2016',
        //                        done: 'false'
        //                    }
        //                ]
        //            },
        //            {
        //                name: 'Update languages settings for any renaming of screens, tabs and controls',
        //                table: 'app.Dim_Language_AppLabels',
        //                dateChanged: '25/10/2016',
        //                done: 'false'
        //            },

        //        ]
        //    },
        //    {

        //        folder: true,
        //        name: 'Planning Screen Configuration',
        //        done: 'false',
        //        children: [
        //           {
        //               folder: true,
        //               name: 'Setup Master Data',
        //               done: 'false',
        //               children: [
        //                   {
        //                       name: 'Review and modify as appropriate',
        //                       table: 'app.Dim_Planning_Volume_MeasureGroups',
        //                       dateChanged: '25/10/2016',
        //                       done: 'false'
        //                   },
        //                   {
        //                       name: 'Review and modify as appropriate',
        //                       table: 'app.Dim_Planning_Volume_Measures',
        //                       dateChanged: '25/10/2016',
        //                       done: 'false'
        //                   },
        //                   {
        //                       name: 'Review and modify as appropriate',
        //                       table: 'app.Dim_Planning_Time_Range',
        //                       dateChanged: '25/10/2016',
        //                       done: 'false'
        //                   },
        //                   {
        //                       name: 'Review and modify as appropriate',
        //                       table: 'app.Dim_Planning_Time_Levels',
        //                       dateChanged: '25/10/2016',
        //                       done: 'false'
        //                   }
        //               ]
        //           },
        //           {
        //               folder: true,
        //               name: 'Test',
        //               done: 'false',
        //               children: [
        //                   {
        //                       name: 'Check volume saves and reloads, and unit of measure correctly set',
        //                       dateChanged: '25/10/2016',
        //                       done: 'false'
        //                   }
        //               ]
        //           }
        //        ]
        //    },
        //     //{
        //     //    folder: true,
        //     //    name: 'Promotions Configuration',
        //     //    done: 'false',
        //     //    children: [
        //     //        {
        //     //            folder: true,
        //     //            name: 'Overall Promotions Behaviour',
        //     //            done: 'false',
        //     //            children: [
        //     //                        {
        //     //                            folder: true,
        //     //                            name: 'Review Sys Config Settings',
        //     //                            done: 'false',
        //     //                            children: [
        //     //                                {
        //     //                                    name: 'Phasing',
        //     //                                    table: 'app.SYS_Config',
        //     //                                    dateChanged: '25/10/2016',
        //     //                                    done: 'false'
        //     //                                },
        //     //                                {
        //     //                                    name: 'PostPromo',
        //     //                                    table: 'app.SYS_Config',
        //     //                                    dateChanged: '25/10/2016',
        //     //                                    done: 'false'
        //     //                                },
        //     //                                {
        //     //                                    name: 'PromotionScenario',
        //     //                                    table: 'app.SYS_Config',
        //     //                                    dateChanged: '25/10/2016',
        //     //                                    done: 'false'
        //     //                                },
        //     //                                {
        //     //                                    name: 'EnablePromotionProductsReferenceData',
        //     //                                    table: 'app.SYS_Config',
        //     //                                    dateChanged: '25/10/2016',
        //     //                                    done: 'false'
        //     //                                },
        //     //                                {
        //     //                                    name: 'EnableTemplateConstraints',
        //     //                                    table: 'app.SYS_Config',
        //     //                                    dateChanged: '25/10/2016',
        //     //                                    done: 'false'
        //     //                                },
        //     //                                {
        //     //                                    name: 'Scenario_IsExportActive',
        //     //                                    table: 'app.SYS_Config',
        //     //                                    dateChanged: '25/10/2016',
        //     //                                    done: 'false'
        //     //                                },
        //     //                                {
        //     //                                    name: 'PromoPowerEditor',
        //     //                                    table: 'app.SYS_Config',
        //     //                                    dateChanged: '25/10/2016',
        //     //                                    done: 'false'
        //     //                                },
        //     //                                {
        //     //                                    name: 'Promotion_FileSelector',
        //     //                                    table: 'app.SYS_Config',
        //     //                                    dateChanged: '25/10/2016',
        //     //                                    done: 'false'
        //     //                                },
        //     //                                {
        //     //                                    name: 'ListingsProductSelectedByDefault CanEditPromoFinancialScreenProductData',
        //     //                                    table: 'app.SYS_Config',
        //     //                                    dateChanged: '25/10/2016',
        //     //                                    done: 'false'
        //     //                                },
        //     //                                {
        //     //                                    name: 'Phasing.Daily',
        //     //                                    table: 'app.SYS_Config',
        //     //                                    dateChanged: '25/10/2016',
        //     //                                    done: 'false'
        //     //                                },
        //     //                                {
        //     //                                    name: 'TriggerPromoOrProduct',
        //     //                                    table: 'app.SYS_Config',
        //     //                                    dateChanged: '25/10/2016',
        //     //                                    done: 'false'
        //     //                                },
        //     //                                {
        //     //                                    name: 'TriggerUnitOrDeal',
        //     //                                    table: 'app.SYS_Config',
        //     //                                    dateChanged: '25/10/2016',
        //     //                                    done: 'false'
        //     //                                },
        //     //                                {
        //     //                                    name: 'PromoPhasing_PostPromo',
        //     //                                    table: 'app.SYS_Config',
        //     //                                    dateChanged: '25/10/2016',
        //     //                                    done: 'false'
        //     //                                },
        //     //                                {
        //     //                                    name: 'AllowOverlappingPromotions',
        //     //                                    table: 'app.SYS_Config',
        //     //                                    dateChanged: '25/10/2016',
        //     //                                    done: 'false'
        //     //                                },
        //     //                                {
        //     //                                    name: 'PostPromoNumDays',
        //     //                                    table: 'app.SYS_Config',
        //     //                                    dateChanged: '25/10/2016',
        //     //                                    done: 'false'
        //     //                                },
        //     //                                {
        //     //                                    name: 'PromoUseRRpc',
        //     //                                    table: 'app.SYS_Config',
        //     //                                    dateChanged: '25/10/2016',
        //     //                                    done: 'false'
        //     //                                },

        //     //                            ]
        //     //                        }
        //     //            ]
        //     //        },
        //       {
        //           folder: true,
        //           open: true,
        //           name: 'Terms Configuration',
        //           done: 'false',
        //       },
        //       {
        //           folder: true,
        //           open: true,
        //           name: 'Management Adjustment Configuration',
        //           done: 'false',

        //       },
        //       {
        //           folder: true,
        //           open: true,
        //           name: 'Risk And Ops Configuration',
        //           done: 'false',

        //       },
        //       {
        //           folder: true,
        //           open: true,
        //           name: 'Funds',
        //           done: 'false',

        //       }


        //];

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
            getNodeChildDetails: function (task) {
                if (task.folder) {
                    return {
                        group: true,
                        children: task.children,
                        expanded: task.open
                    };
                } else {
                    return null;
                }
            },
            icons: {
                groupExpanded: '<i class="fa fa-minus-square-o"/>',
                groupContracted: '<i class="fa fa-plus-square-o"/>'
            },
            onRowClicked: rowClicked

        };


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