trifleJS 0.3
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
|.args                                    | Ready    | Arguments passed to the script.      |
|.cookies                                 | --       | Get or set Cookies for any domain.   | 
|.cookiesEnabled                          | --       | Controls whether cookies are enabled | 
|.version                                 | Ready    | The version of PhantomJS instance.   | 
|.libraryPath                             | Ready    | Used by `.injectJs()` to find scripts. |
|.scriptName                              | Ready    | Name of the invoked script file.     |
|*__Methods__*                                                                              |
|.addCookie(`{cookie}`)                   | --       | Add a Cookie to the CookieJar.       |
|.clearCookies()                          | --       | Delete all Cookies.                  |
|.deleteCookie(`name`)                    | --       | Deletesa Cookie.                     |
|.exit(`returnValue`)                     | Ready    | Exits program with return value.     |
|.injectJS(`filename`)                    | Ready    | Injects external scripts             |
|*__Events__*                                                                               |
|#onError                                 | --       | Errors not caught by a WebPage#onError |

### [Module: WebPage](https://github.com/ariya/phantomjs/wiki/API-Reference-WebPage)

|Feature                                  | Status   | Notes                                |
|-----------------------------------------|----------|--------------------------------------|
|*__Properties__*                                                                           |
|.canGoBack                               | --       |                                      |
|.canGoForward                            | --       |                                      |
|.clipRect                                | --       |                                      |
|.content                                 | --       |                                      |
|.cookies                                 | --       |                                      |
|.customHeaders                           | --       |                                      |
|.plainText                               | --       |                                      |
|.settings                                | --       |                                      |
|.viewportSize                            | --       |                                      |
|.url                                     | --       |                                      |
|.title                                   | --       |                                      |
|*__Methods__*                                                                             |
|.addCookie()                             | --       |                                     |
|.clearCookies()                          | --       |                                     |
|.deleteCookie()                          | --       |                                     |
|.evaluate(`function`, `arg1`, `arg2`,..) | Ready    |                                      |
|.evaluateAsync(`function`)               | --       |                                     |
|.evaluateJavaScript(`str`)               | Ready    |                                      |
|.includeJs(`url`, `callback`)            | Ready    | Callback included                    |
|.injectJs(`filename`)                    | Ready    |                                      |
|.open(`url`, `callback`)                 | Ready    | Callback included                    |
|.openUrl()                               | --       |                                      |
|.reload()                                | --       |                                      |
|.render(`filename`)                      | Ready    |                                      |
|.renderBase64(`format`)                  | Ready    |                                      |
|*__Events__*                                                                               |
|#onAlert                                 | --       |                                      | 
|#onCallback                              | Ready    |                                      | 
|#onConfirm                               | --       |                                      | 
|#onConsoleMessage                        | --       |                                      | 
|#onError                                 | Partial  | Stacktrace not implemented yet       |
|#onLoadStarted                           | Ready    |                                      |
|#onLoadFinished                          | Ready    |                                      |
|#onPrompt                                | --       |                                      | 

### [Module: System](https://github.com/ariya/phantomjs/wiki/API-Reference-System)

|Feature                                  | Status   | Notes                                |
|-----------------------------------------|----------|--------------------------------------|
|*__Properties__*                                                                           |
|.pid                                     | --       | Current Process ID for TrifleJS process. |
|.platform                                | --       | Name of platform, always `phantomjs`. |
|.os                                      | --       | Information about the operating system |
|.env                                     | --       | Key-value pairs of environment variables |
|.args                                    | Ready    | List of the command-line arguments |

### [Module: FileSystem](https://github.com/ariya/phantomjs/wiki/API-Reference-FileSystem)

|Feature                                  | Status   | Notes                                |
|-----------------------------------------|----------|--------------------------------------|
|*__Properties__*                                                                           |
|.separator                               | --       | The path separator for O/S.          |
|.workingDirectory                        | --       | The current working directory.        |
|*__Methods__*                                                                              |
|.changeWorkingDirectory(`path`)          | Ready    | Changes the current `.workingDirectory` |
|.list(`path`)                            | --       | Returns list of files in `path`.       |
|.absolute(`path`)                        | --       |                                      |
|.exists(`path`)                          | --       | `true` if a file or a directory exists.   |
|.isDirectory(`path`)                     | --       | `true` if specified `path` is directory. |
|.isFile(`path`)                          | --       |                                      |
|.read(`path`)                            | --       |                                      |
|.size(`path`)                            | --       |                                      |
|.remove(`path`)                          | --       |                                      |
|.copy(`path`)                            | --       |                                      |

### [COMMAND LINE](https://github.com/ariya/phantomjs/wiki/API-Reference)

|Feature                                  | Status   | Notes                                |
|-----------------------------------------|----------|--------------------------------------|
|REPL input                               | Ready    |                                      |
|--version                                | Ready    |                                      |

## New features

These are additional features added into TrifleJS that are not present in PhantomJS

|Feature                                  | Status   | Notes                                |
|-----------------------------------------|----------|--------------------------------------|
|**COMMAND LINE**                         |
|--emulate:(version)                      | Ready    | Emulates specific IE versions        |
|--render:(url)                           | Ready    | Renders a URL to file and quits      | 


This code is still very much in beta. Check again for updates.

You can download the binary [here](https://github.com/sdesalas/trifleJS/raw/master/Build/Binary/TrifleJS.Latest.zip) if you want to play around with the beta version. 
