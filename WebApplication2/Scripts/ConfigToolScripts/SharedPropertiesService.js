//Shared variables

//share tasks between Table and Wizard controllers
app.service("sharedService", function () {
    var task = "No task set from service";
    return {
        getTask: function () {
            return task;
        },
        setTask: function (value) {
            task = value;
        }
    }
});