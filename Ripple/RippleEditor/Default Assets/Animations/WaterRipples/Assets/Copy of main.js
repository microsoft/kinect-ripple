var floor = Math.floor;
var rain;
var QUALITY = 4;
var canvas, ctx, width, height, size, buffer0, buffer1, aux, i, texture;
var fishQueue = [];
var fishesInformation = [];
var fishCanvas;
var fishMoveCtx;
var fishMoveCanvas;
var fishImages = [];
var flowerCanvas;
var flowerImage;
var flowerCtx;
var flowerZones = [];
var flowerIndex = 0;
var mouseLastMoved = 0;
var mouseMoving = false;
var collisionTimer;
var goalReached = false;
var firstMove = true;
var centerText;
var fontColor;

function notifyWPF() {
    configureScreen("", "Microsoft IT", "#FFFFFF");
    //window.external.loadingDone();
}

function clamp(x, min, max) {
    if (x < min) return min;
    if (x > max) return max - 1;
    return x;
}

function getDataFromImage(img) {
    width = Math.floor(img.width / QUALITY);
    height = Math.floor(img.height / QUALITY);
    canvas.width = width;
    canvas.height = height;
    size = width * height;
    buffer0 = [];
    buffer1 = [];
    for (i = 0; i < size; i++) {
        buffer0.push(0);
        buffer1.push(0);
    }
    ////console.log("Width: "+ document.getElementById('c').width);
    //ctx.fillRect(0,0, width, height);
    ctx.clearRect(0, 0, width, height);
    ////console.log(img);
    ctx.drawImage(img, 0, 0, width, height);
    var fishCtx = document.getElementById("fishCanvas").getContext("2d");
    fishCtx.save();
    fishCtx.strokeStyle = fontColor;
    fishCtx.fillStyle = fontColor;
    fishCtx.globalAlpha = 0.8;
    fishCtx.font = "64px Segoe UI";
    fishCtx.translate((window.innerWidth / 2), (window.innerHeight / 2));
    fishCtx.textAlign = "center"
    for (l = 0; l < 15; l++) {
        fishCtx.fillText(centerText, 0, 0);
    }
    fishCtx.restore();
    return ctx.getImageData(0, 0, width, height);
}

function loadImage(sourceFunction, callback) {
    var img = document.createElement('img');
    img.onload = function () { callback(img); };
    img.src = sourceFunction();
    //img.src ="background.jpg";
}



function disturb(x, y, z) {
    if (x < 2 || x > width - 2 || y < 1 || y > height - 2)
        return;
    var i = x + y * width;
    buffer0[i] += z;
    buffer0[i - 1] -= z;
}

function process() {
    var img = ctx.getImageData(0, 0, width, height),
        data = img.data,
        i, x;

    // average cells to make the surface more even
    for (i = width + 1; i < size - width - 1; i += 2) {
        for (x = 1; x < width - 1; x++, i++) {
            buffer0[i] = (buffer0[i] + buffer0[i + 1] + buffer0[i - 1] + buffer0[i - width] + buffer0[i + width]) / 5;
        }
    }

    for (i = width + 1; i < size - width - 1; i += 2) {
        for (x = 1; x < width - 1; x++, i++) {
            // wave propagation
            var waveHeight = (buffer0[i - 1] + buffer0[i + 1] + buffer0[i + width] + buffer0[i - width]) / 2 - buffer1[i];
            buffer1[i] = waveHeight;
            // calculate index in the texture with some fake referaction
            var ti = i + floor((buffer1[i - 2] - waveHeight) * 0.08) + floor((buffer1[i - width] - waveHeight) * 0.08) * width;
            // clamping
            ti = ti < 0 ? 0 : ti > size ? size : ti;
            // some very fake lighting and caustics based on the wave height
            // and angle
            var light = waveHeight * 2.0 - buffer1[i - 2] * 0.6,
                i4 = i * 4,
                ti4 = ti * 4;
            // clamping
            light = light < -10 ? -10 : light > 100 ? 100 : light;
            data[i4] = texture.data[ti4] + light;
            data[i4 + 1] = texture.data[ti4 + 1] + light;
            data[i4 + 2] = texture.data[ti4 + 2] + light;
        }
    }
    // rain
    //disturb(floor(Math.random()*width), floor(Math.random()*height), Math.random()*10000);
    aux = buffer0;
    buffer0 = buffer1;
    buffer1 = aux;
    ctx.putImageData(img, 0, 0);
}

