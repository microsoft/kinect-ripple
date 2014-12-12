var surface;
var surfaceCtx;
var wR, hR;
var answer;
var answerCtx;
var question;
var questionCtx;
var questions;

var response;
var responseCtx;

var backgroundImage;
var vGoodImage;
var goodImage;
var badImage;
var rightImages;
var wrongImages;
var currentQuestion = 0;
var answers;
var score = 0;

function executeCommandFromFloor(commandParameters) {
    var parameters = commandParameters.split(",");
    switch (parameters[0]) {
        case "answerReceived":
            answerReceived(parameters[1]);
            answers.push(parameters[1]);
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

    switch (commandName) {
        case "refreshView":
            window.external.executeCommand("sendCommandToBottomFloor", commandParametersCSV);
            break;
		case "exitGame":
            window.external.executeCommand("sendCommandToBottomFloor", commandParametersCSV);
            break;		
    }
}


function initializeScreen() {
    loadRightImages();
    loadWrongImages();
    surface = document.getElementById("surface");
    surface.style.width = window.innerWidth;
    surface.style.height = window.innerWidth;
    surface.width = window.innerWidth;
    surface.height = window.innerHeight;
    surfaceCtx = surface.getContext("2d");
    question = document.getElementById("question");
    question.style.width = window.innerWidth;
    question.style.height = window.innerWidth;
    question.width = window.innerWidth;
    question.height = window.innerHeight;
    questionCtx = question.getContext("2d");

    answer = document.getElementById("answer");
    answer.style.width = window.innerWidth;
    answer.style.height = window.innerWidth;
    answer.width = window.innerWidth;
    answer.height = window.innerHeight;
    answerCtx = answer.getContext("2d");

    response = document.getElementById("response");
    response.style.width = window.innerWidth;
    response.style.height = window.innerHeight;
    response.width = window.innerWidth;
    response.height = window.innerHeight;
    responseCtx = response.getContext("2d");

    backgroundImage = new Image();
    backgroundImage.src = "Assets/background.png";
    vGoodImage = new Image();
    vGoodImage.src = "Assets/vGood.png";
    goodImage = new Image();
    goodImage.src = "Assets/good.png";
    badImage = new Image();
    badImage.src = "Assets/bad.png";
    populateQuestions();
    answers = new Array();
    backgroundImage.onload = function () {
        wR = (window.innerWidth / backgroundImage.width);
        hR = (window.innerHeight / backgroundImage.height);
        surfaceCtx.drawImage(backgroundImage, 0, 0, window.innerWidth, window.innerHeight);
        nextQuestion();
    }

}

function loadRightImages()
{
    rightImages = new Array();
    var folder = "Assets/right";
    for (i = 0; i < 40; ++i) {
        if (i < 10) {
            var image = new Image();
            image.src = folder + "/Comp1_0000" + i + ".png";

            rightImages.push(image);
        }
        else {
            var image = new Image();
            image.src = folder + "/Comp1_000" + i + ".png";
            image.onerror = function ()
            {
                console.log("Error in" +  image.src);
            }
            rightImages.push(image);
        }
    }
}

function loadWrongImages()
{
    wrongImages = new Array();
    var folder = "Assets/wrong";
    for (i = 0; i < 40; ++i) {
        if (i < 10) {
            var image = new Image();
            image.src = folder + "/Comp2_0000" + i + ".png";
            wrongImages.push(image);
        }
        else {
            var image = new Image();
            image.src = folder + "/Comp2_000" + i + ".png";
            wrongImages.push(image);
        }
    }
}

function nextQuestion() {
    //play some audio
    question.width = question.width;
    answer.width = answer.width;
    var questionText = questions[currentQuestion].questionText;
    questionCtx.fillStyle = "rgb(204,0,102)";
    var fontSize = Math.floor(64 * hR);
    questionCtx.font = fontSize + " px Segoe UI";
    questionCtx.textAlign = "center";
    questionCtx.textBaseline = "top";
    var textWidth = questionCtx.measureText(questionText).width;
    while (textWidth > (0.7 * window.innerWidth)) {
        fontSize -= 5;
        questionCtx.font = fontSize + " px Segoe UI";
        textWidth = questionCtx.measureText(questionText).width;
    }
    questionCtx.fillRect(window.innerWidth / 2 - textWidth / 2 - 100 * wR, window.innerHeight / 5 - 20 * hR,textWidth + 200*wR, (fontSize* 2.0) + (20*hR));
    questionCtx.fillStyle = "rgb(255,255,255)";
    questionCtx.fillText(questionText, window.innerWidth / 2, window.innerHeight / 5);
    answerCtx.fillStyle = "rgb(59,56,56)";
    var helpTextFontSize = Math.floor(64 * hR) -(36 * hR);
    answerCtx.font = helpTextFontSize + " px Segoe UI";
    var helpText = "<<Kindly stand on one of the floor options to select it>>";
    var helpTextWidth = answerCtx.measureText(helpText).width;
    answerCtx.fillRect(window.innerWidth / 2 - helpTextWidth / 2 - 75 * wR, window.innerHeight * 0.8 - 20 * hR, helpTextWidth + 150 * wR, (helpTextFontSize * 2.0) + (20 * hR));
    answerCtx.fillStyle = "rgb(255,255,255)";
    answerCtx.textAlign = "center";
    answerCtx.textBaseline = "top";
    answerCtx.fillText(helpText, window.innerWidth / 2, window.innerHeight * 0.8);
}

function answerReceived(answer) {
 
    var selectedImage;
    for (i = 0; i < questions[currentQuestion].answers.length; i++) {
       
        if (answer == questions[currentQuestion].answers[i]) {
            selectedImage = questions[currentQuestion].answerImages[i];
            if (answer == questions[currentQuestion].correctAnswer)
            {
                score += 10
                drawSequence(rightImages,0);
            }
            else
            {
                drawSequence(wrongImages, 0);
                //play wrong sequence
            }

            break;
        }
    }
    imageX = (window.innerWidth / 2 - 125 * wR);
    animateFromBottom(selectedImage, imageX, window.innerHeight, window.innerHeight / 2 - (100 * hR));
    currentQuestion++;
}

function drawSequence(images, starting)
{
    response.width = response.width;
    if (starting < 40) {
        responseCtx.drawImage(images[starting], (window.innerWidth - 600 * wR), (window.innerHeight - 450 * hR), 600 * wR, 450 * hR);
        setTimeout(function () {
            drawSequence(images, starting + 1);
        },100)
    }
}

function animateFromBottom(image, imageX, startY, stopY) {
    if (startY > stopY) {
        answer.width = answer.width;
        answerCtx.drawImage(image, imageX, startY, 250 * wR, 250 * wR);
        startY = startY - 10;
        setTimeout(function () {
            animateFromBottom(image, imageX, startY, stopY);
        }, 25);
    }
    else {
        if (currentQuestion == questions.length) {
            setTimeout(function () {
                sayThankYou();
            }, 1000);
        }
        else {
            setTimeout(function () {
                nextQuestion();
            }, 4000);
            setTimeout(function () {
                sendMessageToFloor("refreshView", "");
            }, 4500);
        }
    }
}

function sayThankYou() {
    question.width = question.width;
    answer.width = answer.width;
    var questionText = "Quiz is Over. Your Score: " + score + "/" + (questions.length * 10);
    sendMessageToFloor("QuizOver", ["Quiz is Over. Your Score: " + score + "/" + (questions.length * 10)]);
    currentQuestion++;
    questionCtx.fillStyle = "rgb(204,0,102)";
    var fontSize = Math.floor(64 * hR);
    questionCtx.font = fontSize + " px Segoe UI";
    questionCtx.textAlign = "center";
    questionCtx.textBaseline = "top";
    var textWidth = questionCtx.measureText(questionText).width;
    questionCtx.fillRect(window.innerWidth / 2 - textWidth / 2 - 100 * wR, window.innerHeight / 5 - 20 * hR, textWidth + 200 * wR, (fontSize * 2.0) + (20 * hR));
    questionCtx.fillStyle = "rgb(255,255,255)";
    questionCtx.fillText(questionText, window.innerWidth / 2, window.innerHeight / 5);
    saveFeedback(answers);
    setTimeout(function () {
        sendMessageToFloor("exitGame", "");
    },4500);
}

function saveFeedback(answers) {
    var answersCSV = "Answers";
    for (i = 0; i < answers.length; ++i) {
        answersCSV = answersCSV.concat(",", answers[i]);
    }
   // window.external.executeCommand("saveFeedback", answersCSV);
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


window.onload = function () {
    initializeScreen();
}

