using VirtualHerbarium.AdminPanel.Services;
using Xunit;

namespace VirtualHerbarium.AdminPanel.Tests;

public class AuthServiceTests
{
    [Fact]
    public void Logout_Should_Clear_Token()
    {
        var auth = AuthService.Instance;

        auth.Logout();

        Assert.Null(auth.Token);
    }

[Fact]
    public void Logout_Should_Clear_CurrentUser()
    {
        var auth = AuthService.Instance;

        auth.Logout();

        Assert.Null(auth.CurrentUser);
    }
}