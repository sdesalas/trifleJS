trifleJS
========

An IE (Trident engine) port of PhantomJS


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
