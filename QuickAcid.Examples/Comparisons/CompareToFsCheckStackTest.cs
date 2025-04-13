using FsCheck;
using FsCheck.Fluent;
using FsCheck.Xunit;
using QuickMGenerate;
using QuickAcid.Bolts.Nuts;
using QuickAcid.Bolts;
using QuickAcid.Bolts.Nuts.QuickMGenerateExtensions;

namespace QuickAcid.Examples;

// ====================
// FsCheck Version
// ====================
public enum StackOp { Push, Pop }

public class FsCheckStackSpec
{
    [Property]
    public Property StackShouldBehaveCorrectly()
    {
        var stackOps = Gen.ListOf(
            Gen.OneOf(
                from v in Gen.Choose(0, 100)
                select (StackOp.Push, v), Gen.Constant((StackOp.Pop, 0)))
        );

        return Prop.ForAll(stackOps.ToArbitrary(), ops =>
        {
            var stack = new Stack<int>();
            var pushed = new List<int>();
            var popped = new List<int>();
            var expectedPops = new List<int>();

            foreach (var (op, val) in ops)
            {
                if (op == StackOp.Push)
                {
                    stack.Push(val);
                    pushed.Add(val);
                }
                else if (stack.Count > 0)
                {
                    expectedPops.Add(stack.Peek());
                    popped.Add(stack.Pop());
                }
            }

            var countOkay = stack.Count == (pushed.Count - popped.Count);
            var poppedReverse = popped.SequenceEqual(expectedPops);
            return countOkay && poppedReverse;
        });
    }
}


// ====================
// QuickAcid Version
// ====================
public class StackAcidSpec
{
    [Fact]
    public void StackBehavesCorrectly()
    {
        var run =
            from stack in "stack".AlwaysReported(() => new Stack<int>())
            from pushed in "pushed".AlwaysReported(() => new List<int>())
            from expectedPops in "expectedPops".AlwaysReported(() => new List<int>())
            from popped in "popped".AlwaysReported(() => new List<int>())
            from val in "pushval".ShrinkableInput(MGen.Int(0, 100))
            from action in "step".Choose(
                "push".Act(() => { stack.Push(val); pushed.Add(val); }),
                "pop".ActIf(
                    () => stack.Count > 0,
                    () => { expectedPops.Add(stack.Peek()); popped.Add(stack.Pop()); }))
            from finalCountOkay in "FinalCountOkay".Spec(
                    () => stack.Count == pushed.Count - popped.Count)
            from popsInReverse in "PopsInReverse".Spec(
                () => popped.SequenceEqual(expectedPops))
            select Acid.Test;
        run.Verify(10, 20);
    }
}