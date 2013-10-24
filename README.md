trifleJS 0.1
=========

An Headless Internet Explorer browser using the [.NET WebBrowser Class](http://msdn.microsoft.com/en-us/library/system.windows.forms.webbrowser.aspx) with a Javascript API running on the [V8 JavaScript engine](http://en.wikipedia.org/wiki/V8_(JavaScript_engine\)).

The API is coded as a port of [PhantomJS](http://phantomjs.org) to reduce the learning curve.

![What is TrifleJS?](https://raw.github.com/sdesalas/trifleJS/master/Docs/What.Is.Trifle.png "What is TrifleJS?")

It supports different version of IE interchangeably depending on the current version installed (IE9 can run as IE7, IE8 or IE9 but not IE10).

    C:\> TrifleJS.exe --version:IE8 --render:http://whatbrowser.org/

![IE 8](https://raw.github.com/sdesalas/trifleJS/master/Docs/whatbrowser.org.IE8.png "Running as IE 8")

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
|webpage.includeJs(url, callback)         | Yep      | Callback included            |
|webpage.injectJs(filename)               | Yep      |                              |
|webpage#onConsoleMessage                 | Nope     |                              |
|webpage#onError                          | Nope     |                              |
|webpage#onLoadStarted                    | Yep      |                              |
|webpage#onLoadFinished                   | Yep      |                              |

This code is still very much in beta. Check again for updates.

You can download the binary [here](https://github.com/sdesalas/trifleJS/raw/master/Build/Binary/TrifleJS.zip) if you want to play around with the beta version. 
