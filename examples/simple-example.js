
var page = require("webpage").create();

page.open("http://www.google.com", function(status) {
    console.log("Page load finished, status: " + status);
    page.evaluateJavaScript('var message = "hello from ie";');
    console.log(page.evaluate(function(message1, message2, message3) { return message1 + message2 + message3; }, 'hello argument1', 'argument2', 3));
    page.render("google.gif");
    //phantom.exit();
});

