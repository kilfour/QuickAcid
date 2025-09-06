using QuickAcid.TheyCanFade;

namespace QuickAcid.Tests.MemoryTests;

public class SwapperTests
{
    [Fact]
    public void ScopedSwap()
    {
        var memory = new Memory(() => 0);
        memory.ForThisExecution().SetIfNotAlreadyThere("int", 0);
        Assert.Equal(0, memory.ForThisExecution().Get<int>("int"));
        using (memory.ScopedSwap("int", 42))
        {
            Assert.Equal(42, memory.ForThisExecution().Get<int>("int"));
        }
        Assert.Equal(0, memory.ForThisExecution().Get<int>("int"));
    }

    [Fact]
    public void ScopedSwap_list()
    {
        var memory = new Memory(() => 0);
        List<int> list = [1];
        memory.ForThisExecution().SetIfNotAlreadyThere("list", list);
        var swapper = new MemoryLens(l => ((List<int>)l)[0], (l, el) => { list[0] = (int)el; return el; });
        using (memory.NestedValue(swapper))
        {
            using (memory.ScopedSwap("list", 42))
            {
                Assert.Equal(42, memory.ForThisExecution().Get<int>("list"));
            }
        }
    }

    [Fact]
    public void ScopedSwap_list_in_list()
    {
        var memory = new Memory(() => 0);
        List<int> list = [1];
        List<List<int>> outer = [list];
        memory.ForThisExecution().SetIfNotAlreadyThere("list", outer);

        var outerSwapper = new MemoryLens(
            l => ((List<List<int>>)l)[0],
            (l, el) => { ((List<List<int>>)l)[0] = (List<int>)el; return l; });

        var swapper = new MemoryLens(
            l => ((List<int>)l)[0],
            (l, el) => { ((List<int>)l)[0] = (int)el; return l; });

        using (memory.NestedValue(outerSwapper))
        {
            using (memory.NestedValue(swapper))
            {
                using (memory.ScopedSwap("list", 42))
                {
                    Assert.Equal(42, list.First());
                }
            }
        }
    }
}