# QuickAcid: Known Pitfalls & Time Sappers

Even with a solid engine, property-based testing can get subtle. Here are known gotchas, edge cases, and traps that have already eaten time (so you don't have to donate yours).

---

## 🔁 Binding Before Bound

### ❌ Problem:
You call a method or construct a value **before** the value it depends on is available.

### Example:
```csharp
from tracker in TrackedInput(() => new Tracker(elevator)) // 💥 elevator is not yet bound
from elevator in TrackedInput(() => new Elevator())
```

### ✅ Fix:
Bind in the correct order:
```csharp
from elevator in TrackedInput(() => new Elevator())
from tracker in TrackedInput(() => new Tracker(elevator)) // ✅ elevator is now in scope
```

---

## ⚠️ Premature Method Calls

### ❌ Problem:
Calling a method like `AllRequestsServed(tracker)` **too early** in a LINQ chain causes evaluation with `null`.

### Example:
```csharp
from tracker in TrackedInput(...)
from _ in AllRequestsServed(tracker) // ❌ tracker is null at plan construction time
```

### ✅ Fix:
Inline the logic or delay evaluation:
```csharp
from tracker in TrackedInput(...)
from _ in "Spec".Spec(() => tracker.Requests.All(...))
```

**Or** (advanced):
```csharp
from tracker in TrackedInput(...)
from _ in Defer(() => AllRequestsServed(tracker))
```

---

## 🧨 Assumptions in Specs

### ❌ Problem:
Specs like:
```csharp
.Spec(() => tracker.Requests.All(...))
```
...assume `tracker.Requests` is never null.

### ✅ Fix:
Snapshot mutable state safely:
```csharp
Requests = elevator.Requests.ToList();
```
and ensure any spec logic is wrapped or isolated.

---

## 🧠 Before/After Timing Confusion

### ❌ Misunderstanding:
Thinking that `.Before(...)` runs before the **previous** step.

### ✅ Reality:
`.Before(...)` runs before the **runner it wraps**. It doesn't inspect prior failure state.

If you want a hook that runs after a step only if it succeeded:
```csharp
.AfterSafe(state => { if (!state.IsFailed) ... })
```

---

## 🦗 Silent Failures

### ❌ Problem:
You forget to log or guard inside specs/trackers. Something fails — but QuickAcid doesn’t catch it because it happened outside an `Act(...)`.

### ✅ Fix:
Add temporary logging:
```csharp
QAcidDebug.WriteLine(...);
```
Or wrap with a `try/catch` to isolate:
```csharp
.Spec(() => { try { ... } catch (Exception ex) { QAcidDebug.WriteLine(...); throw; } })
```

---

## ☠️ The `null` That Wasn’t

### ❌ Misleading Symptom:
You get a `NullReferenceException` seemingly out of nowhere.

### ✅ Cause:
One of your LINQ-bound values was used before it was bound. Especially if it's used in a method call (e.g. `new Tracker(...)`) before its dependencies are available.

---

## 🧪 Quick Test Anti-Patterns

- Don’t write a test that uses `.Before(...)` to check for a prior failure. That’s not what it does.
- Don’t assume all exceptions are caught — only `Act(...)` is guarded internally by default.
- Don’t use values in method calls unless they’re guaranteed to be bound first.

---

## 🧼 Debug Tips

- Use `QAcidDebug.Write(...)` generously while shrinking
- Watch for repeated `Do(...)` calls — they mean a new snapshot is being taken
- Log both before and after `Act(...)` if something smells fishy
- Add a `ToString()` to all tracked values for clearer diagnostics

---

Got a weird one that isn't covered here? Good. Add it. Every trap is a feature waiting to happen.

