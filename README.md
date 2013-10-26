trifleJS 0.2
=========

A headless Internet Explorer browser using the [.NET WebBrowser Class](http://msdn.microsoft.com/en-us/library/system.windows.forms.webbrowser.aspx) with a Javascript API running on the [V8 JavaScript engine](http://en.wikipedia.org/wiki/V8_(JavaScript_engine\)).

The API is coded as a port of [PhantomJS](http://phantomjs.org). Basically, if you have used phantom before then you already know how to use TrifleJS.

![What is TrifleJS?](https://raw.github.com/sdesalas/trifleJS/master/Docs/What.Is.Trifle.png "What is TrifleJS?")

It supports different version of IE interchangeably depending on the current version installed (IE9 can emulate IE7, IE8 or IE9 but not IE10).

    C:\> TrifleJS.exe --emulate:IE8 --render:http://whatbrowser.org/

![IE 8](https://raw.github.com/sdesalas/trifleJS/master/Docs/whatbrowser.org.IE8.png "Running as IE 8")

The following is a list of features that have been ported from PhantomJS.

|Feature                                  | Status   | Notes                                |
|-----------------------------------------|----------|--------------------------------------|
|**[phantom](https://github.com/ariya/phantomjs/wiki/API-Reference-phantom)**                              |
|.version                                 | Yep      |                                      | 
|.exit()                                  | Yep      |                                      |
|.injectJS()                              | Yep      |                                      |
|.libraryPath                             | Yep      |                                      |
|.args                                    | Yep      |                                      |
|**[webpage](https://github.com/ariya/phantomjs/wiki/API-Reference-WebPage)**                              | 
|.open(url, callback)                     | Yep      | Callback included                    |
|.render(filename)                        | Yep      |                                      |
|.renderBase64(format)                    | Yep      |                                      |
|.evaluate(function, arg1, arg2,..)       | Yep      |                                      |
|.evaluateJavaScript(str)                 | Yep      |                                      |
|.includeJs(url, callback)                | Yep      | Callback included                    |
|.injectJs(filename)                      | Yep      |                                      |
|#onConsoleMessage                        | Nope     |                                      | 
|#onError                                 | Nope     |                                      |
|#onLoadStarted                           | Yep      |                                      |
|#onLoadFinished                          | Yep      |                                      |
|**[system](https://github.com/ariya/phantomjs/wiki/API-Reference-System)**                               |
|.args                                    | Yep      |                                      |
|**[fs](https://github.com/ariya/phantomjs/wiki/API-Reference-FileSystem)**                                   |
|.changeWorkingDirectory()                | Yep      |                                      |
|**[COMMAND LINE](https://github.com/ariya/phantomjs/wiki/API-Reference)**                         |
|REPL input                               | Yep      |                                      |
|--version                                | Yep      |                                      |

##New features

These are additional features added into TrifleJS that are not present in PhantomJS

|Feature                                  | Status   | Notes                                |
|-----------------------------------------|----------|--------------------------------------|
|**COMMAND LINE**                         |
|--emulate:(version)                      | Yep      | Emulates specific IE versions        |
|--render:(url)                           | Yep      | Renders a URL to file and quits      | 


This code is still very much in beta. Check again for updates.

You can download the binary [here](https://github.com/sdesalas/trifleJS/raw/master/Build/Binary/TrifleJS.Latest.zip) if you want to play around with the beta version. 
