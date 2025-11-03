using Auth.Client.Dto;
using Auth.Client.Interfaces;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace Auth.Client.Pages;

public partial class Login: ComponentBase
{
    private readonly LoginDto _request;
    private readonly EditContext _context;
    private string? _errorMessage = null;
    [Inject] protected NavigationManager Navigation { get; set; }

    [Inject] protected IAuthClient AuthClient { get; set; }
    [Inject] protected ILocalStorageService BrowserStorage { get; set; }

    public Login()
    {
        _request = new LoginDto();
        _context = new EditContext(_request);
    }

    public async Task Submit()
    {
        try
        {
            _errorMessage = null;
            await AuthClient.GetTokenAsync(_request);
            Navigation.NavigateTo("/users");
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            _errorMessage = "Неверный логин или пароль";
        }
        catch (Exception ex)
        {
            _errorMessage = "Ошибка";
        }
    }
}