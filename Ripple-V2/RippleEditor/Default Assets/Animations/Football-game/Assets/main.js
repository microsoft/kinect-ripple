var canvas, netCanvas, ctx, netCtx, QUALITY, field, ball, net, WIDTH, HEIGHT;

var centerText;
var backgroundImage;
var fontColor;
var point1, point2, point3, point4;
var angle1, angle2, angle3;
var currentIndex;
var ballMoving;
var speedCounter;
var fieldWidthRatio;
var fieldHeightRatio;
var timeKeeper;
var startPoint;
var points = [];
var counter = 0;
var gaolReached;
var mouseMoving;
var mouseMoveStack = [];
var resetTimer;
var exitTimer;
var currentPosition;
var boundaries = new Object();
var goalPost = new Object();
var goalLines = new Object();
var ballMovingTimer;

function startAnimation() {
    init();
}

function init() {
    canvas = document.getElementById("mainCanvas");
    ctx = canvas.getContext("2d");
    netCanvas = document.getElementById("netCanvas");
    netCtx = netCanvas.getContext("2d");
    QUALITY = 1;
    field = document.createElement("img");
    ball = document.createElement("img");
    net = document.createElement("img");
    WIDTH = Math.floor(window.innerWidth / QUALITY);
    HEIGHT = Math.floor(window.innerHeight / QUALITY);
    point1 = new Object();
    point1.x = 275;
    point1.y = 550;
    points.push(point1);
    currentPosition = point1;
    point2 = new Object();
    point2.x = 650
    point2.y = 250
    points.push(point2);
    point3 = new Object();
    point3.x = 1000;
    point3.y = 650;
    points.push(point3);
    point4 = new Object();
    point4.x = 1700
    point4.y = 550;
    points.push(point4);
    speedCounter = 0;
    ballMoving = false;
    currentIndex = 0;
    goalReached = false;
    mouseLastMoved = 0;
    boundaries.xStart = 100;
    boundaries.xEnd = 1750;
    boundaries.yStart = 60;
    boundaries.yEnd = 1040;
    goalPost.xStart = 1600;
    goalPost.xEnd = 1750; //not used
    goalPost.yStart = 300;
    goalPost.yEnd = 750;

    goalLines.top = getNewLine(1525, 250, 1525, 850);
    goalLines.bottom = getNewLine(1750, 310, 1750, 750);
    goalLines.left = getNewLine(1525, 250, 1750, 310);
    goalLines.right = getNewLine(1530, 850, 1750, 750);

    resetTimer = initializeTimer();
    exitTimer = initializeTimer();
    ballMovingTimer = initializeTimer();
    timeKeeper = initializeTimer();
    setInterval(function () {
        if ((new Date().getTime() - mouseLastMoved) > 1000) {
            timeKeeper.stopTimer();
            mouseMoving = false;
        }
    }, 1000);
    netCanvas.onmousemove = function (e) {
        counter++;
        ////console.log(counter+1);
        var mousePoint = new Object();
        mousePoint.x = e.clientX;
        mousePoint.y = e.clientY;
        mouseMoveStack.push(mousePoint);
        if (!ballMoving && !goalReached) {
            if (significantMouseMoved()) {
                mouseLastMoved = new Date().getTime();
            }
            ////console.log("LOLLLSDKSDSDK");
            if (!timeKeeper.running) {
                startPoint = new Object();
                startPoint.x = e.clientX;
                startPoint.y = e.clientY;
                timeKeeper.startTimer();
                ////console.log(timeKeeper.getTime());
            }
            if (collision(e.clientX, e.clientY)) {
                timeKeeper.stopTimer();
                start = new Object();

                start.x = ((currentPosition.x) * fieldWidthRatio);
                start.y = ((currentPosition.y) * fieldHeightRatio);
                ////console.log(startPoint);
                ////console.log(point1.x*fieldWidthRatio);
                var lastPoint;
                var tenthLastPoint;
                var slope
                if (mouseMoveStack.length > 10) {
                    lastPoint = mouseMoveStack[mouseMoveStack.length - 1];
                    tenthLastPoint = mouseMoveStack[mouseMoveStack.length - 10];
                    slope = calculateSlope(tenthLastPoint, lastPoint);
                    //console.log("Slope: " + (180/Math.PI)*slope);
                    if (lastPoint.x - tenthLastPoint.x < 0) {

                        if (lastPoint.y - tenthLastPoint.y < 0 || true) {
                            slope = (Math.PI) + slope;
                            //console.log("Changed");
                        }
                        //slope += (Math.PI)/2;
                    }

                }
                else if (mouseMoveStack.length >= 2) {
                    lastPoint = mouseMoveStack[mouseMoveStack.length - 1];
                    slope = calculateSlope(mouseMoveStack[0], lastPoint);
                }
                else {
                    slope = 0;
                }
                ////console.log("Slope: " + slope);

                destination = new Object();
                destination.x = ((points[currentIndex + 1].x) * fieldWidthRatio);
                destination.y = ((points[currentIndex + 1].y) * fieldHeightRatio);
                ////console.log(distance(startPoint,start));
                var speed = 4 * (((distance(startPoint, start)) / 100) / timeKeeper.getTime());
                speed = (speed > 7 && distance(startPoint, start) > 300) ? 8 : (distance(startPoint, start) < 150 && speed > 5) ? 6 : speed + 1;
                speed = speed + (6 / speed) + 1;
                console.log(speed);
                var movementPoints = [];
                movementPoints.push(start);
                document.getElementById("kickSound").play();
                document.getElementById("kickSound").play();
                document.getElementById("kickSound").play();
                moveInDirection(start, slope, speed, movementPoints);
                //move(start, destination, speed);				
            }
        }
    }
}