function getImageData() {
    return finalBackground;
}

function configureScreen(backgroundImage, centerMessage, textColor) {
    centerText = centerMessage;
    fontColor = textColor;
    init();
}

function init() {

    canvas = document.getElementById('c'),
		ctx = canvas.getContext('2d'),
		width = 577,
		height = 385,
		size = width * height,
		buffer0 = [],
		buffer1 = [],
		aux, i, texture;
    canvas.style.width = window.innerWidth + "px";
    canvas.style.height = window.innerHeight + "px";
    for (i = 0; i < size; i++) {
        buffer0.push(0);
        buffer1.push(0);
    }

    loadImage(getImageData, function (img) {
        texture = getDataFromImage(img);
        canvas.width = width;
        canvas.height = height;
        ctx.fillRect(0, 0, width, height);
        setInterval(process, 1000 / 60);
        rain = setInterval(function () {
            disturb(floor(Math.random() * width), floor(Math.random() * height), Math.random() * 10000);
        }, 100);

    });

    setTimeout(function () {
        mouseMoving = false;
        document.getElementById('rippleSound').pause();
        document.getElementById('rainSound').play();
    }, 100);
    setInterval(function () {
        //console.log(rain);
        if (rain == null) {
            document.getElementById('rainSound').play();
            document.getElementById('rippleSound').pause();
            //console.log("NULLLLLLL");
            rain = setInterval(function () {
                disturb(floor(Math.random() * width), floor(Math.random() * height), Math.random() * 10000);
            }, 1000);
        }
    }, 6000);

    setInterval(function () {
        //console.log(rain);
        if (rain == null) {
            document.getElementById('rippleSound').pause();
        }
    }, 500);
    //alert('lol');
    fishCanvas = document.getElementById("fishCanvas");
    fishMoveCanvas = document.getElementById("fishMoveCanvas");
    fishCanvas.style.width = window.innerWidth + "px";;
    fishCanvas.style.height = window.innerHeight + "px";
    fishCanvas.width = window.innerWidth;
    fishCanvas.height = window.innerHeight;
    fishMoveCanvas.style.width = window.innerWidth + "px";;
    fishMoveCanvas.style.height = window.innerHeight + "px";
    fishMoveCanvas.width = window.innerWidth;
    fishMoveCanvas.height = window.innerHeight;
    fishMoveCtx = fishMoveCanvas.getContext("2d");
    var background_images = [];
    for (i = 1; i <= 6; ++i) {
        background_images[i - 1] = document.createElement('img');
        background_images[i - 1].src = "Assets/" + i + ".png";
    }
    fCtx = fishCanvas.getContext("2d");
    fCtx.globalAlpha = 0.8;
    var fWidth = window.innerWidth;
    var fHeight = window.innerHeight;
    var floorWidthRatio = fWidth / 1366;
    var floorHeightRatio = fHeight / 768;
    setTimeout(function () {
        fCtx.drawImage(background_images[0], 0.25 * fWidth, 0.3 * fHeight, background_images[0].width*floorWidthRatio, background_images[0].height*floorHeightRatio);
        fCtx.drawImage(background_images[1], 0.15 * fWidth, 0.6 * fHeight, background_images[1].width*floorWidthRatio, background_images[1].height*floorHeightRatio);
        fCtx.drawImage(background_images[2], 0.65 * fWidth, 0.7 * fHeight, background_images[2].width*floorWidthRatio, background_images[2].height*floorHeightRatio);
        fCtx.drawImage(background_images[3], 0.5 * fWidth, 0.35 * fHeight, background_images[3].width*floorWidthRatio, background_images[3].height*floorHeightRatio);
        fCtx.drawImage(background_images[4], 0.75 * fWidth, 0.35 * fHeight, background_images[4].width*floorWidthRatio, background_images[4].height*floorHeightRatio);
        fCtx.drawImage(background_images[5], 0.8 * fWidth, 0.2 * fHeight, background_images[5].width*floorWidthRatio, background_images[5].height*floorHeightRatio);
    }, 800);
    //console.log(fishCanvas.width);
    var images = ["Assets/red_fish.png", "Assets/white_fish.png", "Assets/yellow_fish.png"];
    for (i = 0; i < images.length; ++i) {
        fishImages[i] = document.createElement("img");
        fishImages[i].src = images[i];
    }


    setTimeout(function () {
        for (i = 0; i < (4 + Math.floor(Math.random() * 3)) ; ++i) {
            var reverse = (Math.random() > 0.5);
            var multiplier = -1;
            if (reverse) {
                multiplier = 1;
            }
            //console.log("Number" + Math.floor(-0.0000000000000001 + (Math.random() * fishImages.length)));
            fishesInformation.push(createNewFish(createPoint(100 + (Math.random() * (window.innerWidth - 100)), 100 + (Math.random() * (window.innerHeight - 100))), fishImages[Math.floor(-0.0000000000000001 + (Math.random() * fishImages.length))], reverse, fishesInformation.length, multiplier));
            drawFishImage(fishesInformation[fishesInformation.length - 1]);
        }
    }, 1000)



    setInterval(function () {
        move();
    }, 30);


    setInterval(function () {
        if ((new Date().getTime() - mouseLastMoved) > 1000) {
            mouseMoving = false;
        }
    }, 1000);

    //flowerCanvas 
    flowerCanvas = document.getElementById("flowerCanvas");
    flowerCanvas.style.width = window.innerWidth + "px";;
    flowerCanvas.style.height = window.innerHeight + "px";
    flowerCanvas.width = window.innerWidth;
    flowerCanvas.height = window.innerHeight;
    flowerCtx = flowerCanvas.getContext("2d");
    //flowerZones
    flowerImage = new Object();
    flowerImage.unlocked = document.createElement("img");
    flowerImage.unlocked.src = unlockedFlower;

    flowerImage.locked = document.createElement("img");
    flowerImage.locked.src = lockedFlower;

    var numberOfZones = 3;
    for (i = 0; i < numberOfZones; ++i) {
        var zone = new Object();
        zone.xStart = 150 + (i * ((window.innerWidth - 150) / numberOfZones));
        zone.yStart = 125;
        zone.xEnd = (i + 1) * ((window.innerWidth - 150) / numberOfZones);
        zone.yEnd = (window.innerHeight) - 125;
        zone.state = "undrawn";
        flowerZones.push(zone);
        //console.log(zone);
    }
    drawLockedFlower(flowerZones[0]);
    /*flowerImage.locked.onload = function () {
	    //for (i = 0; i < numberOfZones; ++i) {
	        drawLockedFlower(flowerZones[0]);
	    //}
	};*/
    collisionTimer = initializeTimer();
    flowerCanvas.onmousemove = function (e) {
        /*if (firstMove) {
            //flowerImage.locked.onload = function () {
            //for (i = 0; i < numberOfZones; ++i) {
            drawLockedFlower(flowerZones[0]);
            //}
            //};
            firstMove = false;
            //alert("lol");
        }*/
        var mouseX = e.clientX;
        var mouseY = e.clientY;
        var point = new Object();
        point.x = mouseX;
        point.y = mouseY;
        if (!goalReached && inCollisionZone(point)) {
            if (!collisionTimer.running) {
                collisionTimer.startTimer();
            }
            else {
                changeFlowerColor(flowerZones[flowerIndex - 1], (collisionTimer.getTime() / 1300), flowerImage.unlocked);
            }
        }
        else {
            if (collisionTimer.running) {
                collisionTimer.stopTimer();
                //console.log("Changing COlor");
                changeFlowerColor(flowerZones[flowerIndex - 1], 1, flowerImage.locked);
                flowerZones[flowerIndex - 1].state = "drawn";
            }
        }
        //From Lower Canvas
        mouseLastMoved = mouseLastMoved = new Date().getTime();
        mouseMoving = true;
        //if(Math.random() < (1/2)){
        disturb(
                floor(e.clientX / innerWidth * width),
                floor(e.clientY / innerHeight * height),
                15000);
        clearInterval(rain);
        document.getElementById('rainSound').pause();
        document.getElementById('rippleSound').play();
        rain = null;
        //}
        fishPoint = new Object();
        fishPoint.x = e.clientX;
        fishPoint.y = e.clientY;
        fishQueue.push(fishPoint);
    };

    //Manages all flower colours and transitions
    var leavesManager = setInterval(function () {
        if (collisionTimer.running && collisionTimer.getTime() > 1300) {
            //console.log("LEAF UNLOCKED");
            //changeFlowerColor(flowerZones[flowerIndex-1]);
            collisionTimer.stopTimer();
            document.getElementById("flowerUnlockSound").play();
            var currentZone = flowerZones[flowerIndex - 1];
            currentZone.state = "unlocked";
            if (flowerZones.length > flowerIndex) {
                drawLockedFlower(flowerZones[flowerIndex]);
            }
            else {
                goalReached = true;
                unlockSystem("System Unlocked");
            }

        }
        else if (collisionTimer.running) {
            changeFlowerColor(flowerZones[flowerIndex - 1], collisionTimer.getTime() / 1300, flowerImage.unlocked);
        }

    }, 10);

    var blinkerManager = setInterval(function () {
        if (!collisionTimer.running) {
            var currentZone = flowerZones[flowerIndex - 1];

            if (flowerZones.length > 0 && (currentZone.state == "drawn" || currentZone.state == "invisible")) {

                currentZone = flowerZones[flowerIndex - 1];
                if (currentZone.state == "drawn") {
                   
                    flowerCtx.save();
                    //flowerCtx.strokeRect(xtr-75, ytr-75, 150, 150);
                    flowerCtx.translate(currentZone.xTranslated, currentZone.yTranslated);
                    flowerCtx.rotate(currentZone.angle);
                    var scrMul = (window.innerWidth / 1200);
                    flowerCtx.clearRect(-100 * scrMul, -100 * scrMul, 200 * scrMul, 200 * scrMul);
                    flowerCtx.restore();
                    currentZone.state = "invisible";
                }
                else if (currentZone.state == "invisible") {
                    changeFlowerColor(currentZone, 1, flowerImage.locked);
                    currentZone.state = "drawn";
                }
            }
        }
    }, 500);

}

