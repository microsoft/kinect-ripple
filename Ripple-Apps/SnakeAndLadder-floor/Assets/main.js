var wR; // widthRatio;
var hR; // heightRatio
var originalTileWidth;
var originalTileHeight;
var actualTileWidth;
var actualTileHeight;
var numberOfRows;
var numberOfColumns;
var backgroundImage;
var foregroundImage;
var playerIcon;
var computerIcon;
var playerPosition ;
var computerPosition;
var requiredNumber;
var blinkingInterval;
var mouseOnNumber;
var commands;
var canvas;
var foregroundCanvas;
var playerCanvas;
var computerCanvas;
var ctx;
var foregroundCtx;
var playerCtx;
var computerCtx;
var height;
var width;
var timer;
var turn;
var winner;
//playerOnMove, playerOnTile,computerOnMove,computerOnTile
var boardState;

//sounds
var backgroundSound;
var wheelMoveSound;
var ladderObtainedSound;
var winSound;
var turnChangeSound;
var snakeBittenSound;

window.onload = function () {
    initializeGame();
};

function stopBlinking() {
    clearInterval(blinkingInterval);
    drawBoard();
}

function createCommand(direction, steps, number) {
    var command = new Object();
    command.direction = direction;
    command.steps = steps;
    command.number = number;
    switch (direction) {
        case "forward":
            command.resultantNumber = number + steps;
            break;
        case "backward":
            command.resultantNumber = number - steps;
    }
    return command;
}


function initializeGame() {
    numberOfRows = 4;
    numberOfColumns = 5;
    backgroundImage = new Image();
    backgroundImage.src = backgroundImageData;

    height = window.innerHeight;
    width = window.innerWidth;
    mouseOnNumber = 0;
    timer = initializeTimer();
    turn = "player";
    boardState = "playerOnMove";
    foregroundImage = new Image();
    foregroundImage.src = foregroundImageData;
    backgroundImage.onload = function () {
        wR = width / backgroundImage.width;
        hR = height / backgroundImage.height;
        originalTileWidth = backgroundImage.width / numberOfColumns;
        originalTileHeight = backgroundImage.height / numberOfRows;
        actualTileWidth = wR * originalTileWidth;
        actualTileHeight = hR * originalTileHeight;
        initializeCanvas();
    };


    initializeComputerAndPlayer();
    initializeSounds();
    commands = new Array();
    populateCommands();

}

function initializeSounds() {
    backgroundSound = document.getElementById("backgroundSound");
    wheelMoveSound = document.getElementById("wheelMoveSound");
    ladderObtainedSound = document.getElementById("ladderObtainedSound");
    winSound = document.getElementById("winSound");
    turnChangeSound = document.getElementById("turnChangeSound");
    snakeBittenSound = document.getElementById("snakeBittenSound");
}

function initializeComputerAndPlayer() {
    playerCanvas = document.getElementById("playerCanvas");
    computerCanvas = document.getElementById("computerCanvas");
    playerCtx = playerCanvas.getContext("2d");
    computerCtx = computerCanvas.getContext("2d");
    playerCanvas.width = width;
    playerCanvas.height = height;
    computerCanvas.width = width;
    computerCanvas.height = height;
    playerCtx.fillRect(0, 0, 100, 100);
    requiredNumber = 1;
    playerPosition = 1;
    computerPosition = 1;
    playerIcon = new Image();
    playerIcon.src = playerImageData;
    playerIcon.onload = function () {
        drawPlayer();
    }
    computerIcon = new Image();
    computerIcon.src = computerImageData;
    computerIcon.onload = function () {
        drawComputer();

    }
}

function populateCommands() {
    commands.push(createCommand("forward", 6, 2));
    commands.push(createCommand("forward", 6, 5));
    commands.push(createCommand("forward", 2, 16));
    commands.push(createCommand("backward", 2, 13));
    commands.push(createCommand("backward", 7, 15));
}