function getNewLine(xStart, yStart, xEnd, yEnd) {
    var line = new Object();
    line.xStart = xStart;
    line.xEnd = xEnd;
    line.yStart = yStart;
    line.yEnd = yEnd;
    return line;
}

function displayChoiceMenu() {
    setTimeout(function () {
        document.getElementById("choiceMenu").style.display = "block";
        mainCanvasCtx = mainCanvas.getContext("2d");
        mainCanvas.width = mainCanvas.width;
        netCanvas.width = netCanvas.width;
        mainCanvasCtx.save();
        mainCanvasCtx.fillStyle = "#000";
        mainCanvasCtx.fillRect(0, 0, window.innerWidth, window.innerHeight);
        mainCanvasCtx.restore();
        writeCenterText("GOAL!");
    }, 500);

}

function hideChoiceMenu() {
    document.getElementById("choiceMenu").style.display = "none";
}

function startCapture(command) {
    switch (command) {
        case "exit":
            document.getElementById("Exit").style.backgroundColor = "#fff";
            document.getElementById("Exit").style.color = "#000";
            exitTimer.startTimer();
            setTimeout(function () {
                if (exitTimer.running && exitTimer.getTime() > 1) {
                    hideChoiceMenu();
                    window.external.executeCommand("exitGame", "");
                }
            }, 2000);
            break;
        case "reset":
            document.getElementById("Reset").style.backgroundColor = "#fff";
            document.getElementById("Reset").style.color = "#000";
            resetTimer.startTimer();
            setTimeout(function () {
                if (resetTimer.running && resetTimer.getTime() > 1) {
                    hideChoiceMenu();
                    resetGame();
                }
            }, 2000);
            break;
    }
}

function stopCapture(command) {
    switch (command) {
        case "exit":
            document.getElementById("Exit").style.backgroundColor = "transparent";
            document.getElementById("Exit").style.color = "#fff";
            exitTimer.stopTimer();
        case "reset":
            document.getElementById("Reset").style.backgroundColor = "transparent";
            document.getElementById("Reset").style.color = "#fff";
            resetTimer.stopTimer();
            break;
    }
}

