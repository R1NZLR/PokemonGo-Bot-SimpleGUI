#region using directives

using System;
using System.Threading.Tasks;
using PokemonGo.RocketAPI.Enums;
using PokemonGo.RocketAPI.Exceptions;
using PokemonGo.RocketAPI.Extensions;
using POGOProtos.Networking.Envelopes;

#endregion

namespace PokemonGo.RocketAPI.Common
{
    public class ApiFailureStrategy : IApiFailureStrategy
    {
        private int _retryCount;
        private readonly Client _client;

        public ApiFailureStrategy(Client client)
        {
            _client = client;
        }

        public async Task<ApiOperation> HandleApiFailure()
        {
            if (_retryCount == 11)
                return ApiOperation.Abort;

            await Task.Delay(500);
            _retryCount++;

            if (_retryCount % 5 == 0)
            {
                DoLogin();
            }

            return ApiOperation.Retry;
        }

        public async void HandleApiSuccess()
        {
            _retryCount = 0;
            await Task.Delay(200);
        }

        private async void DoLogin()
        {
            try
            {
                if (_client.AuthType != AuthType.Ptc)
                {
                    await _client.DoPtcLogin();
                }
                if (_client.AuthType != AuthType.Google)
                {
                    _client.DoGoogleLogin();
                }
                else
                {
                    // TODO: Consoloe log message
                }
            }
            catch (AggregateException ae)
            {
                throw ae.Flatten().InnerException;
            }
            catch (AccountNotVerifiedException)
            {
                // TODO: Console log message
            }
            catch (AccessTokenExpiredException)
            {

                // TODO: Console log message

                await Task.Delay(1000);
            }
            catch (PtcOfflineException)
            {
                // TODO: Console log message

                await Task.Delay(15000);
            }
            catch (InvalidResponseException)
            {
                // TODO: Console log message

                await Task.Delay(5000);
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }
        public async void HandleApiSuccess(RequestEnvelope request, ResponseEnvelope response)
        {
            _retryCount = 0;
            await Task.Delay(200);
        }

        public async Task<ApiOperation> HandleApiFailure(RequestEnvelope request, ResponseEnvelope response)
        {
            if (_retryCount == 11)
                return ApiOperation.Abort;

            await Task.Delay(500);
            _retryCount++;

            if (_retryCount % 5 == 0)
            {
                try
                {
                    DoLogin();
                }
                catch (PtcOfflineException)
                {
                    await Task.Delay(20000);
                }
                catch (AccessTokenExpiredException)
                {
                    await Task.Delay(2000);
                }
                catch (Exception ex) when (ex is InvalidResponseException || ex is TaskCanceledException)
                {
                    await Task.Delay(1000);
                }
            }

            return ApiOperation.Retry;
        }
    }
}