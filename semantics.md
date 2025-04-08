| Concept           | What it *looks* like to the user      | What it *is* in the engine           | Suggested name             |
|-------------------|----------------------------------------|--------------------------------------|----------------------------|
| `.Do(...)`        | One test step                          | A LINQ-side-effect inside a chain    | `step`                     |
| `.Act(...)`       | Raw LINQ side effect                   | One part of a runner                 | (still `step`, just hidden)|
| `.Choose(...)`    | Picks from multiple options            | Generates multiple runners           | `runnerChoice`             |
| `QAcidRunner<T>`  | A composed test definition             | A full test scenario (to the engine) | `testPlan`, `path`, `actionUnit` |
| `.PickOne()`      | Run one of the options                 | One random choice per test run       | `action run`               |
| One execution     | A full run of the testPlan             | `QAcidState.Run(...)` n times        | `test run`, `execution`    |