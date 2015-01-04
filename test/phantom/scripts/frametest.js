
var page = require('webpage').create();

page.open('http://localhost:8897', function(status) {

	console.log(status);
	page.windowName = "main frame";
	console.log(JSON.stringify({
		"page.framesCount": page.framesCount,
		"page.framesName": page.framesName,
		"page.windowName": page.windowName,
		"page.frameName": page.frameName,
		"page.frameUrl": page.frameUrl,
		"page.focusedFrameName": page.focusedFrameName
	}));
	page.switchToFrame('frame1');
	console.log('------');

	console.log(JSON.stringify({
		"page.framesCount": page.framesCount,
		"page.framesName": page.framesName,
		"page.windowName": page.windowName,
		"page.frameName": page.frameName,
		"page.frameUrl": page.frameUrl,
		"page.focusedFrameName": page.focusedFrameName
	}));
	page.switchToMainFrame();
	page.switchToFrame('frame2-1');
	console.log('------');

	console.log(JSON.stringify({
		"page.framesCount": page.framesCount,
		"page.framesName": page.framesName,
		"page.windowName": page.windowName,
		"page.frameName": page.frameName,
		"page.frameUrl": page.frameUrl,
		"page.focusedFrameName": page.focusedFrameName
	}));
	phantom.exit();
})