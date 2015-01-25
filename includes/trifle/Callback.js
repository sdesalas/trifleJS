/*
* Callback.js
*
* By: Steven de Salas
* On: Sep 2013
* 
* Generates a Callback object used for async
* communication between V8 and C# runtime.
* 
*/

// Initialise Namespace
this.trifle = this.trifle || {};

// Wrap code to avoid global vars
(function(trifle) {

    // Closure variable that tracks existing callbacks
    // (hidden from outside world)
    var callbacks = {};

    // Callback Class
    // Define Constructor
    var Callback = trifle.Callback = function(func, scope, defaultArgs) {
        this.func = func;
        this.scope = scope;
        this.defaultArgs = defaultArgs;
        this.id = Callback.newUid();
        console.xdebug('new Callback#' + this.id + '(func, scope, defaultArgs)');
        callbacks[this.id] = this;
    };

    // Unique ID Generator
    Callback.newUid = function() {
        var s4 = function() {
            return Math.floor((1 + Math.random()) * 0x10000)
                 .toString(16)
                 .substring(1);
        };
        return s4() + s4();
    };

    // Execute callback
    Callback.execute = function(id, args) {
        console.xdebug('Callback.execute("' + id + '", [args])');
        var callback = callbacks[id];
        if (callback) {
            if (!args || !args.length) {
                args = callback.defaultArgs;
            }
            callback.func.apply(callback.scope || callback, args);
        }
    }

    // Execute callback and delete reference
    Callback.executeOnce = function(id, args) {
        console.xdebug('Callback.executeOnce("' + id + '", [args])');
        if (typeof id === 'string') {
            Callback.execute(id, args);
            delete callbacks[id];
        }
    }
    
    // Generates a callback and returns the id
    Callback.id = function(func, scope, defaultArgs) {
		return (new Callback(func, scope, defaultArgs)).id;
    };


})(this.trifle);

