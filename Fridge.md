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
## AutoDoc  
in progress
Don't forget `.Sequence()`
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
## The Wordsmith
QuickXmlWrite (json)
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

🧠 1. Start with the Why
People new to property-based testing often think finding a failing case is the end.
Your job: explain that shrinking is the real magic — it takes a messy failure and makes it diagnosable.

“Finding a bug is good.
Shrinking it to the simplest possible bug is what makes it usable.”
--- slide ---
🪜 2. Describe the Two-Stage Process Clearly
QuickAcid shrinks in two main phases — be explicit about that:

Execution shrinking: removes entire steps from the test run

Input shrinking: simplifies individual inputs (numbers, strings, etc.)

You can show a before/after and point out how fewer steps + simpler data lead to better bug reports.
--- slide ---
🧬 3. Mention Shrinkable vs Non-Shrinkable
Highlight that:

Input(...) introduces shrinkable values

Derived(...) and Stashed(...) do not shrink

Shrinking only applies to parts the framework has control over

Bonus: note how users can customize shrinking later.
--- slide ---
🧪 4. Demonstrate with Real Failing Case
Use a compact version of something like:

```csharp
from input in "input".Input(MGen.Int(1, 100))
from act in "act".Act(() => container.Value = input)
from spec in "spec".Spec(() => container.Value != 42)
```
...and show how QuickAcid hones in on input = 42 through shrinking.
--- slide ---
🛑 5. Clarify When Shrinking Stops
Make it clear:

Shrinking stops when no simpler failing case exists

That’s why failing specs need to be deterministic — or else the shrinker gets confused

You can use .Verbose() to watch this process unfold
--- slide ---
✍️ 6. Optional: Add Tips / Footnotes
You can gently mention:

Why flaky specs or side-effecty systems shrink poorly

How .Claim(...), .Where(...), and guards affect shrinking

That shrinking is what makes PBT superior to random fuzzers
--- slide ---
/docs
  ├── intro.md        // what is QuickAcid? (+ account example)
  ├── linq-101.md     // how runners compose
  ├── executions.md   // run/execution/state explained
  ├── combinators.md  // Input, Stashed, Act, etc.
  ├── shrinking.md    // magic sauce
  ├── assays.md       // coverage / summary checks
  ├── touchstones.md  // visibility tools
  ├── custom.md       // building your own combinators or shrinkers
  └── faq.md          // oddities, design rationale

--- slide ---
What is QuickAcid?
QuickAcid is a property-based testing framework for C#.
It’s built around a simple idea: instead of writing examples of what your code should do,
you describe what must always be true, and let the framework try to break it.

It combines:

Declarative test construction via LINQ

Powerful fuzzing and shrinking via QuickMGenerate

Test lifecycle control through composable runners

Human-readable failure reports that shrink failures to their core

You don’t write lots of test cases.
You write invariants — and QuickAcid finds the test cases for you.
--- slide ---
```csharp
  public Report Run(int executionsPerScope, bool throwOnFailure)
    {
        ExecutionNumbers = [.. Enumerable.Repeat(-1, executionsPerScope)];
        for (int j = 0; j < executionsPerScope; j++)
        {
            ExecutionNumbers[CurrentExecutionNumber] = CurrentExecutionNumber;
            Script(this);
            if (CurrentContext.Failed)
            {
                if (Verbose)
                {
                    report.AddEntry(new ReportTitleSectionEntry(["FIRST FAILED RUN"]));
                    report.AddEntry(new ReportRunStartEntry());
                    AddMemoryToReport(report, false);
                }
                HandleFailure();
                return;
            }
            CurrentExecutionNumber++;
            if (CurrentContext.Failed)
            {
                if (throwOnFailure)
                    throw new FalsifiableException(report.ToString(), Exception!) { QAcidReport = report };
                else
                    return report;
            }
            if (CurrentContext.BreakRun)
            {
                break;
            }
            if (AlwaysReport)
            {
                AddMemoryToReport(report, true);
                return report;
            }
        }
        return null!;
    }
```
--- slide ---
```csharp
    public void ShrinkExecutions()
    {
        var max = ExecutionNumbers.Max();
        var current = 0;
        while (current <= max && ExecutionNumbers.Count() > 1)
        {
            using (EnterPhase(QAcidPhase.ShrinkingExecutions))
            {
                Memory.ResetRunScopedInputs();
                foreach (var run in ExecutionNumbers.ToList())
                {
                    CurrentExecutionNumber = run;
                    if (run != current)
                        Script(this);
                    if (CurrentContext.BreakRun)
                        break;
                }
                if (CurrentContext.Failed && !CurrentContext.BreakRun)
                {
                    ExecutionNumbers.Remove(current);
                }
                current++;
                shrinkCount++;
            }
        }
    }
```
--- slide ---
```csharp
    private void ShrinkInputs()
    {
        using (EnterPhase(QAcidPhase.ShrinkingInputs))
        {
            Memory.ResetRunScopedInputs();
            foreach (var executionNumber in ExecutionNumbers.ToList())
            {
                CurrentExecutionNumber = executionNumber;
                Script(this);
                shrinkCount++;
            }
        }
    }
```