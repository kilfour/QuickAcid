# Chapter 1: Your First Property Test

Welcome to QuickAcid. In this chapter, we'll build and test a very simple system — a `Counter` that can be incremented or decremented — and introduce the core ideas behind property-based testing in QuickAcid.

## The System Under Test

```csharp
public class Counter
{
    private int _value;

    public int Value => _value;

    public void Increment() => _value++;
    public void Decrement()
    {
        // 🛮 Try removing the condition below to introduce a bug:
        // The counter should never go negative.
        if (_value > 0)
            _value--;
    }
}
```

The rule is simple: the counter’s value should **never** be negative. It only decrements when its value is greater than zero.

---

## The Property

In property-based testing, we don’t write individual examples — we describe **invariants** and let the framework discover counterexamples for us.  
Here's what we want to verify:

> The value of the counter should always be greater than or equal to zero.

---

## The QuickAcid Test

```csharp
public class Test
{
    public static class K
    {
        public static QKey<Counter> Counter => QKey<Counter>.New("Counter(a.k.a. the Model)");
    }

    [Fact]
    public void Report()
    {
        var report =
            SystemSpecs.Define()
                .Tracked("Counter(a.k.a. the Model)", () => new Counter())
                .Options(opt => [
                    opt.Do("Increment", c => c.Get(K.Counter).Increment()),
                    opt.Do("Decrement", c => c.Get(K.Counter).Decrement())
                ])
                .Assert("always positive", c => c.Get(K.Counter).Value >= 0)
                .DumpItInAcid()
                .AndCheckForGold(10, 10);

        if (report != null)
            Assert.Fail(report.ToString());
    }
}
```

---

## A Failing Case

If you remove the guard clause in `Decrement()` (i.e., allow negative values), QuickAcid will automatically find the failure.

Here’s what the report looks like:

```
 ----------------------------------------
 -- Property 'always positive' was falsified
 -- Original failing run: 3 execution(s)
 -- Shrunk to minimal case:  1 execution(s) (4 shrinks)
 ----------------------------------------
 RUN START :
   => Counter(a.k.a. the Model) (tracked) : Counter { Value = 0 }
 ---------------------------
 EXECUTE : Decrement

 ********************************
  Spec Failed : always positive
 ********************************


   at QuickAcid.Examples.Tutorial.Chapter1.YourFirstPropertyTest.Test.Report() in C:\Code\QuickAcid\QuickAcid.Examples\Tutorial\Chapter1.YourFirstPropertyTest\Test.cs:line 25
```

QuickAcid tells you:
- What property failed
- What operation caused the failure
- How many steps it took to shrink to a minimal counterexample
- Where in the test code the failure occurred

