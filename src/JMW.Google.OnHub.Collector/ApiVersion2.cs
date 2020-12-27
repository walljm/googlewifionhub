using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace JMW.Google.OnHub.Collector
{
    public class ApiVersion2
    {
        private static readonly HttpClient client = new HttpClient();

        public static async Task<string> GetDevicesUsingV2Api(string refreshToken)
        {
            string devicesUrl = "https://googlehomefoyer-pa.googleapis.com/v2/groups/AAAAABlsocQ/stations?prettyPrint=false";
            // string systemsUrl = "https://googlehomefoyer-pa.googleapis.com/v2/groups?prettyPrint=false";

            var authResponse = await client.PostAsync("https://www.googleapis.com/oauth2/v4/token", new FormUrlEncodedContent(new Dictionary<string, string>
            {
                {"client_id", "936475272427.apps.googleusercontent.com"},
                {"grant_type", "refresh_token"},
                {"refresh_token", refreshToken}
            }));
            var auth = await authResponse.Content.ReadAsAsync<AuthResp>();

            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(auth.TokenType, auth.AccessToken);
            var tokenResponse = await client.PostAsync("https://oauthaccountmanager.googleapis.com/v1/issuetoken",
                new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    {"app_id", "com.google.JMW.Google.OnHub.Collector"},
                    {"client_id", "586698244315-vc96jg3mn4nap78iir799fc2ll3rk18s.apps.googleusercontent.com"},
                    {"hl", "en-US"},
                    {"lib_ver", "3.3"},
                    {"response_type", "token"},
                    {
                        "scope",
                        "https://www.googleapis.com/auth/accesspoints https://www.googleapis.com/auth/clouddevices"
                    }
                }));
            var token = await tokenResponse.Content.ReadAsAsync<TokenResp>();

            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(auth.TokenType, token.Token);
            //var systemsResponse = await client.GetAsync(systemsUrl);
            //var systems = await systemsResponse.Content.ReadAsStringAsync();

            var devicesResponse = await client.GetAsync(devicesUrl);
            return await devicesResponse.Content.ReadAsStringAsync();
        }

        private class TokenResp
        {
            public string IssueAdvice { get; set; }
            public string Token { get; set; }
            public string ExpiresIn { get; set; }
            public string IdToken { get; set; }
        }

        private class AuthResp
        {
            public string AccessToken { get; set; }
            public int ExpiresIn { get; set; }
            public string Scope { get; set; }
            public string TokenType { get; set; }
            public string IdToken { get; set; }
        }
    }
}