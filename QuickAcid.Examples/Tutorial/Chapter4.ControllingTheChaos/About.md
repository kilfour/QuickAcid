# Chapter 4: Controlling the Chaos

In Chapter 3, QuickAcid showed us how it finds bugs — and shrinks them to their essence. But what happens when you want to be a little more... deliberate? What if you're not just hunting bugs, but shaping the entire space of possible inputs?

This chapter introduces:
- **Guards**: Filters that constrain shrink attempts
- **Touchstones**: Passive observers that help you understand test coverage

These tools help you refine your test space and catch deeper issues — even when everything *seems* fine.

---

## Guards: Keep the Noise Out — or Keep It Out Entirely

Imagine you're fuzzing a value like this:

```csharp
.Fuzzed("num", MGen.Int(-10, 10))
```

But your system doesn’t support `0`, and it throws when it encounters one. You have a few ways to control this:

---

### 🧪 Guard **only the shrinking**

Let QuickAcid generate everything (including bad values),  
but tell it **not to shrink toward them**:

```csharp
.Fuzzed("nonZero", MGen.Int(-10, 10), x => x != 0)
```

This does **not** affect generation — `0` might still be picked —  
but once a failure is found, QuickAcid will never shrink **toward** `0` if the guard rejects it.

✅ Great when:
- You want to find edge cases like `0`
- But shrinking into them would misrepresent the minimal bug

---

### 🎲 Filter **only the generation**

Use `.Where(...)` to completely exclude values from being generated:

```csharp
.Fuzzed("nonZero", MGen.Int(-10, 10).Where(x => x != 0))
```

QuickAcid will never *generate* `0`.  
However, if `0` *was* generated earlier and caused a failure, QuickAcid might still shrink *toward* it — unless a separate guard is added.

✅ Great when:
- You want to prevent bad values *from entering the system at all*
- But you’re okay with shrinkers potentially exploring toward them unless further constrained

---

### 🛡️ Use a **contract guard** — affect both

Declare your intention **once**, and apply it to both generation *and* shrinking:

```csharp
.Fuzzed("nonZero", MGen.Int(-10, 10).Claim(x => x != 0))
```

This:
- Filters generation (like `.Where(...)`)
- Prevents shrinking toward bad values (like the third `Fuzzed` parameter)
- Reads clearly as a **property-level constraint**

✅ Best when:
- Your test input space is clearly defined
- You want generation and shrinking to respect the same rules


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
