
console.log(phantom);

var success = phantom.injectJs('..\\..\\test\\lib\\jasmine.js');

if (!success) {
    console.error('File not found');
    phantom.exit();
}

if (jasmine) {
    console.log('Jasmine library injected ok');
} else {
    console.error('Could not inject Jasmine library');
}
phantom.exit();
