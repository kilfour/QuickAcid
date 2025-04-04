using System.Text;

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

        private static string GetTrackedInput(QAcidState state)
        {
            if (state.FailingSpec != null)
            {
                var name = state.FailingSpec.Replace(" ", "_");
                return $"public void {name}()";
            }
            return "public void Throws()";
        }

        public static string Pan(QAcidState state)
        {
            var sb = new StringBuilder();
            sb.AppendLine("[Fact]");
            sb.AppendLine(GetFunctionDeclaration(state));
            sb.AppendLine("{");
            sb.AppendLine("    var account = new Account();");
            sb.AppendLine("    account.Withdraw(5);");
            sb.AppendLine("    Assert.True(account.Balance >= 0);");
            sb.AppendLine("}");
            return sb.ToString();
        }
    }
}
