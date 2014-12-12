function gestureReceived(gestureName) {
    window.external.executeCommand("sendCommandToFrontScreen", "gestureRecieved,"+gestureName);
}