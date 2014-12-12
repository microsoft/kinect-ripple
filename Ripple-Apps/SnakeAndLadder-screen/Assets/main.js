var surface, ctx;
var wR, hR;
var currentAngle = 0;
var targetAngle = 0;
var angle = 0;
var power = 0.5;
var lastThreshold = 120;
var totalNumbers = 6;
var winningNumber = 0;
var wheelState = "reset";
var wheelCenter;
var img;
var wR;
var hR;

function executeCommandFromFloor(commandParameters) {
    var parameters = commandParameters.split(",");
	//alert(commandParameters);
    //console.log(parameters[0]);
    switch (parameters[0]) {
        case "updateInstruction":
            //alert(parameters[1]);
            document.getElementById("changing-instruction").innerHTML = parameters[1];
            break;
        case "moveWheel":
            startRotation();
            break;
        case "gameComplete":
            wheelState = "stop";
            break;
    }
}

function sendMessageToFloor(commandName, commandParameters) {
    // console.log(commandParameters[0]);
    var commandParametersCSV = "";
    for (i = 0; i < commandParameters.length; ++i) {
        commandParametersCSV = commandParametersCSV.concat(",", commandParameters[i]);
    }

    commandParametersCSV = commandName.concat(commandParametersCSV);
    //console.log(commandParametersCSV);
    switch (commandName) {
        case "numberRecieved":
            //console.log(commandParametersCSV);
            window.external.executeCommand("sendCommandToBottomFloor", commandParametersCSV);
            break;
    }
}


function initializeScreen() {
    surface = document.getElementById("rotatingWheel");
    surface.style.width = window.innerWidth;
    surface.style.height = window.innerWidth;
    surface.width = window.innerWidth;
    surface.height = window.innerHeight;
    ctx = surface.getContext("2d");
    wheelCenter = new Object();
    wheelCenter.x = 0;
    wheelCenter.y = 0;

    img = new Image();
    img.src = "Assets/snakesLadderBG.png";
    img.onload = function () {
        hR = (window.innerHeight / img.height);
        wR = (window.innerWidth / img.width);
        ctx.drawImage(img, 0, 0, surface.width, surface.height);
        drawWheel(0);

    };
    setInterval(function () {
        var color = document.getElementById("changing-instruction").style.color;
        console.log(color);
        if (color == "rgb(255, 255, 0)") {
            document.getElementById("changing-instruction").style.color = "rgb(255,255,255)";
        }
        else {
            document.getElementById("changing-instruction").style.color = "rgb(255,255,0)";
        }
    }, 500);
    clearSurface();
}

function writeTitle(title) {
    var fontSize = 56;
    ctx.save();
    fontSize = Math.floor(wR * fontSize);
    ctx.font = fontSize + "px Segoe UI Light";
    ctx.fillStyle = "#fff";
    ctx.strokeStyle = "#fff";
    ctx.fillText(title, 320 * wR, 220 * hR);
    ctx.moveTo(320 * wR, (230)* hR);
    ctx.lineTo((320 * wR) + ctx.measureText(title).width, (230) * hR);
    ctx.stroke();
    ctx.restore();
}

function clearSurface() {
    surface.width = surface.width;
    ctx.fillRect(0, 0, surface.width, surface.height);
    ctx.drawImage(img, 0, 0, surface.width, surface.height);
    writeTitle("Snakes & Ladder");
}

function drawStand() {
    ctx.save();
    ctx.beginPath();
    ctx.moveTo(((wheelCenter.x)-(wR*40)), 3);
    ctx.lineTo((wheelCenter.x), hR * 120);
    ctx.lineTo(((wheelCenter.x) + (40 * wR)), 3);
    ctx.closePath();
    ctx.fillStyle = "#E6D9AE";
    ctx.strokeStyle = "#993300";
    ctx.lineWidth = 5;
    ctx.stroke();
    ctx.fill();
    ctx.restore();
}

