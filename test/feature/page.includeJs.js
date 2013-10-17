var page = require("webpage").create();

page.open("http://www.phantomjs.org", function(status) {
    page.includeJs("http://ajax.googleapis.com/ajax/libs/jquery/1.6.1/jquery.min.js", function() {
        console.log(page.evaluate(function() {
            return $("#intro").text();
        }));
        console.wait(5000);
        phantom.exit();
    });
});