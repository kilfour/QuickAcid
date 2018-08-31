using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuickMGenerate;
using QuickMGenerate.UnderTheHood;
using Xunit;

namespace QuickAcid.Tests
{
    public class ActExceptionTests
    {
        [Fact]
        public void SimpleExceptionThrown()
        {
            var test =
                from foo in "foo".Act(() => throw new Exception())
                select Unit.Instance;
            var state = new QAcidState(test);
            state.Run(1);
        }

        [Fact]
        public void TwoActionsExceptionThrown()
        {
            var test =
                from foo in "foo".Act(() => { })
                from bar in "bar".Act(() => throw new Exception())
                select Unit.Instance;
            var state = new QAcidState(test);
            state.Run(1);
        }
    }
}
