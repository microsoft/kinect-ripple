var enemyYPositions = [];	//empty square brackets means new empty array
var enemyXPositions = [];
var enemies = [];
var playerX = 0;
var playerY = 0;
//var avatarImage;
//var enemyImage;
var ticksSurvived = 0;
var mostTicksSurvived = 0;
var canvasWidth;
var canvasHeight;
var level = 0;
var enemiesLeft;
var gameDone = false;
var exitTimer;
var resetTimer;

function setUpGame() {
	
	canvasWidth = window.innerWidth;
	canvasHeight = window.innerHeight;
	level+=1;
	document.getElementById("gameCanvas").width = canvasWidth;
	document.getElementById("gameCanvas").height = canvasHeight;
	document.getElementById("gameCanvas").style.width = canvasWidth;
	document.getElementById("gameCanvas").style.height = canvasHeight;
	document.getElementById("soundControl").play();
	var gameCanvas = document.getElementById("gameCanvas");
	resetTimer = initializeTimer();
	exitTimer = initializeTimer();
	startGame();
	gameCanvas.addEventListener("mousemove", handleMouseMovement);
	setInterval(handleTick, 100);
}

function startGame() {
    enemiesLeft = 7;
    gameDone = false;
    enemies = [];
    var ctx = gameCanvas.getContext("2d");
    ctx.fillStyle = "#000000";
    ctx.fillRect(0, 0, canvasWidth, canvasHeight);
    PlayerCord = getOffsetCoord();
    EnemyCoord = getOffsetCoord();
    drawPlayer(PlayerCord.x, PlayerCord.y);
    document.getElementById("backgroundMusic").play();
    addEnemy();
}

