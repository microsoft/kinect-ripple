var surface;
var surfaceCtx;
var wR, hR;

var answers;
var answersCtx;

var questions;
var currentQuestion = 0;
var neglectedRegion;

var backgroundImage;
var vGoodImage;
var goodImage;
var badImage;

var options;
var collisionTimer;

var kicked;
var kickedOption;
var selectedSmileyImage;
var selectedImageWidth;

//Dynamic Rendering
var lastMousePosition = new Point(Math.random()*window.innerWidth, Math.random()*window.innerHeight);
var numberOfRows = 2;
var numberOfColumns = 2;

//sounds
var optionSelectedSound;


function initializeFloor() {
    surface = document.getElementById("surface");
    surface.style.width = window.innerWidth;
    surface.style.height = window.innerWidth;
    surface.width = window.innerWidth;
    surface.height = window.innerHeight;
    surfaceCtx = surface.getContext("2d");

    answers = document.getElementById("answers");
    answers.style.width = window.innerWidth;
    answers.style.height = window.innerWidth;
    answers.width = window.innerWidth;
    answers.height = window.innerHeight;
    answersCtx = answers.getContext("2d");

    backgroundImage = new Image();
    backgroundImage.src = "Assets/background.png";
    vGoodImage = new Image();
    vGoodImage.src = "Assets/vGood.png";
    goodImage = new Image();
    goodImage.src = "Assets/good.png";
    badImage = new Image();
    badImage.src = "Assets/bad.png";
    collisionTimer = initializeTimer();
    populateQuestions();
    backgroundImage.onload = function () {
        wR = (window.innerWidth / backgroundImage.width);
        hR = (window.innerHeight / backgroundImage.height);
        surfaceCtx.drawImage(backgroundImage, 0, 0, window.innerWidth, window.innerHeight);
        setTimeout(function () {
            neglectedRegion = findMouseRegion();
            defineOptions();
            refreshView();
        }, 1000);
    }
    setInterval(function () {
        zoomImage();
    }, 100);

    optionSelectedSound = document.getElementById("optionSelectedSound");
}

function detectCollision(x, y) {
    try{
        var collisionDetected = false;
        for(i=0;i<options.length;++i){
            if (inCollisionZone(options[i], x, y)) {
                takeActionForCollision(questions[currentQuestion].answerImages[i], questions[currentQuestion].answers[i]);
                collisionDetected = true;
                break;
            }
        }
        if(!collisionDetected) {
            if (collisionTimer.running) {
                collisionTimer.stopTimer();
                refreshView();
            }
        }
    }
    catch (Exception) {
    }
}

function takeActionForCollision(image, imageResponse) {
    if (collisionTimer.running) {
        if (selectedSmileyImage == image) {
            if (collisionTimer.getTime() > 2) {
                kicked = true;
                kickedOption = imageResponse;
                startAnimation(kickedOption);
                optionSelectedSound.play();
            }
        }
        else {
            collisionTimer.stopTimer();
            collisionTimer.startTimer();
            selectedSmileyImage = image;
        }
    }
    else {
        selectedSmileyImage = image;
        collisionTimer.startTimer();
    }
   
}



function zoomImage() {
    if (selectedSmileyImage != null && collisionTimer.running) {
        if (selectedImageWidth < 350) {
            selectedImageWidth += 10;
        }
        drawSmileys();
    }
}

function startAnimation(answer) {
    for (i = 0; i < questions[currentQuestion].answers.length; i++) {
        if (answer == questions[currentQuestion].answers[i]) {
            //consoloe.log("ACCEPTED");
            selectedImage = questions[currentQuestion].answerImages[i];
            animate(options[i]);
            break;
        }
    }
}

function animate(point) {
    if (point.x < window.innerWidth) {
        point.x += 10;
        drawSmileys();
        setTimeout(function () {
            animate(point)
        }, 25);
    }
    else {
        sendMessageToFrontScreen("answerReceived", [kickedOption]);
        if (kickedOption == questions[currentQuestion].correctAnswer) {
            window.external.executeCommand("playAudio", "Your Answer is Correct");
            

        }
        else {
            window.external.executeCommand("playAudio", "Oops! Wrong Answer");
        }
    }
}
function inCollisionZone(point, x, y) {
    try{
        if ((x > point.x && x < point.x + (250 * wR)) && (y > point.y && y < (point.y + 250 * hR))) {
            return true;
        }
        else {
            return false;
        }
    }
    catch (ex) {
    }
}

function refreshView() {
    kicked = false;
    kickedOption = null;
    selectedSmileyImage = null;
    selectedImageWidth = 250;

    drawSmileys();
}

