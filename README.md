SpotifySocket
=============

Exposing a local Spotify client to the web using Websockets

How to setup
=============
Compile the application and run it, this starts the Websocket server up on your local machine, then setup an nginx reverse proxy using the nginx.conf file, just add that to a virtualhost on your server, then point the host in the startup.js file to the host name of the server.

Example:

	AlchemyChatServer = new Alchemy({
	    Server: "stark.jamiehankins.co.uk",
	    Port: "80",
	    Action: 'spotify',
	    DebugMode: true
	});

Nginx Config:

    location /spotify/ {
      proxy_pass http://127.0.0.1:81; //change ip here
      proxy_http_version 1.1;
      proxy_set_header Upgrade $http_upgrade;
      proxy_set_header Connection "upgrade";
    }

![image](http://i.imgur.com/sOhdQYU.png "Demo image")
