
var page = require("webpage").create();

page.open("http://www.google.com", function() {
    var a = 1;
    console.log("loaded!");
});

