
/*
* bootstrapper.js
*
* Initializes environment and runs triflejs unit tests 
* in phantomjs executable.
*
*/

console.log('=============================');
console.log('PhantomJS Benchmark Tests');
console.log('=============================');

trifle = {
	wait : function(ms) {
		var date = new Date()
		var page = phantom.page;
		while (new Date() < new Date(date.getTime() + ms)) { 
			// To emulate trifle.wait() we have
			// to clear the event queue while waiting
			// for timer to reach destination.
			// This can be accomplished using page.sendEvent()
			// which in turn clears the queue for us
			// with a call to QApplication::processEvents();
			// @see http://qt-project.org/wiki/ThreadsEventsQObjects#7494f2b4836907fc1c09311e3a0305e6
			// @see https://github.com/ariya/phantomjs/blob/1.9/src/webpage.cpp#L1394
			page.sendEvent('mousemove');
		}
	},
	Callback : function() {
		this.id = '';
		this.func = function() {};
		this.scope = {};
		defaultArgs = [];
	}
};

trifle.Callback.newUid = function() {};
trifle.Callback.execute = function() {};
trifle.Callback.executeOnce = function() {};
trifle.Callback.id = function() {};

Object.addEvent = function(obj) {
	obj.on = function() {}; 
	obj.fireEvent = function() {};
	obj.listeners = {};
};


console.API = {
	color: function(name, msg) {
		console.log(msg);
	}
}


// Add TrifleJS unit testing tools
phantom.injectJs('../../unit/tools.js');

// Add TrifleJS unit testing spec
phantom.injectJs('../../unit/spec/env.js');
//phantom.injectJs('../../unit/spec/require.js');


