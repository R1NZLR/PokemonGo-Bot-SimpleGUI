#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using POGOProtos.Data;
using POGOProtos.Data.Player;
using POGOProtos.Enums;
using POGOProtos.Inventory;
using POGOProtos.Inventory.Item;
using POGOProtos.Settings.Master;

#endregion

namespace PokemonGo.RocketAPI.Logic
{
    public class Inventory
    {
        private readonly Client _client;

        public Inventory(Client client)
        {
            _client = client;
        }

        public async Task<IEnumerable<PokemonData>> GetDuplicatePokemonToTransfer(
            bool keepPokemonsThatCanEvolve = false, IEnumerable<PokemonId> filter = null)
        {
            var myPokemon = await GetPokemons();
            var pokemonList = myPokemon.Where(p => p.DeployedFortId  != null).ToList(); //Don't evolve pokemon in gyms

            if (filter != null)
                pokemonList = pokemonList.Where(p => !filter.Contains(p.PokemonId)).ToList();
            
            if (keepPokemonsThatCanEvolve)
            {
                var results = new List<PokemonData>();
                var pokemonsThatCanBeTransfered = pokemonList.GroupBy(p => p.PokemonId)
                    .Where(x => x.Count() > 2).ToList();

                var myPokemonSettings = await GetPokemonSettings();
                var pokemonSettings = myPokemonSettings.ToList();

                var myPokemonFamilies = await GetPokemonFamilies();
                var pokemonFamilies = myPokemonFamilies.ToArray();

                foreach (var pokemon in pokemonsThatCanBeTransfered)
                {
                    var settings = pokemonSettings.Single(x => x.PokemonId == pokemon.Key);
                    var familyCandy = pokemonFamilies.Single(x => x != null && settings.FamilyId == x.FamilyId);
                    if (settings.CandyToEvolve == 0)
                        continue;

                    var amountToSkip = familyCandy.Candy_ / settings.CandyToEvolve;
                    amountToSkip = amountToSkip == 0 ? 1 : amountToSkip;

                    results.AddRange(pokemonList.Where(x => x.PokemonId == pokemon.Key && x.Favorite == 0)
                        .OrderByDescending(x => x.Cp)
                        .ThenBy(n => n.StaminaMax)
                        .Skip(amountToSkip)
                        .ToList());
                }

                return results;
            }

            return pokemonList
                .GroupBy(p => p.PokemonId)
                .Where(x => x.Count() > 1)
                .SelectMany(
                    p =>
                        p.Where(x => x.Favorite == 0)
                            .OrderByDescending(x => x.Cp)
                            .ThenBy(n => n.StaminaMax)
                            .Skip(1)
                            .ToList());
        }

        public async Task<IEnumerable<PokemonData>> GetHighestsCP(int limit)
        {
            var myPokemon = await GetPokemons();
            var pokemons = myPokemon.ToList();

            return pokemons.OrderByDescending(x => x.Cp).ThenBy(n => n.StaminaMax).Take(limit);
        }

        public async Task<IEnumerable<PokemonData>> GetHighestsPerfect(int limit)
        {
            var myPokemon = await GetPokemons();
            var pokemons = myPokemon.ToList();

            return pokemons.OrderByDescending(Logic.CalculatePokemonPerfection).Take(limit);
        }

        public async Task<PokemonData> GetHighestPokemonOfTypeByCP(PokemonData pokemon)
        {
            var myPokemon = await GetPokemons();
            var pokemons = myPokemon.ToList();

            return pokemons.Where(x => x.PokemonId == pokemon.PokemonId)
                .OrderByDescending(x => x.Cp)
                .First();
        }

        public async Task<int> GetItemAmountByType(ItemId type)
        {
            var pokeballs = await GetItems();

            return pokeballs.FirstOrDefault(i => i.ItemId == type)?.Count ?? 0;
        }

        public async Task<IEnumerable<ItemData>> GetItems()
        {
            var inventory = await _client.GetInventory();

            return inventory.InventoryDelta.InventoryItems
                .Select(i => i.InventoryItemData?.Item)
                .Where(p => p != null);
        }

        public async Task<IEnumerable<PokedexEntry>> GetPokedexEntries()
        {
            var inventory = await _client.GetInventory();

            return inventory.InventoryDelta.InventoryItems.Select(i => i.InventoryItemData.PokedexEntry);
        }

        public async Task<IEnumerable<ItemData>> GetItemsToRecycle(ISettings settings)
        {
            var myItems = await GetItems();

            return myItems
                .Where(x => settings.ItemRecycleFilter.Any(f => f.Key == x.ItemId && x.Count > f.Value))
                .Select(
                    x =>
                        new ItemData
                        {
                            ItemId = x.ItemId,
                            Count = x.Count - settings.ItemRecycleFilter.Single(f => f.Key == (ItemId)x.ItemId).Value,
                            Unseen = x.Unseen
                        });
        }

        public async Task<IEnumerable<PlayerStats>> GetPlayerStats()
        {
            var inventory = await _client.GetInventory();

            return inventory.InventoryDelta.InventoryItems
                .Select(i => i.InventoryItemData?.PlayerStats)
                .Where(p => p != null);
        }

