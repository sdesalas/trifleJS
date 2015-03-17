
/*
* run.js
*
* Initializes environment and runs triflejs unit tests 
* in phantomjs executable.
*
*/

console.log('=============================');
console.log('PhantomJS Benchmark Tests');
console.log('=============================');


(function Stubs() {

	trifle = {
		wait : function(ms) {
			var date = new Date()
			var page = phantom.page;
			do {
				// To emulate trifle.wait() we have
				// to clear the event queue while waiting
				// for timer to reach destination.
				// This can be accomplished using page.sendEvent()
				// which in turn clears the queue for us
				// with a call to QApplication::processEvents();
				// @see http://qt-project.org/wiki/ThreadsEventsQObjects#7494f2b4836907fc1c09311e3a0305e6
				// @see https://github.com/ariya/phantomjs/blob/1.9/src/webpage.cpp#L1394
				page.sendEvent('mousemove');
			} while (new Date() < new Date(date.getTime() + ms))
		}
	};

	console.API = {
		color: function(name, msg) {
			console.log(msg);
		}
	};

})();


// Add TrifleJS unit testing tools
phantom.injectJs('../../unit/tools.js');

// Add TrifleJS unit testing spec
//phantom.injectJs('../../unit/spec/env.js');
//phantom.injectJs('../../unit/spec/require.js');
//phantom.injectJs('../../unit/spec/phantom.js');
//phantom.injectJs('../../unit/spec/fs.js');
//phantom.injectJs('../../unit/spec/webserver.js');
phantom.injectJs('../../unit/spec/webpage.js');

// Finish off & Report
phantom.injectJs('../../unit/finish.js');

