using Auth.Client.Dto;
using Auth.Client.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace Auth.Client.Pages;

public partial class ChangePassword : ComponentBase
{
    private readonly EditContext _context;
    private string? _errorMessage = null;
    private ChangePasswordDto? _request;
    [Inject] protected NavigationManager Navigation { get; set; }
    [Inject] protected IAuthClient AuthClient { get; set; }

    public ChangePassword()
    {
        _request = new ChangePasswordDto();
        _context = new EditContext(_request);
    }

    private async Task Submit()
    {
        try
        {
            await AuthClient.ChangePasswordAsync(_request);
            Navigation.NavigateTo("/users");
        }
        catch (ServiceErrorException e)
        {
            _errorMessage = e.Message;
        }
        catch (Exception)
        {
            _errorMessage = "Ошибка";
        }
    }
}