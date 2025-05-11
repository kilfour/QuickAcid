## Refactor
```csharp
public abstract record ExecutionOutcome
{
    public static ExecutionOutcome SpecFailed(string specName) => new SpecFailedOutcome(specName);
    public sealed record SpecFailedOutcome(string specName) : ExecutionOutcome;
    public static ExecutionOutcome ExceptionThrown(Exception exception) => new ExceptionThrownOutcome(exception);
    public sealed record ExceptionThrownOutcome(Exception exception) : ExecutionOutcome;

}
```
--- slide ---
## FeedBack Shrinking 
Check always reported input on Start run  
(Only Missing On QDiagniosticState ?)
--- slide ---
## QAcidState.GetPulse
Move To QAcid.GetPulse 
--- slide ---
## Check Project Migration
QuickAcid.Examples.SetTest
--- slide ---
## Code Gen  
lots to do here, getting somewhere
--- slide ---
## check stringifies
--- slide ---
## Bugs  
Multiple .Do's in Fluent => BOOOOM, check Bob
--- slide ---
## Shrinking
 - MaxShrinkTime
 - Reporting
--- slide ---
## Polish
- `Bob.Choose` key/label improvements  
- Unicode, formatting, escaping in reports?
--- slide ---
# Memory PBT
1. Scoped override isolation
Property: overrides only affect the current execution
prop: For any key/value pair and two execution numbers,
only the intended execution sees the override.
2. Reset restores clean state
Property: after ResetRunScopedInputs(), all keys revert or disappear
3. Stashed values persist across runs
Property: stashing a value makes it available after new executions (unless explicitly cleared)
4. Reentrant swap is idempotent
Property: doing ScopedSwap(k,v) twice returns to the original state
5. Codegen reproducibility
Property: replaying the same memory produces the same run and spec outcome
--- slide ---
# visuals in doc
+-------------------------------+
|        QuickAcid Run         |
|  (e.g. Testify(10) executes) |
+-------------------------------+
| +-------------------------+  |
| |     Execution 1         |  |
| |  - Input: 3             |  |
| |  - Result: Pass         |  |
| +-------------------------+  |
| +-------------------------+  |
| |     Execution 2         |  |
| |  - Input: 0             |  |
| |  - Result: Fail         |  |
| +-------------------------+  |
|         ...                  |
+-------------------------------+


Original Failing Run
  [input1 = 5, input2 = 42, input3 = -3]
         ↓
Execution Shrinking (remove steps)
  [input2 = 42]
         ↓
Input Shrinking (simplify values)
  [input2 = 0] ❌ still fails → minimal case found


| Unit Test                   | Property-Based Test (QuickAcid)         |
| --------------------------- | --------------------------------------- |
| Single input, known failure | Random + Shrink to find minimal input   |
| Written for one case        | Covers entire input space               |
| Example-based               | Invariant-based                         |
| High control, low discovery | Lower control, high discovery potential |

FAIL ──▶ Execution Shrinking ──▶ FAIL ──▶ Input Shrinking ──▶ ❌ Minimal Case
--- slide ---
What is QuickAcid?
QuickAcid is a property-based testing framework for C#.
It’s built around a simple idea: instead of writing examples of what your code should do,
you describe what must always be true, and let the framework try to break it.

It combines:

Declarative test construction via LINQ

Powerful fuzzing and shrinking via QuickMGenerate

Test lifecycle control through composable blocks

Human-readable failure reports that shrink failures to their core

You don’t write lots of test cases.
You write invariants — and QuickAcid finds the test cases for you.