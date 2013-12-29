var page = new WebPage();
page.onAlert = function(data, data2) {
    console.log(data, data2);
}

page.evaluateJavaScript("alert('onAlert Test');");

