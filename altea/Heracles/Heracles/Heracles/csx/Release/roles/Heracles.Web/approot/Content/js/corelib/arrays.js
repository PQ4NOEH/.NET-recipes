(function(window) {
    "use strict";
    
    var shuffle = function () {
        var arrays = [], count = 0, length = 0;

        for (var i = 0, l = arguments.length; i < l; i++) {
            if (arguments[i].constructor !== Array) {
                return false;
            }

            if (i === 0) {
                length = arguments[i].length;
            } else if (arguments[i].length !== length) {
                return false;
            }

            arrays.push(arguments[i]);
        }

        count = arrays.length;

        if (length) {
            while (--length) {
                var current = Math.floor(Math.random() * (length + 1));

                for (var i = 0; i < count; i++) {
                    var tmp = arrays[i][current];
                    arrays[i][current] = arrays[i][length];
                    arrays[i][length] = tmp;
                }
            }
        }

        return true;
    };

    window.Array.shuffle = shuffle;

    window.Array.prototype.shuffle = function() {
        return shuffle(this);
    };

    window.Array.prototype.pushRange = function(array) {
        for (var i = 0, l = array.length; i < l; i++) {
            this.push(array[i]);
        }
    };

    Array.prototype.clean = function (deleteValue) {
        for (var i = 0; i < this.length; i++) {
            if (this[i] === deleteValue) {
                this.splice(i, 1);
                i--;
            }
        }
        return this;
    };

    Array.prototype.equals = function (array) {
        // if the other array is a falsy value, return
        if (!array)
            return false;

        // compare lengths - can save a lot of time 
        if (this.length != array.length)
            return false;

        for (var i = 0, l = this.length; i < l; i++) {
            // Check if we have nested arrays
            if (this[i] instanceof Array && array[i] instanceof Array) {
                // recurse into the nested arrays
                if (!this[i].equals(array[i]))
                    return false;
            }
            else if (this[i] != array[i]) {
                // Warning - two different object instances will never be equal: {x:20} != {x:20}
                return false;
            }
        }
        return true;
    }
})(window);