function addEnemy(){
	var enemy = new Object();
	enemy.y = 70+(Math.random() * (canvasHeight-120));
	enemy.x = canvasWidth;
	enemy.speed = 1 + (Math.random()*2);
	enemy.visible = true;
	if(Math.random() > 0.5){
		enemy.faceColor = "#FF0000";
	}
	else{
		enemy.faceColor = "#0000FF";
		enemy.speed += 2;
	}
	enemies.push(enemy);
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

function getOffsetCoord(){
	var coord = new Object();
	var x = Math.random() * (canvasWidth);
	if(x<100){
		x+=100;
	}
	else if(x>(canvasWidth-100)){
		x-=100;
	}
	coord.x = x;
	var y = Math.random() * (canvasHeight);
	if(y<100){
		y+=100;
	}
	else if(x>(canvasHeight-100)){
		y-=100;
	}
	coord.y = y;
	return coord;
}

function notifyWPF() {
    setUpGame();
    window.external.loadingDone();
}

function configureScreen(backgroundImage, centerText, fontColor) {
    //setUpGame();
}

function startNewGame() {
	enemies = [];
	enemyXPositions = [];
	enemyYPositions = [];
	ticksSurvived = 0;
	level = 1;
}

function handleMouseMovement(mouseEvent) {
	playerX = mouseEvent.offsetX;
	playerY = mouseEvent.offsetY;
}

function drawPlayer(x, y){
	var radius = 60 * (window.innerHeight/800);
	var gameCanvas = document.getElementById("gameCanvas");
	var context = gameCanvas.getContext("2d");
	
	context.strokeStyle = '#0000FF';
    context.fillStyle = '#FFFF00';
    context.lineWidth = 4;
    context.beginPath();
	//100,75
    context.arc(x,y,radius,0*Math.PI,2*Math.PI,true);
    context.closePath();
    context.stroke();
    context.fill();
 
 
    // The Smile
    context.strokeStyle = '#FF0000';
    context.lineWidth = 2;
    context.beginPath();
    context.arc(x+20,y,radius-5,0.75*Math.PI,1.25*Math.PI,false); 
    context.stroke();
       
 
    // The Left eye
    context.strokeStyle = '#000000';
    context.fillStyle = '#000000';
    context.beginPath();
    context.arc(x+15,y+20,radius-48,0*Math.PI,2*Math.PI,false);
    context.closePath();
    context.stroke();
    context.fill();
 
    // The Right Eye
    context.strokeStyle = '#000000';
    context.fillStyle = '#000000';
    context.beginPath();
    context.arc(x+15,y-20,radius-48,0*Math.PI,2*Math.PI,false);
    context.closePath();
    context.stroke();
    context.fill();
}

function drawEnemy(enemy,x,y){
    var radius = 60 * (window.innerHeight/800);
	var gameCanvas = document.getElementById("gameCanvas");
	var context = gameCanvas.getContext("2d");
	
	context.strokeStyle = '#FFFF00';
    context.fillStyle = enemy.faceColor;
    context.lineWidth = 4;
    context.beginPath();
	//100,75
    context.arc(x,y,radius,0*Math.PI,2*Math.PI,true);
    context.closePath();
    context.stroke();
    context.fill();
 
 
    // The Smile
    context.strokeStyle = '#000000';
    context.lineWidth = 2;
    context.beginPath();
    context.arc(x+20,y,radius-5,0.75*Math.PI,1.25*Math.PI,false); 
    context.stroke();
       
 
    // The Left eye
    context.strokeStyle = '#00FF00';
    context.fillStyle = '#00FF00';
    context.beginPath();
    context.arc(x+15,y-20,radius-48,0*Math.PI,2*Math.PI,false);
    context.closePath();
    context.stroke();
    context.fill();
 
    // The Right Eye
    context.strokeStyle = '#00FF00';
    context.fillStyle = '#00FF00';
    context.beginPath();
    context.arc(x+15,y+20,radius-48,0*Math.PI,2*Math.PI,false);
    context.closePath();
    context.stroke();
    context.fill();
}

function handleTick() {
	//console.log('tick');
	if(!gameDone){
		var gameCanvas = document.getElementById("gameCanvas");
		var currentEnemyNumber = 0;
		var numberOfEnemies = enemies.length;
		
		if (Math.random() < 1/25)
		{
				//console.log(numberOfEnemies);
				addEnemy();
				enemyYPositions.push(0);
				enemyXPositions.push(Math.random() * canvasWidth);
		}

		while (currentEnemyNumber < numberOfEnemies) {
			enemies[currentEnemyNumber].x = enemies[currentEnemyNumber].x - enemies[currentEnemyNumber].speed - 1;
			currentEnemyNumber = currentEnemyNumber + 1;
		}
		
		gameCanvas.width = gameCanvas.width;		//this erases the contents of the canvas
		var ctx = gameCanvas.getContext("2d");
		ctx.fillStyle="#000000";
		ctx.fillRect(0,0,canvasWidth,canvasHeight);
		drawPlayer(playerX, playerY);
		
		currentEnemyNumber = 0;
		while (currentEnemyNumber < numberOfEnemies) {
			if(enemies[currentEnemyNumber].x < 0){
				//console.log(gameCanvas.width);
				enemies[currentEnemyNumber].x = gameCanvas.width;
				enemies[currentEnemyNumber].visible = true;
				enemies[currentEnemyNumber].y = 70+(Math.random() * (canvasHeight-120));
				if(enemies.faceColor = "#FF0000"){
					enemies[currentEnemyNumber].speed = 1 + (Math.random()*2);
				}
				else{
					enemies[currentEnemyNumber].speed = 3 + (Math.random()*2);
				}
			}
			if(enemies[currentEnemyNumber].visible){
				drawEnemy(enemies[currentEnemyNumber], enemies[currentEnemyNumber].x, enemies[currentEnemyNumber].y);
			}
			currentEnemyNumber = currentEnemyNumber + 1;
		}
		gameCanvas.getContext("2d").save();
		gameCanvas.getContext("2d").translate(canvasWidth - 20, 15);
		gameCanvas.getContext("2d").rotate(Math.PI / 2);
		gameCanvas.getContext("2d").strokeStyle = '#00FF00';
		gameCanvas.getContext("2d").fillStyle = '#00FF00';
		gameCanvas.getContext("2d").font = "72px Iceland";
		gameCanvas.getContext("2d").textBaseline = "top";
		gameCanvas.getContext("2d").fillText("Left: " + enemiesLeft,0,0);
		gameCanvas.getContext("2d").restore();

		currentEnemyNumber = 0;
		while (currentEnemyNumber < numberOfEnemies) {
			if (enemies[currentEnemyNumber].visible && collisionCheck(playerX, playerY, enemies[currentEnemyNumber].x, enemies[currentEnemyNumber].y) ){
				
				document.getElementById("soundControl").play();
				enemies[currentEnemyNumber].visible = false;
				enemiesLeft-=1;
				
				if(enemiesLeft == 0){
					gameDone = true;
					//communicate somehow to WPF
					resetCanvas("Well Done");
					
					
				}
			}
			currentEnemyNumber = currentEnemyNumber + 1;
		}
		
		ticksSurvived = ticksSurvived + level;
		
		level = 1 + Math.floor(ticksSurvived / 400);
		//console.log(level);
	}

}

function displayChoiceMenu() {
    document.getElementById("choiceMenu").style.display = "block";
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
                    startGame();
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

function resetCanvas(message){
	document.getElementById("backgroundMusic").pause();
	document.getElementById("gameEndMusic").play();
	var gameCanvas = document.getElementById("gameCanvas");
	gameCanvas.width = gameCanvas.width;
	var ctx = gameCanvas.getContext("2d");
	ctx.fillStyle="#000000";
	ctx.fillRect(0,0,canvasWidth,canvasHeight);	
	gameCanvas.getContext("2d").strokeStyle = '#00FF00';
	gameCanvas.getContext("2d").fillStyle = '#00FF00';
	gameCanvas.getContext("2d").font = "72px Iceland";
	gameCanvas.getContext("2d").textBaseline = "top";	
	ctx.save();
	ctx.translate((gameCanvas.width/2) , (gameCanvas.height/2) -150);
	ctx.rotate(Math.PI/2);
	ctx.textAlign = "center"
	gameCanvas.getContext("2d").fillText(message, 120,0);	
	ctx.restore();
	displayChoiceMenu();
	
}


function collisionCheck(playerX, playerY, enemyX, enemyY){
	return ((((playerX-enemyX) * (playerX-enemyX)) + ((playerY-enemyY) * (playerY-enemyY))) < (4100*(window.innerHeight/800)*(window.innerHeight/800)));
}