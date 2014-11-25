var page = require("webpage").create();
/*
page.clipRect = {
	left: 150,
	top: 150,
	width: 200,
	height: 200
};
*/
// page.zoomFactor = 2;

page.open("http://www.triflejs.org", function(status) {
    if (status === 'success') {
        page.render("triflejs.org.png");
        console.log('Page rendered');
    } else {
        console.error('Cannot load url.');
    }
    phantom.exit();
});

