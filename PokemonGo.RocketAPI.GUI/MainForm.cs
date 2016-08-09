using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using PokemonGo.RocketAPI.Enums;
using PokemonGo.RocketAPI.Logic;
using PokemonGo.RocketAPI.Extensions;
using PokemonGo.RocketAPI.Logic.Utils;
using PokemonGo.RocketAPI.Exceptions;
using GMap.NET.MapProviders;
using System.Text.RegularExpressions;
using System.Text;
using PokemonGo.RocketAPI.GUI.Helpers;
using PokemonGo.RocketAPI.GUI.Exceptions;
using PokemonGo.RocketAPI.GUI.Navigation;
using GeoCoordinatePortable;
using POGOProtos.Data;
using POGOProtos.Inventory.Item;
using POGOProtos.Map.Fort;
using POGOProtos.Map.Pokemon;
using POGOProtos.Networking.Responses;

namespace PokemonGo.RocketAPI.GUI
{  
    public partial class MainForm : Form
    {
        private System.Timers.Timer _recycleItemTimer;

        private Client _client;
        private Settings _settings;
        private Inventory _inventory;
        private GetPlayerResponse _profile;
        protected int PokemonCount;

        // Persisting Pokemon / Storage Sizes
        private int itemStorageSize;
        private int pokemonStorageSize;
        private int maxNumberOfEggs = 9;

        // Persisting Login Info
        private bool _loginSuccess = false;
        private AuthType _loginMethod;
        private string _username;
        private string _password;

        // Create Console Window
        ConsoleForm console;

        private bool _isFarmingActive;

        public MainForm()
        {
            InitializeComponent();

            // Set Version Information
            this.Text = $"PoGo Bot - SimpleGUI v{typeof(MainForm).Assembly.GetName().Version}";
        }

        private void CleanUp()
        {
            // Clear Labels
            boxStatsExpHour.Clear();
            boxStatsPkmnTotal.Clear();
            boxStatsPkmnHour.Clear();
            boxStatsTimeElapsed.Clear();

            // Clear Labels
            boxLuckyEggsCount.Clear();
            boxIncencesCount.Clear();
            boxPokemonCount.Clear();
            boxInventoryCount.Clear();

            // Clear Experience
            _totalExperience = 0;
            _pokemonCaughtCount = 0;            
        }

        #region Usage Report
        private Timer usageTimer = new Timer();
        private void StartAppUsageReporting()
        {
            usageTimer.Tick += UsageTick;
            usageTimer.Interval = 60000;
            usageTimer.Start();
        }

        private void UsageTick(object sender, EventArgs e)
        {
            APINotifications.UpdateAppUsage();
        }
        #endregion

        private void SetupLocationMap()
        {
            MainMap.DragButton = MouseButtons.Left;
            MainMap.MapProvider = GMapProviders.BingMap;
            MainMap.Position = new GMap.NET.PointLatLng(UserSettings.Default.DefaultLatitude, UserSettings.Default.DefaultLongitude);
            MainMap.MinZoom = 0;
            MainMap.MaxZoom = 24;
            MainMap.Zoom = 15;
        }

        private void UpdateMap(double lat, double lng)
        {
            MainMap.Position = new GMap.NET.PointLatLng(lat, lng);
        }

        private async void MainForm_Load(object sender, EventArgs e)
        {
            try
            {
                // Setup Console
                console = new ConsoleForm();
                console.StartPosition = FormStartPosition.Manual;                
                console.Location = new System.Drawing.Point((Screen.PrimaryScreen.Bounds.Width / 2) - 530, (Screen.PrimaryScreen.Bounds.Height / 2) + 310);

                // Start Loading
                StartLogger();
                CleanUp();

                // Begin Process
                if (!await DisplayLoginWindow())
                    throw new LoginNotSelectedException("Unable to Login");

                DisplayPositionSelector();
                await GetStorageSizes();
                await GetCurrentPlayerInformation();
                await PreflightCheck();

                // Starts the Timer for the Silent Recycle
                _recycleItemTimer = new System.Timers.Timer(5 * 60 * 1000); // 5 Minute timer
                _recycleItemTimer.Start();
                _recycleItemTimer.Elapsed += _recycleItemTimer_Elapsed;
                StartAppUsageReporting();
            }
            catch (LoginNotSelectedException ex)
            {
                MessageBox.Show(ex.Message, "PoGo Bot");
                Application.Exit();
            }
            catch (Exception ex)
            {
                ErrorReportCreator.Create("BotLoadingError", "Unable to Load the Bot", ex);
                MessageBox.Show("Unable to Start the Bot.", "PoGo Bot");
                Application.Exit();
            }            
        }

        private async void _recycleItemTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (GUISettings.Default.enableSilentRecycle)
                await SilentRecycle();
        }

