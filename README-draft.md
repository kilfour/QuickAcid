# QuickAcid

QuickAcid is a property-based testing library for C#. It focuses on modeling stateful systems, testing long-running interactions, and verifying invariants through declarative, LINQ-based specifications.

---

## When to Use QuickAcid

QuickAcid is well-suited for testing:

- Systems with **mutable state** (e.g., stacks, accounts, agents, simulations)
- Behaviors involving **multi-step operations**
- Code with **side effects**, where you care about what happened along the way
- Situations where you want **shrinkable input**, but also want to **track how the system evolves**

## When Not to Use QuickAcid

Stick to traditional unit tests (`[Fact]`, `[Theory]`, etc.) when:

Use xUnit `[Theory]` when:
- You have **simple parameterized tests** with known inputs
- You want to test combinations of arguments statically (e.g., `[InlineData]`, `[MemberData]`)
- You're writing **table-driven unit tests**, not randomized sequences

Use `[InlineData]` when:
- You want to write **concise examples** with fixed values and expected outputs
- Tests should run deterministically and require no setup

Use AutoFixture when:
- You need to **generate basic test data automatically**, especially for large object graphs
- You prefer convention-over-configuration and don't need control over exact sequences or shrinking

- You're validating **pure input/output pairs**
- The logic is small, local, and easily testable in isolation
- You want fast feedback and minimal test setup

Use FsCheck when:

- You're testing **pure functions**
- You want strong shrinking support
- You're comfortable with F#-style composition or writing generators for custom types

---

## Can I Unit Test with QuickAcid?

Yes, but it's not the main goal. QuickAcid can test single operations, but its strength lies in modeling complex, stateful interactions over time.

---

## Core Concepts

- **TrackedInput**: Register system state that you want to refer to or inspect later.
- **ShrinkableInput**: Random values that can be minimized when a spec fails.
- **Act**: Represents an operation in your system with side effects.
- **Spec**: Declares a property (invariant) that must always hold.
- **Choose**: Randomly selects from a set of test paths (can model non-determinism).
- **Verify**: Runs the test many times with random inputs and action paths.

---

## Input Kinds: When to Use What

| Method              | Use When...                                                | Shrinks? | Tracked? |
|---------------------|------------------------------------------------------------|----------|----------|
| `ShrinkableInput`   | The value *directly affects* the spec outcome             | ✅       | ✅       |
| `Input`             | The value is incidental/contextual (e.g., lookup keys)     | ❌       | ❌       |
| `TrackedInput`      | You want to carry mutable state or inspect later           | ❌       | ✅       |
| `ShrinkableInputIf` | *(Not implemented yet)* Potentially useful for conditional shrinking | ✅ | ✅ |
| `InputIf`           | Conditionally include a non-shrinking input                | ❌       | ❌       |

**Note:** `TrackedInput` was previously called `OnceOnlyInput`. It is intended for stateful values that should not be regenerated or shrunk, and need to be passed to future steps or included in output diagnostics.

---

## Guidelines & Best Practices

---

### Choosing the Right Input Kind

| Method              | Use When...                                             | Shrinks? | Tracked? | Notes                                      |
|---------------------|---------------------------------------------------------|----------|----------|---------------------------------------------|
| `ShrinkableInput`   | Value *affects* whether a spec passes/fails             | ✅       | ✅       | Used for values tested by specs             |
| `Input`             | Value is incidental (e.g., keys, labels, lookups)      | ❌       | ❌       | Not tracked, not shrunk                     |
| `TrackedInput`      | You want to carry or inspect this state across steps   | ❌       | ✅       | Formerly known as `OnceOnlyInput`           |
| `InputIf`           | Conditionally include an input (non-shrinking)         | ❌       | ❌       | Useful for control flow, optional behavior  |
| `ShrinkableInputIf` | Shrinkable input *only* when condition is true         | ✅       | ✅       | (Rare) Avoids misleading shrinking          |

---

- 🔤 **Use descriptive spec keys**: Spec names like "Doors should not open while moving" will show up in reports — write them like assertions.
- 🧱 **Name all LINQ bindings uniquely**: Avoid repeated `from _ in ...`. Use `_s1`, `_a1`, etc. to prevent variable name conflicts.
- 🧪 **Use `.SpecIf(...)` for conditional assertions**: Only enforce invariants when meaningful (e.g. elevator shouldn't move *if* doors are open).
- 🧹 **Break out ops into functions**: Compose `QAcidRunner<Acid>` methods for each operation (e.g., `MoveUp(...)`, `CloseDoors(...)`) to reduce clutter.
- 📋 **Use a `Tracker` to snapshot state**: Capturing pre-op values in a custom tracker makes spec writing much clearer and debuggable.

---

## Roadmap

- More expressive shrinking
- Convention-based spec fallback (when no clue is attached)
- Improved trace reporting
- Code generation and minimal repro rendering (disabled for pre-release)

---

## Examples

All examples below use this elevator simulation as a basis:

```csharp
public class Elevator
{
    public int CurrentFloor = 0;
    public bool DoorsOpen = false;

    public void MoveUp() { if (!DoorsOpen) CurrentFloor++; }
    public void MoveDown() { if (!DoorsOpen && CurrentFloor > 0) CurrentFloor--; }
    public void OpenDoors() => DoorsOpen = true;
    public void CloseDoors() => DoorsOpen = false;
}
```

(Examples coming soon)

---

## Getting Started

Coming soon: NuGet instructions, examples, and API docs.

---

## License

TBD

