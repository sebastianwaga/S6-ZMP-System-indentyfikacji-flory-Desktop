using VirtualHerbarium.AdminPanel.ViewModels;
using Xunit;

namespace VirtualHerbarium.AdminPanel.Tests;

public class PlaceholderViewModelTests
{
    [Fact]
    public void Constructor_Should_Set_Text()
    {
        var vm = new PlaceholderViewModel("Test");

        Assert.Equal("Test", vm.Text);
    }

    [Fact]
    public void Text_Should_Not_Be_Null()
    {
        var vm = new PlaceholderViewModel("Hello");

        Assert.NotNull(vm.Text);
    }
}