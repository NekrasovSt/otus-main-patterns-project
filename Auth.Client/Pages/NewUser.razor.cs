using Auth.Client.Dto;
using Auth.Client.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace Auth.Client.Pages;

public partial class NewUser : ComponentBase
{
    private readonly EditContext _context;
    private string? _errorMessage = null;
    private readonly NewUserDto _request;
    [Inject] protected NavigationManager? Navigation { get; set; }
    [Inject] protected IAuthClient? AuthClient { get; set; }

    public NewUser()
    {
        _request = new NewUserDto();
        _context = new EditContext(_request);
    }

    private async Task Submit()
    {
        _errorMessage = null;
        try
        {
            await AuthClient!.AddUserAsync(_request);
            Navigation!.NavigateTo("/users");
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