using QuickExplainIt;

namespace QuickAcid.TestsDeposition._Tools;

public class CreateDoc
{
	[Fact]
	public void Go()
	{
		new Document().ToFile("README.md", typeof(CreateDoc).Assembly);
	}
}