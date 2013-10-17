

var path = phantom.libraryPath;

if (path) {
    console.log('Library Path: ' + path);
} else {
    console.error('Library Path not set!');
}
