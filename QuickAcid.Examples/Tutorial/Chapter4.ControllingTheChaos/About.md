# Chapter 4: Controlling the Chaos

In Chapter 3, QuickAcid showed us how it finds bugs — and shrinks them to their essence. But what happens when you want to be a little more... deliberate? What if you're not just hunting bugs, but shaping the entire space of possible inputs?

This chapter introduces:
- **Guards**: Filters that constrain shrink attempts
- **Touchstones**: Passive observers that help you understand test coverage

These tools help you refine your test space and catch deeper issues — even when everything *seems* fine.

---

## Guards: Keep the Noise Out (During Shrinking)

Imagine you're fuzzing a value like this:

```csharp
.Fuzzed("num", MGen.Int(-10, 10))
```

But your system doesn't support `0`, and throwing on it is expected. You *could* let QuickAcid find it, shrink to it, and report the explosion...

Or you could tell QuickAcid not to shrink to bad input:

```csharp
.Fuzzed("nonZero", MGen.Int(-10, 10), x => x != 0)
```

This does **not** affect the generator itself — only the **shrinking process**. QuickAcid may still generate `0`, but it will not shrink **toward** `0` if it violates the guard. You’re focusing the search and expressing intent.

---

## Touchstones: Did We Even Try?

A passing test might still be bad. What if your guard filters too much? What if only 3 inputs ever made it through?

Touchstones let you see what *actually happened*:

```csharp
.KeepOneEyeOnTheTouchStone()
```

It reports things like:
- How many values passed the guard
- Whether any options were never chosen
- What inputs were filtered out

This is especially useful when things *pass too easily*. A zero-count touchstone might mean:
- A generator mismatch
- A bad guard
- Or just a completely miswired test

You can slide it into any test — even one that's already running well:

```csharp
    .DumpItInAcid()
    .KeepOneEyeOnTheTouchStone()
    .AndCheckForGold(1, 100);
```

For example, here's the input from Chapter 3 that revealed a subtle crash:

```csharp
 ----------------------------------------
 -- FIRST FAILED RUN
 ----------------------------------------
 RUN START :
 ---------------------------
 EXECUTE : parse
   - Input : Commands = [ 42, Y ]
 ---------------------------
 EXECUTE : parse
   - Input : Commands = [ Y, DEL, X ]
 ---------------------------
 EXECUTE : parse
   - Input : Commands = [ X, SET, GET ]
 ---------------------------
 EXECUTE : parse
   - Input : Commands = [ SET, GET ]


 ----------------------------------------
 -- AFTER EXECUTION SHRINKING
 ----------------------------------------
 RUN START :
 ---------------------------
 EXECUTE : parse
   - Input : Commands = [ SET, GET ]


 ----------------------------------------
 -- AFTER INPUT SHRINKING :
 -- Exception thrown
 -- Original failing run: 4 execution(s)
 -- Shrunk to minimal case:  1 execution(s) (5 shrinks)
 ----------------------------------------
 RUN START :
 ---------------------------
 EXECUTE : parse
   - Input : Commands = [ SET ]
```

With the touchstone active, you'd see how often this kind of input appears, and whether the test environment gave it a real chance to shine (or break).
