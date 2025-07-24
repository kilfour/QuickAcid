using QuickAcid.Proceedings;

namespace QuickAcid.Tests.Proceedings.ExtraDepositions;

public class ExtraDepositionsTests : DepositionTest
{
    [Fact]
    public void One()
    {
        var caseFile = CaseFile.WithVerdict(Verdict.FromDossier(Dossier))
                .AddExtraDeposition(new ExtraDeposition("Some Title")
                    .AddMessage("Some Message"));
        var reader = Transcribe(caseFile);
        reader.Skip(3); // ignore spec, coz these come after 
        Assert.Equal(" ──────────────────────────────────────────────────", reader.NextLine());
        Assert.Equal(" Some Title", reader.NextLine());
        Assert.Equal("   Some Message", reader.NextLine());
        EndOfContent(reader);
    }

    [Fact]
    public void Two()
    {
        var caseFile = CaseFile.WithVerdict(Verdict.FromDossier(Dossier))
                .AddExtraDeposition(new ExtraDeposition("Some Title")
                    .AddMessage("Some Message"))
                .AddExtraDeposition(new ExtraDeposition("Some Other Title")
                    .AddMessage("Some Other Message")
                    .AddMessage("Yet Another Message"));
        var reader = Transcribe(caseFile);
        reader.Skip(3); // ignore spec, coz these come after 
        Assert.Equal(" ──────────────────────────────────────────────────", reader.NextLine());
        Assert.Equal(" Some Title", reader.NextLine());
        Assert.Equal("   Some Message", reader.NextLine());
        Assert.Equal(" ──────────────────────────────────────────────────", reader.NextLine());
        Assert.Equal(" Some Other Title", reader.NextLine());
        Assert.Equal("   Some Other Message", reader.NextLine());
        Assert.Equal("   Yet Another Message", reader.NextLine());
        EndOfContent(reader);
    }
}