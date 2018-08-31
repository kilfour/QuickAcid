using Xunit.Abstractions;

namespace QuickAcid.Tests.ZheZhools
{
    public class UsingTestOutput
    {
        protected readonly ITestOutputHelper Output;

        public UsingTestOutput(ITestOutputHelper output)
        {
            Output = output;
        }
    }
}