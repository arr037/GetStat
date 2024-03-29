﻿using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace GetStat.Domain.Web
{
    /// <summary>
    /// Provides HTTP calls for sending and receiving information from a HTTP server
    /// </summary>
    public static class WebRequests
    {
        /// <summary>
        /// GETs a web request to an URL and returns the raw http web response
        /// </summary>
        /// <remarks>IMPORTANT: Remember to close the returned <see cref="HttpWebResponse"/> stream once done</remarks>
        /// <param name="url">The URL</param>
        /// <param name="configureRequest">Allows caller to customize and configure the request prior to the request being sent</param>
        /// <param name="bearerToken">If specified, provides the Authorization header with `bearer token-here` for things like JWT bearer tokens</param>
        /// <returns></returns>
        public static async Task<HttpWebResponse> GetAsync(string url, Action<HttpWebRequest> configureRequest = null, string bearerToken = null)
        {
            #region Setup

            // Create the web request
            var request = WebRequest.CreateHttp(url);

            // Make it a GET request method
            request.Method = HttpMethod.Get.ToString();

            // If we have a bearer token...
            if (bearerToken != null)
                // Add bearer token to header
                request.Headers.Add(HttpRequestHeader.Authorization, $"Bearer {bearerToken}");

            // Any custom work
            configureRequest?.Invoke(request);

            #endregion
            
            // Wrap call...
            try
            {
                // Return the raw server response
                return await request.GetResponseAsync() as HttpWebResponse;
            }
            // Catch Web Exceptions (which throw for things like 401)
            catch (WebException ex)
            {
                // If we got a response...
                if (ex.Response is HttpWebResponse httpResponse)
                    // Return the response
                    return httpResponse;

                // Otherwise, we don't have any information to be able to return
                // So re-throw
                throw;
            }
        }

        /// <summary>
        /// POSTs a web request to an URL and returns the raw http web response
        /// </summary>
        /// <remarks>IMPORTANT: Remember to close the returned <see cref="HttpWebResponse"/> stream once done</remarks>
        /// <param name="url">The URL</param>
        /// <param name="content">The content to post</param>
        /// <param name="sendType">The format to serialize the content into</param>
        /// <param name="returnType">The expected type of content to be returned from the server</param>
        /// <param name="configureRequest">Allows caller to customize and configure the request prior to the content being written and sent</param>
        /// <param name="bearerToken">If specified, provides the Authorization header with `bearer token-here` for things like JWT bearer tokens</param>
        /// <returns></returns>
        public static async Task<HttpWebResponse> PostAsync(string url,
            object content = null, 
            Action<HttpWebRequest> configureRequest = null, 
            string bearerToken = null, CancellationToken cancelToken=default)
        {
            #region Setup

            // Create the web request
            HttpWebRequest request = WebRequest.CreateHttp(url);
            // Make it a POST request method
            request.Method = HttpMethod.Post.ToString();
            // Set the appropriate return type
            request.Accept = KnownContentSerializers.Json.ToMimeString();

            // Set the content type
            request.ContentType = KnownContentSerializers.Json.ToMimeString();

            // If we have a bearer token...
            if (bearerToken != null)
                // Add bearer token to header
                request.Headers.Add(HttpRequestHeader.Authorization, $"Bearer {bearerToken}");

            // Any custom work
            configureRequest?.Invoke(request);
            #endregion

            #region Write Content

            // Set the content length
            if (content == null)
            {
                // Set content length to 0
                request.ContentLength = 0;
            }
            // Otherwise...
            else
            {
                // Create content to write
                var contentString  = JsonConvert.SerializeObject(content);



                // 
                //  NOTE: This GetRequestStreamAsync could throw with a
                //        SocketException (or an inner exception of SocketException)
                //
                //        However, we cannot return anything useful from this 
                //        so we just let it throw out so the caller can handle
                //        this (the other PostAsync call for example).
                //
                //        SocketExceptions are a good indication there is no 
                //        Internet, or no connection or firewalls blocking 
                //        communication.
                //

                // Get body stream...
                using (var requestStream = await request.GetRequestStreamAsync())
                // Create a stream writer from the body stream...
                using (var streamWriter = new StreamWriter(requestStream))
                    // Write content to HTTP body stream
                    await streamWriter.WriteAsync(contentString);
            }

            #endregion

            // Wrap call...
            try
            {
                // Return the raw server response
                return await request.GetResponseAsync(cancelToken);
            }
            // Catch Web Exceptions (which throw for things like 401)
            catch (WebException ex)
            {
                // If we got a response...
                if (ex.Response is HttpWebResponse httpResponse)
                    // Return the response
                    return httpResponse;
                // Otherwise, we don't have any information to be able to return
                // So re-throw
                throw;
            }
        }

        /// <summary>
        /// POSTs a web request to an URL and returns a response of the expected data type
        /// </summary>
        /// <param name="url">The URL</param>
        /// <param name="content">The content to post</param>
        /// <param name="sendType">The format to serialize the content into</param>
        /// <param name="returnType">The expected type of content to be returned from the server</param>
        /// <param name="configureRequest">Allows caller to customize and configure the request prior to the content being written and sent</param>
        /// <param name="bearerToken">If specified, provides the Authorization header with `bearer token-here` for things like JWT bearer tokens</param>
        /// <returns></returns>
        public static async Task<WebRequestResult<TResponse>> PostAsync<TResponse>(string url, 
            object content = null, Action<HttpWebRequest> configureRequest = null, string bearerToken = null, CancellationToken cancelToken= default)
        {
            // Create server response holder
            var serverResponse = default(HttpWebResponse);

            try
            {
                // Make the standard Post call first
                serverResponse = await PostAsync(url, content, configureRequest, bearerToken,cancelToken);
            }
            catch (Exception ex)
            {
                // If we got unexpected error, return that
                return new WebRequestResult<TResponse>
                {
                    // Include exception message
                    ErrorMessage = ex.Message,
                    IsCanceled = cancelToken.IsCancellationRequested
                };
            }

            // Create a result
            var result = serverResponse.CreateWebRequestResult<TResponse>();

            // If the response status code is not 200...
            if (result.StatusCode != HttpStatusCode.OK)
            {
                // Call failed
                // Return no error message so the client can display its own based on the status code

                // Done
                return result;
            }

            // If we have no content to deserialize...
            if (string.IsNullOrEmpty(result.RawServerResponse))
                // Done
                return result;

            // Deserialize raw response
            try
            {
                // If the server response type was not the expected type...
                if (!serverResponse.ContentType.ToLower().Contains(KnownContentSerializers.Json.ToMimeString().ToLower()))
                {
                    // Fail due to unexpected return type
                    result.ErrorMessage = $"Server did not return data in expected type. Expected {KnownContentSerializers.Json.ToMimeString()}, received {serverResponse.ContentType}";

                    // Done
                    return result;
                }

                // Json?
                result.ServerResponse = JsonConvert.DeserializeObject<TResponse>(result.RawServerResponse);
            }
            catch (Exception)
            {
                // If deserialize failed then set error message
                result.ErrorMessage = "Failed to deserialize server response to the expected type";

                // Done
                return result;
            }
           
            // Return result
            return result;
        }
    }
    internal static class Extensions
    {
        public static async Task<HttpWebResponse> GetResponseAsync(this HttpWebRequest request, CancellationToken ct)
        {
            await using (ct.Register(request.Abort, useSynchronizationContext: false))
            {
                try
                {
                    var response = await request.GetResponseAsync();
                    return (HttpWebResponse)response;
                }
                catch (WebException ex)
                {
                    // WebException is thrown when request.Abort() is called,
                    // but there may be many other reasons,
                    // propagate the WebException to the caller correctly
                    if (ct.IsCancellationRequested)
                    {
                        // the WebException will be available as Exception.InnerException
                        throw new OperationCanceledException(ex.Message, ex, ct);
                    }

                    // cancellation hasn't been requested, rethrow the original WebException
                    throw;
                }
            }
        }
    }
}
