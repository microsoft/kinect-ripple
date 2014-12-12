/* Global Variables */
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
    // function that executes on a Right Swipe gesture
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

    // function that executes when numberReceived event is fired
    document.addEventListener("numberReceived", function (evt) {
        numberReceived(evt.commandParameters[0]);
    });

    // initialize the global variables
    initializeGlobals();
}

/* This function is executed only at the beginning of the game */
function startGame()
{
    GameStarted = true;
    document.querySelector("#instructionsContent").style.display = "none";  // hide instructionsContent div
    document.querySelector("#content").style.display = "block";             // make content div visible
    document.querySelector("#contentHeading").innerHTML = "Level " + Level; // select contentHeading div and show Level # inside it
}

/* This function is executed when the user decides to begin the new level */
function startLevel()
{
    AcceptingAnswers = true;
    document.querySelector("#LevelChange").play();  // play the level change audio/music
    document.querySelector("#startInstruction").style.display = "none";
    Timer.startTimer();
    eraseNumbers();     // removes the previous level related numbers
    setInterval(function () {
        if (Timer.running) {
            var remainingTime = TotalLevelTime - Timer.getTime();
            document.querySelector(".time").innerHTML = remainingTime; // display the remaining time

            // if user runs out of time, end the game
            if (remainingTime == 0) {
                endGame();
            }
        }
    }, 900);

    
}

/* This function is executed when a number(input) is received from the user */
function numberReceived(number)
{
    // if the game is accepting answers
    if (AcceptingAnswers) {
        //document.querySelector("#Audio" + number).play();

        // if the number guessed is correct
        if (number == CurrentSequence[CurrentIndex]) {
            document.querySelector("#Correct").play();  // play the correct guess audio/music

            writeNumber();  // writes/displays the correctly guessed number on screen
            CurrentIndex++;

            // if it is the last number in the current sequence
            if (CurrentIndex == (Level + 4)) {
                Score = Score + (Level) * 20; // increment score
                document.querySelector("#points").innerHTML = Score;

                // if the user is yet to reach the last level (15, in this case)
                if (Level < 15) {
                    // begin the next level
                    startNewLevel();
                }
                else {
                    // user has completed(won) the game
                    declareWinner();
                }
            }
        }
        else if (ChancesLeft > 0)   // incorrect guess, but chances left
        {
            ChancesLeft--;  // decrement chances by 1
            document.querySelector("#Wrong").play();    // play the incorrect guess audio/music
            drawChances();  // display the reduced number of chances on screen
        }
        else                        // incorrect guess and no chances left
        {
            // Game Over!
            endGame();
        }
    }
}

/* This function initializes all the global variables when the game starts 
 * Any new global variables added should be initialized here and not in their declaration
 */
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

/* This function draws the number of chances(guesses) left
 * depending on the ChancesLeft global variable
 */
function drawChances()
{
    document.querySelector("#chances").innerHTML = "";
    for (var i = 0; i < ChancesLeft; ++i)
    {
        document.querySelector("#chances").innerHTML += "&#9786;";
    }
}

/* This function preps the next level as well as 
 * displays the instructions for the same
 */
function startNewLevel()
{
    // Refresh the UI for the new level, if required
    document.querySelector("#startInstruction").style.display = "block";    // show the start instruction

    // initialize the new level
    initializeNewLevel();
}

/* This function takes care of the basic housekeeping 
 * that is required to initialize a new level
 */
function initializeNewLevel()
{
    AcceptingAnswers = false;
    document.querySelector("#LevelChange").play();
    Timer.stopTimer();
    Level += 1;     // increment the Level #

    // Generate a new sequence for the current level
    CurrentSequence = generateSequence(Level + 4);

    // Determine the total time to be allocated for the current level
    TotalLevelTime = (Level + 5) * 4 + 1;

    document.querySelector(".time").innerHTML = TotalLevelTime; // show the time on screen
    if (GameStarted) {
        document.querySelector("#contentHeading").innerHTML = "Level " + Level; // update the contentHeading div with current level #
    }
    CurrentIndex = 0;
    writeNumbers();
    NextLevelEnabled = true;
}

/* This function is executed when the user loses the game
 * due to timeout or a predefined number of incorrect guesses
 */
function endGame()
{
    Timer.stopTimer();  // stops the timer
    AcceptingAnswers = false;
    GameOver = true;
    document.querySelector("#content").style.display = "none";
    document.querySelector("#GameOver").play(); // play Game Over audio/music
    document.querySelector("#instructionsContent").innerHTML = "<div style='text-align:center;color:#ff0'> Sorry,  Game Over <br/>Your Score :" + Score + "</div>";
    document.querySelector("#instructionsContent").style.display = "block"; // make instructionsContent div visible

    // Display "End of Game" and send "ExitGame" command to floor
    document.querySelector("#contentHeading").innerHTML = "End Of Game";
    setTimeout(function () {
        rippleHelper.sendCommandToFloor("ExitGame", []);
    }, 2500);
}

/* This function is executed when the user wins the game and is declared a winner */
function declareWinner()
{
    Timer.stopTimer();  // stops the timer
    AcceptingAnswers = false;
    GameOver = true;
    document.querySelector("#content").style.display = "none"; // hide the content div
    document.querySelector("#instructionsContent").innerHTML = "<div style='text-align:center;color:#ff0'> Congratulations, You Won <br/>Your Score :" + Score + "</div>";
    document.querySelector("#instructionsContent").style.display = "block"; // make instructionsContent div visible

    // Display "End of Game" and send "ExitGame" command to floor
    document.querySelector("#contentHeading").innerHTML = "End Of Game";
    setTimeout(function () {
        rippleHelper.sendCommandToFloor("ExitGame", []);
    }, 2500);
}

/* This function generates a random sequence of number 
 * Params:
 * length - determines the length of the sequence to be generated
 */
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

/* This function inserts the sequence of numbers on the screen (inside the hints div) */
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

/* This function erases all the numbers that are currently displayed on the screen
 * by overwriting them with a space (&nbsp;) character
 */
function eraseNumbers()
{
    var hints = document.querySelectorAll(".hint");
    for (var i = 0; i < CurrentSequence.length; ++i) {
        hints[i].innerHTML = "&nbsp;";  // &nbsp; is equivalent of one space
    }
}

/* This function writes(displays) a single number on the screen
 * It is used when the user correctly guesses a number
 */
function writeNumber()
{
    var hints = document.querySelectorAll(".hint");
    hints[CurrentIndex].innerHTML = CurrentSequence[CurrentIndex];
}