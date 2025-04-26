using QuickAcid.Bolts;
using QuickAcid.Bolts.Nuts;

namespace QuickAcid.Tests.Linqy.Spec
{
    public class SpecExceptionTests
    {
        [Fact]
        public void ExceptionThrownBySpecIsNotAQuickAcidException()
        {
            var ex =
                Assert.Throws<Exception>(() => (
                    from spec in "spec".Spec(Throw)
                    select Acid.Test
                ).Verify(1, 1));
            Assert.IsNotType<FalsifiableException>(ex);
            Assert.Contains("QuickAcid.Tests.Linqy.Spec.SpecExceptionTests.Throw()", ex.StackTrace);
        }

        private bool Throw()
        {
            throw new Exception();
        }
    }
}