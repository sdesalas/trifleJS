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

### [Object: `phantom`](https://github.com/ariya/phantomjs/wiki/API-Reference-phantom)

|Feature                                  | Status   | Notes                                |
|-----------------------------------------|----------|--------------------------------------|
|*__Properties__*                                                                           |
|`.args`                                  | Ready    | Arguments passed to the script.      |
|`.cookies`                               | --       | Get or set Cookies for any domain.   | 
|`.cookiesEnabled`                        | --       | Controls whether cookies are enabled | 
|`.version`                               | Ready    | The version of PhantomJS instance.   | 
|`.libraryPath`                           | Ready    | Used by `.injectJs()` to find scripts. |
|`.scriptName`                            | Ready    | Name of the invoked script file.     |
|*__Methods__*                                                                              |
|`.addCookie({cookie})`                   | --       | Add a Cookie to the CookieJar.       |
|`.clearCookies()`                        | --       | Delete all Cookies.                  |
|`.deleteCookie(name)`                    | --       | Deletes a Cookie.                    |
|`.exit(returnValue)`                     | Ready    | Exits program with return value.     |
|`.injectJS(filename)`                    | Ready    | Injects external scripts             |
|*__Events__*                                                                               |
|`#onError`                               | --       | Errors not caught by a WebPage#onError |

### [Module: `WebPage`](https://github.com/ariya/phantomjs/wiki/API-Reference-WebPage)

|Feature                                  | Status   | Notes                                |
|-----------------------------------------|----------|--------------------------------------|
|*__Properties__*                                                                           |
|`.canGoBack`                             | --       | `true` if browser can navigate back. |
|`.canGoForward`                          | --       | `true` if browser can navigate forward. |
|`.clipRect`                              | --       | Page area rasterized during `.render()` |
|`.content`                               | Ready    | HTML content of the web page.     |
|`.cookies`                               | --       | Get/set cookies visible in current URL.  |
|`.customHeaders`                         | Ready    | Additional HTTP headers sent to server. |
|`.paperSize`                             | --       | Size of the page when rendered as PDF.  |
|`.plainText`                             | Ready    | Content of the web page in plain text.  |
|`.settings`                              | --       | Various settings of the web page.    |
|`.scrollPosition`                        | --       | Scroll position of the web page.    |
|`.viewportSize`                          | Ready    | Size of viewport for the layout process. |
|`.url`                                   | Ready    | Current URL of the web page.         |
|`.title`                                 | Ready    | Title of the page.                  |
|`.windowName`                            | --       | Name of the main browser window.   |
|`.zoomFactor`                            | Ready    | Scaling factor for rendering pages. |
|*__Methods__*                                                                             |
|`.addCookie({cookie})`                   | --       | Add a cookie to the page.           |
|`.clearCookies()`                        | --       | Delete all Cookies for current domain. |
|`.close()`                               | Ready    | Closes page to release memory.      |
|`.deleteCookie(name)`                    | --       | Deletes a Cookie in current domain. |
|`.evaluate(function, arg1, arg2,..)`     | Ready    | Evaluates function inside current page.  |
|`.evaluateAsync(function)`               | --       | `evaluate()` without blocking execution. |
|`.evaluateJavaScript(str)`               | Ready    | Evaluates script in current page.  |
|`.includeJs(url, callback)`              | Ready    | Includes script the specified `url`  |
|`.injectJs(filename)`                    | Ready    | Injects script code specified file   |
|`.open(url, callback)`                   | Ready    | Opens `url` and loads it to the page. |
|`.open(url, method, callback)`           | Ready    | As above but using a HTTP method. |
|`.open(url, method, data, callback)`     | Ready    | As above, using a HTTP method and data. |
|`.openUrl(url, httpConf, settings)`      | --       | Opens `url` with specific settings.  |
|`.reload()`                              | --       | Reloads current page.                |
|`.render(filename)`                      | Ready    | Renders page to specified `filename` |
|`.renderBase64(format)`                  | Ready    | Renders page as Base64-encoded string |
|*__Events__*                                                                               |
|`#onAlert`                               | --       | Fires for `alert()` calls on page. | 
|`#onCallback`                            | Ready    | Fires for `window.callPhantom` calls. | 
|`#onConfirm`                             | --       | Fires for `confirm()` calls on page. | 
|`#onConsoleMessage`                      | --       | Fires for `console` messages on page. | 
|`#onError`                               | Partial  | Stacktrace not implemented yet       |
|`#onLoadStarted`                         | Ready    | Fires when page starts loading.     |
|`#onLoadFinished`                        | Ready    | Fires when page finishes loading.    |
|`#onPrompt`                              | --       | Fires when `prompt()` call made on page. | 