function createNewFish(point, image, directionReverse, index, multiplier) {
    var fishInformation = new Object();
    fishInformation.position = point;
    fishInformation.destination = point;
    fishInformation.moving = false;
    fishInformation.image = image;
    fishInformation.reverse = directionReverse;
    fishInformation.multiplier = multiplier;
    fishInformation.imageNumber = 0;
    var movementFunction = function () {
        var fishInformation = fishesInformation[index];
        if (!fishInformation.moving) {
            checkScreenBoundaries(fishInformation);
            //console.log(fishInformation.multiplier);
            var x1;
            if (fishInformation.multiplier == -1) {
                x1 = fishInformation.position.x - ((12 + Math.floor((Math.random() * 7))));
            }
            else {
                x1 = fishInformation.position.x + ((12 + Math.floor((Math.random() * 7))));

            }
            var y1 = fishInformation.position.y + ((Math.random() > (1 / 2) ? 1 : -1) * (Math.floor((Math.random() * 10))));
            //console.log(fishInformation.multiplier * (5 + Math.floor((Math.random() * 10))));
            //console.log("X: " + x1 + " Y: " + y1);
            var newPoint = createPoint(x1, y1);
            /*console.log("NOT MOVING");
            console.log(fishInformation.position);
            console.log(newPoint);*/
            fishInformation.destination = newPoint;
        }
    };
    fishInformation.intervalSetter = setInterval(movementFunction, 20);
    setInterval(function () {
        fishInformation.imageNumber++;
    }, 250);
    return fishInformation;
}

