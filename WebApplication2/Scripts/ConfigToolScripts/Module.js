// get ag-Grid to create an Angular module and register the ag-Grid directive
agGrid.initialiseAgGridWithAngular1(angular);

//app is an Angular module - defined here
var app = angular.module("configToolApp",
    ['ui.bootstrap', 'ngResource', "ngMaterial", "ngAnimate", "ngAria", 'ngRoute', "agGrid", "treeGrid", "chart.js"
       //'ngMessages'
    ]
);