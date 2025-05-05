
using System.Linq.Expressions;
using QuickMGenerate;
using QuickMGenerate.UnderTheHood;

namespace QuickAcid.TheFortyNiners;

public class StrikeArrangeBuilder<TModel>
{
    private readonly Func<TModel> modelFactory;
    private readonly Func<TModel, string> stringify;

    public StrikeArrangeBuilder(Func<TModel> modelFactory, Func<TModel, string> stringify)
    {
        this.modelFactory = modelFactory;
        this.stringify = stringify;
    }

    public StrikeActBuilder<TModel> Arrange(params (string name, object generator)[] fuzzers)
    {
        var mapped = fuzzers.Select(f =>
        {
            if (f.generator is Generator<object> gobj)
                return (f.name, gobj);

            if (f.generator is Delegate del)
            {
                var genType = del.GetType();
                if (genType.IsGenericType && genType.GetGenericTypeDefinition() == typeof(Generator<>))
                {
                    // Convert Generator<T> to Generator<object> dynamically
                    var param = Expression.Parameter(typeof(State), "state");
                    var invoke = Expression.Invoke(Expression.Constant(del), param);
                    var body = Expression.Call(
                        typeof(TestForgeHelpers),
                        nameof(TestForgeHelpers.CastResultToObject),
                        new[] { genType.GenericTypeArguments[0] },
                        invoke);

                    var lambda = Expression.Lambda<Generator<object>>(body, param);
                    var lifted = lambda.Compile();
                    return (f.name, lifted);
                }
            }
            return (f.name, MGen.Constant(f.generator).AsObject());
            //throw new InvalidOperationException($"Invalid generator for {f.name}");
        }).ToArray();

        return new StrikeActBuilder<TModel>(modelFactory, stringify, mapped);
    }

    // public StrikeActBuilder<TModel> Arrange(params (string name, Generator<object> generator)[] fuzzers)
    // {
    //     return new StrikeActBuilder<TModel>(modelFactory, stringify, fuzzers);
    // }
}

public static class TestForgeHelpers
{
    public static IResult<object> CastResultToObject<T>(IResult<T> result)
    {
        return new Result<object>((object)result.Value!, ((Result<T>)result).State);
    }
}