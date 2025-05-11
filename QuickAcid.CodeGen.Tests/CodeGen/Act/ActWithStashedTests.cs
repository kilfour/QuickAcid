using QuickAcid.Bolts;
using QuickAcid.Bolts.Nuts;
using QuickAcid.TestsDeposition._Tools.Models;
using QuickMGenerate;

namespace QuickAcid.TestsDeposition.Linqy.CodeGen.Act
{
    public class ActWithStashedTests
    {
        [Fact(Skip = "wip")]
        public void CodeGen_act_()
        {
            var script =
                from account in "Account".Stashed(() => new Account())
                from withdrawAmount in "withdraw".Input(MGen.Int(42, 42))
                from withdraw in "account.Withdraw:withdraw".Act(() => account.Withdraw(withdrawAmount))
                select Acid.Test;
            var reader = Reader.FromRun(script);
            Assert.Equal("DoingStuff(42);", reader.NextLine());
        }
    }
}