function checkScreenBoundaries(fishInformation) {
    var x1 = fishInformation.position.x;
    var y1 = fishInformation.position.y;
    var inBoundary = true;
    if (x1 > (window.innerWidth + 250)) {
        x1 = 200;
        fishInformation.position.x = x1;
        inBoundary = false;
    }
    if (x1 < -250) {
        x1 = window.innerWidth - 100;
        fishInformation.position.x = x1;
        //fishInformation.reverse = !fishInformation.reverse;
        //fishInformation.multiplier = fishInformation.multiplier*-1;
        inBoundary = false;
    }
    if (y1 < -50) {
        y1 = window.innerHeight - 100;
        fishInformation.position.y = y1;
        inBoundary = false;
    }
    if (y1 > (window.innerHeight + 50)) {
        y1 = 100;
        fishInformation.position.y = y1;
        inBoundary = false;
    }
    //console.log("LLLLLLLLLL" + fishInformation.position);
    if (!inBoundary) {
        fishInformation.moving = false;
    }
    //console.log(inBoundary);
    return inBoundary;
}

function drawFishImage(fishInformation) {
    //fishMoveCtx.drawImage(fishInformation.image,1200,384,600,384,fishInformation.position.x,fishInformation.position.y,150,100);
    var number = ((fishInformation.imageNumber) % 4);
    var row = Math.floor(number / 4);
    var column = number % 4;
    var width = 150;
    var height = 50;
    //console.log("Row: " + row + "Column: "+ column);
    //fishMoveCtx.fillRect(Math.floor(fishInformation.x),Math.floor(fishInformation.y), width, height);
    var sX = (column * 575);
    var sY = (row * 221);

    if (fishInformation.reverse) {
        fishMoveCtx.save();
        fishMoveCtx.translate(fishInformation.position.x, fishInformation.position.y);
        fishMoveCtx.rotate(-Math.PI);
        fishMoveCtx.translate(100, 50);
        fishMoveCtx.drawImage(fishInformation.image, sX, sY, 575, 221, -100, -50, width, height);
        fishMoveCtx.restore();

    }
    else {
        fishMoveCtx.save();
        //fishMoveCtx.rotate(-Math.PI + (Math.random() > 1 / 2 ? 1 : -1) * Math.random(Math.PI / 6));
        fishMoveCtx.drawImage(fishInformation.image, sX, sY, 575, 221, fishInformation.position.x, fishInformation.position.y, width, height);
        fishMoveCtx.restore();

    }
}


