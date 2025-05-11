using QuickAcid.Bolts;
using QuickAcid.Bolts.TheyCanFade;
using QuickMGenerate;
using QuickPulse;
using QuickPulse.Arteries;

namespace QuickAcid.CodeGen
{
    public static class Prospector
    {
        private static void GetFunctionDeclaration(QAcidState state, Signal<string> signal)
        {
            if (state.CurrentContext.FailingSpec != null)
            {
                var name = state.CurrentContext.FailingSpec.Split(":")[0].Replace(" ", "_");
                signal.Pulse($"public void {name}()");
                return;
            }
            signal.Pulse("public void Throws()");
        }

        private static string Lowered(string a) => char.ToLowerInvariant(a[0]) + a[1..];

        private static void GetTrackedInputsCode(string key, Signal<string> signal)
        {
            if (key.Contains(':'))
            {
                var split = key.Split(':');
                var name = split[0];
                var arg = split[1];
                signal.Pulse($"var {Lowered(name)} = new {name}({arg});");
                return;
            }
            signal.Pulse($"var {Lowered(key)} = new {key}();");
        }

        private static void GetTrackedInputsCodes(QAcidState state, Signal<string> signal)
        {
            state.Memory.GetAllTrackedKeys().ForEach(a => GetTrackedInputsCode(a, signal));
        }

        private static void GetActionCode(string key, Access access, Signal<string> signal)
        {
            if (key.Contains(':'))
            {
                var split = key.Split(':');
                var name = split[0];
                var arg = split[1];
                signal.Pulse($"{name}({access.GetAsString(arg)});");
                return;
            }
            signal.Pulse($"{key}();");
        }

        private static void GetExecutionCode(Access access, Signal<string> signal)
        {
            access.ActionKeys
                .ForEach(a => GetActionCode(a, access, signal));
        }

        private static void GetExecutionsCode(QAcidState state, Signal<string> signal)
        {
            state.ExecutionNumbers
                .Where(state.Memory.Has)
                .ForEach(a => GetExecutionCode(state.Memory.For(a), signal));
        }

        private static void GetAssertionCode(QAcidState state, Signal<string> signal)
        {
            if (state.CurrentContext.FailingSpec != null)
            {
                if (state.CurrentContext.FailingSpec.Contains(':'))
                {
                    signal.Pulse($"Assert.True({state.CurrentContext.FailingSpec.Split(":")[1].Trim()});");
                    return;
                }
                signal.Pulse($"Assert.True({state.CurrentContext.FailingSpec});");
                return;
            }
            GetAssertThrowsCode(signal);
        }

        private static void GetAssertThrowsCode(Signal<string> signal)
        {
            signal.Pulse("Assert.Throws(" + "--------- NOT YET ---------" + ");");
        }

        public static void Scoop(QAcidState state, Signal<string> signal)
        {
            signal.Pulse("[Fact]");
            GetFunctionDeclaration(state, signal);
            signal.Pulse("{");
            using (signal.Scoped<int>(a => ++a, a => --a))
            {
                GetTrackedInputsCodes(state, signal);
                GetExecutionsCode(state, signal);
                GetAssertionCode(state, signal);
            }
            signal.Pulse("}");
        }

        public static string Pan(QAcidState state)
        {
            var flow =
                from line in Pulse.Start<string>()
                from indent in Pulse.Gather(0)
                let str = $"{new string(' ', indent.Value * 4)}{line}"
                from trace in Pulse.Trace(str)
                select line;

            var collector = new TheCollector<string>();
            var signal = Signal.From(flow).SetArtery(collector);
            signal.Pulse("namespace Refined.By.QuickAcid;");
            signal.Pulse("");
            signal.Pulse("public class UnitTests");
            signal.Pulse("{");
            using (signal.Scoped<int>(a => ++a, a => --a))
                Scoop(state, signal);
            signal.Pulse("}");
            return string.Join(Environment.NewLine, collector.TheExhibit);
        }
    }
}
