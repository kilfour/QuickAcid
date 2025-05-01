# When *Not* to Use QuickAcid  
_Or: "This is a chainsaw, not a scalpel."_

---

## You're Testing Pure Functions

**Example:**
```csharp
int Add(int x, int y) => x + y;
```

Use **FsCheck** instead:
```csharp
[Property]
public void Add_IsCommutative(int x, int y) => Add(x, y) == Add(y, x);
```

**Why?**  
QuickAcid’s `Act`/`Stashed` system is overkill for simple math functions.

---

## You Need Statistical Metrics

**Example:**  
"Verify 90% of inputs satisfy X"

QuickAcid will **just show you failing cases**, not summary stats.

---

## You're Generating Giant Data Structures

**Problem:**  
`MGen.One<T>` can be slow for:
- 10,000-node graphs  
- Deeply nested JSON  
- Large binary trees  

**Workaround:**  
Build manual generators using:
```csharp
MGen.For<T>().Customize()
```

---

## You Hate Opinionated Tools

If you:
- Roll your eyes at `TheWohlwillProcess`  
- Prefer _"just the facts"_ naming  
- Want total control over behavior  

...then **FsCheck’s clinical approach** may suit you better.

---

## You're Testing Performance

QuickAcid is great for:
- Finding logic bugs  
- Detecting race conditions  

Use **Benchmark.NET** for:
- Memory leaks  
- CPU bottlenecks  
- GC pressure  

---

## You Want FP Purity

QuickAcid embraces:
- Mutable state when needed  
- Imperative-style testing  
- C# idioms over FP dogma  

If you want **Haskell-style property-based testing**, use FsCheck.

---

## Footnote

> “QuickAcid isn’t a FsCheck replacement – it’s for **stateful bugs** that FsCheck struggles with.”

---