function createPoint(x, y) {
    var point = new Object();
    point.x = x;
    point.y = y;
    return point
}

function unlockSystem(message) {
    flowerCanvas.width = flowerCanvas.width;
    document.getElementById("backgroundSound").pause();
    window.external.executeCommand("unlockSystem", "");
    /*document.getElementById("unlockComplete").play();

    var fontColor = "#ffffff";
    flowerCtx.fillStyle = fontColor;
    flowerCtx.strokeStyle = fontColor;
    var startSize = 72
    flowerCtx.font = startSize + "px Segoe UI";
    flowerCtx.textBaseline = "middle";
    flowerCtx.textAlign = "Center";
    var actualX = flowerZones[flowerZones.length-1].xTranslated + 125, actualYStart = 125, actualYEnd = window.innerHeight - 150;
    flowerCtx.save();
    flowerCtx.translate(actualX ,actualYStart);
    flowerCtx.rotate(Math.PI / 2);
    var requiredWidth = (actualYEnd - actualYStart);
    while ((ctx.measureText(message).width) > requiredWidth) {
        startSize = startSize - 2;
        flowerCtx.font = startSize + "px Segoe UI";
    };
    flowerCtx.fillText(message, 0, 0);*/
}

function inCollisionZone(point) {
    var currentZone = flowerZones[flowerIndex - 1];
    var scrMul = window.innerWidth / 1200;
    if (flowerIndex > 0 && ((point.x > (currentZone.xTranslated - (125*scrMul))) && (point.x < (currentZone.xTranslated + (125*scrMul)))) && ((point.y > (currentZone.yTranslated - (100*scrMul))) && (point.y < (currentZone.yTranslated + 100*scrMul)))) {
        //console.log("ZONED");
        //flowerCtx.strokeRect(currentZone.xTranslated - (100 * scrMul), currentZone.yTranslated - (100 * scrMul),200*scrMul,200*scrMul);
        if (currentZone.state == "drawn" || currentZone.state == "invisible") {
            currentZone.state = "steppedOn";
            changeFlowerColor(currentZone, 1, flowerImage.locked);
        }
        return true;
    }
    else {
        return false;
    }
}

