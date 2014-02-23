SpotifyDash
=============

Exposing a local Spotify client to the web using Websockets

How to setup
=============
Compile the application and run it, this starts the Websocket server up on your local machine, then setup an nginx reverse proxy using the nginx.conf file, just add that to a virtualhost on your server and setup the reverse image proxy for the Spotify images, then point the host in the startup.js file to the host name of the server and the reverseImgProxy variable to the URL of your reverse image proxy. The reason for the reverse image proxy on the spotify images are because of CORs and color-theif uses HTML5 canvas to get the average colour.

Example:
	var reverseImgProxy = "http://stark.jamiehankins.co.uk/spotifyimg/"
	AlchemyChatServer = new Alchemy({
	    Server: "stark.jamiehankins.co.uk",
	    Port: "80",
	    Action: 'spotify',
	    DebugMode: false
	});

Nginx Config:

    location /spotify/ {
      proxy_pass http://127.0.0.1:81; //change ip here
      proxy_http_version 1.1;
      proxy_set_header Upgrade $http_upgrade;
      proxy_set_header Connection "upgrade";
    }

    location /spotifyimg/ {
        expires 72h;
        error_page 404 =200 /broken.jpg;
        proxy_pass http://o.scdn.co/;
    }
![image](http://i.imgur.com/RQVAIqr.png "Demo image")
