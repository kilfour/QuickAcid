// using QuickAcid.Bolts;
// using QuickAcid.Bolts.Nuts;
// using QuickAcid.TestsDeposition._Tools;
// using QuickMGenerate;

// namespace QuickAcid.TestsDeposition.Linqy.Basics
// {
//     [Doc(Order = "1-1", Caption = "QuickAcid Linq 101")]
//     public class QuickAcidLinq101s
//     {
//         [Fact]
//         [Doc(Order = "1-1-1", Caption = "What is a Runner?", Content =
// @"Runners are the core abstraction of QuickAcid's LINQ model. 
// They carry both the logic of the test and the mechanisms to generate input, track state, and produce a result.

// A runner is, more precisely, a function that takes a `QAcidState` and returns a `QAcidResult<T>`.
// It encapsulates both what to do and how to do it, with full access to the current test state.
// ```csharp
// public delegate QAcidResult<T> QAcidRunner<T>(QAcidState state);
// ```
// You can think of runners as the building blocks of a property-based test. 
// Each LINQ combinator constructs a new runner by composing existing ones. 
// The final test — the full LINQ query — is just a single, composed runner.
// ")]
//         public void What_is_a_runner()
//         {
//             Assert.IsType<QAcidRunner<int>>("input".Shrinkable(MGen.Int()));

//             Assert.IsType<QAcidRunner<int>>(
//                 "input".Shrinkable(MGen.Int()));
//         }
//     }
// }