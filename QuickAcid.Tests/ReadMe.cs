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
}