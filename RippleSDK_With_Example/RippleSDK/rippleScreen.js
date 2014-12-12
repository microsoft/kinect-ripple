var _DefaultDebugMode = false;
var _DefaultEmulatorMode = false;
var _whitelistCommands = ["sendCommandToBottomFloor"];
var _RippleHelper =  null;
var _RippleKeyboardURL = "http://ripplekeyboardinput.cloudapp.net/api/RippleInput/";
var rippleHelper = new RippleHelper();


if (window.addEventListener) {
    // For standards-compliant web browsers
    window.addEventListener("message", displayMessage, false);
}
else {
    window.attachEvent("onmessage", displayMessage);
}

document.addEventListener("SessionID", function (data) {
    _RippleHelper.sessionID = data.commandParameters[0];
});

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


        rippleHelper.sendCommandToFloor = sendMessageToFloor;
        rippleHelper.listenKeyboardInput = false;
        rippleHelper.keyboardListener = null;
        rippleHelper.enableKeyboardInput = function (bool) {
            if (rippleHelper.sessionID == null) {
                setTimeout(function () {
                    executeExternalCommand("GetSessionID", []);
                },3000);
            }
            if (bool) {
                rippleHelper.listenKeyboardInput = true;
                rippleHelper.keyboardListener = setInterval(function () {
                    getKeyboardInput();
                }, 500);
            }
            else {
                try {
                    if (rippleHelper.keyboardListener!=null) {
                        clearInterval(rippleHelper.keyboardListener);
                    }
                }
                catch (Exception) {
                }
            }
        };
        _RippleHelper = rippleHelper;

    }
    return _RippleHelper;
}

function sendMessageToFloor(commandName, commandParameters) {
    var commandParametersCSV = "";
    for (i = 0; i < commandParameters.length; ++i) {
        commandParametersCSV = commandParametersCSV.concat(",", commandParameters[i]);
    }

    commandParametersCSV = commandName.concat(commandParametersCSV);
    executeExternalCommand("sendCommandToBottomFloor", commandParametersCSV);
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
function getKeyboardInput() {
    if (_RippleHelper.listenKeyboardInput) {
        if (_RippleHelper.sessionID == null) {
            //throw "SessionID not implemented";
        }
        else {
            try {
               
                ajax.get(_RippleKeyboardURL + _RippleHelper.sessionID, {}, function (response, status) {
                    console.log(response);
                    if (status == 200) {
                        try {
                            console.log(_RippleKeyboardURL + _RippleHelper.sessionID);
                            //var parser = new DOMParser();
                            //var xmlDoc = parser.parseFromString(response, "text/xml");
                            var filteredResponse = response;

                            if (filteredResponse != "NoInput") {
                                var evt = document.createEvent("Event");
                                evt.initEvent("keyboardInput", true, false);
                                evt.input = filteredResponse;
                                document.dispatchEvent(evt);
                            }
                        }
                        catch (Exception) {
                        }
                    }
                    else {
                        if (_RippleHelper.debugMode) {
                            console.log("Error" + x.responseText);
                        }
                    }
                });
            }
            catch (Exception) {
                if (_RippleHelper.debugMode) {
                    console.log(Exception);
                }
            }
        }
    }
}



function executeCommandFromFloor(commandParameters) {
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
                sendMessage("floor", "screen", "command", commandParametersCSV);
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

ajax = new Object();

ajax.x = function () {
    if (typeof XMLHttpRequest !== 'undefined') {
        return new XMLHttpRequest();
    }
    var versions = [
        "MSXML2.XmlHttp.5.0",
        "MSXML2.XmlHttp.4.0",
        "MSXML2.XmlHttp.3.0",
        "MSXML2.XmlHttp.2.0",
        "Microsoft.XmlHttp"
    ];

    var xhr;
    for (var i = 0; i < versions.length; i++) {
        try {
            xhr = new ActiveXObject(versions[i]);
            break;
        } catch (e) {
        }
    }
    return xhr;
};

ajax.send = function (url, callback, method, data, sync) {
    
    var x = ajax.x();
    x.open(method, url, sync);
    x.onreadystatechange = function () {
        if (x.readyState == 4) {
            callback(x.responseText, x.status)
        }
    };
    if (method == 'POST') {
        x.setRequestHeader('Content-type', 'application/x-www-form-urlencoded');
    }
    x.send(data)
};

ajax.get = function (url, data, callback, sync) {
    var query = [];
    for (var key in data) {
        query.push(encodeURIComponent(key) + '=' + encodeURIComponent(data[key]));
    }
    ajax.send(url + '?' + query.join('&'), callback, 'GET', null, sync)
};

ajax.post = function (url, data, callback, sync) {
    var query = [];
    for (var key in data) {
        query.push(encodeURIComponent(key) + '=' + encodeURIComponent(data[key]));
    }
    ajax.send(url, callback, 'POST', query.join('&'), sync)
};

function getParameterByName(name) {
    name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
    var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
        results = regex.exec(location.search);
    return results == null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
}

function sendMessage(to, from, type, params) {
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

    switch (type) {
        case "command":
            executeCommandFromFloor(data);
            break;

        case "gesture":
            console.log("Error: Gestures cannot be handled by screen");
            break;

        default:
            executeCommandFromFloor(data);
            break;
    }
}