### [Module: `System`](https://github.com/ariya/phantomjs/wiki/API-Reference-System)

|Feature                                  | Status   | Notes                                |
|-----------------------------------------|----------|--------------------------------------|
|*__Properties__*                                                                           |
|`.pid`                                   | Ready    | Current Process ID for TrifleJS process. |
|`.platform`                              | Ready    | Name of platform, always `phantomjs`. |
|`.os`                                    | Ready    | Information about the operating system |
|`.env`                                   | Ready    | Key-value pairs of environment variables |
|`.args`                                  | Ready    | List of the command-line arguments |

### [Module: `FileSystem`](https://github.com/ariya/phantomjs/wiki/API-Reference-FileSystem)

|Feature                                  | Status   | Notes                                |
|-----------------------------------------|----------|--------------------------------------|
|*__Properties__*                                                                           |
|`.separator`                             | --       | The path separator for O/S.          |
|`.workingDirectory`                      | --       | The current working directory.        |
|*__Query Methods__*                                                                        |
|`.list(path)`                            | Ready    | Returns list of files in `path`.       |
|`.absolute(path)`                        | Ready    | Returns absolute path to file or directory |
|`.exists(path)`                          | Ready    | `true` if a file or a directory exists.   |
|`.isDirectory(path)`                     | Ready    | `true` if specified `path` is directory. |
|`.isFile(path)`                          | Ready    | `true` if specified `path` is a file  |
|*__Directory Methods__*                                                                        |
|`.changeWorkingDirectory(path)`          | Ready    | Changes the current `.workingDirectory` |
|`.makeDirectory(path)`                   | --       | Creates a directory                 |
|`.removeDirectory(path)`                 | --       | Removes a directory                 |
|`.copyTree(path)`                        | --       | Copies a directory tree.            |
|*__File Methods__*                                                                        |
|`.read(path)`                            | Ready    | Reads contents of a file.            |
|`.size(path)`                            | Ready    | Returns size of a file.             |
|`.remove(path)`                          | Ready    | Deletes a file.                      |
|`.copy(path)`                            | --       |                                      |

### [COMMAND LINE](https://github.com/ariya/phantomjs/wiki/API-Reference)

|Feature                                  | Status   | Notes                                |
|-----------------------------------------|----------|--------------------------------------|
|REPL input                               | Ready    | Runs interactive mode.               |
|`--help` or `-h`                         | Ready    | Lists command line options and quits. |
|`--version` or `-v`                      | Ready    | Targetted version of PhantomJS       |
|`--ignore-ssl-errors=[true/false]`       | --       | Ignores SSL errors.                  |
|`--load-images=[true/false]`             | --       | Load all inlined images (default `true`). |
|`--output-encoding=encoding`             | --       | Encoding for terminal output (default `utf8`).|
|`--proxy=address:port`                   | --       | Specifies the proxy server to use.    |
|`--proxy-type=[http/socks5/none]`        | --       | Specifies the type of the proxy server.   |
|`--proxy-auth=user:passw`                | --       | Authentication info for the proxy.  |
|`--script-encoding=encoding`             | --       | Encoding for starting script (default `utf8`).  |
|`--web-security=[true/false]`            | --       | Prevents cross-domain XHR (default `true`).  |
|`--config=/path/to/config.json`          | --       | JSON replacement for command switches.  |


## New features

These are additional features added into TrifleJS that are not present in PhantomJS

|Feature                                  | Status   | Notes                                |
|-----------------------------------------|----------|--------------------------------------|
|**COMMAND LINE**                         |
|`--test` or `t`                          | Partial  | Runs a full regression test          |
|`--emulate:(version)`                    | Ready    | Emulates specific IE versions        |
|`--render:(url)`                         | Ready    | Renders a URL to file and quits      | 


This code is still very much in beta. Check again for updates.

You can download the binary [here](https://github.com/sdesalas/trifleJS/raw/master/Build/Binary/TrifleJS.Latest.zip) if you want to play around with the beta version. 
