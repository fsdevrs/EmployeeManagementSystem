using BaseLibrary.DTOs;
using BaseLibrary.Responses;

namespace ClientLibrary.Services.Contracts
{
    public interface IUserAccountService
    {
        Task<GenralResponse> CreateAsync(Register user);
        Task<LoginResponse> SignInAsync(Login user);
        Task<LoginResponse> RefreshTokenAsync(RefreshToken token);
        Task<WeatherForecast[]> GetWeatherForecasts();
    }
}
