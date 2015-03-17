
var page = require('webpage').create();
var server = require('webserver').create();

// Start a listener to check events
server.listen(8083, function(request, response) { 
	var bodyText = JSON.stringify({
		success: true, 
		url: request.url
	});
	response.write('<html><head><title>eventtest9837423401</title>' +
					'<script>callPhantom("script"); window.onload=function() {callPhantom("onload");}</script>' +
					'</head><body>' + bodyText + '</body></html>'); 
	response.close(); 
});


page.onLoadStarted = function() {
    console.log("LoadStarted");
}

page.onLoadFinished = function(status) {
    console.log("LoadFinished");
}
page.onInitialized = function () {
    console.log("Initialized");
}
page.onCallback = function(event) {
    console.log('Callback: ' + event);
};
page.evaluate(function () {
    callPhantom('beforeopen');
});
page.open('http://localhost:8083/', function (status) {
    //phantom.exit();
});


