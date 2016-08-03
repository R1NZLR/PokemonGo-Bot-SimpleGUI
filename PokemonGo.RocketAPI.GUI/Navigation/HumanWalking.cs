using GeoCoordinatePortable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PokemonGo.RocketAPI.GUI.Helpers;
using PokemonGo.RocketAPI.GeneratedCode;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using GMap.NET;

namespace PokemonGo.RocketAPI.GUI.Navigation
{
    public class HumanWalking
    {
        private GMapControl _map;        
        private Client _client;
        private const double slowSpeed = 10 / 3.6;

        public HumanWalking(Client client)
        {
            _client = client;
        }

        public void assignMapToUpdate(GMapControl map)
        {
            _map = map;
        }

        public async Task<PlayerUpdateResponse> Walk(GeoCoordinate targetLocation, double walkingSpeedKmPerHr, Func<Task> funcionWhileWalking = null)
        {
            // Create Marker Overlay
            GMapOverlay markersOverlay = new GMapOverlay("markers");

            // Create Source Marker
            GMarkerGoogle sourceMarker = new GMarkerGoogle(new PointLatLng(_client.CurrentLat, _client.CurrentLng),
              GMarkerGoogleType.green);
            markersOverlay.Markers.Add(sourceMarker);

            // Create Target Marker
            GMarkerGoogle targetMarker = new GMarkerGoogle(new PointLatLng(targetLocation.Latitude, targetLocation.Longitude),
              GMarkerGoogleType.red);
            markersOverlay.Markers.Add(targetMarker);

            // Set the Marker on the Map
            if (_map != null)
            {                
                _map.Overlays.Add(markersOverlay);
            }

            var speedInMetersPerSecond = walkingSpeedKmPerHr / 3.6;
            var sourceLocation = new GeoCoordinate(_client.CurrentLat, _client.CurrentLng);
            var distance = LocationUtils.CalculateDistanceInMeters(sourceLocation, targetLocation);
            var nextWaypointBearing = LocationUtils.DegreeBearing(sourceLocation, targetLocation);
            var nextWaypointDistance = speedInMetersPerSecond;
            var waypoint = LocationUtils.CreateWaypoint(sourceLocation, nextWaypointDistance, nextWaypointBearing);

            // Notify Distance
            Logger.Write($"Distance to target location: {distance:0.##} meters. Will take {distance / speedInMetersPerSecond:0.##} seconds!");
            Logger.Write("Walking...");

            //Initial walking
            var requestSendDateTime = DateTime.Now;
            var result =
                await
                    _client.UpdatePlayerLocation(waypoint.Latitude, waypoint.Longitude,
                        _client.Settings.DefaultAltitude);

            // Update Map
            if(_map != null)
                _map.Position = new GMap.NET.PointLatLng(waypoint.Latitude, waypoint.Longitude);

            while (LocationUtils.CalculateDistanceInMeters(sourceLocation, targetLocation) >= 30)
            {
                var millisecondsUntilGetUpdatePlayerLocationResponse =
                        (DateTime.Now - requestSendDateTime).TotalMilliseconds;

                sourceLocation = new GeoCoordinate(_client.CurrentLat, _client.CurrentLng);
                var currentDistanceToTarget = LocationUtils.CalculateDistanceInMeters(sourceLocation, targetLocation);

                if (currentDistanceToTarget < 40)
                    if (speedInMetersPerSecond > slowSpeed)
                        speedInMetersPerSecond = slowSpeed;

                nextWaypointDistance = Math.Min(currentDistanceToTarget,
                       millisecondsUntilGetUpdatePlayerLocationResponse / 1000 * speedInMetersPerSecond);
                nextWaypointBearing = LocationUtils.DegreeBearing(sourceLocation, targetLocation);
                waypoint = LocationUtils.CreateWaypoint(sourceLocation, nextWaypointDistance, nextWaypointBearing);

                requestSendDateTime = DateTime.Now;
                result =
                    await
                        _client.UpdatePlayerLocation(waypoint.Latitude, waypoint.Longitude,
                            _client.Settings.DefaultAltitude);

                // Update Map
                if (_map != null)
                    _map.Position = new GMap.NET.PointLatLng(waypoint.Latitude, waypoint.Longitude);

                // Execute While Walking.
                if (funcionWhileWalking != null)
                {
                    DateTime startedPokemonCapture = DateTime.Now;
                    await funcionWhileWalking();
                    DateTime finishedPokemonCapture = DateTime.Now;
                    TimeSpan ts = finishedPokemonCapture - startedPokemonCapture;
                    requestSendDateTime = requestSendDateTime.Subtract(ts);
                }                   

                // Task Delay
                await Task.Delay(1000);
            }

            // Notify Arrival
            Logger.Write("Arrived.");

            // Remove the Markers on the Map
            if (_map != null)
            {
                _map.Overlays.Remove(markersOverlay);
            }

            // Return the Last Update
            return result;
        }
    }
}
