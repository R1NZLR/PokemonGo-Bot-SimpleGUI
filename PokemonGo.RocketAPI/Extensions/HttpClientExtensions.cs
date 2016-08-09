using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Google.Protobuf;
using PokemonGo.RocketAPI.Exceptions;
using POGOProtos.Networking.Envelopes;

namespace PokemonGo.RocketAPI.Extensions
{
    using System;

    public enum ApiOperation
    {
        Retry,
        Abort
    }

    public interface IApiFailureStrategy
    {
        Task<ApiOperation> HandleApiFailure(RequestEnvelope request, ResponseEnvelope response);
        void HandleApiSuccess(RequestEnvelope request, ResponseEnvelope response);
    }

    public static class HttpClientExtensions
    {
        public static async Task<IMessage[]> PostProtoPayload<TRequest>(this System.Net.Http.HttpClient client, 
            string url, RequestEnvelope requestEnvelope,
            IApiFailureStrategy strategy,
            params Type[] responseTypes) where TRequest : IMessage<TRequest>
        {
            var result = new IMessage[responseTypes.Length];
            for (var i = 0; i < responseTypes.Length; i++)
            {
                result[i] = Activator.CreateInstance(responseTypes[i]) as IMessage;
                if (result[i] == null)
                {
                    throw new ArgumentException($"ResponseType {i} is not an IMessage");
                }
            }

            ResponseEnvelope response;
            while ((response = await PostProto<TRequest>(client, url, requestEnvelope)).Returns.Count != responseTypes.Length)
            {
                var operation = await strategy.HandleApiFailure(requestEnvelope, response);
                if (operation == ApiOperation.Abort)
                {
                    throw new InvalidResponseException();
                }
            }

            strategy.HandleApiSuccess(requestEnvelope, response);

            for (var i = 0; i < responseTypes.Length; i++)
            {
                var payload = response.Returns[i];
                result[i].MergeFrom(payload);
            }
            return result;
        }

        public static async Task<TResponsePayload> PostProtoPayload<TRequest, TResponsePayload>(this System.Net.Http.HttpClient client,
            string url, RequestEnvelope requestEnvelope, IApiFailureStrategy strategy) where TRequest : IMessage<TRequest>
            where TResponsePayload : IMessage<TResponsePayload>, new()
        {
            Debug.WriteLine($"Requesting {typeof(TResponsePayload).Name}");
            var response = await PostProto<TRequest>(client, url, requestEnvelope);

            while (response.Returns.Count == 0)
            {
                var operation = await strategy.HandleApiFailure(requestEnvelope, response);
                if (operation == ApiOperation.Abort)
                {
                    break;
                }

                response = await PostProto<TRequest>(client, url, requestEnvelope);
            }

            if (response.Returns.Count == 0)
                throw new InvalidResponseException();

            strategy.HandleApiSuccess(requestEnvelope, response);

            //Decode payload
            //todo: multi-payload support
            var payload = response.Returns[0];
            var parsedPayload = new TResponsePayload();
            parsedPayload.MergeFrom(payload);

            return parsedPayload;
        }

        public static async Task<ResponseEnvelope> PostProto<TRequest>(this System.Net.Http.HttpClient client, string url,
            RequestEnvelope requestEnvelope) where TRequest : IMessage<TRequest>
        {
            //Encode payload and put in envelop, then send
            var data = requestEnvelope.ToByteString();
            var result = await client.PostAsync(url, new ByteArrayContent(data.ToByteArray()));
            await Task.Delay(200);

            //Decode message
            var responseData = await result.Content.ReadAsByteArrayAsync();
            var codedStream = new CodedInputStream(responseData);
            var decodedResponse = new ResponseEnvelope();
            decodedResponse.MergeFrom(codedStream);

            return decodedResponse;
        }

        public static async Task<ResponseEnvelope> PostProto<TRequest>(string url, RequestEnvelope requestEnvelope) where TRequest : IMessage<TRequest>
        {
            return await PostProto<TRequest>(url, requestEnvelope);
        }

        public static async Task<Tuple<T1, T2>> PostProtoPayload<TRequest, T1, T2>(this System.Net.Http.HttpClient client, string url, RequestEnvelope requestEnvelope, IApiFailureStrategy apiFailureStrategy) where TRequest : IMessage<TRequest>
            where T1 : class, IMessage<T1>, new()
            where T2 : class, IMessage<T2>, new()
        {
            var responses = await client.PostProtoPayload<TRequest>(url, requestEnvelope, apiFailureStrategy, typeof(T1), typeof(T2));
            return new Tuple<T1, T2>(responses[0] as T1, responses[1] as T2);
        }

        public static async Task<Tuple<T1, T2, T3>> PostProtoPayload<TRequest, T1, T2, T3>(this System.Net.Http.HttpClient client, string url, RequestEnvelope requestEnvelope, IApiFailureStrategy apiFailureStrategy) where TRequest : IMessage<TRequest>
            where T1 : class, IMessage<T1>, new()
            where T2 : class, IMessage<T2>, new()
            where T3 : class, IMessage<T3>, new()
        {
            var responses = await client.PostProtoPayload<TRequest>(url, requestEnvelope, apiFailureStrategy, typeof(T1), typeof(T2), typeof(T3));
            return new Tuple<T1, T2, T3>(responses[0] as T1, responses[1] as T2, responses[2] as T3);
        }

        public static async Task<Tuple<T1, T2, T3, T4>> PostProtoPayload<TRequest, T1, T2, T3, T4>(this System.Net.Http.HttpClient client, string url, RequestEnvelope requestEnvelope, IApiFailureStrategy apiFailureStrategy) where TRequest : IMessage<TRequest>
            where T1 : class, IMessage<T1>, new()
            where T2 : class, IMessage<T2>, new()
            where T3 : class, IMessage<T3>, new()
            where T4 : class, IMessage<T4>, new()
        {
            var responses = await client.PostProtoPayload<TRequest>(url, requestEnvelope, apiFailureStrategy, typeof(T1), typeof(T2), typeof(T3), typeof(T4));
            return new Tuple<T1, T2, T3, T4>(responses[0] as T1, responses[1] as T2, responses[2] as T3, responses[3] as T4);
        }
        public static async Task<Tuple<T1, T2, T3, T4, T5>> PostProtoPayload<TRequest, T1, T2, T3, T4, T5>(this System.Net.Http.HttpClient client, string url, RequestEnvelope requestEnvelope, IApiFailureStrategy apiFailureStrategy) where TRequest : IMessage<TRequest>
            where T1 : class, IMessage<T1>, new()
            where T2 : class, IMessage<T2>, new()
            where T3 : class, IMessage<T3>, new()
            where T4 : class, IMessage<T4>, new()
            where T5 : class, IMessage<T5>, new()
        {
            var responses = await client.PostProtoPayload<TRequest>(url, requestEnvelope, apiFailureStrategy, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5));
            return new Tuple<T1, T2, T3, T4, T5>(responses[0] as T1, responses[1] as T2, responses[2] as T3, responses[3] as T4, responses[3] as T5);
        }
    }
}