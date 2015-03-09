
/**
* TRIFLEJS IE TOOLS
* By: Steven de Salas
*/

// Add OnCallback functionality
window.callPhantom = function() {
    window.external.xdebug('window.callPhantom(args)');
    var args = [];
    for (var i = 0; i < arguments.length; i++) {
        args.push(arguments[i]);
    }
    return window.external.fireEvent('callback', JSON.stringify(args));
}

// Override javascript alert
window.alert = function(message) {
	window.external.xdebug('window.alert()');
	message = message + "";
	return window.external.fireEvent('alert', JSON.stringify([message]));
}

// Override javascript confirm
window.confirm = function(message) {
	window.external.xdebug('window.confirm()');
	message = message + "";
	return window.external.fireEvent('confirm', JSON.stringify([message]));
}

// Override javascript prompt.
window.prompt = function(message, defaultValue) {
	window.external.xdebug('window.prompt()');
	message = message + "";
	return window.external.fireEvent('prompt', JSON.stringify([message, defaultValue || ""]));
}

// Capture javascript errors
window.onerror = function(msg, url, line, column, e) {
	var caller = arguments.callee ? arguments.callee.caller : '';
	var trace = [{url: url, line: line, column: column, func: caller.toString()}];
	while (!!caller.caller) {
		trace.push({func: caller.caller.toString()});
		caller = caller.caller;
	} 
	return window.external.fireEvent('error', JSON.stringify([msg, trace]));
}




