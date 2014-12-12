var _DefaultDebugMode = false;
var _DefaultEmulatorMode = false;
var _whitelistCommands = ["sendCommandToFrontScreen"];
var _RippleHelper = null;
var rippleHelper = new RippleHelper();

if (window.addEventListener) {
    // For standards-compliant web browsers
    window.addEventListener("message", displayMessage, false);
}
else {
    window.attachEvent("onmessage", displayMessage);
}

function RippleHelper(debugMode, emulatorMode) {
    if (_RippleHelper == null) {
        rippleHelper = new Object();
        rippleHelper.timer = initializeTimer;
        rippleHelper.debugMode = debugMode || _DefaultDebugMode;
        rippleHelper.setDebugMode = function (debugMode) {
            rippleHelper.debugMode = debugMode;
        }

        rippleHelper.emulatorMode = emulatorMode || _DefaultEmulatorMode;
        rippleHelper.setEmulatorMode = function (emulatorMode) {
            rippleHelper.emulatorMode = emulatorMode;
        }

        rippleHelper.goToStart = function () {
            executeExternalCommand("exitGame", "");
        }
        rippleHelper.textToSpeech = function (text) {
            executeExternalCommand("playAudio", text);
        }
        rippleHelper.sendCommandToFrontScreen = sendMessageToFrontScreen;

        rippleHelper.unlockSystem = function () {
            executeExternalCommand("unlockSystem", "");
        }
        rippleHelper.processRippleXMLCommand = function (data) {
            executeExternalCommand("sendCommandForScreenProcessing", data);
        }
        _RippleHelper = rippleHelper;
    }
        return _RippleHelper;
    
}

function gestureReceived(gestureName) {
    var evt = document.createEvent("Event");
    evt.initEvent(gestureName, true, false);
    document.dispatchEvent(evt);
}

function executeCommandFromScreen(commandParameters) {
    var parameters = commandParameters.split(",");
    var commandName = parameters[0];
    var evt = document.createEvent("Event");
    evt.initEvent(commandName, true, false);
    parameters.shift();
    evt.commandParameters = parameters;
    document.dispatchEvent(evt);
  
}

function executeExternalCommand(commandName, commandParametersCSV) {
    if (_RippleHelper.debugMode) {
        if (_RippleHelper.emulatorMode) {
            console.log("EMULATOR MODE IS ON, Command " + commandName + " with Parameters " + commandParametersCSV + " will be executed");

            // check whether it is command is in whitelist
            if (_whitelistCommands.indexOf(commandName) != -1) // command found in whitelisted commands
            {
                // call to sendMessage
                sendMessage("screen", "floor", "command", commandParametersCSV);
            }
        }
        else {
            console.log("DEBUG MODE IS ON, Command " + commandName + " with Parameters " + commandParametersCSV + " will be executed");
        }
    }
    else {
        window.external.executeCommand(commandName, commandParametersCSV);
    }
}

function sendMessageToFrontScreen(commandName, commandParameters) {

    var commandParametersCSV = "";
    for (i = 0; i < commandParameters.length; ++i) {
        commandParametersCSV = commandParametersCSV.concat(",", commandParameters[i]);
    }

    commandParametersCSV = commandName.concat(commandParametersCSV);
    executeExternalCommand("sendCommandToFrontScreen", commandParametersCSV);
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

function sendMessage(to, from, type, params)
{
    var msgString = to + "," + from + "," + type + "," + params;
    window.parent.postMessage(msgString, "*");    // <destination>.postMessage(<message>, <domain>)
    return false;
}

function displayMessage(evt)
{
    var msgString = evt.data;
    var parameters = msgString.split(",");

    var type = parameters.shift();
    var data = parameters.toString();

    switch(type)
    {
        case "command":
            executeCommandFromScreen(data);
            break;

        case "gesture":
            gestureReceived(data);
            break;

        default:
            executeCommandFromScreen(data);
            break;
    }
}