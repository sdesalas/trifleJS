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
                return this.SetTimeout((new trifle.Callback(function() {
                    callback.call(window);
                })).id, ms);
            }
        },
        clearTimeout: function(id) {
            console.xdebug('window.clearTimeout(' + id + ')');
            if (typeof id === 'number') {
                this.ClearTimeout(id);
            }
        },
        setInterval: function(callback, ms) {
            console.xdebug('window.setInterval(callback, ' + ms + ')');
            if (typeof callback === 'function' && typeof ms === 'number') {
                return this.SetInterval((new trifle.Callback(function() {
                    callback.call(window);
                })).id, ms);
            }
        },
        clearInterval: function(id) {
            console.xdebug('window.clearInterval(' + id + ')');
            if (typeof id === 'number') {
                this.ClearInterval(id);
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

   
	// Internal event handling 
	// @usage
	// Object.addEvent(phantom, 'onError', true);
	// Object.addEvent(page, 'onAlert');
	Object.addEvent = function(obj, eventName, unique) {
		if (obj && typeof eventName === 'string') {
			// Add event handling capability
			obj.listeners = obj.listeners || {};
			obj.fireEvent = function() {
				if (arguments.length && obj.listeners) {
					var name = Array.prototype.shift.call(arguments);
					var listeners = obj.listeners[name];
					if (listeners && listeners.length) {
						for(var i = 0; i < listeners.length; i++) {
							listeners[i].apply(obj, arguments);
						}
						return true;
					}
				}
				return false;
			};
			// Use setter/getter for event handling
			Object.defineProperty(obj, eventName, {
				set: function(listener) {
					if (obj.listeners) {
						if (typeof listener === 'function') {
							// Unique events only ever have one listener
							if (unique) {
								obj.listeners[eventName] = [listener];
							} else {
								obj.listeners[eventName] = obj.listeners[eventName] || [];
								obj.listeners[eventName].push(listener);
							}
						} else if (listener === null) {
							// Setting listener to null will wipe existing handler(s)
							delete obj.listeners[eventName];
						}
					}
				},
				get: function() {
					if (obj.listeners) {
						return obj.listeners[eventName] || [];
					}
				}
			});
		}
	};
	
    // Initialise phantom object
    var phantom = GLOBAL.phantom = API.phantom;


	// Define phantom event handler
	Object.addEvent(phantom, 'onError', true);


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
            if (method) {
                switch (args.length) {
                    case 0:
                        this.API[method]("");
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


