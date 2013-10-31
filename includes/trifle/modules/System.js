/*
* System.js
*
* By: Steven de Salas
* On: Sep 2013
* 
* Defines a System class representing a
* general program helper.
* 
*/

// Initialise Namespace
this.trifle = this.trifle || {};
trifle.modules = trifle.modules || {};

// Wrap code to avoid global vars
(function(trifle) {


    // Define Constructor
    var System = trifle.modules.System = function() {
        console.xdebug("new System()");
        // Instantiate a V8 System object and stores it in internal API property
        this.API = trifle.API['System']();
        // Populate other properties
        this.args = this.API.Args;
    }


})(this.trifle);

