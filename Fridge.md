## Refactor
PhaseContext => Copy

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

---
| Feature                             | Description                                            | Effort  |
| ----------------------------------- | ------------------------------------------------------ | ------- |
| 💬 Per-permutation result logging   | Show all result variants side by side                  | Low     |
| 🧠 Spec name interpolation          | Include key values in spec names/messages              | Low     |
| 📄 Structured reporting             | JSON or markdown export of results                     | Medium  |
| 📊 Aggregated results               | Track fail frequency per property/spec                 | Medium  |
| 🧬 Pre/post shrink comparison       | Store and show both pre- and post-shrink values        | Medium  |
| 🧑‍🏫 Teaching mode / verbose trace | Adds explanation-style output for educational purposes | Medium+ |


Focused Shrinking Feedback
Shrink Summary:
- 7 inputs removed
- Minimal case: amount = 3, coins = [2,3]
- Triggered spec: "reversed coins should not change result"


# Shrinking Strats

Flow-Based Shrinking (QuickPulse Style)
var flow =
    from value in Pulse.Start<int>()
    from box in Pulse.Gather(value)
    from _ in Pulse.Effect(() => box.Value--)
    from _ in Pulse.Trace(box.Value)
    select box.Value;
