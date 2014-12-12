var numberOfRows = 4;
var numberOfColumns = 5;
var Numbers;
var mouseOnTile = "0,0";
var colors = ["#82bd2d", "#39AB3E", "#B0EBAD", "C8D97E"];
var solvedTiles = new Array();
var timer = initializeTimer();
var tilesOpened = 0
var gameSolved = false;
var surface;
var ctx;
var commands = new Array();
var forwardCommandNumbers = new Array();
var requiredPlayerTile = "1,1";
var boardState = "playerMove";
var turn = "player";
var requiredTileColor = null;
var computerPosition = 1;
var playerPosition = 1;
window.onload = function () {
    Numbers = populateArray(numberOfRows, numberOfColumns);
    //Numbers = shuffle(Numbers);
    for (i = 1; i <= numberOfColumns; i++) {
        for (j = 1; j <= numberOfRows; j++) {
            var gameTile = createGameTile(Numbers[((i - 1) * numberOfRows) + (j - 1)], j, i);
            document.getElementById("game-container").appendChild(gameTile);
        }
    }
    initializeCanvas();
    setInterval(function () {
        if (boardState == "playerMove") {
                if (requiredTileColor == null) {
                    requiredTileColor = document.getElementById(requiredPlayerTile + "-content").style.backgroundColor;
                }
                if (document.getElementById(requiredPlayerTile + "-content").style.backgroundColor != "rgb(255, 255, 0)") {
                    document.getElementById(requiredPlayerTile + "-content").style.backgroundColor = "#ff0";
                }
                else {
                    document.getElementById(requiredPlayerTile + "-content").style.backgroundColor = requiredTileColor;
                }
        }
    }, 250);

};


function createCommand(direction, steps, tile, number) {
    var command = new Object();
    command.direction = direction;
    command.steps = steps;
    command.tile = tile;
    command.number = number;
    return command;
}


function initializeCanvas() {
    surface = document.getElementById("overlayCanvas");
    surface.width = window.innerWidth;
    surface.height = window.innerHeight;
    ctx = surface.getContext("2d");
    surface.onmousemove = function (e) {
        if (mouseOnTile == "0,0" && !timer.running) {
            timer.startTimer();
        }
        var currentTile = findMouseOnWhichTile(e.clientX, e.clientY);
        // 
        if (currentTile == requiredPlayerTile && timer.getTime() > 1 && currentTile != mouseOnTile) {
            turn = "computer";
            boardState = "playerOnTile";
            document.getElementById(requiredPlayerTile + "-content").style.backgroundColor = requiredTileColor;
            requiredTileColor = null;
            timer.stopTimer();
            if (findCommandForTile(mouseOnTile)) {
                document.getElementById(mouseOnTile + "-front").style.msTransform = "rotateY(0deg)";
                document.getElementById(mouseOnTile + "-back").style.zIndex = 100;
                document.getElementById(mouseOnTile + "-back").style.msTransform = "rotateY(-180deg)";
            }
            mouseOnTile = currentTile;
            playerPosition = parseInt(document.getElementById(mouseOnTile).getAttribute("data-number"));
            console.log("computer:" + computerPosition);
            console.log("player:" + playerPosition);
            var commandForTile = findCommandForTile(currentTile);
            if (commandForTile != null) {
                console.log(commandForTile.direction + ", " + commandForTile.steps + ", " + commandForTile.tile);
                var nextNumber;
                switch (commandForTile.direction) {
                    case "forward":
                        nextNumber = commandForTile.number + commandForTile.steps;
                        break;
                    case "backward":
                        nextNumber = commandForTile.number - commandForTile.steps;
                        break;
                }
                //send Instruction For getTextFromCommand(commandForTile) 
                requiredPlayerTile = getTileByNumber(nextNumber);
                turn = "player";
                boardState = "playerMove";
                document.getElementById(mouseOnTile + "-front").style.msTransform = "rotateY(180deg)";
                document.getElementById(mouseOnTile + "-back").style.zIndex = 300;
                document.getElementById(mouseOnTile + "-back").style.msTransform = "rotateY(0deg)";
            }
            else {
                //Computer Turn Let The Wheel Rotate Automatically
            }
        }
    }
}