function resetGame() {
    canvas.width = canvas.width;
    goalReached = false;
    ballMoving = false;
    drawNetAndField();
    ctx.drawImage(ball, 275 * fieldWidthRatio, 550 * fieldHeightRatio, 100 * (window.innerHeight / 800), 100 * (window.innerHeight / 800));
    currentPosition.x = 275;
    currentPosition.y = 550;
    writeCenterText(centerText);
    document.getElementById("backgroundMusic").play();
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

function writeCenterText(message) {
    ctx.fillStyle = fontColor;
    ctx.strokeStyle = fontColor;
    var startSize = 48
    ctx.font = startSize + "px Segoe UI";
    ctx.textBaseline = "middle";
    ctx.textAlign = "Center";
    var actualXStart = 725;
    var actualXEnd = 1060;
    var requiredWidth = (actualXEnd - actualXStart) * fieldWidthRatio;
    ////console.log(requiredWidth);
    while (ctx.measureText(message).width > requiredWidth) {
        startSize = startSize - 2;
        ctx.font = startSize + "px Segoe UI";
        ////console.log(ctx.measureText(message));
    };
    ctx.fillText(message, (actualXStart * fieldWidthRatio), ((580 - (startSize / 2)) * fieldHeightRatio));


}

function collision(mouseX, mouseY) {
    ballX = (currentPosition.x) * fieldWidthRatio;
    ballY = (currentPosition.y) * fieldHeightRatio;
    ////console.log(ballX+","+ballY);
    ////console.log(mouseX+","+mouseY);


    //ctx.strokeRect(ballX - 50, ballY - 50, 200, 200);
    var scrMul = window.innerHeight / 800;
    if (((mouseX > (ballX - (50 * scrMul))) && (mouseX < (ballX + (150 * scrMul)))) && ((mouseY > (ballY - (50 * scrMul))) && (mouseY < (ballY + (150 * scrMul))))) {
        //alert("BallX: " + ballX + " Bally" + ballY + "mouseX" + mouseX +"mouseY" + mouseY);
        return true;
    }
    else {
        return false;
    }
}


function notifyWPF() {
    configureScreen("Assets/field.jpg", "Microsoft IT", "#FFFFFF");
    //window.external.LoadingDone();
}

function configureScreen(backgroundImage, centerMessage, textColor) {
    init();
    centerText = centerMessage;
    fontColor = textColor;
    //Setting Width Height
    canvas.width = WIDTH;
    canvas.height = HEIGHT;
    netCanvas.width = WIDTH;
    netCanvas.height = HEIGHT;
    //canvas.style.border = "1px solid black";
    canvas.style.width = (window.innerWidth) + "px";
    canvas.style.height = (window.innerHeight) + "px";
    netCanvas.style.height = (window.innerHeight) + "px";
    netCanvas.style.width = (window.innerWidth) + "px";

    /*	angle1 = Math.atan((point2.y-point1.y)/(point2.x-point1.x));
        angle2 = Math.atan((point3.y-point2.y)/(point3.x-point2.x));
        angle3 = Math.atan((point4.y-point3.y)/(point4.x-point3.x));
        
        //console.log(angle1);
        //console.log(angle2);
        //console.log(angle3);*/
    //Drawing On Screen
    field.src = backgroundImage;
    field.onload = function () {
        ctx.drawImage(field, 0, 0, WIDTH, HEIGHT);
        fieldWidthRatio = (WIDTH / field.width);
        fieldHeightRatio = (HEIGHT / field.height);
        ctx.drawImage(ball, (point1.x) * fieldWidthRatio, (point1.y) * fieldHeightRatio, 100 * (window.innerHeight / 800), 100 * (window.innerHeight / 800));
        netCtx.drawImage(net, 1525 * fieldWidthRatio, 240 * fieldHeightRatio, (1750 - 1525) * fieldWidthRatio, (870 - 260) * fieldHeightRatio);
        writeCenterText(centerText);
    };
    net.src = "Assets/net.png";
    net.onload = function () {
        ////console.log("Net Loaded");
        fieldWidthRatio = (WIDTH / field.width);
        fieldHeightRatio = (HEIGHT / field.height);
        //ctx.save();
        /*ctx.translate(WIDTH - 400,(HEIGHT/2)-120);
		ctx.translate(275,100);
		ctx.rotate((86*(Math.PI)/180));*/
        netCtx.drawImage(net, 1525 * fieldWidthRatio, 240 * fieldHeightRatio, (1750 - 1525) * fieldWidthRatio, (870 - 260) * fieldHeightRatio);
        //ctx.strokeRect(-275,-100,550,175);
        //ctx.restore();
    };
    ball.src = "Assets/ball.png";
    ball.onload = function () {
        //////console.log("Ball Loaded");
        fieldWidthRatio = (WIDTH / field.width);
        fieldHeightRatio = (HEIGHT / field.height);
        ctx.drawImage(ball, (point1.x) * fieldWidthRatio, (point1.y) * fieldHeightRatio, 100 * (window.innerHeight / 800), 100 * (window.innerHeight / 800));
    };

    fieldWidthRatio = (WIDTH / field.width);
    fieldHeightRatio = (HEIGHT / field.height);

    writeCenterText(centerText);

    //move(start, destination, 10);
}


function move(start, destination, speed) {
    canvas.width = canvas.width;
    drawNetAndField();
    //////console.log("Distance: " + distance(start,destination));
    if (distance(start, destination) > (10 / speed)) {
        ballMoving = true;
        angle = Math.atan((destination.y - start.y) / (destination.x - start.x));
        start.x = (start.x + (speed) * (Math.cos(angle)));
        start.y = (start.y + (speed) * (Math.sin(angle)));
        ctx.drawImage(ball, start.x, start.y, 100, 100);
        //////console.log(start);
        setTimeout(function () { move(start, destination, speed) }, speed);
    }
    else {
        ctx.drawImage(ball, (destination.x), (destination.y), 100 * (window.innerHeight / 800), 100 * (window.innerHeight / 800));
        ballMoving = false;
        currentIndex += 1;
        speedCounter = 0;
        if (currentIndex == points.length - 1) {
            goalReached = true;
            unlockSystem("System Unlocked");
        }
    }
}

function moveInDirection(start, direction, speed, movementPoints) {
    if (!ballMovingTimer.running) {
        ballMovingTimer.startTimer();
    }
    var currentMousePosition = mouseMoveStack[mouseMoveStack.length - 1];
    if ((Math.abs(speed) > 0.7) && !reachedGoal(start, movementPoints)) {
        canvas.width = canvas.width;
        drawNetAndField();
        ballMoving = true;
        ////console.log("Hello");
        var newPoint = new Object();
        newPoint.x = (start.x + (speed * 2) * (Math.cos(direction)));
        newPoint.y = (start.y + (speed * 2) * (Math.sin(direction)));
        start.x = (start.x + (speed * 2) * (Math.cos(direction)));
        start.y = (start.y + (speed * 2) * (Math.sin(direction)));

        movementPoints[movementPoints.length] = newPoint;
        var speedReverse = false;
        ////console.log("Y Before:" + start.y + " &END " );
        ////console.log((start.y > (boundaries.yEnd*fieldHeightRatio)));
        if ((start.x > (boundaries.xEnd * fieldWidthRatio)) || (start.x < (boundaries.xStart * fieldWidthRatio))) {
            start.x = (start.x < (boundaries.xStart * fieldWidthRatio)) ? start.x + 1 : start.x - 1;
            direction = (Math.PI) - direction;
        }

        else if ((start.y < (boundaries.yStart * fieldHeightRatio)) || (start.y > (boundaries.yEnd * fieldHeightRatio)) || checkNetReflection(movementPoints)) {
            ////console.log("Start:" + start.y);
            ////console.log("YEND:" + boundaries.yEnd*fieldHeightRatio);
            ////console.log("-----");
            start.y = (start.y < boundaries.yStart * fieldHeightRatio) ? start.y + 1 : start.y - 1;
            ////console.log("After Check  " + start.y);
            direction = 2 * (Math.PI) - direction;
        }

        speed = speed - 0.04;

        ////console.log("Start:" + start + " Direction: " + direction + " Speed: " + speed);
        var scrMul = window.innerHeight / 800;
        ctx.drawImage(ball, start.x, start.y, 100 * scrMul, 100 * scrMul);
        ////console.log("Start y" + start.y);
        currentPosition.x = (start.x) / fieldWidthRatio;
        currentPosition.y = (start.y) / fieldHeightRatio;
        if (collision(currentMousePosition.x, currentMousePosition.y) && (currentMousePosition.x > 200 && currentMousePosition.x < (window.innerWidth - 200)) && (currentMousePosition.y > 150 && currentMousePosition.y < (window.innerHeight - 150)) && ballMovingTimer.getTime() > 1) {
            var lastPoint;
            var tenthLastPoint;
            var slope
            if (mouseMoveStack.length > 50) {
                lastPoint = mouseMoveStack[mouseMoveStack.length - 1];
                tenthLastPoint = mouseMoveStack[mouseMoveStack.length - 10];
                slope = calculateSlope(tenthLastPoint, lastPoint);
                //console.log("Slope: " + (180/Math.PI)*slope);
                if (lastPoint.x - tenthLastPoint.x < 0) {

                    if (lastPoint.y - tenthLastPoint.y < 0 || true) {
                        slope = (Math.PI) + slope;
                        //console.log("Changed");
                    }
                    //slope += (Math.PI)/2;
                }

            }
            else if (mouseMoveStack.length >= 2) {
                lastPoint = mouseMoveStack[mouseMoveStack.length - 1];
                slope = calculateSlope(mouseMoveStack[0], lastPoint);
            }
            else {
                slope = 0;
            }
            ////console.log("Slope: " + slope);

            destination = new Object();
            destination.x = ((points[currentIndex + 1].x) * fieldWidthRatio);
            destination.y = ((points[currentIndex + 1].y) * fieldHeightRatio);
            ////console.log(distance(startPoint,start));
            var speed = 4 * (((distance(startPoint, start)) / 100) / timeKeeper.getTime());
            speed = (speed > 7 && distance(startPoint, start) > 300) ? 8 : (distance(startPoint, start) < 150 && speed > 5) ? 6 : speed + 1;
            speed = speed + (6 / speed) + 1;
            var movementPoints = [];
            movementPoints.push(start);
            document.getElementById("kickSound").play();
            document.getElementById("kickSound").play();
            document.getElementById("kickSound").play();
            moveInDirection(start, slope, speed, movementPoints);
        }
        else {
            setTimeout(function () { moveInDirection(start, direction, speed, movementPoints) }, 10);
        }
    }
    else {
        ////console.log(start);
        //console.log(start);
        var scrMul = window.innerHeight / 800;
        ctx.drawImage(ball, (start.x), (start.y), 100 * scrMul, 100 * scrMul);
        /*ctx.strokeRect(start.x,start.y,100,100);
		ctx.strokeRect(start.x+50,start.y,50,100);
		ctx.strokeRect(start.x,start.y+50,100,50);*/
        currentPosition.x = (start.x) / fieldWidthRatio;
        currentPosition.y = (start.y) / fieldHeightRatio;
        ballMoving = false;
        ballMovingTimer.stopTimer();
    }
}

function checkNetReflection(movementPoints) {
    var netReflection = false;

    var currentPoint = movementPoints[movementPoints.length - 1];
    var previousPoint;
    if (movementPoints.length > 2) {
        previousPoint = movementPoints[movementPoints.length - 2];
    }
    else {
        previousPoint = movementPoints[0];
    }



    //console.log("PreviousPoint Y " + previousPoint.y + "Curre " + currentPoint.y +" RIT Distance: " + dotLineLength(currentPoint.x+50, currentPoint.y+50, goalLines.right.xStart*fieldWidthRatio, goalLines.right.yStart*fieldHeightRatio, goalLines.right.xEnd*fieldWidthRatio, goalLines.right.yEnd*fieldHeightRatio, true));
    // Check Left Line

    if ((dotLineLength(currentPoint.x, currentPoint.y, goalLines.left.xStart * fieldWidthRatio, goalLines.left.yStart * fieldHeightRatio, goalLines.left.xEnd * fieldWidthRatio, goalLines.left.yEnd * fieldHeightRatio, true) < 100) && previousPoint.y < currentPoint.y) {
        netReflection = true;
    }

    //Check Right Line
    if ((dotLineLength(currentPoint.x + 50, currentPoint.y + 50, goalLines.right.xStart * fieldWidthRatio, goalLines.right.yStart * fieldHeightRatio, goalLines.right.xEnd * fieldWidthRatio, goalLines.right.yEnd * fieldHeightRatio, true) < 100) && previousPoint.y > currentPoint.y) {
        netReflection = true;
    }

    return netReflection;



}



function goalSideTouch(ballPosition, movementPoints) {
    netGoal = false;
    var currentPoint = movementPoints[movementPoints.length - 1];
    var previousPoint;
    if (movementPoints.length > 2) {
        previousPoint = movementPoints[movementPoints.length - 2];
    }
    else {
        previousPoint = movementPoints[0];
    }

    //console.log("gst PreviousPoint Y " + previousPoint.y + "Curre " + currentPoint.y + " RIT Distance: " + dotLineLength(currentPoint.x + 50, currentPoint.y + 50, goalLines.right.xStart * fieldWidthRatio, goalLines.right.yStart * fieldHeightRatio, goalLines.right.xEnd * fieldWidthRatio, goalLines.right.yEnd * fieldHeightRatio, true));

    if ((dotLineLength(currentPoint.x, currentPoint.y, (goalLines.left.xStart) * fieldWidthRatio, (goalLines.left.yStart + 10) * fieldHeightRatio, goalLines.left.xEnd * fieldWidthRatio, (goalLines.left.yStart + 10) * fieldHeightRatio, true) < 10) && previousPoint.y > currentPoint.y) {
        netGoal = true;
    }
    if ((dotLineLength(currentPoint.x, currentPoint.y, (goalLines.right.xStart + 20) * fieldWidthRatio, (goalLines.right.yStart - 100) * fieldHeightRatio, goalLines.right.xEnd * fieldWidthRatio, (goalLines.right.yEnd - 100) * fieldHeightRatio, true) < 10) && previousPoint.y < currentPoint.y) {
        netGoal = true;
    }
    if ((dotLineLength(currentPoint.x, currentPoint.y, (goalLines.bottom.xStart - 80) * fieldWidthRatio, (goalLines.bottom.yStart) * fieldHeightRatio, (goalLines.bottom.xEnd - 80) * fieldWidthRatio, goalLines.bottom.yEnd * fieldHeightRatio, true) < 10) && previousPoint.x < currentPoint.x) {
        netGoal = true;
    }

    return netGoal;

}

function reachedGoal(ballPosition, movementPoints) {
    /*if(ballPosition.x + 100 > (goalPost.xStart * fieldWidthRatio)){
		//console.log("Ball "  + ballPosition.x);
		//console.log("Ball Y" + ballPosition.y);
		//console.log("XValue" + (goalPost.xStart * fieldWidthRatio));
		//console.log("Y Value Start " +(goalPost.yStart * fieldHeightRatio));
		//console.log("Y Value End " +(goalPost.yEnd * fieldHeightRatio));
	}*/
    if ((goalSideTouch(ballPosition, movementPoints)) || ((ballPosition.x > (goalLines.bottom.xStart * fieldWidthRatio)) && ((ballPosition.y > (goalLines.bottom.yStart * fieldHeightRatio)) && (ballPosition.y < (goalLines.bottom.yEnd * fieldHeightRatio))))) {
        console.log(ballPosition.y);
        goalReached = true;
        unlockSystem("System Unlocked");

        return true;
    }
    else {
        return false;
    }
}

function unlockSystem(message) {
    displayChoiceMenu();
    //resetGame();
    //window.external.executeCommand("unlockSystem", "");
    /*ctx.fillStyle=fontColor;
	ctx.strokeStyle = fontColor;
	var startSize = 72
	ctx.font = startSize + "px Segoe UI";
	ctx.textBaseline = "middle";
	ctx.textAlign = "Center";	
	var actualX = 350,actualYStart = 200, actualYEnd = 1000;
	ctx.save();
	ctx.translate(actualX*fieldWidthRatio, actualYStart*fieldHeightRatio);
	ctx.rotate(Math.PI/2);
	var requiredWidth = (actualYEnd - actualYStart) * fieldHeightRatio;
	while((ctx.measureText(message).width) > requiredWidth){
		startSize = startSize - 2;
		ctx.font = startSize + "px Segoe UI";
	};
	ctx.fillText(message,0,0);*/
    //TODO Something interesting for 25seconds
    document.getElementById("backgroundMusic").pause();
    document.getElementById("gameEndMusic").play();
}

function distance(start, destination) {
    var sum = ((destination.x - start.x) * (destination.x - start.x)) + ((destination.y - start.y) * (destination.y - start.y));
    return Math.sqrt(sum);
}

var dotLineLength = function (x, y, x0, y0, x1, y1, o) {
    function lineLength(x, y, x0, y0) {
        return Math.sqrt((x -= x0) * x + (y -= y0) * y);
    }
    if (o && !(o = function (x, y, x0, y0, x1, y1) {
      if (!(x1 - x0)) return { x: x0, y: y };
    else if (!(y1 - y0)) return { x: x, y: y0 };
      var left, tg = -1 / ((y1 - y0) / (x1 - x0));
      return { x: left = (x1 * (x * tg - y + y0) + x0 * (x * -tg + y - y1)) / (tg * (x1 - x0) + y0 - y1), y: tg * left - tg * x + y };
    }(x, y, x0, y0, x1, y1), o.x >= Math.min(x0, x1) && o.x <= Math.max(x0, x1) && o.y >= Math.min(y0, y1) && o.y <= Math.max(y0, y1))) {
        var l1 = lineLength(x, y, x0, y0), l2 = lineLength(x, y, x1, y1);
        return l1 > l2 ? l2 : l1;
    }
    else {
        var a = y0 - y1, b = x1 - x0, c = x0 * y1 - y0 * x1;
        return Math.abs(a * x + b * y + c) / Math.sqrt(a * a + b * b);
    }
};

function drawNetAndField() {
    ctx.drawImage(field, 0, 0, WIDTH, HEIGHT);
    netCtx.drawImage(net, 1525 * fieldWidthRatio, 240 * fieldHeightRatio, (1750 - 1525) * fieldWidthRatio, (870 - 260) * fieldHeightRatio);
    writeCenterText(centerText);
}

function calculateSlope(point1, point2) {
    return Math.atan((point2.y - point1.y) / (point2.x - point1.x));
}

function significantMouseMoved() {
    if (mouseMoveStack.length > 50) {
        var sumX = 0;
        var sumY = 0;
        for (i = (mouseMoveStack.length - 50) ; i < (mouseMoveStack.length) ; ++i) {
            sumX += mouseMoveStack[i].x;
            sumY += mouseMoveStack[i].y;
        }
        var avgX = sumX / 50;
        var avgY = sumY / 50;
        var avgPoint = new Object();
        avgPoint.x = avgX;
        avgPoint.y = avgY;

        var sumDistanceSquares = 0;
        for (i = (mouseMoveStack.length - 50) ; i < (mouseMoveStack.length) ; ++i) {
            sumDistanceSquares += ((avgPoint.x - mouseMoveStack[i].x) * (avgPoint.x - mouseMoveStack[i].x)) + ((avgPoint.y - mouseMoveStack[i].y) * (avgPoint.y - mouseMoveStack[i].y))
        }
        var standardDeviation = Math.sqrt((sumDistanceSquares / 50));
        //console.log("SD" + standardDeviation);
        if (standardDeviation > 40) {
            return true;
        }
        else {
            return false;
        }

    }
    else {
        return false;

    }
}







