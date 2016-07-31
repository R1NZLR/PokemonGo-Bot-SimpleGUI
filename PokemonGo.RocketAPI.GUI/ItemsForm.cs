using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PokemonGo.RocketAPI.GeneratedCode;
using PokemonGo.RocketAPI.Logic;

namespace PokemonGo.RocketAPI.GUI
{
    public partial class ItemsForm : Form
    {
        private Client _client;
        private Inventory _inventory;

        public ItemsForm(Client client)
        {
            _client = client;
            _inventory = new Inventory(_client);
            InitializeComponent();
        }

        private async void ItemsForm_Load(object sender, EventArgs e)
        {
            var myItems = await _inventory.GetItems();

            var items = myItems as IList<Item> ?? myItems.ToList();
            luckyEggCount.Text = GetItemCount(items, ItemId.ItemLuckyEgg);
            incenseCount.Text = GetItemCount(items, ItemId.ItemIncenseOrdinary);
            potionCount.Text = GetItemCount(items, ItemId.ItemPotion);
            superPotionCount.Text = GetItemCount(items, ItemId.ItemSuperPotion);
            hyperPotionCount.Text = GetItemCount(items, ItemId.ItemHyperPotion);
            pokeBallCount.Text = GetItemCount(items, ItemId.ItemPokeBall);
            greatBallCount.Text = GetItemCount(items, ItemId.ItemGreatBall);
            ultraBallCount.Text = GetItemCount(items, ItemId.ItemUltraBall);
            reviveCount.Text = GetItemCount(items, ItemId.ItemRevive);
            //lureModuleCount.Text = GetItemCount(items, ItemId.item); Not sure what lure module is?
            razzBerryCount.Text = GetItemCount(items, ItemId.ItemRazzBerry);
        }

        private string GetItemCount(IEnumerable<Item> items, ItemId itemId)
        {
            return (items.FirstOrDefault(p => (ItemId)p.Item_ == itemId)?.Count ?? 0).ToString();
        }
    }
}
