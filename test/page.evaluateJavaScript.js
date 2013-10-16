var page = require("webpage").create();

page.open("http://www.triflejs.org", function(status) {
    if (status === 'success') {
        page.evaluateJavaScript('var message = "hello from ie";');
        var message = page.evaluate(function() {
            return message;
        });
        if (message === 'hello from ie') {
            console.log('GOOD!' + message);
        } else {
            console.error('ERROR! ' + message);
        }
    } else {
        console.error('ERROR! Cannot load url.');
    }
    phantom.exit();
});