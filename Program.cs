using System;
using System.Net;
using System.Threading;
using Alchemy;
using Alchemy.Classes;
using Newtonsoft.Json;
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
        public static double Length;
        public static double Percentage;
        public static double TimeIn;

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
            AlbumURL = mh.GetCurrentTrack().GetAlbumArtURL(AlbumArtSize.SIZE_320);
            Volume = "100%";

            Time = formatTime(0) + "/" + formatTime(mh.GetCurrentTrack().GetLength());

            Length = mh.GetCurrentTrack().GetLength();
            
            eh.OnTrackChange += TrackChange;
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
            string[] stuff = new string[] { Song, Artist, Album, AlbumURL, Volume, Time, Math.Round(Percentage).ToString()};

            

            var json = JsonConvert.SerializeObject(stuff);
            context.Send(json);
        }

        private static void TrackChange(TrackChangeEventArgs e)
        {
            Song = e.new_track.GetTrackName();
            Length = e.new_track.GetLength();
            Album = e.new_track.GetAlbumName();
            Artist = e.new_track.GetArtistName();
            AlbumURL = e.new_track.GetAlbumArtURL(AlbumArtSize.SIZE_320);
        }

        private static void VolumeChange(VolumeChangeEventArgs e)
        {
            Volume = ((int)(e.new_volume * 100)).ToString() + "%";
        }

        private static void TimeChange(TrackTimeChangeEventArgs e)
        {
            Time = formatTime(e.track_time) + "/" + formatTime(Length);
            TimeIn = Convert.ToInt16(e.track_time);
            Percentage = (TimeIn / Length) * 100;
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