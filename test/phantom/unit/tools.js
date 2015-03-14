
// Add Helper Functions
function expectHasFunction(o, name) {
    it("should have '" + name + "' function", function() {
        expect(typeof o[name]).toEqual('function');
    });
}

function expectHasProperty(o, name) {
    it("should have '" + name + "' property", function() {
        expect(o.hasOwnProperty(name)).toBeTruthy();
    });
}

function expectHasPropertyString(o, name) {
    expectHasProperty(o, name);

    it("should have '" + name + "' as a string", function() {
        expect(typeof o[name]).toEqual('string');
    });
}

