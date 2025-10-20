using QuickAcid.Proceedings.ClerksOffice;
using QuickAcid.Tests._Tools.Models;
using QuickAcid.Tests._Tools.ThePress;
using QuickFuzzr;
using QuickPulse.Explains;
using QuickPulse.Explains.Text;

namespace QuickAcid.Tests;

[DocFile]
[DocFileHeader("QuickAcid")]
[DocContent("> Drop it in acid. Look for gold.  \n> Like alchemy, but reproducible.\n")]
public class CreateReadMe
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
        Explain.OnlyThis<CreateReadMe>("README.md");
    }

    [Fact]
    [DocHeader("Example")]
    [DocContent("Given a naive `Account` model:")]
    [DocExample(typeof(Account))]

    [DocContent("You can test the overdraft invariant like this:")]
    [DocExample(typeof(CreateReadMe), nameof(ExampleTest))]

    [DocContent("The generic arguments to the various `Script` methods are just lightweight marker records, used for labeling inputs, actions, and specifications in reports:")]
    [DocCodeFile("CreateReadMeTestMarkers.cs", "csharp")]

    [DocHeader("Example Failure Output")]
    [DocContent("A failing property produces a minimal counterexample and a readable execution trace:")]
    [DocCodeFile("CreateReadMe.txt")]
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

        var reader = LinesReader.FromText(TheClerk.Transcribes(article.CaseFile));
        Assert.Equal(" ──────────────────────────────────────────────────", reader.NextLine());
        Assert.Equal(" Test:                    ExampleTest", reader.NextLine());
        Assert.Equal(" Location:                C:\\Code\\QuickAcid\\QuickAcid.Tests\\CreateReadMe.cs:107:1", reader.NextLine());
        Assert.Equal(" Original failing run:    4 executions", reader.NextLine());
        Assert.Equal(" Minimal failing case:    1 execution (after 4 shrinks)", reader.NextLine());
        Assert.Equal(" Seed:                    1584314623", reader.NextLine());
        Assert.Equal(" ──────────────────────────────────────────────────", reader.NextLine());
        Assert.Equal("   => Account (tracked) : { Balance: 0 }", reader.NextLine());
        Assert.Equal(" ──────────────────────────────────────────────────", reader.NextLine());
        Assert.Equal("  Executed (3): Withdraw", reader.NextLine());
        Assert.Equal("   - Input: Withdraw Amount = 9", reader.NextLine());
        Assert.Equal(" ═══════════════════════════════", reader.NextLine());
        Assert.Equal("  ❌ Spec Failed: No overdraft", reader.NextLine());
        Assert.Equal(" ═══════════════════════════════", reader.NextLine());
        Assert.Equal(" Passed Specs", reader.NextLine());
        Assert.Equal(" - No overdraft: 3x", reader.NextLine());
        Assert.Equal(" ──────────────────────────────────────────────────", reader.NextLine());
        Assert.True(reader.EndOfContent());
        // -- save the file for doc, if things change. Can't use PulseToLog as it quotes the output 
        // QuickPulse.Arteries.TheLedger.Rewrites("QuickAcid.Tests/CreateReadMe.txt")
        //     .Absorb(TheClerk.Transcribes(article.CaseFile));
    }

    [CodeSnippet]
    private static void ExampleTest()
    {
        var script =
            from account in Script.Tracked(() => new Account())
            from _ in Script.Choose(
                from amount in Script.Input<Deposit.Amount>().With(Fuzzr.Int(0, 10))
                from act in Script.Act<Deposit>(() => account.Deposit(amount))
                select Acid.Test,
                from amount in Script.Input<Withdraw.Amount>().With(Fuzzr.Int(0, 10))
                from act in Script.Act<Withdraw>(() => account.Withdraw(amount))
                select Acid.Test)
            from spec in Script.Spec<NoOverdraft>(() => account.Balance >= 0)
            select Acid.Test;
        QState.Run(script, 1584314623) // <= reproducible when seeded
            .WithOneRun()
            .And(50.ExecutionsPerRun());
    }
}