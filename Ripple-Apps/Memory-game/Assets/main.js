var numberOfRows = 3;
var numberOfColumns = 4;
var Numbers;
var mouseOnTile = "0,0";
var colors = ["rgb(255,130,0)", "rgb(140,198,0)","rgb(244,114,208)", "rgb(0,174,239)","rgb(0,114,198)", "rgb(0,158,73)","rgb(104,33,122)"];
var solvedTiles = new Array();
var timer = initializeTimer();
var tilesOpened = 0;
var gameSolved = false;

function initializeGame() {
    Numbers = populateArray(numberOfRows, numberOfColumns);
    Numbers = shuffle(Numbers);
    for (i = 1; i <= numberOfRows; i++) {
        for (j = 1; j <= numberOfColumns; j++) {
            var gameTile = createGameTile(Numbers[((i - 1) * numberOfColumns) + j - 1], i, j);
            document.getElementById("game-container").appendChild(gameTile);
        }
    }
    document.getElementById("game-container").onmousemove = function (e) {
        if (gameSolved) {
            return;
        }
        if (mouseOnTile == "0,0" && !timer.running) {
            timer.startTimer();
        }
        var currentTile = findMouseOnWhichTile(e.clientX, e.clientY);
        //console.log(timer.getTime());
        if (currentTile != mouseOnTile && !timer.running && solvedTiles.indexOf(currentTile) == -1) {

            timer.startTimer();
        }
        if (timer.getTime() > 2 && currentTile != mouseOnTile && solvedTiles.indexOf(currentTile) == -1) {
            timer.stopTimer();
            try {
                if (document.getElementById(mouseOnTile).getAttribute("data-number") != document.getElementById(currentTile).getAttribute("data-number")) {
                    if (solvedTiles.indexOf(mouseOnTile) == -1) {
                        document.getElementById(mouseOnTile + "-front").style.msTransform = "rotateY(0deg)";
                        document.getElementById(mouseOnTile + "-back").style.zIndex = 100;
                        document.getElementById(mouseOnTile + "-back").style.msTransform = "rotateY(-180deg)";
                    }
                }
                else if (solvedTiles.indexOf(mouseOnTile) == -1 && solvedTiles.indexOf(currentTile) == -1) {
                    solvedTiles.push(mouseOnTile);
                    solvedTiles.push(currentTile);
                    if (solvedTiles.length == (numberOfRows * numberOfColumns)) {
                        setTimeout(function () {
                            gameSolved = true;
                            var gameContainer = document.getElementById("game-container");
                            gameContainer.innerHTML = "";
                            gameContainer.style.transform = "rotate(90deg)";
                            gameContainer.style.textAlign = "center";
                            gameContainer.style.fontFamily = "Segoe UI";
                            gameContainer.style.fontSize = "5.0em";
                            gameContainer.style.color = "#0f0";
                            gameContainer.style.display = "block";
                            gameContainer.style.height = window.innerHeight;
                            gameContainer.style.width = window.innerWidth;
                            gameContainer.innerHTML = "WELL <br/> DONE!"
                        }, 2500);
                    }
                }
            }
            catch (Exception) {

            }
            mouseOnTile = currentTile;
            //console.log(mouseOnTile);
            document.getElementById(mouseOnTile + "-front").style.msTransform = "rotateY(180deg)";
            document.getElementById(mouseOnTile + "-back").style.zIndex = 300;
            document.getElementById(mouseOnTile + "-back").style.msTransform = "rotateY(0deg)";
            document.getElementById("flip-sound").play();
            tilesOpened++;
            var commandParameters = new Array();
            commandParameters.push(tilesOpened);
            sendMessageToFrontScreen("tileOpened", commandParameters);
            commandParameters = new Array();
            commandParameters.push(document.getElementById(mouseOnTile).getAttribute("data-number"));
            sendMessageToFrontScreen("currentTile", commandParameters);

        }

    }

}

window.onload = function () {
    setTimeout(function () {
        //initializeGame();
    }, 25000);
};

function gestureReceived(gestureName) {
    if (gestureName == "RightSwipe") {
        window.external.executeCommand("exitGame", "");
    }
}

function initializeTimer() {
    var timer = new Object();
    timer.actualObject = null;
    timer.value = 1;
    timer.running = false;
    timer.startTimer = function () {
        timer.running = true;
        timer.value = 1;
        timer.actualObject = setInterval(function () { timer.value += 1 }, 1000)
    }
    timer.stopTimer = function () {
        clearInterval(timer.actualObject);
        timer.actualObject = null;
        timer.running = false;
    }
    timer.getTime = function () {
        return timer.value;
    }
    return timer;
}

