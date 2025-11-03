using Auth.Client.Dto;
using Auth.Client.Interfaces;
using Microsoft.AspNetCore.Components;

namespace Auth.Client.Pages;

public partial class Users : ComponentBase
{
    [Inject] protected IAuthClient AuthClient { get; set; }
    [Inject] protected NavigationManager Navigation { get; set; }
    private bool _isLoading = false;

    private IEnumerable<UserDto> _users = new List<UserDto>();

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        _isLoading = true;
        try
        {
            _users = await AuthClient.GetUsersAsync();
        }
        catch (UnauthorizedAccessException)
        {
            Navigation.NavigateTo("/login");
        }
        finally
        {
            _isLoading = false;
        }
    }

    private void AddUser()
    {
        Navigation.NavigateTo("/new-user");
    }
}