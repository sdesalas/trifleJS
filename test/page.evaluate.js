var page = require("webpage").create();

page.open("http://www.triflejs.org", function(status) {
    if (status === 'success') {
        var message = page.evaluate(function(arg1, arg2, arg3) {
            return arg1 + arg2 + arg3[0] + arg3[1];
        }, 'message 1', 234, [5, '6']);
        if (message === 'message123456') {
            console.log('GOOD!' + message);
        } else {
            console.error('ERROR! ' + message);
        }
    } else {
        console.error('ERROR! Cannot load url.');
    }
    phantom.exit();
});