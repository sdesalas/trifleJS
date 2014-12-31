
// triflejs.exe ..\..\test\feature\opts.ignore-ssl.js --ignore-ssl-errors=true

var page = require("webpage").create();

page.open("https://localhost", function(status) {
    if (status === 'success') {
        page.render("ingnore-ssl.png");
        console.log('Page rendered');
    } else {
        console.error('Cannot load url.');
    }
    phantom.exit();
});
