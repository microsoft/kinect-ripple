var surface, ctx;
var wR, hR;
var currentAngle = 0;
var targetAngle = 0;
var angle = 0;
var power = 3;
var lastThreshold = 120;
var totalNumbers = 6;
var winningNumber = 0;
var wheelState = "reset";

function executeCommandFromFloor(commandParameters) {
    var parameters = commandParameters.split(",");
    console.log(parameters[0]);
    switch (parameters[0]) {
        case "playerMoved":
            //console.log("YES");
            wheelState = "reset";
            break;
        case "moveWheel":
            doRotation();
            break;
    }
}

function sendCommandToFloor(commandName, commandParameters) {
    
}


function initializeScreen() {
    surface = document.getElementById("rotatingWheel");
    surface.style.width = window.innerWidth;
    surface.style.height = window.innerWidth;
    surface.width = window.innerWidth;
    surface.height = window.innerHeight;
    wR = (window.innerWidth / 1366);
    hR = (window.innerHeight / 578);
    ctx = surface.getContext("2d");
    clearSurface();
    drawWheel(0);
}

function clearSurface() {
    surface.width = surface.width;
    ctx.fillRect(0, 0, surface.width, surface.height);
}

function drawStand() {
    ctx.beginPath();
    ctx.save();
    ctx.moveTo(((surface.width / 2)-(wR*30)), 3);
    ctx.lineTo((surface.width / 2), hR * 100);
    ctx.lineTo(((surface.width / 2) + (30*wR)), 3);
    ctx.closePath();
    ctx.fillStyle = "#E6D9AE";
    ctx.strokeStyle = "#993300";
    ctx.lineWidth = 5;
    ctx.stroke();
    ctx.fill();
    ctx.restore();
}

function drawWheel(degrees) {
    clearSurface();
    ctx.save();

    // Translate to the center point of our image.
    ctx.translate(window.innerWidth/2, window.innerHeight/2);

    // Perform the rotation by the angle specified in the global variable (will be 0 the first time).
    ctx.rotate(DegToRad(degrees));

    // Translate back to the top left of our image.
    ctx.translate(-window.innerWidth * 0.5, -window.innerHeight * 0.5);

    totalNumbers = 6;
    var colors = ["#f00", "#0f0", "#00f", "#ff0", "#0ff", "#f0f","#f41","#f92"];
    var radius = (window.innerHeight / 2 - (50 * hR));
    var start = 0;
    for (i = 0; i < totalNumbers; ++i) {
      ctx.strokeStyle = "#fff";
      ctx.fillStyle=colors[i]; 
      ctx.beginPath();
      ctx.arc(window.innerWidth/2, window.innerHeight/2, radius,start, start+Math.PI*2*(1/totalNumbers),false);
      ctx.lineTo(window.innerWidth / 2, window.innerHeight / 2);
      ctx.closePath();
      ctx.fill();
      ctx.lineWidth = 5;
      ctx.stroke();
      ctx.save();
      ctx.fillStyle = "#fff";
      var fontSize = (wR) * 100;
      ctx.font = Math.floor(fontSize) + "px" + " bold Segoe UI";
      var textAngle = start + ((Math.PI * 2 * (1 / totalNumbers)) / 2);
      var textX = (window.innerWidth / 2) + (radius / 2) * Math.cos(textAngle) + Math.cos(textAngle-90) * (hR * 18);
      var textY = (window.innerHeight / 2) + (radius / 2) * Math.sin(textAngle) + Math.sin(textAngle-90)* (hR * 18);
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
    ctx.arc(window.innerWidth / 2, window.innerHeight / 2, (50 * hR), 0, DegToRad(360), false);
    ctx.closePath();
    ctx.strokeStyle = "#fff";
    ctx.fill();


    ctx.strokeStyle = "#fff";
    ctx.moveTo(window.innerWidth / 2 - (50 * hR), window.innerHeight / 2);
    ctx.lineTo(window.innerWidth / 2 + (50 * hR), window.innerHeight / 2);
    ctx.lineWidth = 5;
    ctx.stroke();
    ctx.moveTo(window.innerWidth / 2, window.innerHeight / 2 - (50 * hR));
    ctx.lineTo( window.innerWidth / 2,window.innerHeight / 2 + (50 * hR));
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
            angle = 55;
        else if (angleRemaining > 5000)
            angle = 45;
        else if (angleRemaining > 4000)
            angle = 30;
        else if (angleRemaining > 2500)
            angle = 25;
        else if (angleRemaining > 1800)
            angle = 15;
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
        winningNumber = num
        console.log(winningNumber);
        wheelState = "reset";
        resetWheel();
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
        startRotation();
    });
}

