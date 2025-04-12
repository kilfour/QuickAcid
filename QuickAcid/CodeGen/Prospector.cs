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
                var name = state.FailingSpec.Replace(" ", "_");
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
        private static string GetAlwaysReportedInputCode(Clue clue, Access access)
        {
            var trackedString = $"AlwaysReported: {clue.Key}: DEFAULT";
            return "    " + FollowTheLead(clue, access);
        }

        private static string GetAlwaysReportedInputsCode(XMarksTheSpot xMarksTheSpot, Access access)
        {
            var lines =
                xMarksTheSpot.TheMap
                    .Where(a => a.RunnerType == RunnerType.AlwaysReportedInputRunner)
                    .Select(a => GetAlwaysReportedInputCode(a, access));
            return string.Join(Environment.NewLine, lines);
        }

        private static string GetActionCode(XMarksTheSpot xMarksTheSpot, Access access)
        {
            var clue = xMarksTheSpot.TheMap.SingleOrDefault(a => a.Key == access.ActionKey);
            if (clue == null)
                return $"Action: {access.ActionKey}: DEFAULT";
            return "    " + FollowTheLead(clue, access) + ";";
        }

        private static string GetActionsCode(QAcidState state)
        {
            var sb = new StringBuilder();
            foreach (int actionNumber in state.ExecutionNumbers)
            {
                if (state.Memory.Has(actionNumber))
                    sb.AppendLine(GetActionCode(state.XMarksTheSpot, state.Memory.For(actionNumber)));
            }
            return sb.ToString();
        }

        private static string GetAssertionCode(QAcidState state)
        {
            if (state.FailingSpec != null)
            {
                var clue = state.XMarksTheSpot.TheMap.SingleOrDefault(a => a.Key == state.FailingSpec);
                if (clue == null)
                    return $"Failing Spec: {state.FailingSpec} : DEFAULT";
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

        public static string Pan(QAcidState state)
        {
            var sb = new StringBuilder();
            sb.AppendLine("[Fact]");
            sb.AppendLine(GetFunctionDeclaration(state));
            sb.AppendLine("{");
            sb.AppendLine(GetAlwaysReportedInputsCode(state.XMarksTheSpot, state.Memory.For(0)));
            sb.Append(GetActionsCode(state));
            sb.AppendLine(GetAssertionCode(state));
            sb.AppendLine("}");
            return sb.ToString();
        }
    }
}
