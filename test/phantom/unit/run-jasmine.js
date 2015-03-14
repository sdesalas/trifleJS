
// Launch tests
var jasmineEnv = jasmine.getEnv();

// Add a ConsoleReporter to 
// 1) print with colors on the console 
// 2) exit when finished
jasmineEnv.addReporter(new jasmine.ConsoleReporter(function(msg) {
    // Apply color
    var ansi = {
        green: '\033[32m',
        red: '\033[31m',
        yellow: '\033[33m',
        none: '\033[0m',
        newline: '\n'
    };
    msg = msg.replace(ansi.newline, '').replace(ansi.none, '');
    var printInColor = function(color, message) {
        if (color && message && ansi[color]) {
            console.API.color(color, message.replace(ansi[color], ''));
        }
    }
    // Print messages straight to console  
    if (msg.indexOf(ansi.red) === 0) {
        printInColor('red', msg);
    } else if (msg.indexOf(ansi.yellow) === 0) {
        printInColor('yellow', msg);
    } else if (msg.indexOf(ansi.green) === 0) {
        printInColor('green', msg);
    } else {
        console.log(msg);
    }
}, function(reporter) {
    // On complete
    phantom.exit(reporter.results().failedCount);
}, true));

// Launch tests
jasmineEnv.updateInterval = 500;
jasmineEnv.execute();

