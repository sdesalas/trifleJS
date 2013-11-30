var page = require("webpage").create();

page.open("http://www.phantomjs.org", function(status) {
    console.log("Page load finished, status: " + status);
    page.evaluateJavaScript('var message = "hello from ie";');
    console.log(page.evaluate(function(message1, message2, message3) { return message + message1 + message2 + message3; }, 'hello argument1', 'argument2', 3));
    page.render("phantomjs.org.png");
    page.includeJs("http://ajax.googleapis.com/ajax/libs/jquery/1.6.1/jquery.min.js", function() {
        console.log(page.evaluate(function() {
            return $("#intro").text();
        }));
        trifle.wait(5000);
        phantom.exit();
    });
});