using QuickAcid.Reporting;
using QuickMGenerate;
using QuickMGenerate.UnderTheHood;

namespace QuickAcid.Tests.Strike;

public static class Perform
{
    public static (string name, Delegate action) Action<TModel, TInput>(string name, Action<TModel, TInput> action)
        => (name, action);
}

public class Spike
{
    public class Account
    {
        public int Balance = 0;
        public void Deposit(int amount) { Balance += amount; }
        public void Withdraw(int amount) { Balance -= amount; }
    }

    [Fact(Skip = "demo")]
    public void InitialIdea()
    {
        var report =
        Test.This(() => new Account()) // todo add optional stringify 
            .Arrange(
                ("deposit", MGen.Int(0, 100).AsObject()),
                ("withdraw", MGen.Int(0, 100).AsObject()))
            .Act(
                Perform.Action("deposit", (Account account, int deposit) => account.Deposit(deposit)),
                Perform.Action("withdraw", (Account account, int withdraw) => account.Withdraw(withdraw)))
            .Assert("No Overdraft", account => account.Balance >= 0) // todo use the label 
            .Assert("Balance Capped", account => account.Balance <= 100)
            .Run(1, 10);
        if (report != null)
            Assert.Fail(report.ToString());
    }

    [Fact(Skip = "demo")]
    public void Unit()
    {
        var report =
        Test.This(() => new Account())
            .Arrange(("deposit", MGen.Constant(42).AsObject()))
            .Act(Perform.Action("deposit", (Account account, int withdraw) => account.Withdraw(withdraw)))
            .Assert("No Overdraft", account => account.Balance >= 0)
            .Run(1, 1);
        if (report != null)
            Assert.Fail(report.ToString());
    }
}

// public TestActBuilder<TModel> Arrange(params (string name, object generator)[] fuzzers)
// {
//     var mapped = fuzzers.Select(f =>
//     {
//         if (f.generator is Generator<object> gobj)
//             return (f.name, gobj);

//         if (f.generator is Delegate del)
//         {
//             var genType = del.GetType();
//             if (genType.IsGenericType && genType.GetGenericTypeDefinition() == typeof(Generator<>))
//             {
//                 // Convert Generator<T> to Generator<object> dynamically
//                 var param = Expression.Parameter(typeof(State), "state");
//                 var invoke = Expression.Invoke(Expression.Constant(del), param);
//                 var body = Expression.Call(
//                     typeof(TestForgeHelpers),
//                     nameof(TestForgeHelpers.CastResultToObject),
//                     new[] { genType.GenericTypeArguments[0] },
//                     invoke);

//                 var lambda = Expression.Lambda<Generator<object>>(body, param);
//                 var lifted = lambda.Compile();
//                 return (f.name, lifted);
//             }
//         }

//         throw new InvalidOperationException($"Invalid generator for {f.name}");
//     }).ToArray();

//     return new TestActBuilder<TModel>(modelFactory, mapped);
// }

// public static class TestForgeHelpers
// {
//     public static IResult<object> CastResultToObject<T>(IResult<T> result)
//     {
//         if (!result.Success)
//             return Result.Fail<object>(result.Remainder, result.ErrorMessage ?? "Unknown");

//         return Result.Success<object>(result.Remainder, (object)result.Value);
//     }
// }


public static class Test
{
    public static StrikeModelBuilder<TModel> This<TModel>(Func<TModel> modelFactory)
    {
        return new StrikeModelBuilder<TModel>(modelFactory);
    }


}

public class StrikeModelBuilder<TModel>
{
    private readonly Func<TModel> modelFactory;

    public StrikeModelBuilder(Func<TModel> modelFactory)
    {
        this.modelFactory = modelFactory;
    }

    public StrikeFuzzBuilder<TModel> Arrange(params (string name, Generator<object> generator)[] fuzzers)
    {
        return new StrikeFuzzBuilder<TModel>(modelFactory, fuzzers);
    }
}

public class StrikeFuzzBuilder<TModel>
{
    private readonly Func<TModel> modelFactory;
    private readonly (string name, Generator<object> generator)[] fuzzers;

    public StrikeFuzzBuilder(Func<TModel> modelFactory, (string name, Generator<object> generator)[] fuzzers)
    {
        this.modelFactory = modelFactory;
        this.fuzzers = fuzzers;
    }

    public StrikeDoBuilder<TModel> Act(params (string name, Delegate action)[] operations)
    {
        return new StrikeDoBuilder<TModel>(modelFactory, fuzzers, operations);
    }
}
public class StrikeDoBuilder<TModel>
{
    private readonly Func<TModel> modelFactory;
    private readonly (string name, Generator<object> generator)[] fuzzers;
    private readonly (string name, Delegate action)[] operations;
    private readonly List<Func<TModel, bool>> checks = new();

    public StrikeDoBuilder(
        Func<TModel> modelFactory,
        (string name, Generator<object> generator)[] fuzzers,
        (string name, Delegate action)[] operations)
    {
        this.modelFactory = modelFactory;
        this.fuzzers = fuzzers;
        this.operations = operations;
    }

    public StrikeDoBuilder<TModel> Assert(string label, Func<TModel, bool> invariant)
    {
        checks.Add(invariant);
        return this;
    }

    public QAcidReport Run(int scopes, int executionsPerScope)
    {
        var spec = SystemSpecs.Define()
            .AlwaysReported("Model", modelFactory);

        foreach (var (name, generator) in fuzzers)
        {
            spec = spec.Fuzzed(name, generator);
        }

        spec = spec.Options(opt =>
            operations.Select(op =>
                opt.Do(op.name, ctx =>
                {
                    var model = ctx.GetItAtYourOwnRisk<TModel>("Model");
                    var input = ctx.GetItAtYourOwnRisk<object>(op.name);

                    // dynamic call — fast and dirty for now
                    op.action.DynamicInvoke(model, input);
                })
            ));

        foreach (var check in checks)
        {
            spec = spec.Assert("Spec", ctx =>
            {
                var model = ctx.GetItAtYourOwnRisk<TModel>("Model");
                return check(model);
            });
        }

        return spec
            .DumpItInAcid()
            .AndCheckForGold(scopes, executionsPerScope);
    }
}
