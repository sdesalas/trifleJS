trifleJS 0.4
=========

A headless Internet Explorer browser using the [.NET WebBrowser Class](http://msdn.microsoft.com/en-us/library/system.windows.forms.webbrowser.aspx) with a Javascript API running on the [V8 JavaScript Engine](http://en.wikipedia.org/wiki/V8_(JavaScript_engine)).

The API is coded as a port of [PhantomJS](http://phantomjs.org). Basically, if you have used phantom before then you already know how to use TrifleJS.

![What is TrifleJS?](https://raw.github.com/sdesalas/trifleJS/master/Docs/What.Is.Trifle.png "What is TrifleJS?")

It supports different version of IE interchangeably depending on the current version installed (IE9 can emulate IE7, IE8 or IE9 but not IE10).

    C:\> TrifleJS.exe --emulate=IE8 --render=http://whatbrowser.org/

![IE 8](https://raw.github.com/sdesalas/trifleJS/master/Docs/whatbrowser.org.IE8.png "Running as IE 8")

The following is a list of features that have been ported from PhantomJS. 

We are targetting version 1.7 as webdriver support (added in v 1.8) is too much work to put in at this stage and provides only marginal benefit.

### API Implementation

We are a bit over half-way throguht the [PhantomJS API](http://phantomjs.org/api/).

- [API Status (58%)](http://triflejs.org#post-112)

More information on each component is available on [triflejs.org](http://triflejs.org):

- [Global Methods](http://triflejs.org#post-11)
- [Object: phantom](http://triflejs.org#post-18)
- [Module: WebPage](http://triflejs.org#post-20)
- [Module: System](http://triflejs.org#post-24)
- [Module: FileSystem](http://triflejs.org#post-27)
- [Module: WebServer](http://triflejs.org#post-63)
- [Command Line Options](http://triflejs.org#post-29)

Some of the big ticket items currently missing from the automation are: 

- Cookies
- IE Windows (File Upload, SSL Certificate Error)
- Mouse / Keyboard interaction
- ChildProcess module

Some items that are being deliberately left out:

- Support for WebDriver

### Download

This code is still very much in beta. Check again for updates.

You can download the binary [here](https://github.com/sdesalas/trifleJS/raw/master/Build/Binary/TrifleJS.Latest.zip) if you want to play around with the beta version. 

#### System Requirements

The ideal installation is Windows 7 (which includes .NET 3.5) using IE9 or IE10, however this will still work in 32-bit XP (with SP2) or versions of the [Windows NT Kernel](http://en.wikipedia.org/wiki/Comparison_of_Microsoft_Windows_versions#Windows_NT) released after 2001.

|Software           | Version                       |
|-------------------|-------------------------------|
|Windows            | XP (SP2), 7, 8, Server 2003+  |
|Internet Explorer  | 7, 8, 9 or 10                 |
|.NET Framework     | 3.5+                          |

