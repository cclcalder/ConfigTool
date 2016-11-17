/* ----- Home Setup ----- */
app.controller("HomeSetupCtrl", function ($scope, $routeParams) {
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
app.controller("ModeSetupCtrl", function ($scope) {
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
app.controller("ModeContinueCtrl", function ($scope, $mdDialog, $location) {
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