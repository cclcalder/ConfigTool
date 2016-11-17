/* ----- Angular Routing ----- */
app.config(function ($routeProvider) {
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
        .when("/Table/:tablename", {
            templateUrl: function (params) { console.log("Route: Table/" + params.tablename); return "/Home/Table?tablename=" + params.tablename; },
            controller: "TableCtrl"
            //controller: 'TableEditorCtrl'
        })
        .when("/ModeSetup/:process", {
            templateUrl: function (params) { console.log("Route: ModeSetupView/" + params.process); return "/Home/ModeSetup?process=" + params.process; }

        })
        .when("/Table", {
            templateUrl: function () { console.log("Route: Home/Table/"); return "/Home/Table" }
            //this should be for table editor..
        })
        .when("/About", {
            templateUrl: function () { console.log("Route: Home/About/"); return "/Home/About ;" }
        })
        .otherwise({ redirectTo: "/Home/HomeSetup" });


});