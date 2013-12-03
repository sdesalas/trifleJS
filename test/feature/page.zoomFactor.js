var page = require("webpage").create();

page.zoomFactor = 0.5;
page.open("http://www.triflejs.org", function(status) {
    if (status === 'success') {
        page.render("triflejs.org.png");
        console.log('Page rendered');
    } else {
        console.error('Cannot load url.');
    }
    phantom.exit();
});