function numberRecieved(number) {
    if (boardState == "playerOnTile" && turn == "player") {
        var nextNumber = parseInt(document.getElementById(mouseOnTile).getAttribute("data-number")) + number;
        console.log(nextNumber);
        if(nextNumber > (numberOfRows * numberOfColumns)){
            turn = "computer";
        }
        else{
            requiredPlayerTile = getTileByNumber(nextNumber);
            boardState = "playerMove";
        }


    }
    else if (turn == "computer") {
        //simulate Computer Turn and set turn = player after that and send instructions for player to rotate wheel
        simulateComputerTurn(number);
    }
}

function simulateComputerTurn(number) {
    var nextNumber = computerPosition + number;
    console.log("Next Computer: " + nextNumber);
    if (nextNumber > (numberOfRows * numberOfColumns)) {
        turn = "player";
    }
    else {
        computerTile = getTileByNumber(nextNumber);

        var commandForTile = findCommandForTile(computerTile);
        if (commandForTile != null) {
            document.getElementById(computerTile + "-front").style.msTransform = "rotateY(180deg)";
            document.getElementById(computerTile + "-back").style.zIndex = 300;
            document.getElementById(computerTile + "-back").style.msTransform = "rotateY(0deg)";
            setTimeout(function () {
                document.getElementById(computerTile + "-front").style.msTransform = "rotateY(0deg)";
                document.getElementById(computerTile + "-back").style.zIndex = 100;
                document.getElementById(computerTile + "-back").style.msTransform = "rotateY(-180deg)"
                var furtherNextNumber;
                switch (commandForTile.direction) {
                    case "forward":
                        furtherNextNumber = commandForTile.number + commandForTile.steps;
                        break;
                    case "backward":
                        furtherNextNumber = commandForTile.number - commandForTile.steps;
                        break;
                }
                computerPosition = furtherNextNumber;
                console.log("computer:" + computerPosition);
                console.log("player:" + playerPosition);
                turn = "player";
            }, 3000);

        }
        else {
            computerPosition = nextNumber;
            turn = "player";
            console.log("computer:" + computerPosition);
            console.log("player:" + playerPosition);
            //send instruction to roate wheel for player
        }
      
    }
}

