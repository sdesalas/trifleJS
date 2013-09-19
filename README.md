trifleJS
========

An IE (Trident engine) port of PhantomJS


The following is a list of features that have been ported from PhantomJS.

|Feature                      | Status   | Notes                        |
|-----------------------------|----------|------------------------------|
|**phantom**                  |
|phantom.version              | Yep      |                              |
|phantom.exit()               | Yep      |                              |
|**webpage**                  |
|webpage.open()               | Yep      | Callback included            |
|webpage.render()             | Yep      |                              |
|webpage.renderBase64()       | Yep      |                              |
|webpage.evaluate()           | Yep      |                              |
|webpage.evaluateJavaScript() | Yep      |                              |
|webpage.injectJs()           | Yep      |                              |
|webpage.includeJs()          | Partial  | No Callback yet              |
|webpage#onConsoleMessage     | Nope     |                              |
|webpage#onError              | Nope     |                              |
|webpage#onLoadStarted        | Yep      |                              |
|webpage#onLoadFinished       | Yep      |                              |
