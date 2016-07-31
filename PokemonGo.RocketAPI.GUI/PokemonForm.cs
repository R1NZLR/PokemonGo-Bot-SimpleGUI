using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PokemonGo.RocketAPI.Enums;
using System.Net;
using System.IO;
using PokemonGo.RocketAPI.GeneratedCode;

namespace PokemonGo.RocketAPI.GUI
{
    public partial class PokemonForm : Form
    {
        Client client;

        public PokemonForm(Client client)
        {
            this.client = client;
            InitializeComponent();
        }

        private async void PokemonForm_Load(object sender, EventArgs e)
        {
            await Execute();
        }    
        private async Task Execute()
        {            
            pokemonListView.Clear();
            var inventory = await client.GetInventory();

            var pokemons =
                inventory.InventoryDelta.InventoryItems
                .Select(i => i.InventoryItemData?.Pokemon)
                    .Where(p => p != null && p.PokemonId > 0)
                    .OrderByDescending(key => key.Cp);

            var families = inventory.InventoryDelta.InventoryItems
                .Select(i => i.InventoryItemData?.PokemonFamily)
                .Where(p => p != null && p.FamilyId > 0)
                .OrderByDescending(p => (int)p.FamilyId);

            var imageList = new ImageList { ImageSize = new Size(50, 50) };
            pokemonListView.ShowItemToolTips = true;

            foreach (var pokemon in pokemons)
            {
                imageList.Images.Add(pokemon.PokemonId.ToString(), await GetPokemonImageAsync((int)pokemon.PokemonId));

                pokemonListView.LargeImageList = imageList;
                var listViewItem = new ListViewItem();
                listViewItem.SubItems.Add("Cp: " + pokemon.Cp);

                var currentCandy = families
                    .Where(i => (int)i.FamilyId <= (int)pokemon.PokemonId)
                    .Select(f => f.Candy)
                    .First();
                
                listViewItem.ImageKey = pokemon.PokemonId.ToString();
                var pokemonIv = Math.Floor(Logic.Logic.CalculatePokemonPerfection(pokemon));
                listViewItem.Text = $"{pokemon.PokemonId}\nCP {pokemon.Cp} IV {pokemonIv}%";
                listViewItem.Tag = pokemon.Id;
                listViewItem.ToolTipText = "Candy: " + currentCandy;

                pokemonListView.Items.Add(listViewItem);
            }
        }

        private async Task<Bitmap> GetPokemonImageAsync(int pokemonId)
        {
            WebRequest req = WebRequest.Create("http://pokeapi.co/media/sprites/pokemon/"+pokemonId+".png");
            WebResponse res = await req.GetResponseAsync();
            Stream resStream = res.GetResponseStream();
            return new Bitmap(resStream);
        }

        private void pokemonListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            MessageBox.Show(pokemonListView.SelectedItems[0].Tag.ToString());
        }

        private async void btnTransfer_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Do you really want to transfer selected pokemon(s)?\nYou will never see them again :(", "Confirm Transfer", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                foreach (ListViewItem item in pokemonListView.SelectedItems)
                {
                    var id = (ulong)item.Tag;
                    await client.TransferPokemon(id);
                }
            }

            await Execute();      
        }
    }
}
