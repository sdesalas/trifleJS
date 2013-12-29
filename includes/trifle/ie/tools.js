
/*
// Initialize tools
window.__tools = window.__tools || {};

// Loads a JS file and executes a callback when ready
window.__tools.includeJs = function(url, callbackId) {

    window.external.xdebug(['window.__tools.includeJs(url, callbackId)', url, callbackId]);

    // Generate DOM for new <script/> tagg
    var head = document.getElementsByTagName("head")[0] || document.documentElement;
    var script = document.createElement("script");
    script.id = 'script' + callbackId;
    script.src = url;

    // Use insertBefore
    head.insertBefore(script, head.firstChild);

};

// Checks if a script is ready
window.__tools.isScriptReady = function(callbackId) {

    window.external.xdebug(['window.__tools.isScriptReady(callbackId)', callbackId]);

    var script = document.getElementById('script' + callbackId);
    var head = document.getElementsByTagName("head")[0] || document.documentElement;
    if (script && script.readyState) {
        if (script.readyState === 'loaded' || script.readyState === 'complete') {
            window.external.doCallback(callbackId);
            if (head && script.parentNode) {
                head.removeChild(script);
            }
            return true;
        }
    }
    return false;
}
*/

/**
* WINDOW EXTENSTIONS & OVERRIDES
*/

// Add OnCallback functionality
window.callPhantom = function() {
    window.external.xdebug('window.callPhantom(args)');
    var args = [];
    for (var i = 0; i < arguments.length; i++) {
        args.push(arguments[i]);
    }
    return window.external.callPhantom(JSON.stringify(args));
}

// Override javascript alert
window.alert = function(message) {
	window.external.xdebug('window.alert()');
	message = message + "";
	return window.external.dialog('onAlert', message, null);
}

// Override javascript confirm
window.confirm = function(message) {
	window.external.xdebug('window.confirm()');
	message = message + "";
	return window.external.dialog('onConfirm', message, null);
}

// Override javascript prompt.
window.prompt = function(message, defaultValue) {
	window.external.xdebug('window.prompt()');
	message = message + "";
	return window.external.dialog('onPrompt', message, defaultValue || "");
}

