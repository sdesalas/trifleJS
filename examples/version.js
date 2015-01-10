

var phantom_version = Object.keys(phantom.version).map(function(k) {return phantom.version[k]});
var trifle_version = Object.keys(trifle.version).map(function(k) {return trifle.version[k]});
var ie_version = trifle.emulation;

console.log('phantomjs -> ' + phantom_version.join('.'));
console.log('triflejs -> ' + trifle_version.join('.'));
console.log('internet explorer -> ' + ie_version);
console.log('userAgent -> ' + navigator.userAgent);

