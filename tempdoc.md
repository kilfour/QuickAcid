## QuickAcid Linq 101

### What is a Runner?

Runners are the core abstraction of QuickAcid's LINQ model. 
They carry both the logic of the test and the mechanisms to generate input, track state, and produce a result.

A runner is, more precisely, a function that takes a `QAcidState` and returns a `QAcidResult<T>`.
It encapsulates both what to do and how to do it, with full access to the current test state.
```csharp
public delegate QAcidResult<T> QAcidRunner<T>(QAcidState state);
```


You can think of runners as the building blocks of a property-based test.


Each LINQ combinator constructs a new runner by composing existing ones. 
The final test, the full LINQ query, is just a single, composed runner.


#### `Acid.Test`


In QuickAcid, every test definition ends with:
```csharp
select Acid.Test;
```
`Acid.Test` is a unit value. It represents the fact that there is no meaningful return value to capture,
and serves as a **terminator** for the test chain.
*All well-formed tests should end with it.*

