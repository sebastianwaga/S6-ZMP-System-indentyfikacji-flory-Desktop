using VirtualHerbarium.AdminPanel.ViewModels;
using VirtualHerbarium.AdminPanel.Views;
using Xunit;
using System.ComponentModel;

namespace VirtualHerbarium.AdminPanel.Tests;

public class LoginViewModelTests
{
    [Fact]
    public void Login_Property_Should_Store_Value()
    {
        var vm = new LoginViewModel(null!);

        vm.Login = "admin";

        Assert.Equal("admin", vm.Login);
    }
    [Fact]
    public void Password_Property_Should_Store_Value()
    {
        var vm = new LoginViewModel(null!);

        vm.Password = "123";

        Assert.Equal("123", vm.Password);
    }
    [Fact]
    public void ErrorMessage_Property_Should_Store_Value()
    {
        var vm = new LoginViewModel(null!);

        vm.ErrorMessage = "error";

        Assert.Equal("error", vm.ErrorMessage);
    }

    [Fact]
    public void IsBusy_Should_Be_False_By_Default()
    {
        var vm = new LoginViewModel(null!);

        Assert.False(vm.IsBusy);
    }
    [Fact]
    public void Login_Should_Raise_PropertyChanged()
    {
        var vm = new LoginViewModel(null!);

        string? changedProperty = null;

        vm.PropertyChanged += (sender, args) =>
        {
            changedProperty = args.PropertyName;
        };

        vm.Login = "admin";

        Assert.Equal("Login", changedProperty);
    }
    [Fact]
    public void Password_Should_Raise_PropertyChanged()
    {
        var vm = new LoginViewModel(null!);

        string? changedProperty = null;

        vm.PropertyChanged += (sender, args) =>
        {
            changedProperty = args.PropertyName;
        };

        vm.Password = "123";

        Assert.Equal("Password", changedProperty);
    }
    [Fact]
    public void ErrorMessage_Should_Raise_PropertyChanged()
    {
        var vm = new LoginViewModel(null!);

        string? changedProperty = null;

        vm.PropertyChanged += (s, e) =>
        {
            changedProperty = e.PropertyName;
        };

        vm.ErrorMessage = "Test error";

        Assert.Equal("ErrorMessage", changedProperty);
    }
    [Fact]
    public void IsBusy_Should_Raise_PropertyChanged()
    {
        var vm = new LoginViewModel(null!);

        string? changedProperty = null;

        vm.PropertyChanged += (s, e) =>
        {
            changedProperty = e.PropertyName;
        };

        vm.IsBusy = true;

        Assert.Equal("IsBusy", changedProperty);
    }

}