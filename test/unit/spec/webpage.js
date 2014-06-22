
assert.suite('WEBPAGE.JS');

assert.section('Instantiation');

assert(this.hasOwnProperty('WebPage'), 'this.WebPage exists');
assert(typeof this.WebPage === 'function', 'this.WebPage is a function');
assert(window.hasOwnProperty('WebPage'), 'window.WebPage exists');
assert(typeof window.WebPage === 'function', 'window.WebPage is a function');
var page = require("webpage");
assert(typeof page === 'object', 'require("webpage") returns an object');

assert.section('Properties before loading');

assert(page.url === 'about:blank', 'page.url is "about:blank"');
assert(page.viewportSize != null && page.viewportSize.width, 'page.viewportSize.width is 400');
assert(page.viewportSize != null && page.viewportSize.height, 'page.viewportSize.height is 300');


