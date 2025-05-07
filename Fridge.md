## FeedBack Shrinking 
Check always reported input on Start run  
(Only Missing On QDiagniosticState ?)
## QAcidState.GetPulse
Move To QAcid.GetPulse 
(Maybe old QAcid => QAcidCombinators)
## Check Project Migration
QuickAcid.Examples.SetTest
## AutoDoc  
test stashed
--- slide ---
## JS Testing
With Jint ? 
--- slide ---
## Code Gen  
lots to do here
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