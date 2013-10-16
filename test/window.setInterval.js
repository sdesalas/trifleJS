

/* window.setInterval() */

console.log('Starting..');
var tries = 0;

window.setInterval(function() {
    tries++;
    console.log('Tries..' + tries);
    if (tries > 9) {
        console.log('Finished.');
        phantom.exit();
    }
}, 1000);
