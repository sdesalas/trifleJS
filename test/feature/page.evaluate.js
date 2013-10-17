var page = require("webpage").create();

console.log(trifle);

page.open("http://www.phantomjs.org", function(status) {
    if (status === 'success') {
        var message = page.evaluate(function(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8) {
            var result = arg1 + arg2 + arg3[0] + arg3[1] + arg4.a + ' ' + arg5 + arg6;
            if (arg7 === null) {
                result += arg7;
            };
            if (arg8 === undefined) {
                result += arg8;
            }
            return result;
        }, 'message 1', 234, [5, '6'], { a: "78" }, !true, NaN, null, undefined);
        if (message === 'message 12345678 falseNaNnullundefined') {
            console.log('GOOD! ' + message);
        } else {
            console.error('ERROR! ' + message);
        }
    } else {
        console.error('ERROR! Cannot load url.');
    }
    phantom.exit();
});