#region

using System;
using System.Threading.Tasks;
using PokemonGo.RocketAPI.GeneratedCode;
using PokemonGo.RocketAPI.Logic.Utils;

#endregion

namespace PokemonGo.RocketAPI.Logic
{
    public class Navigation
    {
        private const double SpeedDownTo = 10/3.6;
        private readonly Client _client;

        public Navigation(Client client)
        {
            _client = client;
        }        

        public class Location
        {
            public Location(double latitude, double longitude)
            {
                Latitude = latitude;
                Longitude = longitude;
            }

            public double Latitude { get; set; }
            public double Longitude { get; set; }
        }
    }
}