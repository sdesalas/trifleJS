var page = new WebPage();

page.onAlert = function(message) {
	console.log("onAlert: " + message);
}

page.onConfirm = function(message) {
    console.log("onConfirm", message);
	return false;
}

page.evaluateJavaScript("alert(confirm('onAlert Test'));");