        private void DisplayPositionSelector()
        {
            // Display Position Selector
            LocationSelector locationSelect = new LocationSelector();
            locationSelect.ShowDialog();

            // Check if Position was Selected
            try
            {
                if (!locationSelect.setPos)
                    throw new ArgumentException();

                // Persisting the Initial Position
                _client.SaveLatLng(locationSelect.lat, locationSelect.lng);
                _client.SetCoordinates(locationSelect.lat, locationSelect.lng, UserSettings.Default.DefaultAltitude);
            }
            catch(Exception ex)
            {
                // Write a Detailed Log Report
                ErrorReportCreator.Create("SelectLocation", "Unable To Select Location", ex);

                MessageBox.Show(@"You need to declare a valid starting location.", @"Safety Check");
                MessageBox.Show(@"To protect your account of a possible soft ban, the software will close.", @"Safety Check");
                Application.Exit();
            }

            // Display Starting Location
            Logger.Write($"Starting in Location Lat: {UserSettings.Default.DefaultLatitude} Lng: {UserSettings.Default.DefaultLongitude}");

            // Close the Location Window
            locationSelect.Close();

            // Setup MiniMap
            SetupLocationMap();
        }

        private async Task<bool> DisplayLoginWindow()
        {
            // Display Login
            Hide();
            LoginForm loginForm = new LoginForm();
            loginForm.ShowDialog();                        

            // Check if an Option was Selected
            if (!loginForm.loginSelected)
                throw new LoginNotSelectedException("Login information was not provided. Unable to start bot without this information.");

            // Display Console
            console.Show();

            // Display the Main Window
            Show();

            // Determine Login Method
            if (loginForm.auth == AuthType.Ptc)
                await LoginPtc(loginForm.boxUsername.Text, loginForm.boxPassword.Text);
            if (loginForm.auth == AuthType.Google)
                await LoginGoogle(loginForm.boxUsername.Text, loginForm.boxPassword.Text);            

            // New Login Notification
            // Notify the API (Pending)

            // Select the Location
            Logger.Write("Select Starting Location...");

            // Close the Login Form
            loginForm.Close();

            // Check if Login was Successful
            if (_loginSuccess)
                return true;
            else
                return false;
        }

        private void StartLogger()
        {
            GUILogger guiLog = new GUILogger(LogLevel.Info);
            guiLog.setLoggingBox(console.boxConsole);
            Logger.SetLogger(guiLog);
        }

