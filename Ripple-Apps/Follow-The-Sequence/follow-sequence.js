//GLOBAL VARIABLES;
var Level;
var Score;
var AcceptingAnswers;
var CurrentSequence;
var CurrentIndex;
var ChancesLeft;
var GameOver;
var GameStarted;
var TotalLevelTime;
var Timer;
var NextLevelEnabled;

function initializeGame()
{
    document.addEventListener("RightSwipe", function (e) {
        if (GameStarted)
        {
            startLevel();
        }
        else if(NextLevelEnabled)
        {
          
            startGame();
        }
    });


    document.addEventListener("numberReceived", function (evt) {
        numberReceived(evt.commandParameters[0]);
    });

    initializeGlobals();
}


function startGame()
{
    GameStarted = true;
    document.querySelector("#instructionsContent").style.display = "none";
    document.querySelector("#content").style.display = "block";
    document.querySelector("#contentHeading").innerHTML = "Level " + Level;
   
}

function startLevel()
{
    AcceptingAnswers = true;
    document.querySelector("#LevelChange").play();
    document.querySelector("#startInstruction").style.display = "none";
    Timer.startTimer();
    eraseNumbers();
    setInterval(function () {
        if (Timer.running) {
            var remainingTime = TotalLevelTime - Timer.getTime();
            document.querySelector(".time").innerHTML = remainingTime;
            if (remainingTime == 0) {
                endGame();
            }
        }
    }, 900);

    
}

function numberReceived(number)
{
    if (AcceptingAnswers) {
        document.querySelector("#Audio" + number).play();
        if (number == CurrentSequence[CurrentIndex]) {
            writeNumber();
            CurrentIndex++;
            if (CurrentIndex == (Level + 4)) {
                Score = Score + (Level) * 20
                document.querySelector("#points").innerHTML = Score;
                if (Level < 15) {
                    startNewLevel();
                }
                else {
                    declareWinner();
                }

            }
            

        }
        else if (ChancesLeft > 0) {
            ChancesLeft--;
            drawChances();
        }
        else {
            endGame();
        }
    }
}

function initializeGlobals()
{
    Level = 0;
    Score = 0;
    Timer = rippleHelper.timer();
    initializeNewLevel();
    CurrentIndex = 0;
    ChancesLeft = 5;
    drawChances();
    GameOver = false;
    GameStarted = false;
    
}

function drawChances()
{
    document.querySelector("#chances").innerHTML = "";
    for (var i = 0; i < ChancesLeft; ++i)
    {
        document.querySelector("#chances").innerHTML += "&#9786;";
    }
}

function startNewLevel()
{
    //Do the UI Etc Etc
    document.querySelector("#startInstruction").style.display = "block";
    initializeNewLevel();
    
}

function initializeNewLevel()
{
    AcceptingAnswers = false;
    document.querySelector("#LevelChange").play();
    Timer.stopTimer();
    Level += 1;
    CurrentSequence = generateSequence(Level + 4);
    TotalLevelTime = (Level + 5) * 4 + 1;
    document.querySelector(".time").innerHTML = TotalLevelTime;
    if (GameStarted) {
        document.querySelector("#contentHeading").innerHTML = "Level " + Level;
    }
    CurrentIndex = 0;
    writeNumbers();
    NextLevelEnabled = true;
}

function endGame()
{
    Timer.stopTimer();
    AcceptingAnswers = false;
    GameOver = true;
    document.querySelector("#content").style.display = "none";
    document.querySelector("#GameOver").play();
    document.querySelector("#instructionsContent").innerHTML = "<div style='text-align:center;color:#ff0'> Sorry,  Game Over <br/>Your Score :" + Score + "</div>";;
    
    document.querySelector("#instructionsContent").style.display = "block";
    document.querySelector("#contentHeading").innerHTML = "End Of Game";
    setTimeout(function () {
        rippleHelper.sendCommandToFloor("ExitGame", []);
    }, 2500);
}



function declareWinner()
{
    Timer.stopTimer();
    AcceptingAnswers = false;
    GameOver = true;
    document.querySelector("#content").style.display = "none";
    document.querySelector("#instructionsContent").innerHTML = "<div style='text-align:center;color:#ff0'> Congratulations, You Won <br/>Your Score :" + Score + "</div>";;

    document.querySelector("#instructionsContent").style.display = "block";
    document.querySelector("#contentHeading").innerHTML = "End Of Game";
    setTimeout(function () {
        rippleHelper.sendCommandToFloor("ExitGame", []);
    }, 2500);
}

function generateSequence(length) {
    var sequence = new Array();
    var previousNumber = 0;
    for (var i = 0; i < length; ++i) {
        var numbers = [1, 2, 3, 4];
        var prevIndex = numbers.indexOf(previousNumber);
        if (prevIndex > -1) {
            numbers.splice(prevIndex, 1);
        }

        var currentNumber = numbers[Math.floor(Math.random() * numbers.length)];
        sequence.push(currentNumber);
        previousNumber = currentNumber;
    }
    return sequence;
}

function writeNumbers()
{
    var hints = document.querySelector("#hints");
    hints.innerHTML = "";
    for (var i = 0; i < CurrentSequence.length; ++i)
    {
        var hint = document.createElement("span");
        hint.className = "hint";
        hint.innerHTML = CurrentSequence[i];
        hints.appendChild(hint);
    }
}

function eraseNumbers()
{
    var hints = document.querySelectorAll(".hint");
    for (var i = 0; i < CurrentSequence.length; ++i) {
        hints[i].innerHTML = "&nbsp;";
    }
}

function writeNumber()
{
    var hints = document.querySelectorAll(".hint");
    hints[CurrentIndex].innerHTML = CurrentSequence[CurrentIndex];
}