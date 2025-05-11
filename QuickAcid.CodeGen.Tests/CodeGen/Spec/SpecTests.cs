using QuickAcid.Bolts;
using QuickAcid.Bolts.Nuts;
using QuickAcid.TestsDeposition._Tools;

namespace QuickAcid.TestsDeposition.Linqy.CodeGen.Spec;

public class SpecTests
{
    [Fact]
    public void CodeGen_spec()
    {
        var script = from _ in "TheSpec".Spec(() => false) select Acid.Test;
        var reader = Reader.FromRun(script);
        Assert.Equal("Assert.True(TheSpec);", reader.NextLine());
    }

    [Fact]
    public void CodeGen_spec_colon_spererator()
    {
        var script = from _ in "TheSpec:false == true".Spec(() => false) select Acid.Test;
        var reader = Reader.FromRun(script);
        Assert.Equal("Assert.True(false == true);", reader.NextLine());
    }

    [Fact]
    public void CodeGen_spec_function_name()
    {
        var script = from _ in "TheSpec".Spec(() => false) select Acid.Test;
        var code = new QCodeState(script).GenerateCode();
        var reader = LinesReader.FromText(code);
        reader.Skip(5);
        Assert.Equal("public void TheSpec()", reader.NextLine().Trim());
    }

    [Fact]
    public void CodeGen_spec_function_name_colon_seperator()
    {
        var script = from _ in "TheSpec: ignore me".Spec(() => false) select Acid.Test;
        var code = new QCodeState(script).GenerateCode();
        var reader = LinesReader.FromText(code);
        reader.Skip(5);
        Assert.Equal("public void TheSpec()", reader.NextLine().Trim());
    }
}