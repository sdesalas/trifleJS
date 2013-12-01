
var page = new WebPage();
page.viewportSize = {width: 800, height: 640};
page.open("http://www.triflejs.org", function() {
	page.render("triflejs.800x640.png");
});