function defineOptions() {
    options = new Array();
    var regionWidth = window.innerWidth / numberOfRows;
    var regionHeight = window.innerHeight / numberOfColumns;
    for (i = 1; i <= numberOfRows; ++i) {
        for (j = 1; j <= numberOfColumns ; ++j) {
            if (!(i == neglectedRegion.row && j == neglectedRegion.column)) {
                var x = ((regionWidth * (j - 0.5)) - ((250 - Math.random()*150) * wR));
                var y = ((regionHeight * (i - 0.5)) - ((220 - (Math.random()*50)) * hR));
                options.push(new Point(x, y));
				//console.log(x+","+y+" "+i+","+j);
            }
        }
    }
}


function findMouseRegion() {
    var rowNumber = Math.floor((lastMousePosition.y / (window.innerHeight/2)) + 1);
    var columnNumber = Math.floor((lastMousePosition.x / (window.innerWidth / 2)) + 1);
    var region = new Object();
    region.row = rowNumber;
    region.column = columnNumber;
	//console.log(region.row+"-"+region.column);
    return region;
}

function drawSmileys() {
    answers.width = answers.width;
    for (i = options.length - 1; i >=0; --i) {
        drawSmiley(questions[currentQuestion].answerImages[i], options[i]);
    }
}

function drawSmiley(image, point) {
    answersCtx.save();

    if (image != selectedSmileyImage) {
        answersCtx.translate(point.x + (125 * wR), point.y + (125 * wR));
        answersCtx.rotate(Math.PI / 2);
        answersCtx.translate(-(125 * wR), -(125 * wR));
        answersCtx.drawImage(image, 0, 0, 250 * wR, 250 * wR);
    }
    else {
        answersCtx.translate(point.x + ((selectedImageWidth/2) * wR), point.y + ((selectedImageWidth/2) * wR));
        answersCtx.rotate(Math.PI / 2);
        answersCtx.translate(-((selectedImageWidth/2) * wR), -((selectedImageWidth/2) * wR));
        answersCtx.drawImage(image, 0, 0, selectedImageWidth*wR , selectedImageWidth * wR);
    }
    answersCtx.restore();
}

function populateQuestions() {
    questions = new Array();
    questions.push(new Question("How many tech professionals have come at AWE2014?", ["1-1.png", "1-2.png", "1-3.png"], ["2000+", "1500+", "3000+"], "2000+"));
    questions.push(new Question("How many demos are there at AWE2014?", ["2-1.png", "2-2.png", "2-3.png"], ["100+", "150+", "200+"], "200+"));
    questions.push(new Question("How many talks by top industry leaders are there at AWE2014?", ["3-1.png", "3-2.png", "3-3.png"], ["75+", "100+", "125+"], "100+"));
}


function Question(questionText, answerImages, answers, correctAnswer) {
    var question = new Object();
    question.questionText = questionText;
    question.answerImages = new Array();
    question.answers = new Array();
    for (i = 0; i < answerImages.length; ++i) {
        var image = new Image();
        image.src = "Assets/" + answerImages[i];
        question.answerImages.push(image);
        question.answers.push(answers[i]);
        question.correctAnswer = correctAnswer;
    }
    return question;
}


function Point(x, y) {
    var point = new Object();
    point.x = Math.floor(x);
    point.y = Math.floor(y);
    return point;
}

window.onload = function () {
    initializeFloor();
    answers.addEventListener("mousemove", function (e) {
        lastMousePosition = new Point(e.clientX, e.clientY);
        if (!kicked) {
            detectCollision(e.clientX, e.clientY);

        }
    });
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

function sendMessageToFrontScreen(commandName, commandParameters) {
    
    var commandParametersCSV = "";
    for (i = 0; i < commandParameters.length; ++i) {
        commandParametersCSV = commandParametersCSV.concat(",", commandParameters[i]);
    }

    commandParametersCSV = commandName.concat(commandParametersCSV);
    
    switch (commandName) {
        case "answerReceived":
            window.external.executeCommand("sendCommandToFrontScreen", commandParametersCSV);
            break;
    }
}

function executeCommandFromScreen(commandParameters) {
    var parameters = commandParameters.split(",");
    switch (parameters[0]) {
        case "refreshView":
            neglectedRegion = findMouseRegion();
            defineOptions();
            currentQuestion++;
            refreshView();
            break;
        case "QuizOver":
            window.external.playAudio(parameters[1]);
        case "exitGame":
            window.external.executeCommand("exitGame", "");
            break;
    }
}
