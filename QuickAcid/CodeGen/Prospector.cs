using System.Text;
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

        private static string FollowTheLead(Clue clue, Memory.Access access)
        {
            return clue.SeeWhereItLeads.Invoke(clue.Key, access);
        }
        private static string GetTrackedInputCode(Clue clue, Memory.Access access)
        {
            return "    " + FollowTheLead(clue, access);
        }

        private static string GetTrackedInputsCode(XMarksTheSpot xMarksTheSpot, Memory.Access access)
        {
            var lines =
                xMarksTheSpot.TheMap
                    .Where(a => a.RunnerType == RunnerType.TrackedInputRunner)
                    .Select(a => GetTrackedInputCode(a, access));
            return string.Join(Environment.NewLine, lines);
        }

        private static string GetActionCode(XMarksTheSpot xMarksTheSpot, Memory.Access access)
        {
            var clue = xMarksTheSpot.TheMap.Single(a => a.Key == access.ActionKey);

            return "    " + FollowTheLead(clue, access) + ";";
        }

        private static string GetActionsCode(QAcidState state)
        {
            var sb = new StringBuilder();
            foreach (int actionNumber in state.ActionNumbers)
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
                return GetAssertTrueCode(
                    state.XMarksTheSpot.TheMap.Single(a => a.Key == state.FailingSpec),
                    state.Memory.ForLastAction());
            }
            return GetAssertThrowsCode();
        }

        private static string GetAssertTrueCode(Clue clue, Memory.Access access)
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
            sb.AppendLine(GetTrackedInputsCode(state.XMarksTheSpot, state.Memory.For(0)));
            sb.Append(GetActionsCode(state));
            sb.AppendLine(GetAssertionCode(state));
            sb.AppendLine("}");
            return sb.ToString();
        }
    }
}
