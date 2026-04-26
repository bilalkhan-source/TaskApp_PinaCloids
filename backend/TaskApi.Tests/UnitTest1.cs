using TaskApi.Controllers;

namespace TaskApi.Tests;

public class UnitTest1
{
    [Fact]
    public void TasksController_type_is_loadable()
    {
        Assert.NotNull(typeof(TasksController));
    }
}