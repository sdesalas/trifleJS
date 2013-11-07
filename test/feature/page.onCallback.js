console.log("STARTING");
var page = new WebPage();
page.onCallback = function(data, data2) {
    console.log(data, data2);
}

page.evaluateJavaScript("callPhantom({a: 1}, 'blah');");
console.log("DONE!");

console.log("STARTING ROUND 2");
var page = new WebPage();
var msgA = "a",
            msgB = "b",
            result,
            expected = msgA + msgB;

page.onCallback = function(a, b) {
    console.log("page.onCallback()", a, b);
    return a + b;
};

result = page.evaluate(function(a, b) {
    var x = callPhantom(a, b);
    alert("callPhantom() complete: " + x);
    return x;
}, msgA, msgB);

console.log("RESULT:" + result);
console.log("DONE!");
phantom.exit();