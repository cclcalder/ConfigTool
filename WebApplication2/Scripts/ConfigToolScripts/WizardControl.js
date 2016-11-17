app.controller('WizardController', function ($scope, Items) {
    $scope.items = Items;

    $scope.expand = function(item){
        item.done = !item.done;
    }
    $scope.expand = function (children) {
        children.done = !children.done;
    }

    //todoList.addTodo = function () {
    //    todoList.todos.push({ text: todoList.todoText, done: false });
    //    todoList.todoText = '';
    //};

    $scope.remaining = function () {
        var count = 0;
        angular.forEach(Items, function (item) {
            count += item.complete ? 0 : 1;
        });
        return count;
    };

    //todoList.archive = function () {
    //    var oldTodos = todoList.todos;
    //    todoList.todos = [];
    //    angular.forEach(oldTodos, function (todo) {
    //        if (!todo.done) todoList.todos.push(todo);
    //    });

});
//service 

//var todoList = this;
app.factory('Items', [function() {
    var items = [
        {
            text: 'Master Data Setup',
            done: false,
            complete: false,
            children: [
                {
                    text: 'Setup Sales Orgs and Customer Hierarchy / Levels',
                    done: false,
                    subchildren: [
                        {
                            text: 'Review @Html.ActionLink("SYS_Config", "Index", "Home")',
                            done: false
                        }
                    ]
                },
            {
                text: 'Setup Product Hierarchy / Levels',
                done: false,
                subchildren: [
                    {
                        text: 'Review setup.ETL_UK_Load_Products',
                        done: false
                    }
                ]
            },
            {
                text: 'Setup Measures and Attributes',
                done: false,
                subchildren: [
                    {
                        text: 'app.Dim_Product_Cust_Measures',
                        done: false
                    },
                    {
                        text: 'app.Dim_Product_Sku_Measures',
                        done: false
                    },
                    {
                        text: 'app.Dim_Product_Sku_Cust_Measures',
                        done: false
                    },
                    {
                        text: 'app.Dim_Product_Cust_Attributes',
                        done: false
                    },
                    {
                        text: 'app.Dim_Product_Sku_Attributes',
                        done: false
                    },
                    {
                        text: 'app.Dim_Product_Sku_Cust_Attribute',
                        done: false
                    }
                ]
            },
            ]
        },
        {
            text: 'General Setup',
            done: false,
            complete: false,
            children: [
                {
                    text: 'Set Base Unit Of Measure',
                    done: false,
                    subchildren: [
                        {
                            text: 'app.SYS_Config',
                            done: false
                        }
                    ]
                },
                {
                    text: 'Set Deleting Policy',
                    done: false,
                    subchildren: [
                        {
                            text: 'app.SYS_Config',
                            done: false
                        }
                    ]
                },
                {
                    text: 'Set Password Policy',
                    done: false,
                    subchildren: [
                        {
                            text: 'app.SYS_Config',
                            done: false
                        }
                    ]
                },
                {
                    text: 'Config client Calendar View',
                    done: false,
                    subchildren: [
                        {
                            text: 'clnt.vw_Dim_Calendar',
                            done: false
                        }
                    ]
                },
                {
                    text: 'Review References to “Demo”',
                    done: false,
                    subchildren: [
                        {
                            text: 'app.SYS_Config',
                            done: false
                        }
                    ]
                },
                {
                    text: 'Remove any screens and tabs not in scope',
                    done: false,
                    subchildren: [
                        {
                            text: 'app.SYS_Screens',
                            done: false
                        },
                        {
                            text: 'app.SYS_ScreenTabs',
                            done: false
                        },
                        {
                            text: 'app.Fact_Screen_ScreenGroup',
                            done: false
                        }
                    ]
                },
                {
                    text: 'Update languages settings for any renaming of screens, tabs and controls',
                    done: false,
                    subchildren: [
                        {
                            text: 'app.Dim_Language_AppLabels',
                            done: false
                        }
                    ]
                },

            ]
        },
        {
            text: 'Planning Screen Configuration',
            done: false,
            complete: false,
            children: [
               {
                   text: 'Setup Master Data',
                   done: false,
                   subchildren: [
                       {
                           text: 'Review and modify as appropriate app.Dim_Planning_Volume_MeasureGroups',
                           done: false
                       },
                       {
                           text: 'Review and modify as appropriate app.Dim_Planning_Volume_Measures',
                           done: false
                       },
                       {
                           text: 'Review and modify as appropriate app.Dim_Planning_Time_Range',
                           done: false
                       },
                       {
                           text: 'Review and modify as appropriate app.Dim_Planning_Time_Levels',
                           done: false
                       }
                   ]
               },
               {
                   text: 'Test',
                   done: false,
                   subchildren: [
                       {
                           text: 'Check volume saves and reloads, and unit of measure correctly set',
                           done: false
                       }
                   ]
               }
            ]
        },
        {
            text: 'Promotions Configuration',
            done: false,
            complete: false
        },
        {
            text: 'Terms Configuration',
            done: false,
            complete: false
        },
        {
            text: 'Management Adjustment Configuration',
            done: false,
            complete: false
        },
        {
            text: 'Risk And Ops Configuration',
            done: false,
            complete: false
        },
        {
            text: 'Funds',
            done: false,
            complete: false
        }
    ];

    return items;
}]);


/*----------  ------------*/

