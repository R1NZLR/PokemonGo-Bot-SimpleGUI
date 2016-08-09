#region

using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Google.Protobuf;
using PokemonGo.RocketAPI.Enums;
using PokemonGo.RocketAPI.Exceptions;
using PokemonGo.RocketAPI.Extensions;
using PokemonGo.RocketAPI.Helpers;
using PokemonGo.RocketAPI.Login;
using PokemonGo.RocketAPI.Common;
using POGOProtos.Inventory.Item;
using POGOProtos.Networking.Envelopes;
using POGOProtos.Networking.Requests;
using POGOProtos.Networking.Requests.Messages;
using POGOProtos.Networking.Responses;

#endregion

namespace PokemonGo.RocketAPI
{
    public class Client
    {
        private readonly HttpClient _httpClient;
        private string _apiUrl;
        public AuthType AuthType = AuthType.Google;
        private const string VersionHash = "b1f2bf509a025b7cd76e1c484e2a24411c50f061"; // 

        private string _username;
        private string _password;
        public IApiFailureStrategy ApiFailureStrategy { get; set; }
        private RequestBuilder _requestBuilder;


        public string AuthToken { get; set; }
        internal AuthTicket AuthTicket { get; set; }

        public Client(ISettings settings, string username, string password)
        {
            Settings = settings;
            _username = username;
            _password = password;
            ApiFailureStrategy = new ApiFailureStrategy(this);

            if (File.Exists(Directory.GetCurrentDirectory() + "\\Coords.txt") &&
                File.ReadAllText(Directory.GetCurrentDirectory() + "\\Coords.txt").Contains(":"))
            {
                var latlngFromFile = File.ReadAllText(Directory.GetCurrentDirectory() + "\\Coords.txt");
                var latlng = latlngFromFile.Split(':');
                if (latlng[0].Length != 0 && latlng[1].Length != 0)
                {
                    try
                    {
                        SetCoordinates(Convert.ToDouble(latlng[0]), Convert.ToDouble(latlng[1]),
                            Settings.DefaultAltitude);
                    }
                    catch (FormatException)
                    {
                        Logger.Write("Coordinates in \"Coords.txt\" file is invalid, using the default coordinates ",
                            LogLevel.Warning);
                        SetCoordinates(Settings.DefaultLatitude, Settings.DefaultLongitude, Settings.DefaultAltitude);
                    }
                }
                else
                {
                    SetCoordinates(Settings.DefaultLatitude, Settings.DefaultLongitude, Settings.DefaultAltitude);
                }
            }
            else
            {
                SetCoordinates(Settings.DefaultLatitude, Settings.DefaultLongitude, Settings.DefaultAltitude);
            }

            //Setup HttpClient and create default headers
            var handler = new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
                AllowAutoRedirect = false
            };
            _httpClient = new HttpClient(new RetryHandler(handler));
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Niantic App");
            //"Dalvik/2.1.0 (Linux; U; Android 5.1.1; SM-G900F Build/LMY48G)");
            _httpClient.DefaultRequestHeaders.ExpectContinue = false;
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Connection", "keep-alive");
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "*/*");
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type",
                "application/x-www-form-urlencoded");
        }

        public ISettings Settings { get; }
        public string AccessToken { get; set; }

        public double CurrentLat { get; private set; }
        public double CurrentLng { get; private set; }
        public double CurrentAltitude { get; private set; }

        public void SetRequestBuilder()
        {
            _requestBuilder = new RequestBuilder(AccessToken, AuthType, CurrentLat, CurrentLng,
                CurrentAltitude, AuthTicket);
        }
        public async Task<CatchPokemonResponse> CatchPokemon(ulong encounterId, string spawnPointGuid, double pokemonLat,
            double pokemonLng, ItemId pokeball)
        {
            var customRequest = new CatchPokemonMessage
            {
                EncounterId = encounterId,
                Pokeball = pokeball,
                SpawnPointId= spawnPointGuid,
                HitPokemon = true,
                NormalizedReticleSize = 1.950,
                SpinModifier = 1,
                NormalizedHitPosition = 1
            };

            SetRequestBuilder();
            var catchPokemonRequest = _requestBuilder.GetRequestEnvelope(RequestType.CatchPokemon, customRequest);

            return
                await
                    _httpClient.PostProtoPayload<Request, CatchPokemonResponse>($"https://{_apiUrl}/rpc",
                        catchPokemonRequest, ApiFailureStrategy);
        }
        
        public void DoGoogleLogin()
        {
            AuthType = AuthType.Google;
            AccessToken = GoogleLoginGPSOAuth.DoLogin(_username, _password);
        }

        public async Task DoPtcLogin()
        {
            AuthType = AuthType.Ptc;
            var ptcLogin = new PtcLogin(_username, _password);
            AccessToken = await ptcLogin.GetAccessToken();         
        }

        public async Task<EncounterResponse> EncounterPokemon(ulong encounterId, string spawnPointGuid)
        {
            var customRequest = new EncounterMessage
            {
                EncounterId = encounterId,
                SpawnPointId = spawnPointGuid,
                PlayerLatitude = CurrentLat,
                PlayerLongitude = CurrentLng
            };

            SetRequestBuilder();
            var requestEnvelope = _requestBuilder.GetRequestEnvelope(RequestType.Encounter, customRequest);
            
            return
                await
                    _httpClient.PostProtoPayload<Request, EncounterResponse>($"https://{_apiUrl}/rpc", requestEnvelope, ApiFailureStrategy);
        }

        public async Task<EvolvePokemonResponse> EvolvePokemon(ulong pokemonId)
        {
            var customRequest = new EvolvePokemonMessage
            {
                PokemonId = pokemonId
            };

            SetRequestBuilder();
            var releasePokemonRequest = _requestBuilder.GetRequestEnvelope(RequestType.EvolvePokemon, customRequest);

            return
                await
                    _httpClient.PostProtoPayload<Request, EvolvePokemonResponse>($"https://{_apiUrl}/rpc",
                        releasePokemonRequest, ApiFailureStrategy);
        }

        public async Task<EvolvePokemonResponse> PowerUpPokemon(ulong pokemonId)
        {
            var customRequest = new EvolvePokemonMessage
            {
                PokemonId = pokemonId
            };

            SetRequestBuilder();
            var releasePokemonRequest = _requestBuilder.GetRequestEnvelope(RequestType.UpgradePokemon, customRequest);
            return
                await
                    _httpClient.PostProtoPayload<Request, EvolvePokemonResponse>($"https://{_apiUrl}/rpc",
                        releasePokemonRequest, ApiFailureStrategy);
        }

        public async Task<FortDetailsResponse> GetFort(string fortId, double fortLat, double fortLng)
        {
            var customRequest = new FortDetailsMessage
            {
                FortId = fortId,
                Latitude = fortLat,
                Longitude = fortLng
            };

            SetRequestBuilder();
            var fortDetailRequest = _requestBuilder.GetRequestEnvelope(RequestType.FortDetails, customRequest);
            return
                await
                    _httpClient.PostProtoPayload<Request, FortDetailsResponse>($"https://{_apiUrl}/rpc",
                        fortDetailRequest, ApiFailureStrategy);
        }

        public async Task<GetInventoryResponse> GetInventory()
        {
            SetRequestBuilder();
            var inventoryRequest = _requestBuilder.GetRequestEnvelope(RequestType.GetInventory, new GetInventoryMessage());
            return
                await
                    _httpClient.PostProtoPayload<Request, GetInventoryResponse>($"https://{_apiUrl}/rpc",
                        inventoryRequest, ApiFailureStrategy);
        }

        public async Task<DownloadItemTemplatesResponse> GetItemTemplates()
        {
            SetRequestBuilder();
            var settingsRequest = _requestBuilder.GetRequestEnvelope(RequestType.DownloadItemTemplates, new DownloadItemTemplatesMessage());

            return
                await
                    _httpClient.PostProtoPayload<Request, DownloadItemTemplatesResponse>($"https://{_apiUrl}/rpc",
                        settingsRequest, ApiFailureStrategy);
        }


        public async Task<Tuple<GetMapObjectsResponse, GetHatchedEggsResponse, GetInventoryResponse, CheckAwardedBadgesResponse, DownloadSettingsResponse>> GetMapObjects()
        {
            var customRequest = new GetMapObjectsMessage
            {
                CellId = { S2Helper.GetNearbyCellIds(CurrentLng, CurrentLat) },
                SinceTimestampMs = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                Latitude = CurrentLat,
                Longitude = CurrentLng
            };

            var getHatchedEggsMessage = new GetHatchedEggsMessage();
            var getInventoryMessage = new GetInventoryMessage
            {
                LastTimestampMs = DateTime.UtcNow.ToUnixTime()
            };
            var checkAwardedBadgesMessage = new CheckAwardedBadgesMessage();
            var downloadSettingsMessage = new DownloadSettingsMessage
            {
                Hash = VersionHash
            };

            SetRequestBuilder();
            var request = _requestBuilder.GetRequestEnvelope(
                new Request
                {
                    RequestType = RequestType.GetMapObjects,
                    RequestMessage = customRequest.ToByteString()
                },
                new Request
                {
                    RequestType = RequestType.GetHatchedEggs,
                    RequestMessage = getHatchedEggsMessage.ToByteString()
                }, new Request
                {
                    RequestType = RequestType.GetInventory,
                    RequestMessage = getInventoryMessage.ToByteString()
                }, new Request
                {
                    RequestType = RequestType.CheckAwardedBadges,
                    RequestMessage = checkAwardedBadgesMessage.ToByteString()
                }, new Request
                {
                    RequestType = RequestType.DownloadSettings,
                    RequestMessage = downloadSettingsMessage.ToByteString()
                });

            return
                await _httpClient.PostProtoPayload <Request, GetMapObjectsResponse, GetHatchedEggsResponse, GetInventoryResponse, CheckAwardedBadgesResponse, DownloadSettingsResponse> ($"https://{_apiUrl}/rpc", request, ApiFailureStrategy);
        }

        public async Task<GetPlayerResponse> GetProfile()
        {
            SetRequestBuilder();
            var profileRequest = _requestBuilder.GetInitialRequestEnvelope(new Request { RequestType = RequestType.GetPlayer, RequestMessage = new GetPlayerMessage().ToByteString() });

            return
                await _httpClient.PostProtoPayload<Request, GetPlayerResponse>($"https://{_apiUrl}/rpc", profileRequest, ApiFailureStrategy);
        }

        public async Task<DownloadSettingsResponse> GetSettings()
        {
            SetRequestBuilder();
            var settingsRequest = _requestBuilder.GetRequestEnvelope(RequestType.DownloadSettings, new DownloadSettingsMessage()
            {
                Hash = VersionHash
            });
            return
                await
                    _httpClient.PostProtoPayload<Request, DownloadSettingsResponse>($"https://{_apiUrl}/rpc",
                        settingsRequest, ApiFailureStrategy);
        }

        public async Task<RecycleInventoryItemResponse> RecycleItem(ItemId itemId, int amount)
        {
            var customRequest = new RecycleInventoryItemMessage()
            {
                ItemId = itemId,
                Count = amount
            };

            SetRequestBuilder();
            var releasePokemonRequest = _requestBuilder.GetRequestEnvelope(
                    RequestType.RecycleInventoryItem,
                     customRequest);

            return
                await
                    _httpClient.PostProtoPayload<Request, RecycleInventoryItemResponse>($"https://{_apiUrl}/rpc",
                        releasePokemonRequest, ApiFailureStrategy);
        }


        public void SaveLatLng(double lat, double lng)
        {
            var latlng = lat + ":" + lng;
            File.WriteAllText(Directory.GetCurrentDirectory() + "\\Coords.txt", latlng);
        }

        public async Task<FortSearchResponse> SearchFort(string fortId, double fortLat, double fortLng)
        {
            var customRequest = new FortSearchMessage
            {
                FortId = fortId,
                FortLatitude = fortLat,
                FortLongitude = fortLng,
                PlayerLatitude = CurrentLat,
                PlayerLongitude = CurrentLng
            };
            
            SetRequestBuilder();
            var fortDetailRequest = _requestBuilder.GetRequestEnvelope(RequestType.FortSearch, customRequest);

            return
                await
                    _httpClient.PostProtoPayload<Request, FortSearchResponse>($"https://{_apiUrl}/rpc",
                        fortDetailRequest, ApiFailureStrategy);
        }

        /// <summary>
        ///     For GUI clients only. GUI clients don't use the DoGoogleLogin, but call the GoogleLogin class directly
        /// </summary>
        /// <param name="type"></param>
        public void SetAuthType(AuthType type)
        {
            AuthType = type;
        }

        public static double getElevation(double lat, double lon)
        {
            Random random = new Random();
            double maximum = 11.0f;
            double minimum = 8.6f;
            double return1 = random.NextDouble() * (maximum - minimum) + minimum;

            return return1;
        }

        public void SetCoordinates(double lat, double lng, double altitude)
        {
            CurrentLat = lat;
            CurrentLng = lng;
            CurrentAltitude = getElevation(lat, lng);
            SaveLatLng(lat, lng);
        }

        public async Task SetServer()
        {
            #region Standard intial request messages in right Order
            var requestBuilder = new RequestBuilder(AccessToken, AuthType, CurrentLat, CurrentLng,
                CurrentAltitude, AuthTicket);
            var getPlayerMessage = new GetPlayerMessage();
            var getHatchedEggsMessage = new GetHatchedEggsMessage();
            var getInventoryMessage = new GetInventoryMessage
            {
                LastTimestampMs = DateTime.UtcNow.ToUnixTime()
            };
            var checkAwardedBadgesMessage = new CheckAwardedBadgesMessage();
            var downloadSettingsMessage = new DownloadSettingsMessage
            {
                Hash = VersionHash
            };

            #endregion
            SetRequestBuilder();
            var serverRequest = requestBuilder.GetInitialRequestEnvelope(
                new Request
                {
                    RequestType = RequestType.GetPlayer,
                    RequestMessage = getPlayerMessage.ToByteString()
                }, new Request
                {
                    RequestType = RequestType.GetHatchedEggs,
                    RequestMessage = getHatchedEggsMessage.ToByteString()
                }, new Request
                {
                    RequestType = RequestType.GetInventory,
                    RequestMessage = getInventoryMessage.ToByteString()
                }, new Request
                {
                    RequestType = RequestType.CheckAwardedBadges,
                    RequestMessage = checkAwardedBadgesMessage.ToByteString()
                }, new Request
                {
                    RequestType = RequestType.DownloadSettings,
                    RequestMessage = downloadSettingsMessage.ToByteString()
                });


            var serverResponse = await _httpClient.PostProto<Request>(Resources.RpcUrl, serverRequest);

            if (serverResponse.AuthTicket == null)
            {
                AuthToken = null;
                throw new AccessTokenExpiredException();
            }

            AuthTicket = serverResponse.AuthTicket;
            _apiUrl = serverResponse.ApiUrl;
        }

        public async Task<ReleasePokemonResponse> TransferPokemon(ulong pokemonId)
        {
            var customRequest = new ReleasePokemonMessage
            {
                PokemonId = pokemonId
            };
            SetRequestBuilder();
            var releasePokemonRequest = _requestBuilder.GetRequestEnvelope(RequestType.ReleasePokemon, customRequest);
            return
                await
                    _httpClient.PostProtoPayload<Request, ReleasePokemonResponse>($"https://{_apiUrl}/rpc",
                        releasePokemonRequest, ApiFailureStrategy);
        }

        public async Task<PlayerUpdateResponse> UpdatePlayerLocation(double lat, double lng, double alt)
        {
            SetCoordinates(lat, lng, alt);
            var customRequest = new PlayerUpdateMessage
            {
                Latitude = CurrentLat,
                Longitude = CurrentLng
            };
            SetRequestBuilder();
            var updateRequest = _requestBuilder.GetRequestEnvelope(RequestType.PlayerUpdate, customRequest);

            var updateResponse =
                await
                    _httpClient.PostProtoPayload<Request, PlayerUpdateResponse>($"https://{_apiUrl}/rpc", updateRequest, ApiFailureStrategy);
            return updateResponse;
        }

        public async Task<UseItemCaptureResponse> UseCaptureItem(ulong encounterId, ItemId itemId, string spawnPointGuid)
        {
            var customRequest = new UseItemCaptureMessage
            {
                EncounterId = encounterId,
                ItemId = itemId,
                SpawnPointId = spawnPointGuid
            };
            SetRequestBuilder();
            var useItemRequest = _requestBuilder.GetRequestEnvelope(RequestType.UseItemCapture, customRequest);
            return
                await
                    _httpClient.PostProtoPayload<Request, UseItemCaptureResponse>($"https://{_apiUrl}/rpc",
                        useItemRequest, ApiFailureStrategy);
        }

        public async Task<UseItemCaptureResponse> UseItemExpBoost(ItemId itemId)
        {
            var customRequest = new UseItemCaptureMessage
            {
                ItemId = itemId
            };
            SetRequestBuilder();
            var useItemRequest = _requestBuilder.GetRequestEnvelope(RequestType.UseItemXpBoost, customRequest);
            return
                await
                    _httpClient.PostProtoPayload<Request, UseItemCaptureResponse>($"https://{_apiUrl}/rpc",
                        useItemRequest, ApiFailureStrategy);
        }

        public async Task<UseItemCaptureResponse> UseItemIncense(ItemId itemId)
        {
            var customRequest = new UseItemCaptureMessage
            {
                ItemId = itemId
            };
            SetRequestBuilder();
            var useItemRequest = _requestBuilder.GetRequestEnvelope(RequestType.UseIncense, customRequest);
            return
                await
                    _httpClient.PostProtoPayload<Request, UseItemCaptureResponse>($"https://{_apiUrl}/rpc",
                        useItemRequest, ApiFailureStrategy);
        }
    }
}