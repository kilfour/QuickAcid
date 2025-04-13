# Chapter 3: The Shortest Path to a Bug

Most bugs aren't epic failures. They're just... assumptions.

In this chapter, we test a system that looks harmless: a small command parser. It handles common command patterns like `SET x 42`, `GET x`, and `DEL x`. But what if a command arrives malformed?

QuickAcid doesn't just catch the crash — it tells us exactly **how little** it takes to break the system.

---

## The Bug

Here's our command parser:

```csharp
public static class CommandParser
{
    private static readonly Dictionary<string, string> store = new();

    public static void Parse(List<string> tokens)
    {
        if (tokens.Count == 0)
            return;

        var command = tokens[0];

        switch (command)
        {
            case "SET":
                store[tokens[1]] = tokens[2];
                break;
            case "GET":
                var _ = store.TryGetValue(tokens[1], out var _value);
                break;
            case "DEL":
                store.Remove(tokens[1]);
                break;
        }
    }

    public static void Reset() => store.Clear();
    public static string? Get(string key) => store.TryGetValue(key, out var v) ? v : null;
}
```

It looks fine… until `Parse(["SET"])` comes along. Boom — `tokens[2]` is out of bounds.

---

## The Test

Here's the property-based test that uncovers the bug and **shrinks it to its essence**:

```csharp
SystemSpecs.Define()
    .Fuzzed(K.Commands, MGen.ChooseFromThese("SET", "GET", "DEL", "X", "Y", "42").Many(1, 4))
    .Do("reset", _ => CommandParser.Reset())
    .Do("parse", ctx => CommandParser.Parse(ctx.Get(K.Commands)))
    .Assay("should not throw", _ => true)
    .DumpItInAcid()
    .AndCheckForGold(1, 100);
```

---

## The Output

```text
 ----------------------------------------
 -- Exception thrown
 -- Original failing run: 8 execution(s)
 -- Shrunk to minimal case:  1 execution(s) (9 shrinks)
 ----------------------------------------

 RUN START :
 ---------------------------
 EXECUTE : parse
   - Input : Commands = [ SET ]
System.ArgumentOutOfRangeException: Index was out of range. Must be non-negative and less than the size of the collection. (Parameter 'index')
   at CommandParser.Parse(List`1 tokens)
```

### 💥 Boom. Just one token: `SET`.
That's all it takes.

---

## Why Shrinking Matters

A unit test might fail on some 5-token monster like:
```csharp
["SET", "GET", "42", "DEL", "Y"]
```
But QuickAcid shrinks that chaos down to **just enough** to trigger the bug.

That's the shortest path to the bug — and the whole point of property-based testing.

If example-based testing is like throwing darts at the problem…
QuickAcid is a bloodhound.

---