function changeFlowerColor(zone, globalAlpha, image) {
    flowerCtx.save();
    //flowerCtx.strokeRect(xtr-75, ytr-75, 150, 150);
    flowerCtx.translate(zone.xTranslated, zone.yTranslated);
    flowerCtx.rotate(zone.angle);
    //flowerCtx.clearRect(-75, -75, 150, 150);
    if (image.src == flowerImage.unlocked.src) {
        if (collisionTimer.getTime() > 1000) {
            flowerCtx.globalAlpha = globalAlpha / 10;
        }
        else if (collisionTimer.getTime() > 1500) {
            flowerCtx.globalAlpha = globalAlpha;
        }
        else {
            flowerCtx.globalAlpha = globalAlpha / 100;
        }
    }
    else {
        flowerCtx.globalAlpha = 1;
    }
    //console.log(globalAlpha);
    var scrMul = (window.innerWidth / 1200);
    flowerCtx.drawImage(image, -100 * scrMul, -100 * scrMul, 200 * scrMul, 150 * scrMul);
    flowerCtx.restore();
}

function drawLockedFlower(zone) {
    //flowerCtx.fillRect(zone.xStart, zone.yStart, zone.xEnd - zone.xStart, zone.yEnd - zone.yStart);

    flowerCtx.save();
    var xtr = zone.xStart + (Math.random() * ((zone.xEnd - zone.xStart) / 2));
    var ytr = zone.yStart + (Math.random() * (zone.yEnd - zone.yStart));
    var scrMul = (window.innerWidth / 1200);
    
    flowerCtx.translate(xtr, ytr);
    zone.angle = Math.random() * Math.PI * 2;
    flowerCtx.rotate(zone.angle);
    //flowerCtx.strokeRect(- 100, - 100, 200 * scrMul, 200 * scrMul);
    flowerCtx.drawImage(flowerImage.locked, -100 * scrMul, -100 * scrMul, 200 * scrMul, 150 * scrMul);
    zone.xTranslated = xtr;
    zone.yTranslated = ytr;
    zone.state = "drawn";
    flowerCtx.restore();
    if (flowerIndex == 0) {
         window.external.executeCommand("playAudio", "Please Step on the first blinking flower");
    }
    else {
         window.external.executeCommand("playAudio", "Step on the next now");
    }
    flowerIndex++;
}

function move() {
    fishMoveCanvas.width = fishMoveCanvas.width;
    //console.log(fishInformation);
    for (i = 0; i < fishesInformation.length; ++i) {
        fishInformation = fishesInformation[i];
        //console.log(fishInformation);
        fishInformation.moving = true;
        var speed = 12 + Math.random() * 3;
        var start = fishInformation.position;
        var destination = fishInformation.destination;
        if (checkScreenBoundaries(fishInformation) && distance(start, destination) > speed) {
            start = fishInformation.position;
            angle = Math.atan((destination.y - start.y) / Math.abs((destination.x - start.x)));

            start.x = (start.x + ((fishInformation.multiplier) * ((speed) * (Math.cos(angle)))));
            start.y = (start.y + ((speed) * (Math.sin(angle))));
            fishInformation.position.x = start.x;
            fishInformation.position.y = start.y;
            drawFishImage(fishInformation);
        }
        else {
            checkScreenBoundaries(fishInformation);
            drawFishImage(fishInformation);
            fishInformation.position = destination;
            fishInformation.moving = false;
        }
    }
}

function distance(start, destination) {
    var sum = ((destination.x - start.x) * (destination.x - start.x)) + ((destination.y - start.y) * (destination.y - start.y));
    return Math.sqrt(sum);
}

function initializeTimer() {
    var timer = new Object();
    timer.actualObject = null;
    timer.value = 1;
    timer.running = false;
    timer.startTimer = function () {
        timer.running = true;
        timer.value = 1;
        timer.actualObject = setInterval(function () { timer.value += 100 }, 100)
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