        private async Task LoginGoogle(string username, string password)
        {
            try
            {
                // Login Method
                _loginMethod = AuthType.Google;
                _username = username;
                _password = password;

                // Creating the Settings
                Logger.Write("Adjusting the Settings.");
                UserSettings.Default.AuthType = AuthType.Google.ToString();
                _settings = new Settings();

                // Begin Login
                Logger.Write("Trying to Login with Google Token...");
                Client client = new Client(_settings, username, password);
                
                client.DoGoogleLogin();

                await client.SetServer();
                client.SetRequestBuilder();

                // Server Ready
                Logger.Write("Connected! Server is Ready.");
                _client = client;

                Logger.Write("Attempting to Retrieve Inventory and Player Profile...");
                _inventory = new Inventory(client);
                _profile = await client.GetProfile();
                EnableButtons();
                _loginSuccess = true;
            }
            catch (GoogleException ex)
            {
                if(ex.Message.Contains("NeedsBrowser"))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("It seems you have Google 2 Factor Authentication enabled.");
                    sb.AppendLine("In order to enable this bot to access your Google Account you need to create an App Password and use that one instead of your Google Password.");
                    sb.AppendLine();
                    sb.AppendLine("Please go to: https://security.google.com/settings/security/apppasswords");                    
                    sb.AppendLine("In 'Select App' select 'Other' and 'on my' select 'Windows Computer'.");
                    sb.AppendLine();
                    sb.AppendLine("This will generate a random password use that password login to the bot.");                    
                    MessageBox.Show(sb.ToString(), "Google 2 Factor Authentication");
                    Application.Exit();
                }

                if(ex.Message.Contains("BadAuth"))
                {
                    MessageBox.Show("Your Google Credentials are incorrect.", "Google Login");
                    Application.Exit();
                }
            }
            catch(Exception ex)
            {
                // Error Logging
                ErrorReportCreator.Create("GoogleLoginError", "Unable to Login with Google", ex);

                Logger.Write("Unable to Connect using the Google Token.");
                MessageBox.Show(@"Unable to Authenticate with Login Server.", @"Login Problem");
                Application.Exit();
            }
        }

        private async Task LoginPtc(string username, string password)
        {
            try
            {
                // Login Method
                _loginMethod = AuthType.Ptc;
                _username = username;
                _password = password;

                // Creating the Settings
                Logger.Write("Adjusting the Settings.");
                UserSettings.Default.AuthType = AuthType.Ptc.ToString();
                UserSettings.Default.PtcUsername = username;
                UserSettings.Default.PtcPassword = password;
                _settings = new Settings();

                // Begin Login
                Logger.Write("Trying to Login with PTC Credentials...");
                Client client = new Client(_settings, username, password);
                await client.DoPtcLogin();
                await client.SetServer();

                client.SetRequestBuilder();

                // Server Ready
                Logger.Write("Connected! Server is Ready.");                
                _client = client;

                Logger.Write("Attempting to Retrieve Inventory and Player Profile...");
                _inventory = new Inventory(client);
                _profile = await client.GetProfile();
                EnableButtons();
                _loginSuccess = true;
            }         
            catch(Exception ex)
            {
                // Error Logging
                ErrorReportCreator.Create("PTCLoginError", "Unable to Login with PTC", ex);

                Logger.Write("Unable to Connect using the PTC Credentials.");
                MessageBox.Show(@"Unable to Authenticate with Login Server.", @"Login Problem");
                Application.Exit();
            }
        }

        private void EnableButtons()
        {
            startToolStripMenuItem.Enabled = true;
            transferDuplicatePokemonToolStripMenuItem.Enabled = true;
            recycleItemsToolStripMenuItem.Enabled = true;
            evolveAllPokemonwCandyToolStripMenuItem.Enabled = true;            
            myPokemonToolStripMenuItem.Enabled = true;

            Logger.Write("Ready to Work.");
        }

        private async Task<bool> PreflightCheck()
        {
            // Get Pokemons and Inventory
            var myItems = await _inventory.GetItems();
            var myPokemons = await _inventory.GetPokemons();

            // Write to Console
            var items = myItems as IList<ItemData> ?? myItems.ToList();
            var pokemon = myPokemons as IList<PokemonData> ?? myPokemons.ToList();

            Logger.Write($"Items in Bag: {items.Select(i => i.Count).Sum()}/{itemStorageSize}.");
            Logger.Write($"Lucky Eggs in Bag: {items.FirstOrDefault(p => p.ItemId == ItemId.ItemLuckyEgg)?.Count ?? 0 }");
            Logger.Write($"Pokemons in Bag: {pokemon.Count()}/{pokemonStorageSize}.");

            // Checker for Inventory
            if (items.Select(i => i.Count).Sum() >= itemStorageSize)
            {
                Logger.Write("Unable to Start Farming: You need to have free space for Items.");
                return false;
            }

            // Checker for Pokemons
            if (pokemon.Count() >= pokemonStorageSize - 9) // Eggs are Included in the total count (9/9)
            {
                Logger.Write("Unable to Start Farming: You need to have free space for Pokemons.");
                return false;
            }

            // Ready to Fly
            Logger.Write("Inventory and Pokemon Space, Ready.");
            return true;
        }

        private void disableButtonsDuringFarming()
        {
            // Disable Button
            startToolStripMenuItem.Enabled = false;
            evolveAllPokemonwCandyToolStripMenuItem.Enabled = false;
            recycleItemsToolStripMenuItem.Enabled = false;
            transferDuplicatePokemonToolStripMenuItem.Enabled = false;
            viewMyPokemonsToolStripMenuItem.Enabled = false;
            stopToolStripMenuItem.Enabled = true;
        }

        ////////////////////////
        // EXP COUNTER MODULE //
        ////////////////////////

        private double _totalExperience;
        private int _pokemonCaughtCount;
        private int _pokestopsCount;
        private DateTime _sessionStartTime;
        private readonly Timer _sessionTimer = new Timer();

        private void SetUpTimer()
        {
            _sessionTimer.Tick += TimerTick;
            _sessionTimer.Enabled = true;
        }

        private void TimerTick(object sender, EventArgs e)
        {
            boxStatsTimeElapsed.Text = (DateTime.Now - _sessionStartTime).TotalSeconds.ToString("0") + " Sec.";
            boxStatsExpHour.Text = GetExpPerHour();
            boxStatsPkmnHour.Text = GetPokemonPerHour();
            boxStatsPkmnTotal.Text = _pokemonCaughtCount.ToString();
        }

        private string GetExpPerHour()
        {
            double expPerHour = (_totalExperience * 3600) / (DateTime.Now - _sessionStartTime).TotalSeconds;
            return $"{expPerHour:0.0}";
        }

        private string GetPokemonPerHour()
        {
            double pkmnPerHour = (_pokemonCaughtCount * 3600) / (DateTime.Now - _sessionStartTime).TotalSeconds;
            return $"{pkmnPerHour:0.0}";
        }

        private async void StartBottingSession()
        {
            // Setup the Timer
            _sessionTimer.Interval = 5000;
            _sessionTimer.Start();
            _sessionStartTime = DateTime.Now;

            // Loop Until we Manually Stop
            while(_isFarmingActive)
            {
                try
                {
                    // Start Farming Pokestops/Pokemons.
                    await ExecuteFarmingPokestopsAndPokemons();

                    // Only Auto-Evolve when Continuous.
                    if (_isFarmingActive && GUISettings.Default.autoEvolve)
                    {
                        // Evolve Pokemons.
                        await EvolveAllPokemonWithEnoughCandy();
                    }

                    // Only Transfer when Continuous.
                    if (_isFarmingActive && GUISettings.Default.autoTransfer)
                    {
                        // Transfer Duplicates.
                        await TransferDuplicatePokemon(true);
                    }
                }
                catch (InvalidResponseException)
                {
                    // Need to Re-Authenticate
                    await reauthenticateWithServer();

                    // Disable Buttons
                    disableButtonsDuringFarming();
                }
                catch (Exception ex)
                {
                    // Write Error to Console
                    Logger.Write($"Error: {ex.Message}");

                    // Create Log Report
                    ErrorReportCreator.Create("BotFarming", "General Exception while Farming", ex);

                    // Need to Re-Authenticate (In Testing)
                    await reauthenticateWithServer();

                    // Disable Buttons
                    disableButtonsDuringFarming();
                }
            }           
        }

        private async Task reauthenticateWithServer()
        {
            Logger.Write("------------> InvalidReponseException");
            Logger.Write("<------------ Recovering");

            // Re-Authenticate with Server
            switch (_loginMethod)
            {
                case AuthType.Ptc:
                    await LoginPtc(_username, _password);
                    break;

                case AuthType.Google:
                    await LoginGoogle(_username, _password);
                    break;
            }           
        }

        private void StopBottingSession()
        {
            _sessionTimer.Stop();

            boxPokestopName.Clear();
            boxPokestopInit.Clear();
            boxPokestopCount.Clear();

            MessageBox.Show(@"Please allow a few seconds for the pending tasks to complete.", "PoGo Bot");
        }

        ///////////////////////
        // API LOGIC MODULES //
        ///////////////////////
        
        public async Task GetStorageSizes()
        {
            // Pokemon / Storage  Upgrades
            var pokemonStorageUpgradesCount = 0;
            var itemStorageUpgradesCount = 0;

            var myInventoryUpgrades = await _inventory.GetInventoryUpgrades();

            // Determine the number of upgrades
            if (myInventoryUpgrades.Count() != 0)
            {
                var tmpInventoryUpgrades = myInventoryUpgrades.ToList()[0].ToString();
                itemStorageUpgradesCount = Regex.Matches(tmpInventoryUpgrades, "1002").Count;
                pokemonStorageUpgradesCount = Regex.Matches(tmpInventoryUpgrades, "1001").Count;
            }

            // Calculate storage sizes
            itemStorageSize = (itemStorageUpgradesCount * 50) + 350;
            pokemonStorageSize = (pokemonStorageUpgradesCount * 50) + 250;
        }

        public async Task GetCurrentPlayerInformation()
        {
            var playerName = _profile.PlayerData.Username ?? "";
            var playerStats = await _inventory.GetPlayerStats();
            var playerStat = playerStats.FirstOrDefault();
            if (playerStat != null)
            {
                var xpDifference = GetXpDiff(playerStat.Level);                
                lbName.Text = playerName;
                lbLevel.Text = $"Lv {playerStat.Level}";
                lbExperience.Text = $"{playerStat.Experience - playerStat.PrevLevelXp - xpDifference}/{playerStat.NextLevelXp - playerStat.PrevLevelXp - xpDifference} XP";

                expProgressBar.Minimum = 0;
                expProgressBar.Maximum = (int)(playerStat.NextLevelXp - playerStat.PrevLevelXp - xpDifference);
                expProgressBar.Value = (int)(playerStat.Experience - playerStat.PrevLevelXp - xpDifference);                
            }

            // Get Pokemons and Inventory
            var myItems = await _inventory.GetItems();
            var myPokemons = await _inventory.GetPokemons();
            PokemonCount = myPokemons.Count();

            // Write to Console
            var items = myItems as IList<ItemData> ?? myItems.ToList();
            boxInventoryCount.Text = $"{items.Select(i => i.Count).Sum()}/{itemStorageSize}";
            boxPokemonCount.Text = $"{myPokemons.Count()}/{pokemonStorageSize}";
            boxLuckyEggsCount.Text = (items.FirstOrDefault(p => p.ItemId == ItemId.ItemLuckyEgg)?.Count ?? 0).ToString();
            boxIncencesCount.Text = (items.FirstOrDefault(p => p.ItemId == ItemId.ItemIncenseOrdinary)?.Count ?? 0).ToString();            
        }

        public static int GetXpDiff(int level)
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
                    return 10000;
                case 12:
                    return 10000;
                case 13:
                    return 10000;
                case 14:
                    return 10000;
                case 15:
                    return 15000;
                case 16:
                    return 20000;
                case 17:
                    return 20000;
                case 18:
                    return 20000;
                case 19:
                    return 25000;
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
                    return 500000;
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
                    return 1000000;
                case 40:
                    return 1000000;
            }
            return 0;
        }

        private async Task EvolveAllPokemonWithEnoughCandy()
        {
            // Clear Grid
            dGrid.Rows.Clear();

            // Prepare Grid
            dGrid.ColumnCount = 3;
            dGrid.Columns[0].Name = "Action";
            dGrid.Columns[1].Name = "Pokemon";
            dGrid.Columns[2].Name = "Experience";

            // Logging
            Logger.Write("Selecting Pokemons available for Evolution.");

            var pokemonToEvolve = await _inventory.GetPokemonToEvolve();

            foreach (var pokemon in pokemonToEvolve)
            {
                var evolvePokemonOutProto = await _client.EvolvePokemon(pokemon.Id); 

                if (evolvePokemonOutProto.Result == EvolvePokemonResponse.Types.Result.Success)
                {
                    Logger.Write($"Evolved {pokemon.PokemonId} successfully for {evolvePokemonOutProto.ExperienceAwarded}xp");

                    // GUI Experience
                    _totalExperience += evolvePokemonOutProto.ExperienceAwarded;
                    dGrid.Rows.Insert(0, "Evolved", pokemon.PokemonId.ToString(), evolvePokemonOutProto.ExperienceAwarded);
                }                    
                else
                {
                    Logger.Write($"Failed to evolve {pokemon.PokemonId}. EvolvePokemonOutProto.Result was {evolvePokemonOutProto.Result}, stopping evolving {pokemon.PokemonId}");
                }

                await GetCurrentPlayerInformation();
            }

            // Logging
            Logger.Write("Finished Evolving Pokemons.");
        }

        private async Task TransferDuplicatePokemon(bool keepPokemonsThatCanEvolve = false)
        {
            // Clear Grid
            dGrid.Rows.Clear();

            // Prepare Grid
            dGrid.ColumnCount = 4;
            dGrid.Columns[0].Name = "Action";
            dGrid.Columns[1].Name = "Pokemon";
            dGrid.Columns[2].Name = "CP";
            dGrid.Columns[3].Name = "IV";

            // Logging
            Logger.Write("Selecting Pokemons available for Transfer.");

            var duplicatePokemons = await _inventory.GetDuplicatePokemonToTransfer(keepPokemonsThatCanEvolve);

            foreach (var duplicatePokemon in duplicatePokemons)
            {
                var iv = Logic.Logic.CalculatePokemonPerfection(duplicatePokemon);
                if (iv < GUISettings.Default.minIV && duplicatePokemon.Cp < GUISettings.Default.minCP)
                {
                    var transfer = await _client.TransferPokemon(duplicatePokemon.Id);
                    Logger.Write($"Transferred {duplicatePokemon.PokemonId} with {duplicatePokemon.Cp} CP and an IV of { iv }.");

                    // Add Row to DataGrid
                    dGrid.Rows.Insert(0, "Transferred", duplicatePokemon.PokemonId.ToString(), duplicatePokemon.Cp, $"{iv}%");

                    await GetCurrentPlayerInformation();
                }
                else
                {
                    Logger.Write($"Will not transfer {duplicatePokemon.PokemonId} with {duplicatePokemon.Cp} CP and an IV of { iv }");
                    // Add Row to DataGrid
                    dGrid.Rows.Insert(0, "Not transferred", duplicatePokemon.PokemonId.ToString(), duplicatePokemon.Cp, $"{iv}%");
                }
            }

            // Logging
            Logger.Write("Finished Transfering Pokemons.");
        }

        private async Task RecycleItems()
        {   
            try
            {
                // Clear Grid
                dGrid.Rows.Clear();

                // Prepare Grid
                dGrid.ColumnCount = 3;
                dGrid.Columns[0].Name = "Action";
                dGrid.Columns[1].Name = "Count";
                dGrid.Columns[2].Name = "Item";

                // Logging
                Logger.Write("Recycling Items to Free Space");

                var items = await _inventory.GetItemsToRecycle(_settings);

                foreach (var item in items)
                {
                    await _client.RecycleItem(item.ItemId, item.Count);
                    Logger.Write($"Recycled {item.Count}x {item.ItemId}");

                    // GUI Experience
                    dGrid.Rows.Insert(0, "Recycled", item.Count, (item.ItemId).ToString());
                }

                await GetCurrentPlayerInformation();

                // Logging
                Logger.Write("Recycling Complete.");
            }
            catch(InvalidResponseException)
            {
                await reauthenticateWithServer();
                await RecycleItems();
            }
            catch (Exception ex)
            {
                // Create Error Log
                ErrorReportCreator.Create("SilentRecycleError", "Problem during Silent Recycling", ex);

                Logger.Write($"Error Details: {ex.Message}");
                Logger.Write("Unable to Complete Items Recycling.");
            }            
        }

        private async Task SilentRecycle()
        {
            try
            {
                var items = await _inventory.GetItemsToRecycle(_settings);

                foreach (var item in items)
                {
                    await _client.RecycleItem(item.ItemId, item.Count);
                    await Task.Delay(500);
                }
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private async Task<ItemId> GetBestBall(int? pokemonCp)
        {
            var pokeBallsCount = await _inventory.GetItemAmountByType(ItemId.ItemPokeBall);
            var greatBallsCount = await _inventory.GetItemAmountByType(ItemId.ItemGreatBall);
            var ultraBallsCount = await _inventory.GetItemAmountByType(ItemId.ItemUltraBall);

            if (ultraBallsCount > 0 && pokemonCp >= 1000)
                return ItemId.ItemUltraBall;
            if (greatBallsCount > 0 && pokemonCp >= 1000)
                return ItemId.ItemGreatBall;

            if (ultraBallsCount > 0 && pokemonCp >= 600)
                return ItemId.ItemUltraBall;
            if (greatBallsCount > 0 && pokemonCp >= 600)
                return ItemId.ItemGreatBall;

            if (greatBallsCount > 0 && pokemonCp >= 350)
                return ItemId.ItemGreatBall;

            if (pokeBallsCount > 0)
                return ItemId.ItemPokeBall;
            if (greatBallsCount > 0)
                return ItemId.ItemGreatBall;

            return ultraBallsCount > 0 ? ItemId.ItemUltraBall : ItemId.ItemPokeBall;
        }

        public async Task UseBerry(ulong encounterId, string spawnPointId)
        {
            var inventoryItems = await _inventory.GetItems();
            var berries = inventoryItems.Where(p => p.ItemId == ItemId.ItemRazzBerry);
            var berry = berries.FirstOrDefault();

            if (berry == null)
                return;

            await _client.UseCaptureItem(encounterId, ItemId.ItemRazzBerry, spawnPointId);
            Logger.Write($"Used Razz Berry. Remaining: {berry.Count}");
        }

        public async Task UseLuckyEgg()
        {
            var inventoryItems = await _inventory.GetItems();
            var luckyEggs = inventoryItems.Where(p => p.ItemId == ItemId.ItemLuckyEgg);
            var luckyEgg = luckyEggs.FirstOrDefault();

            if (luckyEgg == null)
                return;
            
            await _client.UseItemExpBoost(ItemId.ItemLuckyEgg);
            Logger.Write($"Used Lucky Egg. Remaining: {luckyEgg.Count - 1}");

            await GetCurrentPlayerInformation();
        }

        public async Task UseIncense()
        {
            var inventoryItems = await _inventory.GetItems();
            var incenses = inventoryItems.Where(p => p.ItemId == ItemId.ItemIncenseOrdinary);
            var incense = incenses.FirstOrDefault();

            if (incense == null)
                return;

            await _client.UseItemIncense(ItemId.ItemIncenseOrdinary);
            Logger.Write($"Used Incense. Remaining: {incense.Count - 1}");

            await GetCurrentPlayerInformation();
        }

        private async Task ExecuteFarmingPokestopsAndPokemons()
        {
            var mapObjects = await _client.GetMapObjects();
            var pokeStops = mapObjects.Item1.MapCells.SelectMany(i => i.Forts).Where(i => i.Type == FortType.Checkpoint && i.CooldownCompleteTimestampMs < DateTime.UtcNow.ToUnixTime());

            var fortDatas = pokeStops as IList<FortData> ?? pokeStops.ToList();
            _pokestopsCount = fortDatas.Count;
            int count = 1;

            foreach (var pokeStop in fortDatas)
            {
                // Use Teleporting if No Human Walking Enabled
                if (!GUISettings.Default.humanWalkingEnabled)
                {
                    await _client.UpdatePlayerLocation(pokeStop.Latitude, pokeStop.Longitude, _settings.DefaultAltitude);
                    UpdateMap(pokeStop.Latitude, pokeStop.Longitude);
                }
                else
                {
                    var human = new HumanWalking(_client);
                    var targetLocation = new GeoCoordinate(pokeStop.Latitude, pokeStop.Longitude);
                    human.assignMapToUpdate(MainMap);
                    await human.Walk(targetLocation, GUISettings.Default.humanWalkingSpeed, ExecuteCatchAllNearbyPokemons);
                }               

                var fortInfo = await _client.GetFort(pokeStop.Id, pokeStop.Latitude, pokeStop.Longitude);

                boxPokestopName.Text = fortInfo.Name;
                boxPokestopInit.Text = count.ToString();
                boxPokestopCount.Text = _pokestopsCount.ToString();
                count++;

                var fortSearch = await _client.SearchFort(pokeStop.Id, pokeStop.Latitude, pokeStop.Longitude);
                Logger.Write($"Loot -> Gems: { fortSearch.GemsAwarded}, Eggs: {fortSearch.PokemonDataEgg} Items: {StringUtils.GetSummedFriendlyNameOfItemAwardList(fortSearch.ItemsAwarded)}");
                Logger.Write("Gained " + fortSearch.ExperienceAwarded + " XP.");

                // Experience Counter
                _totalExperience += fortSearch.ExperienceAwarded;
                
                await GetCurrentPlayerInformation();
                Logger.Write("Attempting to Capture Nearby Pokemons.");

                try
                {
                    await ExecuteCatchAllNearbyPokemons();
                }
                catch(Exception ex)
                {
                    Logger.Write("Error while trying to catch nearby Pokemon(s).");
                    ErrorReportCreator.Create("CatchingNearbyPokemonError", "Unable to Catch Nearby Pokemon(s).", ex);
                }

                if (!_isFarmingActive)
                {
                    Logger.Write("Stopping Farming Pokestops.");
                    return;
                }

                if (GUISettings.Default.autoTransfer && PokemonCount >= pokemonStorageSize - maxNumberOfEggs)
                {
                    await TransferDuplicatePokemon(true);
                }

                Logger.Write("Waiting before moving to the next Pokestop.");
                await Task.Delay(GUISettings.Default.pokestopDelay * 1000);
            }
        }

        private async Task ExecuteCatchAllNearbyPokemons()
        {
            var mapObjects = await _client.GetMapObjects();
            var pokemons = mapObjects.Item1.MapCells.SelectMany(i => i.CatchablePokemons);
            var mapPokemons = pokemons as IList<MapPokemon> ?? pokemons.ToList();

            if (mapPokemons.Any())
                Logger.Write("Found " + mapPokemons.Count + " Pokemons in the area.");

            foreach (var pokemon in mapPokemons)
            {
                await _client.UpdatePlayerLocation(pokemon.Latitude, pokemon.Longitude, _settings.DefaultAltitude);
                var encounterPokemonResponse = await _client.EncounterPokemon(pokemon.EncounterId, pokemon.SpawnPointId);
                var pokemonCp = encounterPokemonResponse?.WildPokemon?.PokemonData?.Cp;
                var pokemonIv = Logic.Logic.CalculatePokemonPerfection(encounterPokemonResponse?.WildPokemon?.PokemonData).ToString("0.00") + "%";
                var pokeball = await GetBestBall(pokemonCp);

                if (encounterPokemonResponse.ToString().Contains("ENCOUNTER_NOT_FOUND"))
                {
                    Logger.Write("Pokemon ran away...");
                    continue;
                }

                Logger.Write($"Fighting {pokemon.PokemonId} with Capture Probability of {(encounterPokemonResponse?.CaptureProbability.CaptureProbability_.First())*100:0.0}%");

                boxPokemonName.Text = pokemon.PokemonId.ToString();
                boxPokemonCaughtProb.Text = (encounterPokemonResponse?.CaptureProbability.CaptureProbability_.First() * 100) + @"%";                

                CatchPokemonResponse caughtPokemonResponse;
                do
                {
                    if (encounterPokemonResponse?.CaptureProbability.CaptureProbability_.First() < (GUISettings.Default.minBerry / 100))
                    {
                        await UseBerry(pokemon.EncounterId, pokemon.SpawnPointId);
                    }
                    
                    caughtPokemonResponse = await _client.CatchPokemon(pokemon.EncounterId, pokemon.SpawnPointId, pokemon.Latitude, pokemon.Longitude, pokeball);
                }
                while (caughtPokemonResponse.Status == CatchPokemonResponse.Types.CatchStatus.CatchMissed);

                Logger.Write(caughtPokemonResponse.Status == CatchPokemonResponse.Types.CatchStatus.CatchSuccess ? $"We caught a {pokemon.PokemonId} with CP {encounterPokemonResponse?.WildPokemon?.PokemonData?.Cp} using a {pokeball}" : $"{pokemon.PokemonId} with CP {encounterPokemonResponse?.WildPokemon?.PokemonData?.Cp} got away while using a {pokeball}..");

                if (caughtPokemonResponse.Status == CatchPokemonResponse.Types.CatchStatus.CatchSuccess)
                {
                    // Calculate Experience
                    int fightExperience = 0;
                    foreach (int exp in caughtPokemonResponse.CaptureAward.Xp)
                        fightExperience += exp;
                    _totalExperience += fightExperience;
                    Logger.Write("Gained " + fightExperience + " XP.");
                    _pokemonCaughtCount++;

                    // Update Pokemon Information
                    APINotifications.UpdatePokemonCaptured(pokemon.PokemonId.ToString(), 
                        encounterPokemonResponse?.WildPokemon?.PokemonData?.Cp, 
                        float.Parse(pokemonIv.Replace('%',' ')),
                        pokemon.Latitude,
                        pokemon.Longitude
                        );

                    // Add Row to the DataGrid
                    dGrid.Rows.Insert(0, "Captured", pokemon.PokemonId.ToString(), encounterPokemonResponse?.WildPokemon?.PokemonData?.Cp, pokemonIv);
                }
                else
                {
                    // Add Row to the DataGrid
                    dGrid.Rows.Insert(0, "Ran Away", pokemon.PokemonId.ToString(), encounterPokemonResponse?.WildPokemon?.PokemonData?.Cp, pokemonIv);
                }

                boxPokemonName.Clear();
                boxPokemonCaughtProb.Clear();

                await GetCurrentPlayerInformation();

                if (!_isFarmingActive)
                {
                    Logger.Write("Stopping Farming Pokemons.");
                    return;
                }

                Logger.Write("Waiting before moving to the next Pokemon.");
                await Task.Delay(GUISettings.Default.pokemonDelay * 1000);
            }
        }

        private bool ForceUnbanning = false;

        private async Task ForceUnban()
        {
            if (!ForceUnbanning)
            {
                ForceUnbanning = true;

                while (_isFarmingActive)
                {
                    stopToolStripMenuItem_Click(null, null);
                    await Task.Delay(10000);
                }

                Logger.Write("Starting force unban...");

                var mapObjects = await _client.GetMapObjects();
                var pokeStops = mapObjects.Item1.MapCells.SelectMany(i => i.Forts).Where(i => i.Type == FortType.Checkpoint && i.CooldownCompleteTimestampMs < DateTime.UtcNow.ToUnixTime());

                await Task.Delay(1000);
                bool done = false;

                foreach (var pokeStop in pokeStops)
                {
                    await _client.UpdatePlayerLocation(pokeStop.Latitude, pokeStop.Longitude, UserSettings.Default.DefaultAltitude);
                    var fortInfo = await _client.GetFort(pokeStop.Id, pokeStop.Latitude, pokeStop.Longitude);

                    if (fortInfo.Name != string.Empty)
                    {
                        Logger.Write("Chosen PokeStop " + fortInfo.Name + ", Starting the Process (Should take less than 1 min)");
                        for (int i = 1; i <= 50; i++)
                        {
                            var fortSearch = await _client.SearchFort(pokeStop.Id, pokeStop.Latitude, pokeStop.Longitude);
                            if (fortSearch.ExperienceAwarded == 0)
                            {

                            }
                            else
                            {
                                Logger.Write("Soft ban has been removed successfully.");
                                done = true;
                                break;
                            }
                        }
                }

                if (!done)
                        Logger.Write("Force unban failed, please try again.");

                    ForceUnbanning = false;
                    break;
                }
            }
            else
            {
                Logger.Write("A action is in play... Please wait.");
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var confirm = MessageBox.Show("Do you want to close the bot?", "PoGo Bot", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm == DialogResult.Yes)
                Application.Exit();
        }

        private async void showStatisticsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var stuff = await _inventory.GetPlayerStats();
            var stats = stuff.FirstOrDefault();
            if (stats != null)
                MessageBox.Show($"Battle Attack Won: {stats.BattleAttackTotal}\n" +
                                $"Battle Attack Total: {stats.BattleAttackTotal}\n" +
                                $"Battle Defended Won: {stats.BattleDefendedWon}\n" +
                                $"Battle Training Won: {stats.BattleTrainingWon}\n" +
                                $"Battle Training Total: {stats.BattleTrainingTotal}\n" +
                                $"Big Magikarp Caught: {stats.BigMagikarpCaught}\n" +
                                $"Eggs Hatched: {stats.EggsHatched}\n" +
                                $"Evolutions: {stats.Evolutions}\n" +
                                $"Km Walked: {stats.KmWalked}\n" +
                                $"Pokestops Visited: {stats.PokeStopVisits}\n" +
                                $"Pokeballs Thrown: {stats.PokeballsThrown}\n" +
                                $"Pokemon Deployed: {stats.PokemonDeployed}\n" +
                                $"Pokemon Captured: {stats.PokemonsCaptured}\n" +
                                $"Pokemon Encountered: {stats.PokemonsEncountered}\n" +
                                $"Prestige Dropped Total: {stats.PrestigeDroppedTotal}\n" +
                                $"Prestige Raised Total: {stats.PrestigeRaisedTotal}\n" +
                                $"Small Rattata Caught: {stats.SmallRattataCaught}\n" +
                                $"Unique Pokedex Entries: {stats.UniquePokedexEntries}", "PoGo Bot");
        }

        private async void startToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!await PreflightCheck())
                return;

            // Disable Buttons
            disableButtonsDuringFarming();

            // Setup the Timer
            _isFarmingActive = true;
            SetUpTimer();
            StartBottingSession();

            // Clear Grid
            dGrid.Rows.Clear();

            // Prepare Grid
            dGrid.ColumnCount = 4;
            dGrid.Columns[0].Name = "Action";
            dGrid.Columns[1].Name = "Pokemon";
            dGrid.Columns[2].Name = "CP";
            dGrid.Columns[3].Name = "IV";
        }

        private void stopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Disable Button
            startToolStripMenuItem.Enabled = true;
            transferDuplicatePokemonToolStripMenuItem.Enabled = true;
            recycleItemsToolStripMenuItem.Enabled = true;
            evolveAllPokemonwCandyToolStripMenuItem.Enabled = true;
            viewMyPokemonsToolStripMenuItem.Enabled = true;

            stopToolStripMenuItem.Enabled = false;

            // Close the Timer
            _isFarmingActive = false;
            StopBottingSession();
        }

        private void displayConsoleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            console.Show();
        }

        private async void recycleItemsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            await RecycleItems();
        }

        private async void luckyEgg0ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            await UseLuckyEgg();
        }

        private async void incence0ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            await UseIncense();
        }

        private void viewMyPokemonsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var myPokemonsListForm = new PokemonForm(_client);
            myPokemonsListForm.ShowDialog();
        }

        private async void evolveAllPokemonwCandyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            await EvolveAllPokemonWithEnoughCandy();
        }

        private async void transferDuplicatePokemonToolStripMenuItem_Click(object sender, EventArgs e)
        {
            await TransferDuplicatePokemon(true);
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GUISettingsForm settingsGUI = new GUISettingsForm();
            settingsGUI.ShowDialog();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("PoGo Bot SimpleGUI is an Open Source Project Created by Jorge Limas." + Environment.NewLine + Environment.NewLine +
                "You can get the latest version for FREE at:" + Environment.NewLine + 
                "https://github.com/Novalys/PokemonGo-Bot-SimpleGUI", "PoGo Bot");
        }

        private async void forceRemoveBanToolStripMenuItem_Click(object sender, EventArgs e)
        {
            await ForceUnban();
        }

        private void snipePokemonsBetaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!_isFarmingActive)
            {
                PokemonSnipingForm snipingForm = new PokemonSnipingForm(_client, _inventory);
                snipingForm.ShowDialog();
            }
            else
            {
                MessageBox.Show("Farming must be stopped before using this feature.", "PoGo Bot");
            }
        }

        private void itemsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var items = new ItemsForm(_client);
            items.ShowDialog();
        }
    }
}
