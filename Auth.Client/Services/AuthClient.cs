using System.Collections;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Auth.Client.Dto;
using Auth.Client.Interfaces;
using Blazored.LocalStorage;

namespace Auth.Client.Services;

public class AuthClient : IAuthClient
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILocalStorageService _localStorageService;

    public AuthClient(IHttpClientFactory httpClientFactory, ILocalStorageService localStorageService)
    {
        _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        _localStorageService = localStorageService ?? throw new ArgumentNullException(nameof(localStorageService));
    }

    public async Task GetTokenAsync(LoginDto dto)
    {
        using var client = _httpClientFactory.CreateClient("auth");
        var result = await client.PostAsJsonAsync("/token", dto);
        result.EnsureSuccessStatusCode();
        var response = await result.Content.ReadFromJsonAsync<LoginResponseDto>();
        await _localStorageService.SetItemAsync("token", response);
    }

    public async Task<IEnumerable<UserDto>> GetUsersAsync()
    {
        var token = await GetToken();

        using var client = _httpClientFactory.CreateClient("auth");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var result = await client.GetAsync("/users");
        result.EnsureSuccessStatusCode();
        var response = await result.Content.ReadFromJsonAsync<IEnumerable<UserDto>>();
        return response;
    }

    public async Task AddUserAsync(NewUserDto dto)
    {
        var token = await GetToken();
        using var client = _httpClientFactory.CreateClient("auth");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        
        var result = await client.PostAsJsonAsync("/add-user", dto);
        if (!result.IsSuccessStatusCode)
        {
            var errorDto = await result.Content.ReadFromJsonAsync<ErrorDto>();
            throw new ServiceErrorException(errorDto.Id, errorDto.Message);
        }
    }

    public async Task ChangePasswordAsync(ChangePasswordDto dto)
    {
        var token = await GetToken();
        using var client = _httpClientFactory.CreateClient("auth");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        
        var result = await client.PostAsJsonAsync("/change-password", dto);
        result.EnsureSuccessStatusCode();
    }

    private async Task<string> GetToken()
    {
        var tokenResult = await _localStorageService.GetItemAsync<LoginResponseDto>("token");
        if (tokenResult == null || tokenResult.Expires < DateTime.UtcNow)
        {
            throw new UnauthorizedAccessException();
        }

        return tokenResult.Token;
    }
}