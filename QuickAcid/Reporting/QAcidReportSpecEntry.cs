using System.Text;

namespace QuickAcid.Reporting;

public class QAcidReportSpecEntry : QAcidReportEntry
{
    public QAcidReportSpecEntry(string key)
        : base(key) { }

    public override string ToString()
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine();
        stringBuilder.AppendLine(" ********************************");
        stringBuilder.AppendLine($"  Spec Failed : {Key}");
        stringBuilder.AppendLine(" ********************************");
        return stringBuilder.ToString();
    }
}


public class QAcidReportTitleSectionEntry : IAmAReportEntry
{
    private string title = "";
    public QAcidReportTitleSectionEntry(string title) { this.title = title; }


    public override string ToString()
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine("// ----------------------------------------");
        stringBuilder.AppendLine($"// -- : {title}");
        stringBuilder.AppendLine("// --");
        stringBuilder.AppendLine("// ----------------------------------------");
        return stringBuilder.ToString();
    }
}