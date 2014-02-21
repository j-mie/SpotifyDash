/*
Copyright 2011 Olivine Labs, LLC. <http://olivinelabs.com>
Licensed under the MIT license: <http://www.opensource.org/licenses/mit-license.php>
*/

(function(window, $) {

    Modernizr.load({
        test: Modernizr.websockets,
        nope: 'js/web-socket-js/web_socket.js'
    });

    // Set URL of your WebSocketMain.swf here, for web-socket-js
    WEB_SOCKET_SWF_LOCATION = 'js/web-socket-js/WebSocketMain.swf';
    WEB_SOCKET_DEBUG = true;

    var AlchemyChatServer = {};
    var me = {};

    function Connect() {
        // If we're using the Flash fallback, we need Flash.
        if (!window.WebSocket && !swfobject.hasFlashPlayerVersion('10.0.0')) {
            alert('Flash Player >= 10.0.0 is required.');
            return;
        }

        // Set up the Alchemy client object
        AlchemyChatServer = new Alchemy({
            Server: "stark.jamiehankins.co.uk", //webs.jamiehankins.co.uk
            Port: "80",
            Action: 'spotify',
            DebugMode: true
        });

        console.log('Connecting...');

        AlchemyChatServer.Connected = function() {
            console.log('Connection established!');
            AlchemyChatServer.Send("helo");

            setInterval(function() {
                AlchemyChatServer.Send("helo")
            }, 1000);
        };

        AlchemyChatServer.Disconnected = function() {
            console.log('Connection closed.');
        };

        AlchemyChatServer.MessageReceived = function(event) {


            var data = JSON.parse(event.data)
            $("#trackname").html("Track name: " + data[0]);
            $("#trackartist").html("Track artist: " + data[1]);
            $("#trackalbum").html("Track album: " + data[2]);
            $("#tracklength").html("Track length: " + data[5]);
            $("#trackart").attr("src", data[3])
            $("#trackbar").attr("style", "width: " + data[6] + "%")
            var colorThief = new ColorThief();
            myImage = $('#trackart');
            dominantColor = colorThief.getColor(myImage);

            $(".progress-bar").css("background-color", dominantColor);
        };

        AlchemyChatServer.Start();
    };


    $(document).ready(function() {
        Connect();
    });

})(window, jQuery);