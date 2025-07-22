using QuickPulse.Explains;

namespace QuickAcid.TestsDeposition._Tools;

public class CreateDoc
{
	[Fact(Skip = "Not Ready")]
	public void Go()
	{
		new Document().ToFile("reference.md", typeof(CreateDoc).Assembly);
	}
}
