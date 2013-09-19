trifleJS
=========

An Headless Internet Explorer browser using the [.NET WebBrowser Class](http://msdn.microsoft.com/en-us/library/system.windows.forms.webbrowser.aspx) with a Javascript API running on the [V8 JavaScript engine](http://en.wikipedia.org/wiki/V8_(JavaScript_engine\)).

The API is coded as a port of PhantomJS to reduce the learning curve.

It supports different version of IE (IE7, IE8, IE9 and IE10) interchangeably depending on the current version installed (IE9 can run as IE7, IE8 or IE9 but not IE10).

The following is a list of features that have been ported from PhantomJS.

|Feature                                  | Status   | Notes                        |
|-----------------------------------------|----------|------------------------------|
|**phantom**                              |
|phantom.version                          | Yep      |                              |
|phantom.exit()                           | Yep      |                              |
|**webpage**                              |
|webpage.open(url, callback)              | Yep      | Callback included            |
|webpage.render(filename)                 | Yep      |                              |
|webpage.renderBase64(format)             | Yep      |                              |
|webpage.evaluate(function, arg1, arg2,..)| Yep      |                              |
|webpage.evaluateJavaScript(str)          | Yep      |                              |
|webpage.includeJs(url, callback)         | Partial  | No Callback yet              |
|webpage.injectJs(filename)               | Yep      |                              |
|webpage#onConsoleMessage                 | Nope     |                              |
|webpage#onError                          | Nope     |                              |
|webpage#onLoadStarted                    | Yep      |                              |
|webpage#onLoadFinished                   | Yep      |                              |
