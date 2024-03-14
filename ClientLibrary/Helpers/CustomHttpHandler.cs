using BaseLibrary.DTOs;
using ClientLibrary.Services.Contracts;
using System.Net;

namespace ClientLibrary.Helpers
{
    public class CustomHttpHandler : DelegatingHandler
    {
        private readonly GetHttpClient _httpClient;
        private readonly LocalStorageService _localstorageService;
        private readonly IUserAccountService _userAccountService;

        public CustomHttpHandler(GetHttpClient httpClient, LocalStorageService localstorageService, IUserAccountService userAccountService)
        {
            _httpClient = httpClient;
            _localstorageService = localstorageService;
            _userAccountService = userAccountService;
        }

        protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            bool loginUrl = request.RequestUri!.AbsoluteUri.Contains("login");
            bool registerUrl = request.RequestUri!.AbsoluteUri.Contains("register");
            bool refreshTokenUrl = request.RequestUri!.AbsoluteUri.Contains("refresh-token");

            if (loginUrl || registerUrl || refreshTokenUrl) return await base.SendAsync(request, cancellationToken);

            var result = await base.SendAsync(request, cancellationToken);
            if(result.StatusCode == HttpStatusCode.Unauthorized)
            {
                var stringToken = await _localstorageService.GetToken();
                if (stringToken == null) return result;

                string token = string.Empty;
                try
                {
                    token = request.Headers.Authorization!.Parameter!;
                }
                catch { }

                var deserilaizedToken = Serializations.DeserializeJsonString<UserSession>(stringToken);
                if (deserilaizedToken is null) return result;

                if (string.IsNullOrEmpty(token))
                {
                    request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", deserilaizedToken.Token);
                    return await base.SendAsync(request, cancellationToken);
                }
                var newJwtToken = await GetRefreshToken(deserilaizedToken.RefreshToken!);
                if(string.IsNullOrEmpty(newJwtToken)) return result;

                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", newJwtToken);
                return await base.SendAsync(request, cancellationToken);
            }
            return result;
        }

        private async Task<string> GetRefreshToken(string refreshToken)
        {
            var result = await _userAccountService.RefreshTokenAsync(new RefreshToken() { Token = refreshToken});
            string serializedToken = Serializations.SerializeObj(new UserSession() { Token = result.Token, RefreshToken = result.RefreshToken });
            await _localstorageService.SetToken(serializedToken);
            return result.Token;
        }
    }
}
