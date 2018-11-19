var BITUTILS = {
    EMPTYGUID: "00000000-0000-0000-0000-000000000000",

    functionExists: function (functionObj) {
        return (typeof functionObj == 'function');
    },

    getQueryString: function () {
        var query = window.location.search;

        query = query.replace('?', '');
        query = query.replace(')', '');

        if (query == '') return null;

        var queryString = new Array();
        var nameValues = query.split('&');
        for (i = 0; (i < nameValues.length) ; i++) {
            var nameValue = nameValues[i];
            var name = nameValue;
            var value = nameValue;
            if (nameValue.indexOf('=') > 0) {
                name = nameValue.split('=')[0];
                value = nameValue.split('=')[1];
            }
            queryString[name] = value;
        }
        return queryString;
    },

    getQueryStringAsObject: function() {
        var urlParams;
        (window.onpopstate = function () {
            var match,
                pl = /\+/g,  // Regex for replacing addition symbol with a space
                search = /([^&=]+)=?([^&]*)/g,
                decode = function (s) { return decodeURIComponent(s.replace(pl, " ")); },
                query = window.location.search.substring(1);

            urlParams = {};
            while (match = search.exec(query))
                urlParams[decode(match[1])] = decode(match[2]);
        })();
        return urlParams;
    },

    //functie om array size te bepalen
    //bij associatieve array's werkt array.length namelijk niet.
    getArraySize: function (array) {
        var size = 0;
        for (key in array) {
            size++;
        }
        return size;
    },

    getFirstFromArray: function (array) {
        var obj;
        for (key in array) {
            obj = array[key];
        }
        return obj;
    }
};