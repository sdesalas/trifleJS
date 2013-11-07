console.log("STARTING");
var page = new WebPage();
page.onCallback = function(data, data2) {
    console.log(data, data2);
}

page.evaluateJavaScript("callPhantom({a: 1}, 'blah');");
console.log("DONE!");
phantom.exit();