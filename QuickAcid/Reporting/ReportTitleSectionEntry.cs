using System.Text;

namespace QuickAcid.Reporting;


// private const string TopLeft = "┌";
// private const string TopRight = "┐";
// private const string BottomLeft = "└";
// private const string BottomRight = "┘";
// private const string Horizontal = "─";
// private const string Vertical = "│";
// ╔ ╗ ╚ ╝ ═ ║
// ╭ ╮ ╰ ╯ ── │ 
public class ReportTitleSectionEntry : IAmAReportEntry
{
    public List<string> Title;
    public ReportTitleSectionEntry(List<string> title) { Title = title; }

    public override string ToString()
    {
        var stringBuilder = new StringBuilder();
        var line = " " + new string('─', 50);
        stringBuilder.Append(line);
        foreach (var str in Title)
        {
            stringBuilder.AppendLine();
            stringBuilder.Append($" {str}");
        }
        return stringBuilder.ToString();
    }
}
