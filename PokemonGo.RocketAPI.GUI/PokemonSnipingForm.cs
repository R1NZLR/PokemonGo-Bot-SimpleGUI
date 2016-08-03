using GeoCoordinatePortable;
using PokemonGo.RocketAPI.Enums;
using PokemonGo.RocketAPI.Extensions;
using PokemonGo.RocketAPI.GeneratedCode;
using PokemonGo.RocketAPI.GUI.Helpers;
using PokemonGo.RocketAPI.Logic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PokemonGo.RocketAPI.GUI
{
    public partial class PokemonSnipingForm : Form
    {
        private Client _client;
        private Inventory _inventory;

        public PokemonSnipingForm(Client client, Inventory inventory)
        {
            _client = client;
            _inventory = inventory;
            InitializeComponent();
        }

        private void lbSnipeUrl_Click(object sender, EventArgs e)
        {
            Process.Start("http://spawns.sebastienvercammen.be/");
        }

        double _lat;
        double _lng;
        private async void btnSnipe_Click(object sender, EventArgs e)
        {
            try
            {
                // Clear Box
                textResults.Clear();

                // Disable Stuff
                boxCoordinates.Enabled = false;
                btnSnipe.Enabled = false;

                // Verify the Coordinates are valid
                string delim = Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator;
                string[] coordinates = boxCoordinates.Text.Trim().Replace(',', '/').Replace('.', char.Parse(delim)).Split('/');
                double lat = double.Parse(coordinates[0]);
                double lng = double.Parse(coordinates[1]);
                textResults.AppendText("Coordinates are valid." + Environment.NewLine);

                // Persist Coordinates
                _lat = lat;
                _lng = lng;

                // Being by moving to the location
                await _client.UpdatePlayerLocation(lat, lng, _client.Settings.DefaultAltitude);
                textResults.AppendText("Teleported to Snipe Location." + Environment.NewLine);

                // Remove the Ban
                textResults.AppendText("Will try to remove soft ban now, this will take less than 1 minute." + Environment.NewLine);
                if (!await ForceUnban())
                {
                    textResults.AppendText("Unable to remove soft ban, most likely the Pokemon(s) will run away." + Environment.NewLine);
                } else
                {
                    textResults.AppendText("Removed soft ban with success." + Environment.NewLine);
                }

                // Maybe walk to the location instead of teleporting...
                var distanceToPokemon = Helpers.LocationUtils.CalculateDistanceInMeters(new GeoCoordinate(_client.CurrentLat, _client.CurrentLng), new GeoCoordinate(lat, lng));
                textResults.AppendText($"Walking from Pokestop to the Pokemon, will take {distanceToPokemon / (60 / 3.6):0.0} seconds..." + Environment.NewLine);
                Navigation.HumanWalking walker = new Navigation.HumanWalking(_client);
                await walker.Walk(new GeoCoordinate(lat, lng), 60);
                textResults.AppendText("Arrived to Pokemon Location, will try to catch it now." + Environment.NewLine);

                // Catch Pokemons in the Area
                await ExecuteCatchAllNearbyPokemons();

                // Finish
                textResults.AppendText("Sniping Complete." + Environment.NewLine);

                // Enable Stuff
                boxCoordinates.Enabled = true;
                btnSnipe.Enabled = true;
            }
            catch(Exception ex)
            {
                textResults.Text = ex.ToString();
                MessageBox.Show("Error during Pokemon Snipe, see details in results box.");
            }
        }

        private bool ForceUnbanning = false;
        private async Task<bool> ForceUnban()
        {
            if (!ForceUnbanning)
            {
                ForceUnbanning = true;

                var mapObjects = await _client.GetMapObjects();
                var pokeStops = mapObjects.MapCells.SelectMany(i => i.Forts).Where(i => i.Type == FortType.Checkpoint && i.CooldownCompleteTimestampMs < DateTime.UtcNow.ToUnixTime());

                await Task.Delay(1000);

                if (pokeStops.Count<FortData>() == 0)
                    textResults.AppendText("No Pokestops Found in the Area, won't be able to remove soft ban." + Environment.NewLine);

                foreach (var pokeStop in pokeStops)
                {
                    await _client.UpdatePlayerLocation(pokeStop.Latitude, pokeStop.Longitude, UserSettings.Default.DefaultAltitude);
                    var fortInfo = await _client.GetFort(pokeStop.Id, pokeStop.Latitude, pokeStop.Longitude);

                    if (fortInfo.Name != string.Empty)
                    {
                        textResults.AppendText($"Using Pokestop in Location {fortInfo.Latitude}, {fortInfo.Longitude} to remove soft ban." + Environment.NewLine);

                        // Check Distance from Original Position
                        double distance = Helpers.LocationUtils.CalculateDistanceInMeters(_lat, _lng, fortInfo.Latitude, fortInfo.Longitude);
                        textResults.AppendText($"The distance from the Pokemon and the Pokestop is {distance:0.0} meters." + Environment.NewLine);

                        for (int i = 1; i <= 50; i++)
                        {
                            var fortSearch = await _client.SearchFort(pokeStop.Id, pokeStop.Latitude, pokeStop.Longitude);
                            if (fortSearch.ExperienceAwarded == 0)
                            {

                            }
                            else
                            {
                                ForceUnbanning = false;
                                return true;
                            }
                        }
                    }

                    ForceUnbanning = false;
                    return false;
                }
                ForceUnbanning = false;
                return false;
            }
            ForceUnbanning = false;
            return false;
        }

        private async Task ExecuteCatchAllNearbyPokemons()
        {
            var mapObjects = await _client.GetMapObjects();

            var pokemons = mapObjects.MapCells.SelectMany(i => i.CatchablePokemons);

            var mapPokemons = pokemons as IList<MapPokemon> ?? pokemons.ToList();
            if (mapPokemons.Count<MapPokemon>() > 0)
                textResults.AppendText("Found " + mapPokemons.Count<MapPokemon>() + " Pokemon(s) in the area." + Environment.NewLine);
            else
            {
                textResults.AppendText("No Pokemon(s) found in the area." + Environment.NewLine);
                return;
            }                

            foreach (var pokemon in mapPokemons)
            {
                var update = await _client.UpdatePlayerLocation(pokemon.Latitude, pokemon.Longitude, _client.Settings.DefaultAltitude);
                var encounterPokemonResponse = await _client.EncounterPokemon(pokemon.EncounterId, pokemon.SpawnpointId);
                var pokemonCp = encounterPokemonResponse?.WildPokemon?.PokemonData?.Cp;
                var pokemonIv = Logic.Logic.CalculatePokemonPerfection(encounterPokemonResponse?.WildPokemon?.PokemonData).ToString("0.00") + "%";
                var pokeball = await GetBestBall(pokemonCp);

                if (encounterPokemonResponse.ToString().Contains("ENCOUNTER_NOT_FOUND"))
                {
                    textResults.AppendText("Pokemon ran away...");
                    continue;
                }
                    

                textResults.AppendText($"Fighting {pokemon.PokemonId} with Capture Probability of {(encounterPokemonResponse?.CaptureProbability.CaptureProbability_.First()) * 100:0.0}%" + Environment.NewLine);
                textResults.AppendText($"Current Location is {pokemon.Latitude}, {pokemon.Longitude}" + Environment.NewLine);

                CatchPokemonResponse caughtPokemonResponse;
                do
                {
                    if (encounterPokemonResponse?.CaptureProbability.CaptureProbability_.First() < (GUISettings.Default.minBerry / 100))
                    {
                        await UseBerry(pokemon.EncounterId, pokemon.SpawnpointId);
                    }

                    caughtPokemonResponse = await _client.CatchPokemon(pokemon.EncounterId, pokemon.SpawnpointId, pokemon.Latitude, pokemon.Longitude, pokeball);
                    await Task.Delay(1000);
                }
                while (caughtPokemonResponse.Status == CatchPokemonResponse.Types.CatchStatus.CatchMissed);

                textResults.AppendText(caughtPokemonResponse.Status == CatchPokemonResponse.Types.CatchStatus.CatchSuccess ? $"We caught a {pokemon.PokemonId} with CP {encounterPokemonResponse?.WildPokemon?.PokemonData?.Cp} using a {pokeball}" + Environment.NewLine : $"{pokemon.PokemonId} with CP {encounterPokemonResponse?.WildPokemon?.PokemonData?.Cp} got away while using a {pokeball}.." + Environment.NewLine);

                if (caughtPokemonResponse.Status == CatchPokemonResponse.Types.CatchStatus.CatchSuccess)
                {
                    // Update Pokemon Information
                    APINotifications.UpdatePokemonCaptured(pokemon.PokemonId.ToString(),
                        encounterPokemonResponse?.WildPokemon?.PokemonData?.Cp,
                        float.Parse(pokemonIv.Replace('%', ' ')),
                        pokemon.Latitude,
                        pokemon.Longitude
                        );
                }
            }
        }

        private async Task<MiscEnums.Item> GetBestBall(int? pokemonCp)
        {
            var pokeBallsCount = await _inventory.GetItemAmountByType(MiscEnums.Item.ITEM_POKE_BALL);
            var greatBallsCount = await _inventory.GetItemAmountByType(MiscEnums.Item.ITEM_GREAT_BALL);
            var ultraBallsCount = await _inventory.GetItemAmountByType(MiscEnums.Item.ITEM_ULTRA_BALL);
            var masterBallsCount = await _inventory.GetItemAmountByType(MiscEnums.Item.ITEM_MASTER_BALL);

            if (masterBallsCount > 0 && pokemonCp >= 1000)
                return MiscEnums.Item.ITEM_MASTER_BALL;
            else if (ultraBallsCount > 0 && pokemonCp >= 1000)
                return MiscEnums.Item.ITEM_ULTRA_BALL;
            else if (greatBallsCount > 0 && pokemonCp >= 1000)
                return MiscEnums.Item.ITEM_GREAT_BALL;

            if (ultraBallsCount > 0 && pokemonCp >= 600)
                return MiscEnums.Item.ITEM_ULTRA_BALL;
            else if (greatBallsCount > 0 && pokemonCp >= 600)
                return MiscEnums.Item.ITEM_GREAT_BALL;

            if (greatBallsCount > 0 && pokemonCp >= 350)
                return MiscEnums.Item.ITEM_GREAT_BALL;

            if (pokeBallsCount > 0)
                return MiscEnums.Item.ITEM_POKE_BALL;
            if (greatBallsCount > 0)
                return MiscEnums.Item.ITEM_GREAT_BALL;
            if (ultraBallsCount > 0)
                return MiscEnums.Item.ITEM_ULTRA_BALL;
            if (masterBallsCount > 0)
                return MiscEnums.Item.ITEM_MASTER_BALL;

            return MiscEnums.Item.ITEM_POKE_BALL;
        }

        public async Task UseBerry(ulong encounterId, string spawnPointId)
        {
            var inventoryItems = await _inventory.GetItems();
            var berries = inventoryItems.Where(p => (ItemId)p.Item_ == ItemId.ItemRazzBerry);
            var berry = berries.FirstOrDefault();

            if (berry == null)
                return;

            var useRaspberry = await _client.UseCaptureItem(encounterId, ItemId.ItemRazzBerry, spawnPointId);
            await Task.Delay(3000);
        }
    }
}
