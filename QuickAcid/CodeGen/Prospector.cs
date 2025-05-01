using System.Text;
using QuickAcid.Bolts;
using QuickAcid.Bolts.TheyCanFade;
using QuickMGenerate;

namespace QuickAcid.CodeGen
{
    public static class Prospector
    {
        private static string GetFunctionDeclaration(QState state)
        {
            if (state.FailingSpec != null)
            {
                var name = state.FailingSpec.Split(":")[0].Replace(" ", "_");
                return $"public void {name}()";
            }
            return "public void Throws()";
        }

        private static string FollowTheLead(Clue clue, Access access)
        {
            if (clue.SeeWhereItLeads == null)
                return "DEFAULT";
            return clue.SeeWhereItLeads.Invoke(clue.Key, access);
        }

        private static string Lowered(string a) => char.ToLowerInvariant(a[0]) + a[1..];

        private static string GetAlwaysReportedInputsCode(string key, XMarksTheSpot xMarksTheSpot, Access access)
        {
            var clue = xMarksTheSpot.TheMap.SingleOrDefault(a => a.Key == key);
            if (clue == null)
                return $"    var {Lowered(key)} = new {key}();";
            return "    " + FollowTheLead(clue, access);
        }

        private static string? GetAlwaysReportedInputsCodes(QState state)
        {
            var lines =
                state.Memory.GetAllAlwaysReportedKeys()
                .Select(a => GetAlwaysReportedInputsCode(a, state.XMarksTheSpot, state.Memory.For(0)));

            return string.Join(Environment.NewLine, lines);
        }

        private static string GetActionCode(XMarksTheSpot xMarksTheSpot, string key, Access access)
        {
            var clue = xMarksTheSpot.TheMap.SingleOrDefault(a => a.Key == key);
            if (clue == null)
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

            return "    " + FollowTheLead(clue, access) + ";";
        }

        private static string GetExecutionCode(XMarksTheSpot xMarksTheSpot, Access access)
        {
            return string.Join(Environment.NewLine,
                access.ActionKeys
                    .Select(a => GetActionCode(xMarksTheSpot, a, access)));
        }

        private static string GetExecutionsCode(QState state)
        {
            var sb = new StringBuilder();
            foreach (int actionNumber in state.ExecutionNumbers)
            {
                if (state.Memory.Has(actionNumber))
                    sb.AppendLine(GetExecutionCode(state.XMarksTheSpot, state.Memory.For(actionNumber)));
            }
            return sb.ToString();
        }

        private static string GetAssertionCode(QState state)
        {
            if (state.FailingSpec != null)
            {
                var clue = state.XMarksTheSpot.TheMap.SingleOrDefault(a => a.Key == state.FailingSpec);
                if (clue == null)
                {
                    if (state.FailingSpec.Contains(':'))
                    {
                        return $"    Assert.True({state.FailingSpec.Split(":")[1].Trim()});";
                    }
                    return $"    Assert.True({state.FailingSpec});";
                }

                return GetAssertTrueCode(
                    state.XMarksTheSpot.TheMap.Single(a => a.Key == state.FailingSpec),
                    state.Memory.ForLastExecution());
            }
            return GetAssertThrowsCode();
        }

        private static string GetAssertTrueCode(Clue clue, Access access)
        {
            return "    Assert.True(" + FollowTheLead(clue, access) + ");";
        }

        private static string GetAssertThrowsCode()
        {
            return "    Assert.Throws(" + "--------- NOT YET ---------" + ");";
        }

        public static string Pan(QState state)
        {
            var sb = new StringBuilder();
            sb.AppendLine("[Fact]");
            sb.AppendLine(GetFunctionDeclaration(state));
            sb.AppendLine("{");
            sb.AppendLine(GetAlwaysReportedInputsCodes(state));
            sb.Append(GetExecutionsCode(state));
            sb.AppendLine(GetAssertionCode(state));
            sb.AppendLine("}");
            return sb.ToString();
        }
    }
}
