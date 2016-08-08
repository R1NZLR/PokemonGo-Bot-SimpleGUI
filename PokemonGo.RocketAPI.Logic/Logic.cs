#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PokemonGo.RocketAPI.Enums;
using PokemonGo.RocketAPI.Exceptions;
using PokemonGo.RocketAPI.Helpers;
using POGOProtos.Data;
using POGOProtos.Enums;
using POGOProtos.Inventory.Item;
using POGOProtos.Map.Pokemon;
using POGOProtos.Networking.Requests.Messages;
using POGOProtos.Networking.Responses;

#endregion

namespace PokemonGo.RocketAPI.Logic
{
    public class Logic
    {
        private readonly Client _client;
        private readonly ISettings _clientSettings;
        private readonly Inventory _inventory;
        private readonly Navigation _navigation;
        private GetPlayerResponse _playerProfile;

        public Logic(ISettings clientSettings)
        {
            _clientSettings = clientSettings;
            _client = new Client(_clientSettings, clientSettings.PtcUsername, clientSettings.PtcPassword);
            _inventory = new Inventory(_client);
            _navigation = new Navigation(_client);
        }

        public static float CalculatePokemonPerfection(PokemonData poke)
        {
            if (poke != null)
                return (poke.IndividualAttack * 2 + poke.IndividualDefense + poke.IndividualStamina) / (4.0f * 15.0f) * 100.0f;
            else
                return 0;
        }

        private async Task DisplayPlayerLevelInTitle(bool updateOnly = false)
        {
            _playerProfile = _playerProfile.PlayerData != null ? _playerProfile : await _client.GetProfile();
            var playerName = _playerProfile.PlayerData.Username ?? "";
            var playerStats = await _inventory.GetPlayerStats();
            var playerStat = playerStats.FirstOrDefault();
            if (playerStat != null)
            {
                var xpDifference = GetXPDiff(playerStat.Level);
                var message =
                    $"{playerName} | Level {playerStat.Level}: {playerStat.Experience - playerStat.PrevLevelXp - xpDifference}/{playerStat.NextLevelXp - playerStat.PrevLevelXp - xpDifference}XP";
                Console.Title = message;
                if (updateOnly == false)
                    Logger.Write(message);
            }
            if (updateOnly == false)
                await Task.Delay(5000);
        }

        public static int GetXPDiff(int level)
        {
            switch (level)
            {
                case 1:
                    return 0;
                case 2:
                    return 1000;
                case 3:
                    return 2000;
                case 4:
                    return 3000;
                case 5:
                    return 4000;
                case 6:
                    return 5000;
                case 7:
                    return 6000;
                case 8:
                    return 7000;
                case 9:
                    return 8000;
                case 10:
                    return 9000;
                case 11:
                case 12:
                case 13:
                case 14:
                    return 10000;
                case 15:
                    return 15000;
                case 16:
                case 17:
                case 18:
                    return 20000;
                case 19:
                case 20:
                    return 25000;
                case 21:
                    return 50000;
                case 22:
                    return 75000;
                case 23:
                    return 100000;
                case 24:
                    return 125000;
                case 25:
                    return 150000;
                case 26:
                    return 190000;
                case 27:
                    return 200000;
                case 28:
                    return 250000;
                case 29:
                    return 300000;
                case 30:
                    return 350000;
                case 31:
                case 32:
                    return 500000;
                case 33:
                    return 750000;
                case 34:
                    return 1000000;
                case 35:
                    return 1250000;
                case 36:
                    return 1500000;
                case 37:
                    return 2000000;
                case 38:
                    return 2500000;
                case 39:
                    return 3000000;
                case 40:
                    return 5000000;
            }
            return 0;
        }

        private async Task EvolveAllPokemonWithEnoughCandy(IEnumerable<PokemonId> filter = null)
        {
            var pokemonToEvolve = await _inventory.GetPokemonToEvolve(filter);
            foreach (var pokemon in pokemonToEvolve)
            {
                var evolvePokemonOutProto = await _client.EvolvePokemon(pokemon.Id);

                if (evolvePokemonOutProto.Result == EvolvePokemonResponse.Types.Result.Success)
                {
                    Logger.Write($"{pokemon.PokemonId} successfully for {evolvePokemonOutProto.ExperienceAwarded}xp",
                        LogLevel.Evolve);
                    await DisplayPlayerLevelInTitle(true);
                }
                else
                    Logger.Write(
                        $"Failed {pokemon.PokemonId}. EvolvePokemonOutProto.Result was {evolvePokemonOutProto.Result}, stopping evolving {pokemon.PokemonId}",
                        LogLevel.Evolve);

                await Task.Delay(3000);
            }
        }