function initializeCanvas() {
    canvas = document.getElementById("gameCanvas");
    ctx = canvas.getContext("2d");
    foregroundCanvas = document.getElementById("foregroundCanvas");
    foregroundCtx = foregroundCanvas.getContext("2d");
    computerCanvas.onmousemove = function (e) {
        var currentNumber = findMouseOnWhichNumber(e.clientX, e.clientY);
        if ((mouseOnNumber == 0 || currentNumber != mouseOnNumber) && !timer.running ) {
            //console.log("timer");
            timer.startTimer();
        }
       
        if (turn == "player" && boardState =="playerOnMove" && currentNumber == requiredNumber && timer.getTime() > 1 && currentNumber != mouseOnNumber) {
            timer.stopTimer();
            mouseOnNumber = currentNumber;
            playerPosition = currentNumber;
            drawPlayer();
            stopBlinking();
           
            if (playerPosition == numberOfRows*numberOfColumns) {
                declareWinner();
            }
            turn = "computer";
            boardState = "playerOnTile";
            var commandForNumber = findCommandForNumber(mouseOnNumber);
            if (commandForNumber != null) {
                turn = "player";
                boardState = "playerOnMove";
                requiredNumber = commandForNumber.resultantNumber;
                blinkNumber(requiredNumber);
                if (commandForNumber.resultantNumber < currentNumber) {
                    sendMessageToFrontScreen("updateInstruction", ["Ooops! <br/><br/>Snake bites you<br/><br/>Move Back"]);
                    snakeBittenSound.play();
                }
                else {
                    ladderObtainedSound.play();
                    sendMessageToFrontScreen("updateInstruction", ["Yeah! <br/><br/>You got Ladder<br/><br/>Move Ahead"]);
                    
                }

            }
            else {
                if (winner != "Player") {
                    turnChangeSound.play();
                    sendMessageToFrontScreen("updateInstruction", ["Computer's Turn"]);
                    setTimeout(function () {
                        sendMessageToFrontScreen("moveWheel", [""]);
                    }, 2000);
                }
            }
        }
       
    };
    canvas.width = window.innerWidth;
    canvas.height = window.innerHeight;
    foregroundCanvas.width = window.innerWidth;
    foregroundCanvas.height = window.innerHeight;
    setTimeout(function(){
        drawBoard();
    },100);
    blinkNumber(requiredNumber);
}

function findCommandForNumber(number) {
    for (i = 0; i < commands.length; ++i) {
        if (commands[i].number == number) {
            return commands[i];
        }
    }
    return null;
}

function findMouseOnWhichNumber(x, y) {
    var rowNumber = Math.floor((y / actualTileHeight) + 1);
    var columnNumber = Math.floor((x / actualTileWidth) + 1);
    var number;
    if (columnNumber % 2 == 1) {
        number = (columnNumber * numberOfRows) + rowNumber;
    }
    else {
        number = (columnNumber * numberOfRows);
        number += ((numberOfRows+1) - rowNumber);
    }
    return number - numberOfRows;
}

function getDimensionsFromNumber(number) {
    var dimensions = new Object();
    var columnNumber = Math.ceil((number / numberOfRows));
    var rowNumber;
    if (columnNumber % 2 == 1) {
        rowNumber = (number % numberOfRows);
        if (rowNumber == 0) {
            rowNumber = numberOfRows;
        }
    }
    else {
        rowNumber = (number % numberOfRows);
        if (rowNumber == 0) {
            rowNumber = numberOfRows;
        }
        rowNumber = (numberOfRows + 1) - rowNumber;
    }
    dimensions.x = (actualTileWidth * (columnNumber - 1));
    dimensions.y = (actualTileHeight * (rowNumber - 1));
    dimensions.width = actualTileWidth;
    dimensions.height = actualTileHeight;
   // console.log(dimensions);
    return dimensions;
    
}

function declareWinner() {
    resetGame();
    winner = (playerPosition == 20) ? "Player" : "Computer";
    if (winner == "Player") {
        winSound.play();
    }
    //sendMessageToFrontScreen("updateInstruction", [winner + "WINS"]);
    computerCtx.save();
    computerCtx.fillStyle = "#f00";
    computerCtx.fillRect(0, 0, width, height);
    computerCtx.fillStyle = "#fff";
    computerCtx.font = "64px Segoe UI Light";
    computerCtx.translate(Math.floor(width / 2), Math.floor(height / 2));
    computerCtx.rotate(Math.PI / 2);
    computerCtx.textAlign = "center";
    computerCtx.fillText(winner + " Wins", 0, 0);
    computerCtx.restore();
    sendMessageToFrontScreen("updateInstruction", [winner + "Wins"]);
}

function resetGame() {
    canvas.width = canvas.width;
    foregroundCanvas.width = foregroundCanvas.width;
    playerCanvas.width = playerCanvas.width;
    computerCanvas.width = computerCanvas.width;
    //stopBlinking();
}

function numberRecieved(number) {
    if (turn == "player") {
        var nextNumber = mouseOnNumber + number;
        if (nextNumber > (numberOfRows * numberOfColumns)) {
            turn = "computer";
            turnChangeSound.play();
            sendMessageToFrontScreen("updateInstruction", ["Computer's Turn"]);			
			setTimeout(function(){
				sendMessageToFrontScreen("moveWheel", [""]);
			},2000);
        }
        else {
            requiredNumber = nextNumber;
            blinkNumber(requiredNumber);
            boardState = "playerOnMove";
            sendMessageToFrontScreen("updateInstruction", ["Please Step On Blinking Number"]);
        }
    }
    else if (turn == "computer") {
        //simulate Computer Turn and set turn = player after that and send instructions for player to rotate wheel
        boardState = "computerOnMove";
        simulateComputerTurn(number);
    }
}

function simulateComputerTurn(number) {
    var nextNumber = computerPosition + number;
    if (nextNumber > (numberOfRows * numberOfColumns)) {
        turn = "player";
        boardState = "computerOnTile";
        turnChangeSound.play();
		sendMessageToFrontScreen("updateInstruction", ["Swipe your <b>right hand</b> to rotate the wheel.<br/><br/> Player's turn"]);
    }
    else {
        blinkNumber(nextNumber);
        moveComputerToNumber(nextNumber);
        sendMessageToFrontScreen("updateInstruction", ["Computer is moving Now"]);
    }
}

