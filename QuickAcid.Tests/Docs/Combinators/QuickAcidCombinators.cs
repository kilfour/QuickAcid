using QuickPulse.Explains;

namespace QuickAcid.TestsDeposition.Docs.Combinators;

[DocFile]
[DocContent(
@"Combinators are the heart of QuickAcid's LINQ-based property testing DSL.  
Each one introduces a **step** in the test pipeline — a value, an action, a condition, or a check — and together they form the skeleton of your test logic.

Most combinators follow a simple pattern: 
they attach behavior to a named step (via `""name"".Combinator(...)`) and compose naturally using LINQ syntax.

While each combinator has its own lifecycle and semantics, they all share one goal:  
to **express your intent declaratively** and let the QuickAcid engine take care of execution, shrinking, and reporting.

The sections below describe these core building blocks.

")]
public class QuickAcidCombinators { }