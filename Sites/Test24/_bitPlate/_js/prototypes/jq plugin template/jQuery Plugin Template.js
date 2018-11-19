// if (!window.L) { window.L = function () { console.log(arguments);} } // optional EZ quick logging for debugging

/**
 * A modified (improved?) version of the jQuery plugin design pattern
 * See http://docs.jquery.com/Plugins/Authoring (near the bottom) for details.
 *
 * ADVANTAGES OF EITHER FRAMEWORK:
 * - Encapsulates additional plugin action methods without polluting the jQuery.fn namespace
 * - Ensures ability to use '$' even in compat modes
 *
 * ADVANTAGES OF THIS VERSION
 * - Maintains object state between subsequent action calls, thus allowing arbitrary
 *   properties' values to be remembered
 * - Plugin object's method scope behaves like normal javascript objects in that
 *   'this' refers to the actual *object* rather than the jquery target for the plugin.
 *   This makes it more natural to work with object properties.  Note the jquery target
 *   is still available as this.$T within the object
 */
(function( $ ){

    /**
     * The plugin namespace, ie for $('.selector').myPluginName(options)
     *
     * Also the id for storing the object state via $('.selector').data()
     */
    var PLUGIN_NS = 'myPluginName';

    /*###################################################################################
     * PLUGIN BRAINS
     *
     * INSTRUCTIONS:
     *
     * To init, call...
     * $('selector').myPluginName(options)
     *
     * Some time later...
     * $('selector').myPluginName('myActionMethod')
     *
     * DETAILS:
     * Once inited with $('...').myPluginName(options), you can call
     * $('...').myPluginName('myAction') where myAction is a method in this
     * class.
     *
     * The scope, ie "this", **is the object itself**.  The jQuery match is stored
     * in the property this.$T.  In general this value should be returned to allow
     * for jQuery chaining by the user.
     *
     * Methods which begin with underscore are private and not
     * publically accessible.
     *
     * CHECK IT OUT...
     * var mySelecta = 'DIV';
     * jQuery(mySelecta).myPluginName();
     * jQuery(mySelecta).myPluginName('publicMethod');
     * jQuery(mySelecta).myPluginName('_privateMethod');
     *
     *###################################################################################*/

    /**
     * Quick setup to allow property and constant declarations ASAP
     * IMPORTANT!  The jquery hook (bottom) will return a ref to the target for chaining on init
     * BUT it requires a reference to this newly instantiated object too! Hence "return this" over "return this.$T"
     * The later is encouraged in all public API methods.
     */
    var Plugin = function ( target, options )
    {
        this.$T = $(target);
        this._init( target, options );
        return this; 

        /** #### OPTIONS #### */
       this.options= $.extend(
            true,               // deep extend
            {
                DEBUG: false,
                defaultOptionA: 'defaultValue'
            },
            options
        );

        /** #### PROPERTIES #### */
        // Here rather than below in Plugin.prototype.myProp and in _init() as this way is DRY-er
        this._testProp = 'testProp!';     // Private property declaration, underscore optional
    }

    /** #### CONSTANTS #### */
    Plugin.MY_CONSTANT = 'value';

    /** #### INITIALISER #### */
    Plugin.prototype._init = function ( target, options )
    { 

    };

    /** #### PUBLIC API (see notes) #### */
    Plugin.prototype.publicMethod = function ()
    {
        console.log('inside publicMethod!');
        console.log(this._testProp);
        this._privateMethod();
        console.log(this._testProp);
        return this.$T;        // support jQuery chaining
    }

    /** #### PRIVATE METHODS #### */
    Plugin.prototype._privateMethod = function ()
    {
        console.log('Scope:');
        console.log(this)
        console.log(this.$T);
        this._testProp = 'some other value!';
    }

    /**
     * EZ Logging/Warning (technically private but saving an '_' is worth it imo)
     */
    Plugin.prototype.DLOG = function ()
    {
        if (!this.DEBUG) return;
        for (var i in arguments) {
            console.log( PLUGIN_NS + ': ', arguments[i] );
        }
    }
    Plugin.prototype.DWARN = function ()
    {
        this.DEBUG &amp;&amp; console.warn( arguments );
    }

/*###################################################################################
 * JQUERY HOOK
 ###################################################################################*/

    /**
     * Generic jQuery plugin instantiation method call logic
     *
     * Method options are stored via jQuery's data() method in the relevant element(s)
     * Notice, myActionMethod mustn't start with an underscore (_) as this is used to
     * indicate private methods on the PLUGIN class.
     */
    $.fn[ PLUGIN_NS ] = function( methodOrOptions )
    {
        if (!$(this).length) {
            return $(this);
        }
        var instance = $(this).data(PLUGIN_NS);

        // CASE: action method (public method on PLUGIN class)
        if ( instance
                &amp;&amp; methodOrOptions.indexOf('_') != 0
                &amp;&amp; instance[ methodOrOptions ]
                &amp;&amp; typeof( instance[ methodOrOptions ] ) == 'function' ) {

            return instance[ methodOrOptions ]( Array.prototype.slice.call( arguments, 1 ) ); 

        // CASE: argument is options object or empty = initialise
        } else if ( typeof methodOrOptions === 'object' || ! methodOrOptions ) {

            instance = new Plugin( $(this), methodOrOptions );    // ok to overwrite if this is a re-init
            $(this).data( PLUGIN_NS, instance );
            return $(this);

        // CASE: method called before init
        } else if ( !instance ) {
            $.error( 'Plugin must be initialised before using method: ' + methodOrOptions );

        // CASE: invalid method
        } else if ( methodOrOptions.indexOf('_') == 0 ) {
            $.error( 'Method ' +  methodOrOptions + ' is private!' );
        } else {
            $.error( 'Method ' +  methodOrOptions + ' does not exist.' );
        }
    };

})(jQuery);