var surface;
var surfaceCtx;
var wR, hR;
var answer;
var answerCtx;
var question;
var questionCtx;
var questions;
var backgroundImage;
var vGoodImage;
var goodImage;
var badImage;
var currentQuestion = 0;
var answers;

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
            break;
        }
    }
    imageX = (window.innerWidth / 2 - 125 * wR);
    animateFromBottom(selectedImage, imageX, window.innerHeight, window.innerHeight / 2 - (100 * hR));
    currentQuestion++;
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
            }, 1000);
            setTimeout(function () {
                sendMessageToFloor("refreshView", "");
            }, 2500);
        }
    }
}

function sayThankYou() {
    question.width = question.width;
    answer.width = answer.width;
    var questionText = "Thank You For Your Feedback";
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
    },1500);
}

function saveFeedback(answers) {
    var answersCSV = "Answers";
    for (i = 0; i < answers.length; ++i) {
        answersCSV = answersCSV.concat(",", answers[i]);
    }
    console.log(answersCSV);
   window.external.executeCommand("saveFeedback", answersCSV);
}

function populateQuestions() {
    questions = new Array();
    questions.push(new Question("How was your overall Experience?",["1-1.png","1-2.png","1-3.png"],["Bad","Good","Very Good"]));
    questions.push(new Question("How would you rate content showcased through Ripple?", ["2-1.png", "2-2.png", "2-3.png"],["Could Be Better","Appropriate","Well Designed"]));
    questions.push(new Question("What Scenarios would you prefer in next Ripple update?", ["3-1.png", "3-2.png", "3-3.png"], ["Employee Services", "Games", "MSIT Information"]));
}

function Question(questionText, answerImages, answers) {
    var question = new Object();
    question.questionText = questionText;
    question.answerImages = new Array();
    question.answers = new Array();
    for (i = 0; i < answerImages.length; ++i) {
        var image = new Image();
        image.src = "Assets/" + answerImages[i];
        question.answerImages.push(image);
        question.answers.push(answers[i]);
    }
    return question;
}

window.onload = function () {
    initializeScreen();
}

