using Xunit;

namespace GameProfile.Utils.Logging;

public class ConsoleLogger(ITestContext context) : ILogger
{
    private readonly ITestContext _testContext = context;

    public void Log(string message)
    {
        if (_testContext?.TestOutputHelper != null)
        {
            _testContext.TestOutputHelper.WriteLine(message);
        }
        else
        {
            Console.WriteLine(message);
        }
    }
}
