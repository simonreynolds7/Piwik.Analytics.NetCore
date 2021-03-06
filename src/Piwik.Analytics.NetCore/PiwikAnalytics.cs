#region license

// http://www.gnu.org/licenses/gpl-3.0.html GPL v3 or later

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Piwik.Analytics.NetCore.Parameters;

namespace Piwik.Analytics.NetCore
{
    public abstract class PiwikAnalytics
    {
        public static void Initialize(string url, string tokenAuth)
        {
            if (!Uri.IsWellFormedUriString(url, UriKind.Absolute))
            {
                throw new ArgumentException(nameof(url), @"url must be well formed");
            }
            
            if (string.IsNullOrWhiteSpace(tokenAuth))
            {
                throw new ArgumentException(nameof(tokenAuth), @"tokenAuth cannot be empty");
            }
            
            Initialize(new Uri(url), tokenAuth);
        }
        
        public static void Initialize(Uri uri, string tokenAuth)
        {
            if (uri == null)
            {
                throw new ArgumentException(nameof(uri), @"uri cannot be null");
            }
            
            if (string.IsNullOrWhiteSpace(tokenAuth))
            {
                throw new ArgumentException(nameof(tokenAuth), @"tokenAuth cannot be empty");
            }
            
            Url = uri;
            _tokenAuth = tokenAuth;
        }
        
        public static Uri Url { get; private set; }
        
        private static string _tokenAuth;

        protected abstract string GetPlugin();

        protected async Task<T> SendRequestAsync<T>(string method, params Parameter[] parameters)
        {
            if (Url == null)
            {
                throw new ArgumentException(nameof(Url), @"Url cannot be null");
            }
            
            if (string.IsNullOrWhiteSpace(_tokenAuth))
            {
                throw new ArgumentException(nameof(_tokenAuth), @"tokenAuth cannot be empty");
            }
            
            var uri = BuildUri(method, parameters);

            var response = await GetResponse(uri);

            var deserializedObject = JsonConvert.DeserializeObject<T>(response);

            if (deserializedObject == null)
            {
                throw new PiwikApiException(
                    "The server response is not deserializable. " +
                    "Please contact the developer with the following details : response = " + response
                    );
            }

            return deserializedObject;
        }
        
        private Uri BuildUri(string method, IEnumerable<Parameter> parameters)
        {
            var uriBuilder = new UriBuilder(Url);

            var defaultParameters = new List<Parameter>
            {
                new SimpleParameter("module", "API"),
                new SimpleParameter("format", "json"),
                new SimpleParameter("token_auth", _tokenAuth),
                new SimpleParameter("method", GetPlugin() + "." + method)
            };

            parameters = parameters.Union(defaultParameters).ToList();

            var queryString = parameters.Aggregate(string.Empty, (current, parameter) => current + parameter.Serialize());

            uriBuilder.Query = queryString;

            return uriBuilder.Uri;
        }
        
        private static async Task<string> GetResponse(Uri uri)
        {
            var wc = new HttpClient();
            var response = await wc.GetStringAsync(uri);
            return response;
        }
    }
}