function findMouseOnWhichTile(x, y) {
    var height = window.innerHeight;
    var width = window.innerWidth;
    rowNumber = (y / (window.innerHeight / numberOfRows)) + 1;
    columnNumber = (x / (window.innerWidth / numberOfColumns)) + 1;
    return Math.floor(rowNumber) + "," + Math.floor(columnNumber);
}

function createGameTile(number, row, column) {
    var gameTile = document.createElement("div");
    gameTile.className = "gameTile"
    gameTile.style.msGridRow = row;
    gameTile.style.msGridColumn = column;
    gameTile.style.textAlign = "center";
    gameTile.style.fontSize = "5.0em";
    gameTile.style.perspective = 1000;
    gameTile.style.msPerspective = 1000;
    gameTile.style.width = "100%";
    gameTile.style.height = "100%";
    gameTile.style.border = "0.01px solid #000";
    gameTile.style.position = "relative";
    gameTile.id = row + "," + column;
    gameTile.setAttribute("data-number", number);



    var frontDiv = document.createElement("div");
    frontDiv.style.background = colors[Math.floor((Math.random() * (colors.length - 1)))];
    frontDiv.style.height = "inherit";
    frontDiv.style.width = "inherit";
    frontDiv.style.backfaceVisibility = "hidden";
    frontDiv.style.position = "absolute";
    frontDiv.style.display = "table";
    frontDiv.style.transition = "2.0s";
    frontDiv.style.zIndex = "200";
    frontDiv.style.border = "0.01px solid #000";
    frontDiv.id = row + "," + column + "-front";

    var innerDiv = document.createElement("div");
    innerDiv.className = "inner";
    var contentDiv = document.createElement("div");
    contentDiv.className = "content";
    innerDiv.style.display = "table-cell";
    innerDiv.style.verticalAlign = "middle"
    innerDiv.style.background = "#ffff88";
    innerDiv.style.border = "10px solid rgb(242,79,33)";

    contentDiv.style.msTransform = "rotate(90deg)";
    contentDiv.style.textAlign = "center";
    //contentDiv.innerHTML = "  <img src=\"Assets/logo.png\" style=\"width:100px;height:100px\"/>";
    contentDiv.innerHTML = " <span style='font-size:1.5em'>? </span>"

    innerDiv.appendChild(contentDiv);
    frontDiv.appendChild(innerDiv);

    var backDiv = document.createElement("div");
    backDiv.className = "outer";
    backDiv.style.display = "table";
    backDiv.style.zIndex = "100";
    backDiv.style.height = "inherit";
    backDiv.style.width = "inherit";
    backDiv.style.transition = "3.0s";
    backDiv.style.border = "0.01px solid #000";
    backDiv.style.position = "absolute";
    backDiv.style.background = "#fff";
    backDiv.style.backfaceVisibility = "hidden";
    backDiv.style.transform = "rotateY(-180deg)";
    backDiv.id = row + "," + column + "-back";

    innerDiv = document.createElement("div");
    innerDiv.className = "inner";
    contentDiv = document.createElement("div");
    contentDiv.className = "content";
    innerDiv.style.display = "table-cell";
    innerDiv.style.verticalAlign = "middle"
    innerDiv.style.background = "rgb(255,207,159)";
    contentDiv.style.msTransform = "rotate(90deg)";
    contentDiv.innerHTML = number;

    innerDiv.appendChild(contentDiv);
    backDiv.appendChild(innerDiv);
    gameTile.appendChild(frontDiv);
    gameTile.appendChild(backDiv);
    return gameTile;

}

function populateArray(rows, columns) {
    var numbers = new Array();
    for (i = 1; i <= ((rows * columns) / 2) ; ++i) {
        numbers.push(i);
        numbers.push(i);
    }
    return numbers;
}

function shuffle(array) {
    var counter = array.length, temp, index;

    // While there are elements in the array
    while (counter--) {
        // Pick a random index
        index = (Math.random() * counter) | 0;

        // And swap the last element with it
        temp = array[counter];
        array[counter] = array[index];
        array[index] = temp;
    }

    return array;
}

function sendMessageToFrontScreen(commandName, commandParameters) {
    // console.log(commandParameters[0]);
    var commandParametersCSV = "";
    for (i = 0; i < commandParameters.length; ++i) {
        commandParametersCSV = commandParametersCSV.concat(",", commandParameters[i]);
    }

    commandParametersCSV = commandName.concat(commandParametersCSV);
    //console.log(commandParametersCSV);
    switch (commandName) {
        case "tileOpened":
            window.external.executeCommand("sendCommandToFrontScreen", commandParametersCSV);
        case "currentTile":
            window.external.executeCommand("sendCommandToFrontScreen", commandParametersCSV);

    }
}

function executeCommandFromScreen(commandParameters) {
    var parameters = commandParameters.split(",");
    console.log(parameters[0]);
    switch (parameters[0]) {
        case "initializeGame":
            //console.log("YES");
            initializeGame();
            break;
    }
}