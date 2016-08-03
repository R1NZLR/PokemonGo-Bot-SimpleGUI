using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PokemonGo.RocketAPI.GUI.Helpers
{
    public static class APINotifications
    {
        private static string API_KEY = "70924d828df79feebc5d3c59fdda4c36fc7ccfa12dbdc9d7";
        private static string API_ENDPOINT = "https://www.notifymyandroid.com/publicapi/notify";
        private static string API_URL = "http://pogo.jorgelimas.net";
        private static string APP = "PoGo Bot";

        public static void SendNotification(string description, string title, int priority)
        {
            WebRequest request = WebRequest.Create(API_ENDPOINT);
            request.Method = "POST";

            byte[] data = Encoding.UTF8.GetBytes(
                $"apikey={API_KEY}&application={APP}&event={title}&description={description}");

            request.ContentLength = data.Length;
            request.ContentType = "application/x-www-form-urlencoded";
            Stream dataStream = request.GetRequestStream();
            dataStream.Write(data, 0, data.Length);
            dataStream.Close();

            WebResponse response = request.GetResponse();
        }

        public static void UpdateAppUsage()
        {
            try
            {
                string ipAddress = new WebClient().DownloadString("http://icanhazip.com");
                string version = typeof(MainForm).Assembly.GetName().Version.ToString();

                WebRequest request = WebRequest.Create(API_URL);
                request.Method = "POST";

                string action = "LOGIN";

                byte[] data = Encoding.UTF8.GetBytes(
                    $"action={action}&ip={ipAddress}&version={version}");

                request.ContentLength = data.Length;
                request.ContentType = "application/x-www-form-urlencoded";
                Stream dataStream = request.GetRequestStream();
                dataStream.Write(data, 0, data.Length);
                dataStream.Close();

                WebResponse response = request.GetResponse();
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    
                }
            }
            catch (Exception)
            {

            }
        }

        public static void UpdatePokemonCaptured(string pokemon, int? cp, float iv, double lat, double lng)
        {
            try
            {
                string ipAddress = new WebClient().DownloadString("http://icanhazip.com");

                WebRequest request = WebRequest.Create(API_URL);
                request.Method = "POST";

                string action = "POKEMON_CATCH";

                byte[] data = Encoding.UTF8.GetBytes(
                    $"action={action}&pokemon={pokemon}&cp={cp}&iv={iv}&lat={lat}&lng={lng}&ip={ipAddress}");

                request.ContentLength = data.Length;
                request.ContentType = "application/x-www-form-urlencoded";
                Stream dataStream = request.GetRequestStream();
                dataStream.Write(data, 0, data.Length);
                dataStream.Close();

                WebResponse response = request.GetResponse();
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    
                }
            }
            catch (Exception)
            {
                
            }
        }
    }
}