function drawBoard() {
    canvas.width = canvas.width;
    foregroundCanvas.width = foregroundCanvas.width;
    ctx.drawImage(backgroundImage, 0, 0, width, height);
    foregroundCtx.drawImage(foregroundImage, 0, 0, width, height);

}

function drawPlayer() {
    playerCanvas.width = playerCanvas.width;
    playerCtx.save();
    var rectangle = getDimensionsFromNumber(playerPosition);
    var playerX = rectangle.x + (0.05 * rectangle.width);
    var playerY = rectangle.y + (0.2 * rectangle.height);
    var playerWidth = (0.45 * rectangle.height);
    var playerHeight = (0.3 * rectangle.width);
    playerCtx.translate((playerX + (playerWidth) / 2), (playerY + (playerHeight) / 2));
    playerCtx.rotate(Math.PI / 2);
    playerCtx.translate(-(playerX + (playerWidth) / 2), -(playerY + (playerHeight) / 2));
    playerCtx.drawImage(playerIcon, playerX, playerY, playerWidth, playerHeight);
    playerCtx.restore();
}

function drawComputer() {
    computerCanvas.width = computerCanvas.width;
    computerCtx.save();
    var rectangle = getDimensionsFromNumber(computerPosition);
    var computerX = rectangle.x + (0.55 * rectangle.width);
    var computerY = rectangle.y + (0.05 * rectangle.height);
    var computerWidth = (0.45 * rectangle.height);
    var computerHeight = (0.3 * rectangle.width);
    computerCtx.translate((computerX + (computerWidth) / 2), (computerY + (computerHeight) / 2));
    computerCtx.rotate(Math.PI / 2);
    computerCtx.translate(-(computerX + (computerWidth) / 2), -(computerY + (computerHeight) / 2));
    computerCtx.drawImage(computerIcon, computerX, computerY, computerWidth, computerHeight);
    computerCtx.restore();
}

function moveComputerToNumber(number) {
    if (computerPosition != number) {
        if (number > computerPosition) {
            computerPosition++;
        }
        else {
            computerPosition--
        }
        drawComputer();
        setTimeout(function () {
            moveComputerToNumber(number);
        }, 700);
    }
    else {
        stopBlinking();
        var commandForNumber = findCommandForNumber(number);
        if (commandForNumber != null) {
            blinkNumber(commandForNumber.resultantNumber);
            moveComputerToNumber(commandForNumber.resultantNumber);
        }
        else {
            boardState = "computerOnTile";
            computerPosition = number;
            turn = "player";
            turnChangeSound.play();
            sendMessageToFrontScreen("updateInstruction", [" Swipe your <b>right hand</b> to rotate the wheel.<br/><br/>Player's turn"]);
            
            if (computerPosition == (numberOfRows * numberOfColumns)) {
                declareWinner();
            }
        }
    }
}

function blinkNumber(number) {
    blinkingInterval = setInterval(function () {
        var rectangle = getDimensionsFromNumber(number);
        var imgData = ctx.getImageData(Math.floor(rectangle.x), Math.floor(rectangle.y), Math.floor(rectangle.width), Math.floor(rectangle.height));
        // invert colors
        for (var i = 0; i < imgData.data.length; i += 4) {
            imgData.data[i] = 255 - imgData.data[i];
            imgData.data[i + 1] = 255 - imgData.data[i + 1];
            imgData.data[i + 2] = 255 - imgData.data[i + 2];
            imgData.data[i + 3] = 255;
        }
        //console.log(imgData.data.length);
        ctx.putImageData(imgData, rectangle.x, rectangle.y);
    }, 500);
}

function getTextFromCommand(command) {
    return "Go " + command.steps + " steps " + command.direction;
}

function getHTMLFromCommand(command) {
    return "Go " + command.steps + " <br/>steps <br/>" + command.direction;
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

function gestureReceived(gestureName) {
    if (gestureName == "LeftSwipe") {
        window.external.executeCommand("exitGame", "");
    }
    if (gestureName == "RightSwipe" && turn =="player" && boardState == "computerOnTile") {
        sendMessageToFrontScreen("moveWheel", [""]);
    }

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
        case "moveWheel":
            wheelMoveSound.play();
            window.external.executeCommand("sendCommandToFrontScreen", commandParametersCSV);
            break;
        case "updateInstruction":
            //window.external.executeCommand("playAudio", commandParameters[1]);
            window.external.executeCommand("sendCommandToFrontScreen", commandParametersCSV);
            break;
    }
}

function executeCommandFromScreen(commandParameters) {
    var parameters = commandParameters.split(",");
    console.log(parameters[0]);
    switch (parameters[0]) {
        case "numberRecieved":
            //console.log("YES");
            numberRecieved(parseInt(parameters[1]));
            break;
    }
}