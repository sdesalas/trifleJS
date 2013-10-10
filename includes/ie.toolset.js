
// Initialize toolset
window.__toolset = window.__toolset || {};

window.__toolset.test = 'hello world';

// Loads a JS file and executes a callback when ready
window.__toolset.includeJs = function(url, callbackId) {

    window.external.xdebug(['window.__toolset.includeJs(url, callbackId)', url, callbackId]);

    // Generate DOM for new <script/> tagg
    var head = document.getElementsByTagName("head")[0] || document.documentElement;
    var script = document.createElement("script");
    script.id = 'script' + callbackId;
    script.src = url;

    // Use insertBefore
    head.insertBefore(script, head.firstChild);

};

// Checks if a script is ready
window.__toolset.isScriptReady = function(callbackId) {

    window.external.xdebug(['window.__toolset.isScriptReady(callbackId)', callbackId]);

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