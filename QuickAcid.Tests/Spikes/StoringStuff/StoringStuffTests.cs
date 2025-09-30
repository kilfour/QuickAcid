using System.Data.SqlTypes;
using QuickAcid.Tests._Tools.ThePress;


namespace QuickAcid.Tests.Spikes.StoringStuff;

public class StoringStuffTests
{
    public enum State { One, Two }

    public class Thing
    {
        public State State { get; private set; } = State.One;

        public void NextState()
        {
            if (State != State.One)
                throw new InvalidOperationException();
            State = State.Two;
        }

        public void DoSomething()
        {
            if (State != State.Two)
                throw new InvalidOperationException();
        }
    }
    public class Storage()
    {
        private Dictionary<Type, List<object>> warehouse = new();

    }
    [Fact]
    public void First_try()
    {

        var script =
            //from _ in Script.Choose()
            from thing in "Thing".Stashed(() => new Thing())
            from two in "Two".ActIf(
                () => thing.State == State.One,
                () => thing.NextState())
            from doSomething in "DoSomething".ActIf(
                () => thing.State == State.Two,
                () => thing.DoSomething())
            select Acid.Test;

        var article = TheJournalist.Unearths(
            QState.Run(script)
            .WithOneRun()
            .And(2.ExecutionsPerRun()));

        // Assert.Equal(2, article.DiagnosisExecutionsCount);

        // Assert.Equal(1, article.DiagnoseExecutions(1).DiagnosisCount);
        // Assert.Equal(1, article.DiagnoseExecutions(1).Diagnosis(1).TraceCount);
        // Assert.Equal("Extra words", article.DiagnoseExecutions(1).Diagnosis(1).Read(1));

        // Assert.Equal(1, article.DiagnoseExecutions(2).DiagnosisCount);
        // Assert.Equal(2, article.DiagnoseExecutions(2).Diagnosis(1).TraceCount);
        // Assert.Equal("Extra words", article.DiagnoseExecutions(2).Diagnosis(1).Read(1));
        // Assert.Equal("More Extra words", article.DiagnoseExecutions(2).Diagnosis(1).Read(2));
    }
}

