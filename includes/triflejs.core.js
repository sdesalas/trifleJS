
// Initialise Namespace
this.triflejs = this.triflejs || {};

(function(GLOBAL) {

    // Add UID generation
    triflejs.uid = function() {
        var s4 = function() {
            return Math.floor((1 + Math.random()) * 0x10000)
             .toString(16)
             .substring(1);
        };

        return s4() + s4();
    };

    // Set interop inside trifle
    triflejs._interop = GLOBAL._interop;
    delete GLOBAL._interop;

    // Initialize callback hashmap
    triflejs.callbacks = {};

    // Callback Class
    // Define Constructor
    var Callback = triflejs.Callback = function(func, scope, defaultArgs) {
        this.func = func;
        this.scope = scope;
        this.defaultArgs = defaultArgs;
        this.id = triflejs.uid();
        console.debug('new Callback#' + this.id + '(func, scope, defaultArgs)');
        triflejs.callbacks[this.id] = this;
    };

    // Callback execution
    Callback.prototype.execute = function() {
        console.debug('Callback#' + this.id + '.prototype.execute()');
        this.func.apply(this.scope || this, arguments || this.defaultArgs)
    }


    // PhantomJs
    this.phantom = this.phantom || {};
    phantom.version = { 'major': 1, 'minor': 0, 'patch': 0 };


    // Console
    this.console = {
        host: console, // Import host object
        clear: function() {
            this.host.clear();
        },
        log: function() {
            this._do('log', arguments);
        },
        error: function() {
            this._do('error', arguments);
        },
        debug: function() {
            this._do('debug', arguments);
        },
        warning: function() {
            this._do('warning', arguments);
        },
        _do: function(method, args) {
            if (method) {
                switch (args.length) {
                    case 0:
                        this.host[method]("");
                        break;
                    case 1:
                        this.host[method](args[0]);
                        break;
                    default:
                        var params = [];
                        for (var i = 0; i < args.length; i++) {
                            params[i] = args[i];
                        }
                        this.host[method](params);
                        break;
                }
            }
        }
    };

})(this);


