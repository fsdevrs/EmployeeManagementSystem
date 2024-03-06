using BaseLibrary.DTOs;
using BaseLibrary.Responses;
using ClientLibrary.Helpers;
using ClientLibrary.Services.Contracts;
using System.Net.Http.Json;

namespace ClientLibrary.Services.Implementations
{
    public class UserAccountService : IUserAccountService
    {
        private readonly GetHttpClient _getHttpClient;

        public UserAccountService(GetHttpClient getHttpClient) 
        {
            _getHttpClient = getHttpClient;
        }

        public const string AuthUrl = "api/authentication";

        public async Task<GenralResponse> CreateAsync(Register user)
        {
            var httpclient = _getHttpClient.GetPublicHttpClient();
            var result = await httpclient.PostAsJsonAsync($"{AuthUrl}/register", user);
            if (!result.IsSuccessStatusCode) return new GenralResponse(false, "Error occured");


            var content = await result.Content.ReadAsStringAsync();
            if (string.IsNullOrEmpty(content))
            {
                return new GenralResponse(false, "Empty response from server");
            }

            return await result.Content.ReadFromJsonAsync<GenralResponse>()!;
        }
        public Task<LoginResponse> SignInAsync(Login user)
        {
            throw new NotImplementedException();
        }
        public Task<LoginResponse> RefreshTokenAsync(RefreshToken token)
        {
            throw new NotImplementedException();
        }
        public Task<WeatherForecast[]> GetWeatherForecasts()
        {
            throw new NotImplementedException();
        }  
    }
}
