// get ag-Grid to create an Angular module and register the ag-Grid directive
agGrid.initialiseAgGridWithAngular1(angular);


var app = angular.module("mvcCRUDApp",
    ['ui.bootstrap', 'ngResource', 'ngTable', 'ngRoute', "agGrid"
        
      //, 'ngMaterial', 'ngAnimate', 'ngAria', 'ngMessages'
    ]
);