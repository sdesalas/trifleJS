trifleJS 0.2
=========

A headless Internet Explorer browser using the [.NET WebBrowser Class](http://msdn.microsoft.com/en-us/library/system.windows.forms.webbrowser.aspx) with a Javascript API running on the [V8 JavaScript engine](http://en.wikipedia.org/wiki/V8_(JavaScript_engine\)).

The API is coded as a port of [PhantomJS](http://phantomjs.org). Basically, if you have used phantom before then you already know how to use TrifleJS.

![What is TrifleJS?](https://raw.github.com/sdesalas/trifleJS/master/Docs/What.Is.Trifle.png "What is TrifleJS?")

It supports different version of IE interchangeably depending on the current version installed (IE9 can emulate IE7, IE8 or IE9 but not IE10).

    C:\> TrifleJS.exe --emulate:IE8 --render:http://whatbrowser.org/

![IE 8](https://raw.github.com/sdesalas/trifleJS/master/Docs/whatbrowser.org.IE8.png "Running as IE 8")

The following is a list of features that have been ported from PhantomJS. 

We are targetting version 1.7 as webdriver support (added in v 1.8) is too much work to put in at this stage.

### Global Methods (Common JS)

|Feature                                  | Status   | Notes                                |
|-----------------------------------------|----------|--------------------------------------|
|.[require()](https://github.com/ariya/phantomjs/wiki/API-Reference#function-require)   | Yep      | Initialises CommonJS modules         |

### [Object: phantom](https://github.com/ariya/phantomjs/wiki/API-Reference-phantom)

|Feature                                  | Status   | Notes                                |
|-----------------------------------------|----------|--------------------------------------|
|*__Properties__*                                                                           |
|.args                                    | Yep      |                                      |
|.cookies                                 | Nope     |                                      | 
|.cookiesEnabled                          | Nope     |                                      | 
|.version                                 | Yep      |                                      | 
|.libraryPath                             | Yep      |                                      |
|*__Functions__*                                                                            |
|.addCookie()                             | Nope      |                                     |
|.clearCookies()                          | Nope      |                                     |
|.deleteCookie()                          | Nope      |                                     |
|.exit()                                  | Yep      |                                      |
|.injectJS()                              | Yep      |                                      |
|*__Callbacks__*                                                                            |
|#onError                                 | Nope      |                                     |

### [Module: WebPage](https://github.com/ariya/phantomjs/wiki/API-Reference-WebPage)

|Feature                                  | Status   | Notes                                |
|-----------------------------------------|----------|--------------------------------------|
|*__Properties__*                                                                           |
|.viewportSize                            | Nope     |                                      |
|.url                                     | Nope     |                                      |
|.title                                   | Nope     |                                      |
|*__Functions__*                                                                            |
|.open(url, callback)                     | Yep      | Callback included                    |
|.render(filename)                        | Yep      |                                      |
|.renderBase64(format)                    | Yep      |                                      |
|.evaluate(function, arg1, arg2,..)       | Yep      |                                      |
|.evaluateJavaScript(str)                 | Yep      |                                      |
|.includeJs(url, callback)                | Yep      | Callback included                    |
|.injectJs(filename)                      | Yep      |                                      |
|*__Callbacks__*                                                                            |
|#onAlert                                 | Nope     |                                      | 
|#onCallback                              | Yep      |                                      | 
|#onConsoleMessage                        | Nope     |                                      | 
|#onError                                 | Partial  | Stacktrace not implemented yet       |
|#onLoadStarted                           | Yep      |                                      |
|#onLoadFinished                          | Yep      |                                      |

### [Module: System](https://github.com/ariya/phantomjs/wiki/API-Reference-System)

|Feature                                  | Status   | Notes                                |
|-----------------------------------------|----------|--------------------------------------|
|.args                                    | Yep      |                                      |

### [Module: FileSystem](https://github.com/ariya/phantomjs/wiki/API-Reference-FileSystem)

|Feature                                  | Status   | Notes                                |
|-----------------------------------------|----------|--------------------------------------|
|.changeWorkingDirectory()                | Yep      |                                      |

### [COMMAND LINE](https://github.com/ariya/phantomjs/wiki/API-Reference)

|Feature                                  | Status   | Notes                                |
|-----------------------------------------|----------|--------------------------------------|
|REPL input                               | Yep      |                                      |
|--version                                | Yep      |                                      |

## New features

These are additional features added into TrifleJS that are not present in PhantomJS

|Feature                                  | Status   | Notes                                |
|-----------------------------------------|----------|--------------------------------------|
|**COMMAND LINE**                         |
|--emulate:(version)                      | Yep      | Emulates specific IE versions        |
|--render:(url)                           | Yep      | Renders a URL to file and quits      | 


This code is still very much in beta. Check again for updates.

You can download the binary [here](https://github.com/sdesalas/trifleJS/raw/master/Build/Binary/TrifleJS.Latest.zip) if you want to play around with the beta version. 
