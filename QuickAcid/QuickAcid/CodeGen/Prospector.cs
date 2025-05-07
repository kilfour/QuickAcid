using QuickAcid.Bolts;
using QuickAcid.Bolts.TheyCanFade;
using QuickMGenerate;
using QuickPulse;
using QuickPulse.Diagnostics;

namespace QuickAcid.CodeGen
{
    public static class Prospector
    {
        private static void GetFunctionDeclaration(QAcidState state, IndentedPulser indentedPulser)
        {
            if (state.FailingSpec != null)
            {
                var name = state.FailingSpec.Split(":")[0].Replace(" ", "_");
                indentedPulser.Monitor($"public void {name}()");
                return;
            }
            indentedPulser.Monitor("public void Throws()");
        }

        private static string Lowered(string a) => char.ToLowerInvariant(a[0]) + a[1..];

        private static void GetTrackedInputsCode(string key, IndentedPulser indentedPulser)
        {
            if (key.Contains(':'))
            {
                var split = key.Split(':');
                var name = split[0];
                var arg = split[1];
                indentedPulser.Monitor($"var {Lowered(name)} = new {name}({arg});");
                return;
            }
            indentedPulser.Monitor($"var {Lowered(key)} = new {key}();");
        }

        private static void GetTrackedInputsCodes(QAcidState state, IndentedPulser indentedPulser)
        {
            state.Memory.GetAllTrackedKeys().ForEach(a => GetTrackedInputsCode(a, indentedPulser));
        }

        private static void GetActionCode(string key, Access access, IndentedPulser indentedPulser)
        {
            if (key.Contains(':'))
            {
                var split = key.Split(':');
                var name = split[0];
                var arg = split[1];
                indentedPulser.Monitor($"{name}({access.GetAsString(arg)});");
                return;
            }
            indentedPulser.Monitor($"{key}();");
        }

        private static void GetExecutionCode(Access access, IndentedPulser indentedPulser)
        {
            access.ActionKeys
                .ForEach(a => GetActionCode(a, access, indentedPulser));
        }

        private static void GetExecutionsCode(QAcidState state, IndentedPulser indentedPulser)
        {
            state.ExecutionNumbers
                .Where(state.Memory.Has)
                .ForEach(a => GetExecutionCode(state.Memory.For(a), indentedPulser));
        }

        private static void GetAssertionCode(QAcidState state, IndentedPulser indentedPulser)
        {
            if (state.FailingSpec != null)
            {
                if (state.FailingSpec.Contains(':'))
                {
                    indentedPulser.Monitor($"Assert.True({state.FailingSpec.Split(":")[1].Trim()});");
                    return;
                }
                indentedPulser.Monitor($"Assert.True({state.FailingSpec});");
                return;
            }
            GetAssertThrowsCode(indentedPulser);
        }

        private static void GetAssertThrowsCode(IndentedPulser indentedPulser)
        {
            indentedPulser.Monitor("Assert.Throws(" + "--------- NOT YET ---------" + ");");
        }

        public static void Scoop(QAcidState state, IndentedPulser pulser)
        {
            pulser.Monitor("[Fact]");
            GetFunctionDeclaration(state, pulser);
            pulser.Monitor("{");
            GetTrackedInputsCodes(state, pulser.Next());
            GetExecutionsCode(state, pulser.Next());
            GetAssertionCode(state, pulser.Next());
            pulser.Monitor("}");
        }



        public static string Pan(QAcidState state)
        {
            List<string> collector = [];
            IPulse pulse = (
                from line in Pulse.From<LineOfCode>()
                from str in Pulse.Shape(() => $"{new string(' ', line.Indent * 4)}{line.Code}")
                from _ in Sink.To(() => collector.Add(str))
                select str).ToPulse();
            var pulser = new IndentedPulser(pulse, 0);
            pulser.Monitor("namespace Refined.By.QuickAcid;");
            pulser.Monitor("");
            pulser.Monitor("public class UnitTests");
            pulser.Monitor("{");
            Scoop(state, pulser.Next());
            pulser.Monitor("}");
            return string.Join(Environment.NewLine, collector);
        }
    }
    public record LineOfCode(int Indent, string Code);
    public record IndentedPulser(IPulse Pulse, int Indent)
    {
        public void Monitor(string code) => Pulse.Monitor(new LineOfCode(Indent, code));
        public IndentedPulser Next() => new(Pulse, Indent + 1);
        public IndentedPulser Prev() => new(Pulse, Math.Max(0, Indent - 1));
    }
}
