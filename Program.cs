using System;
using System.Net;
using System.Threading;
using Alchemy;
using Alchemy.Classes;
using SpotifyAPIv1;
using SpotifyEventHandler = SpotifyAPIv1.SpotifyEventHandler;

namespace WebSocketSpotify
{
    internal class Program
    {
        public static SpotifyAPI spotifyClient = new SpotifyAPI();

        public static string Song;
        public static string Artist;
        public static string Album;
        public static string AlbumURL;
        public static string Volume;
        public static string Time;
        public static int Length;

        private static void Main(string[] args)
        {
            var aServer = new WebSocketServer(81, IPAddress.Any)
            {
                OnReceive = OnReceive,
                TimeOut = new TimeSpan(0, 5, 0)
            };

            aServer.Start();

            if (!SpotifyAPI.IsSpotifyRunning())
            {
                spotifyClient.RunSpotify();
                Thread.Sleep(5000);
            }

            if (!SpotifyAPI.IsSpotifyWebHelperRunning())
            {
                spotifyClient.RunSpotifyWebHelper();
                Thread.Sleep(4000);
            }

            if (!spotifyClient.Connect())
            {
                Console.WriteLine("SpotifyAPI couldn't load!");
            }

            var mh = spotifyClient.GetMusicHandler();
            var eh = spotifyClient.GetEventHandler();
            
            spotifyClient.Update();

            Song = mh.GetCurrentTrack().GetTrackName();
            Artist = mh.GetCurrentTrack().GetArtistName();
            Album = mh.GetCurrentTrack().GetAlbumName();
            AlbumURL = mh.GetCurrentTrack().GetAlbumArtURL(AlbumArtSize.SIZE_640);
            Volume = "100%";
            Time = formatTime(mh.GetCurrentTrack().GetLength()) + "/" + formatTime(mh.GetCurrentTrack().GetLength());

            eh.OnTrackChange += TrackChange;
            eh.OnPlayStateChange += PlayChange;
            eh.OnVolumeChange += VolumeChange;
            eh.OnTrackTimeChange += TimeChange;

            eh.ListenForEvents(true);

            while (true)
            {
                Thread.Sleep(1000);
            }

        }

        static void OnReceive(UserContext context)
        {
            Console.WriteLine("Websocket sending update");
            context.Send(Song);
            context.Send(Artist);
            context.Send(Album);
            context.Send(AlbumURL);
            context.Send(Volume);
            context.Send(Time);
        }

        private static void TrackChange(TrackChangeEventArgs e)
        {
            Song = e.new_track.GetTrackName();
            Length = e.new_track.length;
        }

        private static void VolumeChange(VolumeChangeEventArgs e)
        {
            Volume = ((int)(e.new_volume * 100)).ToString() + "%";
        }
        private static void PlayChange(PlayStateEventArgs e)
        {
            Song = e.playing.ToString();
        }

        private static void TimeChange(TrackTimeChangeEventArgs e)
        {
             Time = formatTime(e.track_time) + "/" + formatTime(Length);
        }

        private static String formatTime(double sec)
        {
            TimeSpan span = TimeSpan.FromSeconds(sec);
            String secs = span.Seconds.ToString(), mins = span.Minutes.ToString();
            if (secs.Length < 2)
                secs = "0" + secs;
            return mins + ":" + secs;
        }
    }
}