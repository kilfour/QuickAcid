// using QuickAcid.Reporting;
// using QuickMGenerate;
// using QuickAcid.Bolts.Nuts;
// using QuickAcid.Bolts;

// namespace QuickAcid.Tests
// {
//     public class LocalVarTests
//     {
//         [Fact]
//         public void LocalVarIsNotReported()
//         {
//             var run =
//                 from input in "input".Input(MGen.Int())
//                 from localVar in "local".LocalVar(() => 1)
//                 from foo in "foo".Act(() => { })
//                 from spec in "spec".Spec(() => input + 1 == input)
//                 select Acid.Test;

//             var report = run.ReportIfFailed();

//             var inputEntry = report.FirstOrDefault<ReportInputEntry>();
//             Assert.NotNull(inputEntry);
//             Assert.Equal("input", inputEntry.Key);

//             var actEntry = report.FirstOrDefault<ReportActEntry>();
//             Assert.NotNull(actEntry);
//             Assert.Equal("foo", actEntry.Key);

//             var specEntry = report.FirstOrDefault<ReportSpecEntry>();
//             Assert.NotNull(specEntry);
//             Assert.Equal("spec", specEntry.Key);
//         }
//     }
// }