        public async Task<IEnumerable<Candy>> GetPokemonFamilies()
        {
            var inventory = await _client.GetInventory();

            return
                inventory.InventoryDelta.InventoryItems.Select(i => i.InventoryItemData?.Candy)
                    .Where(p => p != null && p.FamilyId != PokemonFamilyId.FamilyUnset);
        }

        public async Task<IEnumerable<Candy>> GetPokeListPokemonFamilies()
        {
            var inventory = await _client.GetInventory();
            return
            inventory.InventoryDelta.InventoryItems
                .Select(i => i.InventoryItemData?.Candy)
                .Where(p => p != null && p.FamilyId > 0)
                .OrderByDescending(p => (int)p.FamilyId);
        }

        public async Task<IEnumerable<PokemonData>> GetPokemons()
        {
            var inventory = await _client.GetInventory();

            return
                inventory.InventoryDelta.InventoryItems.Select(i => i.InventoryItemData?.PokemonData)
                    .Where(p => p != null && p.PokemonId > 0);
        }

        public async Task<IEnumerable<PokemonData>> GetPokeListPokemon()
        {
            var inventory = await _client.GetInventory();
            return
                inventory.InventoryDelta.InventoryItems
                    .Select(i => i.InventoryItemData?.PokemonData)
                    .Where(p => p != null && p?.PokemonId > 0)
                    .OrderByDescending(key => key.Cp);
        }

        public async Task<IEnumerable<PokemonSettings>> GetPokemonSettings()
        {
            var templates = await _client.GetItemTemplates();

            return
                templates.ItemTemplates.Select(i => i.PokemonSettings)
                    .Where(p => p != null && p.FamilyId != PokemonFamilyId.FamilyUnset);
        }

        public async Task<IEnumerable<InventoryUpgrades>> GetInventoryUpgrades()
        {
            var inventory = await _client.GetInventory();
            return inventory.InventoryDelta.InventoryItems
                .Select(i => i.InventoryItemData?.InventoryUpgrades)
                .Where(p => p != null);
        }

        public async Task<IEnumerable<PokemonData>> GetPokemonToEvolve(IEnumerable<PokemonId> filter = null)
        {
            var myPokemons = await GetPokemons();
            myPokemons = myPokemons.Where(p => string.IsNullOrEmpty(p.DeployedFortId)).OrderBy(p => p.Cp); //Don't evolve pokemon in gyms

            if (filter != null)
                myPokemons = myPokemons.Where(p => filter.Contains(p.PokemonId));
            
            var pokemons = myPokemons.ToList().OrderByDescending(pokemon => pokemon.PokemonId).ThenByDescending(pokemon => pokemon.Cp);
            
            var myPokemonSettings = await GetPokemonSettings();
            var pokemonSettings = myPokemonSettings.ToList();
            var myPokemonFamilies = await GetPokemonFamilies();
            var pokemonFamilies = myPokemonFamilies.ToArray();
            var myPokedexEntries = await GetPokedexEntries();
            var pokedexEntries = myPokedexEntries.ToList();

            var pokemonToEvolve = new List<PokemonData>();
            var pokemonToExclude = new List<PokemonId>();

            foreach (var pokemon in pokemons)
            {
                var settings = pokemonSettings.Single(x => x.PokemonId == pokemon.PokemonId);
                var familyCandy = pokemonFamilies.Single(x => settings.FamilyId == x.FamilyId);
                var nextEvolution = settings.EvolutionIds.FirstOrDefault();
                var totalEvolutions = pokemonSettings.Count(ps => ps.FamilyId == settings.FamilyId && ps.PokemonId != settings.PokemonId);
                var evolutionCapturedCount =
                    pokedexEntries.FirstOrDefault(x => x?.PokemonId == nextEvolution)?.TimesCaptured;

                //Don't evolve if we can't evolve it
                if (settings.EvolutionIds.Count == 0 || (evolutionCapturedCount > 0 && settings.ParentPokemonId != PokemonId.Missingno) || pokemonToExclude.Contains(settings.PokemonId))
                {
                    pokemonToExclude.Add(settings.PokemonId);
                    continue;
                }

                if (settings.ParentPokemonId != PokemonId.Missingno && evolutionCapturedCount > 0)
                    pokemonToExclude.Add(settings.PokemonId);
                
                var pokemonCandyNeededAlready =
                    pokemonToEvolve.Count(
                        p => pokemonSettings.Single(x => x.PokemonId == p.PokemonId).FamilyId == settings.FamilyId) *
                    settings.CandyToEvolve;

                if (familyCandy.Candy_ - pokemonCandyNeededAlready > settings.CandyToEvolve)
                {
                    if (evolutionCapturedCount == 0 || evolutionCapturedCount == null && totalEvolutions > 1)
                        pokemonToExclude.Add(settings.PokemonId);
                    
                    pokemonToEvolve.Add(pokemon);
                }
                else
                    pokemonToExclude.Remove(settings.PokemonId);
            }

            return pokemonToEvolve;
        }
    }
}