using QuickAcid.Tests._Tools.Models;
using QuickAcid.Tests._Tools.ThePress;
using QuickFuzzr;
using QuickPulse.Explains;

namespace QuickAcid.Tests;

[DocFile]
[DocFileHeader("QuickAcid")]
[DocContent("> Drop it in acid. Look for gold. Like alchemy, but reproducible.")]
public class ReadMe
{
    [Fact]
    [DocContent(
@"QuickAcid is a property-based testing library for C# that combines:

* LINQ-based test scripting
* Shrinkable, structured inputs
* Minimal-case failure reporting
* Customizable shrinking strategies
* Deep state modeling and execution traces

It's designed for sharp diagnostics, elegant expressiveness, and easy extension."
    )]
    public void Generate()
    {
        Explain.OnlyThis<ReadMe>("README.md");
    }

    [Fact]
    [DocHeader("Example")]
    [DocContent("Given a naive `Account` model:")]
    [DocExample(typeof(Account))]
    [DocContent("You can test the overdraft invariant like this:")]
    [DocExample(typeof(ReadMe), nameof(ExampleTest))]
    [DocContent("The generic arguments to the various `Script` methods are just record markers:")]
    [DocCodeFile("ReadMeTestMarkers.cs")]
    [DocHeader("Example Failure Output:")]
    [DocCodeFile("ReadMe.qr")]
    public void Example()
    {
        var article = TheJournalist.Exposes(ExampleTest);

        Assert.Equal(1, article.Total().Executions());
        Assert.Equal(1, article.Total().Actions());

        var action = article.Execution(1).Action(1).Read();
        Assert.Equal("Withdraw", action.Label);

        var tracked = article.Execution(1).Tracked(1).Read();
        Assert.Equal("Account", tracked.Label);
        Assert.Equal("{ Balance: 0 }", tracked.Value);

        Assert.Equal(1, article.Total().Inputs());
        var input = article.Execution(1).Input(1).Read();
        Assert.Equal("Withdraw Amount", input.Label);
        Assert.Equal("9", input.Value);

        var passedSpec = article.PassedSpec(1).Read();
        Assert.Equal("No overdraft", passedSpec.Label);
        Assert.Equal(3, passedSpec.TimesPassed);

        // -- save the file for doc, if things change
        // TheLedger.Rewrites("QuickAcid.Tests/ReadMe.qr").Absorb(TheClerk.Transcribes(article.CaseFile));
    }

    [CodeSnippet]
    [CodeRemove(", 1584314623")]
    private static void ExampleTest()
    {
        var script =
            from account in Script.Tracked(() => new Account())
            from _ in Script.Choose(
                from amount in Script.Input<Deposit.Amount>().With(Fuzz.Int(0, 10))
                from act in Script.Act<Deposit>(() => account.Deposit(amount))
                select Acid.Test,
                from amount in Script.Input<Withdraw.Amount>().With(Fuzz.Int(0, 10))
                from act in Script.Act<Withdraw>(() => account.Withdraw(amount))
                select Acid.Test)
            from spec in Script.Spec<NoOverdraft>(() => account.Balance >= 0)
            select Acid.Test;
        QState.Run(script, 1584314623)
            .WithOneRun()
            .And(50.ExecutionsPerRun());
    }
}