/*
* init.js
*
* By: Steven de Salas
* On: Sep 2013
* 
*
* Generates the core running environment for
* javascript code to execute under
* 
*/

(function(GLOBAL) {

    // Save imported params
    var API = {
        phantom: GLOBAL.phantom,
        trifle: GLOBAL.trifle,
        console: GLOBAL.console,
        decorate: function(obj, config) {
			for (var prop in config) {
				obj[prop] = config[prop];
			}
        }
    };

    delete GLOBAL.phantom;
    delete GLOBAL.trifle;
    delete GLOBAL.console;

    // Decorate window object
    API.decorate(window, {
        phantom: API.phantom,
        setTimeout: function(callback, ms) {
            console.xdebug('window.setTimeout(callback, ' + ms + ')');
            if (typeof callback === 'function' && typeof ms === 'number') {
                return GLOBAL.window.SetTimeout(trifle.Callback.id(function() {
                    callback.call(window);
                }), ms);
            }
        },
        clearTimeout: function(id) {
            console.xdebug('window.clearTimeout(' + id + ')');
            if (typeof id === 'number') {
                GLOBAL.window.ClearTimeout(id);
            }
        },
        setInterval: function(callback, ms) {
            console.xdebug('window.setInterval(callback, ' + ms + ')');
            if (typeof callback === 'function' && typeof ms === 'number') {
                return GLOBAL.window.SetInterval(trifle.Callback.id(function() {
                    callback.call(window);
                }), ms);
            }
        },
        clearInterval: function(id) {
            console.xdebug('window.clearInterval(' + id + ')');
            if (typeof id === 'number') {
                GLOBAL.window.ClearInterval(id);
            }
        },
        addEventListener: function(event, callback, useCapture) {
            if (event == "DOMContentLoaded") {
                setTimeout(function() {
                    var page = WebPage.current;
                    // Check if its ready
                    if (page.content) {
                        callback();
                    } else {
                        // No? Add event listener
                        page.onLoadFinished = function(status) {
                            callback();
                        }
                    }
                }, 1);
            }
        }
    });

    // Apply to global
    GLOBAL.location = window.location;
    GLOBAL.navigator = window.navigator;
    GLOBAL.setTimeout = window.setTimeout;
    GLOBAL.setInterval = window.setInterval;
    GLOBAL.clearTimeout = window.clearTimeout;
    GLOBAL.clearInterval = window.clearInterval;
    GLOBAL.addEventListener = window.addEventListener;


    // Console object
    var console = GLOBAL.console = {
        API: API.console,
        clear: function() {
            this.API.clear();
        },
        log: function() {
            this._do('log', arguments);
        },
        error: function() {
            this._do('error', arguments);
        },
        xdebug: function() {
            this._do('xdebug', arguments);
        },
        debug: function() {
            this._do('debug', arguments);
        },
        warn: function() {
            this._do('warn', arguments);
        },
        _do: function(method, args) {
            if (method && args) {
                switch (args.length) {
                    case 0:
                        // Do nothing
                        // @ref phantomjs> console.log();
                        break;
                    case 1:
                        if (typeof args[0] === 'function') { args[0] = args[0].toString(); }
                        this.API[method](args[0]);
                        break;
                    default:
                        var params = [];
                        for (var i = 0; i < args.length; i++) {
                            if (typeof args[i] === 'function') { args[i] = args[i].toString(); }
                            params[i] = args[i];
                        }
                        this.API[method](params);
                        break;
                }
            }
        }
    };

	// Internal event handling 
	// @usage
	// Object.addEvent(phantom, 'onError', true);
	// phantom.on('error', function(e) { var message = e.message; }
	// phantom.onError = function(e) {var message = e.message; };
	Object.addEvent = function(obj, eventFullName, unique) {
		if (obj && typeof eventFullName === 'string' && eventFullName.length > 2) {
			// Add event handling capability
			if (eventFullName.substr(0, 2) !== 'on') {
				throw new Error("Event names should start with 'on'");
			}
			var eventName = eventFullName.substr(2).toLowerCase();
			obj.listeners = obj.listeners || {};
			obj.listeners[eventName] = {callbacks: [], unique: unique}
			obj.fireEvent = function(name, args, scope) {
				var result;
				if (name && this.listeners) {
					var listener = this.listeners[name];
					var scope = scope || this;
					if (listener && listener.callbacks) {
						for(var i = 0; i < listener.callbacks.length; i++) {
							result = listener.callbacks[i].apply(scope, args || []);
						}
					}
				}
				return result;
			};
			obj.on = function(name, func) {
				if (name && func && obj.listeners) {
					if (!obj.listeners[name]) {
						obj.listeners[name] = {};
					}
					var event = obj.listeners[name];
					if (typeof func === 'function') {
						// Unique events only ever have one listener
						if (event.unique) {
							event.callbacks = [func];
						} else {
							event.callbacks = event.callbacks || [];
							event.callbacks.push(func);
						}
					}
				}
			}
			// Use a function call to store event names in closure scope.
			// We need to do this as a getter/setter does not know its own name
			// when executing.
			var ___defineGetterSetter = function (eventName, eventFullName) {
				// Use setter/getter for event handling
				Object.defineProperty(obj, eventFullName, {
					set: function(listener) {
						if (this.listeners) {
							if (typeof listener === 'function') {
								this.on(eventName, listener);
							} else if (listener === null) {
								// Setting listener to null will wipe existing handler(s)
								delete this.listeners[eventName];
							}
						}
					},
					get: function() {
						if (this.listeners && this.listeners[eventName]) {
							var callbacks = this.listeners[eventName].callbacks;
							// Return last bound event
							if (callbacks && callbacks.length) {
								return callbacks[callbacks.length-1];
							}
						}
					}
				});
			}
			___defineGetterSetter(eventName, eventFullName);
		}
	};
	
    // Initialise phantom object
    var phantom = GLOBAL.phantom = API.phantom;

	// Define phantom default error handler
	Object.addEvent(phantom, 'onError', true);
	phantom.onError = function(msg, trace) {
		if (trace && trace[0]) {
			var err = trace[0];
			console.error(err.file + ' (' + err.line + ',' + err.col + '):' + msg);
		} else {
			console.error(msg);
		}
	};

    // TrifleJS object
    var trifle = GLOBAL.trifle = {
        API: API.trifle,
        version: API.trifle.Version,
        emulation: API.trifle.Emulation,
        wait: function(ms) {
            return API.trifle.Wait(ms || 0);
        },
        // extends a module class
        extend: function(config) {
            if (!config || !config.module || !config.module.call) {
                throw new Error('trifle.extend() called incorrectly');
            }
            return function() {
                var API = config.module();
                if (config.init) {
                    config.init.apply(API, arguments);
                }
                if (config.methods) {
                    for (var method in config.methods) {
                        API[method] = config.methods[method];
                    }
                }
                return API;
            }
        }
    };


    // Loading module framework
    // @see http://wiki.commonjs.org/wiki/Modules/1.1.1
    // @see https://github.com/mschwartz/SilkJS/blob/master/builtin/require.js
    // @see http://wiki.commonjs.org/wiki/Implementations
    var require = GLOBAL.require = function(module) {

		// Normalise paths
		module = module.replace("\\", "/");

        // Initialise if required
        if (typeof require.cache.initialise === 'function') {
            require.cache.initialise.call({});
        }

        // Look in cache for built-in modules and such
        if (require.cache && require.cache[module]) {
            return require.cache[module].exports;
        }

        // Try load from file system
        var fs = new trifle.modules.FileSystem();

        // Is it a file path?
        if (fs.exists(module)) {
            // Add to cache and load
            var path = fs.absolute(module);
            require.cache[path] = { id: path, exports: { id: path } };
            phantom.createModule(path, fs.read(path));
            return require.cache[path].exports;

        // Is it a file path without the extension?
        } else if (fs.exists(module + '.js')) {
            // Add to cache and load
            var path = module + '.js';
            require.cache[path] = { id: path, exports: { id: path } };
            phantom.createModule(path, fs.read(path));
            return require.cache[path].exports;

        } else {
            console.error('Cannot find module "' + module + '"');
        }

    }

    // Define preloaded exports
    require.cache = {
        initialise: function() {
            require.cache = {
                fs: {
                    id: 'fs', 
                    exports: new trifle.modules.FileSystem()
                },
                system: {
                    id: 'system', 
                    exports: new trifle.modules.System()
                },
                webpage: {
                    id: 'webpage', 
                    exports: {
                        create: function() { return  new trifle.modules.WebPage(); }
                    }
                },
                webserver: {
                    id: 'webserver', 
                    exports: {
                        create: function() { return new trifle.modules.WebServer(); }
                    }
                },
                child_process: {
					id: 'child_process',
					exports: new trifle.modules.ChildProcess()
                }
            }
        }
    }

})(this);


