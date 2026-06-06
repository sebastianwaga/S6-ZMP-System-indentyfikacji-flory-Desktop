using VirtualHerbarium.AdminPanel.ViewModels;
using Xunit;

namespace VirtualHerbarium.AdminPanel.Tests;

public class AsyncCommandTests
{
    [Fact]
    public void CanExecute_Should_Be_True_Initially()
    {
        var command = new AsyncCommand(() => Task.CompletedTask);

        Assert.True(command.CanExecute(null));
    }
    [Fact]
    public async Task Execute_Should_Run_Action()
    {
        bool executed = false;

        var command = new AsyncCommand(() =>
        {
            executed = true;
            return Task.CompletedTask;
        });

        command.Execute(null);

        await Task.Delay(50);

        Assert.True(executed);
    }
    [Fact]
    public async Task CanExecute_Should_Be_False_During_Execution()
    {
        var started = false;

        var command = new AsyncCommand(async () =>
        {
            started = true;
            await Task.Delay(100);
        });

        command.Execute(null);

        await Task.Delay(10);

        Assert.True(started);
        Assert.False(command.CanExecute(null));

        await Task.Delay(150);

        Assert.True(command.CanExecute(null));
    }
}