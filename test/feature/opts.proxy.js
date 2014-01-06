
// triflejs.exe ..\..\test\feature\opts.proxy.js --proxy=localhost:8080 --proxy-auth=MacUser:123456

var page = require("webpage").create();

page.open("http://www.triflejs.org", function(status) {
    if (status === 'success') {
        page.render("triflejs.org.png");
        console.log('Page rendered');
    } else {
        console.error('Cannot load url.');
    }
    phantom.exit();
});

