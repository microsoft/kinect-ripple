function executeCommandFromFloor(commandParameters) {
    var parameters = commandParameters.split(",");
    console.log(parameters[0]);
    switch (parameters[0]) {
        case "gestureRecieved":
            //console.log("YES");
            document.getElementById("gesture").innerHTML = "Gesture: " + parameters[1];
            break;
    }
}
