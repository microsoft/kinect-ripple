function executeCommandFromFloor(commandParameters) {
    var parameters = commandParameters.split(",");
    console.log(parameters[0]);
    switch (parameters[0]) {
        case "tileOpened":
            //console.log("YES");
            document.getElementById("tilesInnerText").innerHTML = "Number of Tiles Opened: " + parameters[1];
            break;
        case "currentTile":
            document.getElementById("currentTileInnerText").innerHTML = "CurrentTile: " + parameters[1];

            break;
    }
}
window.onload = function () {
    setTimeout(function () {
        executeCommandFromFloor("tileOpened,7");
        executeCommandFromFloor("currentTile,4");
    }, 1000);
}