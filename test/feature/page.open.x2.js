
var page = new WebPage();

// Open first page
page.viewportSize = {width: 1024, height: 800};
page.open("http://en.wikipedia.org/", function(status) {
    if (status !== "success") { console.log("FAIL to load the address"); phantom.exit(); }
    page.render('open1.png');
    
    page.open("http://google.com/");

    // Open second page
    page.open("http://en.wikipedia.org/wiki/V8_(JavaScript_engine)", function(status) {
        if (status !== "success") { console.log("FAIL to load the address"); phantom.exit(); }
        page.render('open2.png');
        console.log("Success, both pages opened.");
        phantom.exit(); // last open so exit
    });

});