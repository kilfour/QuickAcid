using QuickAcid.Bolts;
using QuickAcid.Bolts.Nuts;
using QuickAcid.TestsDeposition._Tools;

namespace QuickAcid.TestsDeposition.Linqy.CodeGen.Spec;

public class SpecTests
{
    [Fact]
    public void CodeGen_spec_default()
    {
        var run = from _ in "TheSpec".Spec(() => false) select Acid.Test;
        var reader = Reader.FromRun(run);
        Assert.Equal("Assert.True(TheSpec);", reader.NextLine());
    }

    [Fact]
    public void CodeGen_spec_colon_spererator()
    {
        var run = from _ in "TheSpec:false == true".Spec(() => false) select Acid.Test;
        var reader = Reader.FromRun(run);
        Assert.Equal("Assert.True(false == true);", reader.NextLine());
    }

    [Fact]
    public void CodeGen_spec_default_function_name()
    {
        var run = from _ in "TheSpec".Spec(() => false) select Acid.Test;
        var code = new QState(run).GenerateCode().ObserveOnce().Code;
        var reader = LinesReader.FromText(code);
        reader.Skip(5);
        Assert.Equal("public void TheSpec()", reader.NextLine().Trim());
    }

    [Fact]
    public void CodeGen_spec_default_function_name_colon_seperator()
    {
        var run = from _ in "TheSpec: ignore me".Spec(() => false) select Acid.Test;
        var code = new QState(run).GenerateCode().ObserveOnce().Code;
        var reader = LinesReader.FromText(code);
        reader.Skip(5);
        Assert.Equal("public void TheSpec()", reader.NextLine().Trim());
    }
}