function getTileByNumber(number) {
    var column = Math.ceil(number / numberOfRows);
    var row = (number % numberOfRows);
    if (row == 0) {
        row = numberOfRows;
    }
    return row + "," + column;
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
    gameTile.style.border = "1px solid red";
    gameTile.style.position = "relative";
    gameTile.id = row + "," + column;
    gameTile.setAttribute("data-number", number);

    var chance = determineChance(number);

    if (chance) {
        var frontDiv = document.createElement("div");
        frontDiv.style.background = colors[Math.floor((Math.random() * (colors.length - 1)))];
        frontDiv.style.height = "inherit";
        frontDiv.style.width = "inherit";
        frontDiv.style.backfaceVisibility = "hidden";
        frontDiv.style.position = "absolute";
        frontDiv.style.display = "table";
        frontDiv.style.transition = "2.0s";
        frontDiv.style.zIndex = "200";
        frontDiv.style.border = "1px solid red";
        frontDiv.id = row + "," + column + "-front";
    }

    var backDiv = document.createElement("div");
    backDiv.className = "outer";
    backDiv.style.display = "table";
    backDiv.style.zIndex = "100";
    backDiv.style.height = "inherit";
    backDiv.style.width = "inherit";
    backDiv.style.border = "1px solid red";
    backDiv.style.position = "absolute";
    if (chance) {
        backDiv.style.transition = "3.0s";
        backDiv.style.backfaceVisibility = "hidden";
        backDiv.style.transform = "rotateY(-180deg)";
    }
    backDiv.style.background = "#fff";
    backDiv.id = row + "," + column + "-back";

    var innerDiv = document.createElement("div");
    innerDiv.className = "inner";
    var contentDiv = document.createElement("div");
    contentDiv.className = "content";
    innerDiv.style.display = "table-cell";
    innerDiv.style.background = colors[Math.floor((Math.random() * (colors.length - 1)))];
    innerDiv.style.verticalAlign = "middle"
    innerDiv.style.color = "#000";
    innerDiv.id = row + "," + column + "-content";
    contentDiv.style.msTransform = "rotate(90deg)";
    contentDiv.innerHTML = number;
    innerDiv.appendChild(contentDiv);
    if (chance) {
        frontDiv.appendChild(innerDiv);
        var direction = (Math.random() > 0.5 ? "forward" : "backward");
        var tile = row + "," + column;
        var steps;
        invalidCommand = true;
        counter = 0;
        var command;
        while ((invalidCommand == true) && (counter <5) ) {
            steps = Math.ceil((Math.random() * 8));
            switch (direction) {
                case "forward":
                    var remainingSteps = ((numberOfRows * numberOfColumns) - number);
                    steps = (remainingSteps < steps) ? Math.ceil(Math.random() * remainingSteps) : steps;
                    break;
                case "backward":
                    var forwardSteps = number-1;
                    steps = (forwardSteps < steps) ? Math.ceil(Math.random() * forwardSteps) : steps;

            }
            command = createCommand(direction, steps, tile, number);
            invalidCommand = !validateCommand(command);
            console.log();
            counter = counter + 1;
            if (!invalidCommand) {
                commands.push(command);
                break;
            }                       
        }
        if (!invalidCommand) {
            var backInnerDiv = document.createElement("div");
            backInnerDiv.className = "inner";
            var contentDiv = document.createElement("div");
            contentDiv.className = "content";
            backInnerDiv.style.display = "table-cell";
            backInnerDiv.style.verticalAlign = "middle"
            backInnerDiv.style.background = "#fff"
            backInnerDiv.style.color = "#000";
            contentDiv.style.msTransform = "rotate(90deg)";
            contentDiv.style.fontSize = "0.5em";
            contentDiv.innerHTML = getHTMLFromCommand(command);
            backInnerDiv.appendChild(contentDiv);
            backDiv.appendChild(backInnerDiv);
        }
    }
    else {
        backDiv.appendChild(innerDiv);
    }

    gameTile.appendChild(backDiv);
    if (chance) {
        gameTile.appendChild(frontDiv);
    }
    return gameTile;
}

function getTextFromCommand(command) {
    return "Go " + command.steps + " steps " + command.direction;
}

function getHTMLFromCommand(command) {
    return "Go " + command.steps + " <br/>steps <br/>" + command.direction;
}

function validateCommand(command) {
    var resultingNumber;
    switch (command.direction) {
        case "forward":
            resultingNumber = command.number + command.steps;
            forwardCommandNumbers.push(resultingNumber);
        case "backward":
            resultingNumber = command.number - command.steps;
    }
    for (k = 0; k < commands.length; ++k) {
        if (commands[k].number == resultingNumber) {
            return false;
        }
    }
    for (l = 0; l < forwardCommandNumbers.length; ++l) {
        if (forwardCommandNumbers[l] == command.number) {
            return false;
        }
    }

    return true;
}

function findCommandForTile(tile) {
    for (i = 0; i < commands.length; ++i) {
        if (commands[i].tile == tile) {
            return commands[i];
        }
    }
    return null;
}

function populateArray(rows, columns) {
    var numbers = new Array();
    for (i = 1; i <= ((rows * columns)) ; ++i) {
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

function determineChance(number) {
    return (Math.random() < 0.5 ? (number > 1 && number < (numberOfRows * numberOfColumns) && (commands.length < Math.ceil(numberOfRows*numberOfColumns*0.5)) ? true : false) : false);
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