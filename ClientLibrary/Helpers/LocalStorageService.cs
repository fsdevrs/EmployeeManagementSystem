using Blazored.LocalStorage;

namespace ClientLibrary.Helpers
{
    public class LocalStorageService
    {
        private readonly ILocalStorageService _localStorageService;
        public  LocalStorageService(ILocalStorageService localStorageService)
        {
            _localStorageService = localStorageService;
        }
        private const string StorageKey = "authentication-token";
        public async Task<string?> GetToken() => await _localStorageService.GetItemAsStringAsync(StorageKey);
        public async Task SetToken(string item) => await _localStorageService.SetItemAsStringAsync(StorageKey, item);
        public async Task RemoveToken() => await _localStorageService.RemoveItemAsync(StorageKey);
    }
}