function drawWheel(degrees) {
    var radius = (window.innerWidth / 4 - (40 * hR));
    while (radius * 2 > (window.innerHeight-(100*hR))) {
        radius = radius - 10;
    }
    wheelCenter.x = window.innerWidth / 2 + radius;
    wheelCenter.y = window.innerHeight / 2;
    while (wheelCenter.y > (radius + 100)) {
        wheelCenter.y -= 10;
    }
    clearSurface();
    ctx.save();

    // Translate to the center point of our image.
    ctx.translate(wheelCenter.x, wheelCenter.y);

    // Perform the rotation by the angle specified in the global variable (will be 0 the first time).
    ctx.rotate(DegToRad(degrees));

    // Translate back to the top left of our image.
    ctx.translate(-wheelCenter.x, -wheelCenter.y);

    totalNumbers = 6;
    var colors = ["rgb(174,18,167)", "rgb(0,158,73)", "rgb(255,130,0)", "rgb(112,48,160)", "rgb(0,174,239)", "rgb(255,192,0)", "rgb(0,158,73)", "rgb(104,33,122)"];
    var start = 0;
    for (i = 0; i < totalNumbers; ++i) {
      ctx.strokeStyle = "#fff";
      ctx.fillStyle=colors[i]; 
      ctx.beginPath();
      ctx.arc(wheelCenter.x , wheelCenter.y, radius,start, start+Math.PI*2*(1/totalNumbers),false);
      ctx.lineTo(wheelCenter.x, wheelCenter.y);
      ctx.closePath();
      ctx.fill();
      ctx.lineWidth = 5;
      ctx.stroke();
      ctx.save();
      ctx.fillStyle = "#fff";
      var fontSize = (wR) * 150;
      ctx.font = Math.floor(fontSize) + "px" + " bold Segoe UI";
      var textAngle = start + ((Math.PI * 2 * (1 / totalNumbers)) / 2);
      var textX = (wheelCenter.x) + (radius / 2) * Math.cos(textAngle) + Math.cos(textAngle-90) * (hR * 18);
      var textY = (wheelCenter.y) + (radius / 2) * Math.sin(textAngle) + Math.sin(textAngle-90)* (hR * 18);
      var number = ((i + 2) % totalNumbers) + 1 + "";
      ctx.translate(textX, textY);
      ctx.rotate(textAngle+DegToRad(90));
      ctx.translate(-textX, - textY);
      ctx.fillText(number, textX, textY);
      ctx.restore();
      start+=Math.PI*2*(1/totalNumbers); 
    }
    ctx.save();
    ctx.fillStyle = "#000";
    ctx.beginPath();
    ctx.arc(wheelCenter.x, wheelCenter.y, (50 * hR), 0, DegToRad(360), false);
    ctx.closePath();
    ctx.strokeStyle = "#fff";
    ctx.fill();


    ctx.strokeStyle = "#fff";
    ctx.moveTo(wheelCenter.x - (50 * hR), wheelCenter.y);
    ctx.lineTo(wheelCenter.x + (50 * hR), wheelCenter.y);
    ctx.lineWidth = 5;
    ctx.stroke();
    ctx.moveTo(wheelCenter.x, wheelCenter.y - (50 * hR));
    ctx.lineTo( wheelCenter.x,wheelCenter.y + (50 * hR));
    ctx.stroke();

    ctx.restore();
    ctx.restore();
    drawStand();
}

function startRotation() {
    if (wheelState == "reset") {
        drawWheel(0);
        stopAngle = Math.floor(Math.random() * 360);
        if ((stopAngle + 30) % 60 < 5) {
            stopAngle += 10;
        }
        //console.log(stopAngle);
        targetAngle = (360 * (power * 6) + stopAngle);
        lastThreshold = Math.floor(90 + (Math.random() * 90));
        doRotation();
    }
}

function doRotation() {
    wheelState = "spinning";
    drawWheel(currentAngle);
    currentAngle += angle;
    if (currentAngle < targetAngle) {
        var angleRemaining = (targetAngle - currentAngle);
        if (angleRemaining > 6480)
            angle = 125;
        else if (angleRemaining > 5000)
            angle = 95;
        else if (angleRemaining > 4000)
            angle = 85;
        else if (angleRemaining > 2500)
            angle = 55;
        else if (angleRemaining > 1800)
            angle = 35;
        else if (angleRemaining > 900)
            angle = 11.25;
        else if (angleRemaining > 400)
            angle = 7.5;
        else if (angleRemaining > 220)
            angle = 3.80;
        else if (angleRemaining > lastThreshold)
            angle = 1.90;
        else
            angle = 1;
        setTimeout(function () {
            doRotation();
        }, 30);
    }
    else {
        //console.log(stopAngle);
        var num = (Math.ceil(((360-(stopAngle+((360/(totalNumbers*2)))))%360)/(360/totalNumbers))); //Works only for 6 numbers
        if (num == totalNumbers - 1) {
            num = num + 1;
        }
        else {
            num = (num + 1) % totalNumbers;
        }
        winningNumber = num;
        wheelState = "reset";
        resetWheel();
        sendMessageToFloor("numberRecieved", "" + winningNumber);
    }
}

function resetWheel() {
    currentAngle = 0;
    angle = 0;
    targetAngle = 0;
}

function DegToRad(d) {
    return d * 0.0174532925199432957;
}

window.onload = function () {
    initializeScreen();
    surface.addEventListener("click", function () {
        //startRotation();
    });
}