        public async Task Execute()
        {
            Git.CheckVersion();
            Logger.Write(
                $"Make sure Lat & Lng is right. Exit Program if not! Lat: {_client.CurrentLat} Lng: {_client.CurrentLng}",
                LogLevel.Warning);
            Thread.Sleep(3000);
            Logger.Write($"Logging in via: {_clientSettings.AuthType}");

            while (true)
            {
                try
                {
                    if (_clientSettings.AuthType == AuthType.Ptc)
                        await _client.DoPtcLogin();
                    else if (_clientSettings.AuthType == AuthType.Google)
                        _client.DoGoogleLogin();

                    await _client.SetServer();

                    await PostLoginExecute();
                }
                catch (AccessTokenExpiredException)
                {
                    Logger.Write("Access token expired");
                }
                catch (TaskCanceledException)
                {
                    Logger.Write("Task Canceled Exception - Restarting", LogLevel.Error);
                    await Execute();
                }
                catch (UriFormatException)
                {
                    Logger.Write("UriFormatException - Restarting", LogLevel.Error);
                    await Execute();
                }
                catch (ArgumentOutOfRangeException)
                {
                    Logger.Write("ArgumentOutOfRangeException - Restarting", LogLevel.Error);
                    await Execute();
                }
                catch (ArgumentNullException)
                {
                    Logger.Write("ArgumentNullException - Restarting", LogLevel.Error);
                    await Execute();
                }
                catch (NullReferenceException)
                {
                    Logger.Write("NullReferenceException - Restarting", LogLevel.Error);
                    await Execute();
                }
                catch (InvalidResponseException e)
                {
                    Logger.Write("InvalidResponseException - Restarting", LogLevel.Error);
                    Logger.Write("err: " + e);
                    await Execute();
                }
                catch (AggregateException)
                {
                    Logger.Write("AggregateException - Restarting", LogLevel.Error);
                    await Execute();
                }
                await Task.Delay(10000);
            }
        }

        public async Task PostLoginExecute()
        {
            while (true)
            {
                _playerProfile = await _client.GetProfile();
                await DisplayPlayerLevelInTitle();
                if (_clientSettings.EvolveAllPokemonWithEnoughCandy)
                    await EvolveAllPokemonWithEnoughCandy(_clientSettings.PokemonsToEvolve);
                if (_clientSettings.TransferDuplicatePokemon) await TransferDuplicatePokemon();
                await DisplayHighests();
                await RecycleItems();
                
                await Task.Delay(10000);
            }
        }

        private async Task RecycleItems()
        {
            var items = await _inventory.GetItemsToRecycle(_clientSettings);

            foreach (var item in items)
            {
                await _client.RecycleItem(item.ItemId, item.Count);
                Logger.Write($"{item.Count}x {item.ItemId}", LogLevel.Recycling);
                await Task.Delay(500);
            }
        }

        public async Task RepeatAction(int repeat, Func<Task> action)
        {
            for (var i = 0; i < repeat; i++)
                await action();
        }

        private async Task TransferDuplicatePokemon(bool keepPokemonsThatCanEvolve = false)
        {
            var duplicatePokemons =
                await
                    _inventory.GetDuplicatePokemonToTransfer(keepPokemonsThatCanEvolve,
                        _clientSettings.PokemonsNotToTransfer);

            foreach (var duplicatePokemon in duplicatePokemons)
            {
                if (CalculatePokemonPerfection(duplicatePokemon) >= _clientSettings.KeepMinIVPercentage ||
                    duplicatePokemon.Cp > _clientSettings.KeepMinCP)
                    continue;

                var transfer = await _client.TransferPokemon(duplicatePokemon.Id);
                var bestPokemonOfType = await _inventory.GetHighestPokemonOfTypeByCP(duplicatePokemon);
                Logger.Write(
                    $"{duplicatePokemon.PokemonId} with {duplicatePokemon.Cp} ({CalculatePokemonPerfection(duplicatePokemon).ToString("0.00")} % perfect) CP (Best: {bestPokemonOfType.Cp} | ({CalculatePokemonPerfection(bestPokemonOfType).ToString("0.00")} % perfect))",
                    LogLevel.Transfer);
                await Task.Delay(500);
            }
        }

        public async Task UseBerry(ulong encounterId, string spawnPointId)
        {
            var inventoryBalls = await _inventory.GetItems();
            var berries = inventoryBalls.Where(p => p.ItemId == ItemId.ItemRazzBerry);
            var berry = berries.FirstOrDefault();

            if (berry == null)
                return;

            var useRaspberry = await _client.UseCaptureItem(encounterId, ItemId.ItemRazzBerry, spawnPointId);
            Logger.Write($"Used, remaining: {berry.Count}", LogLevel.Berry);
            await Task.Delay(3000);
        }

        private async Task DisplayHighests()
        {
            Logger.Write("====== DisplayHighestsCP ======", LogLevel.Info, ConsoleColor.Yellow);
            var highestsPokemonCP = await _inventory.GetHighestsCP(20);
            foreach (var pokemon in highestsPokemonCP)
                Logger.Write(
                    $"# CP {pokemon.Cp}\t| ({CalculatePokemonPerfection(pokemon).ToString("0.00")}\t% perfect) NAME: '{pokemon.PokemonId}'",
                    LogLevel.Info, ConsoleColor.Yellow);
            Logger.Write("====== DisplayHighestsPerfect ======", LogLevel.Info, ConsoleColor.Yellow);
            var highestsPokemonPerfect = await _inventory.GetHighestsPerfect(10);
            foreach (var pokemon in highestsPokemonPerfect)
            {
                Logger.Write(
                    $"# CP {pokemon.Cp}\t| ({CalculatePokemonPerfection(pokemon).ToString("0.00")}\t% perfect) NAME: '{pokemon.PokemonId}'",
                    LogLevel.Info, ConsoleColor.Yellow);
            }
        }
    }
}
