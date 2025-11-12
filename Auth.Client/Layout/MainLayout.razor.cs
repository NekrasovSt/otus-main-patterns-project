using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;

namespace Auth.Client.Layout;

public partial class MainLayout : LayoutComponentBase
{
    private bool _exists;
    [Inject] protected NavigationManager? Navigation { get; set; }
    [Inject] protected ILocalStorageService? BrowserStorage { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        _exists = await BrowserStorage!.ContainKeyAsync("token");
    }

    void LogOut()
    {
        BrowserStorage!.RemoveItemAsync("token");
        Navigation!.NavigateTo("/login");
    }
}