﻿using QuickAcid.Reporting;
using QuickMGenerate;
using QuickAcid.Bolts.Nuts;
using QuickAcid.Bolts;
using QuickAcid.Bolts.Nuts.QuickMGenerateExtensions;

namespace QuickAcid.Tests.Shrinking.Primitives
{
    public class CharTests
    {
        [Fact]
        public void OneRelevantChar()
        {
            var run =
                from input1 in "input1".ShrinkableInput(MGen.ChooseFrom(['X', 'Y']))
                from input2 in "input2".ShrinkableInput(MGen.Char())
                from foo in "act".Act(() =>
                {
                    if (input1 == 'X') throw new Exception("Boom");
                })
                select Acid.Test;

            var report = run.ReportIfFailed(1, 50);

            var inputEntry = report.FirstOrDefault<ReportInputEntry>();
            Assert.NotNull(inputEntry);
            Assert.Equal("input1", inputEntry.Key);
            Assert.Equal("X", inputEntry.Value); // Could use .ToString(CultureInfo.InvariantCulture) if needed

            var actEntry = report.FirstOrDefault<ReportActEntry>();
            Assert.NotNull(actEntry);
            Assert.Equal("act", actEntry.Key);
            Assert.NotNull(actEntry.Exception);
        }
    }
}