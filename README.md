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

### API Implementation

We are currently about 50-60% throguht the [PhantomJS API](http://phantomjs.org/api/).

For detailed information on which methods are available please refer to [triflejs.org](http://triflejs.org):

- [Global Methods](http://triflejs.org#post-11)
- [Object: phantom](http://triflejs.org#post-18)
- [Module: WebPage](http://triflejs.org#post-20)
- [Module: System](http://triflejs.org#post-24)
- [Module: FileSystem](http://triflejs.org#post-27)
- [Command Line Options](http://triflejs.org#post-29)

Some of the big ticket items currently missing from the automation are: 

- Cookies
- IE Windows (Alert, Confirm, Prompt, SSL Certificate error, etc)
- Mouse / Keyboard interaction
- WebServer and ChildProcess modules

Some items that are being deliberately left out:

- Support for WebDriver

### Download

This code is still very much in beta. Check again for updates.

You can download the binary [here](https://github.com/sdesalas/trifleJS/raw/master/Build/Binary/TrifleJS.Latest.zip) if you want to play around with the beta version. 

Please be aware of the following system requirements:

- Windows XP and above (including Server 2003+)
- IE 7+
- .NET framework 3.5 and above
