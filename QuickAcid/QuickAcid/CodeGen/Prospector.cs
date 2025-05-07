using System.Text;
using QuickAcid.Bolts;
using QuickAcid.Bolts.TheyCanFade;
using QuickMGenerate;

namespace QuickAcid.CodeGen
{
    public static class Prospector
    {
        private static string GetFunctionDeclaration(QAcidState state)
        {
            if (state.FailingSpec != null)
            {
                var name = state.FailingSpec.Split(":")[0].Replace(" ", "_");
                return $"public void {name}()";
            }
            return "public void Throws()";
        }

        private static string Lowered(string a) => char.ToLowerInvariant(a[0]) + a[1..];

        private static string GetTrackedInputsCode(string key)
        {
            return $"    var {Lowered(key)} = new {key}();";
        }

        private static string? GetTrackedInputsCodes(QAcidState state)
        {
            var lines =
                state.Memory.GetAllTrackedKeys()
                .Select(GetTrackedInputsCode);

            return string.Join(Environment.NewLine, lines);
        }

        private static string GetActionCode(string key, Access access)
        {
            if (key.Contains(':'))
            {
                var split = key.Split(':');
                var name = split[0];
                var arg = split[1];
                return $"    {name}({access.GetAsString(arg)});";
            }
            return $"    {key}();";
        }

        private static string GetExecutionCode(Access access)
        {
            return string.Join(Environment.NewLine,
                access.ActionKeys
                    .Select(a => GetActionCode(a, access)));
        }

        private static string GetExecutionsCode(QAcidState state)
        {
            var sb = new StringBuilder();
            foreach (int actionNumber in state.ExecutionNumbers)
            {
                if (state.Memory.Has(actionNumber))
                    sb.AppendLine(GetExecutionCode(state.Memory.For(actionNumber)));
            }
            return sb.ToString();
        }

        private static string GetAssertionCode(QAcidState state)
        {
            if (state.FailingSpec != null)
            {
                if (state.FailingSpec.Contains(':'))
                {
                    return $"    Assert.True({state.FailingSpec.Split(":")[1].Trim()});";
                }
                return $"    Assert.True({state.FailingSpec});";
            }
            return GetAssertThrowsCode();
        }

        private static string GetAssertThrowsCode()
        {
            return "    Assert.Throws(" + "--------- NOT YET ---------" + ");";
        }

        public static string Scoop(QAcidState state)
        {
            var sb = new StringBuilder();
            sb.AppendLine("[Fact]");
            sb.AppendLine(GetFunctionDeclaration(state));
            sb.AppendLine("{");
            sb.AppendLine(GetTrackedInputsCodes(state));
            sb.Append(GetExecutionsCode(state));
            sb.AppendLine(GetAssertionCode(state));
            sb.AppendLine("}");
            return sb.ToString();
        }

        public static string Pan(QAcidState state)
        {
            var sb = new StringBuilder();
            sb.AppendLine("namespace Refined.By.QuickAcid;");
            sb.AppendLine("");
            sb.AppendLine("public class UnitTests");
            sb.AppendLine("{");
            sb.AppendLine(Scoop(state));
            sb.AppendLine("}");
            return sb.ToString();
        }


    }
}
